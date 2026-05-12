using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace HuntingArena
{
    /// <summary>
    /// 大连续狩猎 Mod 入口
    /// 在家园生成一个传送门，玩家踏入后进入仅含大连续狩猎竞技场的特殊章节。
    /// </summary>
    public class Main : SimpleModBehaviour
    {
        private bool _huntingQueued = false;
        private bool _portalActive  = false;

        public override void OnModLoaded()
        {
            Log("大连续狩猎 Mod 加载完成！");

            RoomModAPI.RegisterRoomType(
                key:        "hunting_arena.main",
                factory:    () => new HuntingArenaRoom(),
                sprite:     null,
                spriteName: "大连续狩猎",
                bgmKey:     null
            );

            // 为 1~4 章分别注册布局，无论玩家在哪章进入都能找到房间配置
            string csvPath = System.IO.Path.Combine(ModFolder, "../../RoomLayouts", "hunting_arena.csv");
            for (int ch = 1; ch <= 4; ch++)
            {
                RoomModAPI.RegisterRoomLayout(
                    roomID:        10100 + ch,
                    csvAbsPath:    csvPath,
                    chapterType:   ch,
                    customTypeKey: "hunting_arena.main",
                    playerSpawnX:  9,
                    playerSpawnY:  9
                );
            }

            BattleObject.OnAfterHomeDataLoad += OnAfterHomeDataLoad;
            BattleObject.OnGameStart         += OnGameStart;
        }

        public override void OnModUnloaded()
        {
            BattleObject.OnAfterHomeDataLoad -= OnAfterHomeDataLoad;
            BattleObject.OnGameStart         -= OnGameStart;
            Log("大连续狩猎 Mod 已卸载。");
        }

        // 家园数据加载完成 → 清理旧传送门回调，协程等待家园房间就绪后生成新传送门
        private void OnAfterHomeDataLoad(BattleObject bo)
        {
            _huntingQueued = false;
            _portalActive  = false;
            RoomModAPI.ClearModUnitCallbacks();
            StartCoroutine(SpawnPortalCo());
        }

        private IEnumerator SpawnPortalCo()
        {
            for (int i = 0; i < 40; i++)
            {
                var bo = BattleObject.Instance;
                if (bo != null
                    && !bo.isInBattle
                    && bo.playerObject != null
                    && RoomManager.Instance != null
                    && RoomManager.Instance.currentRoom is RoomHome)
                {
                    TrySpawnPortal(bo);
                    yield break;
                }
                yield return new WaitForSecondsRealtime(0.25f);
            }
        }

        private void TrySpawnPortal(BattleObject bo)
        {
            var lattice = LatticeObject.Instance;
            if (lattice == null) return;

            var homeConfig = ConfigManager.allRoomConfigs?.Find(c => c != null && c.roomType == RoomType.Home);
            Vector2Int basePos = homeConfig != null
                ? new Vector2Int(homeConfig.playerSpawnPos.x + lattice.offsetX,
                                 homeConfig.playerSpawnPos.y + lattice.offsetY)
                : bo.playerObject.unitPos;

            // 传送门生成在 +3 格，玩家攻击 2 次后触发竞技场
            Vector2Int portalPos = basePos + new Vector2Int(3, 0);
            if (lattice.CheckPos(portalPos))
            {
                _portalActive = true;
                RoomModAPI.SpawnModUnit(965, portalPos, onDead: (b) =>
                {
                    if (!_portalActive) return;
                    _portalActive  = false;
                    _huntingQueued = true;
                    Log("玩家攻击传送门，开始大连续狩猎！");
                    b.runHomeLeave();
                });
            }

            GameController.Instance.gameUI.ShowNoticeText("【大连续狩猎】向右走 3 格，攻击传送门！挑战三大 Boss！");
            Log($"竞技场传送门已生成，位置：{portalPos}");
        }

        // 新局开始时（OnGameStart 在 ChangeState(ChapterStart) 之前触发），
        // 若由传送门触发，则在房间序列生成之前就替换为竞技场专属序列。
        // 注意：不能用 OnChapterStart，那时 MapJump 动画已启动，GenerateRoom 即将执行，太晚了。
        private void OnGameStart(BattleObject bo)
        {
            if (!_huntingQueued) return;
            _huntingQueued = false;

            // 先清空，确保 InitChapterRoomTypes 里的 "非空则跳过生成" 分支不会拦截我们的覆盖。
            // OverrideChapterSequence 内部会直接赋值 roomTypesConfig，清空只是防御性处理。
            bo.roomTypesConfig = null;

            RoomModAPI.OverrideChapterSequence(new List<List<RoomTypeEntry>>
            {
                new List<RoomTypeEntry> { RoomTypeEntry.Custom("hunting_arena.main") },
            });

            // 竞技场不属于正常世界地图，跳过地图跳跃动画，直接黑屏转场进入
            RoomModAPI.SkipNextMapJump();

            Log("章节序列已替换为大连续狩猎！");
        }
    }

    /// <summary>
    /// 竞技场房间：从 Boss 池中随机抽取 3 个依次登场。
    /// 第 1 回合刷第 1 个，第 21 回合刷第 2 个，第 41 回合刷第 3 个。
    /// 全部击败后 BattleRoomCheck 检测到 enemyObjects 为空且无后续波次，自动开门通关。
    /// </summary>
    public class HuntingArenaRoom : RoomBattle
    {
        // Boss 池：按难度分为三档，每档各自不重复地抽一个
        // 档位 0（热身）— 章节 1~2 的 Boss，相对较弱
        private static readonly int[] PoolTier0 = { 102, 104, 118, 108, 33 };
        // 档位 1（主菜）— 章节 2~3 强力 Boss
        private static readonly int[] PoolTier1 = { 109, 16, 17, 47, 43, 37, 50, 53, 54 };
        // 档位 2（终结）— 章节 3~4 精英 / 最终 Boss
        private static readonly int[] PoolTier2 = { 107, /* 121, */ 119, 36, 51, 52, /*66,*/ 116 };

        // Boss 名称映射（用于登场播报）
        private static readonly System.Collections.Generic.Dictionary<int, string> BossNames =
            new System.Collections.Generic.Dictionary<int, string>
        {
            { 102, "食人花" },   { 104, "史莱姆大胖" }, { 118, "蜘蛛女王" },
            { 108, "小克之眼" }, { 33,  "火腿骑士" },
            { 109, "鱼维坦" },   { 16,  "小雪球" },     { 17,  "封印石" },
            { 47,  "自动铁球" }, { 43,  "疯狂齿轮" },   { 37,  "千年树精" },
            { 50,  "宝箱大王" }, { 53,  "残暴的龙蝇" }, { 54,  "小电视" },
            { 107, "黑龙女皇" }, { 121, "魔神剑" },     { 119, "魔王之躯" },
            { 36,  "血之女神" }, { 51,  "史莱姆王" },   { 52,  "炸弹哈基米" },
            /*{ 66,  "猪猪存钱罐" },*/ { 116, "恶魔商人" },
        };

        // Boss 登场间隔回合数
        private const int BossInterval = 40;

        // 本场实际抽到的三个 Boss（进入房间时确定）
        private int[] _selectedBosses;

        public HuntingArenaRoom() : base(RoomType.ModCustom) { }

        public override void OnEnterRoom()
        {
            var bo = BattleObject.Instance;

            _selectedBosses = PickBosses(bo);

            // 设置三波 Boss 的生成数据，游戏每回合开始时自动按 turn 生成
            bo.unitSpawnData = new List<WaveSpawnInfo>();
            for (int i = 0; i < _selectedBosses.Length; i++)
            {
                bo.unitSpawnData.Add(new WaveSpawnInfo
                {
                    turn      = 1 + i * BossInterval,
                    unitType  = _selectedBosses[i],
                    unitCount = 1,
                    unitCamp  = UnitCamp.enemy,
                    isBOSS    = false, // 普通敌人阵营以兼容通关检测（enemyObjects）
                });
            }

            string nameList = string.Join(" → ", System.Array.ConvertAll(_selectedBosses, id =>
                BossNames.TryGetValue(id, out var n) ? n : $"#{id}"));
            bo.SpawnBoardWord($"【大连续狩猎】{nameList}\n每隔 {BossInterval} 回合下一个登场，全灭即通关！");
        }

        // 每档随机不重复地各取一个 Boss，使用游戏种子保证对局复现
        private static int[] PickBosses(BattleObject bo)
        {
            int ra = RandomNumberGenerator.GetInt32(1, 100000);
            var rng = new System.Random(ra);//SeedManager.GetRng("hunting_arena");
            return new[]
            {
                PickOne(PoolTier0, rng),
                PickOne(PoolTier1, rng),
                PickOne(PoolTier2, rng),
            };
        }

        private static int PickOne(int[] pool, System.Random rng)
        {
            return pool[rng.Next(pool.Length)];
        }

        public override void OnRoomClear()
        {
            var bo = BattleObject.Instance;
            var pos = bo.playerObject.unitPos;

            // 丰厚奖励：金宝箱 + 大量灵魂 + 稀有能力选择
            bo.SpawnManyUnit(pos, 1, 810);    // 金宝箱
            bo.SpawnManyUnit(pos, 300, 210);  // 300 个灵魂
            bo.SpawnManyUnit(pos, 15, 203);   // 15 个金币
            bo.SpawnOneAbility(pos, 0, 3, 0); // 3 个随机能力供选择

            GameController.Instance.gameUI.ShowNoticeText("狩猎完成！强者之名，永载史册！");

            // 直接触发胜利界面，跳过 ChapterOver → runChapterComplete → ExitGame 链路
            bo.ShowThanks(BattleObject.FinishType.normal);
        }
    }
}