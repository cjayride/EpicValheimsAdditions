using System.Collections.Generic;
using BepInEx.Bootstrap;
using UnityEngine;

namespace EpicValheimsAdditions;

public static class Commands
{
	public const string UpgradeWorldGuid = "upgrade_world";

	public static readonly string[] KeptLocations =
	{
		"SvartalfrQueenAltar_New",
		"Vegvisir_SvartalfrQueen",
		"JotunnAltar",
		"Vegvisir_Jotunn",
		"BlazingDamnedOneAltar",
		"Vegvisir_BlazingDamnedOne",
		"StaminaGreydwarf",
		"StaminaTroll",
		"StaminaWraith"
	};

	public static readonly string[] RetiredPrefabs =
	{
		"HeavymetalVein",
		"FrometalVein_frac",
		"EVA_Rock1", "EVA_Rock2", "EVA_Rock3", "EVA_Rock4", "EVA_Rock5", "EVA_Rock6",
		"EVA_Rock7", "EVA_Rock8", "EVA_Rock9", "EVA_Rock10", "EVA_Rock11", "EVA_Rock12", "EVA_Rock13",
		"EVA_Tree1", "EVA_Tree2", "EVA_Tree3",
		"EVA_Bush1", "EVA_Bush2",
		"Pickable_Snowgil_Shroom",
		"EVA_SutRock1", "EVA_SutRock2", "EVA_SutRock3", "EVA_SutRock4", "EVA_SutRock5", "EVA_SutRock6",
		"EVA_SutRock7", "EVA_SutRock8", "EVA_SutRock9", "EVA_SutRock10", "EVA_SutRock11",
		"EVA_BurningTree1", "EVA_BurningTree2", "EVA_BurningTree3",
		"EVA_BurningBush1", "EVA_BurningBush2", "EVA_BurningBush3",
		"Pickable_Emberbloom",
		"piece_alchemystation", "piece_thorsforge", "Sapling_Emberbloom",
		"Heavyscale", "Drakescale", "Forgedscale",
		"OreHeavymetal", "HeavymetalBar", "OreFrometal", "FrometalBar",
		"NjordsTearstone", "DeepAbyssEssence",
		"PrimordialIce", "UsarPelt", "FaeDust", "BearMeat", "SnowgillShroom",
		"BurningWorldTreeFragment", "MagmaGuck", "AshenHide", "MinotaurMeat",
		"Emberbloom", "EmberbloomSeeds",
		"HeavymetalPickaxeHead", "HeavymetalAxeHead",
		"FrometalPickaxeHead", "FrometalAxeHead",
		"FlametalPickaxeHead", "FlametalAxeHead",
		"BowHeavymetal", "AtgeirHeavymetal", "SledgeHeavymetal", "BattleaxeHeavymetal",
		"SpearHeavymetal", "KnifeHeavymetal", "MaceHeavymetal", "GreatSwordHeavymetal",
		"SwordHeavymetal", "ShieldHeavymetal", "ShieldHeavymetalTower", "AxeHeavymetal", "PickaxeHeavymetal",
		"BowFrometal", "AtgeirFrometal", "SledgeFrometal", "BattleaxeFrometal",
		"SpearFrometal", "KnifeFrometal", "MaceFrometal", "GreatSwordFrometal",
		"SwordFrometal", "EVAShieldFrometal", "EVAShieldFrometalTower", "AxeFrometal", "PickaxeFrometal",
		"ArrowFrometal",
		"EVAShieldFlametal", "EVAShieldFlametalTower",
		"ArrowAbyss", "TridentDeepAbyss", "BowDeepAbyss", "GreatSwordDeepAbyss",
		"YggdrasilElixir", "GjallarhornGumbo", "FrostbiteFuel",
		"MoltenMagesMunch", "AshenTitansFeast", "EruptionElixir",
		"EVA_AdminFood",
		"FrozenStump", "Frozen_Log", "Frozen_Log_Half",
		"Burning_Log", "Burning_Log_Half", "BurningStump"
	};

	private static readonly string RetiredVegetationReset =
		"vegetation_reset HeavymetalVein,FrometalVein_frac," +
		"EVA_Rock1,EVA_Rock2,EVA_Rock3,EVA_Rock4,EVA_Rock5,EVA_Rock6,EVA_Rock7,EVA_Rock8,EVA_Rock9,EVA_Rock10,EVA_Rock11,EVA_Rock12,EVA_Rock13," +
		"EVA_Tree1,EVA_Tree2,EVA_Tree3,EVA_Bush1,EVA_Bush2,Pickable_Snowgil_Shroom," +
		"EVA_SutRock1,EVA_SutRock2,EVA_SutRock3,EVA_SutRock4,EVA_SutRock5,EVA_SutRock6,EVA_SutRock7,EVA_SutRock8,EVA_SutRock9,EVA_SutRock10,EVA_SutRock11," +
		"EVA_BurningTree1,EVA_BurningTree2,EVA_BurningTree3,EVA_BurningBush1,EVA_BurningBush2,EVA_BurningBush3,Pickable_Emberbloom start";

	public static void Register()
	{
		_ = new Terminal.ConsoleCommand(
			"EVA_cleanup",
			"Remove retired EVA vegetation, creatures, stations, and items from this world. Use: EVA_cleanup confirm",
			OnCleanup,
			isCheat: true,
			onlyServer: true,
			onlyAdmin: true);

		_ = new Terminal.ConsoleCommand(
			"EVA_upgrade",
			"Add EVA boss altars and vegvisirs to already-generated zones (requires Upgrade World).",
			OnUpgrade,
			isCheat: true,
			onlyServer: true,
			onlyAdmin: true);
	}

	private static void OnCleanup(Terminal.ConsoleEventArgs args)
	{
		if (!IsServer(args))
		{
			return;
		}

		if (args.Length < 2 || args[1].ToLowerInvariant() != "confirm")
		{
			args.Context.AddString("This removes retired EVA world objects (trees, rocks, veins, creatures, Thor's Forge, alchemy station, old items).");
			args.Context.AddString("Bosses, minibosses, altars, and vegvisirs are kept.");
			args.Context.AddString("Run: EVA_cleanup confirm");
			return;
		}

		int removed = RemoveRetiredZdos(Core.GetCreatureConfiguration());
		args.Context.AddString("EVA_cleanup removed " + removed + " retired ZDOs.");
		Core.Log.LogInfo("EVA_cleanup removed " + removed + " retired ZDOs.");

		if (HasUpgradeWorld())
		{
			args.Context.TryRunCommand(RetiredVegetationReset, false, false);
			args.Context.AddString("Also ran Upgrade World vegetation_reset for retired EVA plants.");
		}
	}

	private static void OnUpgrade(Terminal.ConsoleEventArgs args)
	{
		if (!IsServer(args))
		{
			return;
		}

		if (!HasUpgradeWorld())
		{
			args.Context.AddString("EVA_upgrade needs Upgrade World. New zones still get EVA altars from Jotunn.");
			return;
		}

		string command = "locations_add " + string.Join(",", KeptLocations) + " start";
		args.Context.TryRunCommand(command, false, false);
		args.Context.AddString("Requested Upgrade World to add EVA altars and vegvisirs.");
	}

	private static bool IsServer(Terminal.ConsoleEventArgs args)
	{
		if (ZNet.instance != null && ZNet.instance.IsServer())
		{
			return true;
		}

		args.Context.AddString("EVA commands are server/host only.");
		return false;
	}

	private static bool HasUpgradeWorld()
	{
		return Chainloader.PluginInfos != null && Chainloader.PluginInfos.ContainsKey(UpgradeWorldGuid);
	}

	private static int RemoveRetiredZdos(CreatureConfiguration creatures)
	{
		if (ZDOMan.instance == null)
		{
			return 0;
		}

		HashSet<int> retired = new HashSet<int>();
		foreach (string name in RetiredPrefabs)
		{
			retired.Add(name.GetStableHashCode());
		}

		if (creatures != null)
		{
			if (!creatures.DeepNorthCreatures)
			{
				AddPrefabHashes(retired, EvaCreatures.DeepNorthPrefabs);
				retired.Add("FrostBear_Ragdoll".GetStableHashCode());
				retired.Add("FrostBear_Ragdoll_Cub".GetStableHashCode());
			}

			if (!creatures.AshlandsCreatures)
			{
				AddPrefabHashes(retired, EvaCreatures.AshlandsPrefabs);
				retired.Add("Minotaur_Ragdoll".GetStableHashCode());
				retired.Add("Skarab_Ragdoll".GetStableHashCode());
			}

			if (!creatures.OceanCreatures)
			{
				AddPrefabHashes(retired, EvaCreatures.OceanPrefabs);
			}
		}

		List<ZDO> matches = new List<ZDO>();
		foreach (ZDO zdo in ZDOMan.instance.m_objectsByID.Values)
		{
			if (zdo != null && retired.Contains(zdo.GetPrefab()))
			{
				matches.Add(zdo);
			}
		}

		long self = ZDOMan.GetSessionID();
		foreach (ZDO zdo in matches)
		{
			zdo.SetOwner(self);
			ZDOMan.instance.DestroyZDO(zdo);
		}

		return matches.Count;
	}

	private static void AddPrefabHashes(HashSet<int> retired, string[] prefabNames)
	{
		foreach (string name in prefabNames)
		{
			retired.Add(name.GetStableHashCode());
		}
	}
}
