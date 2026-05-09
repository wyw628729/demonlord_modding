using UnityEngine;

namespace ChargeCannon
{
    /// <summary>
    /// 蓄能炮武器生命周期钩子
    ///
    /// 主要职责：
    ///   1. OnSetSprite     —— 根据技能状态返回对应精灵 key
    ///   2. OnTrySkipButton —— E 键蓄力（转发给技能实例处理）
    ///   3. OnDodgeOrParry  —— 弹反/闪避蓄力
    ///
    /// 技能实例通过 SetSkill() 注入，在 Skill 初始化时调用。
    /// </summary>
    public class ChargeCannonHooks : IWeaponHooks
    {
        private const int WeaponId = 1320;

        // 技能实例引用，由 Skill_Weapon_ChargeCannon.OnInit() 设置
        private Skill_Weapon_ChargeCannon _skill;

        public void SetSkill(Skill_Weapon_ChargeCannon skill)
        {
            _skill = skill;
        }

        public void OnEquip(int playerIndex)
        {
            Debug.Log($"[ChargeCannon] 玩家 {playerIndex} 装备了蓄能炮");
        }

        public void OnUnequip(int playerIndex)
        {
            _skill = null;
            Debug.Log($"[ChargeCannon] 玩家 {playerIndex} 卸下了蓄能炮");
        }

        /// <summary>
        /// 精灵切换钩子：返回对应状态下的精灵 key，null 表示不干预
        /// </summary>
        public string OnSetSprite(string state, int weaponId)
        {
            if (weaponId != WeaponId || _skill == null) return null;

            switch (_skill.CurrentMode)
            {
                case Skill_Weapon_ChargeCannon.CannonMode.Charging:
                    return "charging1" + WeaponId;   // → "charging11320"
                case Skill_Weapon_ChargeCannon.CannonMode.Firing:
                    return "firing" + WeaponId;       // → "firing1320"
                default:
                    return null; // Idle 时不干预
            }
        }

        public int OnAttackOnUnit(UnitObject target, int damage, int distance, int weaponId)
        {
            return damage;
        }

        /// <summary>
        /// E 键拦截：蓄力状态下 E 键继续蓄力，不使用原版跳过逻辑
        /// </summary>
        public bool OnTrySkipButton(int weaponId)
        {
            if (weaponId != WeaponId || _skill == null) return false;

            if (_skill.CurrentMode == Skill_Weapon_ChargeCannon.CannonMode.Charging)
            {
                _skill.OnEKeyPressed();
                return true; // 已处理，阻止原版 E 键行为
            }

            return false;
        }

        public void OnTakeDamage(UnitObject atkUnit, int weaponId)
        {
            // 蓄力时自带减伤（isCharging=true 时游戏内置 30% 伤害减免）
        }

        /// <summary>
        /// 弹反/闪避蓄力1层（可累积至满蓄）
        /// </summary>
        public void OnDodgeOrParry(bool isParry, int weaponId)
        {
            if (weaponId != WeaponId || _skill == null) return;
            _skill.OnDodgeOrParryCharge();
        }
    }
}
