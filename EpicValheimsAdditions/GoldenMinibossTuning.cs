using HarmonyLib;
using UnityEngine;

namespace EpicValheimsAdditions;

internal static class GoldenMinibossTuning
{
	internal static float AuraIntensity = 0.35f;
	internal static int GreydwarfTrophyCost = 10;
	internal static int TrollTrophyCost = 6;
	internal static int WraithTrophyCost = 8;

	internal static void ApplyFromConfig(EVAConfiguration config)
	{
		if (config == null)
		{
			return;
		}

		AuraIntensity = Mathf.Clamp(config.GoldenAuraIntensity, 0.05f, 1f);
		GreydwarfTrophyCost = Mathf.Max(1, config.GoldenGreydwarfTrophyCost);
		TrollTrophyCost = Mathf.Max(1, config.GoldenTrollTrophyCost);
		WraithTrophyCost = Mathf.Max(1, config.GoldenWraithTrophyCost);
	}

	internal static void ApplyToPrefabs()
	{
		DimAura("Golden_Greydwarf_Miniboss");
		DimAura("Golden_Troll_Miniboss");
		DimAura("Golden_Wraith_Miniboss");
		SetAltarCost("StaminaGreydwarf", GreydwarfTrophyCost);
		SetAltarCost("StaminaTroll", TrollTrophyCost);
		SetAltarCost("StaminaWraith", WraithTrophyCost);
	}

	internal static int TrophyCostForBoss(GameObject bossPrefab)
	{
		if (bossPrefab == null)
		{
			return 0;
		}

		string name = bossPrefab.name;
		if (name == "Golden_Greydwarf_Miniboss")
		{
			return GreydwarfTrophyCost;
		}

		if (name == "Golden_Troll_Miniboss")
		{
			return TrollTrophyCost;
		}

		if (name == "Golden_Wraith_Miniboss")
		{
			return WraithTrophyCost;
		}

		return 0;
	}

	private static void SetAltarCost(string prefabName, int amount)
	{
		GameObject prefab = PrefabManagerGet(prefabName);
		if (prefab == null)
		{
			return;
		}

		OfferingBowl[] bowls = prefab.GetComponentsInChildren<OfferingBowl>(true);
		for (int i = 0; i < bowls.Length; i++)
		{
			bowls[i].m_bossItems = amount;
		}
	}

	private static void DimAura(string prefabName)
	{
		GameObject prefab = PrefabManagerGet(prefabName);
		if (prefab == null || AuraIntensity >= 0.999f)
		{
			return;
		}

		float intensity = AuraIntensity;
		Light[] lights = prefab.GetComponentsInChildren<Light>(true);
		for (int i = 0; i < lights.Length; i++)
		{
			lights[i].intensity *= intensity;
			lights[i].range *= Mathf.Lerp(0.65f, 1f, intensity);
		}

		ParticleSystem[] particles = prefab.GetComponentsInChildren<ParticleSystem>(true);
		for (int i = 0; i < particles.Length; i++)
		{
			ParticleSystem.MainModule main = particles[i].main;
			main.startColor = ScaleStartColor(main.startColor, intensity);
			ParticleSystem.EmissionModule emission = particles[i].emission;
			emission.rateOverTimeMultiplier *= intensity;
			emission.rateOverDistanceMultiplier *= intensity;
		}

		Renderer[] renderers = prefab.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < renderers.Length; i++)
		{
			Material[] materials = renderers[i].sharedMaterials;
			if (materials == null)
			{
				continue;
			}

			for (int m = 0; m < materials.Length; m++)
			{
				Material material = materials[m];
				if (material == null)
				{
					continue;
				}

				if (material.HasProperty("_EmissionColor"))
				{
					material.SetColor("_EmissionColor", material.GetColor("_EmissionColor") * intensity);
				}
			}
		}
	}

	private static ParticleSystem.MinMaxGradient ScaleStartColor(ParticleSystem.MinMaxGradient gradient, float intensity)
	{
		Color Scale(Color color)
		{
			color.a *= Mathf.Lerp(0.4f, 1f, intensity);
			color.r = Mathf.Lerp(color.r * 0.55f, color.r, intensity);
			color.g = Mathf.Lerp(color.g * 0.55f, color.g, intensity);
			color.b = Mathf.Lerp(color.b * 0.7f, color.b, intensity);
			return color;
		}

		switch (gradient.mode)
		{
			case ParticleSystemGradientMode.Color:
				gradient.color = Scale(gradient.color);
				break;
			case ParticleSystemGradientMode.TwoColors:
				gradient.colorMin = Scale(gradient.colorMin);
				gradient.colorMax = Scale(gradient.colorMax);
				break;
		}

		return gradient;
	}

	private static GameObject PrefabManagerGet(string name)
	{
		return Jotunn.Managers.PrefabManager.Instance.GetPrefab(name);
	}

	[HarmonyPatch(typeof(OfferingBowl), "Awake")]
	private static class OfferingBowlAwakePatch
	{
		private static void Postfix(OfferingBowl __instance)
		{
			if (__instance == null)
			{
				return;
			}

			int cost = TrophyCostForBoss(__instance.m_bossPrefab);
			if (cost > 0)
			{
				__instance.m_bossItems = cost;
			}
		}
	}
}
