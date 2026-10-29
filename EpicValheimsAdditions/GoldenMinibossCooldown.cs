using System;
using HarmonyLib;
using UnityEngine;

namespace EpicValheimsAdditions;

internal static class GoldenMinibossCooldown
{
	internal const string ZdoCooldownEndKey = "eva_golden_cd_end";
	private static readonly string[] GoldenBossPrefabs =
	{
		"Golden_Greydwarf_Miniboss",
		"Golden_Troll_Miniboss",
		"Golden_Wraith_Miniboss"
	};

	internal static int CooldownMinutes = 120;

	internal static bool IsGoldenMinibossPrefab(GameObject prefab)
	{
		if (prefab == null)
		{
			return false;
		}

		string name = prefab.name;
		foreach (string golden in GoldenBossPrefabs)
		{
			if (name == golden)
			{
				return true;
			}
		}

		return false;
	}

	private static bool IsGoldenMinibossCharacter(Character character)
	{
		if (character == null || character.m_nview == null || !character.m_nview.IsValid())
		{
			return false;
		}

		string prefabName = character.name.Replace("(Clone)", string.Empty);
		foreach (string golden in GoldenBossPrefabs)
		{
			if (prefabName == golden || prefabName.StartsWith(golden, StringComparison.Ordinal))
			{
				return true;
			}
		}

		return false;
	}

	private static double NowSeconds()
	{
		return ZNet.instance.GetTimeSeconds();
	}

	private static double CooldownEndSeconds()
	{
		if (CooldownMinutes <= 0)
		{
			return 0d;
		}

		return NowSeconds() + CooldownMinutes * 60d;
	}

	private static void NotifyCooldown(Humanoid user, double cooldownEnd)
	{
		if (user == null)
		{
			return;
		}

		double remaining = Math.Max(0d, cooldownEnd - NowSeconds());
		int minutes = (int)((remaining + 59d) / 60d);
		user.Message(MessageHud.MessageType.Center,
			$"Golden miniboss altar is resting ({minutes} min remaining).");
	}

	[HarmonyPatch(typeof(OfferingBowl), nameof(OfferingBowl.InitiateSpawnBoss))]
	private static class InitiateSpawnBossPatch
	{
		private static bool Prefix(OfferingBowl __instance)
		{
			if (CooldownMinutes <= 0 || __instance == null || !IsGoldenMinibossPrefab(__instance.m_bossPrefab))
			{
				return true;
			}

			ZNetView nview = __instance.m_nview;
			if (nview == null || !nview.IsValid())
			{
				return true;
			}

			ZDO zdo = nview.GetZDO();
			double cooldownEnd = BitConverter.Int64BitsToDouble(zdo.GetLong(ZdoCooldownEndKey, 0L));
			if (NowSeconds() >= cooldownEnd)
			{
				return true;
			}

			NotifyCooldown(__instance.m_interactUser, cooldownEnd);
			return false;
		}
	}

	[HarmonyPatch(typeof(Character), nameof(Character.OnDeath))]
	private static class OnDeathPatch
	{
		private static void Postfix(Character __instance)
		{
			if (CooldownMinutes <= 0 || !IsGoldenMinibossCharacter(__instance))
			{
				return;
			}

			OfferingBowl altar = FindNearestGoldenAltar(__instance.transform.position);
			if (altar?.m_nview == null || !altar.m_nview.IsValid())
			{
				return;
			}

			altar.m_nview.GetZDO().Set(ZdoCooldownEndKey, BitConverter.DoubleToInt64Bits(CooldownEndSeconds()));
		}
	}

	private static OfferingBowl FindNearestGoldenAltar(Vector3 position)
	{
		OfferingBowl[] bowls = UnityEngine.Object.FindObjectsByType<OfferingBowl>(FindObjectsSortMode.None);
		OfferingBowl best = null;
		float bestDistance = 60f;
		foreach (OfferingBowl bowl in bowls)
		{
			if (!IsGoldenMinibossPrefab(bowl.m_bossPrefab))
			{
				continue;
			}

			float distance = Vector3.Distance(position, bowl.transform.position);
			if (distance >= bestDistance)
			{
				continue;
			}

			bestDistance = distance;
			best = bowl;
		}

		return best;
	}
}
