using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ChargeCannon
{
    /// <summary>
    /// 蓄能炮 Mod 入口类
    ///
    /// 继承 SimpleModBehaviour，由 CodeModRuntime 通过 AddComponent 挂载。
    /// 负责武器注册、解锁状态维护、家园武器架生成与定位。
    /// </summary>
    public class Main : SimpleModBehaviour
    {
        /// <summary>蓄能炮的武器 ID，与 UnitConfigs / SkillConfigs 中的配置对应</summary>
        private const int SummonerWeaponId = 1320;

        /// <summary>家园武器架相对于玩家出生点的偏移量（左2下1格）</summary>
        private static readonly Vector2Int _homeWeaponStandOffset = new Vector2Int(-2, -1);

        /// <summary>
        /// Mod 加载回调 —— 在 CodeModRuntime 加载 DLL 并 AddComponent 后自动调用。
        /// 注册武器、配置攻击范围、订阅家园数据加载事件。
        /// </summary>
        public override void OnModLoaded()
        {
            // 订阅家园数据加载完成事件，用于确保武器已解锁并生成武器架
            BattleObject.OnAfterHomeDataLoad += OnAfterHomeDataLoad;

            Log("蓄能炮武器 Mod 已加载");

            // ── 注册武器 ──────────────────────────────────────────
            // WeaponModAPI.RegisterWeapon 将武器信息写入 WeaponProfileRegistry，
            // 后续 Skill_Weapon_ChargeCannon.Init() 会通过 ID 查找对应的 profile。
            var profile = WeaponModAPI.RegisterWeapon(
                id: 1320,
                displayName: "蓄能炮",
                skillType: "Weapon_ChargeCannon",               // SkillConfigs.csv 中对应的技能类型名
                skillFactory: () => new Skill_Weapon_ChargeCannon(), // 技能实例工厂
                defaultParams: new Dictionary<string, float>
                {
                    { "fireRange", 4f },  // 激光射程（格）
                    { "chargeMax", 3f },  // 最大蓄能层数
                    { "fireWidth", 1f },  // 激光宽度（格）
                },
                hooks: new ChargeCannonHooks(),                 // 生命周期钩子（精灵切换、E键拦截等）
                spriteKeys: new[] { "default1320", "charging1320", "firing1320" }, // 精灵图 key 列表
                unlockHint: "来自蓄能炮 Mod",
                isUnLocked: true
            );

            // ── 主攻击范围：直线穿透激光 ─────────────────────────
            // 射程和宽度从 weaponParams 动态读取，支持运行时修改
            profile.primaryAtkRange = new AtkRangeConfig
            {
                shape = AtkRangeShape.Line,     // 直线形状
                rangeKey = "fireRange",          // 从 weaponParams 读取射程的 key
                widthKey = "fireWidth",          // 从 weaponParams 读取宽度的 key
                rangeDefault = 4,                // 射程默认值（weaponParams 无对应 key 时使用）
                widthDefault = 1,                // 宽度默认值
                startOffset = 1,                 // 从玩家前方1格开始计算范围（避免伤害自身所在格）
                piercing = true,                 // 穿透：激光经过的所有格子都造成伤害
            };

            // ── 近战范围：前方1格 ────────────────────────────────
            // 当敌人距离≤2格时自动切换近战，放弃蓄能炮模式
            profile.meleeAtkRange = new AtkRangeConfig
            {
                shape = AtkRangeShape.Line,
                rangeDefault = 1,                // 近战距离1格
                startOffset = 0,                 // 从自身位置开始
                includeOrigin = false,           // 不包含自身所在格
                piercing = false,                // 不穿透，只命中第一个目标
            };

            Log($"武器注册完成：ID={profile.id}, 名称={profile.displayName}");

            // 重新加载技能配置，使注册的 skillType 生效
            SkillConfigLoader.ReloadConfigs();
        }

        /// <summary>
        /// Mod 卸载回调 —— 取消事件订阅、重载配置以移除注册项
        /// </summary>
        public override void OnModUnloaded()
        {
            BattleObject.OnAfterHomeDataLoad -= OnAfterHomeDataLoad;
            SkillConfigLoader.ReloadConfigs();
            Log("蓄能炮武器 Mod 已卸载");
        }

        /// <summary>
        /// 家园数据加载完成回调 —— 确保武器已解锁并尝试在家园场景生成武器架
        /// </summary>
        /// <param name="bo">BattleObject 单例实例</param>
        private void OnAfterHomeDataLoad(BattleObject bo)
        {
            EnsureWeaponUnlocked(bo);
            StartCoroutine(EnsureHomeWeaponStandCo());
        }

        /// <summary>
        /// 确保蓄能炮武器在武器列表中已解锁。
        /// 如果 weaponDatas 中不存在该武器则添加，已存在则强制标记为已解锁。
        /// </summary>
        /// <param name="bo">BattleObject 单例实例</param>
        private void EnsureWeaponUnlocked(BattleObject bo)
        {
            if (bo == null)
            {
                return;
            }

            // weaponDatas 可能为 null（首次加载），需要初始化
            if (bo.weaponDatas == null)
            {
                bo.weaponDatas = new List<weaponData>();
            }

            // 查找是否已有该武器的数据
            weaponData data = bo.weaponDatas.Find(item => item.weaponID == SummonerWeaponId);
            if (data == null)
            {
                // 首次出现：添加新武器数据并标记为已解锁
                bo.weaponDatas.Add(new weaponData
                {
                    weaponID = SummonerWeaponId,
                    hasUnLock = true,
                    canUnlockInCastle = true
                });
                return;
            }

            // 已存在：确保解锁状态
            data.hasUnLock = true;
            data.canUnlockInCastle = true;
        }

        /// <summary>
        /// 协程：等待家园场景就绪后生成武器架。
        /// 最多等待 40 帧（约10秒），每帧间隔 0.25 秒轮询。
        /// </summary>
        private IEnumerator EnsureHomeWeaponStandCo()
        {
            for (int i = 0; i < 40; i++)
            {
                BattleObject bo = SingletonData<BattleObject>.Instance;

                // 判断家园场景是否就绪：BattleObject 存在、非战斗中、玩家存在、当前房间是家园
                if (bo != null
                    && !bo.isInBattle
                    && bo.playerObject != null
                    && SingletonData<RoomManager>.Instance != null
                    && SingletonData<RoomManager>.Instance.currentRoom is RoomHome)
                {
                    TrySpawnHomeWeaponStand(bo);
                    yield break; // 成功生成，退出协程
                }

                yield return new WaitForSecondsRealtime(0.25f);
            }
            // 超时未就绪，静默退出
        }

        /// <summary>
        /// 尝试在家园场景中生成或移动蓄能炮武器架。
        /// 逻辑：
        ///   1. 计算目标位置（玩家出生点 + 偏移）
        ///   2. 如果目标位置可放置：
        ///      - 已有武器架 → 移动到目标位置
        ///      - 无武器架 → 生成新的
        ///   3. 如果目标位置不可放置且无已有武器架 → 放弃
        /// </summary>
        /// <param name="bo">BattleObject 单例实例</param>
        private void TrySpawnHomeWeaponStand(BattleObject bo)
        {
            if (bo == null || bo.isInBattle || bo.playerObject == null)
            {
                return;
            }

            // 查找场景中是否已存在蓄能炮武器架（UnitCamp.weapon + abilityID 匹配）
            UnitObjectOther? existingStand = bo.Objects
                .OfType<UnitObjectOther>()
                .FirstOrDefault(unit => unit != null && !unit.hasDead && unit.unitCamp == UnitCamp.weapon && unit.abilityID == SummonerWeaponId);

            // 计算武器架目标位置
            Vector2Int desiredPos = GetHomeSpawnPos(bo) + _homeWeaponStandOffset;

            if (CanPlaceUnit(desiredPos, 1, 1))
            {
                // 目标位置可放置
                if (existingStand != null)
                {
                    // 已有武器架，移动到目标位置
                    MoveHomeWeaponStand(existingStand, desiredPos);
                    return;
                }
                // 无已有武器架，继续往下走生成逻辑
            }
            else if (existingStand == null)
            {
                // 目标位置不可放置且无已有武器架，无法生成
                return;
            }

            // 已有武器架但目标位置不可放置 → 不做处理
            if (existingStand != null)
            {
                return;
            }

            // 生成新的武器架单位（UnitID=997 为武器架模板）
            UnitObject stand;
            bo.SpawnUnit(997, UnitCamp.weapon, out stand, bo.playerObject.unitPos, desiredPos);
            UnitObjectOther weaponStand = stand as UnitObjectOther;
            if (weaponStand != null)
            {
                // 将武器架绑定到蓄能炮武器 ID
                weaponStand.SetWeapon(SummonerWeaponId);
            }
        }

        /// <summary>
        /// 获取家园场景中玩家出生点的世界坐标位置。
        /// 优先从房间配置读取，回退到玩家当前位置。
        /// </summary>
        /// <param name="bo">BattleObject 单例实例</param>
        /// <returns>玩家出生点坐标（含格子系统偏移）</returns>
        private Vector2Int GetHomeSpawnPos(BattleObject bo)
        {
            LatticeObject lattice = SingletonData<LatticeObject>.Instance;

            // 从房间配置中查找 Home 类型房间的玩家出生点
            RoomConfig? homeConfig = ConfigManager.allRoomConfigs == null
                ? null
                : ConfigManager.allRoomConfigs.FirstOrDefault(config => config != null && config.roomType == RoomType.Home);

            if (homeConfig != null && lattice != null)
            {
                // 出生点坐标需要加上格子系统偏移量，将配置坐标转换为世界坐标
                return new Vector2Int(homeConfig.playerSpawnPos.x + lattice.offsetX, homeConfig.playerSpawnPos.y + lattice.offsetY);
            }

            // 回退：使用玩家当前位置
            return bo.playerObject.unitPos;
        }

        /// <summary>
        /// 检查指定区域是否可以放置单位。
        /// 逐格检查：格子是否合法（CheckPos）且无其他单位占据。
        /// </summary>
        /// <param name="origin">区域起始坐标</param>
        /// <param name="width">区域宽度（格）</param>
        /// <param name="height">区域高度（格）</param>
        /// <returns>true 表示所有格子均可放置</returns>
        private bool CanPlaceUnit(Vector2Int origin, int width, int height)
        {
            LatticeObject lattice = SingletonData<LatticeObject>.Instance;
            BattleObject bo = SingletonData<BattleObject>.Instance;
            if (lattice == null || bo == null)
            {
                return false;
            }

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Vector2Int cell = origin + new Vector2Int(x, y);
                    // CheckPos: 格子是否在合法范围内
                    // GetUnitOnPos: 格子上是否有已占据且未死亡的单位
                    if (!lattice.CheckPos(cell) || bo.GetUnitOnPos(new List<Vector2Int> { cell }, true).Any(unit => unit != null && unit.isOcuupied && !unit.hasDead))
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        /// <summary>
        /// 将已有的武器架移动到目标位置。
        /// 先释放原格子的占据状态，更新坐标，再占据新格子，最后更新视觉位置。
        /// </summary>
        /// <param name="stand">要移动的武器架单位</param>
        /// <param name="pos">目标位置</param>
        private void MoveHomeWeaponStand(UnitObjectOther stand, Vector2Int pos)
        {
            if (stand == null || stand.unitPos == pos || !CanPlaceUnit(pos, 1, 1))
            {
                return;
            }

            LatticeObject lattice = SingletonData<LatticeObject>.Instance;
            if (lattice == null)
            {
                return;
            }

            // 释放旧格子 → 更新逻辑坐标 → 占据新格子
            stand.SetLatticeFree();
            stand.unitPos = pos;
            stand.SetLatticeOccupied();

            // 同步视觉节点位置
            if (stand.unitNode != null && stand.unitNode.transform != null)
            {
                stand.unitNode.transform.position = lattice.GetWorldPosition(pos) + stand.posOffset;
            }
        }
    }
}
