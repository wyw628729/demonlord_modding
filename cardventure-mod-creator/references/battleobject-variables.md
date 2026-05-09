# BattleObject 可修改变量参考

代码 Mod 通过 `BattleObject` 实例访问这些变量。事件回调参数 `bo` 即为 BattleObject 实例。

## 目录

- [Mod 事件接口](#mod-事件接口)
- [玩家局外属性参数](#玩家局外属性参数)
- [玩家局外系统参数](#玩家局外系统参数)
- [玩家局内数据](#玩家局内数据)
- [玩家数据上限](#玩家数据上限)
- [玩家技能参数](#玩家技能参数)
- [临时参数](#临时参数)

---

## Mod 事件接口

这些是静态事件，在代码 Mod 中通过 `+=` 订阅、`-=` 取消订阅。

| 事件 | 类型 | 触发时机 |
|------|------|---------|
| `OnAfterHomeDataLoad` | `Action<BattleObject>` | 玩家回到王城后 |
| `OnLevelStart` | `Action<BattleObject>` | 新关卡开始时 |
| `OnChapterStart` | `Action<BattleObject>` | 新章节开始时 |
| `OnGameStart` | `Action<BattleObject>` | 新一局开始时 |

---

## 玩家局外属性参数

| 变量名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `playerStartAtk` | SafeInt | 3 | 玩家初始攻击力 |
| `playerSkeletonSaveDeadAdd` | SafeInt | | 玩家其他地方拿到的永久复活加成 |
| `playerBookSaveDeadAdd` | SafeInt | | 玩家额外的复活次数 |

> SafeInt 是防作弊的 XOR 加密整数包装器，支持隐式转换为 int，也可以直接赋 int 值。

---

## 玩家局外系统参数

| 变量名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `playerSouls` | SafeInt | | 积攒的魂魄 |
| `termDatas` | List\<termData\> | | 契约数据 |
| `dogDatas` | List\<DogData\> | | 小狗数据 |
| `bossDefeatedID` | List\<int\> | new | 击败的 BOSS ID 列表 |
| `playerFindgoldSkeleton` | List\<int\> | new | 发现的金头骨 ID |
| `weaponDatas` | List\<weaponData\> | new | 武器解锁数据 |
| `weaponParams` | WeaponParamStore | | 武器动态参数存储（见下方"武器参数存储"章节） |
| `currentWeapon` | int | 0 | 携带的武器 ID |
| `homeWeapon` | int | 0 | 最初携带的武器 ID |
| `soulBuffWeapon` | int | 0 | 王城里给武器加的灵魂更多 buff |
| `soulStartBuff` | float | 0 | 开局灵魂加成 |
| `currentBookSkill` | List\<SkillData\> | new | 携带的书本 |
| `maxBookCarry` | int | 1 | 一次最多携带多少本书 |
| `bookDatas` | List\<bookData\> | new | 书本数据 |
| `currentStickerSkill` | List\<SkillData\> | new | 当前贴纸 |
| `defaultSticker` | List\<int\> | new | 默认的贴纸组合 |
| `maxStickerCarry` | int | 1 | 一次最多携带多少贴纸 |
| `stickerDatas` | List\<stickerData\> | new | 贴纸数据 |
| `npcDatas` | List\<npcData\> | new | NPC 数据 |
| `enemyBookDatasID` | List\<int\> | new | 遇见过的敌人图鉴 ID |
| `abilityBookDatasID` | List\<int\> | new | 遇见过的能力图鉴 ID |
| `CGID` | List\<int\> | new | 看过的 CG |
| `skinData` | List\<int\> | new | 已获得的皮肤 |
| `playerNowSkin` | int | 0 | 当前皮肤 |
| `royalSkinData` | List\<int\> | new | 已获得的王城皮肤 |
| `playerNowRoyalSkin` | int | 0 | 当前王城皮肤 |
| `haveGoldChair` | bool | FALSE | 是否解锁金椅子 |
| `haveRainBowChair` | bool | FALSE | 是否解锁炫彩椅子 |
| `haveReadTeach` | List\<int\> | new | 看过的教学 |
| `haveLuLu` | bool | | 是否解锁了露露 |
| `haveMofei` | bool | FALSE | 是否招募了墨菲 |
| `haveMeetLulu` | bool | FALSE | 本局是否遇见露露 |
| `haveMuTou` | bool | FALSE | 是否招募了木偶妹 |
| `haveMeetDragon2` | bool | FALSE | 是否通关第2章 |
| `firstCompleteLevel` | bool | FALSE | 是否通关第3章 |
| `haveGetSeceretMoney` | bool | FALSE | 是否拿到私房钱 |
| `haveTalkDialogue` | List\<string\> | new | 已进行的对话 |
| `haveMeetLuckyHunter` | bool | FALSE | 第1个 boss 是否幸运猎人 |
| `haveMeetYuWave` | bool | FALSE | 第1个 boss 是否鱼尾坦 |
| `donateShop` | SafeInt | 0 | 给商店的捐款 |
| `donateKey` | int | 0 | 给老奶的捐款 |
| `meetMofeiTime` | SafeInt | 0 | 武器锻造次数 |
| `unlockMofeiStickerTime` | SafeInt | 10 | 遇见几次给贴纸 |
| `donateBlood` | int | 0 | 捐赠的血量 |
| `donateBloodGetSticker` | int | 200 | 捐多少血获得贴纸 |
| `haveNoHurtFinish` | bool | FALSE | 是否无伤通关 |
| `dollSkin` | int | 0 | 人偶皮肤 |
| `haveCheatedWord` | List\<string\> | | 已使用的作弊码 |
| `dontMeetCatRun` | int | 0 | 多少局没遇到猫（6局刷1次） |
| `dontMeetPigRun` | int | 0 | 多少局没遇到猪猪存钱罐（10局刷1次） |
| `dontMeetSlimeKing` | int | 0 | 多少局没遇到史莱姆岛（5局刷1次） |
| `deadInCH1Time` | int | 0 | 多少局在第1章就死了 |

---

## 玩家局内数据

| 变量名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `playerAtk` | SafeInt | | 玩家攻击力 |
| `playerMoney` | SafeInt | | 钱（只在战中生效） |
| `playerKey` | SafeInt | | 钥匙（只在战中生效） |
| `playerHurtTimes` | SafeInt | 0 | 受伤次数统计 |
| `playerAllTurnCountBOR` | SafeInt | 0 | 本局总回合数 |
| `playerGetMoneyCountBOR` | SafeInt | 0 | 本局获取金币总数 |
| `playerGetSoulCountBOR` | SafeInt | 0 | 本局获取灵魂总数 |
| `playerGetBoneCountBOR` | SafeInt | 0 | 本局获取骨头总数 |
| `playTime` | double | | 本局游玩时间 |
| `defaultTimeScale` | float | 1 | 默认时间倍率 |
| `saveDead` | SafeInt | 0 | 本局保命次数 |
| `abilityRefresh` | SafeInt | 0 | 本局刷新次数 |
| `playerSkillDatas` | List\<SkillData\> | new | 本局获得的能力 |
| `shurikenNum` | int | 0 | 携带手里剑数量 |
| `haveSkins` | List\<int\> | new | 当前挂载的皮肤 |
| `currentProp` | List\<int\> | new | 当前携带的道具 ID |
| `propNumMaxAdd` | int | 0 | 战中道具栏数量增加 |
| `haveBeatEnemy` | List\<int\> | new | 本局战斗过的敌人 |
| `usedRoomIDs` | List\<int\> | new | 本局经历的房间 |
| `humanSetSeed` | bool | FALSE | 是否人为设定了种子 |
| `mainSeed` | int | 0 | 本局种子 |
| `getDemonSoul` | bool | FALSE | 是否拿到了所有魔神之魂 |
| `currentMode` | BattleMode | | 战斗模式 |

---

## 玩家数据上限

| 变量名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `playerAtkLimit` | int | 19 | 玩家基础攻击力上限 |
| `playerHpLimit` | int | 99 | 玩家血量上限 |
| `playerMoneyLimit` | int | 99 | 玩家金币上限 |
| `playerSoulLimit` | int | 999 | 玩家灵魂上限 |
| `playerPropLimit` | int | 9 | 玩家道具栏位上限 |
| `playerAbilityLimit` | int | 99 | 玩家能力上限 |

---

## 玩家技能参数

最常用的修改目标，影响游戏玩法数值。

### 通用

| 变量名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `stopSleep` | bool | FALSE | 不允许睡觉 |
| `sceneBaseBlackAdd` | float | 0 | 周围黑暗视野 |
| `biggerEnemyRate` | float | 0.01 | 精英怪出现概率 |
| `everyLvDefenceTime` | int | 0 | 每关免伤次数 |
| `cantGetMoney` | bool | FALSE | 不能获得金币 |
| `cantCure` | bool | FALSE | 不能受到治疗 |
| `playerDmg999` | bool | FALSE | 一刀999 |
| `playerAtkAddOnRight` | int | 0 | 对右侧攻击力提升 |
| `playerDmgAddToEnemy` | int | 0 | 对小怪伤害增加 |
| `playerDmgAddToBoss` | int | 0 | 对 boss 伤害增加 |
| `playerCameraSight` | float | 5 | 初始视野 |
| `playerSkillCoolDown` | int | 0 | 减少的技能冷却 |
| `demonRoomRate` | float | 1 | 恶魔房出现概率 |
| `playerDefendFromLeft` | float | 0 | 左侧减伤 |
| `playerAutoDodge` | float | 0 | 自动闪避概率 |
| `firstMustMofei` | bool | FALSE | 开局必定墨菲 |
| `killAllEnvironment` | bool | FALSE | 一击必杀 |
| `mustMeetGoldness` | bool | FALSE | 必定遇见女神房间 |
| `randomNotDead` | bool | FALSE | 50% 概率复活 |
| `cantParryOrDodge` | bool | FALSE | 不能闪避/弹反 |
| `playerDefendInt` | int | 0 | 减伤 |
| `enemyNumRate` | float | 1 | 敌人数量倍率 |
| `enemyHPRate` | float | 1 | 敌人血量倍率 |
| `trapNumRate` | float | 1 | 陷阱数量倍率 |
| `changeMoneyToHp` | bool | FALSE | 金币视为血量 |
| `showEnemyHp` | bool | FALSE | 随时看见敌人血量 |
| `pushFriend` | bool | FALSE | 队友可被推动 |
| `playerCanFly` | bool | FALSE | 玩家可以飞翔 |
| `playerSightAdd` | int | 0 | 额外视野 |
| `playerInvincible` | bool | FALSE | 英雄无敌 |

### 炸弹相关

| 变量名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `bombFireWorkEffect` | bool | FALSE | 炸弹烟花特效 |
| `playerBoomDisAdd` | int | 0 | 炸弹范围额外增加 |
| `playerBoomAimAdd` | int | 0 | 炸弹瞄准时间额外增加 |
| `playerBoomDefend` | int | 5 | 炸弹对友军固定伤害 |
| `playerBombReduce1` | bool | FALSE | 炸弹伤害最高为1 |
| `bombHurtAddAtk` | int | 0 | 炸弹受伤转化为伤害 |
| `bombSpawnAdd` | int | 0 | 炸弹召唤时额外增加 |
| `bombAimEnemy` | bool | FALSE | 炸弹自动向敌人移动 |
| `bombCure` | int | 0 | 炸弹爆炸后吸血 |
| `bombAtkAdd` | int | 0 | 炸弹伤害增加 |
| `bombUnderGround` | bool | FALSE | 炸弹变地雷 |
| `bombAtkByPlayer` | bool | FALSE | 炸弹起爆器 |
| `bombPushDmg` | int | 0 | 炸弹推动伤害增加 |

### 蝙蝠相关

| 变量名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `batShootNum` | int | 1 | 蝙蝠发射数量 |
| `batShootSlant` | bool | FALSE | 蝙蝠散射 |
| `batShootBack` | bool | FALSE | 蝙蝠背向 |
| `batShootDir4` | bool | FALSE | 蝙蝠4向 |
| `batShootDirOnlyUp` | bool | FALSE | 蝙蝠永远向上 |
| `batOverLapNum` | int | 0 | 蝙蝠贯穿次数 |
| `batFlyShort` | bool | FALSE | 蝙蝠飞行短距离 |
| `fatBat` | int | 0 | 胖蝙蝠 |
| `batBurn` | bool | FALSE | 蝙蝠带燃烧 |

### 闪电相关

| 变量名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `lightningNumRange` | float | 1 | 闪电数量百分比波动上限 |
| `lightningHugeAdd` | float | 0 | 闪电数量额外+20的概率 |
| `lightningNumAdd` | int | 0 | 闪电数量增加 |
| `lightningAtkDis` | int | 5 | 闪电覆盖范围 |
| `lightningAutoAimNum` | int | 10 | 默认多少道闪电自瞄1次 |
| `lightningAutoAim` | bool | FALSE | 是否自瞄 |
| `lightningAimOne` | bool | FALSE | 闪电劈向1个地方 |
| `lightningKillRate` | float | 0 | 闪电概率对小怪造成999伤害 |
| `lightningHitMore` | int | 0 | 闪电击中后额外召唤数量 |
| `lightningDamgeAdd` | int | 0 | 闪电伤害增加 |
| `lightningGetShruiken` | int | 0 | 闪电收集手里剑 |

### 召唤/友军相关

| 变量名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `friendNumMax` | int | 5 | 友军数量上限 |
| `friendHPmaxAdd` | int | 0 | 友军生命值额外增加 |
| `friendAtkAdd` | int | 0 | 友军攻击力额外增加 |
| `friendDeadSpawnBomb` | int | 0 | 友军死亡召唤炸弹 |
| `friendHitCure` | int | 0 | 友军打中回血 |
| `hurtGiveFriend` | bool | FALSE | 受伤送给队友 |
| `friendHurtByBomb` | int | 0 | 友军被炸弹攻击回血 |
| `friendBurnCure` | int | 0 | 友军燃烧回血 |

### 燃烧相关

| 变量名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `burnInfectRate` | float | 0.01 | 燃烧传递概率 |
| `burnTime` | int | 10 | 燃烧时间 |
| `burnDamage` | int | 1 | 燃烧伤害 |
| `playerRebornInFire` | bool | FALSE | 浴火重生 |
| `burnDamageWhenHpLow` | int | 0 | 低血量燃烧伤害增加 |
| `burnNoDmg` | bool | FALSE | 燃烧单位不受其他伤害 |
| `fireEndExplode` | bool | FALSE | 燃烧结束后爆炸 |
| `burnDeadSpawnApple` | int | 0 | 燃烧死亡召唤苹果 |

### 无敌相关

| 变量名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `invincibleHurtBack` | float | 0 | 无敌时受伤返还倍数 |
| `invincibleHalf` | bool | FALSE | 无敌改为伤害减半 |
| `invincibleTimeAdd` | int | 0 | 无敌时间额外增加 |
| `invincibleAutoParry` | bool | FALSE | 无敌时自动弹反 |
| `invincibleAtkAdd` | int | 0 | 无敌时攻击力增加 |
| `unInvincibleAtkAdd` | int | 0 | 不无敌时攻击力增加 |
| `invincibleHurtCure` | int | 0 | 无敌时治疗自己 |
| `invincibleAllFriend` | int | 0 | 所有队友也无敌 |

### 手里剑相关

| 变量名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `shurikenSpawnFreezeRate` | float | 0 | 手里剑附带冰冻概率 |
| `shurikenNumAdd` | int | 0 | 手里剑产量增加 |
| `shurikenAtk` | int | 2 | 手里剑伤害 |
| `shurikenHelpGet` | bool | FALSE | 其他单位帮忙拾取 |
| `shurikenBackRate` | float | 0 | 丢出后返还百分比 |
| `shurikenGetDis` | int | 0 | 吸取手里剑范围 |
| `shurikenAtkMore` | int | 0 | 手里剑攻击范围 |
| `shurikenGetMore` | int | 0 | 超过20后屯更多 |
| `shurikenAtkFriend` | int | 0 | 攻击友军加攻击力 |

### 炮台相关

| 变量名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `cannonChargeNum` | int | 3 | 充能次数 |
| `cannonShootNum` | int | 1 | 每次发射子弹数量 |
| `cannonAtkDis` | int | 4 | 索敌范围 |
| `cannonDullTime` | int | 3 | 冷却时间 |
| `cannonHpMax` | int | 30 | 炮台血量 |
| `cannonDeadSpawnBullet` | int | 0 | 炮台死亡发射子弹 |
| `cannonReborn` | bool | FALSE | 炮台死亡50%复活 |
| `cannonAutoCure` | int | 0 | 炮台每回合回血 |
| `cannonHitAtk` | int | 0 | 被攻击发射子弹 |
| `cannonBulletScale` | int | 1 | 炮台子弹大小 |

### 环境/表情/Emoji

| 变量名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `emojiPosChange` | bool | FALSE | 改变 emoji 位置 |
| `emojiScale` | int | 2 | emoji 大小 |
| `emojiEffectNum` | int | 0 | emoji 效果加成 |
| `emojiAllHappy` | bool | FALSE | emoji 都会天使 |
| `emojiDuartion` | int | 20 | emoji 持续时间 |
| `emojiNumAdd` | int | 0 | 召唤表情数量+1 |
| `emojiSaveDead` | bool | FALSE | 表情替死 |
| `emojiDemonFreeze` | bool | FALSE | 冰冻恶魔 |
| `environReborn` | bool | FALSE | 场景概率复活 |
| `environDmgAdd` | float | 0 | 场景伤害提升 |
| `environHPAdd` | float | 0 | 场景血量提升 |
| `environCanPush` | bool | FALSE | 推动环境 |
| `environAtkTogether` | bool | FALSE | 相邻环境一起攻击 |
| `environAppleAddHPMax` | bool | FALSE | 多余苹果转化生命上限 |
| `environTreeManRate` | float | 0.01 | 召唤树精概率 |
| `environTreeManFriend` | bool | FALSE | 树精是队友 |
| `environSaveDead` | bool | FALSE | 环境替死 |

---

## 临时参数

| 变量名 | 类型 | 默认值 | 说明 |
|--------|------|--------|------|
| `playerObject` | UnitObjectPlayer | | 玩家对象 |
| `murder` | int | 1000 | |
| `autoPlay` | bool | FALSE | 挂机模式 |
| `superAutoPlay` | bool | FALSE | 超级挂机模式 |
| `openTreasureNum` | int | 0 | 开宝箱次数（每关清空） |
| `bossHurtTime` | int | 0 | Boss 受伤次数 |
| `haveCommentThisRun` | bool | FALSE | 本局是否已评价 |

---

## 武器参数存储（WeaponParamStore）

`BattleObject.Instance.weaponParams` 是武器动态参数的双层存储，供代码 Mod 和 `wp:` 前缀能力使用。

### 层级说明

| 层 | 名称 | 何时重置 | 用途 |
|----|------|---------|------|
| `_base` | 基础层 | 切换武器时（重置为 Profile 默认值） | 武器自身参数 |
| `_bonus` | 加成层 | 新局开始时（`ResetAll`） | 能力给的永久加成 |

`Get(key)` 返回两层之和。

### 常用方法

```csharp
var wp = BattleObject.Instance.weaponParams;

// 读取（base + bonus 之和，key 不存在时返回 defaultValue）
float range = wp.Get("fireRange", 4f);
int charge = wp.GetInt("chargeMax", 3);
bool pierce = wp.GetBool("piercing", false);

// 写入基础层（通常在武器技能内使用）
wp.Set("chargeMax", 5f);

// 写入加成层（通常在 wp: 能力或代码 Mod 中使用）
wp.Add("fireRange", 1f);  // 累加，不覆盖

// 切换武器时会自动调用，通常无需手动调用
wp.ResetToDefaults(profile);  // 重置 _base 为 Profile 默认值，保留 _bonus

// 新局开始时会自动调用，通常无需手动调用
wp.ResetAll();  // 清空两层
```

### 在代码 Mod 中修改武器参数

```csharp
public override void OnModLoaded()
{
    BattleObject.OnAfterHomeDataLoad += OnAfterHomeDataLoad;
}

private void OnAfterHomeDataLoad(BattleObject bo)
{
    // 给镰刀的 wp 加成（这个 bonus 在切换武器后仍保留）
    bo.weaponParams.Add("scytheRange", 2f);
}
```

> **注意**：`weaponParams` 与 `BattleObject` 实例关联，游戏结束或新局时会重置。代码 Mod 建议通过 `OnGameStart` 事件在新局开始时重新写入，避免残留上局数据。
