# EpicValheimsAdditions — golden aura + trophy costs

## Goal
Dim golden miniboss aura and raise altar trophy costs for the CJAYCRAFT pack.

## Acceptance criteria
- Aura intensity configurable (`GoldenAuraIntensity`, default 0.35)
- Trophy costs configurable; Wraith default 8 (was 4 on prefab)
- Existing world altars pick up new `m_bossItems`
- Release build succeeds

## Current status
Complete. 2.2.11 built.

## Files changed
- `EpicValheimsAdditions/GoldenMinibossTuning.cs` (new)
- `EpicValheimsAdditions/EVAConfiguration.cs`, `Core.cs`
- `EpicValheimsAdditions.csproj` (ParticleSystemModule)
- docs / changelog / manifest 2.2.11
- client + server `Huntard.EpicValheimsAdditions_Content.json`

## Commands run
- `dotnet build EpicValheimsAdditions.csproj -c Release` — succeeded
- DLL FileVersion 2.2.11 at `D:\AI-Agent-Projects\cjaycraft\EpicValheimsAdditions\bin\Release\net481\EpicValheimsAdditions.dll`

## Remaining issues
- In-game brightness still needs a look; tune `GoldenAuraIntensity` if 0.35 is too dark or still too bright
- Pack DLL not installed to Gale in this task
