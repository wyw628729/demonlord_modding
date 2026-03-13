# 《卡片魔王·只剩个头！》Workshop说明

本游戏当前支持两类 Mod：

1. **图片替换 Mod**
2. **代码 Mod**

如果你只是想换角色贴图、表情、场景图，做图片 Mod 就够了。  
如果你想改一些数据/调用方法，例如“初始贴纸数量更多”“初始攻击力更高”，可以使用代码 Mod

---

## 1. Mod 会从哪里加载？

游戏会扫描并加载：

- **Steam 创意工坊**：你订阅的 Workshop 物品安装目录
- **本地 Mod**：`LocalMods` 文件夹

在游戏内【模组】界面可以对每个 Mod **启用 / 关闭**。

> 部分图片或代码改动可能需要 **重启游戏** 才会完全生效。

---

## 2. 图片替换 Mod
这个mod理论上允许你替换看见到的所有贴图，原理很简单，把对应图片放进对应文件夹即可

### 2.1 文件夹结构
- [单位 ID 与 SpriteKey 对照表：UnitConfig_SpriteKeys.csv](./UnitConfig_SpriteKeys.csv)

推荐结构如下：
```txt
MyMod/  
  mod.json               （对该mod的详情描述，包括名称作者描述等）
  preview.png            （正方形的预览图，建议分辨率 256*256）
  UnitSprites/            (进行图片替换的文件夹名称，固定不变)
    <UnitType>/           (要替换的单位id，纯数字，具体见表格UnitConfig_SpriteKeys.csv，理论上所有你看见的游戏内的单位/场景都能修改)
      <SpriteKey>.png    （要替换的表情key，具体见表格UnitConfig_SpriteKeys.csv）
  ```
> 特殊的，一些没有id的图片（例如色欲挑战的图片）可以直接放在UnitSprites/文件夹下面
> 
> 图片大小一般来说建议128*128，可以根据实际情况调整
> 
> ID和名字不匹配就不会生效。
> 
> 同一张图如果被多个 Mod 替换：后加载的覆盖前加载的。

### 2.2 结构示例
一个示例的结构是：（把露露改成想要的样子）
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
      ....

```

---
## 3. 代码 Mod
这个mod允许你自己写代码并且在某些时机执行某些操作，例如数据修改，调用方法等

### 3.1 文件夹结构
推荐结构如下：
```txt
MyMod/
  mod.json               （对该mod的详情描述，包括名称作者描述等）
  preview.png            （正方形的预览图，建议分辨率 256*256）
  CodeMods/               (进行编码的文件夹名称，固定不变)
    codemod.json          (编译相关的配置)
    TestCodeMod.dll      （编译出来的 dll 文件名）
```

### 3.2 codemod.json 写法
```txt
  "dll": "TestCodeMod.dll", （编译出来的 dll 文件名）
  "entryClass": "TestCodeMod.Main",  （入口类的完整名字 = 命名空间 + 类名）
  "displayName": "Test Code Mod"  （显示名称）
```

### 3.3 最小代码示例

下面这个示例会在进入家园后，把贴纸携带上限改成 3：
```txt
using UnityEngine;

namespace MyCodeMod
{
    public class Main : SimpleModBehaviour
    {
        public override void OnModLoaded()
        {
            BattleObject.OnAfterHomeDataLoad += OnAfterHomeDataLoad;
        }

        public override void OnModUnloaded()
        {
            BattleObject.OnAfterHomeDataLoad -= OnAfterHomeDataLoad;
        }

        private void OnAfterHomeDataLoad(BattleObject bo)
        {
            bo.maxStickerCarry = 3;
            Log("已把 maxStickerCarry 改成 3。");
        }
    }
}
```

### 3.4 如何制作代码 Mod
#### 3.4.1 新建一个 C# 类库工程

推荐使用：Visual Studio 或 Rider

#### 3.4.2 添加引用
代码 Mod 工程至少需要引用你游戏的这两个 dll：
Assembly-CSharp.dll
UnityEngine.CoreModule.dll

通常可以在游戏打包目录中找到：
YourGame_Data/Managed/

#### 3.4.2 编译 dll
编译成功后，你会得到：MyCodeMod.dll

把它和 codemod.json 放进 CodeMods 文件夹即可。

---
## 4.免责声明
代码 Mod 本质上会执行第三方代码。
请只安装你信任来源的 Mod。
使用代码 Mod 可能导致报错、坏档或与其他 Mod 冲突。
