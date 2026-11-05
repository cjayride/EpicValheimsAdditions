using System;

namespace EpicValheimsAdditions;

[Serializable]
public class EVAConfiguration
{
	public string Content_Description { get; set; }

	public BossToggleConfiguration Bosses { get; set; }

	/// <summary>Real-time minutes before a golden miniboss altar can be used again after a kill (0 = off).</summary>
	public int GoldenMinibossCooldownMinutes { get; set; } = 120;

	/// <summary>1 = original EVA brightness. Lower values dim lights, particles, and emission.</summary>
	public float GoldenAuraIntensity { get; set; } = 0.35f;

	public int GoldenGreydwarfTrophyCost { get; set; } = 10;

	public int GoldenTrollTrophyCost { get; set; } = 6;

	public int GoldenWraithTrophyCost { get; set; } = 8;

	public ContentConfiguration Content { get; set; }

	public LocationConfiguration Locations { get; set; }

	public CreatureConfiguration Creatures { get; set; }
}
