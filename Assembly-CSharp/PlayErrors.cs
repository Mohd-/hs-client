using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// Token: 0x020000BB RID: 187
public class PlayErrors
{
	// Token: 0x060009F6 RID: 2550 RVA: 0x0002B424 File Offset: 0x00029624
	public static bool Init()
	{
		if (!PlayErrors.PLAYERRORS_ENABLED)
		{
			return true;
		}
		if (PlayErrors.s_initialized)
		{
			return true;
		}
		if (!PlayErrors.LoadDLL())
		{
			return false;
		}
		PlayErrors.s_initialized = PlayErrors.DLL_PlayErrorsInit();
		Log.PlayErrors.Print("Init: " + PlayErrors.s_initialized, new object[0]);
		if (!PlayErrors.s_initialized)
		{
			PlayErrors.UnloadDLL();
		}
		return PlayErrors.s_initialized;
	}

	// Token: 0x060009F7 RID: 2551 RVA: 0x0002B49C File Offset: 0x0002969C
	public static bool IsInitialized()
	{
		return PlayErrors.s_initialized;
	}

	// Token: 0x060009F8 RID: 2552 RVA: 0x0002B4A3 File Offset: 0x000296A3
	public static void AppQuit()
	{
		if (PlayErrors.PLAYERRORS_ENABLED)
		{
			PlayErrors.UnloadDLL();
			PlayErrors.s_initialized = false;
			Log.PlayErrors.Print("AppQuit: " + PlayErrors.s_initialized, new object[0]);
		}
	}

	// Token: 0x060009F9 RID: 2553 RVA: 0x0002B4E0 File Offset: 0x000296E0
	public static void DisplayPlayError(PlayErrors.ErrorType error, Entity errorSource)
	{
		Log.PlayErrors.Print(string.Concat(new object[]
		{
			"DisplayPlayError: (",
			PlayErrors.s_initialized,
			") ",
			error,
			" ",
			errorSource
		}), new object[0]);
		if (!PlayErrors.s_initialized)
		{
			return;
		}
		if (GameState.Get().GetGameEntity().NotifyOfPlayError(error, errorSource))
		{
			return;
		}
		switch (error)
		{
		case PlayErrors.ErrorType.REQ_MINION_TARGET:
		case PlayErrors.ErrorType.REQ_FRIENDLY_TARGET:
		case PlayErrors.ErrorType.REQ_ENEMY_TARGET:
		case PlayErrors.ErrorType.REQ_DAMAGED_TARGET:
		case PlayErrors.ErrorType.REQ_FROZEN_TARGET:
		case PlayErrors.ErrorType.REQ_TARGET_MAX_ATTACK:
		case PlayErrors.ErrorType.REQ_TARGET_WITH_RACE:
		case PlayErrors.ErrorType.REQ_HERO_TARGET:
		case PlayErrors.ErrorType.REQ_HERO_OR_MINION_TARGET:
		case PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_SPELLS:
		case PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_OPPONENTS:
		case PlayErrors.ErrorType.REQ_TARGET_MIN_ATTACK:
		case PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_HERO_POWERS:
		case PlayErrors.ErrorType.REQ_ENEMY_TARGET_NOT_IMMUNE:
		case PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_BATTLECRIES:
		case PlayErrors.ErrorType.REQ_MINION_OR_ENEMY_HERO:
		case PlayErrors.ErrorType.REQ_LEGENDARY_TARGET:
		case PlayErrors.ErrorType.REQ_TARGET_WITH_BATTLECRY:
		case PlayErrors.ErrorType.REQ_TARGET_WITH_DEATHRATTLE:
			GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(EmoteType.ERROR_TARGET);
			goto IL_2CE;
		case PlayErrors.ErrorType.REQ_TARGET_TO_PLAY:
		case PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE:
		case PlayErrors.ErrorType.REQ_TARGET_FOR_COMBO:
		case PlayErrors.ErrorType.REQ_TARGET_FOR_NO_COMBO:
		case PlayErrors.ErrorType.REQ_STEADY_SHOT:
		case PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND:
		case PlayErrors.ErrorType.REQ_FRIENDLY_MINION_DIED_THIS_TURN:
		case PlayErrors.ErrorType.REQ_FRIENDLY_MINION_DIED_THIS_GAME:
		case PlayErrors.ErrorType.REQ_ENEMY_WEAPON_EQUIPPED:
		case PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS:
			GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(EmoteType.ERROR_PLAY);
			goto IL_2CE;
		case PlayErrors.ErrorType.REQ_NUM_MINION_SLOTS:
		case PlayErrors.ErrorType.REQ_MINION_CAP_IF_TARGET_AVAILABLE:
		case PlayErrors.ErrorType.REQ_MINION_CAP:
			GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(EmoteType.ERROR_FULL_MINIONS);
			goto IL_2CE;
		case PlayErrors.ErrorType.REQ_WEAPON_EQUIPPED:
			GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(EmoteType.ERROR_NEED_WEAPON);
			goto IL_2CE;
		case PlayErrors.ErrorType.REQ_ENOUGH_MANA:
			if (errorSource.IsSpell() && PlayErrors.DoSpellsCostHealth())
			{
				GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(EmoteType.ERROR_PLAY);
			}
			else
			{
				GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(EmoteType.ERROR_NEED_MANA);
			}
			goto IL_2CE;
		case PlayErrors.ErrorType.REQ_YOUR_TURN:
			return;
		case PlayErrors.ErrorType.REQ_NONSTEALTH_ENEMY_TARGET:
			GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(EmoteType.ERROR_STEALTH);
			goto IL_2CE;
		case PlayErrors.ErrorType.REQ_NOT_EXHAUSTED_ACTIVATE:
			if (errorSource.IsHero())
			{
				GameState.Get().GetCurrentPlayer().GetHeroCard().PlayEmote(EmoteType.ERROR_I_ATTACKED);
			}
			else
			{
				GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(EmoteType.ERROR_MINION_ATTACKED);
			}
			goto IL_2CE;
		case PlayErrors.ErrorType.REQ_TARGET_TAUNTER:
			PlayErrors.DisplayTauntErrorEffects();
			goto IL_2CE;
		case PlayErrors.ErrorType.REQ_NOT_MINION_JUST_PLAYED:
			GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(EmoteType.ERROR_JUST_PLAYED);
			goto IL_2CE;
		case PlayErrors.ErrorType.REQ_DRAG_TO_PLAY:
			goto IL_2CE;
		}
		GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(EmoteType.ERROR_GENERIC);
		IL_2CE:
		PlayErrors.PlayRequirementInfo playRequirementInfo = PlayErrors.GetPlayRequirementInfo(errorSource);
		string errorDescription = PlayErrors.GetErrorDescription(error, errorSource, playRequirementInfo);
		if (string.IsNullOrEmpty(errorDescription))
		{
			return;
		}
		GameplayErrorManager.Get().DisplayMessage(errorDescription);
	}

	// Token: 0x060009FA RID: 2554 RVA: 0x0002B7E4 File Offset: 0x000299E4
	public static PlayErrors.ErrorType GetPlayEntityError(Entity source)
	{
		Log.PlayErrors.Print(string.Concat(new object[]
		{
			"GetPlayEntityError (",
			PlayErrors.s_initialized,
			") ",
			source
		}), new object[0]);
		if (!PlayErrors.s_initialized)
		{
			return PlayErrors.ErrorType.NONE;
		}
		Player owningPlayer = PlayErrors.GetOwningPlayer(source);
		if (owningPlayer == null)
		{
			return PlayErrors.ErrorType.NONE;
		}
		PlayErrors.SourceEntityInfo sourceInfo = source.ConvertToSourceInfo(PlayErrors.GetPlayRequirementInfo(source), null);
		PlayErrors.PlayerInfo playerInfo = owningPlayer.ConvertToPlayerInfo();
		PlayErrors.GameStateInfo gameInfo = GameState.Get().ConvertToGameStateInfo();
		PlayErrors.Marshaled_PlayErrorsParams playErrorsParams = new PlayErrors.Marshaled_PlayErrorsParams(sourceInfo, playerInfo, gameInfo);
		List<PlayErrors.Marshaled_TargetEntityInfo> marshaledEntitiesInPlay = PlayErrors.GetMarshaledEntitiesInPlay();
		List<PlayErrors.Marshaled_SourceEntityInfo> marshaledSubCards = PlayErrors.GetMarshaledSubCards(source);
		return PlayErrors.DLL_GetPlayEntityError(playErrorsParams, marshaledSubCards.ToArray(), marshaledSubCards.Count, marshaledEntitiesInPlay.ToArray(), marshaledEntitiesInPlay.Count);
	}

	// Token: 0x060009FB RID: 2555 RVA: 0x0002B8AC File Offset: 0x00029AAC
	public static PlayErrors.ErrorType GetTargetEntityError(Entity source, Entity target)
	{
		if (!PlayErrors.s_initialized)
		{
			return PlayErrors.ErrorType.NONE;
		}
		Player owningPlayer = PlayErrors.GetOwningPlayer(source);
		if (owningPlayer == null)
		{
			return PlayErrors.ErrorType.NONE;
		}
		PlayErrors.SourceEntityInfo sourceInfo = source.ConvertToSourceInfo(PlayErrors.GetPlayRequirementInfo(source), null);
		PlayErrors.PlayerInfo playerInfo = owningPlayer.ConvertToPlayerInfo();
		PlayErrors.GameStateInfo gameInfo = GameState.Get().ConvertToGameStateInfo();
		PlayErrors.Marshaled_PlayErrorsParams playErrorsParams = new PlayErrors.Marshaled_PlayErrorsParams(sourceInfo, playerInfo, gameInfo);
		PlayErrors.Marshaled_TargetEntityInfo target2 = PlayErrors.Marshaled_TargetEntityInfo.ConvertFromTargetEntityInfo(target.ConvertToTargetInfo());
		List<PlayErrors.Marshaled_TargetEntityInfo> marshaledEntitiesInPlay = PlayErrors.GetMarshaledEntitiesInPlay();
		return PlayErrors.DLL_GetTargetEntityError(playErrorsParams, target2, marshaledEntitiesInPlay.ToArray(), marshaledEntitiesInPlay.Count);
	}

	// Token: 0x060009FC RID: 2556 RVA: 0x0002B930 File Offset: 0x00029B30
	public static ulong GetRequirementsMap(List<PlayErrors.ErrorType> requirements)
	{
		if (!PlayErrors.s_initialized)
		{
			return 0UL;
		}
		return PlayErrors.DLL_GetRequirementsMap(requirements.ToArray(), requirements.Count);
	}

	// Token: 0x060009FD RID: 2557 RVA: 0x0002B960 File Offset: 0x00029B60
	private static Player GetOwningPlayer(Entity entity)
	{
		Player player = GameState.Get().GetPlayer(entity.GetControllerId());
		if (player == null)
		{
			Log.PlayErrors.Print(string.Format("Error retrieving controlling player of entity {0} in PlayErrors.GetOwningPlayer()!", entity.GetName()), new object[0]);
		}
		return player;
	}

	// Token: 0x060009FE RID: 2558 RVA: 0x0002B9A8 File Offset: 0x00029BA8
	private static PlayErrors.PlayRequirementInfo GetPlayRequirementInfo(Entity entity)
	{
		if (entity.GetZone() == TAG_ZONE.HAND)
		{
			return entity.GetMasterPower().GetPlayRequirementInfo();
		}
		if (entity.GetZone() == TAG_ZONE.SETASIDE)
		{
			return entity.GetMasterPower().GetPlayRequirementInfo();
		}
		if (entity.IsHeroPower())
		{
			return entity.GetMasterPower().GetPlayRequirementInfo();
		}
		if (entity.ShouldUseBattlecryPower())
		{
			return entity.GetMasterPower().GetPlayRequirementInfo();
		}
		return entity.GetAttackPower().GetPlayRequirementInfo();
	}

	// Token: 0x060009FF RID: 2559 RVA: 0x0002BA20 File Offset: 0x00029C20
	private static List<PlayErrors.Marshaled_SourceEntityInfo> GetMarshaledSubCards(Entity source)
	{
		List<PlayErrors.Marshaled_SourceEntityInfo> list = new List<PlayErrors.Marshaled_SourceEntityInfo>();
		foreach (int id in source.GetSubCardIDs())
		{
			Entity entity = GameState.Get().GetEntity(id);
			if (entity == null)
			{
				Log.PlayErrors.Print(string.Format("Subcard of {0} is null in GetMarshaledSubCards()!", source.GetName()), new object[0]);
			}
			else
			{
				PlayErrors.SourceEntityInfo sourceInfo = entity.ConvertToSourceInfo(Power.GetDefaultMasterPower().GetPlayRequirementInfo(), source);
				list.Add(PlayErrors.Marshaled_SourceEntityInfo.ConvertFromSourceEntityInfo(sourceInfo));
			}
		}
		return list;
	}

	// Token: 0x06000A00 RID: 2560 RVA: 0x0002BAD4 File Offset: 0x00029CD4
	private static List<PlayErrors.Marshaled_TargetEntityInfo> GetMarshaledEntitiesInPlay()
	{
		List<PlayErrors.Marshaled_TargetEntityInfo> list = new List<PlayErrors.Marshaled_TargetEntityInfo>();
		foreach (Zone zone in ZoneMgr.Get().FindZonesForTag(TAG_ZONE.PLAY))
		{
			foreach (Card card in zone.GetCards())
			{
				Entity entity = card.GetEntity();
				if (entity.GetZone() == TAG_ZONE.PLAY)
				{
					list.Add(PlayErrors.Marshaled_TargetEntityInfo.ConvertFromTargetEntityInfo(entity.ConvertToTargetInfo()));
				}
			}
		}
		return list;
	}

	// Token: 0x06000A01 RID: 2561 RVA: 0x0002BBA4 File Offset: 0x00029DA4
	private static bool CanShowMinionTauntError()
	{
		Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
		int num;
		int num2;
		GameState.Get().GetTauntCounts(opposingSidePlayer, out num, out num2);
		return num > 0 && num2 == 0;
	}

	// Token: 0x06000A02 RID: 2562 RVA: 0x0002BBDC File Offset: 0x00029DDC
	private static void DisplayTauntErrorEffects()
	{
		if (PlayErrors.CanShowMinionTauntError())
		{
			GameState.Get().GetFriendlySidePlayer().GetHeroCard().PlayEmote(EmoteType.ERROR_TAUNT);
		}
		GameState.Get().ShowEnemyTauntCharacters();
	}

	// Token: 0x06000A03 RID: 2563 RVA: 0x0002BC14 File Offset: 0x00029E14
	private static bool DoSpellsCostHealth()
	{
		Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
		return friendlySidePlayer.HasTag(GAME_TAG.SPELLS_COST_HEALTH);
	}

	// Token: 0x06000A04 RID: 2564 RVA: 0x0002BC38 File Offset: 0x00029E38
	private static string GetErrorDescription(PlayErrors.ErrorType type, Entity errorSource, PlayErrors.PlayRequirementInfo requirementInfo)
	{
		Log.PlayErrors.Print(string.Concat(new object[]
		{
			"GetErrorDescription: ",
			type,
			" ",
			requirementInfo
		}), new object[0]);
		switch (type)
		{
		case PlayErrors.ErrorType.REQ_ENOUGH_MANA:
			if (errorSource.IsSpell() && PlayErrors.DoSpellsCostHealth())
			{
				return GameStrings.Get("GAMEPLAY_PlayErrors_REQ_ENOUGH_HEALTH");
			}
			return GameStrings.Get("GAMEPLAY_PlayErrors_REQ_ENOUGH_MANA");
		case PlayErrors.ErrorType.REQ_YOUR_TURN:
			return string.Empty;
		default:
			switch (type)
			{
			case PlayErrors.ErrorType.REQ_TARGET_MAX_ATTACK:
				return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_TARGET_MAX_ATTACK", new object[]
				{
					requirementInfo.paramMaxAtk
				});
			default:
				switch (type)
				{
				case PlayErrors.ErrorType.REQ_TARGET_TAUNTER:
					if (PlayErrors.CanShowMinionTauntError())
					{
						return GameStrings.Get("GAMEPLAY_PlayErrors_REQ_TARGET_TAUNTER_MINION");
					}
					return GameStrings.Get("GAMEPLAY_PlayErrors_REQ_TARGET_TAUNTER_CHARACTER");
				default:
				{
					if (type == PlayErrors.ErrorType.NONE)
					{
						Debug.LogWarning("PlayErrors.GetErrorDescription() - Action is not valid, but no error string found.");
						return string.Empty;
					}
					if (type == PlayErrors.ErrorType.REQ_MINIMUM_ENEMY_MINIONS)
					{
						return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_MINIMUM_ENEMY_MINIONS", new object[]
						{
							requirementInfo.paramMinNumEnemyMinions
						});
					}
					if (type == PlayErrors.ErrorType.REQ_TARGET_MIN_ATTACK)
					{
						return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_TARGET_MIN_ATTACK", new object[]
						{
							requirementInfo.paramMinAtk
						});
					}
					if (type == PlayErrors.ErrorType.REQ_MINIMUM_TOTAL_MINIONS)
					{
						return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_MINIMUM_TOTAL_MINIONS", new object[]
						{
							requirementInfo.paramMinNumTotalMinions
						});
					}
					string key = null;
					if (PlayErrors.s_playErrorsMessages.TryGetValue(type, out key))
					{
						return GameStrings.Get(key);
					}
					return PlayErrors.ErrorInEditorOnly("[Unity Editor] Unknown play error ({0})", new object[]
					{
						type
					});
				}
				case PlayErrors.ErrorType.REQ_ACTION_PWR_IS_MASTER_PWR:
					return PlayErrors.ErrorInEditorOnly("[Unity Editor] Action power must be master power", new object[0]);
				}
				break;
			case PlayErrors.ErrorType.REQ_TARGET_WITH_RACE:
				return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_TARGET_WITH_RACE", new object[]
				{
					GameStrings.GetRaceName((TAG_RACE)requirementInfo.paramRace)
				});
			}
			break;
		case PlayErrors.ErrorType.REQ_SECRET_CAP:
			return GameStrings.Format("GAMEPLAY_PlayErrors_REQ_SECRET_CAP", new object[]
			{
				GameState.Get().GetMaxSecretsPerPlayer()
			});
		}
	}

	// Token: 0x06000A05 RID: 2565 RVA: 0x0002BE4C File Offset: 0x0002A04C
	private static string ErrorInEditorOnly(string format, params object[] args)
	{
		return string.Empty;
	}

	// Token: 0x06000A06 RID: 2566 RVA: 0x0002BE54 File Offset: 0x0002A054
	private static IntPtr GetFunction(string name)
	{
		IntPtr procAddress = DLLUtils.GetProcAddress(PlayErrors.s_DLL, name);
		if (procAddress == IntPtr.Zero)
		{
			Debug.LogError("Could not load PlayErrors." + name + "()");
			PlayErrors.AppQuit();
		}
		return procAddress;
	}

	// Token: 0x06000A07 RID: 2567 RVA: 0x0002BE98 File Offset: 0x0002A098
	private static bool LoadDLL()
	{
		PlayErrors.s_DLL = FileUtils.LoadPlugin("PlayErrors32", true);
		if (PlayErrors.s_DLL == IntPtr.Zero)
		{
			return false;
		}
		PlayErrors.DLL_PlayErrorsInit = (PlayErrors.DelPlayErrorsInit)Marshal.GetDelegateForFunctionPointer(PlayErrors.GetFunction("PlayErrorsInit"), typeof(PlayErrors.DelPlayErrorsInit));
		PlayErrors.DLL_GetRequirementsMap = (PlayErrors.DelGetRequirementsMap)Marshal.GetDelegateForFunctionPointer(PlayErrors.GetFunction("GetRequirementsMap"), typeof(PlayErrors.DelGetRequirementsMap));
		PlayErrors.DLL_GetPlayEntityError = (PlayErrors.DelGetPlayEntityError)Marshal.GetDelegateForFunctionPointer(PlayErrors.GetFunction("GetPlayEntityError"), typeof(PlayErrors.DelGetPlayEntityError));
		PlayErrors.DLL_GetTargetEntityError = (PlayErrors.DelGetTargetEntityError)Marshal.GetDelegateForFunctionPointer(PlayErrors.GetFunction("GetTargetEntityError"), typeof(PlayErrors.DelGetTargetEntityError));
		return PlayErrors.s_DLL != IntPtr.Zero;
	}

	// Token: 0x06000A08 RID: 2568 RVA: 0x0002BF68 File Offset: 0x0002A168
	private static void UnloadDLL()
	{
		if (PlayErrors.s_initialized)
		{
			Log.PlayErrors.Print("Unloading PlayErrors DLL..", new object[0]);
			if (!DLLUtils.FreeLibrary(PlayErrors.s_DLL))
			{
				Debug.LogError(string.Format("error unloading {0}", "PlayErrors32"));
			}
			PlayErrors.s_DLL = IntPtr.Zero;
		}
	}

	// Token: 0x040004CD RID: 1229
	public const string PLAYERRORS_DLL_FILENAME = "PlayErrors32";

	// Token: 0x040004CE RID: 1230
	private static bool PLAYERRORS_ENABLED = true;

	// Token: 0x040004CF RID: 1231
	private static bool s_initialized = false;

	// Token: 0x040004D0 RID: 1232
	private static Map<PlayErrors.ErrorType, string> s_playErrorsMessages = new Map<PlayErrors.ErrorType, string>
	{
		{
			PlayErrors.ErrorType.REQ_MINION_TARGET,
			"GAMEPLAY_PlayErrors_REQ_MINION_TARGET"
		},
		{
			PlayErrors.ErrorType.REQ_FRIENDLY_TARGET,
			"GAMEPLAY_PlayErrors_REQ_FRIENDLY_TARGET"
		},
		{
			PlayErrors.ErrorType.REQ_ENEMY_TARGET,
			"GAMEPLAY_PlayErrors_REQ_ENEMY_TARGET"
		},
		{
			PlayErrors.ErrorType.REQ_DAMAGED_TARGET,
			"GAMEPLAY_PlayErrors_REQ_DAMAGED_TARGET"
		},
		{
			PlayErrors.ErrorType.REQ_ENCHANTED_TARGET,
			"GAMEPLAY_PlayErrors_REQ_ENCHANTED_TARGET"
		},
		{
			PlayErrors.ErrorType.REQ_FROZEN_TARGET,
			"GAMEPLAY_PlayErrors_REQ_FROZEN_TARGET"
		},
		{
			PlayErrors.ErrorType.REQ_CHARGE_TARGET,
			"GAMEPLAY_PlayErrors_REQ_CHARGE_TARGET"
		},
		{
			PlayErrors.ErrorType.REQ_TARGET_MAX_ATTACK,
			"GAMEPLAY_PlayErrors_REQ_TARGET_MAX_ATTACK"
		},
		{
			PlayErrors.ErrorType.REQ_NONSELF_TARGET,
			"GAMEPLAY_PlayErrors_REQ_NONSELF_TARGET"
		},
		{
			PlayErrors.ErrorType.REQ_TARGET_WITH_RACE,
			"GAMEPLAY_PlayErrors_REQ_TARGET_WITH_RACE"
		},
		{
			PlayErrors.ErrorType.REQ_TARGET_TO_PLAY,
			"GAMEPLAY_PlayErrors_REQ_TARGET_TO_PLAY"
		},
		{
			PlayErrors.ErrorType.REQ_NUM_MINION_SLOTS,
			"GAMEPLAY_PlayErrors_REQ_NUM_MINION_SLOTS"
		},
		{
			PlayErrors.ErrorType.REQ_WEAPON_EQUIPPED,
			"GAMEPLAY_PlayErrors_REQ_WEAPON_EQUIPPED"
		},
		{
			PlayErrors.ErrorType.REQ_YOUR_TURN,
			"GAMEPLAY_PlayErrors_REQ_YOUR_TURN"
		},
		{
			PlayErrors.ErrorType.REQ_NONSTEALTH_ENEMY_TARGET,
			"GAMEPLAY_PlayErrors_REQ_NONSTEALTH_ENEMY_TARGET"
		},
		{
			PlayErrors.ErrorType.REQ_HERO_TARGET,
			"GAMEPLAY_PlayErrors_REQ_HERO_TARGET"
		},
		{
			PlayErrors.ErrorType.REQ_SECRET_CAP,
			"GAMEPLAY_PlayErrors_REQ_SECRET_CAP"
		},
		{
			PlayErrors.ErrorType.REQ_MINION_CAP_IF_TARGET_AVAILABLE,
			"GAMEPLAY_PlayErrors_REQ_MINION_CAP_IF_TARGET_AVAILABLE"
		},
		{
			PlayErrors.ErrorType.REQ_MINION_CAP,
			"GAMEPLAY_PlayErrors_REQ_MINION_CAP"
		},
		{
			PlayErrors.ErrorType.REQ_TARGET_ATTACKED_THIS_TURN,
			"GAMEPLAY_PlayErrors_REQ_TARGET_ATTACKED_THIS_TURN"
		},
		{
			PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE,
			"GAMEPLAY_PlayErrors_REQ_TARGET_IF_AVAILABLE"
		},
		{
			PlayErrors.ErrorType.REQ_MINIMUM_ENEMY_MINIONS,
			"GAMEPLAY_PlayErrors_REQ_MINIMUM_ENEMY_MINIONS"
		},
		{
			PlayErrors.ErrorType.REQ_TARGET_FOR_COMBO,
			"GAMEPLAY_PlayErrors_REQ_TARGET_FOR_COMBO"
		},
		{
			PlayErrors.ErrorType.REQ_NOT_EXHAUSTED_ACTIVATE,
			"GAMEPLAY_PlayErrors_REQ_NOT_EXHAUSTED_ACTIVATE"
		},
		{
			PlayErrors.ErrorType.REQ_UNIQUE_SECRET,
			"GAMEPLAY_PlayErrors_REQ_UNIQUE_SECRET"
		},
		{
			PlayErrors.ErrorType.REQ_CAN_BE_ATTACKED,
			"GAMEPLAY_PlayErrors_REQ_CAN_BE_ATTACKED"
		},
		{
			PlayErrors.ErrorType.REQ_ACTION_PWR_IS_MASTER_PWR,
			"GAMEPLAY_PlayErrors_REQ_ACTION_PWR_IS_MASTER_PWR"
		},
		{
			PlayErrors.ErrorType.REQ_TARGET_MAGNET,
			"GAMEPLAY_PlayErrors_REQ_TARGET_MAGNET"
		},
		{
			PlayErrors.ErrorType.REQ_ATTACK_GREATER_THAN_0,
			"GAMEPLAY_PlayErrors_REQ_ATTACK_GREATER_THAN_0"
		},
		{
			PlayErrors.ErrorType.REQ_ATTACKER_NOT_FROZEN,
			"GAMEPLAY_PlayErrors_REQ_ATTACKER_NOT_FROZEN"
		},
		{
			PlayErrors.ErrorType.REQ_HERO_OR_MINION_TARGET,
			"GAMEPLAY_PlayErrors_REQ_HERO_OR_MINION_TARGET"
		},
		{
			PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_SPELLS,
			"GAMEPLAY_PlayErrors_REQ_CAN_BE_TARGETED_BY_SPELLS"
		},
		{
			PlayErrors.ErrorType.REQ_SUBCARD_IS_PLAYABLE,
			"GAMEPLAY_PlayErrors_REQ_SUBCARD_IS_PLAYABLE"
		},
		{
			PlayErrors.ErrorType.REQ_TARGET_FOR_NO_COMBO,
			"GAMEPLAY_PlayErrors_REQ_TARGET_FOR_NO_COMBO"
		},
		{
			PlayErrors.ErrorType.REQ_NOT_MINION_JUST_PLAYED,
			"GAMEPLAY_PlayErrors_REQ_NOT_MINION_JUST_PLAYED"
		},
		{
			PlayErrors.ErrorType.REQ_NOT_EXHAUSTED_HERO_POWER,
			"GAMEPLAY_PlayErrors_REQ_NOT_EXHAUSTED_HERO_POWER"
		},
		{
			PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_OPPONENTS,
			"GAMEPLAY_PlayErrors_REQ_CAN_BE_TARGETED_BY_OPPONENTS"
		},
		{
			PlayErrors.ErrorType.REQ_ATTACKER_CAN_ATTACK,
			"GAMEPLAY_PlayErrors_REQ_ATTACKER_CAN_ATTACK"
		},
		{
			PlayErrors.ErrorType.REQ_TARGET_MIN_ATTACK,
			"GAMEPLAY_PlayErrors_REQ_TARGET_MIN_ATTACK"
		},
		{
			PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_HERO_POWERS,
			"GAMEPLAY_PlayErrors_REQ_CAN_BE_TARGETED_BY_HERO_POWERS"
		},
		{
			PlayErrors.ErrorType.REQ_ENEMY_TARGET_NOT_IMMUNE,
			"GAMEPLAY_PlayErrors_REQ_ENEMY_TARGET_NOT_IMMUNE"
		},
		{
			PlayErrors.ErrorType.REQ_ENTIRE_ENTOURAGE_NOT_IN_PLAY,
			"GAMEPLAY_PlayErrors_REQ_ENTIRE_ENTOURAGE_NOT_IN_PLAY"
		},
		{
			PlayErrors.ErrorType.REQ_MINIMUM_TOTAL_MINIONS,
			"GAMEPLAY_PlayErrors_REQ_MINIMUM_TOTAL_MINIONS"
		},
		{
			PlayErrors.ErrorType.REQ_MUST_TARGET_TAUNTER,
			"GAMEPLAY_PlayErrors_REQ_MUST_TARGET_TAUNTER"
		},
		{
			PlayErrors.ErrorType.REQ_UNDAMAGED_TARGET,
			"GAMEPLAY_PlayErrors_REQ_UNDAMAGED_TARGET"
		},
		{
			PlayErrors.ErrorType.REQ_CAN_BE_TARGETED_BY_BATTLECRIES,
			"GAMEPLAY_PlayErrors_REQ_CAN_BE_TARGETED_BY_BATTLECRIES"
		},
		{
			PlayErrors.ErrorType.REQ_STEADY_SHOT,
			"GAMEPLAY_PlayErrors_REQ_STEADY_SHOT"
		},
		{
			PlayErrors.ErrorType.REQ_MINION_OR_ENEMY_HERO,
			"GAMEPLAY_PlayErrors_REQ_MINION_OR_ENEMY_HERO"
		},
		{
			PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND,
			"GAMEPLAY_PlayErrors_REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND"
		},
		{
			PlayErrors.ErrorType.REQ_LEGENDARY_TARGET,
			"GAMEPLAY_PlayErrors_REQ_LEGENDARY_TARGET"
		},
		{
			PlayErrors.ErrorType.REQ_FRIENDLY_MINION_DIED_THIS_TURN,
			"GAMEPLAY_PlayErrors_REQ_FRIENDLY_MINION_DIED_THIS_TURN"
		},
		{
			PlayErrors.ErrorType.REQ_FRIENDLY_MINION_DIED_THIS_GAME,
			"GAMEPLAY_PlayErrors_REQ_FRIENDLY_MINION_DIED_THIS_GAME"
		},
		{
			PlayErrors.ErrorType.REQ_ENEMY_WEAPON_EQUIPPED,
			"GAMEPLAY_PlayErrors_REQ_ENEMY_WEAPON_EQUIPPED"
		},
		{
			PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS,
			"GAMEPLAY_PlayErrors_REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS"
		},
		{
			PlayErrors.ErrorType.REQ_TARGET_WITH_BATTLECRY,
			"GAMEPLAY_PlayErrors_REQ_TARGET_WITH_BATTLECRY"
		},
		{
			PlayErrors.ErrorType.REQ_TARGET_WITH_DEATHRATTLE,
			"GAMEPLAY_PlayErrors_REQ_TARGET_WITH_DEATHRATTLE"
		},
		{
			PlayErrors.ErrorType.REQ_DRAG_TO_PLAY,
			"GAMEPLAY_PlayErrors_REQ_DRAG_TO_PLAY"
		}
	};

	// Token: 0x040004D1 RID: 1233
	private static PlayErrors.DelPlayErrorsInit DLL_PlayErrorsInit;

	// Token: 0x040004D2 RID: 1234
	private static PlayErrors.DelGetRequirementsMap DLL_GetRequirementsMap;

	// Token: 0x040004D3 RID: 1235
	private static PlayErrors.DelGetPlayEntityError DLL_GetPlayEntityError;

	// Token: 0x040004D4 RID: 1236
	private static PlayErrors.DelGetTargetEntityError DLL_GetTargetEntityError;

	// Token: 0x040004D5 RID: 1237
	private static IntPtr s_DLL = IntPtr.Zero;

	// Token: 0x0200034A RID: 842
	public enum ErrorType
	{
		// Token: 0x04001A84 RID: 6788
		NONE,
		// Token: 0x04001A85 RID: 6789
		REQ_MINION_TARGET,
		// Token: 0x04001A86 RID: 6790
		REQ_FRIENDLY_TARGET,
		// Token: 0x04001A87 RID: 6791
		REQ_ENEMY_TARGET,
		// Token: 0x04001A88 RID: 6792
		REQ_DAMAGED_TARGET,
		// Token: 0x04001A89 RID: 6793
		REQ_ENCHANTED_TARGET,
		// Token: 0x04001A8A RID: 6794
		REQ_FROZEN_TARGET,
		// Token: 0x04001A8B RID: 6795
		REQ_CHARGE_TARGET,
		// Token: 0x04001A8C RID: 6796
		REQ_TARGET_MAX_ATTACK,
		// Token: 0x04001A8D RID: 6797
		REQ_NONSELF_TARGET,
		// Token: 0x04001A8E RID: 6798
		REQ_TARGET_WITH_RACE,
		// Token: 0x04001A8F RID: 6799
		REQ_TARGET_TO_PLAY,
		// Token: 0x04001A90 RID: 6800
		REQ_NUM_MINION_SLOTS,
		// Token: 0x04001A91 RID: 6801
		REQ_WEAPON_EQUIPPED,
		// Token: 0x04001A92 RID: 6802
		REQ_ENOUGH_MANA,
		// Token: 0x04001A93 RID: 6803
		REQ_YOUR_TURN,
		// Token: 0x04001A94 RID: 6804
		REQ_NONSTEALTH_ENEMY_TARGET,
		// Token: 0x04001A95 RID: 6805
		REQ_HERO_TARGET,
		// Token: 0x04001A96 RID: 6806
		REQ_SECRET_CAP,
		// Token: 0x04001A97 RID: 6807
		REQ_MINION_CAP_IF_TARGET_AVAILABLE,
		// Token: 0x04001A98 RID: 6808
		REQ_MINION_CAP,
		// Token: 0x04001A99 RID: 6809
		REQ_TARGET_ATTACKED_THIS_TURN,
		// Token: 0x04001A9A RID: 6810
		REQ_TARGET_IF_AVAILABLE,
		// Token: 0x04001A9B RID: 6811
		REQ_MINIMUM_ENEMY_MINIONS,
		// Token: 0x04001A9C RID: 6812
		REQ_TARGET_FOR_COMBO,
		// Token: 0x04001A9D RID: 6813
		REQ_NOT_EXHAUSTED_ACTIVATE,
		// Token: 0x04001A9E RID: 6814
		REQ_UNIQUE_SECRET,
		// Token: 0x04001A9F RID: 6815
		REQ_TARGET_TAUNTER,
		// Token: 0x04001AA0 RID: 6816
		REQ_CAN_BE_ATTACKED,
		// Token: 0x04001AA1 RID: 6817
		REQ_ACTION_PWR_IS_MASTER_PWR,
		// Token: 0x04001AA2 RID: 6818
		REQ_TARGET_MAGNET,
		// Token: 0x04001AA3 RID: 6819
		REQ_ATTACK_GREATER_THAN_0,
		// Token: 0x04001AA4 RID: 6820
		REQ_ATTACKER_NOT_FROZEN,
		// Token: 0x04001AA5 RID: 6821
		REQ_HERO_OR_MINION_TARGET,
		// Token: 0x04001AA6 RID: 6822
		REQ_CAN_BE_TARGETED_BY_SPELLS,
		// Token: 0x04001AA7 RID: 6823
		REQ_SUBCARD_IS_PLAYABLE,
		// Token: 0x04001AA8 RID: 6824
		REQ_TARGET_FOR_NO_COMBO,
		// Token: 0x04001AA9 RID: 6825
		REQ_NOT_MINION_JUST_PLAYED,
		// Token: 0x04001AAA RID: 6826
		REQ_NOT_EXHAUSTED_HERO_POWER,
		// Token: 0x04001AAB RID: 6827
		REQ_CAN_BE_TARGETED_BY_OPPONENTS,
		// Token: 0x04001AAC RID: 6828
		REQ_ATTACKER_CAN_ATTACK,
		// Token: 0x04001AAD RID: 6829
		REQ_TARGET_MIN_ATTACK,
		// Token: 0x04001AAE RID: 6830
		REQ_CAN_BE_TARGETED_BY_HERO_POWERS,
		// Token: 0x04001AAF RID: 6831
		REQ_ENEMY_TARGET_NOT_IMMUNE,
		// Token: 0x04001AB0 RID: 6832
		REQ_ENTIRE_ENTOURAGE_NOT_IN_PLAY,
		// Token: 0x04001AB1 RID: 6833
		REQ_MINIMUM_TOTAL_MINIONS,
		// Token: 0x04001AB2 RID: 6834
		REQ_MUST_TARGET_TAUNTER,
		// Token: 0x04001AB3 RID: 6835
		REQ_UNDAMAGED_TARGET,
		// Token: 0x04001AB4 RID: 6836
		REQ_CAN_BE_TARGETED_BY_BATTLECRIES,
		// Token: 0x04001AB5 RID: 6837
		REQ_STEADY_SHOT,
		// Token: 0x04001AB6 RID: 6838
		REQ_MINION_OR_ENEMY_HERO,
		// Token: 0x04001AB7 RID: 6839
		REQ_TARGET_IF_AVAILABLE_AND_DRAGON_IN_HAND,
		// Token: 0x04001AB8 RID: 6840
		REQ_LEGENDARY_TARGET,
		// Token: 0x04001AB9 RID: 6841
		REQ_FRIENDLY_MINION_DIED_THIS_TURN,
		// Token: 0x04001ABA RID: 6842
		REQ_FRIENDLY_MINION_DIED_THIS_GAME,
		// Token: 0x04001ABB RID: 6843
		REQ_ENEMY_WEAPON_EQUIPPED,
		// Token: 0x04001ABC RID: 6844
		REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS,
		// Token: 0x04001ABD RID: 6845
		REQ_TARGET_WITH_BATTLECRY,
		// Token: 0x04001ABE RID: 6846
		REQ_TARGET_WITH_DEATHRATTLE,
		// Token: 0x04001ABF RID: 6847
		REQ_DRAG_TO_PLAY
	}

	// Token: 0x0200061C RID: 1564
	public class GameStateInfo
	{
		// Token: 0x06004460 RID: 17504 RVA: 0x00148A27 File Offset: 0x00146C27
		public GameStateInfo()
		{
			this.currentStep = TAG_STEP.MAIN_BEGIN;
		}

		// Token: 0x04002B5B RID: 11099
		public TAG_STEP currentStep;
	}

	// Token: 0x02000626 RID: 1574
	public class PlayerInfo
	{
		// Token: 0x060044CB RID: 17611 RVA: 0x0014AB98 File Offset: 0x00148D98
		public PlayerInfo()
		{
			this.id = 0;
			this.numResources = 0;
			this.numFriendlyMinionsInPlay = 0;
			this.numEnemyMinionsInPlay = 0;
			this.numMinionSlotsPerPlayer = 0;
			this.numOpenSecretSlots = 0;
			this.numDragonsInHand = 0;
			this.numFriendlyMinionsThatDiedThisTurn = 0;
			this.numFriendlyMinionsThatDiedThisGame = 0;
			this.currentDefense = 0;
			this.isCurrentPlayer = false;
			this.weaponEquipped = false;
			this.enemyWeaponEquipped = false;
			this.comboActive = false;
			this.steadyShotRequiresTarget = false;
			this.spellsCostHealth = false;
		}

		// Token: 0x04002B9F RID: 11167
		public int id;

		// Token: 0x04002BA0 RID: 11168
		public int numResources;

		// Token: 0x04002BA1 RID: 11169
		public int numFriendlyMinionsInPlay;

		// Token: 0x04002BA2 RID: 11170
		public int numEnemyMinionsInPlay;

		// Token: 0x04002BA3 RID: 11171
		public int numMinionSlotsPerPlayer;

		// Token: 0x04002BA4 RID: 11172
		public int numOpenSecretSlots;

		// Token: 0x04002BA5 RID: 11173
		public int numDragonsInHand;

		// Token: 0x04002BA6 RID: 11174
		public int numFriendlyMinionsThatDiedThisTurn;

		// Token: 0x04002BA7 RID: 11175
		public int numFriendlyMinionsThatDiedThisGame;

		// Token: 0x04002BA8 RID: 11176
		public int currentDefense;

		// Token: 0x04002BA9 RID: 11177
		public bool isCurrentPlayer;

		// Token: 0x04002BAA RID: 11178
		public bool weaponEquipped;

		// Token: 0x04002BAB RID: 11179
		public bool enemyWeaponEquipped;

		// Token: 0x04002BAC RID: 11180
		public bool comboActive;

		// Token: 0x04002BAD RID: 11181
		public bool steadyShotRequiresTarget;

		// Token: 0x04002BAE RID: 11182
		public bool spellsCostHealth;
	}

	// Token: 0x020006A3 RID: 1699
	public class TargetEntityInfo
	{
		// Token: 0x06004771 RID: 18289 RVA: 0x00156E34 File Offset: 0x00155034
		public TargetEntityInfo()
		{
			this.id = 0;
			this.owningPlayerID = 0;
			this.damage = 0;
			this.attack = 0;
			this.race = 0;
			this.rarity = 0;
			this.cardType = TAG_CARDTYPE.MINION;
			this.isImmune = false;
			this.canBeAttacked = true;
			this.canBeTargetedByOpponents = true;
			this.canBeTargetedBySpells = true;
			this.canBeTargetedByHeroPowers = true;
			this.canBeTargetedByBattlecries = true;
			this.isFrozen = false;
			this.isEnchanted = false;
			this.isStealthed = false;
			this.isTaunter = false;
			this.isMagnet = false;
			this.hasCharge = false;
			this.hasAttackedThisTurn = false;
			this.hasBattlecry = false;
			this.hasDeathrattle = false;
		}

		// Token: 0x04002E6D RID: 11885
		public int id;

		// Token: 0x04002E6E RID: 11886
		public int owningPlayerID;

		// Token: 0x04002E6F RID: 11887
		public int damage;

		// Token: 0x04002E70 RID: 11888
		public int attack;

		// Token: 0x04002E71 RID: 11889
		public int race;

		// Token: 0x04002E72 RID: 11890
		public int rarity;

		// Token: 0x04002E73 RID: 11891
		public TAG_CARDTYPE cardType;

		// Token: 0x04002E74 RID: 11892
		public bool isImmune;

		// Token: 0x04002E75 RID: 11893
		public bool canBeAttacked;

		// Token: 0x04002E76 RID: 11894
		public bool canBeTargetedByOpponents;

		// Token: 0x04002E77 RID: 11895
		public bool canBeTargetedBySpells;

		// Token: 0x04002E78 RID: 11896
		public bool canBeTargetedByHeroPowers;

		// Token: 0x04002E79 RID: 11897
		public bool canBeTargetedByBattlecries;

		// Token: 0x04002E7A RID: 11898
		public bool isFrozen;

		// Token: 0x04002E7B RID: 11899
		public bool isEnchanted;

		// Token: 0x04002E7C RID: 11900
		public bool isStealthed;

		// Token: 0x04002E7D RID: 11901
		public bool isTaunter;

		// Token: 0x04002E7E RID: 11902
		public bool isMagnet;

		// Token: 0x04002E7F RID: 11903
		public bool hasCharge;

		// Token: 0x04002E80 RID: 11904
		public bool hasAttackedThisTurn;

		// Token: 0x04002E81 RID: 11905
		public bool hasBattlecry;

		// Token: 0x04002E82 RID: 11906
		public bool hasDeathrattle;
	}

	// Token: 0x020006A4 RID: 1700
	public class SourceEntityInfo
	{
		// Token: 0x06004772 RID: 18290 RVA: 0x00156EE4 File Offset: 0x001550E4
		public SourceEntityInfo()
		{
			this.requirementsMap = 0UL;
			this.id = 0;
			this.cost = 0;
			this.attack = 0;
			this.minAttackRequirement = 0;
			this.maxAttackRequirement = 0;
			this.raceRequirement = 0;
			this.numMinionSlotsRequirement = 0;
			this.numMinionSlotsWithTargetRequirement = 0;
			this.minTotalMinionsRequirement = 0;
			this.minFriendlyMinionsRequirement = 0;
			this.minEnemyMinionsRequirement = 0;
			this.numTurnsInPlay = 0;
			this.numAttacksThisTurn = 0;
			this.numAttacksAllowedThisTurn = 1;
			this.cardType = TAG_CARDTYPE.MINION;
			this.zone = TAG_ZONE.SETASIDE;
			this.isSecret = false;
			this.isDuplicateSecret = false;
			this.isExhausted = false;
			this.isMasterPower = false;
			this.isActionPower = false;
			this.isActivatePower = false;
			this.isAttackPower = false;
			this.isFrozen = false;
			this.hasBattlecry = false;
			this.canAttack = true;
			this.entireEntourageInPlay = false;
			this.hasCharge = false;
			this.isChoiceMinion = false;
			this.cannotAttackHeroes = false;
		}

		// Token: 0x04002E83 RID: 11907
		public ulong requirementsMap;

		// Token: 0x04002E84 RID: 11908
		public int id;

		// Token: 0x04002E85 RID: 11909
		public int cost;

		// Token: 0x04002E86 RID: 11910
		public int attack;

		// Token: 0x04002E87 RID: 11911
		public int minAttackRequirement;

		// Token: 0x04002E88 RID: 11912
		public int maxAttackRequirement;

		// Token: 0x04002E89 RID: 11913
		public int raceRequirement;

		// Token: 0x04002E8A RID: 11914
		public int numMinionSlotsRequirement;

		// Token: 0x04002E8B RID: 11915
		public int numMinionSlotsWithTargetRequirement;

		// Token: 0x04002E8C RID: 11916
		public int minTotalMinionsRequirement;

		// Token: 0x04002E8D RID: 11917
		public int minFriendlyMinionsRequirement;

		// Token: 0x04002E8E RID: 11918
		public int minEnemyMinionsRequirement;

		// Token: 0x04002E8F RID: 11919
		public int numTurnsInPlay;

		// Token: 0x04002E90 RID: 11920
		public int numAttacksThisTurn;

		// Token: 0x04002E91 RID: 11921
		public int numAttacksAllowedThisTurn;

		// Token: 0x04002E92 RID: 11922
		public TAG_CARDTYPE cardType;

		// Token: 0x04002E93 RID: 11923
		public TAG_ZONE zone;

		// Token: 0x04002E94 RID: 11924
		public bool isSecret;

		// Token: 0x04002E95 RID: 11925
		public bool isDuplicateSecret;

		// Token: 0x04002E96 RID: 11926
		public bool isExhausted;

		// Token: 0x04002E97 RID: 11927
		public bool isMasterPower;

		// Token: 0x04002E98 RID: 11928
		public bool isActionPower;

		// Token: 0x04002E99 RID: 11929
		public bool isActivatePower;

		// Token: 0x04002E9A RID: 11930
		public bool isAttackPower;

		// Token: 0x04002E9B RID: 11931
		public bool isFrozen;

		// Token: 0x04002E9C RID: 11932
		public bool hasBattlecry;

		// Token: 0x04002E9D RID: 11933
		public bool canAttack;

		// Token: 0x04002E9E RID: 11934
		public bool entireEntourageInPlay;

		// Token: 0x04002E9F RID: 11935
		public bool hasCharge;

		// Token: 0x04002EA0 RID: 11936
		public bool isChoiceMinion;

		// Token: 0x04002EA1 RID: 11937
		public bool cannotAttackHeroes;
	}

	// Token: 0x020006A5 RID: 1701
	public class PlayRequirementInfo
	{
		// Token: 0x06004773 RID: 18291 RVA: 0x00156FD4 File Offset: 0x001551D4
		public PlayRequirementInfo()
		{
			this.requirementsMap = 0UL;
			this.paramMinAtk = 0;
			this.paramMaxAtk = 0;
			this.paramRace = 0;
			this.paramNumMinionSlots = 0;
			this.paramNumMinionSlotsWithTarget = 0;
			this.paramMinNumTotalMinions = 0;
			this.paramMinNumFriendlyMinions = 0;
			this.paramMinNumEnemyMinions = 0;
		}

		// Token: 0x04002EA2 RID: 11938
		public ulong requirementsMap;

		// Token: 0x04002EA3 RID: 11939
		public int paramMinAtk;

		// Token: 0x04002EA4 RID: 11940
		public int paramMaxAtk;

		// Token: 0x04002EA5 RID: 11941
		public int paramRace;

		// Token: 0x04002EA6 RID: 11942
		public int paramNumMinionSlots;

		// Token: 0x04002EA7 RID: 11943
		public int paramNumMinionSlotsWithTarget;

		// Token: 0x04002EA8 RID: 11944
		public int paramMinNumTotalMinions;

		// Token: 0x04002EA9 RID: 11945
		public int paramMinNumFriendlyMinions;

		// Token: 0x04002EAA RID: 11946
		public int paramMinNumEnemyMinions;
	}

	// Token: 0x020008DA RID: 2266
	[StructLayout(0, Pack = 1)]
	private struct Marshaled_TargetEntityInfo
	{
		// Token: 0x0600556B RID: 21867 RVA: 0x00198490 File Offset: 0x00196690
		public static PlayErrors.Marshaled_TargetEntityInfo ConvertFromTargetEntityInfo(PlayErrors.TargetEntityInfo targetInfo)
		{
			return new PlayErrors.Marshaled_TargetEntityInfo
			{
				id = targetInfo.id,
				owningPlayerID = targetInfo.owningPlayerID,
				damage = targetInfo.damage,
				attack = targetInfo.attack,
				cardType = targetInfo.cardType,
				race = targetInfo.race,
				rarity = targetInfo.rarity,
				isImmune = targetInfo.isImmune,
				canBeAttacked = targetInfo.canBeAttacked,
				canBeTargetedByOpponents = targetInfo.canBeTargetedByOpponents,
				canBeTargetedBySpells = targetInfo.canBeTargetedBySpells,
				canBeTargetedByHeroPowers = targetInfo.canBeTargetedByHeroPowers,
				canBeTargetedByBattlecries = targetInfo.canBeTargetedByBattlecries,
				isFrozen = targetInfo.isFrozen,
				isEnchanted = targetInfo.isEnchanted,
				isStealthed = targetInfo.isStealthed,
				isTaunter = targetInfo.isTaunter,
				isMagnet = targetInfo.isMagnet,
				hasCharge = targetInfo.hasCharge,
				hasAttackedThisTurn = targetInfo.hasAttackedThisTurn,
				hasBattlecry = targetInfo.hasBattlecry,
				hasDeathrattle = targetInfo.hasDeathrattle
			};
		}

		// Token: 0x04003B5C RID: 15196
		public int id;

		// Token: 0x04003B5D RID: 15197
		public int owningPlayerID;

		// Token: 0x04003B5E RID: 15198
		public int damage;

		// Token: 0x04003B5F RID: 15199
		public int attack;

		// Token: 0x04003B60 RID: 15200
		public int race;

		// Token: 0x04003B61 RID: 15201
		public int rarity;

		// Token: 0x04003B62 RID: 15202
		[MarshalAs(8)]
		public TAG_CARDTYPE cardType;

		// Token: 0x04003B63 RID: 15203
		[MarshalAs(4)]
		public bool isImmune;

		// Token: 0x04003B64 RID: 15204
		[MarshalAs(4)]
		public bool canBeAttacked;

		// Token: 0x04003B65 RID: 15205
		[MarshalAs(4)]
		public bool canBeTargetedByOpponents;

		// Token: 0x04003B66 RID: 15206
		[MarshalAs(4)]
		public bool canBeTargetedBySpells;

		// Token: 0x04003B67 RID: 15207
		[MarshalAs(4)]
		public bool canBeTargetedByHeroPowers;

		// Token: 0x04003B68 RID: 15208
		[MarshalAs(4)]
		public bool canBeTargetedByBattlecries;

		// Token: 0x04003B69 RID: 15209
		[MarshalAs(4)]
		public bool isFrozen;

		// Token: 0x04003B6A RID: 15210
		[MarshalAs(4)]
		public bool isEnchanted;

		// Token: 0x04003B6B RID: 15211
		[MarshalAs(4)]
		public bool isStealthed;

		// Token: 0x04003B6C RID: 15212
		[MarshalAs(4)]
		public bool isTaunter;

		// Token: 0x04003B6D RID: 15213
		[MarshalAs(4)]
		public bool isMagnet;

		// Token: 0x04003B6E RID: 15214
		[MarshalAs(4)]
		public bool hasCharge;

		// Token: 0x04003B6F RID: 15215
		[MarshalAs(4)]
		public bool hasAttackedThisTurn;

		// Token: 0x04003B70 RID: 15216
		[MarshalAs(4)]
		public bool hasBattlecry;

		// Token: 0x04003B71 RID: 15217
		[MarshalAs(4)]
		public bool hasDeathrattle;
	}

	// Token: 0x020008DB RID: 2267
	[StructLayout(0, Pack = 1)]
	private struct Marshaled_SourceEntityInfo
	{
		// Token: 0x0600556C RID: 21868 RVA: 0x001985C4 File Offset: 0x001967C4
		public static PlayErrors.Marshaled_SourceEntityInfo ConvertFromSourceEntityInfo(PlayErrors.SourceEntityInfo sourceInfo)
		{
			return new PlayErrors.Marshaled_SourceEntityInfo
			{
				requirementsMap = sourceInfo.requirementsMap,
				id = sourceInfo.id,
				cost = sourceInfo.cost,
				attack = sourceInfo.attack,
				minAttackRequirement = sourceInfo.minAttackRequirement,
				maxAttackRequirement = sourceInfo.maxAttackRequirement,
				raceRequirement = sourceInfo.raceRequirement,
				numMinionSlotsRequirement = sourceInfo.numMinionSlotsRequirement,
				numMinionSlotsWithTargetRequirement = sourceInfo.numMinionSlotsWithTargetRequirement,
				minTotalMinionsRequirement = sourceInfo.minTotalMinionsRequirement,
				minFriendlyMinionsRequirement = sourceInfo.minFriendlyMinionsRequirement,
				minEnemyMinionsRequirement = sourceInfo.minEnemyMinionsRequirement,
				numTurnsInPlay = sourceInfo.numTurnsInPlay,
				numAttacksThisTurn = sourceInfo.numAttacksThisTurn,
				numAttacksAllowedThisTurn = sourceInfo.numAttacksAllowedThisTurn,
				cardType = sourceInfo.cardType,
				zone = sourceInfo.zone,
				isSecret = sourceInfo.isSecret,
				isDuplicateSecret = sourceInfo.isDuplicateSecret,
				isExhausted = sourceInfo.isExhausted,
				isMasterPower = sourceInfo.isMasterPower,
				isActionPower = sourceInfo.isActionPower,
				isActivatePower = sourceInfo.isActivatePower,
				isAttackPower = sourceInfo.isAttackPower,
				isFrozen = sourceInfo.isFrozen,
				hasBattlecry = sourceInfo.hasBattlecry,
				canAttack = sourceInfo.canAttack,
				entireEntourageInPlay = sourceInfo.entireEntourageInPlay,
				hasCharge = sourceInfo.hasCharge,
				isChoiceMinion = sourceInfo.isChoiceMinion,
				cannotAttackHeroes = sourceInfo.cannotAttackHeroes
			};
		}

		// Token: 0x04003B72 RID: 15218
		public ulong requirementsMap;

		// Token: 0x04003B73 RID: 15219
		public int id;

		// Token: 0x04003B74 RID: 15220
		public int cost;

		// Token: 0x04003B75 RID: 15221
		public int attack;

		// Token: 0x04003B76 RID: 15222
		public int minAttackRequirement;

		// Token: 0x04003B77 RID: 15223
		public int maxAttackRequirement;

		// Token: 0x04003B78 RID: 15224
		public int raceRequirement;

		// Token: 0x04003B79 RID: 15225
		public int numMinionSlotsRequirement;

		// Token: 0x04003B7A RID: 15226
		public int numMinionSlotsWithTargetRequirement;

		// Token: 0x04003B7B RID: 15227
		public int minTotalMinionsRequirement;

		// Token: 0x04003B7C RID: 15228
		public int minFriendlyMinionsRequirement;

		// Token: 0x04003B7D RID: 15229
		public int minEnemyMinionsRequirement;

		// Token: 0x04003B7E RID: 15230
		public int numTurnsInPlay;

		// Token: 0x04003B7F RID: 15231
		public int numAttacksThisTurn;

		// Token: 0x04003B80 RID: 15232
		public int numAttacksAllowedThisTurn;

		// Token: 0x04003B81 RID: 15233
		[MarshalAs(8)]
		public TAG_CARDTYPE cardType;

		// Token: 0x04003B82 RID: 15234
		[MarshalAs(8)]
		public TAG_ZONE zone;

		// Token: 0x04003B83 RID: 15235
		[MarshalAs(4)]
		public bool isSecret;

		// Token: 0x04003B84 RID: 15236
		[MarshalAs(4)]
		public bool isDuplicateSecret;

		// Token: 0x04003B85 RID: 15237
		[MarshalAs(4)]
		public bool isExhausted;

		// Token: 0x04003B86 RID: 15238
		[MarshalAs(4)]
		public bool isMasterPower;

		// Token: 0x04003B87 RID: 15239
		[MarshalAs(4)]
		public bool isActionPower;

		// Token: 0x04003B88 RID: 15240
		[MarshalAs(4)]
		public bool isActivatePower;

		// Token: 0x04003B89 RID: 15241
		[MarshalAs(4)]
		public bool isAttackPower;

		// Token: 0x04003B8A RID: 15242
		[MarshalAs(4)]
		public bool isFrozen;

		// Token: 0x04003B8B RID: 15243
		[MarshalAs(4)]
		public bool hasBattlecry;

		// Token: 0x04003B8C RID: 15244
		[MarshalAs(4)]
		public bool canAttack;

		// Token: 0x04003B8D RID: 15245
		[MarshalAs(4)]
		public bool entireEntourageInPlay;

		// Token: 0x04003B8E RID: 15246
		[MarshalAs(4)]
		public bool hasCharge;

		// Token: 0x04003B8F RID: 15247
		[MarshalAs(4)]
		public bool isChoiceMinion;

		// Token: 0x04003B90 RID: 15248
		[MarshalAs(4)]
		public bool cannotAttackHeroes;
	}

	// Token: 0x020008DC RID: 2268
	[StructLayout(0, Pack = 1)]
	private struct Marshaled_PlayerInfo
	{
		// Token: 0x0600556D RID: 21869 RVA: 0x00198770 File Offset: 0x00196970
		public static PlayErrors.Marshaled_PlayerInfo ConvertFromPlayerInfo(PlayErrors.PlayerInfo playerInfo)
		{
			return new PlayErrors.Marshaled_PlayerInfo
			{
				id = playerInfo.id,
				numResources = playerInfo.numResources,
				numFriendlyMinionsInPlay = playerInfo.numFriendlyMinionsInPlay,
				numEnemyMinionsInPlay = playerInfo.numEnemyMinionsInPlay,
				numMinionSlotsPerPlayer = playerInfo.numMinionSlotsPerPlayer,
				numOpenSecretSlots = playerInfo.numOpenSecretSlots,
				numDragonsInHand = playerInfo.numDragonsInHand,
				numFriendlyMinionsThatDiedThisTurn = playerInfo.numFriendlyMinionsThatDiedThisTurn,
				numFriendlyMinionsThatDiedThisGame = playerInfo.numFriendlyMinionsThatDiedThisGame,
				currentDefense = playerInfo.currentDefense,
				isCurrentPlayer = playerInfo.isCurrentPlayer,
				weaponEquipped = playerInfo.weaponEquipped,
				enemyWeaponEquipped = playerInfo.enemyWeaponEquipped,
				comboActive = playerInfo.comboActive,
				steadyShotRequiresTarget = playerInfo.steadyShotRequiresTarget,
				spellsCostHealth = playerInfo.spellsCostHealth
			};
		}

		// Token: 0x04003B91 RID: 15249
		public int id;

		// Token: 0x04003B92 RID: 15250
		public int numResources;

		// Token: 0x04003B93 RID: 15251
		public int numFriendlyMinionsInPlay;

		// Token: 0x04003B94 RID: 15252
		public int numEnemyMinionsInPlay;

		// Token: 0x04003B95 RID: 15253
		public int numMinionSlotsPerPlayer;

		// Token: 0x04003B96 RID: 15254
		public int numOpenSecretSlots;

		// Token: 0x04003B97 RID: 15255
		public int numDragonsInHand;

		// Token: 0x04003B98 RID: 15256
		public int numFriendlyMinionsThatDiedThisTurn;

		// Token: 0x04003B99 RID: 15257
		public int numFriendlyMinionsThatDiedThisGame;

		// Token: 0x04003B9A RID: 15258
		public int currentDefense;

		// Token: 0x04003B9B RID: 15259
		[MarshalAs(4)]
		public bool isCurrentPlayer;

		// Token: 0x04003B9C RID: 15260
		[MarshalAs(4)]
		public bool weaponEquipped;

		// Token: 0x04003B9D RID: 15261
		[MarshalAs(4)]
		public bool enemyWeaponEquipped;

		// Token: 0x04003B9E RID: 15262
		[MarshalAs(4)]
		public bool comboActive;

		// Token: 0x04003B9F RID: 15263
		[MarshalAs(4)]
		public bool steadyShotRequiresTarget;

		// Token: 0x04003BA0 RID: 15264
		[MarshalAs(4)]
		public bool spellsCostHealth;
	}

	// Token: 0x020008DD RID: 2269
	[StructLayout(0, Pack = 1)]
	private struct Marshaled_GameStateInfo
	{
		// Token: 0x0600556E RID: 21870 RVA: 0x00198858 File Offset: 0x00196A58
		public static PlayErrors.Marshaled_GameStateInfo ConvertFromGameStateInfo(PlayErrors.GameStateInfo gameInfo)
		{
			return new PlayErrors.Marshaled_GameStateInfo
			{
				currentStep = gameInfo.currentStep
			};
		}

		// Token: 0x04003BA1 RID: 15265
		[MarshalAs(8)]
		public TAG_STEP currentStep;
	}

	// Token: 0x020008DE RID: 2270
	[StructLayout(0, Pack = 1)]
	private struct Marshaled_PlayErrorsParams
	{
		// Token: 0x0600556F RID: 21871 RVA: 0x0019887C File Offset: 0x00196A7C
		public Marshaled_PlayErrorsParams(PlayErrors.SourceEntityInfo sourceInfo, PlayErrors.PlayerInfo playerInfo, PlayErrors.GameStateInfo gameInfo)
		{
			this.source = PlayErrors.Marshaled_SourceEntityInfo.ConvertFromSourceEntityInfo(sourceInfo);
			this.player = PlayErrors.Marshaled_PlayerInfo.ConvertFromPlayerInfo(playerInfo);
			this.game = PlayErrors.Marshaled_GameStateInfo.ConvertFromGameStateInfo(gameInfo);
		}

		// Token: 0x04003BA2 RID: 15266
		[MarshalAs(27)]
		public PlayErrors.Marshaled_SourceEntityInfo source;

		// Token: 0x04003BA3 RID: 15267
		[MarshalAs(27)]
		public PlayErrors.Marshaled_PlayerInfo player;

		// Token: 0x04003BA4 RID: 15268
		[MarshalAs(8)]
		public PlayErrors.Marshaled_GameStateInfo game;
	}

	// Token: 0x020008DF RID: 2271
	// (Invoke) Token: 0x06005571 RID: 21873
	[UnmanagedFunctionPointer(2)]
	private delegate bool DelPlayErrorsInit();

	// Token: 0x020008E0 RID: 2272
	// (Invoke) Token: 0x06005575 RID: 21877
	[UnmanagedFunctionPointer(2)]
	private delegate ulong DelGetRequirementsMap(PlayErrors.ErrorType[] requirements, int numRequirements);

	// Token: 0x020008E1 RID: 2273
	// (Invoke) Token: 0x06005579 RID: 21881
	[UnmanagedFunctionPointer(2)]
	private delegate PlayErrors.ErrorType DelGetPlayEntityError(PlayErrors.Marshaled_PlayErrorsParams playErrorsParams, PlayErrors.Marshaled_SourceEntityInfo[] subCards, int numSubCards, PlayErrors.Marshaled_TargetEntityInfo[] enititiesInPlay, int numEntitiesInPlay);

	// Token: 0x020008E2 RID: 2274
	// (Invoke) Token: 0x0600557D RID: 21885
	[UnmanagedFunctionPointer(2)]
	private delegate PlayErrors.ErrorType DelGetTargetEntityError(PlayErrors.Marshaled_PlayErrorsParams playErrorsParams, PlayErrors.Marshaled_TargetEntityInfo target, PlayErrors.Marshaled_TargetEntityInfo[] entitiesInPlay, int numEntitiesInPlay);
}
