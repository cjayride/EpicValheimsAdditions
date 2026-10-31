using System.Collections.Generic;
using BepInEx.Bootstrap;
using UnityEngine;

namespace EpicValheimsAdditions;

public static class Commands
{
	public const string UpgradeWorldGuid = "upgrade_world";

	private const string RpcCleanup = "Huntard.EpicValheimsAdditions.CmdCleanup";
	private const string RpcUpgrade = "Huntard.EpicValheimsAdditions.CmdUpgrade";
	private const string RpcConsoleOutput = "Huntard.EpicValheimsAdditions.CmdConsoleOutput";

	private static bool rpcRegistered;

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
			onlyServer: false,
			onlyAdmin: true);

		_ = new Terminal.ConsoleCommand(
			"EVA_upgrade",
			"Add EVA boss altars and vegvisirs to already-generated zones (requires Upgrade World).",
			OnUpgrade,
			isCheat: true,
			onlyServer: false,
			onlyAdmin: true);
	}

	// RPC handlers are registered lazily by Core.Update() once ZRoutedRpc exists.
	// Deliberately NOT registered from a harmony patch on ZNet.Awake: that puts this mod
	// inside the game's network-startup path, which is on the critical path of every join.
	public static bool TryRegisterRpcHandlers()
	{
		if (rpcRegistered)
		{
			return true;
		}

		if (ZRoutedRpc.instance == null)
		{
			return false;
		}

		rpcRegistered = true;
		ZRoutedRpc.instance.Register<bool>(RpcCleanup, RpcOnCleanup);
		ZRoutedRpc.instance.Register(RpcUpgrade, RpcOnUpgrade);
		ZRoutedRpc.instance.Register<string>(RpcConsoleOutput, RpcOnConsoleOutput);
		return true;
	}

	private static void OnCleanup(Terminal.ConsoleEventArgs args)
	{
		bool confirm = args.Length >= 2 && args[1].ToLowerInvariant() == "confirm";
		if (ZNet.instance != null && ZNet.instance.IsServer())
		{
			RunCleanupLocal(args, confirm);
			return;
		}

		if (!confirm)
		{
			AddCleanupPreview(args);
			return;
		}

		if (!TrySendToServer(RpcCleanup, true))
		{
			args.Context.AddString("Could not reach the server. Join the world and try again.");
		}
		else
		{
			args.Context.AddString("EVA_cleanup sent to server. Watch the console for results.");
		}
	}

	private static void OnUpgrade(Terminal.ConsoleEventArgs args)
	{
		if (ZNet.instance != null && ZNet.instance.IsServer())
		{
			RunUpgradeLocal(args);
			return;
		}

		if (!TrySendToServer(RpcUpgrade))
		{
			args.Context.AddString("Could not reach the server. Join the world and try again.");
		}
		else
		{
			args.Context.AddString("EVA_upgrade sent to server. Watch the console for results.");
		}
	}

	private static void RpcOnCleanup(long sender, bool confirm)
	{
		if (ZNet.instance == null || !ZNet.instance.IsServer())
		{
			return;
		}

		if (!IsPeerAdmin(sender))
		{
			SendConsoleOutput(sender, "EVA_cleanup rejected: admin only.");
			return;
		}

		if (!confirm)
		{
			SendCleanupPreview(sender);
			return;
		}

		int removed = RemoveRetiredZdos(Core.GetCreatureConfiguration());
		Core.Log.LogInfo("EVA_cleanup removed " + removed + " retired ZDOs.");
		SendConsoleOutput(sender, "EVA_cleanup removed " + removed + " retired ZDOs.");

		if (HasUpgradeWorld())
		{
			RunVegetationReset();
			SendConsoleOutput(sender, "Also ran Upgrade World vegetation_reset for retired EVA plants.");
		}
	}

	private static void RpcOnUpgrade(long sender)
	{
		if (ZNet.instance == null || !ZNet.instance.IsServer())
		{
			return;
		}

		if (!IsPeerAdmin(sender))
		{
			SendConsoleOutput(sender, "EVA_upgrade rejected: admin only.");
			return;
		}

		if (!HasUpgradeWorld())
		{
			SendConsoleOutput(sender, "EVA_upgrade needs Upgrade World. New zones still get EVA altars from Jotunn.");
			return;
		}

		string command = "locations_add " + string.Join(",", KeptLocations) + " start";
		if (Console.instance != null)
		{
			Console.instance.TryRunCommand(command, false, false);
		}

		SendConsoleOutput(sender, "Requested Upgrade World to add EVA altars and vegvisirs.");
	}

	private static void RpcOnConsoleOutput(long sender, string message)
	{
		if (ZNet.instance != null && ZNet.instance.IsServer())
		{
			return;
		}

		if (Console.instance != null)
		{
			Console.instance.Print(message);
		}
		else
		{
			Core.Log.LogInfo(message);
		}
	}

	private static void RunCleanupLocal(Terminal.ConsoleEventArgs args, bool confirm)
	{
		if (!confirm)
		{
			AddCleanupPreview(args);
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

	private static void RunUpgradeLocal(Terminal.ConsoleEventArgs args)
	{
		if (!HasUpgradeWorld())
		{
			args.Context.AddString("EVA_upgrade needs Upgrade World. New zones still get EVA altars from Jotunn.");
			return;
		}

		string command = "locations_add " + string.Join(",", KeptLocations) + " start";
		args.Context.TryRunCommand(command, false, false);
		args.Context.AddString("Requested Upgrade World to add EVA altars and vegvisirs.");
	}

	private static void AddCleanupPreview(Terminal.ConsoleEventArgs args)
	{
		args.Context.AddString("This removes retired EVA world objects (trees, rocks, veins, creatures, Thor's Forge, alchemy station, old items).");
		args.Context.AddString("Bosses, minibosses, altars, and vegvisirs are kept.");
		args.Context.AddString("Run: EVA_cleanup confirm");
	}

	private static void SendCleanupPreview(long peerUid)
	{
		SendConsoleOutput(peerUid, "This removes retired EVA world objects (trees, rocks, veins, creatures, Thor's Forge, alchemy station, old items).");
		SendConsoleOutput(peerUid, "Bosses, minibosses, altars, and vegvisirs are kept.");
		SendConsoleOutput(peerUid, "Run: EVA_cleanup confirm");
	}

	private static void RunVegetationReset()
	{
		if (Console.instance != null)
		{
			Console.instance.TryRunCommand(RetiredVegetationReset, false, false);
		}
	}

	private static bool TrySendToServer(string rpcName, params object[] parameters)
	{
		if (ZRoutedRpc.instance == null || ZNet.instance == null || ZNet.instance.IsServer())
		{
			return false;
		}

		ZNetPeer server = ZNet.instance.GetServerPeer();
		if (server == null)
		{
			return false;
		}

		ZRoutedRpc.instance.InvokeRoutedRPC(server.m_uid, rpcName, parameters);
		return true;
	}

	private static void SendConsoleOutput(long peerUid, string message)
	{
		if (ZRoutedRpc.instance == null)
		{
			return;
		}

		ZRoutedRpc.instance.InvokeRoutedRPC(peerUid, RpcConsoleOutput, message);
	}

	private static bool IsPeerAdmin(long peerUid)
	{
		if (ZNet.instance == null)
		{
			return false;
		}

		ZNetPeer peer = ZNet.instance.GetPeer(peerUid);
		if (peer?.m_rpc == null)
		{
			return false;
		}

		ISocket socket = peer.m_rpc.GetSocket();
		if (socket == null)
		{
			return false;
		}

		return ZNet.instance.IsAdmin(socket.GetHostName());
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
