---
name: cardventure-mod-creator
description: |
  创建《卡片魔王·只剩个头！》(Cardventure) 的 Mod。当用户想要为 Cardventure 游戏制作 Mod、替换精灵图、新增/修改能力、编写代码 Mod、注册自定义武器、创建 mod.json 或 codemod.json、调试 Mod 加载问题时使用此 skill。即使用户只是提到"改个立绘"、"加个技能"、"写个 Mod"、"替换贴图"、"新武器"、"自定义武器"，也应触发此 skill。
---

# Cardventure Mod 制作指南

本 skill 指导你为《卡片魔王·只剩个头！》创建四种类型的 Mod：精灵替换、能力 Mod、代码 Mod、自定义武器 Mod。

## 快速决策

用户说想做什么 → 你应该建议的 Mod 类型：

| 用户意图 | Mod 类型 | 难度 |
|---------|---------|------|
| 换角色立绘/表情/武器图标 | 精灵替换 | 低 |
| 改能力数值/新增能力 | 能力 Mod | 中 |
| 改游戏数据/注册事件/写逻辑 | 代码 Mod | 高 |
| 新增完整武器（攻击技能+能力+精灵） | 自定义武器 Mod | 高 |
| 以上任意组合 | 混合 Mod | 视情况 |

一个 Mod 可以同时包含以上所有类型的内容。

---

## 1. Mod 放置位置

游戏从两个位置加载 Mod：

- **Steam 创意工坊**：订阅的 Workshop 物品
- **本地 Mod**：`Application.persistentDataPath/LocalMods/`
  - Windows 默认路径：`C:\Users\<用户名>\AppData\LocalLow\YuWave\DemonLordJustABlock\LocalMods`

游戏内【模组】界面可启用/禁用每个 Mod。

---

## 2. Mod 目录结构总览

```
MyMod/
  mod.json                    ← 必需：Mod 元数据
  icon.png                    ← 推荐：Mod 图标（优先级最高）
  preview.png                 ← 备选：图标（优先级第2）
  UnitSprites/                ← 精灵替换 Mod（可选）
    <UnitType>/
      <SpriteKey>.png
    <SpecialName>.png
  AbilityConfigs/             ← 能力 Mod（可选）
    ModSkillConfigs.csv
    unit<AbilityID>.png
  CodeMods/                   ← 代码 Mod（可选）
    <CodeModName>/            ← 推荐用子目录
      codemod.json
      <CodeModName>.dll
```

目录名 `UnitSprites`、`AbilityConfigs`、`CodeMods` 和文件名 `ModSkillConfigs.csv`、`codemod.json` 都是固定的，游戏按这些名称查找。

---

## 3. mod.json 格式

这是 Mod 的元数据文件，必须放在 Mod 根目录下。

```json
{
  "title": "Mod 标题",
  "description": "Mod 描述",
  "author": "作者名",
  "version": "1.0.0"
}
```

### 字段说明

| 字段 | 必需 | 说明 |
|------|------|------|
| `title` | 推荐 | Mod 标题，缺失时用目录名 |
| `name` | 备选 | `title` 的备选字段，优先级低于 `title` |
| `description` | 可选 | Mod 描述 |
| `author` | 推荐 | 作者名 |
| `authorName` | 备选 | `author` 的备选字段，优先级低于 `author` |
| `version` | 可选 | 版本号 |

### 重要提醒

**不要在 mod.json 中放 `dll` 或 `entryClass` 字段！** 游戏不会从 mod.json 读取代码 Mod 配置，代码 Mod 的配置在 `CodeMods/codemod.json` 中。在 mod.json 写这两个字段不会导致崩溃，但没有任何效果，且会误导他人。

### 图标文件

放在 Mod 根目录，游戏按以下优先级加载：

1. `icon.png`（最高优先级）
2. `preview.png`
3. `thumbnail.png`
4. `cover.png`

推荐使用正方形图片，256×256 分辨率，PNG 格式。

---

## 4. 精灵替换 Mod

### 4.1 原理

在 `UnitSprites/` 目录下放入正确命名的 PNG 文件，游戏扫描后会覆盖原图。后加载的 Mod 覆盖先加载的（同一键名时最后生效的赢）。

### 4.2 单位精灵

按单位 ID 创建子目录，放入对应 key 名的 PNG：

```
UnitSprites/
  111/              ← 单位 ID 111（露露）
    default.png     ← 默认立绘
    atk1.png        ← 攻击动作
    happy.png       ← 开心表情
    sad.png         ← 悲伤表情
    move.png        ← 移动
    shock.png       ← 震惊
  1102/             ← 单位 ID 1102（露露 NPC）
    default.png
    fight.png
    happy.png
```

key 名必须与游戏内部的 SpriteKey 完全一致。需要查阅 sprite key 对照表时，读取 `references/sprite-keys-reference.md`。

### 4.3 特殊精灵

不属于特定单位 ID 的图片直接放在 `UnitSprites/` 根目录：

```
UnitSprites/
  luluHappySprite.png      ← 噜噜开心表情（色欲挑战用）
  luluAtkSprite.png        ← 噜噜攻击表情
  luluAtk1Sprite.png       ← 噜噜攻击表情2
  weapon1301.png           ← 武器 1301 图标
  weapon1302.png           ← 武器 1302 图标
```

特殊精灵类别：
- **噜噜表情**：`luluHappySprite`、`luluAtkSprite`、`luluAtk1Sprite`
- **武器图标**：`weapon` + 武器 ID（如 `weapon1300` ~ `weapon1314`）

### 4.4 制作要点

- 图片格式必须是 PNG
- 推荐尺寸从 128×128 开始，根据实际资源调整
- 覆盖精灵会保留原始的 pivot 和 PPU 设置（游戏自动处理）
- 如果多个 Mod 覆盖同一精灵，最后加载的生效，无冲突提示

---

## 5. 能力 Mod

### 5.1 原理

在 `AbilityConfigs/` 目录下放 `ModSkillConfigs.csv`，游戏先加载原版能力表，再加载 Mod 的 CSV。同 ID 覆盖原版，新 ID 则新增能力。

### 5.2 目录结构

```
AbilityConfigs/
  ModSkillConfigs.csv       ← 固定文件名
  unit19100.png             ← 能力 ID 19100 的图标
  unit9150.png              ← 能力 ID 9150 的图标
```

图标命名规则：`unit<能力ID>.png`。如果没提供图标，游戏会尝试加载原版图标。

### 5.3 CSV 格式

表头和原版 `SkillConfigs.csv` 完全一致（中文表头）。解析器按列位置解析，不依赖表头文本，所以表头内容不影响功能，但保持一致有助于可读性。

```csv
能力ID,类型,触发器,冷却,参数名1,参数1,参数名2,参数2,参数名3,参数3,持续时间,名字,描述,所属流派,稀有度,可选次数,武器通用能力,是否基础能力,是否进图鉴,是否正常投放
19100,spawnLightning,hurt,0,,10,,,,,,复仇闪电,受伤时召唤10道闪电,1202,2,,,,,
9150,spawn,atk,20,spawnUnit,2,,1,,,,魔王兵,攻击20次召唤1个弓箭手,1203,1,,,,,
```

### 5.4 字段详解

完整字段说明、所有能力类型和触发器列表见 `references/ability-csv-reference.md`。

必填字段：**id**（唯一 ID，重复则覆盖原版）、**type**（能力类型）、**trigger**（触发时机）、**名字**、**描述**。其余字段均有合理默认值，按需填写即可。所属流派（poolType）决定能力在哪个流派池中出现，1200–1318 是原版流派，Mod 武器专属能力使用武器 ID（如 1320）。

### 5.5 制作建议

新增能力建议使用大于 10000 的 ID 避免与原版冲突。不同 `type` 读取不同参数，参考原版 `SkillConfigs.csv` 中同类能力的写法。

---

## 6. 代码 Mod

### 6.1 原理

编写 C# 类库，继承 `SimpleModBehaviour`，游戏通过 `Assembly.LoadFrom` 加载 DLL 并用 `AddComponent` 挂载到 GameObject 上。代码 Mod 与游戏运行在同一 AppDomain，拥有完全访问权限。

### 6.2 目录结构

推荐使用子目录（支持单个 Mod 包含多个代码 Mod）：

```
CodeMods/
  MyCodeMod/              ← 代码 Mod 包目录
    codemod.json          ← 代码 Mod 清单
    MyCodeMod.dll         ← 编译好的 DLL
  AnotherCodeMod/
    codemod.json
    AnotherCodeMod.dll
```

也可以直接放在根目录（只适合单个代码 Mod）：

```
CodeMods/
  codemod.json
  MyCodeMod.dll
```

### 6.3 codemod.json 格式

```json
{
  "dll": "MyCodeMod.dll",
  "entryClass": "MyCodeMod.Main",
  "displayName": "My Cool Mod"
}
```

| 字段 | 必需 | 说明 |
|------|------|------|
| `dll` | 是 | DLL 文件名（相对于 codemod.json 所在目录） |
| `entryClass` | 是 | 入口类完整名称（命名空间.类名） |
| `displayName` | 否 | 显示名称，用于日志和 GameObject 命名。未设置时使用类名 |

也支持 `code_mod.json` 作为文件名（推荐统一使用 `codemod.json`）。

### 6.4 SimpleModBehaviour API

```csharp
public abstract class SimpleModBehaviour : MonoBehaviour
{
    public string ModFolder { get; internal set; }   // codemod.json 所在目录
    public string ModName { get; internal set; }     // 显示名称

    public virtual void OnModLoaded() { }    // Mod 加载完成后调用
    public virtual void OnModUnloaded() { }  // Mod 卸载时调用

    protected void Log(string msg)           // 带前缀的日志
    {
        Debug.Log($"[CodeMod:{ModName}] {msg}");
    }
}
```

因为继承 `MonoBehaviour`，你可以使用所有 Unity 生命周期（`Start`、`Update`、协程等）。

### 6.5 BattleObject 事件与示例

游戏提供了 4 个静态事件供代码 Mod 订阅：`OnAfterHomeDataLoad`（回王城后）、`OnLevelStart`（新关卡）、`OnChapterStart`（新章节）、`OnGameStart`（新一局）。

完整事件列表、订阅方式和代码示例见 `references/code-mod-reference.md`。可修改的 BattleObject 变量见 `references/battleobject-variables.md`。

### 6.6 制作流程

1. Visual Studio 创建 C# 类库项目
2. 添加引用：`Assembly-CSharp.dll`、`UnityEngine.CoreModule.dll`（位于游戏目录 `DemonLordJustABlock_Data/Managed/`）
3. 创建继承 `SimpleModBehaviour` 的入口类，重写 `OnModLoaded` 和 `OnModUnloaded`
4. 编译生成 DLL，与 `codemod.json` 一起放入 `CodeMods/` 目录

详细步骤和调试技巧见 `references/code-mod-reference.md`。

### 6.7 重要限制

- **程序集无法卸载**：.NET 不支持真正卸载 DLL，卸载 Mod 只销毁 GameObject，旧类型和静态字段仍留在 AppDomain 中。
- **无沙箱**：代码 Mod 拥有完全的游戏进程访问权限。
- **`OnModUnloaded` 中必须取消事件订阅**：否则事件回调会引用已销毁的对象导致内存泄漏或崩溃。

---

## 7. 自定义武器 Mod（WeaponModAPI）

游戏内置了完整的武器注册 API，代码 Mod 可以通过 `WeaponModAPI.RegisterWeapon` 注册全新武器，让它出现在武器架、拥有自己的攻击技能、精灵和能力加成系统。

### 7.1 推荐武器 ID

原版武器 ID 为 1300–1318，Mod 武器推荐使用 **≥ 1320** 的 ID 避免冲突。

### 7.2 注册武器

在 `OnModLoaded()` 中调用 `WeaponModAPI.RegisterWeapon`：

```csharp
using System.Collections.Generic;
using UnityEngine;

namespace ChargeCannon
{
    public class Main : SimpleModBehaviour
    {
        public override void OnModLoaded()
        {
            var profile = WeaponModAPI.RegisterWeapon(
                id:           1320,
                displayName:  "蓄能炮",
                skillType:    "Weapon_ChargeCannon",
                skillFactory: () => new Skill_Weapon_ChargeCannon(),
                defaultParams: new Dictionary<string, float>
                {
                    { "fireRange",  4f },
                    { "chargeMax",  3f },
                },
                hooks:        new ChargeCannonHooks(),
                spriteKeys:   new[] { "charging11320", "firing1320" },
                unlockHint:   "来自蓄能炮 Mod"
            );

            // 注册后可设置攻击范围
            profile.primaryAtkRange = new AtkRangeConfig
            {
                shape    = AtkRangeShape.Line,
                rangeKey = "fireRange",   // 从 weaponParams 读取
            };
        }

        public override void OnModUnloaded() { }
    }
}
```

### 7.3 RegisterWeapon 参数说明

| 参数 | 类型 | 必需 | 说明 |
|------|------|------|------|
| `id` | int | 是 | 武器 ID，推荐 ≥ 1320 |
| `displayName` | string | 是 | 武器架显示名 |
| `skillType` | string | 是 | 技能类型名，与 `codemod.dll` 里的类名对应 |
| `skillFactory` | `Func<Skill>` | 是 | 返回新技能实例的工厂方法 |
| `defaultParams` | `Dictionary<string, float>` | 否 | 武器默认参数，切换武器时重置 |
| `hooks` | `IWeaponHooks` | 否 | 武器生命周期钩子（见 7.4） |
| `spriteKeys` | `string[]` | 否 | 自定义精灵 key 列表，用于 WorkshopSpriteOverrideDB |
| `unlockHint` | string | 否 | 未解锁时的提示文本 |
| `isUnLocked` | bool | 否 | 默认 true（对玩家开放） |

### 7.4 IWeaponHooks 生命周期钩子

实现 `IWeaponHooks` 接口在武器生命周期的关键时刻插入自定义逻辑。所有方法有默认空实现，只重写关心的部分：

| 方法 | 说明 |
|------|------|
| `OnEquip(int playerIndex)` | 装备此武器时 |
| `OnUnequip(int playerIndex)` | 卸下此武器时 |
| `OnSetSprite(string state, int weaponId) → string?` | 返回非 null 则用该 key 作为精灵，返回 null 继续原版逻辑 |
| `OnAttackOnUnit(UnitObject target, int damage, int distance, int weaponId) → int` | 命中单位时，可修改并返回最终伤害 |
| `OnTrySkipButton(int weaponId) → bool` | 按 E 键时，返回 true 则跳过原版逻辑 |
| `OnTakeDamage(UnitObject atkUnit, int weaponId)` | 受伤后（伤害已处理完毕） |
| `OnDodgeOrParry(bool isParry, int weaponId)` | 闪避（isParry=false）或弹反（isParry=true）后 |

完整带注释的实现示例见 `references/weapon-mod-reference.md`。

### 7.5 武器参数（wp: 前缀能力）

注册武器时通过 `defaultParams` 声明武器的可调参数（如射程、蓄能层数），玩家拾取能力时可通过 `wp:` 前缀修改这些参数：

```csharp
defaultParams: new Dictionary<string, float>
{
    { "fireRange",  4f },
    { "chargeMax",  3f },
}
```

在 `AbilityConfigs/ModSkillConfigs.csv` 中添加对应能力，格式见 `references/ability-csv-reference.md` Section "新增武器专属能力（wp: 前缀）"。

在武器技能逻辑中读取这些参数：

```csharp
var store = BattleObject.Instance.weaponParams;
int range  = store.GetInt("fireRange", 4);    // base + bonus 之和
int charge = store.GetInt("chargeMax", 3);
```

`wp:` 加成写入 `_bonus` 层，切换武器后保留，新局开始时清空。详见 `references/battleobject-variables.md`。

### 7.6 AtkRangeConfig 攻击范围

通过 `profile.primaryAtkRange` 声明主攻击范围，`AtkRangeResolver.Resolve` 在技能中计算格子列表：

```csharp
profile.primaryAtkRange = new AtkRangeConfig
{
    shape         = AtkRangeShape.Line,
    rangeKey      = "fireRange",   // 从 weaponParams 动态读取
    rangeDefault  = 4,
    startOffset   = 1,             // 从玩家前方1格开始
    piercing      = true,
};
```

| 形状 (AtkRangeShape) | 说明 |
|---------------------|------|
| `Line` | 直线 |
| `Wide` | 宽线 |
| `Cross` | 十字 |
| `Ring` | 环形 |
| `Area` | 矩形区域 |
| `Custom` | 自定义，不由 Resolver 处理 |

完整字段说明和使用示例见 `references/weapon-mod-reference.md`。

### 7.7 自定义武器精灵

1. 在 `UnitSprites/` 根目录放武器图标：`weapon1320.png`
2. 在 `UnitSprites/1000/` 放形态精灵（**玩家 ID 固定为 1000**，游戏按单位 ID 查找精灵，因此所有武器的玩家形态精灵都放在 1000/ 目录下，key 名中嵌入武器 ID 避免冲突）：
   ```
   UnitSprites/
     weapon1320.png
     1000/
       default1320.png
       charging11320.png
       firing1320.png
   ```
3. `RegisterWeapon` 时把自定义 key 传入 `spriteKeys`，游戏会将它们注册到 `WorkshopSpriteOverrideDB`，`OnSetSprite` 钩子返回这些 key 时会自动从 Mod 精灵库读取。

### 7.8 完整目录示例

```
MyWeaponMod/
  mod.json
  icon.png
  UnitSprites/
    weapon1320.png          ← 武器图标
    1000/
      default1320.png       ← 默认形态
      charging11320.png     ← 蓄力形态
      firing1320.png        ← 发射形态
  AbilityConfigs/
    ModSkillConfigs.csv     ← wp: 前缀能力
    unit19200.png           ← 能力图标
  CodeMods/
    ChargeCannon/
      codemod.json
      ChargeCannon.dll      ← 包含 Main、Skill_Weapon_ChargeCannon、ChargeCannonHooks
```

---

## 8. 验证与调试

### 8.1 常见问题检查清单

创建 Mod 后，按以下清单检查：

**元数据与结构：**
- [ ] mod.json 在 Mod 根目录下，且**不含 dll/entryClass 字段**（这两个只属于 codemod.json）
- [ ] 图标文件为 `icon.png`（推荐）或 `preview.png`

**精灵替换：**
- [ ] UnitSprites/ 下的目录名是数字（单位 ID），文件名与 sprite key 完全一致（区分大小写）
- [ ] 武器形态精灵放在 `UnitSprites/1000/`（玩家 ID 固定为 1000）

**能力 Mod：**
- [ ] CSV 文件名为 `ModSkillConfigs.csv`（不是 SkillConfigs.csv）
- [ ] 新增能力的 ID > 10000

**代码 Mod：**
- [ ] codemod.json 包含 `dll` 和 `entryClass`，文件名与实际一致
- [ ] 入口类继承 `SimpleModBehaviour`，`OnModUnloaded` 中取消所有事件订阅

**自定义武器：**
- [ ] 武器 ID ≥ 1320
- [ ] `RegisterWeapon` 在 `OnModLoaded` 中调用
- [ ] `spriteKeys` 中的 key 与 `OnSetSprite` 返回的 key 一致
- [ ] `wp:` 能力的参数名与 `defaultParams` 中的 key 拼写一致

### 8.2 日志关键词

在 Unity 日志中搜索以下关键词定位问题：

| 关键词 | 含义 |
|--------|------|
| `[WorkshopModsRuntime]` | Mod 发现与状态管理 |
| `[CodeModRuntime]` | 代码 Mod 加载 |
| `已加载代码 Mod` | 代码 Mod 加载成功 |
| `manifest 缺少 dll 字段` | codemod.json 格式错误 |
| `找不到 dll` | DLL 文件路径不对 |
| `找不到入口类` | entryClass 与实际类名不匹配 |
| `入口类没有继承 SimpleModBehaviour` | 入口类没继承基类 |
| `🔁 覆盖技能 ID=` | 能力覆盖成功 |
| `➕ 新增技能 ID=` | 能力新增成功 |
| `✅ 已加载 Mod 技能表` | Mod 技能表加载成功 |
| `[WeaponModAPI] 注册武器 ID=` | 自定义武器注册成功 |
| `✨ wp: 能力加成` | `wp:` 前缀能力生效 |
| `⚠️ wp: 参数写入失败` | `BattleObject` 未初始化时写入 |

### 8.3 安全提示

**提醒用户：代码 Mod 本质执行第三方代码，只应安装来自信任来源的 Mod。**

---

## 9. 工作流

根据用户意图创建相应的 Mod 结构：
1. **确认 Mod 类型**（换图/加能力/写代码/新武器/混合）
2. **创建目录**并生成 `mod.json`
3. **按类型生成**：精灵替换（提示用户放 PNG）、能力 Mod（生成 CSV 模板）、代码 Mod（生成 codemod.json + C# 模板）、自定义武器（生成完整武器入口模板 + Hooks + Skill + CSV）
4. **验证**：用检查清单审查生成的文件
5. **测试提示**：建议用户在游戏内启用 Mod 测试效果

生成代码 Mod 时，务必确保 `OnModUnloaded` 中取消所有事件订阅（`-=`），避免内存泄漏或崩溃。
