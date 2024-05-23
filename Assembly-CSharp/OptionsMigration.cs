using System;

// Token: 0x02000113 RID: 275
public static class OptionsMigration
{
	// Token: 0x06000CD0 RID: 3280 RVA: 0x00032FF4 File Offset: 0x000311F4
	public static bool UpgradeClientOptions()
	{
		int i = Options.Get().GetInt(Option.CLIENT_OPTIONS_VERSION);
		int startingVersion = i;
		if (!Options.Get().HasOption(Option.CLIENT_OPTIONS_VERSION))
		{
			if (!OptionsMigration.UpgradeClientOptions_V2())
			{
				return false;
			}
			i = 2;
		}
		while (i < 2)
		{
			OptionsMigration.UpgradeCallback upgradeCallback;
			if (!OptionsMigration.s_clientUpgradeCallbacks.TryGetValue(i, out upgradeCallback))
			{
				Error.AddDevFatal("OptionsMigration.UpgradeClientOptions() - Current version is {0} and there is no function to upgrade to {1}. Latest is {2}.", new object[]
				{
					i,
					i + 1,
					2
				});
				return false;
			}
			if (!upgradeCallback(startingVersion))
			{
				return false;
			}
			i++;
		}
		return true;
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x00033090 File Offset: 0x00031290
	private static bool UpgradeClientOptions_V2()
	{
		Options.Get().SetInt(Option.CLIENT_OPTIONS_VERSION, 2);
		return Options.Get().GetInt(Option.CLIENT_OPTIONS_VERSION) == 2;
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x000330B8 File Offset: 0x000312B8
	public static bool UpgradeServerOptions()
	{
		int i = Options.Get().GetInt(Option.SERVER_OPTIONS_VERSION);
		int startingVersion = i;
		if (!Options.Get().HasOption(Option.SERVER_OPTIONS_VERSION))
		{
			if (!OptionsMigration.UpgradeServerOptions_V2())
			{
				return false;
			}
			i = 2;
		}
		while (i < 3)
		{
			OptionsMigration.UpgradeCallback upgradeCallback;
			if (!OptionsMigration.s_serverUpgradeCallbacks.TryGetValue(i, out upgradeCallback))
			{
				Error.AddDevFatal("OptionsMigration.UpgradeServerOptions() - Current version is {0} and there is no function to upgrade to {1}. Latest is {2}.", new object[]
				{
					i,
					i + 1,
					3
				});
				return false;
			}
			if (!upgradeCallback(startingVersion))
			{
				return false;
			}
			i++;
		}
		return true;
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x00033154 File Offset: 0x00031354
	private static bool UpgradeServerOptions_V2()
	{
		Options.Get().SetInt(Option.SERVER_OPTIONS_VERSION, 2);
		return Options.Get().GetInt(Option.SERVER_OPTIONS_VERSION) == 2;
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x00033180 File Offset: 0x00031380
	private static bool UpgradeServerOptions_V3(int startingVersion)
	{
		if (startingVersion != 0)
		{
			bool flag = false;
			if (AchieveManager.Get() != null && AchieveManager.Get().IsReady())
			{
				flag = AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.VANILLA_HEROES);
			}
			else if (Options.Get().GetBool(Option.HAS_SEEN_EXPERT_AI_UNLOCK))
			{
				flag = true;
			}
			if (flag)
			{
				Options.Get().SetBool(Option.HAS_SEEN_UNLOCK_ALL_HEROES_TRANSITION, true);
				Options.Get().SetBool(Option.HAS_CLICKED_COLLECTION_BUTTON_FOR_NEW_CARD, true);
				Options.Get().SetBool(Option.HAS_CLICKED_COLLECTION_BUTTON_FOR_NEW_DECK, true);
			}
			if (Options.Get().GetBool(Option.HAS_STARTED_A_DECK))
			{
				Options.Get().SetBool(Option.HAS_REMOVED_CARD_FROM_DECK, true);
				Options.Get().SetBool(Option.HAS_SEEN_DELETE_DECK_REMINDER, true);
			}
		}
		Options.Get().SetInt(Option.SERVER_OPTIONS_VERSION, 3);
		return Options.Get().GetInt(Option.SERVER_OPTIONS_VERSION) == 3;
	}

	// Token: 0x040006FF RID: 1791
	public const int LATEST_CLIENT_VERSION = 2;

	// Token: 0x04000700 RID: 1792
	public const int LATEST_SERVER_VERSION = 3;

	// Token: 0x04000701 RID: 1793
	private static readonly Map<int, OptionsMigration.UpgradeCallback> s_clientUpgradeCallbacks = new Map<int, OptionsMigration.UpgradeCallback>();

	// Token: 0x04000702 RID: 1794
	private static readonly Map<int, OptionsMigration.UpgradeCallback> s_serverUpgradeCallbacks = new Map<int, OptionsMigration.UpgradeCallback>
	{
		{
			2,
			new OptionsMigration.UpgradeCallback(OptionsMigration.UpgradeServerOptions_V3)
		}
	};

	// Token: 0x02000A7E RID: 2686
	// (Invoke) Token: 0x06005D84 RID: 23940
	private delegate bool UpgradeCallback(int startingVersion);
}
