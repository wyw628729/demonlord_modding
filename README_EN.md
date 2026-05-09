# Cardventure: Just a Head - Mod Guide

> [中文版本](./README.md)


## Table of Contents

- [1. Where are Mods loaded from?](#1-where-are-mods-loaded-from)
- [2. Basic Mod Folder Structure](#2-basic-mod-folder-structure)
- [3. Sprite Replacement Mod](#3-sprite-replacement-mod)
- [4. Ability Mod](#4-ability-mod)
- [5. Code Mod](#5-code-mod)
- [6. Custom Weapon Mod](#6-custom-weapon-mod)
- [7. Troubleshooting](#7-troubleshooting)
- [8. Disclaimer](#8-disclaimer)

This game currently supports 4 types of Mods:

1. **Sprite Replacement Mod**
2. **Ability Mod**
3. **Code Mod**
4. **Custom Weapon Mod**

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

#### mod.json

The basic information file for the Mod, used to fill in the name, author, description, and other details.

```json
{
  "title": "My Mod",
  "description": "Mod description",
  "author": "Author name",
  "version": "1.0.0"
}
```

**Field Description:**

| Field | Required | Description |
|-------|----------|-------------|
| `title` | Recommended | Mod title. If missing, falls back to `name` field or directory name |
| `name` | Fallback | Fallback field for `title`, lower priority than `title` |
| `description` | Optional | Mod description |
| `author` | Recommended | Author name |
| `authorName` | Fallback | Fallback field for `author`, lower priority than `author` |
| `version` | Optional | Version number |

> **Important Reminder**: Do NOT include `dll` or `entryClass` fields in mod.json! The game does not read Code Mod configuration from mod.json. Code Mod configuration belongs in `CodeMods/codemod.json`.

#### Mod Icon

Mod icon files are placed in the Mod root directory. The game loads them in the following priority order:

1. `icon.png` (highest priority, recommended)
2. `preview.png`
3. `thumbnail.png`
4. `cover.png`

Recommended: use square images, `256×256` resolution, PNG format.

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

### 4.4 Ability Field Explanation

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

### 4.5 Recommended Workflow

If this is your first time trying an Ability Mod, it is recommended to start like this:

- First copy an example `AbilityConfigs` folder
- First try overriding an original ability
- After confirming that it works, try adding a new ability
- Finally, add your own icon for it

---

## 5. Code Mod

A Code Mod allows you to write your own logic in C# and execute it at specific times.  
For example: modifying initial values, calling existing methods, expanding some gameplay behavior, and so on.

> **Advanced Example: Custom Weapon Mod**  
> If you want to create a complete custom weapon with its own skill, exclusive abilities, and independent sprites, refer to [WeaponModExample](./WeaponModExample).  
> This example demonstrates how to use `WeaponModAPI` to register weapons, implement weapon hooks, configure attack ranges, create weapon-exclusive abilities, and other advanced techniques.

### 5.1 Folder Structure

Code Mods support two placement methods:

**Method 1: Directly in CodeMods root directory** (suitable for single Code Mod)

```txt
MyMod/
  mod.json
  icon.png
  CodeMods/            (`CodeMods` is the fixed folder name used for loading)
    codemod.json       (used to configure the dll file)
    MyCodeMod.dll    
```

**Method 2: Using subdirectories** (recommended, supports multiple Code Mods in one Mod)

```txt
MyMod/
  mod.json
  icon.png
  CodeMods/
    MyCodeMod/         (Code Mod package directory)
      codemod.json
      MyCodeMod.dll
    AnotherCodeMod/    (another Code Mod)
      codemod.json
      AnotherCodeMod.dll
```

### 5.2 `codemod.json` Configuration

Example:

```json
{
  "dll": "MyCodeMod.dll",
  "entryClass": "MyCodeMod.Main",
  "displayName": "My Cool Mod"
}
```

**Field Description:**

| Field | Required | Description |
|-------|----------|-------------|
| `dll` | Yes | DLL filename (relative to codemod.json directory) |
| `entryClass` | Yes | Full entry class name (namespace.classname) |
| `displayName` | No | Display name, used for logging and GameObject naming. Falls back to class name if not set |

> **Note**: Also supports using `code_mod.json` as the filename, but `codemod.json` is recommended for consistency.

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

## 6. Custom Weapon Mod

Custom Weapon Mods are an advanced application of Code Mods, allowing you to create complete weapons with independent skill systems, exclusive abilities, and custom sprites.

### 6.1 Features

- **Complete weapon system**: Register new weapon ID, display name, skill type
- **Lifecycle hooks**: Equip/unequip, sprite switching, damage modification, hurt/dodge/parry events
- **Attack range configuration**: Various attack shapes (line, fan, circle), supports piercing, reverse shooting, etc.
- **Weapon-exclusive abilities**: Use `wp:` prefix to create abilities that only affect this weapon
- **Dynamic parameter system**: Weapon parameters persist when switching weapons, reset when starting a new run

### 6.2 Complete Example

[WeaponModExample](./WeaponModExample) provides a complete charge cannon weapon example, including:

- Weapon registration and skill system
- Lifecycle hook implementation (sprite switching, damage bonus)
- Attack range configuration (line piercing shot)
- Weapon-exclusive abilities (range +1, max charge layers +1)
- Complete directory structure and compilation instructions

See [WeaponModExample/README_WeaponMod.md](./WeaponModExample/README_WeaponMod.md) for details

### 6.3 Core API

**Registering a weapon:**

```csharp
WeaponModAPI.RegisterWeapon(
    id:           1320,                          // Weapon ID (≥ 1320 to avoid conflicts)
    displayName:  "Charge Cannon",               // Display name
    skillType:    "Weapon_ChargeCannon",         // Skill type name
    skillFactory: () => new Skill_Weapon_ChargeCannon(),
    defaultParams: new Dictionary<string, float>
    {
        { "fireRange",  4f },                    // Default range
        { "chargeMax",  3f },                    // Default max charge layers
    },
    hooks:        new ChargeCannonHooks(),       // Lifecycle hooks
    spriteKeys:   new[] { "charging11320", "firing1320" },
    unlockHint:   "From Charge Cannon Mod",
    isUnLocked:   true
);
```

**Lifecycle hooks interface (`IWeaponHooks`):**

```csharp
public interface IWeaponHooks
{
    void OnEquip(int playerIndex);                              // When equipped
    void OnUnequip(int playerIndex);                            // When unequipped
    string OnSetSprite(string state, int weaponId);             // Sprite switching
    int OnAttackOnUnit(UnitObject target, int damage, int distance, int weaponId);  // Attack damage modification
    bool OnTrySkipButton(int weaponId);                         // E key skip behavior
    void OnTakeDamage(UnitObject atkUnit, int weaponId);        // When taking damage
    void OnDodgeOrParry(bool isParry, int weaponId);            // When dodging/parrying
}
```

**Attack range configuration:**

```csharp
profile.primaryAtkRange = new AtkRangeConfig
{
    shape         = AtkRangeShape.Line,          // Shape: line
    rangeKey      = "fireRange",                 // Read range from weaponParams
    rangeDefault  = 4,                           // Default range 4 tiles
    startOffset   = 1,                           // Start from 1 tile in front of player
    piercing      = true,                        // Piercing attack
};
```

**Weapon-exclusive abilities (CSV configuration):**

```csv
id,type,trigger,cooldown,paramName1,param1,paramName2,param2,paramName3,param3,durationTime,name,description,poolType,abilityLevel,chooseMaxTime,allWeaponUse,isBase,isInBook,isUnLocked
19200,passive,isOnce,0,wp:fireRange,1,,,,,,Extended Barrel,Charge Cannon range +1,1320,2,,,,,
19201,passive,isOnce,0,wp:chargeMax,1,,,,,,Super Battery,Charge Cannon max charge +1,1320,1,,,,,
```

> **`wp:` prefix rules**:  
> - Modifies `weaponParams` (weapon dynamic parameters)
> - Persists when switching weapons
> - Cleared when starting a new run
> - Set poolType to weapon ID (e.g., `1320`) to limit to this weapon only

### 6.4 Sprite Resources

Custom weapon sprites are placed in the `UnitSprites/1000/` directory (**player ID is fixed at 1000**):

```txt
WeaponModExample/
  UnitSprites/
    weapon1320.png              ← Weapon icon
    1000/                       ← Player form sprites (ID fixed at 1000)
      default1320.png           ← Default form
      charging11320.png         ← Charging form
      firing1320.png            ← Firing form
```

**Naming convention**: Embed weapon ID in sprite key (e.g., `charging11320`) to avoid sprite conflicts between different weapons.

### 6.5 Best Practices

1. **Use weapon ID ≥ 1320**: Avoid conflicts with vanilla weapons (1300-1318)
2. **Refer to the complete example**: [WeaponModExample](./WeaponModExample) contains all necessary code and configuration
3. **Test locally first**: Place the Mod in `LocalMods/` for testing before publishing to the Workshop
4. **Check logs**: Game logs are located at `AppData\LocalLow\YuWave\DemonLordJustABlock\Player.log` for debugging

---

## 7. Troubleshooting

### TypeLoadException: DefaultInterpolatedStringHandler

**Error message:**

```
[CodeModRuntime] Load failed
System.TypeLoadException: Could not resolve type with token 01000011 from typeref
(expected class 'System.Runtime.CompilerServices.DefaultInterpolatedStringHandler'
in assembly 'System.Runtime, Version=8.0.0.0, ...)
```

**Cause:**  
The Code Mod project targets `net8.0` (or any other .NET 6+), but Unity runs on **Mono**, whose compatibility layer is equivalent to .NET Standard 2.1.  
When targeting .NET 6+, C# 10 automatically optimizes `$"..."` string interpolation to use `DefaultInterpolatedStringHandler` — a type that does not exist in Unity's Mono runtime, causing the DLL to fail at load time.

**Fix:**  
Set the `.csproj` target framework to `netstandard2.1`, lower the language version to `9`, and remove `<ImplicitUsings>`:

```xml
<!-- Before (incorrect) -->
<PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <LangVersion>10</LangVersion>
    <ImplicitUsings>enable</ImplicitUsings>
</PropertyGroup>

<!-- After (correct) -->
<PropertyGroup>
    <TargetFramework>netstandard2.1</TargetFramework>
    <LangVersion>9</LangVersion>
</PropertyGroup>
```

After recompiling, the DLL will be fully compatible with Unity Mono.

---

## 8. Disclaimer

This game allows players to expand content through Mods, but full compatibility between all Mods is not guaranteed.

Code Mods essentially execute third-party code, so please only install Mods from sources you trust.
