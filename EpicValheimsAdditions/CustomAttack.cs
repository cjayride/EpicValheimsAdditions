using System;

namespace EpicValheimsAdditions;

[Serializable]
public class CustomAttack
{
	public string AttackName { get; set; }

	public string AttackAnimation { get; set; }

	public float AiAttackRange { get; set; }

	public float AiAttackRangeMin { get; set; }

	public float AiAttackInterval { get; set; }

	public HitData.DamageTypes? Damages { get; set; }
}
