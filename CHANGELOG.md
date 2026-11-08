# Changelog

## 2.2.12

- EVA boss altars (Svartalfar Queen, Hel Demon, Jotunn) show the required offering and count on hover.
- Golden miniboss altar hover uses the same `Offer: N× item` format.

## 2.2.11

- Golden miniboss aura is dimmed (`GoldenAuraIntensity`, default `0.35`; `1` is original brightness).
- Golden altar trophy costs are configurable. Defaults: Greydwarf **10**, Troll **6**, Wraith **8**
  (prefab value was 4). Existing worlds pick up the new counts on altar load.
- Golden miniboss altars show required trophy count on hover (e.g. `8× Wraith trophy`).

## 2.2.10

- Built against the current Steam Valheim public branch (1.0.21, build 26249040). Harmony targets
  `OfferingBowl.InitiateSpawnBoss` and `Character.OnDeath` are still present.
- Faster load: Harmony only patches EVA cooldown types (not the whole assembly), bundle asset names
  are cached, and disabled creature packs are not loaded into Jotunn.
- Less hitching: admin RPC wait uses a coroutine instead of a permanent `Update()`, golden-miniboss
  death checks use prefab hashes, and altar lookup no longer scans every `OfferingBowl` in the scene.

## 2.2.9

- Assembly metadata is now generated from the project version, so the DLL's `FileVersion` /
  `AssemblyVersion` report 2.2.9 (2.2.7/2.2.8 builds reported a stale 2.2.7 file version).
  No functional change from 2.2.8.

## 2.2.8

- Admin command RPC handlers are now registered from the plugin's own `Update()` once
  `ZRoutedRpc` exists. The 2.2.7 harmony patch on `ZNet.Awake` is removed, so the mod no longer
  runs inside the game's network-startup path (the code path every client join goes through).
  Commands, bosses, creatures, and assets are unchanged.

## 2.2.7

- `EVA_cleanup` and `EVA_upgrade` run on dedicated servers when an admin uses the in-game console while connected (RPC to server).

## 2.2.6

- Restored optional EVA biome creatures with config toggles (default **on**): `DeepNorthCreatures`, `AshlandsCreatures`, `OceanCreatures`.
- Spawns match original EVA rates (chance, caps, intervals). Draugr Mariner still requires the `Killed_Jotunn` world key.
- `EVA_cleanup confirm` removes creature instances when their toggle is off.

## 2.2.5

- Startup log uses the real plugin version.
- Missing nested attack prefabs are treated as optional (no crash).
- Black Forge summon recipes use the `blackforge` station prefab (`YmirsSoulEssence`, `FenrirsHeart`).
- Galdr table summon uses `piece_magetable` (`CursedEffigy`).

## 2.2.3 / 2.2.4

- Boss HP tuned for 3–4 player groups (Queen 24k, Hel Demon 45k, Jotunn 100k).
- Deep North is the hardest EVA boss; Ashlands sits between Mistlands and Deep North.

## 2.2.2

- Golden miniboss coin payouts raised for a ~120 minute altar cooldown (per player).

## 2.2.1

- Golden miniboss altar cooldown after a kill (`GoldenMinibossCooldownMinutes`, default 120).
- Auto-migrate boss JSON when Jotunn HP is inflated or biome difficulty order is wrong.

## 2.2.0

- Valheim 1.0 / Jotunn 2.30 rebuild.
- Keeps only the three EVA bosses, three golden minibosses, altars, and vegvisirs.
- Removes EVA weapons, ores, vegetation, extra creatures, and vanilla biome overwrites.
- Vanilla late-game summon recipes.
- `EVA_cleanup confirm` and `EVA_upgrade` (Upgrade World, if installed).
