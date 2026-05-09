# 能力 CSV 字段参考

## CSV 列定义（按位置）

游戏解析器按列位置读取，不依赖表头文本。以下是 20 列的完整定义：

| 列号 | 字段名 | 类型 | 说明 | 默认值 |
|------|--------|------|------|-------|
| 0 | id | int | 能力唯一 ID。重复 ID 覆盖原版，新 ID 新增能力 | 必填 |
| 1 | type | string | 能力类型，决定能力做什么 | 必填 |
| 2 | trigger | string | 触发时机，决定什么时候走冷却和执行效果 | 必填 |
| 3 | cooldown | int | 冷却次数，达到后能力执行 | 0 |
| 4 | paramName1 | string | 参数1名称 | 空 |
| 5 | param1 | float | 参数1值 | 0 |
| 6 | paramName2 | string | 参数2名称 | 空 |
| 7 | param2 | float | 参数2值 | 0 |
| 8 | paramName3 | string | 参数3名称 | 空 |
| 9 | param3 | float | 参数3值 | 0 |
| 10 | durationTime | string | 持续时间（部分能力用到） | 空 |
| 11 | name | string | 能力名称 | 必填 |
| 12 | description | string | 能力描述 | 必填 |
| 13 | poolType | int | 所属能力池/流派 | 9999 |
| 14 | abilityLevel | int | 稀有度等级 | 1 |
| 15 | chooseMaxTime | int | 最多选择次数 | 999(无限) |
| 16 | allWeaponUse | bool | 最初幻想武器能否随机到该锻造能力 | FALSE |
| 17 | isBase | bool | 是否流派基础能力 | TRUE |
| 18 | isInBook | bool | 是否加入图鉴 | TRUE |
| 19 | isUnLocked | bool | 是否正常投放 | TRUE |

---

## 能力类型 (type)

不同 type 读取不同参数组，创建能力时参考原版 SkillConfigs.csv 中同类能力的参数写法。

### 常用 type

| type | 说明 | 典型参数 |
|------|------|---------|
| `spawnBomb` | 生成炸弹 | paramName1=炸弹类型, param1=数量 |
| `spawnBat` | 生成蝙蝠 | paramName1=蝙蝠类型, param1=数量 |
| `spawnLightning` | 召唤闪电 | paramName1留空, param1=闪电数量 |
| `spawn` | 召唤单位 | paramName1=spawnUnit, param1=单位ID, paramName2留空, param2=数量 |
| `passive` | 改变参数变量 | paramName1=变量名, param1=变量值 |
| `buff` | 增益效果 | paramName1=效果类型, param1=数值 |
| `other` | 其他特殊效果 | 视具体实现 |
| `ability` | 获取/改变能力 | paramName1=能力相关参数 |
| `selfUnit` | 对自身单位操作 | paramName1=操作类型 |
| `showUI` | 显示UI | paramName1=UI类型 |
| `emoji` | 召唤表情 | paramName1=表情参数 |
| `changeCamp` | 改变阵营 | paramName1=阵营参数 |
| `player` | 玩家操作 | paramName1=操作类型 |

### 武器 type

| type | 武器 |
|------|------|
| `Weapon_Strike` | 突刺类武器 |
| `Weapon_Laser` | 激光类武器 |
| `Weapon_Punch` | 拳击类武器 |

---

## 触发器 (trigger)

| trigger | 说明 |
|---------|------|
| `move` | 移动时 |
| `atk` | 攻击时 |
| `hurt` | 受伤时 |
| `turn` | 回合开始时 |
| `levelStart` | 关卡开始时 |
| `levelStartFront` | 关卡开始前 |
| `unitDead` | 单位死亡时 |
| `environmentDead` | 环境物件被破坏时 |
| `activeSkill` | 按主动技能键时 |
| `isOnce` | 仅触发一次 |
| `onBorn` | 出生时 |
| `firstTurn` | 首回合时 |
| `onDead` | 自身死亡时 |
| `onOverLap` | 重叠时 |
| `onPlayerOverLap` | 与玩家重叠时 |
| `onGroundOverLap` | 落地时 |
| `hurtByPlayer` | 被玩家攻击时 |
| `hurtByBomb` | 被炸弹攻击时 |
| `propUse` | 使用道具时 |
| `dodge` | 闪避时 |
| `parry` | 弹反时 |
| `chargeAtk` | 蓄力攻击时 |
| `onDoorDead` | 门被破坏时 |
| `onRightInput` | 右侧输入时 |
| `bossLvStart` | Boss关开始时 |
| `enemyDead` | 敌人死亡时 |
| `lightSpawn` | 闪电生成时 |
| `lightningOnHit` | 闪电命中时 |
| `friendDead` | 友军死亡时 |
| `batDead` | 蝙蝠死亡时 |
| `bombOver` | 炸弹爆炸时 |
| `playerReborn` | 玩家复活时 |
| `getInvincible` | 获得无敌时 |
| `resourceLap` | 资源重叠时 |
| `onDropInWater` | 落水时 |
| `atkOnUnit` | 攻击单位时 |
| `atkOnTree` | 攻击树时 |

---

## 能力池/流派 (poolType)

| poolType | 流派 |
|----------|------|
| 1200 | 炸弹流 |
| 1201 | 蝙蝠流 |
| 1202 | 闪电流 |
| 1203 | 召唤流 |
| 1204 | 燃烧流 |
| 1205 | 手里剑流 |
| 1206 | 道具流 |
| 1207 | 环境/树木流 |
| 1208 | 无敌流 |
| 1210 | 史莱姆炮流 |
| 1299 | 通用/稀有 |
| 1298 | 能力操控 |
| 1297 | 护盾/铠甲 |
| 1300-1318 | 各武器专属锻造能力 |
| 1501 | 消耗品道具 |
| 1401 | 章节实验能力 |
| 7 | 贴纸 |

### 复合 poolType

部分能力属于两个流派，poolType 写成两个数字拼接，如 `12001202` = 炸弹流 + 闪电流。

---

## CSV 示例

### 新增闪电能力

```csv
能力ID,类型,触发器,冷却,参数名1,参数1,参数名2,参数2,参数名3,参数3,持续时间,名字,描述,所属流派,稀有度,可选次数,武器通用能力,是否基础能力,是否进图鉴,是否正常投放
19100,spawnLightning,hurt,0,,10,,,,,,复仇闪电,受伤时召唤10道闪电,1202,2,,,,,
```

### 覆盖原有召唤能力

```csv
9150,spawn,atk,20,spawnUnit,2,,1,,,,魔王兵,攻击20次召唤1个弓箭手,1203,1,,,,,
```

### 新增被动参数修改

```csv
19001,passive,levelStart,0,maxStickerCarry,1,,,,,,贴纸扩展,开局时贴纸携带上限+1,1299,1,,,,,
```

### 新增武器专属能力（wp: 前缀）

`wp:` 前缀是专为自定义武器设计的参数修改机制。用 `passive` 类型搭配 `wp:<参数名>` 作为 paramName，可以在玩家拾取能力时向武器的 `weaponParams` 加成，而不影响 BattleObject 的全局参数。

> 这需要游戏中注册了对应的 Mod 武器（通过 `WeaponModAPI.RegisterWeapon`）。原版武器不读 `weaponParams`，所以 `wp:` 前缀对原版武器无效。

```csv
能力ID,类型,触发器,冷却,参数名1,参数1,参数名2,参数2,参数名3,参数3,持续时间,名字,描述,所属流派,稀有度,可选次数,武器通用能力,是否基础能力,是否进图鉴,是否正常投放
19200,passive,isOnce,0,wp:fireRange,1,,,,,,增程炮管,蓄能炮射程+1,1320,2,,,,,
19201,passive,isOnce,0,wp:chargeMax,1,,,,,,超级电池,蓄能炮最大蓄能层数+1,1320,1,,,,,
```

`wp:` 后接武器 Profile 中 `defaultParams` 定义的参数名（如 `fireRange`、`chargeMax`）。加成写入 `_bonus` 层，切换武器后仍保留，新局开始时清空。

---

## 制作建议

1. 新增能力使用 ID > 10000 避免与原版冲突
2. 不同 type 读取不同参数，务必参考原版 SkillConfigs.csv 中同类能力的写法
3. 先覆盖一个原版能力确认格式正确，再尝试新增
4. CSV 中空行不影响解析但建议删除
5. 提供图标 `unit<能力ID>.png` 可让能力有自定义图标，否则使用原版默认图标
