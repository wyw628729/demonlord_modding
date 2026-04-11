# Cardventure: Just a Head - Mod Guide

> [中文版本](./README.md)


## Table of Contents

- [1. Where are Mods loaded from?](#1-where-are-mods-loaded-from)
- [2. Basic Mod Folder Structure](#2-basic-mod-folder-structure)
- [3. Sprite Replacement Mod](#3-sprite-replacement-mod)
- [4. Ability Mod](#4-ability-mod)
- [5. Code Mod](#5-code-mod)
- [6. Disclaimer](#6-disclaimer)

This game currently supports **3 types of Mods**:

1. **Sprite Replacement Mod**
2. **Ability Mod**
3. **Code Mod**

- If you only want to replace character sprites, expressions, or scene textures, use a Sprite Replacement Mod.
- If you want to modify existing abilities or create new ones, use an Ability Mod.
- If you want to modify data, register events, or call in-game methods, you will need a Code Mod.

---

## 1. Where are Mods loaded from?

The game will scan and load Mods from the following locations:

- **Steam Workshop**: Mods you have subscribed to
- **Local Mods**: the `LocalMods` folder  
  (usually located at `AppData\LocalLow\YuWave\DemonLordJustABlock\LocalMods`)

You can **enable / disable** each Mod in the in-game **Mod menu**.

---

## 2. Basic Mod Folder Structure

- [You can view a simple Mod example here](./TestModExample)

A complete Mod folder typically looks like this:

```txt
MyMod/
  mod.json
  preview.png
  UnitSprites/        (optional: sprite replacement)
  CodeMods/           (optional: code mod)
  AbilityConfigs/     (optional: ability mod)
```

### 2.1 Required Files

- `mod.json`  
  Contains basic Mod information such as name, author, and description.

- `preview.png`  
  Preview image of the Mod.  
  Recommended: square image, 256×256 resolution.

A Mod can contain only sprites, only code, or a combination of both.

---

## 3. Sprite Replacement Mod

The principle is simple:  
Place PNG files with the correct names into the specified folder, and the game will override the original assets.

---

## 4. Ability Mod

You can now use Mods to:

- Override existing abilities
- Create completely new abilities

---

## 5. Code Mod

Code Mods allow you to write custom logic in C#.

---

## 6. Disclaimer

This game allows Modding, but compatibility between Mods is not guaranteed.
