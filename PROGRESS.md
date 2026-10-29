# EpicValheimsAdditions — Boss-Only Update

## Goal
Keep the three EVA bosses, three golden minibosses, their altars, and vegvisirs. Leave vanilla biomes unchanged. Add a world cleanup command for retired EVA objects.

## Acceptance criteria
- Project compiles against current Valheim 1.0 / Jotunn 2.30.2
- No EVA weapons, ores, vegetation, or extra creatures are registered
- Summon items use vanilla late-game recipes
- `EVA_cleanup confirm` removes retired world ZDOs
- `EVA_upgrade` can inject kept locations when Upgrade World is installed

## Current status
2.2.5 packaged for Thunderstore as `EpicValheimsAdditions_cjayfix` (plugin GUID unchanged). Zip in `/dist`.

## Environment
- Valheim 1.0.20, Unity 6000.0.75
- Jotunn 2.30.2
- EVA version 2.2.1

## Files changed
- `EpicValheimsAdditions.csproj` — net481, HintPaths, publicizer
- `Environment.props` — Valheim and Jotunn paths
- `EpicValheimsAdditions/Core.cs` — boss-only registration
- `EpicValheimsAdditions/Commands.cs` — EVA_cleanup / EVA_upgrade
- `EpicValheimsAdditions/BossToggleConfiguration.cs` — new toggles
- `EpicValheimsAdditions/EVAConfiguration.cs` — Bosses section
- `EpicValheimsAdditions/CustomAttack.cs` — HitData.DamageTypes
- `EpicValheimsAdditions/BossBalance.cs` — Mistlands / Ashlands / Deep North defaults + migration
- `EpicValheimsAdditions/GoldenMinibossCooldown.cs` — Harmony altar cooldown after kill
- `Properties/AssemblyInfo.cs` — 2.2.1
- `beta_cjaycraft/config/drop_that.character_drop.EvaBalance.cfg` (+ server copy) — boss-only drop fixes, coin tier order
- Deleted `UpgradeWorld/Upgrade.cs` and `UpgradeWorld/CommandRegistration.cs`

## Commands run
- `dotnet build EpicValheimsAdditions.csproj -c Release` — succeeded
- Output: `D:\AI-Agent-Projects\cjaycraft\EpicValheimsAdditions\bin\Release\net481\EpicValheimsAdditions.dll`

## Remaining issues
- In-game verification not run (boss summons, vegvisirs, cleanup)
- Existing EVA weapons/items in inventories will vanish because those prefabs are no longer registered
- Cosmetic zone vegetation without ZDOs may need Upgrade World `vegetation_reset` in addition to EVA_cleanup
