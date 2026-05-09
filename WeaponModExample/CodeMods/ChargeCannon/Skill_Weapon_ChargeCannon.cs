using System.Collections.Generic;
using UnityEngine;

namespace ChargeCannon
{
    /// <summary>
    /// 蓄能炮武器技能
    ///
    /// 状态机：Idle → Charging（蓄力中）→ Firing（开火中）→ Idle
    ///
    /// 核心机制：
    ///   p.isCharging = true  ——  在 PlayerInputCheck 里跳过门/箱检测，直接把方向键输入派发给
    ///                             OnInputDir（即本 Skill 的 Execute），让技能完全接管每一格输入。
    ///   p.PlayerActionOver()  ——  结束回合但不算攻击（蓄力/开火维持状态时用）
    ///   p.PlayerAttackOver()  ——  结束回合并算作一次攻击（开火打出伤害后用）
    /// </summary>
    public class Skill_Weapon_ChargeCannon : Skill_Weapon
    {
        // ── 状态机（公开枚举供 Hooks 访问）──────────────────────────
        public enum CannonMode { Idle, Charging, Firing }

        private CannonMode mode = CannonMode.Idle;

        /// <summary>供 ChargeCannonHooks 读取当前模式，用于精灵切换</summary>
        public CannonMode CurrentMode => mode;

        // 当前蓄能层数
        private int chargeCount = 0;

        // 开火时锁定的瞄准方向（侧方位移时保持方向不变）
        private Vector2Int fireAimDir;

        // 开火剩余段数
        private int firingRoundsLeft = 0;

        // ── 内部缓存，由 WeaponModAPI.RegisterWeapon 设置 ─────────
        private const int WeaponId = 1320;
        private const int MeleeRadius = 2;    // 自动近战检测半径
        private const int ChargeStartDist = 3; // 距敌几格开始蓄力

        // ── 公开入口：外部（ChargeCannonHooks）通知 E 键 ────────────
        public void OnEKeyPressed()
        {
            if (p == null) return;
            if (mode == CannonMode.Charging)
                DoCharge(p.aimDir);        // E 键当作"向前蓄力"
        }

        /// <summary>供 ChargeCannonHooks 在弹反/闪避时调用，触发蓄力1层</summary>
        public void OnDodgeOrParryCharge()
        {
            if (p == null) return;
            if (mode == CannonMode.Idle) EnterCharging();
            if (mode == CannonMode.Charging) DoCharge(p.aimDir);
            // Firing 状态不响应
        }

        // 初始化时向钩子注入自身引用，使 Hooks 可以访问技能状态
        public override void Init(UnitObjectAbility unit, SkillData data, SkillConfig config, bool showAbilityUI)
        {
            base.Init(unit, data, config, showAbilityUI);
            var profile = WeaponProfileRegistry.Get(WeaponId);
            if (profile?.hooks is ChargeCannonHooks hooks)
                hooks.SetSkill(this);
        }

        // ── Skill.Execute 入口 ────────────────────────────────────
        // 当 isCharging=true 时，每次玩家按方向键 → OnInputDir → Execute
        // 当 isCharging=false（Idle）时，由系统在"玩家按攻击键或移动键相邻敌人"时调用
        public override void Execute(UnitObject unitObject)
        {
            base.Execute(unitObject);
            // base.Execute 会把 p / dir 设置好（来自 Skill_Weapon 基类）
            // p  = unitObject as UnitObjectPlayer
            // dir = unitObject.aimDir

            if (p == null) return;

            // ── 优先检测近战：半径2格内有敌人时放弃蓄能炮，进入近战 ──
            if (HasNearEnemy(MeleeRadius))
            {
                SwitchToMelee();
                return;
            }

            switch (mode)
            {
                case CannonMode.Idle:    HandleIdle();    break;
                case CannonMode.Charging: HandleCharging(); break;
                case CannonMode.Firing:   HandleFiring();   break;
            }
        }

        // ─────────────────────────────────────────────────────────
        // Idle 模式：判断是否应该开始蓄力
        // ─────────────────────────────────────────────────────────
        private void HandleIdle()
        {
            var enemy = BattleObject.Instance.GetNearestEnemy(p.unitPos, p.unitCamp);
            if (enemy == null) { NormalMove(); return; }

            int dist = ManhattanDist(p.unitPos, enemy.unitPos);
            if (dist <= ChargeStartDist && dir == p.aimDir)
            {
                // 距敌≤3格且按的是前进方向 → 开始蓄力
                EnterCharging();
                DoCharge(dir);
            }
            else
            {
                NormalMove();
            }
        }

        // ─────────────────────────────────────────────────────────
        // Charging 模式
        // ─────────────────────────────────────────────────────────
        private void HandleCharging()
        {
            int chargeMax = GetChargeMax();

            // 反方向键 → 取消蓄力并后退
            if (dir == -p.aimDir)
            {
                CancelCharge();
                return;
            }

            // 侧向键（非前进也非后退）
            if (dir != p.aimDir && dir != -p.aimDir)
            {
                // 差一层即满蓄时：转向并直接开火
                if (chargeCount == chargeMax - 1)
                {
                    TurnTo(dir);
                    chargeCount = chargeMax; // 转向同时完成最后一层
                    TriggerFire();
                }
                else
                {
                    // 普通侧向：转向 + 蓄力1层
                    TurnTo(dir);
                    DoCharge(dir);
                }
                return;
            }

            // 前进方向键（或 E 键走到这里）→ 蓄力1层
            DoCharge(dir);
        }

        // ─────────────────────────────────────────────────────────
        // Firing 模式
        // ─────────────────────────────────────────────────────────
        private void HandleFiring()
        {
            // 反方向键 → 取消剩余激光并后退
            if (dir == -fireAimDir)
            {
                CancelFiring();
                return;
            }

            // 侧向键：保持瞄准方向不变，位移后继续开火
            if (dir != fireAimDir)
            {
                SideMove(dir);
                return;
            }

            // 前进方向键 → 开火一段
            ShootOnce(fireAimDir);
        }

        // ─────────────────────────────────────────────────────────
        // 蓄力操作
        // ─────────────────────────────────────────────────────────
        private void EnterCharging()
        {
            mode = CannonMode.Charging;
            p.isCharging = true;
        }

        private void DoCharge(Vector2Int chargeDir)
        {
            chargeCount++;
            int chargeMax = GetChargeMax();

            p.SetSprite("charging1" + WeaponId);
            p.listenDelay += 0.25f;

            if (chargeCount >= chargeMax)
            {
                // 满蓄 → 直接开火
                TriggerFire();
            }
            else
            {
                p.PlayerActionOver(); // 蓄力中：结束回合但保持 isCharging
            }
        }

        // ─────────────────────────────────────────────────────────
        // 开火触发
        // ─────────────────────────────────────────────────────────
        private void TriggerFire()
        {
            mode = CannonMode.Firing;
            p.isCharging = true;          // 开火状态也保持接管输入
            fireAimDir = p.aimDir;        // 锁定开火方向
            firingRoundsLeft = 2;         // 共2段激光

            ShootOnce(fireAimDir);
        }

        // ─────────────────────────────────────────────────────────
        // 发射一段激光
        // ─────────────────────────────────────────────────────────
        private void ShootOnce(Vector2Int shootDir)
        {
            var profile = WeaponProfileRegistry.Get(WeaponId);
            if (profile == null) { EndFiring(); return; }

            var store = BattleObject.Instance.weaponParams;
            var tiles = AtkRangeResolver.Resolve(
                profile.primaryAtkRange,
                store,
                p.unitPos,
                shootDir
            );

            p.SetSprite("firing" + WeaponId);

            // 伤害 = 玩家攻击力（已包含能力加成）× 蓄能层数系数（满蓄=chargeMax）
            int dmg = p.unitAtk * GetChargeMax();
            p.AddDamageRange(tiles, out _, dmg);
            p.AtkFinish("firing" + WeaponId);

            firingRoundsLeft--;
            if (firingRoundsLeft <= 0)
            {
                EndFiring();
                p.PlayerAttackOver();
            }
            else
            {
                // 还有剩余段数，结束本回合但保持 Firing 状态
                p.PlayerActionOver();
            }
        }

        // ─────────────────────────────────────────────────────────
        // 侧方位移（Firing 状态）
        // ─────────────────────────────────────────────────────────
        private void SideMove(Vector2Int moveDir)
        {
            if (!p.PlayerNeedMove(moveDir, 1))
            {
                // 无法位移，直接开火
                ShootOnce(fireAimDir);
                return;
            }

            // 位移 + 回调中恢复瞄准方向 + 发射
            var savedAimDir = fireAimDir;
            p.MoveToPos(moveDir, 1, 1f, default, "move", "no", 0, () =>
            {
                // 位移完成后恢复开炮方向
                p.aimDir = savedAimDir;
                p.ChangeSpriteRotate(savedAimDir.x < 0);
                ShootOnce(savedAimDir);
            });
        }

        // ─────────────────────────────────────────────────────────
        // 转向（蓄力时侧向按键）
        // ─────────────────────────────────────────────────────────
        private void TurnTo(Vector2Int newDir)
        {
            p.aimDir = newDir;
            p.ChangeSpriteRotate(newDir.x < 0);
        }

        // ─────────────────────────────────────────────────────────
        // 取消蓄力
        // ─────────────────────────────────────────────────────────
        private void CancelCharge()
        {
            chargeCount = 0;
            mode = CannonMode.Idle;
            p.isCharging = false;
            p.SetSprite("default");
            // 后退一格（如果可以）
            if (p.PlayerNeedMove(-p.aimDir, 1))
                p.PlayerMove(-p.aimDir, 1, () => p.PlayerActionOver());
            else
                p.PlayerActionOver();
        }

        // ─────────────────────────────────────────────────────────
        // 取消开火
        // ─────────────────────────────────────────────────────────
        private void CancelFiring()
        {
            EndFiring();
            // 后退一格（如果可以）
            if (p.PlayerNeedMove(-fireAimDir, 1))
                p.PlayerMove(-fireAimDir, 1, () => p.PlayerActionOver());
            else
                p.PlayerActionOver();
        }

        // ─────────────────────────────────────────────────────────
        // 结束开火状态（重置）
        // ─────────────────────────────────────────────────────────
        private void EndFiring()
        {
            chargeCount = 0;
            firingRoundsLeft = 0;
            mode = CannonMode.Idle;
            p.isCharging = false;
        }

        // ─────────────────────────────────────────────────────────
        // 自动近战切换（放弃蓄能炮，用近战攻击）
        // ─────────────────────────────────────────────────────────
        private void SwitchToMelee()
        {
            // 中断任何蓄力/开火
            bool wasCharging = (mode != CannonMode.Idle);
            chargeCount = 0;
            firingRoundsLeft = 0;
            mode = CannonMode.Idle;
            p.isCharging = false;

            // 用近战攻击范围造成伤害
            var profile = WeaponProfileRegistry.Get(WeaponId);
            if (profile?.meleeAtkRange != null)
            {
                var store = BattleObject.Instance.weaponParams;
                var tiles = AtkRangeResolver.Resolve(
                    profile.meleeAtkRange,
                    store,
                    p.unitPos,
                    dir
                );
                p.AddDamageRange(tiles, out _, p.unitAtk);
                p.AtkFinish("default");
                p.PlayerAttackOver();
            }
            else
            {
                // 没有近战配置，普通移动
                NormalMove();
            }
        }

        // ─────────────────────────────────────────────────────────
        // 普通移动（Idle 时不在蓄力距离内）
        // ─────────────────────────────────────────────────────────
        private void NormalMove()
        {
            p.PlayerMove(dir, 1, () => p.PlayerMoveOver());
        }

        // ─────────────────────────────────────────────────────────
        // 辅助方法
        // ─────────────────────────────────────────────────────────
        private int GetChargeMax()
        {
            return BattleObject.Instance.weaponParams.GetInt("chargeMax", 3);
        }

        private bool HasNearEnemy(int radius)
        {
            var bo = BattleObject.Instance;
            foreach (var enemy in bo.enemyObjects)
            {
                if (enemy == null || enemy.hasDead) continue;
                if (ManhattanDist(p.unitPos, enemy.unitPos) <= radius)
                    return true;
            }
            return false;
        }

        private static int ManhattanDist(Vector2Int a, Vector2Int b)
        {
            return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
        }
    }
}
