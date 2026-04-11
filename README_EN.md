# Cardventure: Just a Head - Mod Guide

> [中文版本](./README.md)


## Table of Contents

- [1. Where are Mods loaded from?](#1-where-are-mods-loaded-from)
- [2. Basic Mod Folder Structure](#2-basic-mod-folder-structure)
- [3. Sprite Replacement Mod](#3-sprite-replacement-mod)
- [4. Ability Mod](#4-ability-mod)
- [5. Code Mod](#5-code-mod)
- [6. Disclaimer](#6-disclaimer)

This game currently supports 3 types of Mods:

1. **Sprite Replacement Mod**
2. **Ability Mod**
3. **Code Mod**

- If you only want to replace character sprites, expressions, scene textures, and similar content, a Sprite Replacement Mod is enough.
- If you want to replace existing abilities or create your own new abilities, you can use an Ability Mod.
- If you want to modify data, register events, or call methods in the game, you will need a Code Mod.

---

## 1. Where are Mods loaded from?

The game will scan and load Mods from the following locations:

- **Steam Workshop**: the Workshop items you have subscribed to
- **Local Mods**: the `LocalMods` folder (usually located at `AppData\LocalLow\YuWave\DemonLordJustABlock\LocalMods`)

You can **enable / disable** each Mod in the in-game **Mods** menu.

---

## 2. Basic Mod Folder Structure

- [You can see a simple Mod example here](./TestModExample)

A complete Mod folder usually looks like this:

```txt
MyMod/
  mod.json
  preview.png
  UnitSprites/        (optional: Sprite Replacement Mod)
  CodeMods/           (optional: Code Mod)
  AbilityConfigs/     (optional: Ability Mod)
```

### 2.1 Required Files

- `mod.json`  
  The basic information file for the Mod, used to fill in the name, author, description, and other details.

- `preview.png`  
  The preview image for the Mod. A square image is recommended, with a suggested resolution of `256×256`.

A Mod can contain only sprite replacements, only code, or both at the same time.

---

## 3. Sprite Replacement Mod

The principle of a Sprite Replacement Mod is very simple:  
put PNG files with the correct names into the specified folder, and the game will override the original images after scanning them.

In theory, as long as the corresponding resource key exists in the game, most visible sprites can be replaced.

### 3.1 Folder Structure

- [Unit ID and SpriteKey Reference: UnitConfig_SpriteKeys.csv](./GuideDocument/UnitConfig_SpriteKeys.csv)

The recommended structure is:

```txt
MyMod/
  mod.json
  preview.png
  UnitSprites/           (`UnitSprites` is the fixed folder name used for loading)
    <UnitType>/          (unit ID, usually a number)
      <SpriteKey>.png    (the image key name, which must exactly match the key name in the table)
```

> Notes:
> - Recommended image format: `PNG`
> - Recommended size: you can usually start from `128×128` and adjust according to the actual asset

### 3.2 Special Images

Some images do not belong to a specific unit ID, such as certain standalone UI images or special event images.  
These images can be placed directly in the root folder of `UnitSprites/`, using the corresponding resource name as the filename.

For example, to replace all Lulu-related images (including the Lust challenge):

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

## 4. Ability Mod

You can now use a Mod to override existing abilities in the game or add completely new abilities.

The implementation is simple:  
you only need to provide an `AbilityConfigs` folder in your Mod, put a `ModSkillConfigs.csv` file inside it, and then add several ability icons if needed.

### 4.1 Folder Structure

The recommended structure is:

```txt
MyAbilityMod/
  mod.json
  preview.png
  AbilityConfigs/             (`AbilityConfigs` is the fixed folder name used for loading)
    ModSkillConfigs.csv       (`ModSkillConfigs.csv` is the fixed file name used for loading)
    unit19001.png             (the icon of the ability; the icon filename is recommended to be unit<AbilityID>.png)
    unit19002.png
```

- For example, if you add a new ability with ID `19001`, and you put `unit19001.png` inside the `AbilityConfigs` folder, the game will automatically read it as the icon of that ability.

### 4.2 How It Works

The game will first load the original ability table, and then load your Mod’s `AbilityConfigs/ModSkillConfigs.csv`.

The rules are:

- If the ability ID in the Mod already exists, it will override the original ability.
- If the ability ID in the Mod does not exist, it will add a new ability.
- To avoid conflicts, it is recommended to use a relatively large new ID when adding abilities, for example, greater than `10000`.

### 4.3 CSV Format

The fields used by an Ability Mod are exactly the same as the game’s original `SkillConfig`.

You can directly refer to:

- [Example ModSkillConfigs file: ModSkillConfigs.csv](./GuideDocument/AbilityConfigs/ModSkillConfigs.csv)

I will also provide the current local **SkillConfigs.csv** for reference:

- [Local SkillConfigs file: SkillConfigs.csv](./GuideDocument/SkillConfigs.csv)

You only need to fill it in according to the original table format.

### 4.5 Ability Field Explanation

I will not make every field overly complicated here. You can directly compare them with the complete local `SkillConfigs.csv` example file.

- **id**: The unique ID of the ability. (Using an existing ID = override the original ability; using a new ID = add a new ability)
- **type**: The ability type, meaning what the ability actually does. For example: spawning bombs (`spawnBomb`), summoning lightning (`spawnLightning`), changing parameter variables (`passive`), `other`, etc.
- **trigger**: The trigger timing, meaning under what condition this ability counts cooldown and activates its effect, such as when moving (`move`), when a level starts (`levelStart`), or when pressing the active-skill key (`activeSkill`).
- **cooldown**: The cooldown count. The ability will execute only after the corresponding count is reached. The default is `0`.
- **parameters**:
- - `paramName1 / param1`
- - `paramName2 / param2`
- - `paramName3 / param3`
- - These groups of fields are the actual parameters of the ability.
- - Different `type` values read different parameters, so you usually need to refer to abilities of the same type in the original `SkillConfigs.csv`.
- **durationTime**: Some abilities use a duration (for example, the Demon God True Word ability), while others do not. If it is not needed, you can leave it at the default.
- **name**: The name of the ability
- **description**: The description of the ability
- **poolType**: The ability pool / school it belongs to. For example, the bomb school is `1200`. Different schools, weapons, colorful abilities, and so on are usually related to this field.
- **abilityLevel**: The rarity level. The default is `1`.
- **chooseMaxTime**: The maximum number of times this ability can be selected. By default, it is unlimited.
- **allWeaponUse**: This specifically refers to whether the Original Fantasy weapon can randomly obtain this forge ability. The default is `FALSE`.
- **isBase**: Whether it is a base ability of a certain school. If not, then the player must first obtain that school’s base ability before this one can appear. The default is `TRUE`.
- **isInBook**: Whether it will be added to the collection book. The default is `TRUE`.
- **isUnLocked**: Whether it is normally available for distribution. The default is `TRUE`.

### 4.6 Recommended Workflow

If this is your first time trying an Ability Mod, it is recommended to start like this:

- First copy an example `AbilityConfigs` folder
- First try overriding an original ability
- After confirming that it works, try adding a new ability
- Finally, add your own icon for it

---

## 5. Code Mod

A Code Mod allows you to write your own logic in C# and execute it at specific times.  
For example: modifying initial values, calling existing methods, expanding some gameplay behavior, and so on.

### 5.1 Folder Structure

The recommended structure is:

```txt
MyMod/
  mod.json
  preview.png
  CodeMods/            (`CodeMods` is the fixed folder name used for loading)
    codemod.json       (used to configure the dll file)
    MyCodeMod.dll    
```

### 5.2 `codemod.json` Configuration

Example:

```txt
{
  "dll": "MyCodeMod.dll",             (the compiled dll filename)
  "entryClass": "MyCodeMod.Main"      (the full name of the entry class, that is, **namespace + class name**)
}
```

### 5.3 Workflow

First, create a new C# Class Library project in Visual Studio.  
Then add the following references to the Code Mod project:

- `Assembly-CSharp.dll`
- `UnityEngine.CoreModule.dll`

You can usually find them in the game directory:

```txt
DemonLordJustABlock_Data/Managed/
```

After compiling successfully, you will get a file like this:

```txt
MyCodeMod.dll
```

Put it together with `codemod.json` into the `CodeMods` folder for testing.

It is recommended to test with a Local Mod first, and after confirming that it works correctly, organize it into a Workshop version.

### 5.4 Interface Introduction

- [You can view a simple code example here: it sets the number of stickers the player can carry to 3 when initializing the Demon Castle](./GuideDocument/TestCodeMod.cs)

- [You can view some commonly used properties and their meanings here](./GuideDocument/BattleObject_Mod_Variables.csv)

- If you need more documentation or API information, feel free to message Yuwei, and I will add more in time.

---

## 6. Disclaimer

This game allows players to expand content through Mods, but full compatibility between all Mods is not guaranteed.

Code Mods essentially execute third-party code, so please only install Mods from sources you trust.
