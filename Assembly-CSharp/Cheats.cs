using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using bgs;
using bnet.protocol.attribute;
using PegasusShared;
using PegasusUtil;
using SimpleJSON;
using SpectatorProto;
using UnityEngine;

// Token: 0x02000207 RID: 519
public class Cheats
{
	// Token: 0x06001F0F RID: 7951 RVA: 0x000934A7 File Offset: 0x000916A7
	public static Cheats Get()
	{
		return Cheats.s_instance;
	}

	// Token: 0x06001F10 RID: 7952 RVA: 0x000934AE File Offset: 0x000916AE
	public static void Initialize()
	{
		Cheats.s_instance = new Cheats();
		Cheats.s_instance.InitializeImpl();
	}

	// Token: 0x06001F11 RID: 7953 RVA: 0x000934C4 File Offset: 0x000916C4
	public string GetBoard()
	{
		return this.m_board;
	}

	// Token: 0x06001F12 RID: 7954 RVA: 0x000934CC File Offset: 0x000916CC
	public bool IsYourMindFree()
	{
		return this.m_isYourMindFree;
	}

	// Token: 0x06001F13 RID: 7955 RVA: 0x000934D4 File Offset: 0x000916D4
	public bool IsLaunchingQuickGame()
	{
		return this.m_quickLaunchState.m_launching;
	}

	// Token: 0x06001F14 RID: 7956 RVA: 0x000934E1 File Offset: 0x000916E1
	public bool QuickGameSkipMulligan()
	{
		return this.m_quickLaunchState.m_skipMulligan;
	}

	// Token: 0x06001F15 RID: 7957 RVA: 0x000934EE File Offset: 0x000916EE
	public bool QuickGameFlipHeroes()
	{
		return this.m_quickLaunchState.m_flipHeroes;
	}

	// Token: 0x06001F16 RID: 7958 RVA: 0x000934FB File Offset: 0x000916FB
	public bool QuickGameMirrorHeroes()
	{
		return this.m_quickLaunchState.m_mirrorHeroes;
	}

	// Token: 0x06001F17 RID: 7959 RVA: 0x00093508 File Offset: 0x00091708
	public string QuickGameOpponentHeroCardId()
	{
		return this.m_quickLaunchState.m_opponentHeroCardId;
	}

	// Token: 0x06001F18 RID: 7960 RVA: 0x00093515 File Offset: 0x00091715
	public bool HandleKeyboardInput()
	{
		return ApplicationMgr.IsInternal() && this.HandleQuickPlayInput();
	}

	// Token: 0x06001F19 RID: 7961 RVA: 0x00093530 File Offset: 0x00091730
	private void InitializeImpl()
	{
		CheatMgr cheatMgr = CheatMgr.Get();
		if (ApplicationMgr.IsInternal())
		{
			cheatMgr.RegisterCheatHandler("collectionfirstxp", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_collectionfirstxp), "Set the number of page and cover flips to zero", string.Empty, string.Empty);
			cheatMgr.RegisterCheatHandler("board", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_board), "Set which board will be loaded on the next game", "<BRM|STW|GVG>", "BRM");
			cheatMgr.RegisterCheatHandler("brode", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_brode), "Brode's personal cheat", string.Empty, string.Empty);
			cheatMgr.RegisterCheatHandler("resettips", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_resettips), "Resets Innkeeper tips for collection manager", string.Empty, string.Empty);
			cheatMgr.RegisterCheatHandler("questcomplete", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_questcomplete), "Shows the quest complete achievement screen", "<quest_id>", "58");
			cheatMgr.RegisterCheatHandler("questprogress", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_questprogress), "Pop up a quest progress toast", "<title> <description> <progress> <maxprogress>", "Hello World 3 10");
			cheatMgr.RegisterCheatHandler("questwelcome", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_questwelcome), "Open list of daily quests", "<fromLogin>", "true");
			cheatMgr.RegisterCheatHandler("newquest", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_newquest), "Shows a new quest, only usable while a quest popup is active", null, null);
			cheatMgr.RegisterCheatHandler("storepassword", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_storepassword), "Show store challenge popup", string.Empty, string.Empty);
			cheatMgr.RegisterCheatHandler("retire", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_retire), "Retires your draft deck", string.Empty, string.Empty);
			cheatMgr.RegisterCheatHandler("defaultcardback", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_defaultcardback), "Set your cardback as if through the options menu", "<cardback id>", null);
			cheatMgr.RegisterCheatHandler("disconnect", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_disconnect), "Disconnects you from a game in progress.", null, null);
			cheatMgr.RegisterCheatHandler("seasonroll", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_seasonroll), "Open the season end dialog", "<season number> <ending rank> <is wild rank>", "20 7 false");
			cheatMgr.RegisterCheatHandler("playnullsound", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_playnullsound), "Tell SoundManager to play a null sound.", null, null);
			cheatMgr.RegisterCheatHandler("spectate", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_spectate), "Connects to a game server to spectate", "<ip_address> <port> <game_handle> <spectator_password> [gameType] [missionId]", null);
			cheatMgr.RegisterCheatHandler("party", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_party), "Run a variety of party related commands", "[sub command] [subcommand args]", "list");
			cheatMgr.RegisterCheatHandler("cheat", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_cheat), "Send a cheat command to the server", "<command> <arguments>", null);
			cheatMgr.RegisterCheatAlias("cheat", new string[]
			{
				"c"
			});
			cheatMgr.RegisterCheatHandler("autohand", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_autohand), "Set whether PhoneUI automatically hides your hand after playing a card", "<true/false>", "true");
			cheatMgr.RegisterCheatHandler("fixedrewardcomplete", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_fixedrewardcomplete), "Shows the visual for a fixed reward", "<fixed_reward_map_id>", null);
			cheatMgr.RegisterCheatHandler("iks", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_iks), "Open InnKeepersSpecial with a custom url", "<url>", null);
			cheatMgr.RegisterCheatHandler("adventureChallengeUnlock", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_adventureChallengeUnlock), "Show adventure challenge unlock", "<wing number>", null);
			cheatMgr.RegisterCheatHandler("quote", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_quote), string.Empty, "<character> <line> [sound]", "Innkeeper VO_INNKEEPER_FORGE_COMPLETE_22 VO_INNKEEPER_ARENA_COMPLETE");
			cheatMgr.RegisterCheatHandler("demotext", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_demotext), string.Empty, "<line>", "HelloWorld!");
			cheatMgr.RegisterCheatHandler("popuptext", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_popuptext), string.Empty, "<line>", "HelloWorld!");
			cheatMgr.RegisterCheatHandler("favoritehero", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_favoritehero), "Change your favorite hero for a class (only works from CollectionManager)", "<class_id> <hero_card_id> <hero_premium>", null);
			cheatMgr.RegisterCheatHandler("rewardboxes", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_rewardboxes), "Open the reward box screen with example rewards", "<card|cardback|gold|dust|random> <num_boxes>", string.Empty);
			cheatMgr.RegisterCheatHandler("rankchange", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_rankchange), "Open the rankchange twoscoop", "<start_rank> <end_rank> [start_stars] [end_stars] [chest|winstreak]", "6 5 chest");
			cheatMgr.RegisterCheatHandler("easyrank", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_easyrank), "Easier cheat command to set your rank on the util server", "<rank>", "16");
			cheatMgr.RegisterCheatHandler("timescale", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_timescale), "Cheat to change the timescale", "<timescale>", "0.5");
			cheatMgr.RegisterCheatHandler("onlygold", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_onlygold), "In collection manager, do you want to see gold, nogold, or both?", "<command name>", string.Empty);
			cheatMgr.RegisterCheatHandler("help", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_help), "Get help for a specific command or list of commands", "<command name>", string.Empty);
			cheatMgr.RegisterCheatHandler("example", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_example), "Run an example of this command if one exists", "<command name>", null);
			cheatMgr.RegisterCheatHandler("tb", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_tavernbrawl), "Run a variety of Tavern Brawl related commands", "[subcommand] [subcommand args]", "view");
			cheatMgr.RegisterCheatHandler("util", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_utilservercmd), "Run a cheat on the UTIL server you're connected to.", "[subcommand] [subcommand args]", "help");
			cheatMgr.RegisterCheatHandler("game", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_gameservercmd), "[NYI] Run a cheat on the GAME server you're connected to.", "[subcommand] [subcommand args]", "help");
			Network.Get().RegisterNetHandler(324, new Network.NetHandler(this.OnProcessCheat_utilservercmd_OnResponse), null);
			cheatMgr.RegisterCheatHandler("scenario", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_scenario), "Launch a scenario.", "<scenario_id> [<game_type_id>] [<deck_name>|<deck_id>]", null);
			cheatMgr.RegisterCheatAlias("scenario", new string[]
			{
				"mission"
			});
			cheatMgr.RegisterCheatHandler("exportcards", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_exportcards), "Export images of cards", null, null);
			cheatMgr.RegisterCheatHandler("freeyourmind", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_freeyourmind), "And the rest will follow", null, null);
			cheatMgr.RegisterCheatHandler("reloadgamestrings", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_reloadgamestrings), "Reload all game strings from GLUE/GLOBAL/etc.", null, null);
			cheatMgr.RegisterCheatHandler("attn", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_userattentionmanager), "Prints out what UserAttentionBlockers, if any, are currently active.", null, null);
			cheatMgr.RegisterCheatHandler("auto_exportgamestate", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_autoexportgamestate), "Save JSON file serializing some of GameState", null, null);
		}
		cheatMgr.RegisterCheatHandler("has", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_HasOption), "Query whether a Game Option exists.", null, null);
		cheatMgr.RegisterCheatHandler("get", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_GetOption), "Get the value of a Game Option.", null, null);
		cheatMgr.RegisterCheatHandler("set", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_SetOption), "Set the value of a Game Option.", null, null);
		cheatMgr.RegisterCheatHandler("getvar", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_GetVar), "Get the value of a client.config var.", null, null);
		cheatMgr.RegisterCheatHandler("setvar", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_SetVar), "Set the value of a client.config var.", null, null);
		cheatMgr.RegisterCheatHandler("nav", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_navigation), "Debug Navigation.GoBack", null, null);
		cheatMgr.RegisterCheatHandler("delete", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_DeleteOption), "Delete a Game Option; the absence of option may trigger default behavior", null, null);
		cheatMgr.RegisterCheatAlias("delete", new string[]
		{
			"del"
		});
		cheatMgr.RegisterCheatHandler("warning", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_warning), "Show a warning message", "<message>", "Test You're a cheater and you've been warned!");
		cheatMgr.RegisterCheatHandler("fatal", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_fatal), "Brings up the Fatal Error screen", "<error to display>", "Hearthstone cheated and failed!");
		cheatMgr.RegisterCheatHandler("exit", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_exit), "Exit the application", string.Empty, string.Empty);
		cheatMgr.RegisterCheatAlias("exit", new string[]
		{
			"quit"
		});
		cheatMgr.RegisterCheatHandler("log", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_log), null, null, null);
		cheatMgr.RegisterCheatHandler("autodraft", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_autodraft), "Sets Arena autodraft on/off.", "<on | off>", "on");
		cheatMgr.RegisterCheatHandler("alert", new CheatMgr.ProcessCheatCallback(this.OnProcessCheat_alert), "Show a popup alert", "header=<string> text=<string> icon=<bool> response=<ok|confirm|cancel|confirm_cancel> oktext=<string> confirmtext=<string>", "header=header text=body text icon=true response=confirm");
		cheatMgr.RegisterCheatAlias("alert", new string[]
		{
			"popup",
			"dialog"
		});
	}

	// Token: 0x06001F1A RID: 7962 RVA: 0x00093D50 File Offset: 0x00091F50
	private void ParseErrorText(string[] args, string rawArgs, out string header, out string message)
	{
		header = ((args.Length != 0) ? args[0] : "[PH] Header");
		if (args.Length <= 1)
		{
			message = "[PH] Message";
		}
		else
		{
			int num = 0;
			bool flag = false;
			for (int i = 0; i < rawArgs.Length; i++)
			{
				char c = rawArgs.get_Chars(i);
				if (char.IsWhiteSpace(c))
				{
					if (flag)
					{
						num = i;
						break;
					}
				}
				else
				{
					flag = true;
				}
			}
			message = rawArgs.Substring(num).Trim();
		}
	}

	// Token: 0x06001F1B RID: 7963 RVA: 0x00093DDC File Offset: 0x00091FDC
	private AlertPopup.PopupInfo GenerateAlertInfo(string rawArgs)
	{
		Map<string, string> map = this.ParseAlertArgs(rawArgs);
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_showAlertIcon = false;
		popupInfo.m_headerText = "Header";
		popupInfo.m_text = "Message";
		popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
		popupInfo.m_okText = "OK";
		popupInfo.m_confirmText = "Confirm";
		popupInfo.m_cancelText = "Cancel";
		foreach (KeyValuePair<string, string> keyValuePair in map)
		{
			string key = keyValuePair.Key;
			string text = keyValuePair.Value;
			if (key.Equals("header"))
			{
				popupInfo.m_headerText = text;
			}
			else if (key.Equals("text"))
			{
				popupInfo.m_text = text;
			}
			else if (key.Equals("response"))
			{
				text = text.ToLowerInvariant();
				if (text.Equals("ok"))
				{
					popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
				}
				else if (text.Equals("confirm"))
				{
					popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM;
				}
				else if (text.Equals("cancel"))
				{
					popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CANCEL;
				}
				else if (text.Equals("confirm_cancel") || text.Equals("cancel_confirm"))
				{
					popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
				}
			}
			else if (key.Equals("icon"))
			{
				popupInfo.m_showAlertIcon = GeneralUtils.ForceBool(text);
			}
			else if (key.Equals("oktext"))
			{
				popupInfo.m_okText = text;
			}
			else if (key.Equals("confirmtext"))
			{
				popupInfo.m_confirmText = text;
			}
			else if (key.Equals("canceltext"))
			{
				popupInfo.m_cancelText = text;
			}
			else if (key.Equals("offset"))
			{
				string[] array = text.Split(new char[0]);
				Vector3 offset = default(Vector3);
				if (array.Length % 2 == 0)
				{
					for (int i = 0; i < array.Length; i += 2)
					{
						string text2 = array[i].ToLowerInvariant();
						string str = array[i + 1];
						if (text2.Equals("x"))
						{
							offset.x = GeneralUtils.ForceFloat(str);
						}
						else if (text2.Equals("y"))
						{
							offset.y = GeneralUtils.ForceFloat(str);
						}
						else if (text2.Equals("z"))
						{
							offset.z = GeneralUtils.ForceFloat(str);
						}
					}
				}
				popupInfo.m_offset = offset;
			}
			else if (key.Equals("padding"))
			{
				popupInfo.m_padding = GeneralUtils.ForceFloat(text);
			}
		}
		return popupInfo;
	}

	// Token: 0x06001F1C RID: 7964 RVA: 0x000940E8 File Offset: 0x000922E8
	private Map<string, string> ParseAlertArgs(string rawArgs)
	{
		Map<string, string> map = new Map<string, string>();
		int num = -1;
		string text = null;
		int num3;
		for (int i = 0; i < rawArgs.Length; i++)
		{
			char c = rawArgs.get_Chars(i);
			if (c == '=')
			{
				int num2 = -1;
				for (int j = i - 1; j >= 0; j--)
				{
					char c2 = rawArgs.get_Chars(j);
					char c3 = rawArgs.get_Chars(j + 1);
					if (!char.IsWhiteSpace(c2))
					{
						num2 = j;
					}
					if (char.IsWhiteSpace(c2) && !char.IsWhiteSpace(c3))
					{
						break;
					}
				}
				if (num2 >= 0)
				{
					num3 = num2 - 2;
					if (text != null)
					{
						map[text] = rawArgs.Substring(num, num3 - num + 1);
					}
					num = i + 1;
					text = rawArgs.Substring(num2, i - num2).Trim().ToLowerInvariant();
				}
			}
		}
		num3 = rawArgs.Length - 1;
		if (text != null)
		{
			map[text] = rawArgs.Substring(num, num3 - num + 1);
		}
		return map;
	}

	// Token: 0x06001F1D RID: 7965 RVA: 0x000941FE File Offset: 0x000923FE
	private bool OnAlertProcessed(DialogBase dialog, object userData)
	{
		this.m_alert = (AlertPopup)dialog;
		return true;
	}

	// Token: 0x06001F1E RID: 7966 RVA: 0x0009420D File Offset: 0x0009240D
	private void OnAlertResponse(AlertPopup.Response response, object userData)
	{
		this.m_alert = null;
	}

	// Token: 0x06001F1F RID: 7967 RVA: 0x00094218 File Offset: 0x00092418
	private bool HandleQuickPlayInput()
	{
		if (SceneMgr.Get() == null)
		{
			return false;
		}
		if (!Input.GetKey(304) && !Input.GetKey(303))
		{
			return false;
		}
		if (Input.GetKeyDown(293))
		{
			this.PrintQuickPlayLegend();
			return false;
		}
		Cheats.QuickLaunchAvailability quickLaunchAvailability = this.GetQuickLaunchAvailability();
		if (quickLaunchAvailability != Cheats.QuickLaunchAvailability.OK)
		{
			return false;
		}
		ScenarioDbId scenarioDbId = ScenarioDbId.INVALID;
		string opponentHeroCardId = null;
		foreach (KeyValuePair<KeyCode, ScenarioDbId> keyValuePair in Cheats.s_quickPlayKeyMap)
		{
			KeyCode key = keyValuePair.Key;
			ScenarioDbId value = keyValuePair.Value;
			if (Input.GetKeyDown(key))
			{
				scenarioDbId = value;
				opponentHeroCardId = Cheats.s_opponentHeroKeyMap[key];
				break;
			}
		}
		if (scenarioDbId == ScenarioDbId.INVALID)
		{
			return false;
		}
		this.m_quickLaunchState.m_mirrorHeroes = false;
		this.m_quickLaunchState.m_flipHeroes = false;
		this.m_quickLaunchState.m_skipMulligan = true;
		this.m_quickLaunchState.m_opponentHeroCardId = opponentHeroCardId;
		if ((Input.GetKey(307) || Input.GetKey(308)) && (Input.GetKey(305) || Input.GetKey(306)))
		{
			this.m_quickLaunchState.m_mirrorHeroes = true;
			this.m_quickLaunchState.m_skipMulligan = false;
			this.m_quickLaunchState.m_flipHeroes = false;
		}
		else if (Input.GetKey(305) || Input.GetKey(306))
		{
			this.m_quickLaunchState.m_flipHeroes = false;
			this.m_quickLaunchState.m_skipMulligan = false;
			this.m_quickLaunchState.m_mirrorHeroes = false;
		}
		else if (Input.GetKey(307) || Input.GetKey(308))
		{
			this.m_quickLaunchState.m_flipHeroes = true;
			this.m_quickLaunchState.m_skipMulligan = false;
			this.m_quickLaunchState.m_mirrorHeroes = false;
		}
		this.LaunchQuickGame((int)scenarioDbId, 1, null);
		return true;
	}

	// Token: 0x06001F20 RID: 7968 RVA: 0x0009442C File Offset: 0x0009262C
	private void PrintQuickPlayLegend()
	{
		string message = string.Format("F1: {0}\nF2: {1}\nF3: {2}\nF4: {3}\nF5: {4}\nF6: {5}\nF7: {6}\nF8: {7}\nF9: {8}\n(CTRL and ALT will Show mulligan)\nSHIFT + CTRL = Hero on players side\nSHIFT + ALT = Hero on opponent side\nSHIFT + ALT + CTRL = Hero on both sides", new object[]
		{
			this.GetQuickPlayMissionName(282),
			this.GetQuickPlayMissionName(283),
			this.GetQuickPlayMissionName(284),
			this.GetQuickPlayMissionName(285),
			this.GetQuickPlayMissionName(286),
			this.GetQuickPlayMissionName(287),
			this.GetQuickPlayMissionName(288),
			this.GetQuickPlayMissionName(289),
			this.GetQuickPlayMissionName(290)
		});
		if (UIStatus.Get() != null)
		{
			UIStatus.Get().AddInfo(message);
		}
		string text = string.Format("F1: {0}  F2: {1}  F3: {2}  F4: {3}  F5: {4}  F6: {5}  F7: {6}  F8: {7}  F9: {8}\n(CTRL and ALT will Show mulligan) -- SHIFT + CTRL = Hero on players side -- SHIFT + ALT = Hero on opponent side -- SHIFT + ALT + CTRL = Hero on both sides", new object[]
		{
			this.GetQuickPlayMissionShortName(282),
			this.GetQuickPlayMissionShortName(283),
			this.GetQuickPlayMissionShortName(284),
			this.GetQuickPlayMissionShortName(285),
			this.GetQuickPlayMissionShortName(286),
			this.GetQuickPlayMissionShortName(287),
			this.GetQuickPlayMissionShortName(288),
			this.GetQuickPlayMissionShortName(289),
			this.GetQuickPlayMissionShortName(290)
		});
		Debug.Log(text);
	}

	// Token: 0x06001F21 RID: 7969 RVA: 0x0009457A File Offset: 0x0009277A
	private string GetQuickPlayMissionName(KeyCode keyCode)
	{
		return this.GetQuickPlayMissionName((int)Cheats.s_quickPlayKeyMap[keyCode]);
	}

	// Token: 0x06001F22 RID: 7970 RVA: 0x0009458D File Offset: 0x0009278D
	private string GetQuickPlayMissionShortName(KeyCode keyCode)
	{
		return this.GetQuickPlayMissionShortName((int)Cheats.s_quickPlayKeyMap[keyCode]);
	}

	// Token: 0x06001F23 RID: 7971 RVA: 0x000945A0 File Offset: 0x000927A0
	private string GetQuickPlayMissionName(int missionId)
	{
		return this.GetQuickPlayMissionNameImpl(missionId, "NAME");
	}

	// Token: 0x06001F24 RID: 7972 RVA: 0x000945AE File Offset: 0x000927AE
	private string GetQuickPlayMissionShortName(int missionId)
	{
		return this.GetQuickPlayMissionNameImpl(missionId, "SHORT_NAME");
	}

	// Token: 0x06001F25 RID: 7973 RVA: 0x000945BC File Offset: 0x000927BC
	private string GetQuickPlayMissionNameImpl(int missionId, string columnName)
	{
		ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(missionId);
		if (record != null)
		{
			DbfLocValue dbfLocValue = (DbfLocValue)record.GetVar(columnName);
			if (dbfLocValue != null)
			{
				return dbfLocValue.GetString(true);
			}
		}
		string result = missionId.ToString();
		try
		{
			ScenarioDbId scenarioDbId = (ScenarioDbId)missionId;
			result = scenarioDbId.ToString();
		}
		catch (Exception)
		{
		}
		return result;
	}

	// Token: 0x06001F26 RID: 7974 RVA: 0x00094628 File Offset: 0x00092828
	private Cheats.QuickLaunchAvailability GetQuickLaunchAvailability()
	{
		if (this.m_quickLaunchState.m_launching)
		{
			return Cheats.QuickLaunchAvailability.ACTIVE_GAME;
		}
		if (SceneMgr.Get().IsInGame())
		{
			return Cheats.QuickLaunchAvailability.ACTIVE_GAME;
		}
		if (GameMgr.Get().IsFindingGame())
		{
			return Cheats.QuickLaunchAvailability.FINDING_GAME;
		}
		if (SceneMgr.Get().GetNextMode() != SceneMgr.Mode.INVALID)
		{
			return Cheats.QuickLaunchAvailability.SCENE_TRANSITION;
		}
		if (!SceneMgr.Get().IsSceneLoaded())
		{
			return Cheats.QuickLaunchAvailability.SCENE_TRANSITION;
		}
		if (LoadingScreen.Get().IsTransitioning())
		{
			return Cheats.QuickLaunchAvailability.ACTIVE_GAME;
		}
		if (CollectionManager.Get() == null || !CollectionManager.Get().IsFullyLoaded())
		{
			return Cheats.QuickLaunchAvailability.COLLECTION_NOT_READY;
		}
		return Cheats.QuickLaunchAvailability.OK;
	}

	// Token: 0x06001F27 RID: 7975 RVA: 0x000946B8 File Offset: 0x000928B8
	private void LaunchQuickGame(int missionId, GameType gameType = 1, CollectionDeck deck = null)
	{
		this.m_quickLaunchState.m_launching = true;
		long num;
		string text;
		if (deck == null)
		{
			num = Options.Get().GetLong(Option.LAST_CUSTOM_DECK_CHOSEN);
			deck = CollectionManager.Get().GetDeck(num);
			if (deck == null)
			{
				TAG_CLASS tag_CLASS = TAG_CLASS.MAGE;
				num = CollectionManager.Get().GetPreconDeck(tag_CLASS).ID;
				text = string.Format("Precon {0}", GameStrings.GetClassName(tag_CLASS));
			}
			else
			{
				text = deck.Name;
			}
		}
		else
		{
			num = deck.ID;
			text = deck.Name;
		}
		string quickPlayMissionName = this.GetQuickPlayMissionName(missionId);
		string message = string.Format("Launching {0}\nDeck: {1}", quickPlayMissionName, text);
		UIStatus.Get().AddInfo(message);
		Time.timeScale = SceneDebugger.Get().m_MaxTimeScale;
		SceneMgr.Get().RegisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		GameMgr.Get().SetPendingAutoConcede(true);
		GameMgr.Get().RegisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
		GameMgr.Get().FindGame(gameType, missionId, num, 0L);
	}

	// Token: 0x06001F28 RID: 7976 RVA: 0x000947B1 File Offset: 0x000929B1
	private void OnSceneLoaded(SceneMgr.Mode mode, Scene scene, object userData)
	{
		if (SceneMgr.Get().GetPrevMode() != SceneMgr.Mode.GAMEPLAY)
		{
			return;
		}
		if (mode == SceneMgr.Mode.GAMEPLAY)
		{
			return;
		}
		SceneMgr.Get().UnregisterSceneLoadedEvent(new SceneMgr.SceneLoadedCallback(this.OnSceneLoaded));
		this.m_quickLaunchState = new Cheats.QuickLaunchState();
	}

	// Token: 0x06001F29 RID: 7977 RVA: 0x000947F0 File Offset: 0x000929F0
	private bool OnFindGameEvent(FindGameEventData eventData, object userData)
	{
		switch (eventData.m_state)
		{
		case FindGameState.CLIENT_CANCELED:
		case FindGameState.CLIENT_ERROR:
		case FindGameState.BNET_QUEUE_CANCELED:
		case FindGameState.BNET_ERROR:
		case FindGameState.SERVER_GAME_CANCELED:
			GameMgr.Get().UnregisterFindGameEvent(new GameMgr.FindGameCallback(this.OnFindGameEvent));
			this.m_quickLaunchState = new Cheats.QuickLaunchState();
			break;
		}
		return false;
	}

	// Token: 0x06001F2A RID: 7978 RVA: 0x00094864 File Offset: 0x00092A64
	private JSONArray GetCardlistJson(List<Card> list)
	{
		JSONArray jsonarray = new JSONArray();
		for (int i = 0; i < list.Count; i++)
		{
			JSONClass cardJson = this.GetCardJson(list[i].GetEntity());
			jsonarray.Add(cardJson);
		}
		return jsonarray;
	}

	// Token: 0x06001F2B RID: 7979 RVA: 0x000948AC File Offset: 0x00092AAC
	private JSONClass GetCardJson(Entity card)
	{
		if (card == null)
		{
			return null;
		}
		JSONClass jsonclass = new JSONClass();
		jsonclass["cardName"] = card.GetName();
		jsonclass["cardID"] = card.GetCardId();
		jsonclass["entityID"].AsInt = card.GetEntityId();
		JSONArray jsonarray = new JSONArray();
		if (card.GetTags() != null)
		{
			Map<int, int> map = card.GetTags().GetMap();
			foreach (KeyValuePair<int, int> keyValuePair in map)
			{
				JSONClass jsonclass2 = new JSONClass();
				string text = Enum.GetName(typeof(GAME_TAG), keyValuePair.Key);
				if (text == null)
				{
					text = "NOTAG_" + keyValuePair.Key.ToString();
				}
				jsonclass2[text].AsInt = keyValuePair.Value;
				jsonarray.Add(jsonclass2);
			}
			jsonclass["tags"] = jsonarray;
		}
		JSONArray jsonarray2 = new JSONArray();
		List<Entity> enchantments = card.GetEnchantments();
		for (int i = 0; i < Enumerable.Count<Entity>(enchantments); i++)
		{
			JSONClass cardJson = this.GetCardJson(enchantments[i]);
			jsonarray2.Add(cardJson);
		}
		jsonclass["enchantments"] = jsonarray2;
		return jsonclass;
	}

	// Token: 0x06001F2C RID: 7980 RVA: 0x00094A2C File Offset: 0x00092C2C
	private bool OnProcessCheat_HasOption(string func, string[] args, string rawArgs)
	{
		string str = args[0];
		Option @enum;
		try
		{
			@enum = EnumUtils.GetEnum<Option>(str, 5);
		}
		catch (ArgumentException)
		{
			return false;
		}
		string text = string.Format("HasOption: {0} = {1}", EnumUtils.GetString<Option>(@enum), Options.Get().HasOption(@enum));
		Debug.Log(text);
		UIStatus.Get().AddInfo(text);
		return true;
	}

	// Token: 0x06001F2D RID: 7981 RVA: 0x00094A9C File Offset: 0x00092C9C
	private bool OnProcessCheat_GetOption(string func, string[] args, string rawArgs)
	{
		string str = args[0];
		Option @enum;
		try
		{
			@enum = EnumUtils.GetEnum<Option>(str, 5);
		}
		catch (ArgumentException)
		{
			return false;
		}
		string text = string.Format("GetOption: {0} = {1}", EnumUtils.GetString<Option>(@enum), Options.Get().GetOption(@enum));
		Debug.Log(text);
		UIStatus.Get().AddInfo(text);
		return true;
	}

	// Token: 0x06001F2E RID: 7982 RVA: 0x00094B08 File Offset: 0x00092D08
	private bool OnProcessCheat_SetOption(string func, string[] args, string rawArgs)
	{
		string text = args[0];
		Option @enum;
		try
		{
			@enum = EnumUtils.GetEnum<Option>(text, 5);
		}
		catch (ArgumentException)
		{
			return false;
		}
		if (args.Length < 2)
		{
			return false;
		}
		string text2 = args[1];
		Type optionType = Options.Get().GetOptionType(@enum);
		if (optionType == typeof(bool))
		{
			bool val;
			if (!GeneralUtils.TryParseBool(text2, out val))
			{
				return false;
			}
			Options.Get().SetBool(@enum, val);
		}
		else if (optionType == typeof(int))
		{
			int val2;
			if (!GeneralUtils.TryParseInt(text2, out val2))
			{
				return false;
			}
			Options.Get().SetInt(@enum, val2);
		}
		else if (optionType == typeof(long))
		{
			long val3;
			if (!GeneralUtils.TryParseLong(text2, out val3))
			{
				return false;
			}
			Options.Get().SetLong(@enum, val3);
		}
		else if (optionType == typeof(float))
		{
			float val4;
			if (!GeneralUtils.TryParseFloat(text2, out val4))
			{
				return false;
			}
			Options.Get().SetFloat(@enum, val4);
		}
		else if (optionType == typeof(string))
		{
			text2 = rawArgs.Remove(0, text.Length + 1);
			Options.Get().SetString(@enum, text2);
		}
		if (@enum == Option.CURSOR)
		{
			Cursor.visible = Options.Get().GetBool(Option.CURSOR);
		}
		else if (@enum == Option.FAKE_PACK_OPENING)
		{
			NetCache.Get().ReloadNetObject<NetCache.NetCacheBoosters>();
		}
		else if (@enum == Option.FAKE_PACK_COUNT && GameUtils.IsFakePackOpeningEnabled())
		{
			NetCache.Get().ReloadNetObject<NetCache.NetCacheBoosters>();
		}
		string text3 = string.Format("SetOption: {0} to {1}. GetOption = {2}", EnumUtils.GetString<Option>(@enum), text2, Options.Get().GetOption(@enum));
		Debug.Log(text3);
		UIStatus.Get().AddInfo(text3);
		return true;
	}

	// Token: 0x06001F2F RID: 7983 RVA: 0x00094CD8 File Offset: 0x00092ED8
	private bool OnProcessCheat_GetVar(string func, string[] args, string rawArgs)
	{
		string text = args[0];
		string text2 = string.Format("Var: {0} = {1}", text, Vars.Key(text).GetStr(null) ?? "(null)");
		Debug.Log(text2);
		UIStatus.Get().AddInfo(text2);
		return true;
	}

	// Token: 0x06001F30 RID: 7984 RVA: 0x00094D20 File Offset: 0x00092F20
	private bool OnProcessCheat_SetVar(string func, string[] args, string rawArgs)
	{
		string text = args[0];
		string text2 = (args.Length >= 2) ? args[1] : null;
		VarsInternal.Get().Set(text, text2);
		string text3 = string.Format("Var: {0} = {1}", text, text2 ?? "(null)");
		Debug.Log(text3);
		UIStatus.Get().AddInfo(text3);
		if (text.Equals("Arena.AutoDraft", 3) && DraftDisplay.Get() != null)
		{
			DraftDisplay.Get().StartCoroutine(DraftDisplay.Get().RunAutoDraftCheat());
		}
		return true;
	}

	// Token: 0x06001F31 RID: 7985 RVA: 0x00094DB4 File Offset: 0x00092FB4
	private bool OnProcessCheat_autodraft(string func, string[] args, string rawArgs)
	{
		string text = args[0];
		bool flag = string.IsNullOrEmpty(text) || GeneralUtils.ForceBool(text);
		VarsInternal.Get().Set("Arena.AutoDraft", (!flag) ? "false" : "true");
		if (flag && DraftDisplay.Get() != null)
		{
			DraftDisplay.Get().StartCoroutine(DraftDisplay.Get().RunAutoDraftCheat());
		}
		else if (!flag)
		{
			SceneDebugger.SetDevTimescale(1f);
		}
		string text2 = string.Format("Arena autodraft turned {0}.", (!flag) ? "off" : "on");
		Debug.Log(text2);
		UIStatus.Get().AddInfo(text2);
		return true;
	}

	// Token: 0x06001F32 RID: 7986 RVA: 0x00094E70 File Offset: 0x00093070
	private bool OnProcessCheat_onlygold(string func, string[] args, string rawArgs)
	{
		string text = args[0].ToLowerInvariant();
		string text2 = text;
		if (text2 != null)
		{
			if (Cheats.<>f__switch$map7F == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
				dictionary.Add("gold", 0);
				dictionary.Add("normal", 0);
				dictionary.Add("standard", 0);
				dictionary.Add("both", 1);
				Cheats.<>f__switch$map7F = dictionary;
			}
			int num;
			if (Cheats.<>f__switch$map7F.TryGetValue(text2, ref num))
			{
				if (num != 0)
				{
					if (num != 1)
					{
						goto IL_9F;
					}
					Options.Get().DeleteOption(Option.COLLECTION_PREMIUM_TYPE);
				}
				else
				{
					Options.Get().SetString(Option.COLLECTION_PREMIUM_TYPE, text);
				}
				return true;
			}
		}
		IL_9F:
		UIStatus.Get().AddError("Unknown cmd: " + ((!string.IsNullOrEmpty(text)) ? text : "(blank)") + "\nValid cmds: gold, standard, both");
		return false;
	}

	// Token: 0x06001F33 RID: 7987 RVA: 0x00094F50 File Offset: 0x00093150
	private bool OnProcessCheat_navigation(string func, string[] args, string rawArgs)
	{
		string text = args[0].ToLowerInvariant();
		string text2 = text;
		if (text2 != null)
		{
			if (Cheats.<>f__switch$map80 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("debug", 0);
				dictionary.Add("dump", 1);
				Cheats.<>f__switch$map80 = dictionary;
			}
			int num;
			if (Cheats.<>f__switch$map80.TryGetValue(text2, ref num))
			{
				if (num == 0)
				{
					Navigation.NAVIGATION_DEBUG = (args.Length < 2 || GeneralUtils.ForceBool(args[1]));
					if (Navigation.NAVIGATION_DEBUG)
					{
						Navigation.DumpStack();
						UIStatus.Get().AddInfo("Navigation debugging turned on - see Console or output log for nav dump.");
					}
					else
					{
						UIStatus.Get().AddInfo("Navigation debugging turned off.");
					}
					return true;
				}
				if (num == 1)
				{
					Navigation.DumpStack();
					UIStatus.Get().AddInfo("Navigation dumped, see Console or output log.");
					return true;
				}
			}
		}
		UIStatus.Get().AddError("Unknown cmd: " + ((!string.IsNullOrEmpty(text)) ? text : "(blank)") + "\nValid cmds: debug, dump");
		return true;
	}

	// Token: 0x06001F34 RID: 7988 RVA: 0x00095060 File Offset: 0x00093260
	private bool OnProcessCheat_DeleteOption(string func, string[] args, string rawArgs)
	{
		string str = args[0];
		Option @enum;
		try
		{
			@enum = EnumUtils.GetEnum<Option>(str, 5);
		}
		catch (ArgumentException)
		{
			return false;
		}
		Options.Get().DeleteOption(@enum);
		string text = string.Format("DeleteOption: {0}. HasOption = {1}.", EnumUtils.GetString<Option>(@enum), Options.Get().HasOption(@enum));
		Debug.Log(text);
		UIStatus.Get().AddInfo(text);
		return true;
	}

	// Token: 0x06001F35 RID: 7989 RVA: 0x000950DC File Offset: 0x000932DC
	private bool OnProcessCheat_collectionfirstxp(string func, string[] args, string rawArgs)
	{
		Options.Get().SetInt(Option.COVER_MOUSE_OVERS, 0);
		Options.Get().SetInt(Option.PAGE_MOUSE_OVERS, 0);
		return true;
	}

	// Token: 0x06001F36 RID: 7990 RVA: 0x000950F9 File Offset: 0x000932F9
	private bool OnProcessCheat_board(string func, string[] args, string rawArgs)
	{
		this.m_board = args[0].ToUpperInvariant();
		return true;
	}

	// Token: 0x06001F37 RID: 7991 RVA: 0x0009510A File Offset: 0x0009330A
	private bool OnProcessCheat_resettips(string func, string[] args, string rawArgs)
	{
		Options.Get().SetBool(Option.HAS_SEEN_COLLECTIONMANAGER, false);
		return true;
	}

	// Token: 0x06001F38 RID: 7992 RVA: 0x0009511C File Offset: 0x0009331C
	private bool OnProcessCheat_brode(string func, string[] args, string rawArgs)
	{
		NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.ALL, new Vector3(133.1f, NotificationManager.DEPTH, 54.2f), GameStrings.Get("VO_INNKEEPER_FORGE_1WIN"), "VO_INNKEEPER_ARENA_1WIN", 0f, null);
		return true;
	}

	// Token: 0x06001F39 RID: 7993 RVA: 0x00095160 File Offset: 0x00093360
	private bool OnProcessCheat_questcomplete(string func, string[] args, string rawArgs)
	{
		QuestToast.ShowQuestToast(UserAttentionBlocker.ALL, null, false, AchieveManager.Get().GetAchievement(int.Parse(rawArgs)));
		return true;
	}

	// Token: 0x06001F3A RID: 7994 RVA: 0x00095188 File Offset: 0x00093388
	private bool OnProcessCheat_questwelcome(string func, string[] args, string rawArgs)
	{
		bool fromLogin = false;
		if (args.Length > 0 && !string.IsNullOrEmpty(args[0]))
		{
			GeneralUtils.TryParseBool(args[0], out fromLogin);
		}
		WelcomeQuests.Show(UserAttentionBlocker.ALL, fromLogin, null, false);
		return true;
	}

	// Token: 0x06001F3B RID: 7995 RVA: 0x000951C4 File Offset: 0x000933C4
	private bool OnProcessCheat_newquest(string func, string[] args, string rawArgs)
	{
		if (WelcomeQuests.Get() == null)
		{
			return false;
		}
		QuestTile firstQuestTile = WelcomeQuests.Get().GetFirstQuestTile();
		firstQuestTile.SetupTile(AchieveManager.Get().GetAchievement(int.Parse(rawArgs)));
		return true;
	}

	// Token: 0x06001F3C RID: 7996 RVA: 0x00095208 File Offset: 0x00093408
	private bool OnProcessCheat_questprogress(string func, string[] args, string rawArgs)
	{
		if (args.Length != 4)
		{
			return false;
		}
		string questName = args[0];
		string questDescription = args[1];
		int progress = int.Parse(args[2]);
		int maxProgress = int.Parse(args[3]);
		if (GameToastMgr.Get() != null)
		{
			GameToastMgr.Get().AddQuestProgressToast(questName, questDescription, progress, maxProgress);
			return true;
		}
		return false;
	}

	// Token: 0x06001F3D RID: 7997 RVA: 0x0009525C File Offset: 0x0009345C
	private bool OnProcessCheat_retire(string func, string[] args, string rawArgs)
	{
		if (DemoMgr.Get().GetMode() != DemoMode.BLIZZCON_2013)
		{
			return false;
		}
		DraftManager draftManager = DraftManager.Get();
		if (draftManager == null)
		{
			return false;
		}
		Network.RetireDraftDeck(draftManager.GetDraftDeck().ID, draftManager.GetSlot());
		return true;
	}

	// Token: 0x06001F3E RID: 7998 RVA: 0x000952A0 File Offset: 0x000934A0
	private bool OnProcessCheat_storepassword(string func, string[] args, string rawArgs)
	{
		if (this.m_loadingStoreChallengePrompt)
		{
			return true;
		}
		if (this.m_storeChallengePrompt == null)
		{
			this.m_loadingStoreChallengePrompt = true;
			AssetLoader.GameObjectCallback callback = delegate(string name, GameObject go, object callbackData)
			{
				CheatMgr.Get().StartCoroutine(this.StorePasswordCoroutine(name, go, callbackData));
			};
			AssetLoader.Get().LoadGameObject("StoreChallengePrompt", callback, null, false);
		}
		else if (this.m_storeChallengePrompt.IsShown())
		{
			this.m_storeChallengePrompt.Hide();
		}
		else
		{
			CheatMgr.Get().StartCoroutine(this.StorePasswordCoroutine(this.m_storeChallengePrompt.name, this.m_storeChallengePrompt.gameObject, null));
		}
		return true;
	}

	// Token: 0x06001F3F RID: 7999 RVA: 0x00095340 File Offset: 0x00093540
	private string GetChallengeUrl(string type)
	{
		string text = string.Format("https://login-qa-us.web.blizzard.net/login/admin/challenge/create/ct_{0}", type.ToLower());
		string text2 = "{0}?email={1}&programId={2}&platformId={3}&redirectUrl={4}&messageKey={5}&notifyRisk={6}&chooseChallenge={7}&challengeType={8}&riskTransId={9}";
		string text3 = "joe_balance@zmail.blizzard.com";
		string text4 = "wtcg";
		string text5 = "*";
		string text6 = "none";
		string empty = string.Empty;
		bool flag = false;
		bool flag2 = false;
		string empty2 = string.Empty;
		string empty3 = string.Empty;
		return string.Format(text2, new object[]
		{
			text,
			text3,
			text4,
			text5,
			text6,
			empty,
			flag,
			flag2,
			empty2,
			empty3
		});
	}

	// Token: 0x06001F40 RID: 8000 RVA: 0x000953E4 File Offset: 0x000935E4
	private IEnumerator StorePasswordCoroutine(string name, GameObject go, object callbackData)
	{
		this.m_loadingStoreChallengePrompt = false;
		this.m_storeChallengePrompt = go.GetComponent<StoreChallengePrompt>();
		this.m_storeChallengePrompt.Hide();
		Dictionary<string, string> headers = new Dictionary<string, string>();
		headers["Accept"] = "application/json;charset=UTF-8";
		headers["Accept-Language"] = Localization.GetBnetLocaleName();
		string createUrl = this.GetChallengeUrl("cvv");
		Debug.Log("creating challenge with url " + createUrl);
		WWW createChallenge = new WWW(createUrl, null, headers);
		yield return createChallenge;
		Debug.Log("challenge response is " + createChallenge.text);
		JSONNode createChallengeJson = JSON.Parse(createChallenge.text);
		string challengeUrl = createChallengeJson["challenge_url"];
		Debug.Log("challenge url is " + challengeUrl);
		yield return this.m_storeChallengePrompt.StartCoroutine(this.m_storeChallengePrompt.Show(challengeUrl));
		yield break;
	}

	// Token: 0x06001F41 RID: 8001 RVA: 0x00095410 File Offset: 0x00093610
	private bool OnProcessCheat_defaultcardback(string func, string[] args, string rawArgs)
	{
		if (args.Length == 0)
		{
			return false;
		}
		int defaultCardBack;
		if (!int.TryParse(args[0].ToLowerInvariant(), ref defaultCardBack))
		{
			return false;
		}
		ConnectAPI.SetDefaultCardBack(defaultCardBack);
		return true;
	}

	// Token: 0x06001F42 RID: 8002 RVA: 0x00095444 File Offset: 0x00093644
	private bool OnProcessCheat_disconnect(string func, string[] args, string rawArgs)
	{
		if (!Network.IsConnectedToGameServer())
		{
			return false;
		}
		bool flag = args == null || args.Length == 0 || args[0] != "force";
		Log.LoadingScreen.Print("Cheats.OnProcessCheat_disconnect() - reconnect={0}", new object[]
		{
			ReconnectMgr.Get().IsReconnectEnabled()
		});
		if (flag)
		{
			if (ReconnectMgr.Get().IsReconnectEnabled())
			{
				Network.DisconnectFromGameServer();
			}
			else
			{
				Network.Concede();
			}
		}
		else
		{
			ConnectAPI.SimulateUncleanDisconnectFromGameServer();
		}
		return true;
	}

	// Token: 0x06001F43 RID: 8003 RVA: 0x000954D4 File Offset: 0x000936D4
	private bool OnProcessCheat_warning(string func, string[] args, string rawArgs)
	{
		string header;
		string message;
		this.ParseErrorText(args, rawArgs, out header, out message);
		Error.AddWarning(header, message, new object[0]);
		return true;
	}

	// Token: 0x06001F44 RID: 8004 RVA: 0x000954FB File Offset: 0x000936FB
	private bool OnProcessCheat_fatal(string func, string[] args, string rawArgs)
	{
		Error.AddFatal(rawArgs);
		return true;
	}

	// Token: 0x06001F45 RID: 8005 RVA: 0x00095504 File Offset: 0x00093704
	private bool OnProcessCheat_suicide(string func, string[] args, string rawArgs)
	{
		string text = args[0].ToLowerInvariant();
		int num = 0;
		int.TryParse(text, ref num);
		Application.CommitSuicide(num);
		return true;
	}

	// Token: 0x06001F46 RID: 8006 RVA: 0x0009552C File Offset: 0x0009372C
	private bool OnProcessCheat_exit(string func, string[] args, string rawArgs)
	{
		GeneralUtils.ExitApplication();
		return true;
	}

	// Token: 0x06001F47 RID: 8007 RVA: 0x00095534 File Offset: 0x00093734
	private bool OnProcessCheat_log(string func, string[] args, string rawArgs)
	{
		string text = args[0].ToLowerInvariant();
		if (text == "load" || text == "reload")
		{
			Log.Get().Load();
			return true;
		}
		return false;
	}

	// Token: 0x06001F48 RID: 8008 RVA: 0x0009557C File Offset: 0x0009377C
	private bool OnProcessCheat_alert(string func, string[] args, string rawArgs)
	{
		AlertPopup.PopupInfo info = this.GenerateAlertInfo(rawArgs);
		if (this.m_alert == null)
		{
			DialogManager.Get().ShowPopup(info, new DialogManager.DialogProcessCallback(this.OnAlertProcessed));
		}
		else
		{
			this.m_alert.UpdateInfo(info);
		}
		return true;
	}

	// Token: 0x06001F49 RID: 8009 RVA: 0x000955CC File Offset: 0x000937CC
	private bool GetBonusStarsAndLevel(int lastSeasonRank, out int bonusStars, out int newSeasonRank)
	{
		int num = 26 - lastSeasonRank;
		bonusStars = num - 1;
		int num2 = 1;
		newSeasonRank = 26 - num2;
		switch (num)
		{
		case 1:
			num2 = 1;
			break;
		case 2:
			num2 = 1;
			break;
		case 3:
			num2 = 1;
			break;
		case 4:
			num2 = 2;
			break;
		case 5:
			num2 = 2;
			break;
		case 6:
			num2 = 3;
			break;
		case 7:
			num2 = 3;
			break;
		case 8:
			num2 = 4;
			break;
		case 9:
			num2 = 4;
			break;
		case 10:
			num2 = 5;
			break;
		case 11:
			num2 = 5;
			break;
		case 12:
			num2 = 6;
			break;
		case 13:
			num2 = 6;
			break;
		case 14:
			num2 = 6;
			break;
		case 15:
			num2 = 7;
			break;
		case 16:
			num2 = 7;
			break;
		case 17:
			num2 = 7;
			break;
		case 18:
			num2 = 8;
			break;
		case 19:
			num2 = 8;
			break;
		case 20:
			num2 = 8;
			break;
		case 21:
			num2 = 9;
			break;
		case 22:
			num2 = 9;
			break;
		case 23:
			num2 = 9;
			break;
		case 24:
			num2 = 10;
			break;
		case 25:
			num2 = 10;
			break;
		case 26:
			num2 = 10;
			break;
		default:
			return false;
		}
		newSeasonRank = 26 - num2;
		return true;
	}

	// Token: 0x06001F4A RID: 8010 RVA: 0x00095728 File Offset: 0x00093928
	private bool OnProcessCheat_seasonroll(string func, string[] args, string rawArgs)
	{
		if (args.Length < 3)
		{
			UIStatus.Get().AddInfo("please specify the following params:\n<season number> <ending rank> <is wild rank>");
			return true;
		}
		int seasonID;
		if (!int.TryParse(args[0].ToLowerInvariant(), ref seasonID))
		{
			UIStatus.Get().AddInfo("please enter a valid season #");
			return true;
		}
		int num;
		if (!int.TryParse(args[1].ToLowerInvariant(), ref num))
		{
			UIStatus.Get().AddInfo("please enter a valid rank #");
			return true;
		}
		bool isWild;
		if (!GeneralUtils.TryParseBool(args[2], out isWild))
		{
			UIStatus.Get().AddInfo("please enter a valid bool value for 3rd parameter <is wild rank>");
			return true;
		}
		int bonusStars;
		int boostedRank;
		if (!this.GetBonusStarsAndLevel(num, out bonusStars, out boostedRank))
		{
			UIStatus.Get().AddInfo("could not find an appropriate BonusStarsAndLevel for Rank " + num + " :(");
			return true;
		}
		SeasonEndDialog.SeasonEndInfo seasonEndInfo = new SeasonEndDialog.SeasonEndInfo();
		seasonEndInfo.m_seasonID = seasonID;
		seasonEndInfo.m_rank = num;
		seasonEndInfo.m_chestRank = num;
		seasonEndInfo.m_legendIndex = 0;
		seasonEndInfo.m_bonusStars = bonusStars;
		seasonEndInfo.m_boostedRank = boostedRank;
		seasonEndInfo.m_isWild = isWild;
		seasonEndInfo.m_rankedRewards = new List<RewardData>();
		if (num <= 20)
		{
			CardBackDbfRecord record = GameDbf.CardBack.GetRecord((CardBackDbfRecord r) => r.Source == "season" && r.Data1 == (long)seasonID);
			int cardBackID = (record != null) ? record.ID : 0;
			if (record == null)
			{
				IEnumerable<CardBackDbfRecord> enumerable = Enumerable.Where<CardBackDbfRecord>(GameDbf.CardBack.GetRecords(), (CardBackDbfRecord r) => r.Source == "season");
				long num2 = Enumerable.Min<CardBackDbfRecord>(enumerable, (CardBackDbfRecord r) => r.Data1);
				long num3 = Enumerable.Max<CardBackDbfRecord>(enumerable, (CardBackDbfRecord r) => r.Data1);
				UIStatus.Get().AddInfo(string.Format("NOTE: there is no cardback for Season {0}, using default cardback.\nKnown seasons with cardbacks: {1}-{2}", seasonID, num2, num3), 10f);
			}
			seasonEndInfo.m_rankedRewards.Add(new CardBackRewardData(cardBackID));
			seasonEndInfo.m_rankedRewards.Add(new CardRewardData("EX1_279", TAG_PREMIUM.GOLDEN, 1));
			if (num <= 15)
			{
				seasonEndInfo.m_rankedRewards.Add(new CardRewardData("EX1_279", TAG_PREMIUM.GOLDEN, 1));
			}
			if (num <= 10)
			{
				seasonEndInfo.m_rankedRewards.Add(new CardRewardData("EX1_279", TAG_PREMIUM.GOLDEN, 1));
			}
			if (num <= 5)
			{
				seasonEndInfo.m_rankedRewards.Add(new CardRewardData("EX1_279", TAG_PREMIUM.GOLDEN, 1));
			}
		}
		seasonEndInfo.m_isFake = true;
		DialogManager.DialogRequest dialogRequest = new DialogManager.DialogRequest();
		dialogRequest.m_type = DialogManager.DialogType.SEASON_END;
		dialogRequest.m_info = seasonEndInfo;
		dialogRequest.m_isFake = true;
		DialogManager.Get().AddToQueue(dialogRequest);
		return true;
	}

	// Token: 0x06001F4B RID: 8011 RVA: 0x000959F1 File Offset: 0x00093BF1
	private bool OnProcessCheat_playnullsound(string func, string[] args, string rawArgs)
	{
		SoundManager.Get().Play(null);
		return true;
	}

	// Token: 0x06001F4C RID: 8012 RVA: 0x00095A00 File Offset: 0x00093C00
	private bool OnProcessCheat_spectate(string func, string[] args, string rawArgs)
	{
		if (args.Length >= 1 && args[0] == "waiting")
		{
			SpectatorManager.Get().ShowWaitingForNextGameDialog();
			return true;
		}
		if (args.Length >= 4)
		{
			if (!Enumerable.Any<string>(args, (string a) => string.IsNullOrEmpty(a)))
			{
				JoinInfo joinInfo = new JoinInfo();
				joinInfo.ServerIpAddress = args[0];
				joinInfo.SecretKey = args[3];
				uint serverPort;
				if (!uint.TryParse(args[1], ref serverPort))
				{
					Error.AddWarning("Spectate Cheat Error", "error parsing the port # (uint) argument: " + args[1], new object[0]);
					return false;
				}
				joinInfo.ServerPort = serverPort;
				int num;
				if (!int.TryParse(args[2], ref num))
				{
					Error.AddWarning("Spectate Cheat Error", "error parsing the game_handle (int) argument: " + args[2], new object[0]);
					return false;
				}
				joinInfo.GameHandle = num;
				joinInfo.GameType = 0;
				joinInfo.MissionId = 2;
				if (args.Length >= 5 && int.TryParse(args[4], ref num))
				{
					joinInfo.GameType = num;
				}
				if (args.Length >= 6 && int.TryParse(args[5], ref num))
				{
					joinInfo.MissionId = num;
				}
				GameMgr.Get().SpectateGame(joinInfo);
				return true;
			}
		}
		Error.AddWarning("Spectate Cheat Error", "spectate cheat must have the following args:\n\nspectate ipaddress port game_handle spectator_password [gameType] [missionId]", new object[0]);
		return false;
	}

	// Token: 0x17000330 RID: 816
	// (get) Token: 0x06001F4D RID: 8013 RVA: 0x00095B56 File Offset: 0x00093D56
	private static Logger PartyLogger
	{
		get
		{
			return Log.Henry;
		}
	}

	// Token: 0x06001F4E RID: 8014 RVA: 0x00095B60 File Offset: 0x00093D60
	private static void SubscribePartyEvents()
	{
		if (Cheats.s_hasSubscribedToPartyEvents)
		{
			return;
		}
		BnetParty.OnError += delegate(PartyError error)
		{
			Cheats.PartyLogger.Print("{0} code={1} feature={2} party={3} str={4}", new object[]
			{
				error.DebugContext,
				error.ErrorCode,
				error.FeatureEvent.ToString(),
				new PartyInfo(error.PartyId, error.PartyType),
				error.StringData
			});
		};
		BnetParty.OnJoined += delegate(OnlineEventType e, PartyInfo party, LeaveReason? reason)
		{
			Cheats.PartyLogger.Print("Party.OnJoined {0} party={1} reason={2}", new object[]
			{
				e,
				party,
				(reason == null) ? "null" : reason.Value.ToString()
			});
		};
		BnetParty.OnPrivacyLevelChanged += delegate(PartyInfo party, PrivacyLevel privacy)
		{
			Cheats.PartyLogger.Print("Party.OnPrivacyLevelChanged party={0} privacy={1}", new object[]
			{
				party,
				privacy
			});
		};
		BnetParty.OnMemberEvent += delegate(OnlineEventType e, PartyInfo party, BnetGameAccountId memberId, bool isRolesUpdate, LeaveReason? reason)
		{
			Cheats.PartyLogger.Print("Party.OnMemberEvent {0} party={1} memberId={2} isRolesUpdate={3} reason={4}", new object[]
			{
				e,
				party,
				memberId,
				isRolesUpdate,
				(reason == null) ? "null" : reason.Value.ToString()
			});
		};
		BnetParty.OnReceivedInvite += delegate(OnlineEventType e, PartyInfo party, ulong inviteId, InviteRemoveReason? reason)
		{
			Cheats.PartyLogger.Print("Party.OnReceivedInvite {0} party={1} inviteId={2} reason={3}", new object[]
			{
				e,
				party,
				inviteId,
				(reason == null) ? "null" : reason.Value.ToString()
			});
		};
		BnetParty.OnSentInvite += delegate(OnlineEventType e, PartyInfo party, ulong inviteId, bool senderIsMyself, InviteRemoveReason? reason)
		{
			PartyInvite sentInvite = BnetParty.GetSentInvite(party.Id, inviteId);
			Cheats.PartyLogger.Print("Party.OnSentInvite {0} party={1} inviteId={2} senderIsMyself={3} isRejoin={4} reason={5}", new object[]
			{
				e,
				party,
				inviteId,
				senderIsMyself,
				(sentInvite != null) ? sentInvite.IsRejoin.ToString() : "null",
				(reason == null) ? "null" : reason.Value.ToString()
			});
		};
		BnetParty.OnReceivedInviteRequest += delegate(OnlineEventType e, PartyInfo party, InviteRequest request, InviteRequestRemovedReason? reason)
		{
			Cheats.PartyLogger.Print("Party.OnReceivedInviteRequest {0} party={1} target={2} {3} requester={4} {5} reason={6}", new object[]
			{
				e,
				party,
				request.TargetName,
				request.TargetId,
				request.RequesterName,
				request.RequesterId,
				(reason == null) ? "null" : reason.Value.ToString()
			});
		};
		BnetParty.OnChatMessage += delegate(PartyInfo party, BnetGameAccountId speakerId, string msg)
		{
			Cheats.PartyLogger.Print("Party.OnChatMessage party={0} speakerId={1} msg={2}", new object[]
			{
				party,
				speakerId,
				msg
			});
		};
		BnetParty.OnPartyAttributeChanged += delegate(PartyInfo party, string key, Variant attrVal)
		{
			string text = "null";
			if (attrVal.HasIntValue)
			{
				text = "[long]" + attrVal.IntValue.ToString();
			}
			else if (attrVal.HasStringValue)
			{
				text = "[string]" + attrVal.StringValue;
			}
			else if (attrVal.HasBlobValue)
			{
				byte[] blobValue = attrVal.BlobValue;
				if (blobValue != null)
				{
					text = "blobLength=" + blobValue.Length;
					try
					{
						string @string = Encoding.UTF8.GetString(blobValue);
						if (@string != null)
						{
							text = text + " decodedUtf8=" + @string;
						}
					}
					catch (ArgumentException)
					{
					}
				}
			}
			Cheats.PartyLogger.Print("BnetParty.OnPartyAttributeChanged party={0} key={1} value={2}", new object[]
			{
				party,
				key,
				text
			});
		};
		Cheats.s_hasSubscribedToPartyEvents = true;
	}

	// Token: 0x06001F4F RID: 8015 RVA: 0x00095CB0 File Offset: 0x00093EB0
	private static PartyId ParsePartyId(string cmd, string arg, int argIndex, ref string errorMsg)
	{
		PartyId partyId = null;
		ulong low;
		if (ulong.TryParse(arg, ref low))
		{
			PartyId[] joinedPartyIds = BnetParty.GetJoinedPartyIds();
			if (low >= 0UL && joinedPartyIds.Length > 0 && low < (ulong)joinedPartyIds.LongLength)
			{
				partyId = joinedPartyIds[(int)(checked((IntPtr)low))];
			}
			else
			{
				partyId = Enumerable.FirstOrDefault<PartyId>(joinedPartyIds, (PartyId p) => p.Lo == low);
			}
			if (partyId == null)
			{
				errorMsg = string.Concat(new object[]
				{
					"party ",
					cmd,
					": couldn't find party at index, or with PartyId low bits: ",
					low
				});
			}
		}
		else
		{
			PartyType type;
			if (!EnumUtils.TryGetEnum<PartyType>(arg, out type))
			{
				errorMsg = string.Concat(new string[]
				{
					"party ",
					cmd,
					": unable to parse party (index or LowBits or type)",
					(argIndex < 0) ? string.Empty : (" at arg index=" + argIndex),
					" (",
					arg,
					"), please specify the Low bits of a PartyId or a PartyType."
				});
			}
			else
			{
				partyId = Enumerable.FirstOrDefault<PartyId>(Enumerable.Select<PartyInfo, PartyId>(Enumerable.Where<PartyInfo>(BnetParty.GetJoinedParties(), (PartyInfo info) => info.Type == type), (PartyInfo info) => info.Id));
				if (partyId == null)
				{
					errorMsg = "party " + cmd + ": no joined party with PartyType: " + arg;
				}
			}
		}
		return partyId;
	}

	// Token: 0x06001F50 RID: 8016 RVA: 0x00095E38 File Offset: 0x00094038
	private bool OnProcessCheat_party(string func, string[] args, string rawArgs)
	{
		if (args.Length >= 1)
		{
			if (!Enumerable.Any<string>(args, (string a) => string.IsNullOrEmpty(a)))
			{
				string cmd = args[0];
				if (cmd == "unsubscribe")
				{
					BnetParty.RemoveFromAllEventHandlers(this);
					Cheats.s_hasSubscribedToPartyEvents = false;
					Cheats.PartyLogger.Print("party {0}: unsubscribed.", new object[]
					{
						cmd
					});
					return true;
				}
				bool result = true;
				string[] array = Enumerable.ToArray<string>(Enumerable.Skip<string>(args, 1));
				string text = null;
				Cheats.SubscribePartyEvents();
				string cmd2 = cmd;
				if (cmd2 != null)
				{
					if (Cheats.<>f__switch$map81 == null)
					{
						Dictionary<string, int> dictionary = new Dictionary<string, int>(20);
						dictionary.Add("create", 0);
						dictionary.Add("leave", 1);
						dictionary.Add("dissolve", 1);
						dictionary.Add("join", 2);
						dictionary.Add("chat", 3);
						dictionary.Add("invite", 4);
						dictionary.Add("accept", 5);
						dictionary.Add("decline", 5);
						dictionary.Add("revoke", 6);
						dictionary.Add("requestinvite", 7);
						dictionary.Add("ignorerequest", 8);
						dictionary.Add("setleader", 9);
						dictionary.Add("kick", 10);
						dictionary.Add("setprivacy", 11);
						dictionary.Add("setlong", 12);
						dictionary.Add("setstring", 12);
						dictionary.Add("setblob", 12);
						dictionary.Add("clearattr", 13);
						dictionary.Add("subscribe", 14);
						dictionary.Add("list", 14);
						Cheats.<>f__switch$map81 = dictionary;
					}
					int num;
					if (Cheats.<>f__switch$map81.TryGetValue(cmd2, ref num))
					{
						switch (num)
						{
						case 0:
							if (array.Length < 1)
							{
								text = "party create: requires a PartyType: " + string.Join(" | ", Enumerable.ToArray<string>(Enumerable.Select<PartyType, string>(Enumerable.Cast<PartyType>(Enum.GetValues(typeof(PartyType))), (PartyType v) => string.Concat(new object[]
								{
									v,
									" (",
									v,
									")"
								}))));
							}
							else
							{
								int num2;
								PartyType partyType;
								if (int.TryParse(array[0], ref num2))
								{
									partyType = num2;
								}
								else if (!EnumUtils.TryGetEnum<PartyType>(array[0], out partyType))
								{
									text = "party create: unknown PartyType specified: " + array[0];
								}
								if (text == null)
								{
									BnetGameAccountId myGameAccountId = BnetPresenceMgr.Get().GetMyGameAccountId();
									BnetId bnetId = BnetUtils.CreatePegasusBnetId(myGameAccountId);
									byte[] array2 = ProtobufUtil.ToByteArray(bnetId);
									BnetParty.CreateParty(partyType, 2, array2, delegate(PartyType t, PartyId partyId)
									{
										Cheats.PartyLogger.Print("BnetParty.CreateSuccessCallback type={0} partyId={1}", new object[]
										{
											t,
											partyId
										});
									});
								}
							}
							goto IL_2048;
						case 1:
						{
							bool flag = cmd == "dissolve";
							if (array.Length == 0)
							{
								Cheats.PartyLogger.Print("NOTE: party {0} without any arguments will {0} all joined parties.", new object[]
								{
									cmd
								});
								PartyInfo[] joinedParties = BnetParty.GetJoinedParties();
								if (joinedParties.Length == 0)
								{
									Cheats.PartyLogger.Print("No joined parties.", new object[0]);
								}
								foreach (PartyInfo partyInfo in joinedParties)
								{
									Cheats.PartyLogger.Print("party {0}: {1} party {2}", new object[]
									{
										cmd,
										(!flag) ? "leaving" : "dissolving",
										partyInfo
									});
									if (flag)
									{
										BnetParty.DissolveParty(partyInfo.Id);
									}
									else
									{
										BnetParty.Leave(partyInfo.Id);
									}
								}
							}
							else
							{
								for (int j = 0; j < array.Length; j++)
								{
									string arg2 = array[j];
									string text2 = null;
									PartyId partyId17 = Cheats.ParsePartyId(cmd, arg2, j, ref text2);
									if (text2 != null)
									{
										Cheats.PartyLogger.Print(text2, new object[0]);
									}
									if (partyId17 != null)
									{
										Cheats.PartyLogger.Print("party {0}: {1} party {2}", new object[]
										{
											cmd,
											(!flag) ? "leaving" : "dissolving",
											BnetParty.GetJoinedParty(partyId17)
										});
										if (flag)
										{
											BnetParty.DissolveParty(partyId17);
										}
										else
										{
											BnetParty.Leave(partyId17);
										}
									}
								}
							}
							goto IL_2048;
						}
						case 2:
							if (array.Length < 1)
							{
								text = "party " + cmd + ": must specify an online friend index or a partyId (Hi-Lo format)";
							}
							else
							{
								PartyType partyType2 = 0;
								foreach (string text3 in array)
								{
									int num3 = text3.IndexOf('-');
									int num4 = -1;
									PartyId partyId2 = null;
									if (num3 >= 0)
									{
										string text4 = text3.Substring(0, num3);
										string text5 = (text3.Length <= num3) ? string.Empty : text3.Substring(num3 + 1);
										ulong num5;
										ulong num6;
										if (ulong.TryParse(text4, ref num5) && ulong.TryParse(text5, ref num6))
										{
											partyId2 = new PartyId(num5, num6);
										}
										else
										{
											text = "party " + cmd + ": unable to parse partyId (in format Hi-Lo).";
										}
									}
									else if (int.TryParse(text3, ref num4))
									{
										BnetPlayer[] array5 = Enumerable.ToArray<BnetPlayer>(Enumerable.Where<BnetPlayer>(BnetFriendMgr.Get().GetFriends(), (BnetPlayer p) => p.IsOnline() && p.GetHearthstoneGameAccount() != null));
										if (num4 < 0 || num4 >= array5.Length)
										{
											text = string.Concat(new object[]
											{
												"party ",
												cmd,
												": no online friend at index ",
												num4
											});
										}
										else
										{
											text = "party " + cmd + ": Not-Yet-Implemented: find partyId from online friend's presence.";
										}
									}
									else
									{
										text = "party " + cmd + ": unable to parse online friend index.";
									}
									if (partyId2 != null)
									{
										BnetParty.JoinParty(partyId2, partyType2);
									}
								}
							}
							goto IL_2048;
						case 3:
						{
							PartyId[] joinedPartyIds = BnetParty.GetJoinedPartyIds();
							if (array.Length < 1)
							{
								text = "party chat: must specify 1-2 arguments: party (index or LowBits or type) or a message to send.";
							}
							else
							{
								int num7 = 1;
								PartyId partyId3 = Cheats.ParsePartyId(cmd, array[0], -1, ref text);
								if (partyId3 == null && joinedPartyIds.Length > 0)
								{
									text = null;
									partyId3 = joinedPartyIds[0];
									num7 = 0;
								}
								if (partyId3 != null)
								{
									BnetParty.SendChatMessage(partyId3, string.Join(" ", Enumerable.ToArray<string>(Enumerable.Skip<string>(array, num7))));
								}
							}
							goto IL_2048;
						}
						case 4:
						{
							PartyId partyId4 = null;
							int num8 = 1;
							if (array.Length == 0)
							{
								PartyId[] joinedPartyIds2 = BnetParty.GetJoinedPartyIds();
								if (joinedPartyIds2.Length > 0)
								{
									partyId4 = joinedPartyIds2[0];
									num8 = 0;
								}
								else
								{
									text = "party invite: no joined parties to invite to.";
								}
							}
							else
							{
								partyId4 = Cheats.ParsePartyId(cmd, array[0], -1, ref text);
							}
							if (partyId4 != null)
							{
								string[] array6 = Enumerable.ToArray<string>(Enumerable.Skip<string>(array, num8));
								HashSet<BnetPlayer> hashSet = new HashSet<BnetPlayer>();
								IEnumerable<BnetPlayer> enumerable = Enumerable.Where<BnetPlayer>(BnetFriendMgr.Get().GetFriends(), (BnetPlayer p) => p.IsOnline() && p.GetHearthstoneGameAccount() != null);
								if (array6.Length == 0)
								{
									Cheats.PartyLogger.Print("NOTE: party invite without any arguments will pick the first online friend.", new object[0]);
									BnetPlayer bnetPlayer = Enumerable.FirstOrDefault<BnetPlayer>(enumerable);
									if (bnetPlayer == null)
									{
										text = "party invite: no online Hearthstone friend found.";
									}
									else
									{
										hashSet.Add(bnetPlayer);
									}
								}
								else
								{
									for (int l = 0; l < array6.Length; l++)
									{
										string arg = array6[l];
										int num9;
										if (int.TryParse(arg, ref num9))
										{
											BnetPlayer bnetPlayer2 = Enumerable.ElementAtOrDefault<BnetPlayer>(enumerable, num9);
											if (bnetPlayer2 == null)
											{
												text = "party invite: no online Hearthstone friend index " + num9;
											}
											else
											{
												hashSet.Add(bnetPlayer2);
											}
										}
										else
										{
											IEnumerable<BnetPlayer> enumerable2 = Enumerable.Where<BnetPlayer>(enumerable, (BnetPlayer p) => p.GetBattleTag().ToString().Contains(arg, 5) || (p.GetFullName() != null && p.GetFullName().Contains(arg, 5)));
											if (!Enumerable.Any<BnetPlayer>(enumerable2))
											{
												text = string.Concat(new object[]
												{
													"party invite: no online Hearthstone friend matching name ",
													arg,
													" (arg index ",
													l,
													")"
												});
											}
											else
											{
												foreach (BnetPlayer bnetPlayer3 in enumerable2)
												{
													if (!hashSet.Contains(bnetPlayer3))
													{
														hashSet.Add(bnetPlayer3);
														break;
													}
												}
											}
										}
									}
								}
								foreach (BnetPlayer bnetPlayer4 in hashSet)
								{
									BnetGameAccountId hearthstoneGameAccountId = bnetPlayer4.GetHearthstoneGameAccountId();
									if (BnetParty.IsMember(partyId4, hearthstoneGameAccountId))
									{
										Cheats.PartyLogger.Print("party invite: already a party member of {0}: {1}", new object[]
										{
											bnetPlayer4,
											BnetParty.GetJoinedParty(partyId4)
										});
									}
									else
									{
										Cheats.PartyLogger.Print("party invite: inviting {0} {1} to party {2}", new object[]
										{
											hearthstoneGameAccountId,
											bnetPlayer4,
											BnetParty.GetJoinedParty(partyId4)
										});
										BnetParty.SendInvite(partyId4, hearthstoneGameAccountId);
									}
								}
							}
							goto IL_2048;
						}
						case 5:
						{
							bool flag2 = cmd == "accept";
							PartyInvite[] receivedInvites = BnetParty.GetReceivedInvites();
							if (receivedInvites.Length == 0)
							{
								text = "party " + cmd + ": no received party invites.";
							}
							else if (array.Length == 0)
							{
								Cheats.PartyLogger.Print("NOTE: party {0} without any arguments will {0} all received invites.", new object[]
								{
									cmd
								});
								foreach (PartyInvite partyInvite in receivedInvites)
								{
									Cheats.PartyLogger.Print("party {0}: {1} inviteId={2} from {3} for party {4}.", new object[]
									{
										cmd,
										(!flag2) ? "declining" : "accepting",
										partyInvite.InviteId,
										partyInvite.InviterName,
										new PartyInfo(partyInvite.PartyId, partyInvite.PartyType)
									});
									if (flag2)
									{
										BnetParty.AcceptReceivedInvite(partyInvite.InviteId);
									}
									else
									{
										BnetParty.DeclineReceivedInvite(partyInvite.InviteId);
									}
								}
							}
							else
							{
								for (int n = 0; n < array.Length; n++)
								{
									ulong indexOrId;
									if (ulong.TryParse(array[n], ref indexOrId))
									{
										PartyInvite partyInvite2;
										if (indexOrId < (ulong)receivedInvites.LongLength)
										{
											partyInvite2 = receivedInvites[(int)(checked((IntPtr)indexOrId))];
										}
										else
										{
											partyInvite2 = Enumerable.FirstOrDefault<PartyInvite>(receivedInvites, (PartyInvite inv) => inv.InviteId == indexOrId);
											if (partyInvite2 == null)
											{
												Cheats.PartyLogger.Print("party {0}: unable to find received invite (id or index): {1}", new object[]
												{
													cmd,
													array[n]
												});
											}
										}
										if (partyInvite2 != null)
										{
											Cheats.PartyLogger.Print("party {0}: {1} inviteId={2} from {3} for party {4}.", new object[]
											{
												cmd,
												(!flag2) ? "declining" : "accepting",
												partyInvite2.InviteId,
												partyInvite2.InviterName,
												new PartyInfo(partyInvite2.PartyId, partyInvite2.PartyType)
											});
											if (flag2)
											{
												BnetParty.AcceptReceivedInvite(partyInvite2.InviteId);
											}
											else
											{
												BnetParty.DeclineReceivedInvite(partyInvite2.InviteId);
											}
										}
									}
									else
									{
										Cheats.PartyLogger.Print("party {0}: unable to parse invite (id or index): {1}", new object[]
										{
											cmd,
											array[n]
										});
									}
								}
							}
							goto IL_2048;
						}
						case 6:
						{
							PartyId partyId5 = null;
							if (array.Length == 0)
							{
								Cheats.PartyLogger.Print("NOTE: party {0} without any arguments will {0} all sent invites for all parties.", new object[]
								{
									cmd
								});
								PartyId[] joinedPartyIds3 = BnetParty.GetJoinedPartyIds();
								if (joinedPartyIds3.Length == 0)
								{
									Cheats.PartyLogger.Print("party {0}: no joined parties.", new object[]
									{
										cmd
									});
								}
								foreach (PartyId partyId6 in joinedPartyIds3)
								{
									foreach (PartyInvite partyInvite3 in BnetParty.GetSentInvites(partyId6))
									{
										Cheats.PartyLogger.Print("party {0}: revoking inviteId={1} from {2} for party {3}.", new object[]
										{
											cmd,
											partyInvite3.InviteId,
											partyInvite3.InviterName,
											BnetParty.GetJoinedParty(partyId6)
										});
										BnetParty.RevokeSentInvite(partyId6, partyInvite3.InviteId);
									}
								}
							}
							else
							{
								partyId5 = Cheats.ParsePartyId(cmd, array[0], -1, ref text);
							}
							if (partyId5 != null)
							{
								PartyInfo joinedParty = BnetParty.GetJoinedParty(partyId5);
								PartyInvite[] sentInvites2 = BnetParty.GetSentInvites(partyId5);
								if (sentInvites2.Length == 0)
								{
									text = string.Concat(new object[]
									{
										"party ",
										cmd,
										": no sent invites for party ",
										joinedParty
									});
								}
								else
								{
									string[] array9 = Enumerable.ToArray<string>(Enumerable.Skip<string>(array, 1));
									if (array9.Length == 0)
									{
										Cheats.PartyLogger.Print("NOTE: party {0} without specifying InviteId (or index) will {0} all sent invites.", new object[]
										{
											cmd
										});
										foreach (PartyInvite partyInvite4 in sentInvites2)
										{
											Cheats.PartyLogger.Print("party {0}: revoking inviteId={1} from {2} for party {3}.", new object[]
											{
												cmd,
												partyInvite4.InviteId,
												partyInvite4.InviterName,
												joinedParty
											});
											BnetParty.RevokeSentInvite(partyId5, partyInvite4.InviteId);
										}
									}
									else
									{
										for (int num13 = 0; num13 < array9.Length; num13++)
										{
											ulong indexOrId;
											if (ulong.TryParse(array9[num13], ref indexOrId))
											{
												PartyInvite partyInvite5;
												if (indexOrId < (ulong)sentInvites2.LongLength)
												{
													partyInvite5 = sentInvites2[(int)(checked((IntPtr)indexOrId))];
												}
												else
												{
													partyInvite5 = Enumerable.FirstOrDefault<PartyInvite>(sentInvites2, (PartyInvite inv) => inv.InviteId == indexOrId);
													if (partyInvite5 == null)
													{
														Cheats.PartyLogger.Print("party {0}: unable to find sent invite (id or index): {1} for party {2}", new object[]
														{
															cmd,
															array9[num13],
															joinedParty
														});
													}
												}
												if (partyInvite5 != null)
												{
													Cheats.PartyLogger.Print("party {0}: revoking inviteId={1} from {2} for party {3}.", new object[]
													{
														cmd,
														partyInvite5.InviteId,
														partyInvite5.InviterName,
														joinedParty
													});
													BnetParty.RevokeSentInvite(partyId5, partyInvite5.InviteId);
												}
											}
											else
											{
												Cheats.PartyLogger.Print("party {0}: unable to parse invite (id or index): {1}", new object[]
												{
													cmd,
													array9[num13]
												});
											}
										}
									}
								}
							}
							goto IL_2048;
						}
						case 7:
							if (array.Length < 2)
							{
								text = "party " + cmd + ": must specify a partyId (Hi-Lo format) and an online friend index";
							}
							else
							{
								PartyType partyType3 = 0;
								foreach (string text6 in array)
								{
									int num15 = text6.IndexOf('-');
									int num16 = -1;
									PartyId partyId7 = null;
									BnetGameAccountId bnetGameAccountId = null;
									if (num15 >= 0)
									{
										string text7 = text6.Substring(0, num15);
										string text8 = (text6.Length <= num15) ? string.Empty : text6.Substring(num15 + 1);
										ulong num17;
										ulong num18;
										if (ulong.TryParse(text7, ref num17) && ulong.TryParse(text8, ref num18))
										{
											partyId7 = new PartyId(num17, num18);
										}
										else
										{
											text = "party " + cmd + ": unable to parse partyId (in format Hi-Lo).";
										}
									}
									else if (int.TryParse(text6, ref num16))
									{
										BnetPlayer[] array12 = Enumerable.ToArray<BnetPlayer>(Enumerable.Where<BnetPlayer>(BnetFriendMgr.Get().GetFriends(), (BnetPlayer p) => p.IsOnline() && p.GetHearthstoneGameAccount() != null));
										if (num16 < 0 || num16 >= array12.Length)
										{
											text = string.Concat(new object[]
											{
												"party ",
												cmd,
												": no online friend at index ",
												num16
											});
										}
										else
										{
											bnetGameAccountId = array12[num16].GetHearthstoneGameAccountId();
										}
									}
									else
									{
										text = "party " + cmd + ": unable to parse online friend index.";
									}
									if (partyId7 != null && bnetGameAccountId != null)
									{
										BnetParty.RequestInvite(partyId7, bnetGameAccountId, BnetPresenceMgr.Get().GetMyGameAccountId(), partyType3);
									}
								}
							}
							goto IL_2048;
						case 8:
						{
							PartyId[] joinedPartyIds4 = BnetParty.GetJoinedPartyIds();
							if (joinedPartyIds4.Length == 0)
							{
								Cheats.PartyLogger.Print("party {0}: no joined parties.", new object[]
								{
									cmd
								});
							}
							else
							{
								foreach (PartyId partyId8 in joinedPartyIds4)
								{
									foreach (InviteRequest inviteRequest in BnetParty.GetInviteRequests(partyId8))
									{
										Cheats.PartyLogger.Print("party {0}: ignoring request to invite {0} {1} from {2} {3}.", new object[]
										{
											inviteRequest.TargetName,
											inviteRequest.TargetId,
											inviteRequest.RequesterName,
											inviteRequest.RequesterId
										});
										BnetParty.IgnoreInviteRequest(partyId8, inviteRequest.TargetId);
									}
								}
							}
							goto IL_2048;
						}
						case 9:
						{
							IEnumerable<PartyId> enumerable3 = null;
							int num21 = -1;
							if (array.Length >= 2 && (!int.TryParse(array[1], ref num21) || num21 < 0))
							{
								text = string.Format("party {0}: invalid memberIndex={1}", cmd, array[1]);
							}
							if (array.Length == 0)
							{
								Cheats.PartyLogger.Print("NOTE: party {0} without any arguments will {0} to first member in all parties.", new object[]
								{
									cmd
								});
								PartyId[] joinedPartyIds5 = BnetParty.GetJoinedPartyIds();
								if (joinedPartyIds5.Length == 0)
								{
									Cheats.PartyLogger.Print("party {0}: no joined parties.", new object[]
									{
										cmd
									});
								}
								else
								{
									enumerable3 = joinedPartyIds5;
								}
							}
							else
							{
								PartyId partyId9 = Cheats.ParsePartyId(cmd, array[0], -1, ref text);
								if (partyId9 != null)
								{
									enumerable3 = new PartyId[]
									{
										partyId9
									};
								}
							}
							if (enumerable3 != null)
							{
								foreach (PartyId partyId10 in enumerable3)
								{
									PartyMember[] members = BnetParty.GetMembers(partyId10);
									if (num21 >= 0)
									{
										if (num21 >= members.Length)
										{
											Cheats.PartyLogger.Print("party {0}: party={1} has no member at index={2}", new object[]
											{
												cmd,
												BnetParty.GetJoinedParty(partyId10),
												num21
											});
										}
										else
										{
											PartyMember partyMember = members[num21];
											BnetParty.SetLeader(partyId10, partyMember.GameAccountId);
										}
									}
									else if (Enumerable.Any<PartyMember>(members, (PartyMember m) => m.GameAccountId != BnetPresenceMgr.Get().GetMyGameAccountId()))
									{
										BnetParty.SetLeader(partyId10, Enumerable.First<PartyMember>(members, (PartyMember m) => m.GameAccountId != BnetPresenceMgr.Get().GetMyGameAccountId()).GameAccountId);
									}
									else
									{
										Cheats.PartyLogger.Print("party {0}: party={1} has no member not myself to set as leader.", new object[]
										{
											cmd,
											BnetParty.GetJoinedParty(partyId10)
										});
									}
								}
							}
							goto IL_2048;
						}
						case 10:
						{
							PartyId partyId11 = null;
							if (array.Length == 0)
							{
								Cheats.PartyLogger.Print("NOTE: party {0} without any arguments will {0} all members for all parties (other than self).", new object[]
								{
									cmd
								});
								PartyId[] joinedPartyIds6 = BnetParty.GetJoinedPartyIds();
								if (joinedPartyIds6.Length == 0)
								{
									Cheats.PartyLogger.Print("party {0}: no joined parties.", new object[]
									{
										cmd
									});
								}
								foreach (PartyId partyId12 in joinedPartyIds6)
								{
									foreach (PartyMember partyMember2 in BnetParty.GetMembers(partyId12))
									{
										if (!(partyMember2.GameAccountId == BnetPresenceMgr.Get().GetMyGameAccountId()))
										{
											Cheats.PartyLogger.Print("party {0}: kicking memberId={1} from party {2}.", new object[]
											{
												cmd,
												partyMember2.GameAccountId,
												BnetParty.GetJoinedParty(partyId12)
											});
											BnetParty.KickMember(partyId12, partyMember2.GameAccountId);
										}
									}
								}
							}
							else
							{
								partyId11 = Cheats.ParsePartyId(cmd, array[0], -1, ref text);
							}
							if (partyId11 != null)
							{
								PartyInfo joinedParty2 = BnetParty.GetJoinedParty(partyId11);
								PartyMember[] members3 = BnetParty.GetMembers(partyId11);
								if (members3.Length == 1)
								{
									text = string.Concat(new object[]
									{
										"party ",
										cmd,
										": no members (other than self) for party ",
										joinedParty2
									});
								}
								else
								{
									string[] array15 = Enumerable.ToArray<string>(Enumerable.Skip<string>(array, 1));
									if (array15.Length == 0)
									{
										Cheats.PartyLogger.Print("NOTE: party {0} without specifying member index will {0} all members (other than self).", new object[]
										{
											cmd
										});
										foreach (PartyMember partyMember3 in members3)
										{
											if (!(partyMember3.GameAccountId == BnetPresenceMgr.Get().GetMyGameAccountId()))
											{
												Cheats.PartyLogger.Print("party {0}: kicking memberId={1} from party {2}.", new object[]
												{
													cmd,
													partyMember3.GameAccountId,
													joinedParty2
												});
												BnetParty.KickMember(partyId11, partyMember3.GameAccountId);
											}
										}
									}
									else
									{
										for (int num25 = 0; num25 < array15.Length; num25++)
										{
											ulong indexOrId;
											if (ulong.TryParse(array15[num25], ref indexOrId))
											{
												PartyMember partyMember4;
												if (indexOrId < (ulong)members3.LongLength)
												{
													partyMember4 = members3[(int)(checked((IntPtr)indexOrId))];
												}
												else
												{
													partyMember4 = Enumerable.FirstOrDefault<PartyMember>(members3, (PartyMember m) => m.GameAccountId.GetLo() == indexOrId);
													if (partyMember4 == null)
													{
														Cheats.PartyLogger.Print("party {0}: unable to find member (id or index): {1} for party {2}", new object[]
														{
															cmd,
															array15[num25],
															joinedParty2
														});
													}
												}
												if (partyMember4 != null)
												{
													if (partyMember4.GameAccountId == BnetPresenceMgr.Get().GetMyGameAccountId())
													{
														Cheats.PartyLogger.Print("party {0}: cannot kick yourself (argIndex={1}); party={2}", new object[]
														{
															cmd,
															num25,
															joinedParty2
														});
													}
													else
													{
														Cheats.PartyLogger.Print("party {0}: kicking memberId={1} from party {2}.", new object[]
														{
															cmd,
															partyMember4.GameAccountId,
															joinedParty2
														});
														BnetParty.KickMember(partyId11, partyMember4.GameAccountId);
													}
												}
											}
											else
											{
												Cheats.PartyLogger.Print("party {0}: unable to parse member (id or index): {1}", new object[]
												{
													cmd,
													array15[num25]
												});
											}
										}
									}
								}
							}
							goto IL_2048;
						}
						case 11:
						{
							PartyId partyId13 = null;
							if (array.Length < 2)
							{
								text = "party setprivacy: must specify a party (index or LowBits or type) and a PrivacyLevel: " + string.Join(" | ", Enumerable.ToArray<string>(Enumerable.Select<PrivacyLevel, string>(Enumerable.Cast<PrivacyLevel>(Enum.GetValues(typeof(PrivacyLevel))), (PrivacyLevel v) => string.Concat(new object[]
								{
									v,
									" (",
									v,
									")"
								}))));
							}
							else
							{
								partyId13 = Cheats.ParsePartyId(cmd, array[0], -1, ref text);
							}
							if (partyId13 != null)
							{
								PrivacyLevel? privacyLevel = default(PrivacyLevel?);
								int num26;
								PrivacyLevel privacyLevel2;
								if (int.TryParse(array[1], ref num26))
								{
									privacyLevel = new PrivacyLevel?(num26);
								}
								else if (!EnumUtils.TryGetEnum<PrivacyLevel>(array[1], out privacyLevel2))
								{
									text = "party setprivacy: unknown PrivacyLevel specified: " + array[1];
								}
								else
								{
									privacyLevel = new PrivacyLevel?(privacyLevel2);
								}
								if (privacyLevel != null)
								{
									Cheats.PartyLogger.Print("party setprivacy: setting PrivacyLevel={0} for party {1}.", new object[]
									{
										privacyLevel.Value,
										BnetParty.GetJoinedParty(partyId13)
									});
									BnetParty.SetPrivacy(partyId13, privacyLevel.Value);
								}
							}
							goto IL_2048;
						}
						case 12:
						{
							bool flag3 = cmd == "setlong";
							bool flag4 = cmd == "setstring";
							bool flag5 = cmd == "setblob";
							int num27 = 1;
							PartyId partyId14 = null;
							if (array.Length < 2)
							{
								text = "party " + cmd + ": must specify attributeKey and a value.";
							}
							else
							{
								partyId14 = Cheats.ParsePartyId(cmd, array[0], -1, ref text);
								if (partyId14 == null)
								{
									PartyId[] joinedPartyIds7 = BnetParty.GetJoinedPartyIds();
									if (joinedPartyIds7.Length > 0)
									{
										Cheats.PartyLogger.Print("party {0}: treating first argument as attributeKey (and not PartyId) - will use PartyId at index 0", new object[]
										{
											cmd
										});
										text = null;
										partyId14 = joinedPartyIds7[0];
									}
								}
								else
								{
									Cheats.PartyLogger.Print("party {0}: treating first argument as PartyId (second argument will be attributeKey)", new object[]
									{
										cmd
									});
								}
							}
							if (partyId14 != null)
							{
								bool flag6 = false;
								string text9 = array[num27];
								string text10 = string.Join(" ", Enumerable.ToArray<string>(Enumerable.Skip<string>(array, num27 + 1)));
								if (flag3)
								{
									long num28;
									if (long.TryParse(text10, ref num28))
									{
										BnetParty.SetPartyAttributeLong(partyId14, text9, num28);
										flag6 = true;
									}
								}
								else if (flag4)
								{
									BnetParty.SetPartyAttributeString(partyId14, text9, text10);
									flag6 = true;
								}
								else if (flag5)
								{
									byte[] bytes = Encoding.UTF8.GetBytes(text10);
									BnetParty.SetPartyAttributeBlob(partyId14, text9, bytes);
									flag6 = true;
								}
								else
								{
									text = "party " + cmd + ": unhandled attribute type!";
								}
								if (flag6)
								{
									Cheats.PartyLogger.Print("party {0}: complete key={1} val={2} party={3}", new object[]
									{
										cmd,
										text9,
										text10,
										BnetParty.GetJoinedParty(partyId14)
									});
								}
							}
							goto IL_2048;
						}
						case 13:
						{
							PartyId partyId15 = null;
							if (array.Length < 2)
							{
								text = "party " + cmd + ": must specify attributeKey.";
							}
							else
							{
								partyId15 = Cheats.ParsePartyId(cmd, array[0], -1, ref text);
								if (partyId15 == null)
								{
									PartyId[] joinedPartyIds8 = BnetParty.GetJoinedPartyIds();
									if (joinedPartyIds8.Length > 0)
									{
										Cheats.PartyLogger.Print("party {0}: treating first argument as attributeKey (and not PartyId) - will use PartyId at index 0", new object[]
										{
											cmd
										});
										text = null;
										partyId15 = joinedPartyIds8[0];
									}
								}
								else
								{
									Cheats.PartyLogger.Print("party {0}: treating first argument as PartyId (second argument will be attributeKey)", new object[]
									{
										cmd
									});
								}
							}
							if (partyId15 != null)
							{
								string text11 = array[1];
								BnetParty.ClearPartyAttribute(partyId15, text11);
								Cheats.PartyLogger.Print("party {0}: cleared key={1} party={2}", new object[]
								{
									cmd,
									text11,
									BnetParty.GetJoinedParty(partyId15)
								});
							}
							goto IL_2048;
						}
						case 14:
						{
							IEnumerable<PartyId> enumerable4 = null;
							if (array.Length == 0)
							{
								PartyInfo[] joinedParties2 = BnetParty.GetJoinedParties();
								if (joinedParties2.Length == 0)
								{
									Cheats.PartyLogger.Print("party list: no joined parties.", new object[0]);
								}
								else
								{
									Cheats.PartyLogger.Print("party list: listing all joined parties and the details of the party at index 0.", new object[0]);
									enumerable4 = new PartyId[]
									{
										joinedParties2[0].Id
									};
								}
								for (int num29 = 0; num29 < joinedParties2.Length; num29++)
								{
									Cheats.PartyLogger.Print("   {0}", new object[]
									{
										Cheats.GetPartySummary(joinedParties2[num29], num29)
									});
								}
							}
							else
							{
								enumerable4 = Enumerable.Where<PartyId>(Enumerable.Select<string, PartyId>(array, delegate(string a, int i)
								{
									string text13 = null;
									PartyId result2 = Cheats.ParsePartyId(cmd, a, i, ref text13);
									if (text13 != null)
									{
										Cheats.PartyLogger.Print(text13, new object[0]);
									}
									return result2;
								}), (PartyId p) => p != null);
							}
							if (enumerable4 != null)
							{
								int num30 = -1;
								foreach (PartyId partyId16 in enumerable4)
								{
									num30++;
									PartyInfo joinedParty3 = BnetParty.GetJoinedParty(partyId16);
									Cheats.PartyLogger.Print("party {0}: {1}", new object[]
									{
										cmd,
										Cheats.GetPartySummary(BnetParty.GetJoinedParty(partyId16), num30)
									});
									PartyMember[] members4 = BnetParty.GetMembers(partyId16);
									if (members4.Length == 0)
									{
										Cheats.PartyLogger.Print("   no members.", new object[0]);
									}
									else
									{
										Cheats.PartyLogger.Print("   members:", new object[0]);
									}
									for (int num31 = 0; num31 < members4.Length; num31++)
									{
										bool flag7 = members4[num31].GameAccountId == BnetPresenceMgr.Get().GetMyGameAccountId();
										Logger partyLogger = Cheats.PartyLogger;
										string format = "      [{0}] {1} isMyself={2} isLeader={3} roleIds={4}";
										object[] array17 = new object[5];
										array17[0] = num31;
										array17[1] = members4[num31].GameAccountId;
										array17[2] = flag7;
										array17[3] = members4[num31].IsLeader(joinedParty3.Type);
										array17[4] = string.Join(",", Enumerable.ToArray<string>(Enumerable.Select<uint, string>(members4[num31].RoleIds, (uint r) => r.ToString())));
										partyLogger.Print(format, array17);
									}
									PartyInvite[] sentInvites3 = BnetParty.GetSentInvites(partyId16);
									if (sentInvites3.Length == 0)
									{
										Cheats.PartyLogger.Print("   no sent invites.", new object[0]);
									}
									else
									{
										Cheats.PartyLogger.Print("   sent invites:", new object[0]);
									}
									for (int num32 = 0; num32 < sentInvites3.Length; num32++)
									{
										PartyInvite invite = sentInvites3[num32];
										Cheats.PartyLogger.Print("      {0}", new object[]
										{
											Cheats.GetPartyInviteSummary(invite, num32)
										});
									}
									KeyValuePair<string, object>[] allPartyAttributes = BnetParty.GetAllPartyAttributes(partyId16);
									if (allPartyAttributes.Length == 0)
									{
										Cheats.PartyLogger.Print("   no party attributes.", new object[0]);
									}
									else
									{
										Cheats.PartyLogger.Print("   party attributes:", new object[0]);
									}
									foreach (KeyValuePair<string, object> keyValuePair in allPartyAttributes)
									{
										string text12 = (keyValuePair.Value != null) ? string.Format("[{0}]{1}", keyValuePair.Value.GetType().Name, keyValuePair.Value.ToString()) : "<null>";
										if (keyValuePair.Value is byte[])
										{
											byte[] array18 = (byte[])keyValuePair.Value;
											text12 = "blobLength=" + array18.Length;
											try
											{
												string @string = Encoding.UTF8.GetString(array18);
												if (@string != null)
												{
													text12 = text12 + " decodedUtf8=" + @string;
												}
											}
											catch (ArgumentException)
											{
											}
										}
										Cheats.PartyLogger.Print("      {0}={1}", new object[]
										{
											keyValuePair.Key ?? "<null>",
											text12
										});
									}
								}
							}
							PartyInvite[] receivedInvites2 = BnetParty.GetReceivedInvites();
							if (receivedInvites2.Length == 0)
							{
								Cheats.PartyLogger.Print("party list: no received party invites.", new object[0]);
							}
							else
							{
								Cheats.PartyLogger.Print("party list: received party invites:", new object[0]);
							}
							for (int num34 = 0; num34 < receivedInvites2.Length; num34++)
							{
								PartyInvite invite2 = receivedInvites2[num34];
								Cheats.PartyLogger.Print("   {0}", new object[]
								{
									Cheats.GetPartyInviteSummary(invite2, num34)
								});
							}
							goto IL_2048;
						}
						}
					}
				}
				text = "party: unknown party cmd: " + cmd;
				IL_2048:
				if (text != null)
				{
					Cheats.PartyLogger.Print(text, new object[0]);
					Error.AddWarning("Party Cheat Error", text, new object[0]);
					result = false;
				}
				return result;
			}
		}
		string message = "USAGE: party [cmd] [args]\nCommands: create | join | leave | dissolve | list | invite | accept | decline | revoke | requestinvite | ignorerequest | setleader | kick | chat | setprivacy | setlong | setstring | setblob | clearattr | subscribe | unsubscribe";
		Error.AddWarning("Party Cheat Error", message, new object[0]);
		return false;
	}

	// Token: 0x06001F51 RID: 8017 RVA: 0x00097F34 File Offset: 0x00096134
	private static string GetPartyInviteSummary(PartyInvite invite, int index)
	{
		return string.Format("{0}: inviteId={1} sender={2} recipient={3} party={4}", new object[]
		{
			(index < 0) ? string.Empty : string.Format("[{0}] ", index),
			invite.InviteId,
			invite.InviterId + " " + invite.InviterName,
			invite.InviteeId,
			new PartyInfo(invite.PartyId, invite.PartyType)
		});
	}

	// Token: 0x06001F52 RID: 8018 RVA: 0x00097FBC File Offset: 0x000961BC
	private static string GetPartySummary(PartyInfo info, int index)
	{
		PartyMember leader = BnetParty.GetLeader(info.Id);
		return string.Format("{0}{1}: members={2} invites={3} privacy={4} leader={5}", new object[]
		{
			(index < 0) ? string.Empty : string.Format("[{0}] ", index),
			info,
			BnetParty.CountMembers(info.Id) + ((!BnetParty.IsPartyFull(info.Id, true)) ? string.Empty : "(full)"),
			BnetParty.GetSentInvites(info.Id).Length,
			BnetParty.GetPrivacyLevel(info.Id),
			(leader != null) ? leader.GameAccountId.ToString() : "null"
		});
	}

	// Token: 0x06001F53 RID: 8019 RVA: 0x0009808C File Offset: 0x0009628C
	private bool OnProcessCheat_cheat(string func, string[] args, string rawArgs)
	{
		Network.SendConsoleCmdToServer(rawArgs);
		return true;
	}

	// Token: 0x06001F54 RID: 8020 RVA: 0x000980A4 File Offset: 0x000962A4
	private bool OnProcessCheat_autohand(string func, string[] args, string rawArgs)
	{
		if (args.Length == 0)
		{
			return false;
		}
		string strVal = args[0];
		bool flag;
		if (!GeneralUtils.TryParseBool(strVal, out flag))
		{
			return false;
		}
		if (InputManager.Get() == null)
		{
			return false;
		}
		string text;
		if (flag)
		{
			text = "auto hand hiding is on";
		}
		else
		{
			text = "auto hand hiding is off";
		}
		Debug.Log(text);
		UIStatus.Get().AddInfo(text);
		InputManager.Get().SetHideHandAfterPlayingCard(flag);
		return true;
	}

	// Token: 0x06001F55 RID: 8021 RVA: 0x00098114 File Offset: 0x00096314
	private bool OnProcessCheat_adventureChallengeUnlock(string func, string[] args, string rawArgs)
	{
		if (args.Length < 1)
		{
			return false;
		}
		int num;
		if (!int.TryParse(args[0].ToLowerInvariant(), ref num))
		{
			return false;
		}
		List<int> list = new List<int>();
		list.Add(num);
		AdventureMissionDisplay.Get().ShowClassChallengeUnlock(list);
		return true;
	}

	// Token: 0x06001F56 RID: 8022 RVA: 0x0009815C File Offset: 0x0009635C
	private bool OnProcessCheat_iks(string func, string[] args, string rawArgs)
	{
		if (args.Length < 1)
		{
			return false;
		}
		InnKeepersSpecial.Get().adUrlOverride = args[0];
		WelcomeQuests.Show(UserAttentionBlocker.ALL, true, null, false);
		return true;
	}

	// Token: 0x06001F57 RID: 8023 RVA: 0x0009818C File Offset: 0x0009638C
	private bool OnProcessCheat_quote(string func, string[] args, string rawArgs)
	{
		if (args.Length < 2)
		{
			return false;
		}
		string text = args[0];
		string text2 = args[1];
		string soundName = text2;
		if (args.Length > 2)
		{
			soundName = args[2];
		}
		if (text.ToLowerInvariant().Contains("innkeeper"))
		{
			NotificationManager.Get().CreateInnkeeperQuote(UserAttentionBlocker.ALL, NotificationManager.DEFAULT_CHARACTER_POS, GameStrings.Get(text2), soundName, 0f, null);
		}
		else
		{
			if (!text.EndsWith("_Quote"))
			{
				text += "_Quote";
			}
			NotificationManager.Get().CreateCharacterQuote(text, NotificationManager.DEFAULT_CHARACTER_POS, GameStrings.Get(text2), soundName, true, 0f, null, CanvasAnchor.BOTTOM_LEFT);
		}
		return true;
	}

	// Token: 0x06001F58 RID: 8024 RVA: 0x00098230 File Offset: 0x00096430
	private bool OnProcessCheat_popuptext(string func, string[] args, string rawArgs)
	{
		if (args.Length < 1)
		{
			return false;
		}
		string text = args[0];
		NotificationManager.Get().CreatePopupText(UserAttentionBlocker.ALL, Box.Get().m_LeftDoor.transform.position, TutorialEntity.HELP_POPUP_SCALE, text, true);
		return true;
	}

	// Token: 0x06001F59 RID: 8025 RVA: 0x00098274 File Offset: 0x00096474
	private bool OnProcessCheat_demotext(string func, string[] args, string rawArgs)
	{
		if (args.Length < 1)
		{
			return false;
		}
		string demoText = args[0];
		DemoMgr.Get().CreateDemoText(demoText);
		return true;
	}

	// Token: 0x06001F5A RID: 8026 RVA: 0x0009829C File Offset: 0x0009649C
	private bool OnProcessCheat_favoritehero(string func, string[] args, string rawArgs)
	{
		if (!(SceneMgr.Get().GetScene() is CollectionManagerScene))
		{
			Debug.LogWarning("OnProcessCheat_favoritehero must be used from the CollectionManagaer!");
			return false;
		}
		if (args.Length != 3)
		{
			return false;
		}
		int num;
		if (!int.TryParse(args[0].ToLowerInvariant(), ref num))
		{
			return false;
		}
		TAG_CLASS tag_CLASS;
		if (!EnumUtils.TryCast<TAG_CLASS>(num, out tag_CLASS))
		{
			return false;
		}
		string name = args[1];
		int num2;
		if (!int.TryParse(args[2].ToLowerInvariant(), ref num2))
		{
			return false;
		}
		TAG_PREMIUM premium;
		if (!EnumUtils.TryCast<TAG_PREMIUM>(num2, out premium))
		{
			return false;
		}
		NetCache.CardDefinition cardDefinition = new NetCache.CardDefinition
		{
			Name = name,
			Premium = premium
		};
		Log.Rachelle.Print("OnProcessCheat_favoritehero setting favorite hero to {0} for class {1}", new object[]
		{
			cardDefinition,
			tag_CLASS
		});
		Network.SetFavoriteHero(tag_CLASS, cardDefinition);
		return true;
	}

	// Token: 0x06001F5B RID: 8027 RVA: 0x00098374 File Offset: 0x00096574
	private bool OnProcessCheat_help(string func, string[] args, string rawArgs)
	{
		StringBuilder stringBuilder = new StringBuilder();
		string text = null;
		if (args.Length > 0 && !string.IsNullOrEmpty(args[0]))
		{
			text = args[0];
		}
		List<string> list = new List<string>();
		if (text != null)
		{
			foreach (string text2 in CheatMgr.Get().GetCheatCommands())
			{
				if (text2.Contains(text))
				{
					list.Add(text2);
				}
			}
		}
		else
		{
			foreach (string text3 in CheatMgr.Get().GetCheatCommands())
			{
				list.Add(text3);
			}
		}
		Debug.Log(string.Concat(new object[]
		{
			"found commands ",
			list,
			" ",
			list.Count
		}));
		if (list.Count == 1)
		{
			text = list[0];
		}
		if (text == null || list.Count != 1)
		{
			if (text == null)
			{
				stringBuilder.Append("All available cheat commands:\n");
			}
			else
			{
				stringBuilder.Append("Cheat commands containing: \"" + text + "\"\n");
			}
			int num = 0;
			string text4 = string.Empty;
			foreach (string text5 in list)
			{
				text4 = text4 + text5 + ", ";
				num++;
				if (num > 4)
				{
					num = 0;
					stringBuilder.Append(text4);
					text4 = string.Empty;
				}
			}
			if (!string.IsNullOrEmpty(text4))
			{
				stringBuilder.Append(text4);
			}
			UIStatus.Get().AddInfo(stringBuilder.ToString(), 10f);
		}
		else
		{
			string empty = string.Empty;
			CheatMgr.Get().cheatDesc.TryGetValue(text, out empty);
			string empty2 = string.Empty;
			CheatMgr.Get().cheatArgs.TryGetValue(text, out empty2);
			stringBuilder.Append("Usage: ");
			stringBuilder.Append(text);
			if (!string.IsNullOrEmpty(empty2))
			{
				stringBuilder.Append(" " + empty2);
			}
			if (!string.IsNullOrEmpty(empty))
			{
				stringBuilder.Append("\n(" + empty + ")");
			}
			UIStatus.Get().AddInfo(stringBuilder.ToString(), 10f);
		}
		return true;
	}

	// Token: 0x06001F5C RID: 8028 RVA: 0x00098638 File Offset: 0x00096838
	private bool OnProcessCheat_fixedrewardcomplete(string func, string[] args, string rawArgs)
	{
		Scene scene = SceneMgr.Get().GetScene();
		if (args.Length < 1 || string.IsNullOrEmpty(args[0]))
		{
			return false;
		}
		int fixedRewardMapID;
		if (!GeneralUtils.TryParseInt(args[0], out fixedRewardMapID))
		{
			return false;
		}
		if (scene is Login || scene is Hub)
		{
			return FixedRewardsMgr.Get().Cheat_ShowFixedReward(fixedRewardMapID, new FixedRewardsMgr.DelPositionNonToastReward(this.PositionLoginFixedReward), Login.REWARD_PUNCH_SCALE, Login.REWARD_SCALE);
		}
		if (scene is AdventureScene)
		{
			return FixedRewardsMgr.Get().Cheat_ShowFixedReward(fixedRewardMapID, delegate(Reward reward)
			{
				reward.transform.localPosition = AdventureScene.REWARD_LOCAL_POS;
			}, AdventureScene.REWARD_PUNCH_SCALE, AdventureScene.REWARD_SCALE);
		}
		return false;
	}

	// Token: 0x06001F5D RID: 8029 RVA: 0x00098704 File Offset: 0x00096904
	private void PositionLoginFixedReward(Reward reward)
	{
		Scene scene = SceneMgr.Get().GetScene();
		reward.transform.parent = scene.transform;
		reward.transform.localRotation = Quaternion.identity;
		reward.transform.localPosition = Login.REWARD_LOCAL_POS;
	}

	// Token: 0x06001F5E RID: 8030 RVA: 0x00098750 File Offset: 0x00096950
	private bool OnProcessCheat_example(string func, string[] args, string rawArgs)
	{
		if (args.Length < 1 || string.IsNullOrEmpty(args[0]))
		{
			return false;
		}
		string text = args[0];
		string empty = string.Empty;
		if (!CheatMgr.Get().cheatExamples.TryGetValue(text, out empty))
		{
			return false;
		}
		CheatMgr.Get().ProcessCheat(text + " " + empty);
		return true;
	}

	// Token: 0x06001F5F RID: 8031 RVA: 0x000987B0 File Offset: 0x000969B0
	private bool OnProcessCheat_tavernbrawl(string func, string[] args, string rawArgs)
	{
		string message = "USAGE: tb [cmd] [args]\nCommands: view, get, set, refresh, scenario, reset";
		if (args.Length >= 1)
		{
			if (!Enumerable.Any<string>(args, (string a) => string.IsNullOrEmpty(a)))
			{
				string text = args[0];
				string[] array = Enumerable.ToArray<string>(Enumerable.Skip<string>(args, 1));
				string text2 = null;
				string text3 = text;
				if (text3 != null)
				{
					if (Cheats.<>f__switch$map83 == null)
					{
						Dictionary<string, int> dictionary = new Dictionary<string, int>(8);
						dictionary.Add("help", 0);
						dictionary.Add("reset", 1);
						dictionary.Add("refresh", 2);
						dictionary.Add("get", 3);
						dictionary.Add("set", 3);
						dictionary.Add("view", 4);
						dictionary.Add("scen", 5);
						dictionary.Add("scenario", 5);
						Cheats.<>f__switch$map83 = dictionary;
					}
					int num;
					if (Cheats.<>f__switch$map83.TryGetValue(text3, ref num))
					{
						switch (num)
						{
						case 0:
							text2 = "usage";
							break;
						case 1:
							if (array.Length == 0)
							{
								text2 = "Please specify what to reset: seen, toserver";
							}
							else if ("toserver".Equals(array[0], 3))
							{
								if (TavernBrawlManager.Get().IsCheated)
								{
									TavernBrawlManager.Get().Cheat_ResetToServerData();
									TavernBrawlMission tavernBrawlMission = TavernBrawlManager.Get().CurrentMission();
									if (tavernBrawlMission == null)
									{
										text2 = "TB settings reset to server-specified Scenario ID <null>";
									}
									else
									{
										text2 = "TB settings reset to server-specified Scenario ID " + tavernBrawlMission.missionId;
									}
								}
								else
								{
									text2 = "TB not locally cheated. Already using server-specified data.";
								}
							}
							else if ("seen".Equals(array[0], 3))
							{
								int num2 = 0;
								if (array.Length > 1 && !int.TryParse(array[1], ref num2))
								{
									text2 = "Error parsing new seen value: " + array[1];
								}
								if (text2 == null)
								{
									TavernBrawlManager.Get().Cheat_ResetSeenStuff(num2);
									text2 = "all \"seentb*\" client-options reset to " + num2;
								}
							}
							else
							{
								text2 = "Unknown reset parameter: " + array[0];
							}
							break;
						case 2:
							TavernBrawlManager.Get().RefreshServerData();
							text2 = "TB refreshing";
							break;
						case 3:
						{
							bool flag = text == "set";
							string text4 = Enumerable.FirstOrDefault<string>(array);
							if (string.IsNullOrEmpty(text4))
							{
								text2 = string.Format("Please specify a TB variable to {0}. Variables:RefreshTime", text);
							}
							else
							{
								string text5 = null;
								string text6 = text4.ToLower();
								if (text6 != null)
								{
									if (Cheats.<>f__switch$map82 == null)
									{
										Dictionary<string, int> dictionary = new Dictionary<string, int>(1);
										dictionary.Add("refreshtime", 0);
										Cheats.<>f__switch$map82 = dictionary;
									}
									int num3;
									if (Cheats.<>f__switch$map82.TryGetValue(text6, ref num3))
									{
										if (num3 == 0)
										{
											if (flag)
											{
												text2 = "cannot set RefreshTime";
											}
											else if (TavernBrawlManager.Get().IsRefreshingTavernBrawlInfo)
											{
												text2 = "refreshing right now";
											}
											else
											{
												text5 = TavernBrawlManager.Get().ScheduledSecondsToRefresh + " secs";
											}
										}
									}
								}
								if (flag)
								{
									text2 = string.Format("tb set {0} {1} successful.", text4, (array.Length < 2) ? "null" : array[1]);
								}
								else if (string.IsNullOrEmpty(text2))
								{
									text2 = string.Format("tb variable {0}: {1}", text4, text5 ?? "null");
								}
							}
							break;
						}
						case 4:
						{
							TavernBrawlMission tavernBrawlMission2 = TavernBrawlManager.Get().CurrentMission();
							if (tavernBrawlMission2 == null)
							{
								text2 = "No active Tavern Brawl at this time.";
							}
							else
							{
								string text7 = string.Empty;
								string text8 = string.Empty;
								ScenarioDbfRecord record = GameDbf.Scenario.GetRecord(tavernBrawlMission2.missionId);
								if (record != null)
								{
									text7 = record.Name;
									text8 = record.Description;
								}
								text2 = string.Format("Active TB: [{0}] {1}\n{2}", tavernBrawlMission2.missionId, text7, text8);
							}
							break;
						}
						case 5:
							if (array.Length < 1)
							{
								text2 = "tb scenario: requires an ID parameter";
							}
							else
							{
								int scenarioId;
								if (!int.TryParse(array[0], ref scenarioId))
								{
									text2 = "tb scenario: invalid non-integer Scenario ID " + array[0];
								}
								if (text2 == null)
								{
									TavernBrawlManager.Get().Cheat_SetScenario(scenarioId);
								}
							}
							break;
						}
					}
				}
				if (text2 != null)
				{
					UIStatus.Get().AddInfo(text2, 5f);
				}
				return true;
			}
		}
		UIStatus.Get().AddInfo(message, 10f);
		return true;
	}

	// Token: 0x06001F60 RID: 8032 RVA: 0x00098C08 File Offset: 0x00096E08
	private bool OnProcessCheat_utilservercmd(string func, string[] args, string rawArgs)
	{
		if (args.Length < 1)
		{
			UIStatus.Get().AddError("Must specify a sub-command.");
			return true;
		}
		string cmd = args[0].ToLower();
		string[] cmdArgs = Enumerable.ToArray<string>(Enumerable.Skip<string>(args, 1));
		string text = (cmdArgs.Length != 0) ? cmdArgs[0].ToLower() : null;
		AlertPopup.ResponseCallback responseCallback = delegate(AlertPopup.Response response, object userData)
		{
			if (response != AlertPopup.Response.CONFIRM && response != AlertPopup.Response.OK)
			{
				return;
			}
			DebugCommandRequest debugCommandRequest = new DebugCommandRequest();
			debugCommandRequest.Command = cmd;
			debugCommandRequest.Args.AddRange(cmdArgs);
			ConnectAPI.SendDebugCommandRequest(debugCommandRequest);
		};
		bool flag = true;
		string cmd2 = cmd;
		if (cmd2 != null)
		{
			if (Cheats.<>f__switch$map87 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(6);
				dictionary.Add("help", 0);
				dictionary.Add("tb", 1);
				dictionary.Add("arena", 2);
				dictionary.Add("ranked", 3);
				dictionary.Add("deck", 4);
				dictionary.Add("banner", 5);
				Cheats.<>f__switch$map87 = dictionary;
			}
			int num;
			if (Cheats.<>f__switch$map87.TryGetValue(cmd2, ref num))
			{
				switch (num)
				{
				case 0:
					flag = false;
					break;
				case 1:
				{
					string text2 = text;
					if (text2 != null)
					{
						if (Cheats.<>f__switch$map84 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
							dictionary.Add("help", 0);
							dictionary.Add("view", 0);
							dictionary.Add("list", 0);
							dictionary.Add("reset", 1);
							Cheats.<>f__switch$map84 = dictionary;
						}
						int num2;
						if (Cheats.<>f__switch$map84.TryGetValue(text2, ref num2))
						{
							if (num2 != 0)
							{
								if (num2 == 1)
								{
									string text3 = (cmdArgs.Length >= 2) ? cmdArgs[1].ToLower() : null;
									flag = (text3 != "help");
								}
							}
							else
							{
								flag = false;
							}
						}
					}
					break;
				}
				case 2:
					flag = false;
					break;
				case 3:
				{
					flag = false;
					string text2 = text;
					if (text2 != null)
					{
						if (Cheats.<>f__switch$map85 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
							dictionary.Add("seasonroll", 0);
							dictionary.Add("seasonreset", 0);
							Cheats.<>f__switch$map85 = dictionary;
						}
						int num2;
						if (Cheats.<>f__switch$map85.TryGetValue(text2, ref num2))
						{
							if (num2 == 0)
							{
								flag = true;
							}
						}
					}
					break;
				}
				case 4:
				{
					string text2 = text;
					if (text2 != null)
					{
						if (Cheats.<>f__switch$map86 == null)
						{
							Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
							dictionary.Add("view", 0);
							dictionary.Add("test", 0);
							Cheats.<>f__switch$map86 = dictionary;
						}
						int num2;
						if (Cheats.<>f__switch$map86.TryGetValue(text2, ref num2))
						{
							if (num2 == 0)
							{
								flag = false;
							}
						}
					}
					break;
				}
				case 5:
					flag = false;
					if (string.IsNullOrEmpty(text) || text == "help")
					{
						UIStatus.Get().AddInfo("Usage: util banner <list | reset bannerId=#>\n\nClear seen banners (wooden signs at login) up to bannerId= arg. If no parameters, clears out just latest known bannerId. If bannerId=0, all seen banners are cleared.", 5f);
						return true;
					}
					if (text == "list")
					{
						StringBuilder stringBuilder = new StringBuilder();
						bool flag2 = true;
						foreach (BannerDbfRecord bannerDbfRecord in Enumerable.OrderByDescending<BannerDbfRecord, int>(GameDbf.Banner.GetRecords(), (BannerDbfRecord r) => r.ID))
						{
							if (!flag2)
							{
								stringBuilder.Append("\n");
							}
							flag2 = false;
							stringBuilder.AppendFormat("{0}. {1}", bannerDbfRecord.ID, bannerDbfRecord.NoteDesc);
						}
						UIStatus.Get().AddInfo(stringBuilder.ToString(), 5f);
						return true;
					}
					break;
				}
			}
		}
		this.m_lastUtilServerCmd = args;
		if (flag)
		{
			AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
			popupInfo.m_headerText = "Run UTIL server command?";
			popupInfo.m_text = "You are about to run a UTIL Server command - this may affect other players on this environment and possibly change configuration on this environment.\n\nPlease confirm you want to do this.";
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
			popupInfo.m_responseCallback = responseCallback;
			DialogManager.Get().ShowPopup(popupInfo);
		}
		else
		{
			responseCallback(AlertPopup.Response.OK, null);
		}
		return true;
	}

	// Token: 0x06001F61 RID: 8033 RVA: 0x00099040 File Offset: 0x00097240
	private void OnProcessCheat_utilservercmd_OnResponse()
	{
		DebugCommandResponse debugCommandResponse = ConnectAPI.GetDebugCommandResponse();
		bool flag = false;
		string text = "null response";
		string text2 = (this.m_lastUtilServerCmd != null && this.m_lastUtilServerCmd.Length != 0) ? this.m_lastUtilServerCmd[0] : string.Empty;
		string[] array = (this.m_lastUtilServerCmd != null) ? Enumerable.ToArray<string>(Enumerable.Skip<string>(this.m_lastUtilServerCmd, 1)) : new string[0];
		string text3 = (array.Length != 0) ? array[0] : null;
		string text4 = (array.Length >= 2) ? array[1].ToLower() : null;
		this.m_lastUtilServerCmd = null;
		if (debugCommandResponse != null)
		{
			flag = debugCommandResponse.Success;
			text = string.Format("{0} {1}", (!debugCommandResponse.Success) ? "FAILED:" : "Success:", (!debugCommandResponse.HasResponse) ? "reply=<blank>" : debugCommandResponse.Response);
		}
		LogLevel level = (!flag) ? LogLevel.Error : LogLevel.Info;
		Log.Net.Print(level, text, new object[0]);
		bool flag2 = true;
		float delay = 5f;
		if (flag)
		{
			if (text2 == "tb")
			{
				if (text3 == "scenario" || text3 == "scen" || text3 == "season" || text3 == "end_offset" || text3 == "start_offset" || (text3 == "reset" && text4 != "help"))
				{
					TavernBrawlManager.Get().RefreshServerData();
				}
			}
			else if (text2 == "ranked")
			{
				if (text3 == "medal")
				{
					Time.timeScale = SceneDebugger.Get().m_MaxTimeScale;
				}
				if (text3 == "medal" || text3 == "seasonroll")
				{
					flag = (flag && (!debugCommandResponse.HasResponse || !debugCommandResponse.Response.StartsWith("Error")));
					if (flag)
					{
						NetCache.Get().ReloadNetObject<NetCache.NetCacheProfileNotices>();
						text = "Success";
						delay = 0.5f;
					}
					else if (debugCommandResponse.HasResponse)
					{
						text = debugCommandResponse.Response;
					}
				}
			}
			else if (text2 == "banner" && text3 == "reset")
			{
				NetCache.Get().ReloadNetObject<NetCache.NetCacheProfileProgress>();
			}
			if ((text2 == "ranked" || text2 == "arena") && text3 == "reward")
			{
				flag = (flag && (!debugCommandResponse.HasResponse || !debugCommandResponse.Response.StartsWith("Error")));
				if (flag)
				{
					text = Cheats.Cheat_ShowRewardBoxes(text);
					if (text == null)
					{
						delay = 0.5f;
						text = "Success";
					}
					else
					{
						flag = false;
					}
				}
			}
		}
		if (flag2)
		{
			if (flag)
			{
				UIStatus.Get().AddInfo(text, delay);
			}
			else
			{
				UIStatus.Get().AddError(text);
			}
		}
	}

	// Token: 0x06001F62 RID: 8034 RVA: 0x0009938C File Offset: 0x0009758C
	private static string Cheat_ShowRewardBoxes(string parsableRewardBags)
	{
		if (SceneMgr.Get().IsInGame())
		{
			return "Cannot display reward boxes in gameplay.";
		}
		string[] array = parsableRewardBags.Trim().Split(new char[]
		{
			' '
		}, 1);
		if (array.Length < 2)
		{
			return "Error parsing reply, should start with 'Success:' then player_id: " + parsableRewardBags;
		}
		if (array.Length < 3)
		{
			return "No rewards returned by server: reply=" + parsableRewardBags;
		}
		List<NetCache.ProfileNotice> list = new List<NetCache.ProfileNotice>();
		array = Enumerable.ToArray<string>(Enumerable.Skip<string>(array, 2));
		for (int i = 0; i < array.Length; i++)
		{
			int num = 0;
			int num2 = i * 2;
			if (num2 >= array.Length)
			{
				break;
			}
			if (!int.TryParse(array[num2], ref num))
			{
				return string.Concat(new object[]
				{
					"Reward at index ",
					num2,
					" (",
					array[num2],
					") is not an int: reply=",
					parsableRewardBags
				});
			}
			if (num != 0)
			{
				num2++;
				if (num2 >= array.Length)
				{
					return string.Concat(new object[]
					{
						"No reward bag data at index ",
						num2,
						": reply=",
						parsableRewardBags
					});
				}
				long num3 = 0L;
				if (!long.TryParse(array[num2], ref num3))
				{
					return string.Concat(new object[]
					{
						"Reward Data at index ",
						num2,
						" (",
						array[num2],
						") is not a long int: reply=",
						parsableRewardBags
					});
				}
				NetCache.ProfileNotice profileNotice = null;
				TAG_PREMIUM premium = TAG_PREMIUM.NORMAL;
				switch (num)
				{
				case 1:
				case 12:
				case 14:
				case 15:
					profileNotice = new NetCache.ProfileNoticeRewardBooster
					{
						Id = (int)num3,
						Count = 1
					};
					break;
				case 2:
					profileNotice = new NetCache.ProfileNoticeRewardGold
					{
						Amount = (int)num3
					};
					break;
				case 3:
					profileNotice = new NetCache.ProfileNoticeRewardDust
					{
						Amount = (int)num3
					};
					break;
				case 4:
				case 5:
				case 6:
				case 7:
					goto IL_246;
				case 8:
				case 9:
				case 10:
				case 11:
					premium = TAG_PREMIUM.GOLDEN;
					goto IL_246;
				case 13:
					profileNotice = new NetCache.ProfileNoticeRewardCardBack
					{
						CardBackID = (int)num3
					};
					break;
				default:
					Debug.LogError(string.Concat(new object[]
					{
						"Unknown Reward Bag Type: ",
						num,
						" (data=",
						num3,
						") at index ",
						num2,
						": reply=",
						parsableRewardBags
					}));
					break;
				}
				IL_2E4:
				if (profileNotice != null)
				{
					list.Add(profileNotice);
					goto IL_2F3;
				}
				goto IL_2F3;
				IL_246:
				profileNotice = new NetCache.ProfileNoticeRewardCard
				{
					CardID = GameUtils.TranslateDbIdToCardId((int)num3),
					Premium = premium
				};
				goto IL_2E4;
			}
			IL_2F3:;
		}
		RewardBoxesDisplay rewardBoxesDisplay = Object.FindObjectOfType<RewardBoxesDisplay>();
		if (rewardBoxesDisplay != null)
		{
			float secondsToWait = 0f;
			if (rewardBoxesDisplay.IsClosing)
			{
				secondsToWait = 0.1f;
			}
			else
			{
				rewardBoxesDisplay.Close();
			}
			ApplicationMgr.Get().ScheduleCallback(secondsToWait, false, delegate(object userData)
			{
				Cheats.Cheat_ShowRewardBoxes(parsableRewardBags);
			}, null);
			return null;
		}
		List<RewardData> rewards = RewardUtils.GetRewards(list);
		AssetLoader.GameObjectCallback callback = delegate(string name, GameObject go, object callbackData)
		{
			RewardBoxesDisplay component = go.GetComponent<RewardBoxesDisplay>();
			component.SetRewards(callbackData as List<RewardData>);
			if (UniversalInputManager.UsePhoneUI)
			{
				component.m_Root.transform.position = new Vector3(0f, 14.7f, 3f);
			}
			else
			{
				component.m_Root.transform.position = new Vector3(0f, 131.2f, -3.2f);
			}
			if (Box.Get() != null && Box.Get().GetBoxCamera() != null)
			{
				BoxCamera.State state = Box.Get().GetBoxCamera().GetState();
				if (state == BoxCamera.State.OPENED)
				{
					component.m_Root.transform.position += new Vector3(-3f, 0f, 4.6f);
					if (UniversalInputManager.UsePhoneUI)
					{
						component.m_Root.transform.position += new Vector3(0f, 0f, -7f);
					}
					else
					{
						component.transform.localScale = Vector3.one * 0.6f;
					}
				}
			}
			component.AnimateRewards();
		};
		AssetLoader.Get().LoadGameObject("RewardBoxes", callback, rewards, false);
		return null;
	}

	// Token: 0x06001F63 RID: 8035 RVA: 0x0009972E File Offset: 0x0009792E
	private bool OnProcessCheat_gameservercmd(string func, string[] args, string rawArgs)
	{
		return true;
	}

	// Token: 0x06001F64 RID: 8036 RVA: 0x00099734 File Offset: 0x00097934
	private bool OnProcessCheat_rewardboxes(string func, string[] args, string rawArgs)
	{
		string text = args[0].ToLower();
		if (string.IsNullOrEmpty(text))
		{
		}
		int num = 5;
		if (args.Length > 1)
		{
			GeneralUtils.TryParseInt(args[1], out num);
		}
		BoosterDbId[] array = Enumerable.ToArray<BoosterDbId>(Enumerable.Where<BoosterDbId>(Enumerable.Cast<BoosterDbId>(Enum.GetValues(typeof(BoosterDbId))), (BoosterDbId i) => i != BoosterDbId.INVALID));
		BoosterDbId boosterDbId = array[Random.Range(0, array.Length)];
		string text2 = "Success: 123456";
		text2 = text2 + " " + 13;
		text2 = text2 + " " + Random.Range(1, 34);
		text2 = text2 + " " + 1;
		text2 = text2 + " " + (int)boosterDbId;
		text2 = text2 + " " + 3;
		text2 = text2 + " " + Random.Range(1, 31) * 5;
		text2 = text2 + " " + 2;
		text2 = text2 + " " + Random.Range(1, 31) * 5;
		text2 = text2 + " " + ((Random.Range(0, 2) != 0) ? 10 : 6);
		text2 = text2 + " " + GameUtils.TranslateCardIdToDbId("EX1_279");
		string text3 = Cheats.Cheat_ShowRewardBoxes(text2);
		if (text3 != null)
		{
			UIStatus.Get().AddError(text3);
		}
		return true;
	}

	// Token: 0x06001F65 RID: 8037 RVA: 0x000998E0 File Offset: 0x00097AE0
	private bool OnProcessCheat_rankchange(string func, string[] args, string rawArgs)
	{
		string text = args[0].ToLower();
		if (string.IsNullOrEmpty(text))
		{
		}
		AssetLoader.GameObjectCallback callback = delegate(string name, GameObject go, object callbackData)
		{
			RankChangeTwoScoop component = go.GetComponent<RankChangeTwoScoop>();
			if (UniversalInputManager.UsePhoneUI)
			{
				component.transform.localPosition = new Vector3(0f, 156.5f, 1.4f);
			}
			else
			{
				component.transform.localPosition = new Vector3(0f, 292f, -9f);
			}
			component.CheatRankUp(args);
			component.Initialize(null, null);
		};
		AssetLoader.Get().LoadGameObject("RankChangeTwoScoop", callback, null, false);
		return true;
	}

	// Token: 0x06001F66 RID: 8038 RVA: 0x0009993C File Offset: 0x00097B3C
	private bool OnProcessCheat_easyrank(string func, string[] args, string rawArgs)
	{
		string text = args[0].ToLower();
		if (string.IsNullOrEmpty(text))
		{
			text = "20";
		}
		int num = 25;
		if (!int.TryParse(text, ref num))
		{
			return false;
		}
		int num2 = 26 - num;
		int num3 = 0;
		num3 += Mathf.Min(num2, 5) * 2;
		if (num2 > 5)
		{
			num3 += Mathf.Min(num2 - 5, 5) * 3;
		}
		if (num2 > 10)
		{
			num3 += Mathf.Min(num2 - 10, 5) * 4;
		}
		if (num2 > 15)
		{
			num3 += Mathf.Min(num2 - 15, 10) * 5;
		}
		CheatMgr.Get().ProcessCheat(string.Format("util ranked set starlevel={0}", num2));
		CheatMgr.Get().ProcessCheat(string.Format("util ranked set beststarlevel={0}", num2));
		CheatMgr.Get().ProcessCheat(string.Format("util ranked set stars={0}", num3));
		return true;
	}

	// Token: 0x06001F67 RID: 8039 RVA: 0x00099A20 File Offset: 0x00097C20
	private bool OnProcessCheat_timescale(string func, string[] args, string rawArgs)
	{
		string text = args[0].ToLower();
		if (string.IsNullOrEmpty(text))
		{
			UIStatus.Get().AddInfo(string.Format("Current timeScale is: {0}", SceneDebugger.GetDevTimescale()), 3f * SceneDebugger.GetDevTimescale());
			return true;
		}
		float num = 1f;
		if (!float.TryParse(text, ref num))
		{
			return false;
		}
		SceneDebugger.SetDevTimescale(num);
		UIStatus.Get().AddInfo(string.Format("Setting timescale to: {0}", num), 3f * num);
		return true;
	}

	// Token: 0x06001F68 RID: 8040 RVA: 0x00099AAC File Offset: 0x00097CAC
	private bool OnProcessCheat_scenario(string func, string[] args, string rawArgs)
	{
		string text = args[0];
		int missionId;
		if (!GeneralUtils.TryParseInt(text, out missionId))
		{
			Error.AddWarning("scenario Cheat Error", "Error reading a scenario id from \"{0}\"", new object[]
			{
				text
			});
			return false;
		}
		GameType gameType = 1;
		CollectionDeck collectionDeck = null;
		if (args.Length > 1)
		{
			string text2 = args[1];
			int num;
			if (!GeneralUtils.TryParseInt(text2, out num))
			{
				Error.AddWarning("scenario Cheat Error", "Error reading a game type id from \"{0}\"", new object[]
				{
					text2
				});
				return false;
			}
			gameType = num;
			if (args.Length > 2)
			{
				string deckIdentStr = string.Join(" ", Enumerable.ToArray<string>(Enumerable.Skip<string>(args, 2)));
				long id;
				if (GeneralUtils.TryParseLong(deckIdentStr, out id))
				{
					collectionDeck = CollectionManager.Get().GetDeck(id);
				}
				if (collectionDeck == null)
				{
					collectionDeck = Enumerable.FirstOrDefault<KeyValuePair<long, CollectionDeck>>(Enumerable.Where<KeyValuePair<long, CollectionDeck>>(CollectionManager.Get().GetDecks(), (KeyValuePair<long, CollectionDeck> x) => x.Value.Name.Equals(deckIdentStr, 1))).Value;
				}
			}
		}
		Cheats.QuickLaunchAvailability quickLaunchAvailability = this.GetQuickLaunchAvailability();
		if (quickLaunchAvailability != Cheats.QuickLaunchAvailability.OK)
		{
			switch (quickLaunchAvailability)
			{
			case Cheats.QuickLaunchAvailability.FINDING_GAME:
				Error.AddDevWarning("scenario Cheat Error", "You are already finding a game.", new object[0]);
				break;
			case Cheats.QuickLaunchAvailability.ACTIVE_GAME:
				Error.AddDevWarning("scenario Cheat Error", "You are already in a game.", new object[0]);
				break;
			case Cheats.QuickLaunchAvailability.SCENE_TRANSITION:
				Error.AddDevWarning("scenario Cheat Error", "Can't start a game because a scene transition is active.", new object[0]);
				break;
			case Cheats.QuickLaunchAvailability.COLLECTION_NOT_READY:
				Error.AddDevWarning("scenario Cheat Error", "Can't start a game because your collection is not fully loaded.", new object[0]);
				break;
			default:
				Error.AddDevWarning("scenario Cheat Error", "Can't start a game: {0}", new object[]
				{
					quickLaunchAvailability
				});
				break;
			}
			return false;
		}
		this.LaunchQuickGame(missionId, gameType, collectionDeck);
		return true;
	}

	// Token: 0x06001F69 RID: 8041 RVA: 0x00099C6B File Offset: 0x00097E6B
	private bool OnProcessCheat_exportcards(string func, string[] args, string rawArgs)
	{
		Application.LoadLevel("ExportCards");
		return true;
	}

	// Token: 0x06001F6A RID: 8042 RVA: 0x00099C78 File Offset: 0x00097E78
	private bool OnProcessCheat_freeyourmind(string func, string[] args, string rawArgs)
	{
		this.m_isYourMindFree = true;
		return true;
	}

	// Token: 0x06001F6B RID: 8043 RVA: 0x00099C82 File Offset: 0x00097E82
	private bool OnProcessCheat_reloadgamestrings(string func, string[] args, string rawArgs)
	{
		GameStrings.ReloadAll();
		return true;
	}

	// Token: 0x06001F6C RID: 8044 RVA: 0x00099C8C File Offset: 0x00097E8C
	private bool OnProcessCheat_userattentionmanager(string func, string[] args, string rawArgs)
	{
		string text = UserAttentionManager.DumpUserAttentionBlockers("OnProcessCheat_userattentionmanager");
		UIStatus.Get().AddInfo(string.Format("Current UserAttentionBlockers: {0}", text));
		return true;
	}

	// Token: 0x06001F6D RID: 8045 RVA: 0x00099CBC File Offset: 0x00097EBC
	private bool OnProcessCheat_autoexportgamestate(string func, string[] args, string rawArgs)
	{
		if (SceneMgr.Get().GetMode() != SceneMgr.Mode.GAMEPLAY)
		{
			return false;
		}
		string text = (!string.IsNullOrEmpty(args[0])) ? args[0] : "GameStateExportFile";
		JSONClass jsonclass = new JSONClass();
		Map<int, Player> playerMap = GameState.Get().GetPlayerMap();
		foreach (KeyValuePair<int, Player> keyValuePair in playerMap)
		{
			string text2 = "Player" + keyValuePair.Key;
			jsonclass[text2]["Hero"] = this.GetCardJson(keyValuePair.Value.GetHero());
			jsonclass[text2]["HeroPower"] = this.GetCardJson(keyValuePair.Value.GetHeroPower());
			if (keyValuePair.Value.HasWeapon())
			{
				jsonclass[text2]["Weapon"] = this.GetCardJson(keyValuePair.Value.GetWeaponCard().GetEntity());
			}
			jsonclass[text2]["CardsInBattlefield"] = this.GetCardlistJson(keyValuePair.Value.GetBattlefieldZone().GetCards());
			if (keyValuePair.Value.GetSide() == Player.Side.FRIENDLY)
			{
				jsonclass[text2]["CardsInHand"] = this.GetCardlistJson(keyValuePair.Value.GetHandZone().GetCards());
				jsonclass[text2]["ActiveSecrets"] = this.GetCardlistJson(keyValuePair.Value.GetSecretZone().GetCards());
			}
		}
		string text3 = string.Format("{0}\\{1}.json", Environment.GetFolderPath(0), text);
		File.WriteAllText(text3, jsonclass.ToString(string.Empty));
		return true;
	}

	// Token: 0x04001134 RID: 4404
	private static Cheats s_instance;

	// Token: 0x04001135 RID: 4405
	private string m_board;

	// Token: 0x04001136 RID: 4406
	private bool m_loadingStoreChallengePrompt;

	// Token: 0x04001137 RID: 4407
	private StoreChallengePrompt m_storeChallengePrompt;

	// Token: 0x04001138 RID: 4408
	private bool m_isYourMindFree;

	// Token: 0x04001139 RID: 4409
	private AlertPopup m_alert;

	// Token: 0x0400113A RID: 4410
	private static readonly Map<KeyCode, ScenarioDbId> s_quickPlayKeyMap = new Map<KeyCode, ScenarioDbId>
	{
		{
			282,
			ScenarioDbId.PRACTICE_EXPERT_MAGE
		},
		{
			283,
			ScenarioDbId.PRACTICE_EXPERT_HUNTER
		},
		{
			284,
			ScenarioDbId.PRACTICE_EXPERT_WARRIOR
		},
		{
			285,
			ScenarioDbId.PRACTICE_EXPERT_SHAMAN
		},
		{
			286,
			ScenarioDbId.PRACTICE_EXPERT_DRUID
		},
		{
			287,
			ScenarioDbId.PRACTICE_EXPERT_PRIEST
		},
		{
			288,
			ScenarioDbId.PRACTICE_EXPERT_ROGUE
		},
		{
			289,
			ScenarioDbId.PRACTICE_EXPERT_PALADIN
		},
		{
			290,
			ScenarioDbId.PRACTICE_EXPERT_WARLOCK
		}
	};

	// Token: 0x0400113B RID: 4411
	private static readonly Map<KeyCode, string> s_opponentHeroKeyMap = new Map<KeyCode, string>
	{
		{
			282,
			"HERO_08"
		},
		{
			283,
			"HERO_05"
		},
		{
			284,
			"HERO_01"
		},
		{
			285,
			"HERO_02"
		},
		{
			286,
			"HERO_06"
		},
		{
			287,
			"HERO_09"
		},
		{
			288,
			"HERO_03"
		},
		{
			289,
			"HERO_04"
		},
		{
			290,
			"HERO_07"
		}
	};

	// Token: 0x0400113C RID: 4412
	private Cheats.QuickLaunchState m_quickLaunchState = new Cheats.QuickLaunchState();

	// Token: 0x0400113D RID: 4413
	private static bool s_hasSubscribedToPartyEvents = false;

	// Token: 0x0400113E RID: 4414
	private string[] m_lastUtilServerCmd;

	// Token: 0x02000931 RID: 2353
	private enum QuickLaunchAvailability
	{
		// Token: 0x04003DB2 RID: 15794
		OK,
		// Token: 0x04003DB3 RID: 15795
		FINDING_GAME,
		// Token: 0x04003DB4 RID: 15796
		ACTIVE_GAME,
		// Token: 0x04003DB5 RID: 15797
		SCENE_TRANSITION,
		// Token: 0x04003DB6 RID: 15798
		COLLECTION_NOT_READY
	}

	// Token: 0x02000932 RID: 2354
	private class QuickLaunchState
	{
		// Token: 0x04003DB7 RID: 15799
		public bool m_launching;

		// Token: 0x04003DB8 RID: 15800
		public bool m_skipMulligan;

		// Token: 0x04003DB9 RID: 15801
		public bool m_flipHeroes;

		// Token: 0x04003DBA RID: 15802
		public bool m_mirrorHeroes;

		// Token: 0x04003DBB RID: 15803
		public string m_opponentHeroCardId;
	}
}
