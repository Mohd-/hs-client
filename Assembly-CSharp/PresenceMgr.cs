using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using bgs;
using bgs.types;

// Token: 0x02000226 RID: 550
public class PresenceMgr
{
	// Token: 0x06002112 RID: 8466 RVA: 0x000A2536 File Offset: 0x000A0736
	public static PresenceMgr Get()
	{
		if (PresenceMgr.s_instance == null)
		{
			PresenceMgr.s_instance = new PresenceMgr();
			PresenceMgr.s_instance.Initialize();
		}
		return PresenceMgr.s_instance;
	}

	// Token: 0x06002113 RID: 8467 RVA: 0x000A255B File Offset: 0x000A075B
	public bool SetStatus(params Enum[] args)
	{
		return this.SetStatusImpl(args);
	}

	// Token: 0x06002114 RID: 8468 RVA: 0x000A2564 File Offset: 0x000A0764
	public bool SetStatus_EnteringAdventure(AdventureDbId adventureId, AdventureModeDbId adventureModeId)
	{
		KeyValuePair<AdventureDbId, AdventureModeDbId> key = new KeyValuePair<AdventureDbId, AdventureModeDbId>(adventureId, adventureModeId);
		PresenceMgr.PresenceTargets presenceTargets;
		if (PresenceMgr.s_adventurePresenceMap.TryGetValue(key, out presenceTargets))
		{
			this.SetStatus(new Enum[]
			{
				PresenceStatus.ADVENTURE_SCENARIO_SELECT,
				presenceTargets.EnteringAdventureValue
			});
			return true;
		}
		return false;
	}

	// Token: 0x06002115 RID: 8469 RVA: 0x000A25B5 File Offset: 0x000A07B5
	public bool SetStatus_PlayingMission(ScenarioDbId missionId)
	{
		return PresenceMgr.s_stringKeyMap.ContainsKey(missionId) && this.SetStatus(new Enum[]
		{
			PresenceStatus.ADVENTURE_SCENARIO_PLAYING_GAME,
			missionId
		});
	}

	// Token: 0x06002116 RID: 8470 RVA: 0x000A25F0 File Offset: 0x000A07F0
	public bool SetStatus_SpectatingMission(ScenarioDbId missionId)
	{
		AdventureDbId adventureId = GameUtils.GetAdventureId((int)missionId);
		AdventureModeDbId adventureModeId = GameUtils.GetAdventureModeId((int)missionId);
		KeyValuePair<AdventureDbId, AdventureModeDbId> key = new KeyValuePair<AdventureDbId, AdventureModeDbId>(adventureId, adventureModeId);
		PresenceMgr.PresenceTargets presenceTargets;
		return PresenceMgr.s_adventurePresenceMap.TryGetValue(key, out presenceTargets) && this.SetStatus(new Enum[]
		{
			presenceTargets.SpectatingValue
		});
	}

	// Token: 0x06002117 RID: 8471 RVA: 0x000A2643 File Offset: 0x000A0843
	public Enum[] GetStatus()
	{
		return this.m_status;
	}

	// Token: 0x06002118 RID: 8472 RVA: 0x000A264B File Offset: 0x000A084B
	public Enum[] GetPrevStatus()
	{
		return this.m_prevStatus;
	}

	// Token: 0x06002119 RID: 8473 RVA: 0x000A2653 File Offset: 0x000A0853
	public bool SetPrevStatus()
	{
		return this.SetStatusImpl(this.m_prevStatus);
	}

	// Token: 0x0600211A RID: 8474 RVA: 0x000A2664 File Offset: 0x000A0864
	public bool IsRichPresence(params Enum[] status)
	{
		if (status == null)
		{
			return false;
		}
		if (status.Length == 0)
		{
			return false;
		}
		foreach (Enum @enum in status)
		{
			if (@enum == null)
			{
				return false;
			}
			if (PresenceMgr.s_richPresenceMap.ContainsKey(@enum))
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x0600211B RID: 8475 RVA: 0x000A26B8 File Offset: 0x000A08B8
	public string GetStatusText(BnetPlayer player)
	{
		List<string> list = new List<string>();
		string key = null;
		PresenceStatus status_Internal = this.GetStatus_Internal(player, ref key, list, null);
		if (status_Internal == PresenceStatus.UNKNOWN)
		{
			BnetGameAccount bestGameAccount = player.GetBestGameAccount();
			return (!(bestGameAccount == null)) ? bestGameAccount.GetRichPresence() : null;
		}
		string[] args = list.ToArray();
		return GameStrings.Format(key, args);
	}

	// Token: 0x0600211C RID: 8476 RVA: 0x000A2710 File Offset: 0x000A0910
	public PresenceStatus GetStatus(BnetPlayer player)
	{
		string text = null;
		return this.GetStatus_Internal(player, ref text, null, null);
	}

	// Token: 0x0600211D RID: 8477 RVA: 0x000A272C File Offset: 0x000A092C
	public Enum[] GetStatusEnums(BnetPlayer player)
	{
		string text = null;
		List<Enum> list = new List<Enum>();
		this.GetStatus_Internal(player, ref text, null, list);
		return list.ToArray();
	}

	// Token: 0x0600211E RID: 8478 RVA: 0x000A2754 File Offset: 0x000A0954
	private PresenceStatus GetStatus_Internal(BnetPlayer player, ref string statusKey, List<string> stringArgs = null, List<Enum> enumVals = null)
	{
		PresenceStatus presenceStatus = PresenceStatus.UNKNOWN;
		if (player == null)
		{
			return presenceStatus;
		}
		BnetGameAccount bestGameAccount = player.GetBestGameAccount();
		if (bestGameAccount == null)
		{
			return presenceStatus;
		}
		BnetGameAccount hearthstoneGameAccount = player.GetHearthstoneGameAccount();
		if (hearthstoneGameAccount == null)
		{
			return presenceStatus;
		}
		byte[] array;
		if (!hearthstoneGameAccount.TryGetGameFieldBytes(17U, out array))
		{
			return presenceStatus;
		}
		Enum @enum = null;
		using (MemoryStream memoryStream = new MemoryStream(array))
		{
			using (BinaryReader binaryReader = new BinaryReader(memoryStream))
			{
				if (!this.DecodeStatusVal(binaryReader, ref @enum, ref statusKey))
				{
					return presenceStatus;
				}
				presenceStatus = (PresenceStatus)((int)@enum);
				if (enumVals != null)
				{
					enumVals.Add(presenceStatus);
				}
				if (stringArgs != null || enumVals != null)
				{
					while (memoryStream.Position < (long)array.Length)
					{
						string key = null;
						if (!this.DecodeStatusVal(binaryReader, ref @enum, ref key))
						{
							return presenceStatus;
						}
						if (enumVals != null)
						{
							enumVals.Add(@enum);
						}
						if (stringArgs != null)
						{
							string text = GameStrings.Get(key);
							stringArgs.Add(text);
						}
					}
				}
			}
		}
		return presenceStatus;
	}

	// Token: 0x0600211F RID: 8479 RVA: 0x000A289C File Offset: 0x000A0A9C
	public static Map<Enum, string> GetEnumStringMap()
	{
		return PresenceMgr.s_stringKeyMap;
	}

	// Token: 0x06002120 RID: 8480 RVA: 0x000A28A4 File Offset: 0x000A0AA4
	private void Initialize()
	{
		for (int i = 0; i < PresenceMgr.s_enumIdList.Length; i++)
		{
			Type type = PresenceMgr.s_enumIdList[i];
			if (Enum.GetUnderlyingType(type) != typeof(int))
			{
				throw new Exception(string.Format("Underlying type of enum {0} (underlying={1}) must {2} be to used by Presence system.", type.FullName, Enum.GetUnderlyingType(type).FullName, typeof(int).Name));
			}
			byte b = (byte)(i + 1);
			this.m_enumToIdMap.Add(type, b);
			this.m_idToEnumMap.Add(b, type);
		}
	}

	// Token: 0x06002121 RID: 8481 RVA: 0x000A2938 File Offset: 0x000A0B38
	private bool SetStatusImpl(Enum[] status)
	{
		if (!Network.ShouldBeConnectedToAurora())
		{
			return false;
		}
		if (!Network.IsLoggedIn())
		{
			return true;
		}
		if (status == null || status.Length == 0)
		{
			Error.AddDevFatal("PresenceMgr.SetStatusImpl() - Received status of length 0. Setting empty status is not supported.", new object[0]);
			return false;
		}
		if (GeneralUtils.AreArraysEqual<Enum>(this.m_status, status))
		{
			return true;
		}
		if (!this.SetRichPresence(status))
		{
			return false;
		}
		if (!this.SetGamePresence(status))
		{
			return false;
		}
		this.m_prevStatus = this.m_status;
		bool flag = this.m_prevStatus == null || this.m_prevStatus.Length == 0;
		this.m_status = new Enum[status.Length];
		Array.Copy(status, this.m_status, status.Length);
		if (flag || !PresenceMgr.IsStatusPlayingGame((PresenceStatus)status[0]))
		{
			SpectatorManager.Get().UpdateMySpectatorInfo();
		}
		return true;
	}

	// Token: 0x06002122 RID: 8482 RVA: 0x000A2A14 File Offset: 0x000A0C14
	private bool SetRichPresence(Enum[] status)
	{
		Enum[] array = new Enum[status.Length];
		for (int i = 0; i < status.Length; i++)
		{
			Enum @enum = status[i];
			Enum enum2;
			if (PresenceMgr.s_richPresenceMap.TryGetValue(@enum, out enum2))
			{
				if (enum2 == null)
				{
					return false;
				}
			}
			else
			{
				enum2 = @enum;
			}
			array[i] = enum2;
		}
		if (Enumerable.Any<Enum>(array, (Enum e) => !RichPresence.s_streamIds.ContainsKey(e.GetType())))
		{
			Enum[] array2 = new Enum[]
			{
				array[0]
			};
			array = array2;
		}
		if (GeneralUtils.AreArraysEqual<Enum>(this.m_richPresence, array))
		{
			return true;
		}
		this.m_richPresence = array;
		if (!Network.ShouldBeConnectedToAurora())
		{
			string text = "Caller should check for Battle.net connection before calling SetRichPresence {0}";
			object obj;
			if (array == null)
			{
				obj = string.Empty;
			}
			else
			{
				obj = string.Join(", ", Enumerable.ToArray<string>(Enumerable.Select<Enum, string>(array, (Enum x) => x.ToString())));
			}
			Error.AddDevFatal(string.Format(text, obj), new object[0]);
			return false;
		}
		if (array == null)
		{
			return false;
		}
		if (array.Length == 0)
		{
			return false;
		}
		RichPresenceUpdate[] array3 = new RichPresenceUpdate[array.Length];
		for (int j = 0; j < array.Length; j++)
		{
			Enum enum3 = array[j];
			Type type = enum3.GetType();
			FourCC fourCC = RichPresence.s_streamIds[type];
			RichPresenceUpdate richPresenceUpdate = default(RichPresenceUpdate);
			richPresenceUpdate.presenceFieldIndex = (ulong)((j != 0) ? (458752 + j) : 0);
			richPresenceUpdate.programId = BnetProgramId.HEARTHSTONE.GetValue();
			richPresenceUpdate.streamId = fourCC.GetValue();
			richPresenceUpdate.index = Convert.ToUInt32(enum3);
			array3[j] = richPresenceUpdate;
		}
		BattleNet.SetRichPresence(array3);
		return true;
	}

	// Token: 0x06002123 RID: 8483 RVA: 0x000A2BD8 File Offset: 0x000A0DD8
	private bool SetGamePresence(Enum[] status)
	{
		bool result;
		using (MemoryStream memoryStream = new MemoryStream())
		{
			using (BinaryWriter binaryWriter = new BinaryWriter(memoryStream))
			{
				for (int i = 0; i < status.Length; i++)
				{
					byte b;
					int num;
					if (!this.EncodeStatusVal(status, i, out b, out num))
					{
						return false;
					}
					binaryWriter.Write(b);
					binaryWriter.Write(num);
				}
				byte[] buffer = memoryStream.GetBuffer();
				byte[] array = new byte[memoryStream.Position];
				Array.Copy(buffer, array, array.Length);
				result = BnetPresenceMgr.Get().SetGameField(17U, array);
			}
		}
		return result;
	}

	// Token: 0x06002124 RID: 8484 RVA: 0x000A2CA8 File Offset: 0x000A0EA8
	private bool EncodeStatusVal(Enum[] status, int index, out byte id, out int intVal)
	{
		Enum @enum = status[index];
		Type type = @enum.GetType();
		intVal = Convert.ToInt32(@enum);
		if (!this.m_enumToIdMap.TryGetValue(type, out id))
		{
			Error.AddDevFatal("PresenceMgr.EncodeStatusVal() - {0} at index {1} belongs to type {2}, which has no id", new object[]
			{
				@enum,
				index,
				type
			});
			return false;
		}
		return true;
	}

	// Token: 0x06002125 RID: 8485 RVA: 0x000A2D00 File Offset: 0x000A0F00
	private bool DecodeStatusVal(BinaryReader reader, ref Enum enumVal, ref string key)
	{
		key = null;
		byte b = 0;
		int num = 0;
		int num2 = (int)reader.BaseStream.Position;
		int num3 = num2 + 1;
		try
		{
			b = reader.ReadByte();
			num3 = (int)reader.BaseStream.Position;
		}
		catch (Exception ex)
		{
			Log.Henry.Print("PresenceMgr.DecodeStatusVal - unable to decode enum id {0} at index {1} : {2} {3}", new object[]
			{
				b,
				num2,
				ex.GetType().FullName,
				ex.Message
			});
			return false;
		}
		Type type;
		if (!this.m_idToEnumMap.TryGetValue(b, out type))
		{
			Log.Henry.Print("PresenceMgr.DecodeStatusVal - id {0} at index {1}, has no enum type", new object[]
			{
				b,
				num2
			});
			return false;
		}
		try
		{
			num = reader.ReadInt32();
		}
		catch (Exception ex2)
		{
			Log.Henry.Print("PresenceMgr.DecodeStatusVal - unable to decode enum value {0} at index {1} : {2} {3}", new object[]
			{
				b,
				num3,
				ex2.GetType().FullName,
				ex2.Message
			});
			return false;
		}
		if (type == typeof(PresenceStatus))
		{
			PresenceStatus presenceStatus = (PresenceStatus)num;
			enumVal = presenceStatus;
			if (!PresenceMgr.s_stringKeyMap.TryGetValue(presenceStatus, out key))
			{
				Log.Henry.Print("PresenceMgr.DecodeStatusVal - value {0}.{1} at index {2}, has no string", new object[]
				{
					type,
					presenceStatus,
					num3
				});
				return false;
			}
		}
		else if (type == typeof(PresenceTutorial))
		{
			PresenceTutorial presenceTutorial = (PresenceTutorial)num;
			enumVal = presenceTutorial;
			if (!PresenceMgr.s_stringKeyMap.TryGetValue(presenceTutorial, out key))
			{
				Log.Henry.Print("PresenceMgr.DecodeStatusVal - value {0}.{1} at index {2}, has no string", new object[]
				{
					type,
					presenceTutorial,
					num3
				});
				return false;
			}
		}
		else if (type == typeof(PresenceAdventureMode))
		{
			PresenceAdventureMode presenceAdventureMode = (PresenceAdventureMode)num;
			enumVal = presenceAdventureMode;
			if (!PresenceMgr.s_stringKeyMap.TryGetValue(presenceAdventureMode, out key))
			{
				Log.Henry.Print("PresenceMgr.DecodeStatusVal - value {0}.{1} at index {2}, has no string", new object[]
				{
					type,
					presenceAdventureMode,
					num3
				});
				return false;
			}
		}
		else if (type == typeof(ScenarioDbId))
		{
			ScenarioDbId scenarioDbId = (ScenarioDbId)num;
			enumVal = scenarioDbId;
			if (!PresenceMgr.s_stringKeyMap.TryGetValue(scenarioDbId, out key))
			{
				Log.Henry.Print("PresenceMgr.DecodeStatusVal - value {0}.{1} at index {2}, has no string", new object[]
				{
					type,
					scenarioDbId,
					num3
				});
				return false;
			}
		}
		return true;
	}

	// Token: 0x06002126 RID: 8486 RVA: 0x000A2FE0 File Offset: 0x000A11E0
	public static bool IsStatusPlayingGame(PresenceStatus status)
	{
		switch (status)
		{
		case PresenceStatus.ARENA_GAME:
		case PresenceStatus.FRIENDLY_GAME:
			break;
		default:
			switch (status)
			{
			case PresenceStatus.PLAY_GAME:
			case PresenceStatus.PRACTICE_GAME:
				break;
			default:
				switch (status)
				{
				case PresenceStatus.TAVERN_BRAWL_GAME:
				case PresenceStatus.TAVERN_BRAWL_FRIENDLY_GAME:
					break;
				default:
					if (status != PresenceStatus.TUTORIAL_GAME && status != PresenceStatus.ADVENTURE_SCENARIO_PLAYING_GAME)
					{
						return false;
					}
					break;
				}
				break;
			}
			break;
		}
		return true;
	}

	// Token: 0x0400122E RID: 4654
	private static readonly Map<Enum, Enum> s_richPresenceMap = new Map<Enum, Enum>
	{
		{
			PresenceStatus.LOGIN,
			null
		},
		{
			PresenceStatus.WELCOMEQUESTS,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.STORE,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.QUESTLOG,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.PACKOPENING,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.COLLECTION,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.DECKEDITOR,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.CRAFTING,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.PLAY_DECKPICKER,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.PLAY_QUEUE,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.PRACTICE_DECKPICKER,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.ARENA_PURCHASE,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.ARENA_FORGE,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.ARENA_IDLE,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.ARENA_QUEUE,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.ARENA_REWARD,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.FRIENDLY_DECKPICKER,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.ADVENTURE_CHOOSING_MODE,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.ADVENTURE_SCENARIO_SELECT,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.TAVERN_BRAWL_SCREEN,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.TAVERN_BRAWL_DECKEDITOR,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.TAVERN_BRAWL_QUEUE,
			PresenceStatus.HUB
		},
		{
			PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING,
			PresenceStatus.HUB
		}
	};

	// Token: 0x0400122F RID: 4655
	private static readonly Map<Enum, string> s_stringKeyMap = new Map<Enum, string>
	{
		{
			PresenceStatus.LOGIN,
			"PRESENCE_STATUS_LOGIN"
		},
		{
			PresenceStatus.TUTORIAL_PREGAME,
			"PRESENCE_STATUS_TUTORIAL_PREGAME"
		},
		{
			PresenceStatus.TUTORIAL_GAME,
			"PRESENCE_STATUS_TUTORIAL_GAME"
		},
		{
			PresenceStatus.WELCOMEQUESTS,
			"PRESENCE_STATUS_WELCOMEQUESTS"
		},
		{
			PresenceStatus.HUB,
			"PRESENCE_STATUS_HUB"
		},
		{
			PresenceStatus.STORE,
			"PRESENCE_STATUS_STORE"
		},
		{
			PresenceStatus.QUESTLOG,
			"PRESENCE_STATUS_QUESTLOG"
		},
		{
			PresenceStatus.PACKOPENING,
			"PRESENCE_STATUS_PACKOPENING"
		},
		{
			PresenceStatus.COLLECTION,
			"PRESENCE_STATUS_COLLECTION"
		},
		{
			PresenceStatus.DECKEDITOR,
			"PRESENCE_STATUS_DECKEDITOR"
		},
		{
			PresenceStatus.CRAFTING,
			"PRESENCE_STATUS_CRAFTING"
		},
		{
			PresenceStatus.PLAY_DECKPICKER,
			"PRESENCE_STATUS_PLAY_DECKPICKER"
		},
		{
			PresenceStatus.PLAY_QUEUE,
			"PRESENCE_STATUS_PLAY_QUEUE"
		},
		{
			PresenceStatus.PLAY_GAME,
			"PRESENCE_STATUS_PLAY_GAME"
		},
		{
			PresenceStatus.PRACTICE_DECKPICKER,
			"PRESENCE_STATUS_PRACTICE_DECKPICKER"
		},
		{
			PresenceStatus.PRACTICE_GAME,
			"PRESENCE_STATUS_PRACTICE_GAME"
		},
		{
			PresenceStatus.ARENA_PURCHASE,
			"PRESENCE_STATUS_ARENA_PURCHASE"
		},
		{
			PresenceStatus.ARENA_FORGE,
			"PRESENCE_STATUS_ARENA_FORGE"
		},
		{
			PresenceStatus.ARENA_IDLE,
			"PRESENCE_STATUS_ARENA_IDLE"
		},
		{
			PresenceStatus.ARENA_QUEUE,
			"PRESENCE_STATUS_ARENA_QUEUE"
		},
		{
			PresenceStatus.ARENA_GAME,
			"PRESENCE_STATUS_ARENA_GAME"
		},
		{
			PresenceStatus.ARENA_REWARD,
			"PRESENCE_STATUS_ARENA_REWARD"
		},
		{
			PresenceStatus.FRIENDLY_DECKPICKER,
			"PRESENCE_STATUS_FRIENDLY_DECKPICKER"
		},
		{
			PresenceStatus.FRIENDLY_GAME,
			"PRESENCE_STATUS_FRIENDLY_GAME"
		},
		{
			PresenceStatus.ADVENTURE_CHOOSING_MODE,
			"PRESENCE_STATUS_ADVENTURE_CHOOSING_MODE"
		},
		{
			PresenceStatus.ADVENTURE_SCENARIO_SELECT,
			"PRESENCE_STATUS_ADVENTURE_SCENARIO_SELECT"
		},
		{
			PresenceStatus.ADVENTURE_SCENARIO_PLAYING_GAME,
			"PRESENCE_STATUS_ADVENTURE_SCENARIO_PLAYING_GAME"
		},
		{
			PresenceStatus.TAVERN_BRAWL_SCREEN,
			"PRESENCE_STATUS_TAVERN_BRAWL_SCREEN"
		},
		{
			PresenceStatus.TAVERN_BRAWL_DECKEDITOR,
			"PRESENCE_STATUS_TAVERN_BRAWL_DECKEDITOR"
		},
		{
			PresenceStatus.TAVERN_BRAWL_QUEUE,
			"PRESENCE_STATUS_TAVERN_BRAWL_QUEUE"
		},
		{
			PresenceStatus.TAVERN_BRAWL_GAME,
			"PRESENCE_STATUS_TAVERN_BRAWL_GAME"
		},
		{
			PresenceStatus.TAVERN_BRAWL_FRIENDLY_WAITING,
			"PRESENCE_STATUS_TAVERN_BRAWL_FRIENDLY_WAITING"
		},
		{
			PresenceStatus.TAVERN_BRAWL_FRIENDLY_GAME,
			"PRESENCE_STATUS_TAVERN_BRAWL_FRIENDLY_GAME"
		},
		{
			PresenceStatus.SPECTATING_GAME_TUTORIAL,
			"PRESENCE_STATUS_SPECTATING_GAME_TUTORIAL"
		},
		{
			PresenceStatus.SPECTATING_GAME_PRACTICE,
			"PRESENCE_STATUS_SPECTATING_GAME_PRACTICE"
		},
		{
			PresenceStatus.SPECTATING_GAME_PLAY,
			"PRESENCE_STATUS_SPECTATING_GAME_PLAY"
		},
		{
			PresenceStatus.SPECTATING_GAME_ARENA,
			"PRESENCE_STATUS_SPECTATING_GAME_ARENA"
		},
		{
			PresenceStatus.SPECTATING_GAME_FRIENDLY,
			"PRESENCE_STATUS_SPECTATING_GAME_FRIENDLY"
		},
		{
			PresenceStatus.SPECTATING_GAME_ADVENTURE_NAXX_NORMAL,
			"PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_NAXX_NORMAL"
		},
		{
			PresenceStatus.SPECTATING_GAME_ADVENTURE_NAXX_HEROIC,
			"PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_NAXX_HEROIC"
		},
		{
			PresenceStatus.SPECTATING_GAME_ADVENTURE_NAXX_CLASS_CHALLENGE,
			"PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_NAXX_CLASS_CHALLENGE"
		},
		{
			PresenceStatus.SPECTATING_GAME_ADVENTURE_BRM_NORMAL,
			"PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_BRM_NORMAL"
		},
		{
			PresenceStatus.SPECTATING_GAME_ADVENTURE_BRM_HEROIC,
			"PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_BRM_HEROIC"
		},
		{
			PresenceStatus.SPECTATING_GAME_ADVENTURE_BRM_CLASS_CHALLENGE,
			"PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_BRM_CLASS_CHALLENGE"
		},
		{
			PresenceStatus.SPECTATING_GAME_ADVENTURE_LOE_NORMAL,
			"PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_LOE_NORMAL"
		},
		{
			PresenceStatus.SPECTATING_GAME_ADVENTURE_LOE_HEROIC,
			"PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_LOE_HEROIC"
		},
		{
			PresenceStatus.SPECTATING_GAME_ADVENTURE_LOE_CLASS_CHALLENGE,
			"PRESENCE_STATUS_SPECTATING_GAME_ADVENTURE_LOE_CLASS_CHALLENGE"
		},
		{
			PresenceStatus.SPECTATING_GAME_TAVERN_BRAWL,
			"PRESENCE_STATUS_SPECTATING_GAME_TAVERN_BRAWL"
		},
		{
			PresenceTutorial.HOGGER,
			"PRESENCE_TUTORIAL_HOGGER"
		},
		{
			PresenceTutorial.MILLHOUSE,
			"PRESENCE_TUTORIAL_MILLHOUSE"
		},
		{
			PresenceTutorial.MUKLA,
			"PRESENCE_TUTORIAL_MUKLA"
		},
		{
			PresenceTutorial.HEMET,
			"PRESENCE_TUTORIAL_HEMET"
		},
		{
			PresenceTutorial.ILLIDAN,
			"PRESENCE_TUTORIAL_ILLIDAN"
		},
		{
			PresenceTutorial.CHO,
			"PRESENCE_TUTORIAL_CHO"
		},
		{
			PresenceAdventureMode.NAXX_NORMAL,
			"PRESENCE_ADVENTURE_MODE_NAXX_NORMAL"
		},
		{
			PresenceAdventureMode.NAXX_HEROIC,
			"PRESENCE_ADVENTURE_MODE_NAXX_HEROIC"
		},
		{
			PresenceAdventureMode.NAXX_CLASS_CHALLENGE,
			"PRESENCE_ADVENTURE_MODE_NAXX_CLASS_CHALLENGE"
		},
		{
			PresenceAdventureMode.BRM_NORMAL,
			"PRESENCE_ADVENTURE_MODE_BRM_NORMAL"
		},
		{
			PresenceAdventureMode.BRM_HEROIC,
			"PRESENCE_ADVENTURE_MODE_BRM_HEROIC"
		},
		{
			PresenceAdventureMode.BRM_CLASS_CHALLENGE,
			"PRESENCE_ADVENTURE_MODE_BRM_CLASS_CHALLENGE"
		},
		{
			PresenceAdventureMode.LOE_NORMAL,
			"PRESENCE_ADVENTURE_MODE_LOE_NORMAL"
		},
		{
			PresenceAdventureMode.LOE_HEROIC,
			"PRESENCE_ADVENTURE_MODE_LOE_HEROIC"
		},
		{
			PresenceAdventureMode.LOE_CLASS_CHALLENGE,
			"PRESENCE_ADVENTURE_MODE_LOE_CLASS_CHALLENGE"
		},
		{
			ScenarioDbId.NAXX_ANUBREKHAN,
			"PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_ANUBREKHAN"
		},
		{
			ScenarioDbId.NAXX_FAERLINA,
			"PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_FAERLINA"
		},
		{
			ScenarioDbId.NAXX_MAEXXNA,
			"PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_MAEXXNA"
		},
		{
			ScenarioDbId.NAXX_NOTH,
			"PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_NOTH"
		},
		{
			ScenarioDbId.NAXX_HEIGAN,
			"PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_HEIGAN"
		},
		{
			ScenarioDbId.NAXX_LOATHEB,
			"PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_LOATHEB"
		},
		{
			ScenarioDbId.NAXX_RAZUVIOUS,
			"PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_RAZUVIOUS"
		},
		{
			ScenarioDbId.NAXX_GOTHIK,
			"PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_GOTHIK"
		},
		{
			ScenarioDbId.NAXX_HORSEMEN,
			"PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_HORSEMEN"
		},
		{
			ScenarioDbId.NAXX_PATCHWERK,
			"PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_PATCHWERK"
		},
		{
			ScenarioDbId.NAXX_GROBBULUS,
			"PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_GROBBULUS"
		},
		{
			ScenarioDbId.NAXX_GLUTH,
			"PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_GLUTH"
		},
		{
			ScenarioDbId.NAXX_THADDIUS,
			"PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_THADDIUS"
		},
		{
			ScenarioDbId.NAXX_SAPPHIRON,
			"PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_SAPPHIRON"
		},
		{
			ScenarioDbId.NAXX_KELTHUZAD,
			"PRESENCE_SCENARIO_NAXX_NORMAL_SCENARIO_KELTHUZAD"
		},
		{
			ScenarioDbId.NAXX_HEROIC_ANUBREKHAN,
			"PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_ANUBREKHAN"
		},
		{
			ScenarioDbId.NAXX_HEROIC_FAERLINA,
			"PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_FAERLINA"
		},
		{
			ScenarioDbId.NAXX_HEROIC_MAEXXNA,
			"PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_MAEXXNA"
		},
		{
			ScenarioDbId.NAXX_HEROIC_NOTH,
			"PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_NOTH"
		},
		{
			ScenarioDbId.NAXX_HEROIC_HEIGAN,
			"PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_HEIGAN"
		},
		{
			ScenarioDbId.NAXX_HEROIC_LOATHEB,
			"PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_LOATHEB"
		},
		{
			ScenarioDbId.NAXX_HEROIC_RAZUVIOUS,
			"PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_RAZUVIOUS"
		},
		{
			ScenarioDbId.NAXX_HEROIC_GOTHIK,
			"PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_GOTHIK"
		},
		{
			ScenarioDbId.NAXX_HEROIC_HORSEMEN,
			"PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_HORSEMEN"
		},
		{
			ScenarioDbId.NAXX_HEROIC_PATCHWERK,
			"PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_PATCHWERK"
		},
		{
			ScenarioDbId.NAXX_HEROIC_GROBBULUS,
			"PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_GROBBULUS"
		},
		{
			ScenarioDbId.NAXX_HEROIC_GLUTH,
			"PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_GLUTH"
		},
		{
			ScenarioDbId.NAXX_HEROIC_THADDIUS,
			"PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_THADDIUS"
		},
		{
			ScenarioDbId.NAXX_HEROIC_SAPPHIRON,
			"PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_SAPPHIRON"
		},
		{
			ScenarioDbId.NAXX_HEROIC_KELTHUZAD,
			"PRESENCE_SCENARIO_NAXX_HEROIC_SCENARIO_KELTHUZAD"
		},
		{
			ScenarioDbId.NAXX_CHALLENGE_HUNTER_V_LOATHEB,
			"PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_HUNTER"
		},
		{
			ScenarioDbId.NAXX_CHALLENGE_WARRIOR_V_GROBBULUS,
			"PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_WARRIOR"
		},
		{
			ScenarioDbId.NAXX_CHALLENGE_ROGUE_V_MAEXXNA,
			"PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_ROGUE"
		},
		{
			ScenarioDbId.NAXX_CHALLENGE_DRUID_V_FAERLINA,
			"PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_DRUID"
		},
		{
			ScenarioDbId.NAXX_CHALLENGE_PRIEST_V_THADDIUS,
			"PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_PRIEST"
		},
		{
			ScenarioDbId.NAXX_CHALLENGE_SHAMAN_V_GOTHIK,
			"PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_SHAMAN"
		},
		{
			ScenarioDbId.NAXX_CHALLENGE_MAGE_V_HEIGAN,
			"PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_MAGE"
		},
		{
			ScenarioDbId.NAXX_CHALLENGE_PALADIN_V_KELTHUZAD,
			"PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_PALADIN"
		},
		{
			ScenarioDbId.NAXX_CHALLENGE_WARLOCK_V_HORSEMEN,
			"PRESENCE_SCENARIO_NAXX_CLASS_CHALLENGE_WARLOCK"
		},
		{
			ScenarioDbId.BRM_GRIM_GUZZLER,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_GRIM_GUZZLER"
		},
		{
			ScenarioDbId.BRM_DARK_IRON_ARENA,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_DARK_IRON_ARENA"
		},
		{
			ScenarioDbId.BRM_THAURISSAN,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_THAURISSAN"
		},
		{
			ScenarioDbId.BRM_GARR,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_GARR"
		},
		{
			ScenarioDbId.BRM_MAJORDOMO,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_MAJORDOMO"
		},
		{
			ScenarioDbId.BRM_BARON_GEDDON,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_BARON_GEDDON"
		},
		{
			ScenarioDbId.BRM_OMOKK,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_OMOKK"
		},
		{
			ScenarioDbId.BRM_DRAKKISATH,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_DRAKKISATH"
		},
		{
			ScenarioDbId.BRM_REND_BLACKHAND,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_REND_BLACKHAND"
		},
		{
			ScenarioDbId.BRM_RAZORGORE,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_RAZORGORE"
		},
		{
			ScenarioDbId.BRM_VAELASTRASZ,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_VAELASTRASZ"
		},
		{
			ScenarioDbId.BRM_CHROMAGGUS,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_CHROMAGGUS"
		},
		{
			ScenarioDbId.BRM_NEFARIAN,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_NEFARIAN"
		},
		{
			ScenarioDbId.BRM_OMNOTRON,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_OMNOTRON"
		},
		{
			ScenarioDbId.BRM_MALORIAK,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_MALORIAK"
		},
		{
			ScenarioDbId.BRM_ATRAMEDES,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_ATRAMEDES"
		},
		{
			ScenarioDbId.BRM_ZOMBIE_NEF,
			"PRESENCE_SCENARIO_BRM_NORMAL_SCENARIO_ZOMBIE_NEF"
		},
		{
			ScenarioDbId.BRM_HEROIC_GRIM_GUZZLER,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_GRIM_GUZZLER"
		},
		{
			ScenarioDbId.BRM_HEROIC_DARK_IRON_ARENA,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_DARK_IRON_ARENA"
		},
		{
			ScenarioDbId.BRM_HEROIC_THAURISSAN,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_THAURISSAN"
		},
		{
			ScenarioDbId.BRM_HEROIC_GARR,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_GARR"
		},
		{
			ScenarioDbId.BRM_HEROIC_MAJORDOMO,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_MAJORDOMO"
		},
		{
			ScenarioDbId.BRM_HEROIC_BARON_GEDDON,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_BARON_GEDDON"
		},
		{
			ScenarioDbId.BRM_HEROIC_OMOKK,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_OMOKK"
		},
		{
			ScenarioDbId.BRM_HEROIC_DRAKKISATH,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_DRAKKISATH"
		},
		{
			ScenarioDbId.BRM_HEROIC_REND_BLACKHAND,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_REND_BLACKHAND"
		},
		{
			ScenarioDbId.BRM_HEROIC_RAZORGORE,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_RAZORGORE"
		},
		{
			ScenarioDbId.BRM_HEROIC_VAELASTRASZ,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_VAELASTRASZ"
		},
		{
			ScenarioDbId.BRM_HEROIC_CHROMAGGUS,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_CHROMAGGUS"
		},
		{
			ScenarioDbId.BRM_HEROIC_NEFARIAN,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_NEFARIAN"
		},
		{
			ScenarioDbId.BRM_HEROIC_OMNOTRON,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_OMNOTRON"
		},
		{
			ScenarioDbId.BRM_HEROIC_MALORIAK,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_MALORIAK"
		},
		{
			ScenarioDbId.BRM_HEROIC_ATRAMEDES,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_ATRAMEDES"
		},
		{
			ScenarioDbId.BRM_HEROIC_ZOMBIE_NEF,
			"PRESENCE_SCENARIO_BRM_HEROIC_SCENARIO_ZOMBIE_NEF"
		},
		{
			ScenarioDbId.BRM_CHALLENGE_HUNTER_V_GUZZLER,
			"PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_HUNTER"
		},
		{
			ScenarioDbId.BRM_CHALLENGE_WARRIOR_V_GARR,
			"PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_WARRIOR"
		},
		{
			ScenarioDbId.BRM_CHALLENGE_ROGUE_V_VAELASTRASZ,
			"PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_ROGUE"
		},
		{
			ScenarioDbId.BRM_CHALLENGE_DRUID_V_BLACKHAND,
			"PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_DRUID"
		},
		{
			ScenarioDbId.BRM_CHALLENGE_PRIEST_V_DRAKKISATH,
			"PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_PRIEST"
		},
		{
			ScenarioDbId.BRM_CHALLENGE_SHAMAN_V_GEDDON,
			"PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_SHAMAN"
		},
		{
			ScenarioDbId.BRM_CHALLENGE_MAGE_V_DARK_IRON_ARENA,
			"PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_MAGE"
		},
		{
			ScenarioDbId.BRM_CHALLENGE_PALADIN_V_OMNOTRON,
			"PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_PALADIN"
		},
		{
			ScenarioDbId.BRM_CHALLENGE_WARLOCK_V_RAZORGORE,
			"PRESENCE_SCENARIO_BRM_CLASS_CHALLENGE_WARLOCK"
		},
		{
			ScenarioDbId.LOE_ZINAAR,
			"PRESENCE_SCENARIO_LOE_NORMAL_ZINAAR"
		},
		{
			ScenarioDbId.LOE_SUN_RAIDER_PHAERIX,
			"PRESENCE_SCENARIO_LOE_NORMAL_SUN_RAIDER_PHAERIX"
		},
		{
			ScenarioDbId.LOE_TEMPLE_ESCAPE,
			"PRESENCE_SCENARIO_LOE_NORMAL_TEMPLE_ESCAPE"
		},
		{
			ScenarioDbId.LOE_SCARVASH,
			"PRESENCE_SCENARIO_LOE_NORMAL_SCARVASH"
		},
		{
			ScenarioDbId.LOE_MINE_CART,
			"PRESENCE_SCENARIO_LOE_NORMAL_MINE_CART"
		},
		{
			ScenarioDbId.LOE_ARCHAEDAS,
			"PRESENCE_SCENARIO_LOE_NORMAL_ARCHAEDAS"
		},
		{
			ScenarioDbId.LOE_SLITHERSPEAR,
			"PRESENCE_SCENARIO_LOE_NORMAL_SLITHERSPEAR"
		},
		{
			ScenarioDbId.LOE_GIANTFIN,
			"PRESENCE_SCENARIO_LOE_NORMAL_GIANTFIN"
		},
		{
			ScenarioDbId.LOE_LADY_NAZJAR,
			"PRESENCE_SCENARIO_LOE_NORMAL_LADY_NAZJAR"
		},
		{
			ScenarioDbId.LOE_SKELESAURUS,
			"PRESENCE_SCENARIO_LOE_NORMAL_SKELESAURUS"
		},
		{
			ScenarioDbId.LOE_STEEL_SENTINEL,
			"PRESENCE_SCENARIO_LOE_NORMAL_STEEL_SENTINEL"
		},
		{
			ScenarioDbId.LOE_RAFAAM_1,
			"PRESENCE_SCENARIO_LOE_NORMAL_RAFAAM_1"
		},
		{
			ScenarioDbId.LOE_RAFAAM_2,
			"PRESENCE_SCENARIO_LOE_NORMAL_RAFAAM_2"
		},
		{
			ScenarioDbId.LOE_HEROIC_ZINAAR,
			"PRESENCE_SCENARIO_LOE_HEROIC_ZINAAR"
		},
		{
			ScenarioDbId.LOE_HEROIC_SUN_RAIDER_PHAERIX,
			"PRESENCE_SCENARIO_LOE_HEROIC_SUN_RAIDER_PHAERIX"
		},
		{
			ScenarioDbId.LOE_HEROIC_TEMPLE_ESCAPE,
			"PRESENCE_SCENARIO_LOE_HEROIC_TEMPLE_ESCAPE"
		},
		{
			ScenarioDbId.LOE_HEROIC_SCARVASH,
			"PRESENCE_SCENARIO_LOE_HEROIC_SCARVASH"
		},
		{
			ScenarioDbId.LOE_HEROIC_MINE_CART,
			"PRESENCE_SCENARIO_LOE_HEROIC_MINE_CART"
		},
		{
			ScenarioDbId.LOE_HEROIC_ARCHAEDAS,
			"PRESENCE_SCENARIO_LOE_HEROIC_ARCHAEDAS"
		},
		{
			ScenarioDbId.LOE_HEROIC_SLITHERSPEAR,
			"PRESENCE_SCENARIO_LOE_HEROIC_SLITHERSPEAR"
		},
		{
			ScenarioDbId.LOE_HEROIC_GIANTFIN,
			"PRESENCE_SCENARIO_LOE_HEROIC_GIANTFIN"
		},
		{
			ScenarioDbId.LOE_HEROIC_LADY_NAZJAR,
			"PRESENCE_SCENARIO_LOE_HEROIC_LADY_NAZJAR"
		},
		{
			ScenarioDbId.LOE_HEROIC_SKELESAURUS,
			"PRESENCE_SCENARIO_LOE_HEROIC_SKELESAURUS"
		},
		{
			ScenarioDbId.LOE_HEROIC_STEEL_SENTINEL,
			"PRESENCE_SCENARIO_LOE_HEROIC_STEEL_SENTINEL"
		},
		{
			ScenarioDbId.LOE_HEROIC_RAFAAM_1,
			"PRESENCE_SCENARIO_LOE_HEROIC_RAFAAM_1"
		},
		{
			ScenarioDbId.LOE_HEROIC_RAFAAM_2,
			"PRESENCE_SCENARIO_LOE_HEROIC_RAFAAM_2"
		},
		{
			ScenarioDbId.LOE_CHALLENGE_WARRIOR_V_ZINAAR,
			"PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_WARRIOR"
		},
		{
			ScenarioDbId.LOE_CHALLENGE_WARLOCK_V_SUN_RAIDER,
			"PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_WARLOCK"
		},
		{
			ScenarioDbId.LOE_CHALLENGE_DRUID_V_SCARVASH,
			"PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_DRUID"
		},
		{
			ScenarioDbId.LOE_CHALLENGE_PALADIN_V_ARCHAEDUS,
			"PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_PALADIN"
		},
		{
			ScenarioDbId.LOE_CHALLENGE_HUNTER_V_SLITHERSPEAR,
			"PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_HUNTER"
		},
		{
			ScenarioDbId.LOE_CHALLENGE_SHAMAN_V_GIANTFIN,
			"PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_SHAMAN"
		},
		{
			ScenarioDbId.LOE_CHALLENGE_PRIEST_V_NAZJAR,
			"PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_PRIEST"
		},
		{
			ScenarioDbId.LOE_CHALLENGE_ROGUE_V_SKELESAURUS,
			"PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_ROGUE"
		},
		{
			ScenarioDbId.LOE_CHALLENGE_MAGE_V_SENTINEL,
			"PRESENCE_SCENARIO_LOE_CLASS_CHALLENGE_MAGE"
		}
	};

	// Token: 0x04001230 RID: 4656
	private static readonly Map<KeyValuePair<AdventureDbId, AdventureModeDbId>, PresenceMgr.PresenceTargets> s_adventurePresenceMap = new Map<KeyValuePair<AdventureDbId, AdventureModeDbId>, PresenceMgr.PresenceTargets>
	{
		{
			new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.NAXXRAMAS, AdventureModeDbId.NORMAL),
			new PresenceMgr.PresenceTargets(PresenceAdventureMode.NAXX_NORMAL, PresenceStatus.SPECTATING_GAME_ADVENTURE_NAXX_NORMAL)
		},
		{
			new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.NAXXRAMAS, AdventureModeDbId.HEROIC),
			new PresenceMgr.PresenceTargets(PresenceAdventureMode.NAXX_HEROIC, PresenceStatus.SPECTATING_GAME_ADVENTURE_NAXX_HEROIC)
		},
		{
			new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.NAXXRAMAS, AdventureModeDbId.CLASS_CHALLENGE),
			new PresenceMgr.PresenceTargets(PresenceAdventureMode.NAXX_CLASS_CHALLENGE, PresenceStatus.SPECTATING_GAME_ADVENTURE_NAXX_CLASS_CHALLENGE)
		},
		{
			new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.BRM, AdventureModeDbId.NORMAL),
			new PresenceMgr.PresenceTargets(PresenceAdventureMode.BRM_NORMAL, PresenceStatus.SPECTATING_GAME_ADVENTURE_BRM_NORMAL)
		},
		{
			new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.BRM, AdventureModeDbId.HEROIC),
			new PresenceMgr.PresenceTargets(PresenceAdventureMode.BRM_HEROIC, PresenceStatus.SPECTATING_GAME_ADVENTURE_BRM_HEROIC)
		},
		{
			new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.BRM, AdventureModeDbId.CLASS_CHALLENGE),
			new PresenceMgr.PresenceTargets(PresenceAdventureMode.BRM_CLASS_CHALLENGE, PresenceStatus.SPECTATING_GAME_ADVENTURE_BRM_CLASS_CHALLENGE)
		},
		{
			new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.LOE, AdventureModeDbId.NORMAL),
			new PresenceMgr.PresenceTargets(PresenceAdventureMode.LOE_NORMAL, PresenceStatus.SPECTATING_GAME_ADVENTURE_LOE_NORMAL)
		},
		{
			new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.LOE, AdventureModeDbId.HEROIC),
			new PresenceMgr.PresenceTargets(PresenceAdventureMode.LOE_HEROIC, PresenceStatus.SPECTATING_GAME_ADVENTURE_LOE_HEROIC)
		},
		{
			new KeyValuePair<AdventureDbId, AdventureModeDbId>(AdventureDbId.LOE, AdventureModeDbId.CLASS_CHALLENGE),
			new PresenceMgr.PresenceTargets(PresenceAdventureMode.LOE_CLASS_CHALLENGE, PresenceStatus.SPECTATING_GAME_ADVENTURE_LOE_CLASS_CHALLENGE)
		}
	};

	// Token: 0x04001231 RID: 4657
	private static readonly Type[] s_enumIdList = new Type[]
	{
		typeof(PresenceStatus),
		typeof(PresenceTutorial),
		typeof(PresenceAdventureMode),
		typeof(ScenarioDbId)
	};

	// Token: 0x04001232 RID: 4658
	private static PresenceMgr s_instance;

	// Token: 0x04001233 RID: 4659
	private Map<Type, byte> m_enumToIdMap = new Map<Type, byte>();

	// Token: 0x04001234 RID: 4660
	private Map<byte, Type> m_idToEnumMap = new Map<byte, Type>();

	// Token: 0x04001235 RID: 4661
	private Enum[] m_prevStatus;

	// Token: 0x04001236 RID: 4662
	private Enum[] m_status;

	// Token: 0x04001237 RID: 4663
	private Enum[] m_richPresence;

	// Token: 0x020005DA RID: 1498
	private struct PresenceTargets
	{
		// Token: 0x0600428B RID: 17035 RVA: 0x001410D8 File Offset: 0x0013F2D8
		public PresenceTargets(PresenceAdventureMode enteringAdventureValue, PresenceStatus spectatingValue)
		{
			this.EnteringAdventureValue = enteringAdventureValue;
			this.SpectatingValue = spectatingValue;
		}

		// Token: 0x04002A55 RID: 10837
		public PresenceAdventureMode EnteringAdventureValue;

		// Token: 0x04002A56 RID: 10838
		public PresenceStatus SpectatingValue;
	}
}
