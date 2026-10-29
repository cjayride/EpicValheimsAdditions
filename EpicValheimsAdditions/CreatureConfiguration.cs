using System;

namespace EpicValheimsAdditions;

[Serializable]
public class CreatureConfiguration
{
	public bool DeepNorthCreatures { get; set; } = true;

	public bool AshlandsCreatures { get; set; } = true;

	public bool OceanCreatures { get; set; } = true;
}
