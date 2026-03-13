# 《卡片魔王·只剩个头！》Mod 说明

本游戏当前支持两类 Mod：

1. **图片替换 Mod**
2. **代码 Mod**

如果你只是想替换角色立绘、表情、场景贴图等内容，使用图片替换 Mod 即可。  
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

---
## 4. 代码 Mod

代码 Mod 允许你通过 C# 编写自己的逻辑，并在特定时机执行操作。  
例如：修改初始数值、调用现有方法、扩展部分游戏行为……

### 4.1 文件夹结构

推荐结构如下：

```txt
MyMod/
  mod.json
  preview.png
  CodeMods/            （`CodeMods` 文件夹名称是固定的，用于读取）
    codemod.json       （用于配置dll文件）
    MyCodeMod.dll    
```

### 4.2 `codemod.json` 配置

示例：

```txt
{
  "dll": "MyCodeMod.dll",             （编译生成的 dll 文件名）
  "entryClass": "MyCodeMod.Main"      （入口类的完整名称，即 **命名空间 + 类名**）
}
```

### 4.3 制作流程

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


### 4.4 接口介绍

- [你可以在这里查看一个简单的代码案例：用于实现玩家初始化王城时，将可携带贴纸数量设置为3](./GuideDocument/TestCodeMod.cs)

- [你可以在这里查看部分常用的属性，和他们的含义](./GuideDocument/BattleObject_Mod_Variables.csv)

- 如果你需要更多的文档，api介绍，欢迎私信鱼尾，我会及时补充

---

## 5. 免责声明

本游戏允许玩家通过 Mod 扩展内容，但不保证所有 Mod 之间完全兼容哦~  

代码 Mod 本质上会执行第三方代码，请只安装你信任来源的 Mod~
