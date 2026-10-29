# ⚔️ Epic Valheim Additions

### Community Rebuild for Valheim 1.0

**Current release:** [2.2.7](https://github.com/cjayride/EpicValheimsAdditions/releases/tag/v2.2.7) · **Source:** [github.com/cjayride/EpicValheimsAdditions](https://github.com/cjayride/EpicValheimsAdditions)

A community rebuild of **Huntardy's Epic Valheim Additions**, updated for Valheim 1.0.

This version brings back **optional extra bosses, golden minibosses, and optional biome creatures** while removing outdated weapons, ores, and vegetation. Vanilla Ashlands and Deep North world generation are not overwritten.

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

Spawns use the original EVA chance and caps. Turn a toggle **off** and run `EVA_cleanup confirm` to remove existing instances of those creatures.

Ursar **taming** from original EVA is not included — only world spawns.

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
- Overrides to vanilla Ashlands and Deep North **world generation** (trees, veins, etc.)

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

Run these as the **host** or a player on the server **admin list**. Enable cheats/devcommands in the in-game console (F5) when prompted.

On a **dedicated server** there is no keyboard on the server process itself. From **2.2.7** onward, an admin connected from a client can run `EVA_cleanup` / `EVA_upgrade` in F5; the mod forwards them to the server. Your Steam ID must be on that world’s `adminlist.txt`, and this mod must be installed on **both** server and client.

For other console work on dedicated servers (including Upgrade World’s own commands), install [Server devcommands](https://thunderstore.io/c/valheim/p/JereKuusela/Server_devcommands/) on the **server** so admins can run authorized commands while connected.

| Command | Description |
|---|---|
| `EVA_cleanup` | Preview only: explains what `confirm` would remove. **Does not add** altars or locations. |
| `EVA_cleanup confirm` | Removes **retired** EVA world objects (old ores, vegetation, stations, items, etc.) and **creatures whose toggles are off**. **Keeps** bosses, minibosses, altars, and vegvisirs. |
| `EVA_upgrade` | Backfills **missing** EVA altars, vegvisirs, and golden miniboss sites on **already-generated** terrain. **Requires** [Upgrade World](https://thunderstore.io/c/valheim/p/JereKuusela/Upgrade_World/) on the server. |

### Missing altars or vegvisirs?

- **`EVA_cleanup` will not fix missing locations.** It only strips legacy EVA content from the rebuild; it never places altars.
- **`EVA_upgrade` is the EVA command for missing sites.** It runs Upgrade World’s `locations_add` for EVA altars, vegvisirs, and golden miniboss locations (`StaminaGreydwarf`, `StaminaTroll`, `StaminaWraith`).
- **Upgrade World must be installed on the server** or `EVA_upgrade` only prints a reminder. Without it, EVA locations still appear in **newly generated** zones via Jotunn, but old explored chunks are not backfilled.
- After `EVA_upgrade`, travel and load biomes so zones can update; Upgrade World works over time—it does not spawn an altar at your feet.
- Check `Huntard.EpicValheimsAdditions_Content.json`: if a boss toggle is **off** (`MistlandsBoss`, `AshlandsBoss`, `DeepNorthBoss`, `GoldenMinibosses`), that content is not registered until the toggle is on and the server restarts.

When Upgrade World is present, `EVA_cleanup confirm` may also run a targeted `vegetation_reset` for **retired** EVA plants (cosmetic vegetation that is not a simple ZDO may still need Upgrade World separately).

### Companion mods (strongly recommended on dedicated servers)

| Mod | Why |
|---|---|
| [Upgrade World](https://thunderstore.io/c/valheim/p/JereKuusela/Upgrade_World/) | Required for `EVA_upgrade`, optional vegetation reset during `EVA_cleanup confirm`, and general world maintenance. **Server-side.** |
| [Server devcommands](https://thunderstore.io/c/valheim/p/JereKuusela/Server_devcommands/) | Lets admins run console commands (including Upgrade World) while connected to a dedicated server. **Server-side.** |

### Cleaning bad or dead objects (Upgrade World workflow)

On long-lived servers, admins often run Upgrade World’s maintenance sequence to remove broken or orphaned world objects—for example:

```
devcommands
upgrade
clean_objects
start
```

You **absolutely need both** [Upgrade World](https://thunderstore.io/c/valheim/p/JereKuusela/Upgrade_World/) and [Server devcommands](https://thunderstore.io/c/valheim/p/JereKuusela/Server_devcommands/) on the **dedicated server** for that workflow. `devcommands` alone is not enough: `upgrade` / `clean_objects` / `start` are Upgrade World commands.

That sequence is **general server hygiene**, not a replacement for `EVA_cleanup confirm`. Use **`EVA_cleanup confirm`** for retired EVA prefabs after the rebuild; use **Upgrade World** (with or without `EVA_upgrade`) for missing locations and dead-object cleanup across the whole world.

`EVA_cleanup` is optional and intended for existing worlds that previously spawned content removed by this rebuild.

## 🔧 Building from Source

1. Copy `Environment.props.sample` to `Environment.props` and set Valheim + Jotunn paths.
2. `git lfs pull` (required for `EpicValheimsAdditions.eva_assets`).
3. `dotnet build EpicValheimsAdditions.csproj -c Release`

Thunderstore packaging files live in `Package/`. See [CHANGELOG.md](CHANGELOG.md) for release history.

## 🙏 Credits

Original author and contributors:

Huntard, Horem, blaxxun, Azumatt, AeehyehssReeper, Furok, thedefside, Belasias, and everyone credited on the original EVA project page.

This community rebuild updates compatibility and content scope for Valheim 1.0. It does not claim ownership of the original mod, bosses, assets, or artwork.

---

*Return to battle. Challenge new bosses. Keep your vanilla world intact.*
