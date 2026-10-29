using System;

namespace EpicValheimsAdditions;

[Serializable]
public class EVAConfiguration
{
	public string Content_Description { get; set; }

	public BossToggleConfiguration Bosses { get; set; }

	/// <summary>Real-time minutes before a golden miniboss altar can be used again after a kill (0 = off).</summary>
	public int GoldenMinibossCooldownMinutes { get; set; } = 120;

	public ContentConfiguration Content { get; set; }

	public LocationConfiguration Locations { get; set; }

	public CreatureConfiguration Creatures { get; set; }
}
