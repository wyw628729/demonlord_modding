# 《卡片魔王·只剩个头！》Workshop / Mod 图片替换说明

本游戏当前支持 **图片替换 Mod**。  
Mod 的原理很简单：把同名 PNG 放到指定文件夹 → 游戏扫描并覆盖原图片。

---

## 1. Mod 会从哪里加载？

游戏会扫描并加载：
- **Steam 创意工坊**：你订阅的 Workshop 物品安装目录

在游戏内【模组】界面可以对每个 Mod **一键启用/关闭**。

> 部分图片可能因为缓存需要 **切换场景或重启游戏** 才完全生效。
---

## 2. Mod 文件夹结构

推荐结构：

```txt
MyMod/
  UnitSprites/
    <UnitType>/
      <SpriteKey>.png
```

`<UnitType>`：单位类型（数字，比如 1001、2003，具体见附带的单位id表格，理论上所有你看见的游戏内的单位都能修改）

`<SpriteKey>`：图片 key（具体见附带的常用key名称）

示例：
```txt
MyMod/
  UnitSprites/
    1001/
      default.png
      move.png
```

## 3. 命名规则

文件名 = 覆盖 key
名字不匹配就不会生效。

同一张图如果被多个 Mod 替换：后加载的覆盖前加载的。
