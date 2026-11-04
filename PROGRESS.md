# EpicValheimsAdditions — 1.0.21 compatibility + performance

## Goal
Confirm EVA compiles against the latest Steam Valheim, then reduce load cost and runtime hitching.

## Acceptance criteria
- Project compiles against current Valheim public branch + Jotunn 2.30.2
- Harmony targets used by golden miniboss cooldown still exist
- No extra per-frame Update after RPC registration
- Disabled creature packs are not loaded
- Golden death path does not scan every OfferingBowl

## Current status
2.2.10 built: `D:\AI-Agent-Projects\cjaycraft\EpicValheimsAdditions\bin\Release\net481\EpicValheimsAdditions.dll`

## Environment
- Steam Valheim public buildid 26249040 (Patch 1.0.21, Oct 28 2026)
- Installed path `D:\SteamLibrary\steamapps\common\Valheim`
- Jotunn 2.30.2
- EVA version 2.2.10

## Files changed
- `EpicValheimsAdditions/Core.cs` — 2.2.10, cached bundle names, coroutine RPC wait, nested Harmony patches only
- `EpicValheimsAdditions/EvaCreatures.cs` — do not load creature prefabs when toggles are off
- `EpicValheimsAdditions/GoldenMinibossCooldown.cs` — prefab-hash death check, last-altar + overlap lookup
- `EpicValheimsAdditions/Commands.cs` — comment only
- `EpicValheimsAdditions.csproj` — UnityEngine.PhysicsModule
- `Properties/AssemblyInfo.cs` — 2.2.10
- `CHANGELOG.md`, `Package/CHANGELOG.md`

## Commands run
- SteamDB public branch buildid 26249040 matches local `appmanifest_892970.acf`
- assembly_valheim contains `InitiateSpawnBoss`, `OnDeath`, `OfferingBowl`, `DeepNorth`, `AshLands`
- `dotnet build EpicValheimsAdditions.csproj -c Release` — succeeded (1 unused LitJson CLS warning)
- DLL FileVersion 2.2.10

## Remaining issues
- In-game verification not run (boss summons, vegvisirs, cleanup, golden cooldown)
- Asset bundle still loads synchronously on Awake (inherent Unity bundle cost)
- Extra biome creatures still add spawn-system work when their toggles are on
