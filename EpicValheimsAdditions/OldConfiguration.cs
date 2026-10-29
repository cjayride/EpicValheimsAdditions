using System;

namespace EpicValheimsAdditions;

[Serializable]
public class OldConfiguration
{
	public string Description { get; set; }

	public bool MistlandsLocations { get; set; }

	public bool Mistlands { get; set; }

	public bool DeepNorthLocations { get; set; }

	public bool DeepNorth { get; set; }

	public bool AshlandsLocations { get; set; }

	public bool Ashlands { get; set; }
}
