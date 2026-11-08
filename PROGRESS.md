# EpicValheimsAdditions — altar offering hover

## Goal
Show required offering type and count on EVA boss altars (and keep the same format on golden miniboss altars).

## Acceptance criteria
- Hovering Svartalfar Queen / Hel Demon / Jotunn altars shows `Offer: N× item`
- Golden miniboss altars use the same hover format
- Existing worlds pick up `m_bossItem` if it was missing on the placed altar
- Release build succeeds

## Current status
Complete. 2.2.12 built.

## Files changed
- `EpicValheimsAdditions/GoldenMinibossTuning.cs` — bind EVA boss offerings, hover all EVA altars
- Version 2.2.12 (Core, AssemblyInfo, Package/manifest, changelogs, README)

## Commands run
- `dotnet build EpicValheimsAdditions.csproj -c Release` — succeeded
- DLL FileVersion 2.2.12 at `D:\AI-Agent-Projects\cjaycraft\EpicValheimsAdditions\bin\Release\net481\EpicValheimsAdditions.dll`

## Remaining issues
- In-game hover still needs a player check after install; not copied to Gale/modpack in this task
