using System;
using System.Collections.Generic;
using System.IO;
using BepInEx;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Utils;
using LitJson;
using UnityEngine;

namespace EpicValheimsAdditions;

[BepInPlugin(ModGUID, ModName, ModVersion)]
[BepInDependency(Jotunn.Main.ModGuid, BepInDependency.DependencyFlags.HardDependency)]
public class Core : BaseUnityPlugin
{
	public const string ModName = "Epic Valheims Additions - by Huntard";
	public const string ModVersion = "2.2.5";
	public const string ModGUID = "Huntard.EpicValheimsAdditions";

	public static string configPath = Path.Combine(BepInEx.Paths.ConfigPath, "Huntard.EpicValheimsAdditions.json");
	public static string configPath2 = Path.Combine(BepInEx.Paths.ConfigPath, "Huntard.EpicValheimsAdditions_Content.json");

	internal static BepInEx.Logging.ManualLogSource Log;

	private AssetBundle assetBundle;
	private readonly List<string> sfxAssetNames = new List<string>();
	private EVAConfiguration contentConfig;

	private readonly string contentDescription =
		"EVA 2.2+ keeps only the three custom bosses, three golden minibosses, their altars, and vegvisirs. " +
		"MistlandsBoss / DeepNorthBoss / AshlandsBoss / GoldenMinibosses toggle those encounters. " +
		"GoldenMinibossCooldownMinutes sets real-time altar cooldown after a kill (default 120). " +
		"Weapons, ores, vegetation, and EVA creatures are retired. Use EVA_cleanup confirm to strip them from existing worlds.";

	private void Awake()
	{
		Log = Logger;
		Logger.LogInfo("Loading EVA " + ModVersion + " (bosses, minibosses, altars, vegvisirs only)...");
		assetBundle = AssetUtils.LoadAssetBundleFromResources("eva_assets", typeof(Core).Assembly);
		if (assetBundle == null)
		{
			Logger.LogError("Failed to load eva_assets.");
			return;
		}

		contentConfig = LoadContentConfig();
		GoldenMinibossCooldown.CooldownMinutes = contentConfig.GoldenMinibossCooldownMinutes;
		new Harmony(ModGUID).PatchAll(typeof(GoldenMinibossCooldown).Assembly);
		RegisterSupportPrefabs();
		RegisterEncounters();
		RegisterSummonItems();
		Commands.Register();

		ZoneManager.OnVanillaLocationsAvailable += AddLocations;
		PrefabManager.OnPrefabsRegistered += OnPrefabsReady;
		Logger.LogInfo("EVA registration queued.");
	}

	private void OnPrefabsReady()
	{
		FixSfx();
		ApplyBossConfig();
	}

	private void RegisterSupportPrefabs()
	{
		string[] prefabs =
		{
			"SvarTentaRoot",
			"Blazing_ragdoll",
			"Jotunn_ragdoll",
			"SvartalfarQueen_Ragdoll",
			"Greydwarf_elite_ragdoll_gold",
			"Troll_ragdoll_gold",
			"vfx_offering_ashlands",
			"lightningAOE:sfx",
			"bow_projectile_svar",
			"spawn_arrows_svartalfr",
			"sfx_death_gold",
			"sfx_attack_hit_gold",
			"Greydwarf_elite_attack_gold",
			"troll_punch",
			"troll_throw",
			"troll_groundslam",
			"wraith_melee_gold",
			"vfx_wraith_death_gold",
			"projectile_meteor_blazing",
			"SvarTentaRoot_attack"
		};
		foreach (string prefab in prefabs)
		{
			RegisterPrefab(prefab);
		}
	}

	private void RegisterEncounters()
	{
		if (contentConfig.Bosses.MistlandsBoss)
		{
			RegisterPrefab("SvartalfarQueen");
			RegisterItem("SvartalfarQueenGreatSword");
			RegisterItem("SvartalfarQueenBow");
			RegisterItem("SvartalfarQueenBowArrowStorm");
			RegisterItem("SvartalfarQueen_rootspawn");
			RegisterItem("TrophySvartalfarQueen");
			RegisterPrefab("SvartalfrQueenAltar_New");
			RegisterPrefab("Vegvisir_SvartalfrQueen");
		}

		if (contentConfig.Bosses.DeepNorthBoss)
		{
			RegisterPrefab("Jotunn");
			RegisterItem("Jotunn_Groundslam");
			RegisterItem("Jotunn_Groundslam2");
			RegisterItem("Jotunn_Shoot");
			RegisterItem("TrophyJotunn");
			RegisterPrefab("JotunnAltar");
			RegisterPrefab("Vegvisir_Jotunn");
		}

		if (contentConfig.Bosses.AshlandsBoss)
		{
			RegisterPrefab("HelDemon");
			RegisterItem("BlazingMace");
			RegisterItem("BlazingMace2");
			RegisterItem("BlazingMace3");
			RegisterItem("BlazingMace4");
			RegisterItem("BlazingShield");
			RegisterItem("EVA_BlazingHelm");
			RegisterItem("EVA_BlazingChest");
			RegisterItem("EVA_BlazingBoots");
			RegisterItem("Blazing_Nova");
			RegisterItem("Blazing_Meteors");
			RegisterItem("Blazing_Shoot");
			RegisterItem("TrophyHelDemon");
			RegisterPrefab("BlazingDamnedOneAltar");
			RegisterPrefab("Vegvisir_BlazingDamnedOne");
		}

		if (contentConfig.Bosses.GoldenMinibosses)
		{
			RegisterPrefab("Golden_Greydwarf_Miniboss");
			RegisterPrefab("Golden_Troll_Miniboss");
			RegisterPrefab("Golden_Wraith_Miniboss");
			RegisterPrefab("StaminaGreydwarf");
			RegisterPrefab("StaminaTroll");
			RegisterPrefab("StaminaWraith");
		}
	}

	private void RegisterSummonItems()
	{
		if (contentConfig.Bosses.MistlandsBoss)
		{
			RegisterItem("CursedEffigy", new ItemConfig
			{
				Amount = 1,
				CraftingStation = CraftingStations.GaldrTable,
				Requirements = new[]
				{
					new RequirementConfig { Item = "YggdrasilWood", Amount = 30 },
					new RequirementConfig { Item = "BlackMarble", Amount = 20 },
					new RequirementConfig { Item = "TrophySeekerQueen", Amount = 1 }
				}
			});
		}

		if (contentConfig.Bosses.DeepNorthBoss)
		{
			RegisterItem("YmirsSoulEssence", new ItemConfig
			{
				Amount = 1,
				CraftingStation = CraftingStations.BlackForge,
				Requirements = new[]
				{
					new RequirementConfig { Item = "FrostCore", Amount = 4 },
					new RequirementConfig { Item = "Ice", Amount = 20 },
					new RequirementConfig { Item = "TrophyBarka", Amount = 1 }
				}
			});
		}

		if (contentConfig.Bosses.AshlandsBoss)
		{
			RegisterItem("FenrirsHeart", new ItemConfig
			{
				Amount = 1,
				CraftingStation = CraftingStations.BlackForge,
				Requirements = new[]
				{
					new RequirementConfig { Item = "FlametalNew", Amount = 10 },
					new RequirementConfig { Item = "AskHide", Amount = 8 },
					new RequirementConfig { Item = "TrophyFader", Amount = 1 }
				}
			});
		}
	}

	private void AddLocations()
	{
		if (contentConfig.Bosses.MistlandsBoss)
		{
			AddCustomLocation("SvartalfrQueenAltar_New", Heightmap.Biome.Mistlands, 1000f, 4000f, 3, 15f, false, 1f, true, false, true);
			AddCustomLocation("Vegvisir_SvartalfrQueen", Heightmap.Biome.Mistlands, 1000f, 1000f, 35, 12f, false, 1f, true, false, true);
		}

		if (contentConfig.Bosses.DeepNorthBoss)
		{
			AddCustomLocation("JotunnAltar", Heightmap.Biome.DeepNorth, 1000f, 4000f, 3, 20f, false, 1f, true, false, true);
			AddCustomLocation("Vegvisir_Jotunn", Heightmap.Biome.DeepNorth, 1000f, 1000f, 15, 6f, false, 1f, true, false, true);
		}

		if (contentConfig.Bosses.AshlandsBoss)
		{
			AddCustomLocation("BlazingDamnedOneAltar", Heightmap.Biome.AshLands, 1000f, 4000f, 3, 15f, false, 1f, true, false, true);
			AddCustomLocation("Vegvisir_BlazingDamnedOne", Heightmap.Biome.AshLands, 1000f, 1000f, 15, 4f, false, 1f, true, false, true);
		}

		if (contentConfig.Bosses.GoldenMinibosses)
		{
			AddCustomLocation("StaminaGreydwarf", Heightmap.Biome.Meadows, 1000f, 150f, 15, 4f, false, 1f, true, false, true);
			AddCustomLocation("StaminaTroll", Heightmap.Biome.BlackForest, 8500f, 150f, 15, 4f, false, 1f, true, false, true);
			AddCustomLocation("StaminaWraith", Heightmap.Biome.Swamp, 8500f, 150f, 15, 4f, false, 1f, true, false, true);
		}

		Logger.LogInfo("Loaded EVA locations.");
	}

	private void AddCustomLocation(string prefabName, Heightmap.Biome biome, float maxAltitude, float minDistanceFromSimilar, int quantity, float exteriorRadius, bool randomRotation, float minAltitude, bool clearArea, bool unique, bool priotized)
	{
		GameObject prefab = assetBundle.LoadAsset<GameObject>(prefabName);
		if (prefab == null)
		{
			Logger.LogError("Missing location prefab " + prefabName);
			return;
		}

		GameObject container = ZoneManager.Instance.CreateLocationContainer(prefab);
		ZoneManager.Instance.AddCustomLocation(new CustomLocation(container, true, new LocationConfig
		{
			Biome = biome,
			MaxAltitude = maxAltitude,
			MinDistanceFromSimilar = minDistanceFromSimilar,
			Unique = unique,
			Quantity = quantity,
			Priotized = priotized,
			ExteriorRadius = exteriorRadius,
			RandomRotation = randomRotation,
			MinAltitude = minAltitude,
			ClearArea = clearArea
		}));
	}

	private GameObject LoadEvaGameObject(string assetName)
	{
		GameObject prefab = assetBundle.LoadAsset<GameObject>(assetName);
		if (prefab != null)
		{
			return prefab;
		}

		string needle = assetName.ToLowerInvariant();
		foreach (string path in assetBundle.GetAllAssetNames())
		{
			if (path.IndexOf(needle, StringComparison.OrdinalIgnoreCase) < 0)
			{
				continue;
			}

			prefab = assetBundle.LoadAsset<GameObject>(path);
			if (prefab != null)
			{
				Logger.LogInfo("Loaded '" + assetName + "' from bundle path " + path);
				return prefab;
			}
		}

		return null;
	}

	private void RegisterPrefab(string assetName)
	{
		string name = TrackSfx(assetName);
		GameObject prefab = LoadEvaGameObject(name);
		if (prefab == null)
		{
			Logger.LogWarning("Missing prefab " + name + " (nested on a boss prefab is OK; JSON damage override for this name will be skipped).");
			return;
		}

		PrefabManager.Instance.AddPrefab(new CustomPrefab(prefab, true));
	}

	private void RegisterItem(string assetName, ItemConfig itemConfig = null)
	{
		string name = TrackSfx(assetName);
		GameObject prefab = LoadEvaGameObject(name);
		if (prefab == null)
		{
			Logger.LogWarning("Missing item " + name);
			return;
		}

		CustomItem item = itemConfig == null
			? new CustomItem(prefab, true)
			: new CustomItem(prefab, true, itemConfig);
		ItemManager.Instance.AddItem(item);
	}

	private string TrackSfx(string assetName)
	{
		if (!assetName.Contains("sfx"))
		{
			return assetName;
		}

		if (assetName.EndsWith(":sfx"))
		{
			assetName = assetName.Substring(0, assetName.Length - 4);
		}

		sfxAssetNames.Add(assetName);
		return assetName;
	}

	private void FixSfx()
	{
		AudioSource template = PrefabManager.Cache.GetPrefab<AudioSource>("sfx_arrow_hit");
		if (template == null)
		{
			return;
		}

		foreach (string name in sfxAssetNames)
		{
			GameObject prefab = PrefabManager.Cache.GetPrefab<GameObject>(name);
			if (prefab == null)
			{
				continue;
			}

			AudioSource source = prefab.GetComponentInChildren<AudioSource>();
			if (source != null)
			{
				source.outputAudioMixerGroup = template.outputAudioMixerGroup;
			}
		}
	}

	private void ApplyBossConfig()
	{
		if (!File.Exists(configPath))
		{
			GenerateConfigFile();
			Logger.LogInfo("Generated boss config.");
			return;
		}

		List<BossConfig> configs = GetJson<BossConfig>(configPath);
		if (configs == null)
		{
			return;
		}

		if (MigrateBossBalanceIfNeeded(configs))
		{
			File.WriteAllText(configPath, JsonMapper.ToJson(configs));
			Logger.LogInfo("Migrated boss balance (Mistlands < Ashlands < Deep North).");
		}

		foreach (BossConfig config in configs)
		{
			UpdateBossHealth(config);
			UpdateBossAttacks(config);
		}

		Logger.LogInfo("Loaded boss config.");
	}

	private bool MigrateBossBalanceIfNeeded(List<BossConfig> configs)
	{
		if (!BossBalance.NeedsBalanceMigration(configs))
		{
			return false;
		}

		Dictionary<string, List<string>> attackMap = BuildBossAttackMap();
		for (int i = 0; i < configs.Count; i++)
		{
			BossConfig config = configs[i];
			if (!BossBalance.IsMainBoss(config.BossPrefabName))
			{
				continue;
			}

			if (!attackMap.TryGetValue(config.BossPrefabName, out List<string> attacks))
			{
				continue;
			}

			configs[i] = CreateBossConfig(config.BossPrefabName, attacks);
		}

		return true;
	}

	private void GenerateConfigFile()
	{
		Dictionary<string, List<string>> attacks = BuildBossAttackMap();
		List<BossConfig> list = new List<BossConfig>();
		foreach (KeyValuePair<string, List<string>> pair in attacks)
		{
			BossConfig config = CreateBossConfig(pair.Key, pair.Value);
			if (config != null)
			{
				list.Add(config);
			}
		}

		File.WriteAllText(configPath, JsonMapper.ToJson(list));
	}

	private Dictionary<string, List<string>> BuildBossAttackMap()
	{
		Dictionary<string, List<string>> attacks = new Dictionary<string, List<string>>();
		if (contentConfig.Bosses.MistlandsBoss)
		{
			attacks["SvartalfarQueen"] = new List<string>
			{
				"SvartalfarQueenGreatSword", "SvartalfarQueenBow", "SvartalfarQueen_rootspawn",
				"SvarTentaRoot_attack", "SvartalfarQueenBowArrowStorm", "bow_projectile_svar"
			};
		}

		if (contentConfig.Bosses.DeepNorthBoss)
		{
			attacks["Jotunn"] = new List<string> { "Jotunn_Groundslam", "Jotunn_Groundslam2", "Jotunn_Shoot" };
		}

		if (contentConfig.Bosses.AshlandsBoss)
		{
			attacks["HelDemon"] = new List<string>
			{
				"BlazingMace", "BlazingMace2", "BlazingMace3", "BlazingMace4",
				"Blazing_Nova", "Blazing_Shoot", "Blazing_Meteors", "projectile_meteor_blazing"
			};
		}

		if (contentConfig.Bosses.GoldenMinibosses)
		{
			attacks["Golden_Greydwarf_Miniboss"] = new List<string> { "Greydwarf_elite_attack_gold" };
			attacks["Golden_Troll_Miniboss"] = new List<string> { "troll_punch", "troll_groundslam", "troll_throw" };
			attacks["Golden_Wraith_Miniboss"] = new List<string> { "wraith_melee_gold" };
		}

		return attacks;
	}

	private BossConfig CreateBossConfig(string bossPrefabName, List<string> attackNames)
	{
		Humanoid prefab = PrefabManager.Cache.GetPrefab<Humanoid>(bossPrefabName);
		if (prefab == null)
		{
			Logger.LogWarning("Cannot generate config for " + bossPrefabName);
			return null;
		}

		int health = BossBalance.IsMainBoss(bossPrefabName)
			? BossBalance.GetDefaultHealth(bossPrefabName)
			: (int)prefab.m_health;

		BossConfig config = new BossConfig
		{
			BossPrefabName = bossPrefabName,
			Health = health,
			Attacks = new List<CustomAttack>()
		};
		AddCustomAttacks(config, attackNames);
		ScaleMainBossAttackDamages(config);
		return config;
	}

	private static void ScaleMainBossAttackDamages(BossConfig config)
	{
		if (!BossBalance.IsMainBoss(config.BossPrefabName) || config.Attacks == null)
		{
			return;
		}

		float scale = BossBalance.GetDamageScale(config.BossPrefabName);
		foreach (CustomAttack attack in config.Attacks)
		{
			if (!attack.Damages.HasValue)
			{
				continue;
			}

			attack.Damages = BossBalance.ScaleDamages(attack.Damages.Value, scale);
		}
	}

	private void AddCustomAttacks(BossConfig config, List<string> attackNames)
	{
		foreach (string attackName in attackNames)
		{
			if (attackName.ToLowerInvariant().Contains("projectile"))
			{
				Projectile projectile = PrefabManager.Cache.GetPrefab<Projectile>(attackName);
				if (projectile == null)
				{
					Logger.LogWarning("Missing projectile " + attackName);
					continue;
				}

				config.Attacks.Add(new CustomAttack
				{
					AttackName = attackName,
					Damages = projectile.m_damage
				});
				continue;
			}

			ItemDrop item = PrefabManager.Cache.GetPrefab<ItemDrop>(attackName);
			if (item == null)
			{
				Logger.LogWarning("Missing attack " + attackName);
				continue;
			}

			config.Attacks.Add(new CustomAttack
			{
				AttackName = attackName,
				AttackAnimation = item.m_itemData.m_shared.m_attack.m_attackAnimation,
				AiAttackRange = item.m_itemData.m_shared.m_aiAttackRange,
				AiAttackRangeMin = item.m_itemData.m_shared.m_aiAttackRangeMin,
				AiAttackInterval = item.m_itemData.m_shared.m_aiAttackInterval,
				Damages = item.m_itemData.m_shared.m_damages
			});
		}
	}

	private void UpdateBossHealth(BossConfig config)
	{
		try
		{
			Humanoid prefab = PrefabManager.Cache.GetPrefab<Humanoid>(config.BossPrefabName);
			if (prefab == null)
			{
				return;
			}

			prefab.m_health = config.Health;
		}
		catch (Exception ex)
		{
			Logger.LogError("Loading config for " + config.BossPrefabName + " failed. " + ex.Message);
		}
	}

	private void UpdateBossAttacks(BossConfig config)
	{
		if (config.Attacks == null)
		{
			return;
		}

		foreach (CustomAttack attack in config.Attacks)
		{
			try
			{
				if (attack.AttackName.ToLowerInvariant().Contains("projectile"))
				{
					Projectile projectile = PrefabManager.Cache.GetPrefab<Projectile>(attack.AttackName);
					if (projectile != null && attack.Damages.HasValue)
					{
						projectile.m_damage = attack.Damages.Value;
					}

					continue;
				}

				ItemDrop item = PrefabManager.Cache.GetPrefab<ItemDrop>(attack.AttackName);
				if (item == null)
				{
					continue;
				}

				if (attack.Damages.HasValue)
				{
					item.m_itemData.m_shared.m_damages = attack.Damages.Value;
				}

				if (attack.AttackAnimation != null)
				{
					item.m_itemData.m_shared.m_attack.m_attackAnimation = attack.AttackAnimation;
				}

				item.m_itemData.m_shared.m_aiAttackRange = attack.AiAttackRange;
				item.m_itemData.m_shared.m_aiAttackRangeMin = attack.AiAttackRangeMin;
				item.m_itemData.m_shared.m_aiAttackInterval = attack.AiAttackInterval;
			}
			catch (Exception ex)
			{
				Logger.LogError("Loading config for " + attack.AttackName + " failed. " + ex.Message);
			}
		}
	}

	private EVAConfiguration LoadContentConfig()
	{
		EVAConfiguration defaults = GetDefaultEVAConfiguration();
		if (!File.Exists(configPath2))
		{
			SaveEVAConfiguration(defaults);
			Logger.LogInfo("Generated content configuration.");
			return defaults;
		}

		string json = File.ReadAllText(configPath2);
		List<EVAConfiguration> list = JsonMapper.ToObject<List<EVAConfiguration>>(json);
		if (list == null || list.Count == 0)
		{
			SaveEVAConfiguration(defaults);
			return defaults;
		}

		EVAConfiguration loaded = list[0];
		if (loaded.Bosses != null)
		{
			Logger.LogInfo("Loaded content configuration.");
			return loaded;
		}

		if (loaded.Locations != null || loaded.Content != null)
		{
			EVAConfiguration fromNested = new EVAConfiguration
			{
				Content_Description = contentDescription,
				Bosses = new BossToggleConfiguration
				{
					MistlandsBoss = loaded.Locations?.MistlandsLocations ?? loaded.Content?.Mistlands ?? true,
					DeepNorthBoss = loaded.Locations?.DeepNorthLocations ?? loaded.Content?.DeepNorth ?? true,
					AshlandsBoss = loaded.Locations?.AshlandsLocations ?? loaded.Content?.Ashlands ?? true,
					GoldenMinibosses = true
				}
			};
			SaveEVAConfiguration(fromNested);
			Logger.LogInfo("Migrated nested content configuration to boss-only toggles.");
			return fromNested;
		}

		List<OldConfiguration> oldList = JsonMapper.ToObject<List<OldConfiguration>>(json);
		OldConfiguration old = oldList != null && oldList.Count > 0 ? oldList[0] : null;
		EVAConfiguration migrated = new EVAConfiguration
		{
			Content_Description = contentDescription,
			Bosses = new BossToggleConfiguration
			{
				MistlandsBoss = old?.MistlandsLocations ?? old?.Mistlands ?? true,
				DeepNorthBoss = old?.DeepNorthLocations ?? old?.DeepNorth ?? true,
				AshlandsBoss = old?.AshlandsLocations ?? old?.Ashlands ?? true,
				GoldenMinibosses = true
			}
		};
		SaveEVAConfiguration(migrated);
		Logger.LogInfo("Migrated old content configuration to boss-only toggles.");
		return migrated;
	}

	private void SaveEVAConfiguration(EVAConfiguration config)
	{
		config.Content_Description = contentDescription;
		File.WriteAllText(configPath2, JsonMapper.ToJson(new List<EVAConfiguration> { config }));
	}

	private EVAConfiguration GetDefaultEVAConfiguration()
	{
		return new EVAConfiguration
		{
			Content_Description = contentDescription,
			Bosses = new BossToggleConfiguration
			{
				MistlandsBoss = true,
				DeepNorthBoss = true,
				AshlandsBoss = true,
				GoldenMinibosses = true
			},
			GoldenMinibossCooldownMinutes = 120
		};
	}

	internal static List<T> GetJson<T>(string path)
	{
		string json = AssetUtils.LoadText(path);
		return JsonMapper.ToObject<List<T>>(json);
	}
}
