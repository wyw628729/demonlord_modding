# Demon Lord: Just a Block! Mod Guide

> [中文说明 / Chinese Version](./README.md)

This game currently supports two types of mods:

1. **Image Replacement Mods**
2. **Code Mods**

If you only want to replace character portraits, expressions, background textures, or similar visual assets, an image replacement mod is enough.  
If you want to modify data, register events, or call in-game methods, you will need a code mod.

---

## 1. Where are mods loaded from?

The game scans and loads mods from the following locations:

- **Steam Workshop**: the Workshop items you have subscribed to
- **Local Mods**: the `LocalMods` folder (usually located at `AppData\LocalLow\YuWave\DemonLordJustABlock\LocalMods`)

You can **enable / disable** each mod from the in-game **Mods** menu.

---

## 2. Basic mod folder structure

- [You can view a simple mod example here](./TestModExample)

A complete mod folder usually looks like this:

```txt
MyMod/
  mod.json
  preview.png
  UnitSprites/        (optional: image replacement mod)
  CodeMods/           (optional: code mod)
```

### 2.1 Required files

- `mod.json`  
  The basic information file for the mod, used to define the name, author, description, and other metadata.

- `preview.png`  
  The preview image for the mod. A square image is recommended, with a suggested resolution of `256×256`.

A mod can contain only image replacements, only code, or both.

---

## 3. Image replacement mods

Image replacement mods work in a very simple way:  
place PNG files with the correct names into the designated folders, and the game will scan and override the original images.

In theory, as long as the corresponding asset key exists in the game, most visible textures can be replaced.

### 3.1 Folder structure

- [Unit ID and SpriteKey reference table: UnitConfig_SpriteKeys.csv](./GuideDocument/UnitConfig_SpriteKeys.csv)

Recommended structure:

```txt
MyMod/
  mod.json
  preview.png
  UnitSprites/           (`UnitSprites` is a fixed folder name used for loading)
    <UnitType>/          (unit ID, usually numeric)
      <SpriteKey>.png    (the image key name; it must exactly match the key name in the table)
```

> Note:
> - Recommended image format: `PNG`
> - Recommended size: you can usually start from `128×128` and adjust based on the actual asset

### 3.2 Special images

Some images do not belong to a specific unit ID, such as certain standalone UI graphics or special event images.  
These can be placed directly in the root directory of `UnitSprites/`, using the corresponding asset name as the filename.

For example, to replace all Lulu-related images (including the Lust Challenge):

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

## 4. Code mods

Code mods allow you to write your own logic in C# and execute it at specific times.  
For example: modifying initial values, calling existing methods, or extending certain in-game behaviors.

### 4.1 Folder structure

Recommended structure:

```txt
MyMod/
  mod.json
  preview.png
  CodeMods/            (`CodeMods` is a fixed folder name used for loading)
    codemod.json       (used to configure the DLL file)
    MyCodeMod.dll
```

### 4.2 `codemod.json` configuration

Example:

```txt
{
  "dll": "MyCodeMod.dll",             (the name of the compiled DLL file)
  "entryClass": "MyCodeMod.Main"      (the full name of the entry class, i.e. **namespace + class name**)
}
```

### 4.3 Workflow

First, create a new C# Class Library project in Visual Studio.  
Then add the following references to your code mod project:

- `Assembly-CSharp.dll`
- `UnityEngine.CoreModule.dll`

You can usually find them in the game directory here:

```txt
DemonLordJustABlock_Data/Managed/
```

After compiling successfully, you will get a file like this:

```txt
MyCodeMod.dll
```

Place it together with `codemod.json` in the `CodeMods` folder to test it.

It is recommended to test with a local mod first. After confirming everything works correctly, you can then package it into a Workshop version.

### 4.4 Interface references

- [You can view a simple code example here: when the player initializes the Demon King Castle, set the number of stickers they can carry to 3](./GuideDocument/TestCodeMod.cs)

- [You can view some commonly used properties and their meanings here](./GuideDocument/BattleObject_Mod_Variables.csv)

- If you need more documentation or API references, feel free to message 鱼尾 and I will add them as soon as possible

---

## 5. Disclaimer

This game allows players to expand its content through mods, but full compatibility between all mods is not guaranteed.  

Code mods execute third-party code by nature, so please only install mods from sources you trust.
