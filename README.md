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
- [单位 ID 与 SpriteKey 对照表：UnitConfig_SpriteKeys.csv](./UnitConfig_SpriteKeys.csv)

推荐结构如下：
```txt
MyMod/
  UnitSprites/            (进行图片替换的文件夹名称，固定不变)
    <UnitType>/           (要替换的单位id，纯数字，具体见表格UnitConfig_SpriteKeys.csv，理论上所有你看见的游戏内的单位/场景都能修改)
      <SpriteKey>.png    （要替换的表情key，具体见表格UnitConfig_SpriteKeys.csv）
  preview.png            （正方形的预览图，建议分辨率 256*256）
  ```
> 特殊的，一些没有id的图片（例如色欲挑战的图片）可以直接放在UnitSprites/文件夹下面
> 
> 图片大小一般来说建议128*128，可以根据实际情况调整
> 
> ID和名字不匹配就不会生效。
> 
> 同一张图如果被多个 Mod 替换：后加载的覆盖前加载的。


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

