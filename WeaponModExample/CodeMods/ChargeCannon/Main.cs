using System.Collections.Generic;
using UnityEngine;

namespace ChargeCannon
{
    /// <summary>
    /// 蓄能炮 Mod 入口类
    /// </summary>
    public class Main : SimpleModBehaviour
    {
        public override void OnModLoaded()
        {
            Log("蓄能炮武器 Mod 已加载");

            // 注册武器
            var profile = WeaponModAPI.RegisterWeapon(
                id:           1320,
                displayName:  "蓄能炮",
                skillType:    "Weapon_ChargeCannon",
                skillFactory: () => new Skill_Weapon_ChargeCannon(),
                defaultParams: new Dictionary<string, float>
                {
                    { "fireRange",  4f },    // 激光射程（格）
                    { "chargeMax",  3f },    // 最大蓄能层数
                    { "fireWidth",  1f },    // 激光宽度（格）
                },
                hooks:        new ChargeCannonHooks(),
                spriteKeys:   new[] { "charging11320", "firing1320" },
                unlockHint:   "来自蓄能炮 Mod",
                isUnLocked:   true
            );

            // 主攻击范围：直线穿透激光
            profile.primaryAtkRange = new AtkRangeConfig
            {
                shape        = AtkRangeShape.Line,
                rangeKey     = "fireRange",   // 从 weaponParams 动态读取射程
                widthKey     = "fireWidth",   // 从 weaponParams 动态读取宽度
                rangeDefault = 4,
                widthDefault = 1,
                startOffset  = 1,             // 从玩家前方1格开始
                piercing     = true,          // 穿透所有目标
            };

            // 近战范围：L形 / 直接用十字近战
            profile.meleeAtkRange = new AtkRangeConfig
            {
                shape        = AtkRangeShape.Line,
                rangeDefault = 1,
                startOffset  = 0,
                includeOrigin = false,
                piercing     = false,
            };

            Log($"武器注册完成：ID={profile.id}, 名称={profile.displayName}");
        }

        public override void OnModUnloaded()
        {
            Log("蓄能炮武器 Mod 已卸载");
        }
    }
}
