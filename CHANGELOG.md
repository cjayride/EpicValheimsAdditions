# Changelog

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
