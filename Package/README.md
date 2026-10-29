# ⚔️ Epic Valheim Additions

### Community Rebuild for Valheim 1.0

A community rebuild of **Huntardy's Epic Valheim Additions**, updated for Valheim 1.0.

This version brings back **optional extra bosses and golden minibosses** while removing outdated content and preserving vanilla Ashlands and Deep North.

> **Important:** This is not the original 2.1.2 biome overhaul. All original bosses, assets, and artwork belong to their respective creators.

## 🔗 Original Mod

- **Thunderstore:** [Huntardy's Epic Valheim Additions](https://thunderstore.io/c/valheim/p/Huntardys/EpicValheimsAdditions/)
- **Nexus Mods:** [Epic Valheim Additions](https://www.nexusmods.com/valheim/mods/1475)

**Original Author:** Huntard

## ✨ Features

### 👑 Additional Bosses

Three optional bosses return, each with an altar and vegvisir in its associated biome.

- **Svartalfar Queen** — Mistlands
- **Hel Demon (Blazing Damned One)** — Ashlands
- **Jotunn** — Deep North

### 🐾 Optional Biome Creatures (default on)

| Toggle | Creatures |
|--------|-----------|
| `DeepNorthCreatures` | Frostwing Fae, Crystalhide Ursar (+ cub) |
| `AshlandsCreatures` | Magma Skarab, Infernal Minotaur |
| `OceanCreatures` | Draugr Mariner (after Jotunn is defeated — requires world key `Killed_Jotunn`) |

Spawns use the original EVA chance and caps. Turn a toggle off and run `EVA_cleanup confirm` to remove existing instances.

### 🪙 Golden Minibosses

- Golden miniboss encounters in the Meadows, Black Forest, and Swamp.
- Altars use their matching vanilla trophies.
- Configurable boss health, attack values, and content availability.
- Default altar cooldown of **120 real-time minutes** after a golden miniboss is defeated.
- Cooldown can be customized or disabled.

### ⚖️ Boss Difficulty

Default boss balance is designed for groups of **3–4 players**.

Difficulty progression:

**Mistlands → Ashlands → Deep North**

Boss health and attack values can be customized through JSON configuration files.

## 🗑️ Removed Content

To keep this rebuild focused and preserve vanilla biome content, the following features have been removed:

- Weapons and ores
- Vegetation
- Thor's Forge
- Overrides to vanilla Ashlands and Deep North content

Existing worlds may retain objects created by previous versions. See the cleanup commands below if you want to remove retired EVA objects.

## 📦 Requirements

- **Valheim 1.0**
- [BepInExPack Valheim](https://thunderstore.io/c/valheim/p/denikson/BepInExPack_Valheim/)
- [Jötunn 2.30.2 or newer](https://thunderstore.io/c/valheim/p/ValheimModding/Jotunn/)

## 📥 Installation

1. Install BepInExPack Valheim and Jötunn.
2. Install this community rebuild on **both your client and dedicated server**.
3. Remove the original `Huntardys-EpicValheimsAdditions` package to prevent duplicate EVA DLLs.
4. Launch the game to generate the configuration files.
5. Configure boss availability and difficulty as desired.

**Plugin GUID:** `Huntard.EpicValheimsAdditions`

The original plugin GUID is retained, so existing JSON configurations remain compatible.

## 🗿 Summoning Recipes

All summon items use vanilla crafting stations.

| Summon Item | Crafting Station | Materials |
|---|---|---|
| **Cursed Effigy** | Galdr table | 30 Yggdrasil Wood, 20 Black Marble, 1 Seeker Queen trophy |
| **Ymir's Soul Essence** | Black Forge | 4 Frost Cores, 20 Ice, 1 Barka trophy |
| **Fenrir's Heart** | Black Forge | 10 Flametal, 8 Asksvin hide, 1 Fader trophy |

Golden miniboss altars continue to use their corresponding vanilla trophies.

### Golden Miniboss Cooldown

The default cooldown is **120 real-time minutes** after a kill.

Configuration key: `GoldenMinibossCooldownMinutes`

- Set a positive value to customize the cooldown.
- Set `0` to disable the cooldown.

## ⚙️ Configuration

Configuration files are located in:

`BepInEx/config/`

| File | Purpose |
|---|---|
| `Huntard.EpicValheimsAdditions.json` | Boss health and attack values |
| `Huntard.EpicValheimsAdditions_Content.json` | Bosses, creatures, golden minibosses, cooldown |

Boss toggles: `MistlandsBoss`, `AshlandsBoss`, `DeepNorthBoss`, `GoldenMinibosses`

Creature toggles (default **true**): `DeepNorthCreatures`, `AshlandsCreatures`, `OceanCreatures`

### Custom Loot

**Custom drop tables are not included in this DLL.** Use [Drop That](https://thunderstore.io/c/valheim/p/ASharpPen/Drop_That/) or a similar mod to configure custom loot.

## 🛠️ Host & Admin Commands

Run these commands as the host or an administrator.

| Command | Description |
|---|---|
| `EVA_cleanup` | Starts the cleanup workflow for retired EVA world objects. |
| `EVA_cleanup confirm` | Confirms removal of retired EVA world objects. |
| `EVA_upgrade` | Adds retained EVA locations if [Upgrade World](https://thunderstore.io/c/valheim/p/JereKuusela/Upgrade_World/) is installed. |

Cleanup is optional and intended for existing worlds that previously spawned content removed by this rebuild.

## 🙏 Credits

Original author and contributors:

Huntard, Horem, blaxxun, Azumatt, AeehyehssReeper, Furok, thedefside, Belasias, and everyone credited on the original EVA project page.

This community rebuild updates compatibility and content scope for Valheim 1.0. It does not claim ownership of the original mod, bosses, assets, or artwork.

---

*Return to battle. Challenge new bosses. Keep your vanilla world intact.*