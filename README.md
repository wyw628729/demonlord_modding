# 《卡片魔王·只剩个头！》Mod 说明

> [English Version](./README_EN.md)

---

## AI 辅助制作 Mod（推荐）

如果你在使用 AI:比如[Claude Code](https://claude.ai/code)、Codex、GLM、DeepSeek等等，可以利用内置的 **cardventure-mod-creator** skill 来辅助制作 Mod。

它能帮你：
- 根据你的想法快速生成 Mod 目录结构和配置文件
- 编写代码 Mod 的 C# 入口类、钩子、技能类
- 配置自定义武器（`WeaponModAPI`、攻击范围、专属能力）
- 解释 API 用法和排查 Mod 加载问题

**使用方式**：直接在 Claude Code 中用自然语言描述你想做的 Mod，例如：

```
帮我做一个替换露露立绘的 Mod
帮我新增一个让玩家每次攻击后回1点血的能力
帮我创建一个会爆炸的新武器，武器ID是1321
```

skill 会自动识别你的意图并给出对应的文件结构、代码模板和配置示例。

---

## 目录

- [1. Mod 会从哪里加载？](#1-mod-会从哪里加载)
- [2. Mod 文件夹基础结构](#2-mod-文件夹基础结构)
- [3. 图片替换 Mod](#3-图片替换-mod)
- [4. 能力 Mod](#4-能力-mod)
- [5. 代码 Mod](#5-代码-mod)
- [6. 自定义武器 Mod](#6-自定义武器-mod)
- [7. 常见问题](#7-常见问题)
- [8. 免责声明](#8-免责声明)

本游戏当前支持4类 Mod：

1. **图片替换 Mod**
2. **能力 Mod**
3. **代码 Mod**
4. **自定义武器 Mod**

- 如果你只是想替换角色立绘、表情、场景贴图等内容，使用图片替换 Mod 即可。
- 如果你想替换已有能力，或者自己新增能力，可以使用能力 Mod。
- 如果你想修改数据、注册事件、调用游戏中的方法，则需要使用代码 Mod。

---


## 1. Mod 会从哪里加载？

游戏会扫描并加载以下位置的 Mod：

- **Steam 创意工坊**：你订阅的 Workshop 物品目录
- **本地 Mod**：`LocalMods` 文件夹（一般在AppData\LocalLow\YuWave\DemonLordJustABlock\LocalMods）

你可以在游戏内的【模组】界面中对每个 Mod 进行**启用 / 禁用**。

---

## 2. Mod 文件夹基础结构

- [你可以在这里看见一个简单的MOD案例 ](./TestModExample)
  
一个完整的 Mod 文件夹通常如下：

```txt
MyMod/
  mod.json
  preview.png
  UnitSprites/        （可选：图片替换 Mod）
  CodeMods/           （可选：代码 Mod）
  AbilityConfigs/     （可选：能力 Mod）
```

### 2.1 必要文件

#### mod.json

Mod 的基础信息文件，用于填写名称、作者、描述等内容。

```json
{
  "title": "我的 Mod",
  "description": "Mod 描述",
  "author": "作者名",
  "version": "1.0.0"
}
```

**字段说明：**

| 字段 | 必需 | 说明 |
|------|------|------|
| `title` | 推荐 | Mod 标题，缺失时会使用 `name` 字段或目录名 |
| `name` | 备选 | `title` 的备选字段，优先级低于 `title` |
| `description` | 可选 | Mod 描述 |
| `author` | 推荐 | 作者名 |
| `authorName` | 备选 | `author` 的备选字段，优先级低于 `author` |
| `version` | 可选 | 版本号 |

> **重要提醒**：不要在 mod.json 中放 `dll` 或 `entryClass` 字段！游戏不会从 mod.json 读取代码 Mod 配置，代码 Mod 的配置在 `CodeMods/codemod.json` 中。

#### Mod 图标

Mod 图标文件放在 Mod 根目录，游戏按以下优先级加载：

1. `icon.png`（最高优先级，推荐使用）
2. `preview.png`
3. `thumbnail.png`
4. `cover.png`

推荐使用正方形图片，分辨率 `256×256`，PNG 格式。

一个 Mod 可以只包含图片替换，也可以只包含代码，或者两者同时包含。

---

## 3. 图片替换 Mod

图片替换 Mod 的原理很简单：  
把对应命名的 PNG 文件放入指定文件夹，游戏扫描后就会覆盖原图片。

理论上，只要游戏中存在对应资源键，大多数可见贴图都可以被替换。

### 3.1 文件夹结构

- [单位 ID 与 SpriteKey 对照表：UnitConfig_SpriteKeys.csv](./GuideDocument/UnitConfig_SpriteKeys.csv)

推荐结构如下：

```txt
MyMod/
  mod.json
  preview.png
  UnitSprites/           （`UnitSprites` 文件夹名称是固定的，用于读取）
    <UnitType>/          （单位 ID，通常是纯数字）
      <SpriteKey>.png    （为图片的key名，需与表格中的key名完全一致）
```
> 注意：
> - 图片推荐格式：`PNG`
> - 推荐尺寸：通常可从 `128×128` 开始，根据实际资源调整

### 3.2 特殊图片

部分图片不属于某个单位 ID，例如某些独立 UI 图或特殊事件图。  
这类图片可以直接放在 `UnitSprites/` 根目录下，文件名使用对应资源名即可。

例如，将所有的露露替换（包括色欲挑战）：

```txt
LuLuMod/
  UnitSprites/
    luluHappySprite.png
    luluAtkSprite.png
    luluAtk1Sprite.png
    1102/
      default.png
      happy.png
      move.png
```

---
## 4. 能力 Mod

你现在可以通过 Mod：覆盖游戏中已有的能力、新增一个全新的能力

实现方式很简单：
只需要在 Mod 里提供一个 AbilityConfigs 文件夹，里面放一个 ModSkillConfigs.csv，再放若干个能力图标即可。

### 4.1 文件夹结构

推荐结构如下：

```txt
MyAbilityMod/
  mod.json
  preview.png
  AbilityConfigs/             （`AbilityConfigs` 文件夹名称是固定的，用于读取）
    ModSkillConfigs.csv   （`ModSkillConfigs.csv` 文件名称是固定的，用于读取）
    unit19001.png            （能力的图标，图标文件建议命名为 unit技能ID.png）
    unit19002.png
```

- 例如，如果你新增了能力 ID 是 19001，并且你在 AbilityConfigs 文件夹里放了这张图：unit19001.png，游戏就会自动把它当作这个能力的图标读取。


### 4.2 工作方式

游戏会先读取原版能力表，然后再读取你 Mod 里的 AbilityConfigs/ModSkillConfigs.csv。

规则如下：
-  如果 Mod 中的能力 ID 已经存在：会覆盖原版能力
-  如果 Mod 中的能力 ID 不存在：会新增一个能力
-  为了避免冲突，建议大家新增能力时尽量使用一个较大的新 ID，比如大于10000

### 4.3 CSV 格式

能力 Mod 使用的字段，和游戏原版 SkillConfig 完全一致。

你可以直接参考：
- [示例ModSkillConfigs文件：ModSkillConfigs.csv](./GuideDocument/AbilityConfigs/ModSkillConfigs.csv)

并且我也会附上的当前本地 **SkillConfigs.csv** 供你参考：
- [本地的SkillConfigs文件：SkillConfigs.csv](./GuideDocument/SkillConfigs.csv)

你只需要按照原表的格式填写即可。

### 4.4 能力字段说明

这里不会把所有字段讲得特别复杂，你可以直接对照本地完整的 SkillConfigs.csv示例文件来看。

- **id**：能力的唯一编号。（重复已有 ID = 覆盖原能力，使用新 ID = 新增能力）
- **类型**：能力类型，也就是这个能力具体会做什么，例如：生成炸弹（spawnBomb）、召唤闪电（spawnLightning）、改变参数变量（passive）、other等
- **触发器**：触发时机，也就是这个能力在什么情况下走冷却、触发效果，比如移动时（move）、关卡开始时（levelStart）、按道具键时（activeSkill）等
- **冷却**：冷却次数，达到对应次数后，能力才会执行，默认是0。
- **参数**：
- - paramName1 / param1
- - paramName2 / param2
- - paramName3 / param3
- - 这几组字段是能力的具体参数。
- - 不同 type 会读取不同参数，所以你通常需要参考原版 SkillConfigs.csv 中同类能力的写法。
- **持续时间**：有些能力会用到持续时间（例如魔神真言能力），有些则不会，如果不需要可以默认
- **名字**：能力名字
- **描述**：能力描述
- **所属流派**：所属能力池 / 流派池，比如炸弹流是1200，不同流派、武器、彩色能力等通常都和它有关
- **稀有度**：稀有度等级，默认是1
- **可选次数**：这个能力最多能被选择多少次，默认是无穷
- **武器通用能力**：这里特指最初幻想武器能否随机到这个锻造能力，默认是FALSE
- **是否基础能力**：是否是某个流派的基础能力，如果不是，那么就需要玩家选到这个流派的基础能力，才会投放它，默认是TRUE
- **是否进图鉴**：是否会加入图鉴，默认是TRUE
- **是否正常投放**：是否正常投放，默认是TRUE


### 4.5 推荐做法

如果你是第一次尝试能力 Mod，建议这样开始：

- 先复制一个示例 AbilityConfigs 文件夹
- 先试着覆盖一个原版能力
- 确认生效后，再尝试新增一个能力
- 最后再给它加上自己的图标



---
## 5. 代码 Mod

代码 Mod 允许你通过 C# 编写自己的逻辑，并在特定时机执行操作。  
例如：修改初始数值、调用现有方法、扩展部分游戏行为……

> **进阶示例：自定义武器 Mod**  
> 如果你想创建自带技能、专属能力、独立精灵的完整自定义武器，可以参考 [WeaponModExample](./WeaponModExample)。  
> 该示例展示了如何使用 `WeaponModAPI` 注册武器、实现武器钩子、配置攻击范围、创建武器专属能力等高级用法。

### 5.1 文件夹结构

代码 Mod 支持两种放置方式：

**方式 1：直接放在 CodeMods 根目录**（适合单个代码 Mod）

```txt
MyMod/
  mod.json
  icon.png
  CodeMods/            （`CodeMods` 文件夹名称是固定的，用于读取）
    codemod.json       （用于配置 dll 文件）
    MyCodeMod.dll    
```

**方式 2：使用子目录**（推荐，支持单个 Mod 包含多个代码 Mod）

```txt
MyMod/
  mod.json
  icon.png
  CodeMods/
    MyCodeMod/         （代码 Mod 包目录）
      codemod.json
      MyCodeMod.dll
    AnotherCodeMod/    （另一个代码 Mod）
      codemod.json
      AnotherCodeMod.dll
```

### 5.2 `codemod.json` 配置

示例：

```json
{
  "dll": "MyCodeMod.dll",
  "entryClass": "MyCodeMod.Main",
  "displayName": "My Cool Mod"
}
```

**字段说明：**

| 字段 | 必需 | 说明 |
|------|------|------|
| `dll` | 是 | DLL 文件名（相对于 codemod.json 所在目录） |
| `entryClass` | 是 | 入口类完整名称（命名空间.类名） |
| `displayName` | 否 | 显示名称，用于日志和 GameObject 命名。未设置时使用类名 |

> **备注**：也支持使用 `code_mod.json` 作为文件名，但推荐统一使用 `codemod.json`。

### 5.3 制作流程

先使用Visual Studio新建一个 C# 类库工程
然后为代码 Mod 工程添加如下引用：

- `Assembly-CSharp.dll`
- `UnityEngine.CoreModule.dll`

通常你可以在游戏目录中找到：

```txt
DemonLordJustABlock_Data/Managed/
```

编译成功后，你会得到类似这样的文件：

```txt
MyCodeMod.dll
```

将它与 `codemod.json` 一起放入 `CodeMods` 文件夹中即可进行测试。

推荐先使用本地 Mod 进行测试，确认功能正常后，再整理为 Workshop 版本。


### 5.4 接口介绍

- [你可以在这里查看一个简单的代码案例：用于实现玩家初始化王城时，将可携带贴纸数量设置为3](./GuideDocument/TestCodeMod.cs)

- [你可以在这里查看部分常用的属性，和他们的含义](./GuideDocument/BattleObject_Mod_Variables.csv)

- 如果你需要更多的文档，api介绍，欢迎私信鱼尾，我会及时补充


---

## 6. 自定义武器 Mod

自定义武器 Mod 是代码 Mod 的高级应用，允许你创建拥有独立技能系统、专属能力和自定义精灵的完整武器。

### 6.1 特性

- **完整的武器系统**：注册新武器 ID、显示名称、技能类型
- **生命周期钩子**：装备/卸下、精灵切换、伤害修正、受伤/闪避/弹反等事件
- **攻击范围配置**：直线、扇形、圆形等多种攻击形状，支持穿透、反向射击等特性
- **武器专属能力**：使用 `wp:` 前缀创建只对该武器生效的能力
- **动态参数系统**：武器参数在切换武器后保留，新局开始时重置

### 6.2 完整示例

[WeaponModExample](./WeaponModExample) 提供了一个完整的蓄能炮武器示例，包含：

- 武器注册与技能系统
- 生命周期钩子实现（精灵切换、伤害加成）
- 攻击范围配置（直线穿透射击）
- 武器专属能力（射程+1、最大蓄能层数+1）
- 完整的目录结构和编译说明

详见 [WeaponModExample/README_WeaponMod.md](./WeaponModExample/README_WeaponMod.md)

### 6.3 核心 API

**注册武器：**

```csharp
WeaponModAPI.RegisterWeapon(
    id:           1320,                          // 武器 ID (≥ 1320 避免冲突)
    displayName:  "蓄能炮",                      // 显示名称
    skillType:    "Weapon_ChargeCannon",         // 技能类型名
    skillFactory: () => new Skill_Weapon_ChargeCannon(),
    defaultParams: new Dictionary<string, float>
    {
        { "fireRange",  4f },                    // 默认射程
        { "chargeMax",  3f },                    // 默认最大蓄能层数
    },
    hooks:        new ChargeCannonHooks(),       // 生命周期钩子
    spriteKeys:   new[] { "charging11320", "firing1320" },
    unlockHint:   "来自蓄能炮 Mod",
    isUnLocked:   true
);
```

**生命周期钩子接口 (`IWeaponHooks`)：**

```csharp
public interface IWeaponHooks
{
    void OnEquip(int playerIndex);                              // 装备时
    void OnUnequip(int playerIndex);                            // 卸下时
    string OnSetSprite(string state, int weaponId);             // 精灵切换
    int OnAttackOnUnit(UnitObject target, int damage, int distance, int weaponId);  // 攻击伤害修正
    bool OnTrySkipButton(int weaponId);                         // E 键跳过行为
    void OnTakeDamage(UnitObject atkUnit, int weaponId);        // 受伤时
    void OnDodgeOrParry(bool isParry, int weaponId);            // 闪避/弹反时
}
```

**攻击范围配置：**

```csharp
profile.primaryAtkRange = new AtkRangeConfig
{
    shape         = AtkRangeShape.Line,          // 形状：直线
    rangeKey      = "fireRange",                 // 从 weaponParams 读取射程
    rangeDefault  = 4,                           // 默认射程 4 格
    startOffset   = 1,                           // 从玩家前方 1 格开始
    piercing      = true,                        // 穿透攻击
};
```

**武器专属能力 (CSV 配置)：**

```csv
能力ID,类型,触发器,冷却,参数名1,参数1,参数名2,参数2,参数名3,参数3,持续时间,名字,描述,所属流派,稀有度,可选次数,武器通用能力,是否基础能力,是否进图鉴,是否正常投放
19200,passive,isOnce,0,wp:fireRange,1,,,,,,增程炮管,蓄能炮射程+1,1320,2,,,,,
19201,passive,isOnce,0,wp:chargeMax,1,,,,,,超级电池,蓄能炮最大蓄能层数+1,1320,1,,,,,
```

> **`wp:` 前缀规则**：  
> - 修改的是 `weaponParams`（武器动态参数）
> - 切换武器后仍保留
> - 新局开始时清空
> - 所属流派填武器 ID（如 `1320`）可限制只对该武器生效

### 6.4 精灵资源

自定义武器的精灵放在 `UnitSprites/1000/` 目录（**玩家 ID 固定为 1000**）：

```txt
WeaponModExample/
  UnitSprites/
    weapon1320.png              ← 武器图标
    1000/                       ← 玩家形态精灵（ID 固定为 1000）
      default1320.png           ← 默认形态
      charging11320.png         ← 蓄力形态
      firing1320.png            ← 发射形态
```

**命名规范**：精灵 key 中嵌入武器 ID（如 `charging11320`）避免不同武器的精灵冲突。

### 6.5 推荐做法

1. **武器 ID 使用 ≥ 1320**：避免与原版武器（1300-1318）冲突
2. **参考完整示例**：[WeaponModExample](./WeaponModExample) 包含了所有必要的代码和配置
3. **先本地测试**：将 Mod 放到 `LocalMods/` 测试，确认无误后再发布到创意工坊
4. **查看日志**：游戏日志位于 `AppData\LocalLow\YuWave\DemonLordJustABlock\Player.log`，可用于调试

---

## 7. 常见问题

### TypeLoadException：DefaultInterpolatedStringHandler

**错误信息：**

```
[CodeModRuntime] 加载失败
System.TypeLoadException: Could not resolve type with token 01000011 from typeref
(expected class 'System.Runtime.CompilerServices.DefaultInterpolatedStringHandler'
in assembly 'System.Runtime, Version=8.0.0.0, ...)
```

**原因：**  
代码 Mod 工程的目标框架设置为 `net8.0`（或其他 .NET 6+），而 Unity 运行在 **Mono** 上，Mono 的兼容层相当于 .NET Standard 2.1。  
C# 10 对 `net6.0+` 目标会自动将 `$"..."` 字符串插值优化为使用 `DefaultInterpolatedStringHandler`，而该类型不存在于 Unity 的 Mono 运行时，因此 DLL 在加载时直接报错。

**解决方案：**  
将 `.csproj` 的目标框架改为 `netstandard2.1`，语言版本降为 `9`，并移除 `<ImplicitUsings>`：

```xml
<!-- 修改前（错误） -->
<PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>10</LangVersion>
    <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>

<!-- 修改后（正确） -->
<PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
    <LangVersion>9</LangVersion>
</PropertyGroup>
```

重新编译后，DLL 将与 Unity Mono 完全兼容。

---

## 8. 免责声明

本游戏允许玩家通过 Mod 扩展内容，但不保证所有 Mod 之间完全兼容哦~  

代码 Mod 本质上会执行第三方代码，请只安装你信任来源的 Mod~
