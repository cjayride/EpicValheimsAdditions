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

	private static readonly int[] GoldenPrefabHashes =
	{
		"Golden_Greydwarf_Miniboss".GetStableHashCode(),
		"Golden_Troll_Miniboss".GetStableHashCode(),
		"Golden_Wraith_Miniboss".GetStableHashCode()
	};

	private static readonly Collider[] AltarOverlapBuffer = new Collider[24];
	private static OfferingBowl lastUsedGoldenAltar;

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

	private static bool IsGoldenPrefabHash(int prefabHash)
	{
		foreach (int hash in GoldenPrefabHashes)
		{
			if (prefabHash == hash)
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

		return IsGoldenPrefabHash(character.m_nview.GetZDO().GetPrefab());
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
				lastUsedGoldenAltar = __instance;
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
		if (lastUsedGoldenAltar != null && lastUsedGoldenAltar.m_nview != null && lastUsedGoldenAltar.m_nview.IsValid()
			&& IsGoldenMinibossPrefab(lastUsedGoldenAltar.m_bossPrefab)
			&& (position - lastUsedGoldenAltar.transform.position).sqrMagnitude <= 60f * 60f)
		{
			return lastUsedGoldenAltar;
		}

		int hits = Physics.OverlapSphereNonAlloc(position, 60f, AltarOverlapBuffer);
		OfferingBowl best = null;
		float bestDistanceSq = 60f * 60f;
		for (int i = 0; i < hits; i++)
		{
			Collider collider = AltarOverlapBuffer[i];
			AltarOverlapBuffer[i] = null;
			if (collider == null)
			{
				continue;
			}

			OfferingBowl bowl = collider.GetComponentInParent<OfferingBowl>();
			if (bowl == null || !IsGoldenMinibossPrefab(bowl.m_bossPrefab))
			{
				continue;
			}

			float distanceSq = (position - bowl.transform.position).sqrMagnitude;
			if (distanceSq >= bestDistanceSq)
			{
				continue;
			}

			bestDistanceSq = distanceSq;
			best = bowl;
		}

		return best;
	}
}
