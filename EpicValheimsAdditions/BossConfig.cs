using System;
using System.Collections.Generic;

namespace EpicValheimsAdditions;

[Serializable]
public class BossConfig
{
	public string BossPrefabName { get; set; }

	public int Health { get; set; }

	public List<CustomAttack> Attacks { get; set; }
}
