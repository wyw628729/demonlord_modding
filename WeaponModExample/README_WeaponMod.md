# 蓄能炮武器 Mod 示例

本示例展示了如何为《卡片魔王·只剩个头！》创建一个完整的自定义武器 Mod，以蓄能炮为例，实现了完整的蓄力/开火状态机。

---

## 目录

- [目录结构](#目录结构)
- [武器特性](#武器特性)
- [专属能力](#专属能力)
- [武器 Mod 原理](#武器-mod-原理)
  - [系统概览](#系统概览)
  - [WeaponProfile — 武器档案](#weaponprofile--武器档案)
  - [IWeaponHooks — 生命周期钩子](#iweaponhooks--生命周期钩子)
  - [Skill 武器技能类与状态机](#skill-武器技能类与状态机)
  - [isCharging — 输入接管机制](#ischarging--输入接管机制)
  - [攻击范围系统](#攻击范围系统)
  - [WeaponParamStore — 武器参数系统](#weaponparamstore--武器参数系统)
  - [wp: 前缀能力](#wp-前缀能力)
  - [精灵替换机制](#精灵替换机制)
- [编译说明](#编译说明)
- [注意事项](#注意事项)

---

## 目录结构

```
WeaponModExample/
  mod.json                              ← Mod 元数据
  icon.png                              ← Mod 图标（需自行添加）
  UnitSprites/                          ← 精灵替换
    weapon1320.png                      ← 武器图标（需自行添加）
    1000/                               ← 玩家形态精灵（ID 固定为 1000）
      default1320.png                   ← 默认形态（需自行添加）
      charging11320.png                 ← 蓄力形态（需自行添加）
      firing1320.png                    ← 发射形态（需自行添加）
  AbilityConfigs/                       ← 武器专属能力
    ModSkillConfigs.csv                 ← 能力配置
    unit19200.png                       ← 能力图标（需自行添加）
    unit19201.png                       ← 能力图标（需自行添加）
  CodeMods/                             ← 代码 Mod
    ChargeCannon/
      codemod.json                      ← 代码 Mod 配置
      Main.cs                           ← 入口类（注册武器）
      ChargeCannonHooks.cs              ← 武器生命周期钩子
      Skill_Weapon_ChargeCannon.cs      ← 武器技能类（完整状态机）
      ChargeCannon.dll                  ← 编译后的 DLL（需自行编译）
```

---

## 武器特性

- **武器 ID**: 1320
- **武器名称**: 蓄能炮
- **攻击机制**:
  - 直线穿透激光，默认射程 4 格，宽度 1 格
  - 蓄能状态机：Idle → Charging（蓄力中）→ Firing（开火中）→ Idle
  - 开火持续 2 回合，每回合射出一段激光
  - 蓄力时只能转向，不能移动；开火时可以侧方位移并保持瞄准方向不变
  - 距离满蓄差 1 层时侧向转向 → 直接满蓄开火
  - E 键蓄力、弹反/闪避蓄力
  - 半径 2 格内有敌人 → 自动切换近战模式
  - 反向键取消剩余开火段数并后退

---

## 专属能力

| 能力 ID | 名称 | 效果 | 流派 | 稀有度 |
|---------|------|------|------|--------|
| 19200 | 增程炮管 | 射程 +1 | 1320 | 2 |
| 19201 | 超级电池 | 最大蓄能层数 +1 | 1320 | 1 |

---

## 武器 Mod 原理

### 系统概览

```
WeaponModAPI.RegisterWeapon()
        │
        ▼
  WeaponProfile          ← 武器档案（ID、名称、攻击范围、参数默认值）
        │
        ├── IWeaponHooks ← 生命周期钩子（装备/卸下/精灵/E键/弹反等）
        │
        └── skillFactory ← 创建技能实例（装备时创建一次，随武器存活）
                │
                ▼
         Skill 子类        ← 完整状态机（isCharging 接管输入）
                │
                └── AtkRangeResolver  ← 解析攻击范围，返回目标格子列表
```

调用链：
1. **Mod 加载** → `Main.OnModLoaded()` → `WeaponModAPI.RegisterWeapon()` 把武器档案写入全局注册表
2. **玩家装备武器** → `IWeaponHooks.OnEquip()` 被调用，`weaponParams` 从档案的 `defaultParams` 初始化，**技能实例创建**
3. **玩家按方向键** → 如果 `isCharging = true`，`PlayerInputCheck()` 跳过门/箱检测，直接调用 `OnInputDir` → `Skill.Execute()`
4. **普通移动** → `isCharging = false` 时，玩家走到敌人旁边触发攻击，调用 `Skill.Execute()`
5. **E 键** → `IWeaponHooks.OnTrySkipButton()` → 蓄力状态下调用技能的 `OnEKeyPressed()`
6. **弹反** → `IWeaponHooks.OnDodgeOrParry()` → 调用技能的 `OnDodgeOrParryCharge()`

---

### WeaponProfile — 武器档案

`WeaponModAPI.RegisterWeapon()` 返回一个 `WeaponProfile` 对象，注册完成后可以继续配置攻击范围：

```csharp
var profile = WeaponModAPI.RegisterWeapon(
    id:           1320,
    displayName:  "蓄能炮",
    skillType:    "Weapon_ChargeCannon",
    skillFactory: () => new Skill_Weapon_ChargeCannon(),
    defaultParams: new Dictionary<string, float>
    {
        { "fireRange",  4f },   // 激光射程（格）
        { "chargeMax",  3f },   // 最大蓄能层数
        { "fireWidth",  1f },   // 激光宽度（格）
    },
    hooks:        new ChargeCannonHooks(),
    spriteKeys:   new[] { "charging11320", "firing1320" },
    unlockHint:   "来自蓄能炮 Mod",
    isUnLocked:   true
);

profile.primaryAtkRange = new AtkRangeConfig { ... };   // 远程攻击范围
profile.meleeAtkRange   = new AtkRangeConfig { ... };   // 近战攻击范围
```

| 参数 | 说明 |
|------|------|
| `id` | 武器唯一 ID，建议 ≥ 1320（原版占用 1300-1318） |
| `displayName` | 游戏内显示的武器名称 |
| `skillType` | 技能类型标识符，用于日志输出和存档 |
| `skillFactory` | 装备时调用一次，返回 Skill 实例；实例随武器存活，字段自然持久 |
| `defaultParams` | `weaponParams` 的初始值，装备武器时自动写入 |
| `hooks` | 实现 `IWeaponHooks` 的钩子对象 |
| `spriteKeys` | 声明此武器用到的自定义精灵 key，游戏据此预加载 |

---

### IWeaponHooks — 生命周期钩子

钩子让你在不修改游戏源码的情况下介入武器的各个关键时机：

```csharp
public interface IWeaponHooks
{
    void OnEquip(int playerIndex);                              // 装备时
    void OnUnequip(int playerIndex);                            // 卸下时
    string OnSetSprite(string state, int weaponId);             // 精灵切换
    int OnAttackOnUnit(UnitObject target, int damage, int distance, int weaponId); // 伤害修正
    bool OnTrySkipButton(int weaponId);                         // E 键拦截（返回 true = 已处理）
    void OnTakeDamage(UnitObject atkUnit, int weaponId);        // 受伤时
    void OnDodgeOrParry(bool isParry, int weaponId);            // 闪避/弹反时
}
```

**蓄能炮的钩子职责：**

```csharp
// 精灵切换：根据状态机模式返回对应精灵
public string OnSetSprite(string state, int weaponId)
{
    switch (skill.CurrentMode)
    {
        case CannonMode.Charging: return "charging1" + weaponId;  // "charging11320"
        case CannonMode.Firing:   return "firing" + weaponId;     // "firing1320"
        default:                  return null;                     // Idle：不干预
    }
}

// E 键：蓄力状态下触发蓄力，而非默认跳过行为
public bool OnTrySkipButton(int weaponId)
{
    if (skill.CurrentMode == CannonMode.Charging) {
        skill.OnEKeyPressed();
        return true;   // 已处理，阻止原版行为
    }
    return false;
}

// 弹反/闪避触发蓄力
public void OnDodgeOrParry(bool isParry, int weaponId)
{
    skill.OnDodgeOrParryCharge();
}
```

---

### Skill 武器技能类与状态机

武器技能类继承 `Skill_Weapon`（不是 `Skill`），可以自动获得 `p`（玩家对象）和 `dir`（当前输入方向）字段：

```csharp
// Skill_Weapon 基类（游戏源码）
public class Skill_Weapon : Skill {
    public UnitObjectPlayer p;   // 玩家对象，Execute 时自动设置
    public Vector2Int dir;       // 当前输入方向，Execute 时自动设置
    public override void Execute(UnitObject target) {
        p = target as UnitObjectPlayer;
        dir = target.aimDir;
        p.unitAtk = p.CaculateRealAtk();  // 基础攻击力刷新
    }
}
```

蓄能炮的技能类维护一个三态状态机：

```
[Idle] ──距敌≤3格按前进键──► [Charging] ──蓄满──► [Firing] ──2段打完──► [Idle]
  ▲                              │                    │
  │           反向键 CancelCharge │    反向键 CancelFiring │
  └──────────────────────────────┘◄───────────────────┘
```

关键字段（均为实例字段，自然跨回合持久）：

```csharp
public enum CannonMode { Idle, Charging, Firing }
private CannonMode mode = CannonMode.Idle;
private int chargeCount = 0;
private Vector2Int fireAimDir;   // 开火时锁定的瞄准方向
private int firingRoundsLeft = 0;
```

---

### isCharging — 输入接管机制

**这是蓄力/开火类武器的核心机制**，来自原版大剑（`Skill_Weapon_Sword`）：

```csharp
// UnitObjectPlayer.PlayerInputCheck()（游戏源码）
void PlayerInputCheck()
{
    if (isCharging)
    {
        // isCharging=true 时跳过门/箱检测，直接把输入派发给技能
        OnInputDir?.Invoke(this);  // → Skill.Execute()
        return;
    }
    PlayerCheckDoor();  // 正常流程
    // ...
}
```

设置 `p.isCharging = true` 后，每次玩家按方向键都直接进入 `Skill.Execute()`，技能完全控制这一回合的行为：

| 要实现的效果 | 技能内部操作 |
|------------|------------|
| 蓄力不移动 | 不调用 `PlayerMove()`，调用 `PlayerActionOver()` |
| 蓄力改变朝向 | `p.aimDir = newDir` + `p.ChangeSpriteRotate()` |
| 开火一段 | `p.AddDamageRange(tiles, out _, dmg)` + `p.AtkFinish()` + `PlayerAttackOver()` |
| 开火后继续接管 | 继续保持 `isCharging = true`，`PlayerActionOver()` 结束回合 |
| 退出接管 | `p.isCharging = false` |

**注意**：`isCharging = true` 时游戏内置 30% 受伤减免（蓄力护盾），无需额外代码。

---

### 侧方位移保持瞄准（来自 Laser 的 MoveToPos 回调模式）

开火时按垂直于瞄准方向的键，实现"横移但保持炮口朝向"：

```csharp
private void SideMove(Vector2Int moveDir)
{
    var savedAimDir = fireAimDir;  // 开火时锁定的方向

    p.MoveToPos(moveDir, 1, 1f, default, "move", "no", 0, () =>
    {
        // 位移完成后的回调：恢复炮口方向
        p.aimDir = savedAimDir;
        p.ChangeSpriteRotate(savedAimDir.x < 0);
        ShootOnce(savedAimDir);  // 位移后继续开火
    });
}
```

这与原版激光炮（`Skill_Weapon_Laser`）的后坐力实现完全一致：`MoveToPos` 的回调在移动结束后恢复 `aimDir`。

---

### 攻击范围系统

`AtkRangeConfig` 描述武器的攻击形状，`AtkRangeResolver.Resolve()` 根据玩家位置和朝向计算出实际命中的格子列表。

**AtkRangeConfig 字段：**

| 字段 | 类型 | 说明 |
|------|------|------|
| `shape` | `AtkRangeShape` | 攻击形状：`Line`、`Wide`、`Cross`、`Ring`、`Area`、`Custom` |
| `rangeKey` | `string` | 从 `weaponParams` 读取射程的参数名 |
| `widthKey` | `string` | 从 `weaponParams` 读取宽度的参数名 |
| `rangeDefault` | `int` | 默认射程（格） |
| `widthDefault` | `int` | 默认宽度（格） |
| `startOffset` | `int` | 攻击起点偏移（0 = 玩家所在格，1 = 前方1格） |
| `includeOrigin` | `bool` | 是否包含玩家自身所在格 |
| `piercing` | `bool` | 是否穿透所有单位 |

**蓄能炮的攻击范围：**

```csharp
// 远程：直线穿透激光
profile.primaryAtkRange = new AtkRangeConfig
{
    shape        = AtkRangeShape.Line,
    rangeKey     = "fireRange",
    widthKey     = "fireWidth",
    rangeDefault = 4,
    widthDefault = 1,
    startOffset  = 1,    // 从玩家前方1格开始
    piercing     = true,
};

// 近战：短程攻击（自动切换时使用）
profile.meleeAtkRange = new AtkRangeConfig
{
    shape        = AtkRangeShape.Line,
    rangeDefault = 1,
    startOffset  = 0,
};
```

---

### WeaponParamStore — 武器参数系统

`BattleObject.Instance.weaponParams` 存储当前武器的动态参数：

**双层结构：**
- `_base`：来自 `defaultParams`，切换武器时重置
- `_bonus`：`wp:` 前缀能力累加的增量，跨武器持久，新局清空

```csharp
var store = BattleObject.Instance.weaponParams;
int range = store.GetInt("fireRange", 4);       // 读取（_base + _bonus）
int max   = store.GetInt("chargeMax", 3);
```

**何时用 `weaponParams`，何时用技能字段？**

| 数据 | 建议存放位置 | 原因 |
|------|------------|------|
| 蓄能层数、开火剩余段数、当前状态 | **技能实例字段** | 技能随武器存活，自然持久 |
| 射程、宽度、伤害倍率 | **weaponParams** | 能力系统（`wp:` 前缀）需要从这里读取并修改 |

---

### wp: 前缀能力

`wp:` 前缀告诉能力系统：这个参数修改的是 `weaponParams._bonus`。

```csv
能力ID,类型,触发器,冷却,参数名1,参数1,...,名字,描述,所属流派,稀有度,...
19200,passive,isOnce,0,wp:fireRange,1,,,,,,增程炮管,蓄能炮射程+1,1320,2,,,,,
19201,passive,isOnce,0,wp:chargeMax,1,,,,,,超级电池,蓄能炮最大蓄能层数+1,1320,1,,,,,
```

| 字段 | 值 | 说明 |
|------|---|------|
| `参数名1` | `wp:fireRange` | 修改 weaponParams["fireRange"] |
| `参数1` | `1` | 增量 +1 |
| `所属流派` | `1320` | 武器 ID，只在装备该武器时投放此能力 |

---

### 精灵替换机制

**玩家 ID 固定为 `1000`**，所有武器的玩家形态精灵放在 `UnitSprites/1000/`，key 中嵌入武器 ID 避免冲突：

```
UnitSprites/
  weapon1320.png            ← 武器列表图标（key = "weapon1320"）
  1000/
    default1320.png         ← 玩家默认形态（key = "default1320"，id1320武器装备时显示）
    charging11320.png       ← 玩家蓄力形态（key = "charging11320"）
    firing1320.png          ← 玩家发射形态（key = "firing1320"）
```

`OnSetSprite(state, weaponId)` 钩子返回精灵 key 字符串则使用该精灵；返回 `null` 则使用原版逻辑。

---

## 编译说明

1. **创建 C# 类库项目**
   - Visual Studio → 新建项目 → 类库（.NET Framework 4.7.2 或更高）

2. **添加引用**
   - 添加 `Assembly-CSharp.dll`（游戏程序集）
   - 添加 `UnityEngine.CoreModule.dll`
   - 引用位于：`游戏目录/DemonLordJustABlock_Data/Managed/`

3. **编译**
   - 将 `Main.cs`、`ChargeCannonHooks.cs`、`Skill_Weapon_ChargeCannon.cs` 添加到项目
   - 生成 → 生成解决方案
   - 将生成的 `ChargeCannon.dll` 复制到 `CodeMods/ChargeCannon/` 目录

4. **测试**
   - 将整个 `WeaponModExample/` 复制到本地 Mod 目录：
     `C:\Users\<用户名>\AppData\LocalLow\YuWave\DemonLordJustABlock\LocalMods\`
   - 启动游戏，在【模组】界面启用 Mod
   - 查看日志：`C:\Users\<用户名>\AppData\LocalLow\YuWave\DemonLordJustABlock\Player.log`

---

## 精灵资源

本示例不包含精灵图片，需自行准备：

| 文件 | 用途 | 建议尺寸 |
|------|------|----------|
| `icon.png` | Mod 图标 | 256×256 |
| `weapon1320.png` | 武器图标 | 128×128 |
| `default1320.png` | 默认形态 | 根据实际调整 |
| `charging11320.png` | 蓄力形态 | 根据实际调整 |
| `firing1320.png` | 发射形态 | 根据实际调整 |
| `unit19200.png` | 增程炮管能力图标 | 128×128 |
| `unit19201.png` | 超级电池能力图标 | 128×128 |

---

## 注意事项

1. **武器 ID 使用 ≥ 1320**：避免与原版武器（1300-1318）冲突
2. **精灵 key 命名规范**：在 key 中嵌入武器 ID（如 `charging11320`）避免不同武器精灵冲突
3. **玩家形态精灵目录**：所有武器的玩家形态精灵都放在 `1320/` 目录 （当前的武器id）
4. **技能继承 `Skill_Weapon`**：不要继承 `Skill`，`Skill_Weapon` 会在 `Execute` 时自动设置 `p` 和 `dir`
5. **状态持久化**：技能实例随武器装备周期存活，`mode`、`chargeCount` 等字段直接写在技能类里，无需存入 `weaponParams`
6. **`weaponParams` 用于能力系统**：射程、宽度等需要被 `wp:` 前缀能力修改的参数才放入 `weaponParams`
7. **Hooks 的技能引用**：Hooks 无法在注册时拿到技能实例，通过在 `Init` 中调用 `hooks.SetSkill(this)` 注入
8. **卸载时注意清理**：`OnUnequip` 中清空 Hooks 里的技能引用，防止引用悬空
