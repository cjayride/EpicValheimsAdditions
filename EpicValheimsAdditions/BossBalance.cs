using System;
using System.Collections.Generic;

namespace EpicValheimsAdditions;

internal static class BossBalance
{
	// Tuned for ~3–4 players (not solo, not 8p raid). Optional bosses sit above vanilla capstone.
	public const int MistlandsHealth = 24000;
	public const int AshlandsHealth = 45000;
	public const int DeepNorthHealth = 100000;

	public const float MistlandsDamageScale = 1.06f;
	public const float AshlandsDamageScale = 1.32f;
	public const float DeepNorthDamageScale = 1.58f;

	private const int LegacyMistlandsHealth = 15000;
	private const int LegacyAshlandsHealth = 32000;
	private const int LegacyDeepNorthHealth = 52000;
	private const int PriorMistlandsHealth = 22000;
	private const int PriorAshlandsHealth = 40000;
	private const int PriorDeepNorthHealth = 80000;

	private static readonly HashSet<string> MainBosses = new HashSet<string>(StringComparer.Ordinal)
	{
		"SvartalfarQueen",
		"HelDemon",
		"Jotunn"
	};

	public static bool IsMainBoss(string prefabName)
	{
		return MainBosses.Contains(prefabName);
	}

	public static int GetDefaultHealth(string prefabName)
	{
		switch (prefabName)
		{
			case "SvartalfarQueen":
				return MistlandsHealth;
			case "HelDemon":
				return AshlandsHealth;
			case "Jotunn":
				return DeepNorthHealth;
			default:
				return -1;
		}
	}

	public static float GetDamageScale(string prefabName)
	{
		switch (prefabName)
		{
			case "SvartalfarQueen":
				return MistlandsDamageScale;
			case "HelDemon":
				return AshlandsDamageScale;
			case "Jotunn":
				return DeepNorthDamageScale;
			default:
				return 1f;
		}
	}

	public static HitData.DamageTypes ScaleDamages(HitData.DamageTypes damages, float scale)
	{
		if (scale == 1f)
		{
			return damages;
		}

		return new HitData.DamageTypes
		{
			m_damage = damages.m_damage * scale,
			m_blunt = damages.m_blunt * scale,
			m_slash = damages.m_slash * scale,
			m_pierce = damages.m_pierce * scale,
			m_chop = damages.m_chop * scale,
			m_pickaxe = damages.m_pickaxe * scale,
			m_fire = damages.m_fire * scale,
			m_frost = damages.m_frost * scale,
			m_lightning = damages.m_lightning * scale,
			m_poison = damages.m_poison * scale,
			m_spirit = damages.m_spirit * scale,
			m_nonPlayer = damages.m_nonPlayer * scale
		};
	}

	/// <summary>
	/// Ensures Mistlands &lt; Ashlands &lt; Deep North for HP and fixes legacy Jotunn inflated HP.
	/// </summary>
	public static bool NeedsBalanceMigration(IReadOnlyList<BossConfig> configs)
	{
		BossConfig queen = Find(configs, "SvartalfarQueen");
		BossConfig hel = Find(configs, "HelDemon");
		BossConfig jot = Find(configs, "Jotunn");
		if (queen == null || hel == null || jot == null)
		{
			return false;
		}

		if (jot.Health > 100000)
		{
			return true;
		}

		if (hel.Health <= queen.Health || jot.Health <= hel.Health)
		{
			return true;
		}

		if (MatchesTriple(queen.Health, hel.Health, jot.Health, LegacyMistlandsHealth, LegacyAshlandsHealth, LegacyDeepNorthHealth))
		{
			return true;
		}

		return MatchesTriple(queen.Health, hel.Health, jot.Health, PriorMistlandsHealth, PriorAshlandsHealth, PriorDeepNorthHealth);
	}

	private static bool MatchesTriple(int queenHp, int helHp, int jotHp, int q, int h, int j)
	{
		return queenHp == q && helHp == h && jotHp == j;
	}

	private static BossConfig Find(IReadOnlyList<BossConfig> configs, string name)
	{
		foreach (BossConfig config in configs)
		{
			if (config.BossPrefabName == name)
			{
				return config;
			}
		}

		return null;
	}
}
