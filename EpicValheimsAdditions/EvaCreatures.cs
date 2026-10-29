using System;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using UnityEngine;

namespace EpicValheimsAdditions;

internal static class EvaCreatures
{
	internal static readonly string[] DeepNorthPrefabs =
	{
		"CrystalhideUrsar",
		"CrystalhideUrsar_Cub",
		"FrostwingFae"
	};

	internal static readonly string[] AshlandsPrefabs =
	{
		"MagmaSkarab",
		"InfernalMinotaur"
	};

	internal static readonly string[] OceanPrefabs =
	{
		"DraugrMariner"
	};

	internal static void Register(AssetBundle assetBundle, CreatureConfiguration creatures, Action<string> registerPrefab)
	{
		if (assetBundle == null)
		{
			return;
		}

		if (creatures == null)
		{
			creatures = new CreatureConfiguration();
		}

		if (creatures.DeepNorthCreatures)
		{
			registerPrefab("FrostBear_Ragdoll");
			registerPrefab("FrostBear_Ragdoll_Cub");
			AddDeepNorth(assetBundle);
		}
		else
		{
			RegisterPrefabOnly(registerPrefab, DeepNorthPrefabs);
		}

		if (creatures.AshlandsCreatures)
		{
			registerPrefab("Minotaur_Ragdoll");
			registerPrefab("Skarab_Ragdoll");
			AddAshlands(assetBundle);
		}
		else
		{
			RegisterPrefabOnly(registerPrefab, AshlandsPrefabs);
			Core.Log.LogInfo("EVA Ashlands creatures are turned off");
		}

		if (creatures.OceanCreatures)
		{
			AddOcean(assetBundle);
		}
		else
		{
			RegisterPrefabOnly(registerPrefab, OceanPrefabs);
			Core.Log.LogInfo("EVA Ocean creatures are turned off");
		}

		if (!creatures.DeepNorthCreatures)
		{
			Core.Log.LogInfo("EVA Deep North creatures are turned off");
		}

		Core.Log.LogInfo("Loaded EVA creatures.");
	}

	private static void RegisterPrefabOnly(Action<string> registerPrefab, string[] prefabNames)
	{
		foreach (string name in prefabNames)
		{
			registerPrefab(name);
		}
	}

	private static void AddDeepNorth(AssetBundle assetBundle)
	{
		AddCreature(assetBundle, "CrystalhideUrsar", Character.Faction.MountainMonsters, Heightmap.Biome.DeepNorth,
			33f, 5, 1, 3, 330, "Crystalhide Ursar", null);
		AddCreature(assetBundle, "CrystalhideUrsar_Cub", Character.Faction.MountainMonsters, Heightmap.Biome.DeepNorth,
			5f, 1, 1, 1, 60, "Crystalhide Ursar Cub", null);
		AddCreature(assetBundle, "FrostwingFae", Character.Faction.MountainMonsters, Heightmap.Biome.DeepNorth,
			15f, 3, 1, 1, 500, "Frostwing Fae", null);
	}

	private static void AddAshlands(AssetBundle assetBundle)
	{
		AddCreature(assetBundle, "MagmaSkarab", Character.Faction.Demon, Heightmap.Biome.AshLands,
			15f, 3, 1, 2, 250, "Magma Skarab", null);
		AddCreature(assetBundle, "InfernalMinotaur", Character.Faction.Demon, Heightmap.Biome.AshLands,
			33f, 5, 1, 3, 400, "Infernal Minotaur", null);
	}

	private static void AddOcean(AssetBundle assetBundle)
	{
		AddCreature(assetBundle, "DraugrMariner", Character.Faction.SeaMonsters, Heightmap.Biome.Ocean,
			10f, 3, 1, 1, 500, "Draugr Mariner", "Killed_Jotunn");
	}

	private static void AddCreature(
		AssetBundle assetBundle,
		string prefabName,
		Character.Faction faction,
		Heightmap.Biome biome,
		float spawnChance,
		int maxSpawned,
		int minLevel,
		int maxLevel,
		int spawnInterval,
		string creatureName,
		string requiredGlobalKey)
	{
		GameObject prefab = assetBundle.LoadAsset<GameObject>(prefabName);
		if (prefab == null)
		{
			Core.Log.LogWarning("Missing creature prefab " + prefabName);
			return;
		}

		CreatureConfig config = new CreatureConfig { Name = creatureName };
		config.Faction = faction;

		if (prefabName == "DraugrMariner")
		{
			SpawnConfig ocean = BuildBaseSpawn(prefabName, spawnChance, maxSpawned, biome, requiredGlobalKey, minLevel, maxLevel, spawnInterval);
			ocean.MaxAltitude = -5f;
			ocean.MinAltitude = -100f;
			ocean.SpawnDistance = 50f;
			ocean.GroupRadius = 3f;
			ocean.MinGroupSize = 1;
			ocean.MaxGroupSize = 1;
			ocean.MinTilt = 0f;
			ocean.MaxTilt = 35f;
			ocean.SpawnInForest = true;
			ocean.SpawnOutsideForest = true;
			ocean.BiomeArea = Heightmap.BiomeArea.Median;
			ocean.GroundOffset = 0.5f;
			ocean.SpawnAtDay = false;
			ocean.SpawnAtNight = true;
			config.AddSpawnConfig(ocean);
		}
		else
		{
			SpawnConfig land = BuildBaseSpawn(prefabName, spawnChance, maxSpawned, biome, requiredGlobalKey, minLevel, maxLevel, spawnInterval);
			land.SpawnDistance = 30f;
			land.GroupRadius = 10f;
			config.AddSpawnConfig(land);
		}

		CreatureManager.Instance.AddCreature(new CustomCreature(prefab, true, config));
	}

	private static SpawnConfig BuildBaseSpawn(
		string prefabName,
		float spawnChance,
		int maxSpawned,
		Heightmap.Biome biome,
		string requiredGlobalKey,
		int minLevel,
		int maxLevel,
		int spawnInterval)
	{
		SpawnConfig spawn = new SpawnConfig
		{
			Name = prefabName,
			SpawnChance = spawnChance,
			MaxSpawned = maxSpawned,
			Biome = biome,
			RequiredGlobalKey = requiredGlobalKey,
			MinLevel = minLevel,
			MaxLevel = maxLevel,
			SpawnInterval = spawnInterval
		};
		return spawn;
	}
}
