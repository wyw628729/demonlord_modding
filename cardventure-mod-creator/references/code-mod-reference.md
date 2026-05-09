# 代码 Mod 详细参考

## 目录

- [BattleObject 事件](#battleobject-事件)
- [代码 Mod 完整示例](#代码-mod-完整示例)
- [制作流程](#制作流程)
- [调试技巧](#调试技巧)

---

## BattleObject 事件

游戏提供了 4 个静态事件供代码 Mod 订阅，在特定游戏生命周期触发：

| 事件 | 类型 | 触发时机 |
|------|------|---------|
| `BattleObject.OnAfterHomeDataLoad` | `Action<BattleObject>` | 玩家回到王城后（每次返回王城触发） |
| `BattleObject.OnLevelStart` | `Action<BattleObject>` | 新关卡开始时（每个关卡开始前） |
| `BattleObject.OnChapterStart` | `Action<BattleObject>` | 新章节开始时（每章开始前） |
| `BattleObject.OnGameStart` | `Action<BattleObject>` | 新一局开始时（新游戏或死亡重开） |

### 订阅方式

```csharp
public override void OnModLoaded()
{
    // 订阅事件
    BattleObject.OnAfterHomeDataLoad += OnAfterHomeDataLoad;
    BattleObject.OnGameStart += OnGameStart;
}

public override void OnModUnloaded()
{
    // 必须取消订阅，否则引用泄漏
    BattleObject.OnAfterHomeDataLoad -= OnAfterHomeDataLoad;
    BattleObject.OnGameStart -= OnGameStart;
}

private void OnAfterHomeDataLoad(BattleObject bo)
{
    bo.maxStickerCarry = 3;
    Log("贴纸携带上限改为 3");
}

private void OnGameStart(BattleObject bo)
{
    bo.playerStartAtk = 5;
    Log("开局攻击力改为 5");
}
```

### 可修改的变量

所有可修改的 BattleObject 变量及说明见 `battleobject-variables.md`。

---

## 代码 Mod 完整示例

以下是一个完整的代码 Mod 示例，展示了事件订阅、变量修改、日志输出和清理：

```csharp
using UnityEngine;

namespace MyCodeMod
{
    public class Main : SimpleModBehaviour
    {
        public override void OnModLoaded()
        {
            Log("Mod 加载成功！");
            
            // 订阅事件
            BattleObject.OnAfterHomeDataLoad += OnAfterHomeDataLoad;
            BattleObject.OnGameStart += OnGameStart;
        }

        public override void OnModUnloaded()
        {
            // 必须取消订阅，防止内存泄漏
            BattleObject.OnAfterHomeDataLoad -= OnAfterHomeDataLoad;
            BattleObject.OnGameStart -= OnGameStart;
            
            Log("Mod 已卸载。");
        }

        private void OnAfterHomeDataLoad(BattleObject bo)
        {
            // 回到王城时修改贴纸携带上限
            bo.maxStickerCarry = 3;
            Log("已把贴纸携带上限改为 3。");
        }

        private void OnGameStart(BattleObject bo)
        {
            // 新局开始时修改初始攻击力
            bo.playerStartAtk = 5;
            Log("新局开始，初始攻击力改为 5。");
        }
    }
}
```

对应的 `codemod.json`：

```json
{
  "dll": "MyCodeMod.dll",
  "entryClass": "MyCodeMod.Main",
  "displayName": "My Cool Mod"
}
```

---

## 制作流程

### 1. 创建项目

使用 Visual Studio（或 Rider）创建 C# 类库项目（.NET Framework 4.7.2 或更高）。

### 2. 添加引用

在项目中添加以下 DLL 引用（位于游戏安装目录 `DemonLordJustABlock_Data/Managed/`）：

- `Assembly-CSharp.dll`（游戏程序集，包含 BattleObject、SimpleModBehaviour 等）
- `UnityEngine.CoreModule.dll`（Unity 核心模块）

### 3. 创建入口类

创建一个继承 `SimpleModBehaviour` 的类作为入口点：

```csharp
using UnityEngine;

namespace MyMod
{
    public class Main : SimpleModBehaviour
    {
        public override void OnModLoaded()
        {
            // 初始化代码
            Log("Mod loaded!");
        }

        public override void OnModUnloaded()
        {
            // 清理代码
        }
    }
}
```

### 4. 编译生成 DLL

在 Visual Studio 中点击"生成 → 生成解决方案"，在 `bin/Debug/` 或 `bin/Release/` 目录下找到生成的 DLL。

### 5. 创建 codemod.json

在 DLL 所在目录创建 `codemod.json`：

```json
{
  "dll": "MyMod.dll",
  "entryClass": "MyMod.Main",
  "displayName": "My Awesome Mod"
}
```

其中 `entryClass` 是 `命名空间.类名` 格式。

### 6. 打包并测试

将 DLL 和 `codemod.json` 一起放入 Mod 目录：

```
MyMod/
  mod.json
  icon.png
  CodeMods/
    MyMod/
      codemod.json
      MyMod.dll
```

启动游戏，在【模组】界面启用 Mod，查看 Unity 日志确认加载成功。

---

## 调试技巧

### 查看日志

Unity 日志文件位置：
- Windows: `C:\Users\<用户名>\AppData\LocalLow\YuWave\DemonLordJustABlock\Player.log`

搜索关键词：
- `[CodeMod:` — 你的 Mod 日志（通过 `Log()` 输出）
- `[CodeModRuntime]` — 代码 Mod 加载系统日志
- `已加载代码 Mod` — 加载成功
- `找不到 dll` / `找不到入口类` — 配置错误

### 常见问题

**Q: 加载失败，提示"找不到入口类"？**

A: 检查 `codemod.json` 中的 `entryClass` 是否与实际类的完整名称一致（包括命名空间）。

**Q: 修改 BattleObject 变量无效？**

A: 确认变量在正确的时机修改（如 `playerStartAtk` 应在 `OnGameStart` 中修改，而不是 `OnModLoaded`）。

**Q: Mod 卸载后游戏崩溃？**

A: 检查 `OnModUnloaded` 中是否取消了所有事件订阅（`-=`）。

### 进阶调试

可以使用 [dnSpy](https://github.com/dnSpy/dnSpy) 附加到游戏进程进行断点调试（需要将 DLL 编译为 Debug 模式并保留 .pdb 文件）。
