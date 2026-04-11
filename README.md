# 《卡片魔王·只剩个头！》Mod 说明

> [English Version](./README_EN.md)


本游戏当前支持两类 Mod：

1. **图片替换 Mod**
2. **能力 Mod**
3. **代码 Mod**

如果你只是想替换角色立绘、表情、场景贴图等内容，使用图片替换 Mod 即可。  
如果你想替换已有能力，或者自己新增能力，可以使用能力 Mod。
如果你想修改数据、注册事件、调用游戏中的方法，则需要使用代码 Mod。

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

- `mod.json`  
  Mod 的基础信息文件，用于填写名称、作者、描述等内容。

- `preview.png`  
  Mod 预览图。建议使用正方形图片，推荐分辨率为 `256×256`。

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


## 4. 能力 Mod（替换 / 自创能力）

你现在可以通过 Mod：覆盖游戏中已有的能力、新增一个全新的能力

实现方式很简单：
只需要在 Mod 里提供一个 AbilityConfigs 文件夹，里面放一个 ModSkillConfigs.csv，再放若干个能力图标即可。

### 4.1 文件夹结构

推荐结构如下：

```txt
MyAbilityMod/
  mod.json
  preview.png
  AbilityConfigs/   （`AbilityConfigs` 文件夹名称是固定的，用于读取）
    ModSkillConfigs.csv   （`ModSkillConfigs.csv` 文件名称是固定的，用于读取）
    unit19001.png   （能力的图标，图标文件建议命名为 unit技能ID.png）
    unit19002.png
```

> - 例如，如果你新增了能力 ID 是 19001，并且你在 AbilityConfigs 文件夹里放了这张图：unit19001.png，游戏就会自动把它当作这个能力的图标读取。


### 4.2 工作方式

游戏会先读取原版能力表，然后再读取你 Mod 里的 AbilityConfigs/ModSkillConfigs.csv。

> 规则如下：
> - 如果 Mod 中的能力 ID 已经存在：会覆盖原版能力
> - 如果 Mod 中的能力 ID 不存在：会新增一个能力
> - 为了避免冲突，建议大家新增能力时尽量使用一个较大的新 ID，比如大于10000

### 4.3 CSV 格式

能力 Mod 使用的字段，和游戏原版 SkillConfig 完全一致。

你可以直接参考：
- [示例ModSkillConfigs文件：ModSkillConfigs.csv](./GuideDocument/AbilityConfigs/ModSkillConfigs.csv)

并且我也会附上的当前本地 **SkillConfigs.csv** 供你参考：
- [本地的SkillConfigs文件：SkillConfigs.csv](./GuideDocument/SkillConfigs.csv)

你只需要按照原表的格式填写即可。

### 4.5 能力字段说明

这里不会把所有字段讲得特别复杂，你可以直接对照本地完整的 SkillConfigs.csv示例文件来看。

> 基础字段
- **id**：能力的唯一编号。（重复已有 ID = 覆盖原能力，使用新 ID = 新增能力）
- **type**：能力类型，也就是这个能力具体会做什么，例如：生成炸弹（spawnBomb）、召唤闪电（spawnLightning）、改变参数变量（passive）、other等
-**trigger**：触发时机，也就是这个能力在什么情况下走冷却、触发效果，比如移动时（move）、关卡开始时（levelStart）、按道具键时（activeSkill）等
- **cooldownNum**：冷却次数，达到对应次数后，能力才会执行，默认是0。
- **参数字段**：
-- paramName1 / param1
-- paramName2 / param2
-- paramName3 / param3
-- 这几组字段是能力的具体参数。
-- 不同 type 会读取不同参数，所以你通常需要参考原版 SkillConfigs.csv 中同类能力的写法。
- **持续时间字段**：有些能力会用到持续时间（例如魔神真言能力），有些则不会，如果不需要可以默认
- **name**：能力名字
- **description**：能力描述
- **poolType**：所属能力池 / 流派池，比如炸弹流是1200，不同流派、武器、彩色能力等通常都和它有关
- **abilityLevel**：稀有度等级，默认是1
- **chooseMaxTime**：这个能力最多能被选择多少次，默认是无穷
- **isBase**：是否是某个流派的基础能力，如果不是，那么就需要玩家选到这个流派的基础能力，才会投放它，默认是TRUE
- **武器通用能力**：这里特指最初幻想武器能否随机到这个锻造能力，默认是FALSE
- **isInBook**：是否会加入图鉴，默认是TRUE
- **是否正常投放**：字面意思，能否正常投放，默认是TRUE


### 4.6 推荐做法

如果你是第一次尝试能力 Mod，建议这样开始：

> - 先复制一个示例 AbilityConfigs 文件夹
> - 先试着覆盖一个原版能力
> - 确认生效后，再尝试新增一个能力
> - 最后再给它加上自己的图标




---
## 5. 代码 Mod

代码 Mod 允许你通过 C# 编写自己的逻辑，并在特定时机执行操作。  
例如：修改初始数值、调用现有方法、扩展部分游戏行为……

### 5.1 文件夹结构

推荐结构如下：

```txt
MyMod/
  mod.json
  preview.png
  CodeMods/            （`CodeMods` 文件夹名称是固定的，用于读取）
    codemod.json       （用于配置dll文件）
    MyCodeMod.dll    
```

### 5.2 `codemod.json` 配置

示例：

```txt
{
  "dll": "MyCodeMod.dll",             （编译生成的 dll 文件名）
  "entryClass": "MyCodeMod.Main"      （入口类的完整名称，即 **命名空间 + 类名**）
}
```

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



## 6. 免责声明

本游戏允许玩家通过 Mod 扩展内容，但不保证所有 Mod 之间完全兼容哦~  

代码 Mod 本质上会执行第三方代码，请只安装你信任来源的 Mod~
