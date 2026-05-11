using UnityEngine;

namespace ChargeCannon
{
    /// <summary>
    /// 蓄能炮武器生命周期钩子
    ///
    /// 实现 IWeaponHooks 接口，由 WeaponModAPI 在注册武器时传入。
    /// 游戏引擎在特定时机（精灵切换、按键拦截、弹反/闪避等）回调对应方法。
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
        /// <summary>蓄能炮的武器 ID，与注册时一致</summary>
        private const int WeaponId = 1320;

        /// <summary>技能实例引用，由 Skill_Weapon_ChargeCannon.Init() 通过 SetSkill() 设置</summary>
        private Skill_Weapon_ChargeCannon _skill;

        /// <summary>
        /// 注入技能实例引用。在 Skill_Weapon_ChargeCannon.Init() 中调用，
        /// 使 Hooks 可以访问技能的当前状态（CannonMode）并调用技能方法。
        /// </summary>
        /// <param name="skill">蓄能炮技能实例</param>
        public void SetSkill(Skill_Weapon_ChargeCannon skill)
        {
            _skill = skill;
        }

        /// <summary>
        /// 武器装备回调 —— 玩家装备该武器时触发
        /// </summary>
        /// <param name="playerIndex">装备武器的玩家索引</param>
        public void OnEquip(int playerIndex)
        {
            Debug.Log($"[ChargeCannon] 玩家 {playerIndex} 装备了蓄能炮");
        }

        /// <summary>
        /// 武器卸下回调 —— 玩家卸下该武器时触发。
        /// 清除技能引用以避免悬空引用。
        /// </summary>
        /// <param name="playerIndex">卸下武器的玩家索引</param>
        public void OnUnequip(int playerIndex)
        {
            _skill = null;
            Debug.Log($"[ChargeCannon] 玩家 {playerIndex} 卸下了蓄能炮");
        }

        /// <summary>
        /// 精灵切换钩子 —— 当游戏引擎尝试设置玩家精灵时回调。
        /// 根据技能当前状态（Charging / Firing）返回对应的精灵 key，
        /// 返回 null 表示不干预，由引擎使用默认精灵。
        /// </summary>
        /// <param name="state">引擎请求的精灵状态（如 "default"、"move"、"sleep"）</param>
        /// <param name="weaponId">当前武器 ID</param>
        /// <returns>精灵 key 字符串，或 null 表示不干预</returns>
        public string OnSetSprite(string state, int weaponId)
        {
            // 非蓄能炮武器或不在此技能生命周期内 → 不干预
            if (weaponId != WeaponId || _skill == null)
            {
                return null;
            }

            // 通用状态（默认、移动、睡眠）→ 使用蓄能炮默认精灵
            if (state == "default" || state == "move" || state == "sleep")
            {
                return "default1320";
            }

            // 根据技能当前模式返回对应精灵
            switch (_skill.CurrentMode)
            {
                case Skill_Weapon_ChargeCannon.CannonMode.Charging:
                    return "charging1" + WeaponId;   // 蓄力中精灵 → "charging1320"
                case Skill_Weapon_ChargeCannon.CannonMode.Firing:
                    return "firing" + WeaponId;       // 开火中精灵 → "firing1320"
                default:
                    return null; // Idle 时不干预，使用引擎默认行为
            }
        }

        /// <summary>
        /// 攻击命中单位回调 —— 可修改伤害值。
        /// 当前未做特殊处理，直接返回原始伤害。
        /// </summary>
        /// <param name="target">被攻击的目标单位</param>
        /// <param name="damage">原始伤害值</param>
        /// <param name="distance">攻击距离（格）</param>
        /// <param name="weaponId">武器 ID</param>
        /// <returns>修改后的伤害值</returns>
        public int OnAttackOnUnit(UnitObject target, int damage, int distance, int weaponId)
        {
            return damage;
        }

        /// <summary>
        /// E 键（跳过/等待按钮）拦截钩子。
        /// 蓄力状态下 E 键继续蓄力而非执行原版跳过行为。
        /// </summary>
        /// <param name="weaponId">当前武器 ID</param>
        /// <returns>true 表示已处理该按键，阻止原版 E 键行为；false 表示不干预</returns>
        public bool OnTrySkipButton(int weaponId)
        {
            // 非蓄能炮或技能未就绪 → 不拦截
            if (weaponId != WeaponId || _skill == null) return false;

            // 蓄力中：E 键视为"向前蓄力"操作
            if (_skill.CurrentMode == Skill_Weapon_ChargeCannon.CannonMode.Charging)
            {
                _skill.OnEKeyPressed();
                return true; // 已处理，阻止原版 E 键行为
            }

            return false;
        }

        /// <summary>
        /// 受伤回调 —— 可用于实现受伤时的特殊效果。
        /// 蓄力状态下 isCharging=true，游戏引擎已内置 30% 伤害减免，此处无需额外处理。
        /// </summary>
        /// <param name="atkUnit">攻击者单位</param>
        /// <param name="weaponId">武器 ID</param>
        public void OnTakeDamage(UnitObject atkUnit, int weaponId)
        {
            // 蓄力时自带减伤（isCharging=true 时游戏内置 30% 伤害减免）
        }

        /// <summary>
        /// 弹反/闪避回调 —— 玩家成功弹反或闪避时触发。
        /// 每次弹反/闪避为蓄能炮蓄力1层，可累积至满蓄。
        /// </summary>
        /// <param name="isParry">true 表示弹反，false 表示闪避</param>
        /// <param name="weaponId">当前武器 ID</param>
        public void OnDodgeOrParry(bool isParry, int weaponId)
        {
            if (weaponId != WeaponId || _skill == null) return;
            _skill.OnDodgeOrParryCharge();
        }
    }
}
