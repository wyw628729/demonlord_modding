# 自定义武器 Mod 详细参考

## 目录

- [IWeaponHooks 接口](#iweaponhooks-接口)
- [AtkRangeConfig 攻击范围配置](#atkrangeconfig-攻击范围配置)
- [WeaponProfile 注册后可设置的字段](#weaponprofile-注册后可设置的字段)

---

## IWeaponHooks 接口

`IWeaponHooks` 让代码 Mod 在武器生命周期的关键时刻插入自定义逻辑。所有方法都有默认空实现，只需重写关心的部分。

```csharp
public class MyWeaponHooks : IWeaponHooks
{
    // 玩家装备此武器时调用
    public void OnEquip(int playerIndex) { }

    // 玩家卸下此武器时调用（切换武器或局结束）
    public void OnUnequip(int playerIndex) { }

    // 游戏调用 SetSprite 时触发。
    // 返回非 null 的 string → 用该 key 查找精灵（覆盖默认逻辑）
    // 返回 null → 不干预，继续原版 SetSprite 流程
    public string OnSetSprite(string state, int weaponId)
    {
        // 示例：攻击时用自定义精灵
        if (state == "atk")
            return "firing" + weaponId;    // → "firing1320"
        return null;
    }

    // 武器攻击命中单位时调用，可修改最终伤害
    // target: 被命中的单位；damage: 基础伤害；distance: 攻击距离（格数）
    // 返回值为实际应用的伤害
    public int OnAttackOnUnit(UnitObject target, int damage, int distance, int weaponId)
    {
        return damage; // 不修改时原样返回
    }

    // 玩家按 E 键（主动技能键）时调用
    // 返回 true = 本 Mod 已处理，跳过原版按键逻辑
    // 返回 false = 继续原版逻辑
    public bool OnTrySkipButton(int weaponId) => false;

    // 玩家持此武器时受到伤害，伤害处理完毕后调用
    // atkUnit: 攻击者（可为 null，如陷阱伤害）
    public void OnTakeDamage(UnitObject atkUnit, int weaponId) { }

    // 成功闪避或弹反时调用
    // isParry: true = 弹反，false = 闪避
    public void OnDodgeOrParry(bool isParry, int weaponId) { }
}
```

### 注册示例

```csharp
public override void OnModLoaded()
{
    WeaponModAPI.RegisterWeapon(
        id: 1320,
        displayName: "蓄能炮",
        skillType: "Weapon_ChargeCannon",
        skillFactory: () => new Skill_Weapon_ChargeCannon(),
        hooks: new MyWeaponHooks()
    );
}
```

---

## AtkRangeConfig 攻击范围配置

`AtkRangeConfig` 声明武器主攻击的覆盖格子，由 `AtkRangeResolver.Resolve` 在技能执行时计算出具体格子列表。

### 字段说明

| 字段 | 类型 | 说明 | 默认值 |
|------|------|------|-------|
| `shape` | `AtkRangeShape` | 覆盖形状（见下表） | 必填 |
| `rangeKey` | string | 从 `weaponParams` 读取射程的参数名（动态） | 空 |
| `rangeDefault` | int | `rangeKey` 不存在时的默认射程 | 0 |
| `startOffset` | int | 从玩家前方第几格开始（0=玩家所在格） | 0 |
| `halfWidth` | int | `Wide`/`Area` 的半宽（一侧格数） | 0 |
| `piercing` | bool | 是否穿透（不因碰到单位停止） | false |

### AtkRangeShape 形状

| 形状 | 说明 |
|------|------|
| `Line` | 直线：沿攻击方向延伸 range 格 |
| `Wide` | 宽线：range 格长 × (2*halfWidth+1) 格宽 |
| `Cross` | 十字：4 方向各 range 格 |
| `Ring` | 环形：曼哈顿距离 1~range 的所有格 |
| `Area` | 矩形：以玩家为中心，(2*range+1)×(2*halfWidth+1) |
| `Custom` | 自定义：不由 Resolver 处理，在技能代码中自行计算 |

### 完整示例

```csharp
// 注册时设置
profile.primaryAtkRange = new AtkRangeConfig
{
    shape        = AtkRangeShape.Line,
    rangeKey     = "fireRange",   // 从 weaponParams.Get("fireRange") 动态读取
    rangeDefault = 4,             // fireRange 不存在时默认 4 格
    startOffset  = 1,             // 从玩家前方第 1 格开始，不打脚下
    piercing     = true,          // 穿透
};

// 技能执行时计算命中格子列表
var tiles = AtkRangeResolver.Resolve(
    profile.primaryAtkRange,
    BattleObject.Instance.weaponParams,
    player.unitPos,
    player.aimDir
);

// 对每个格子上的单位造成伤害
foreach (var tile in tiles)
{
    var unit = BattleObject.Instance.GetUnitOnPos(tile);
    if (unit != null)
        unit.Hurt(damage, player);
}
```

---

## WeaponProfile 注册后可设置的字段

`WeaponModAPI.RegisterWeapon` 返回已注册的 `WeaponProfile`，可在注册后继续设置：

```csharp
var profile = WeaponModAPI.RegisterWeapon( /* ... */ );

// 主攻击范围（决定攻击命中哪些格子）
profile.primaryAtkRange = new AtkRangeConfig { ... };

// 近战攻击范围（部分武器有两段攻击）
profile.meleeAtkRange = new AtkRangeConfig { ... };
```
