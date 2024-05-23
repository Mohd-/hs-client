using System;
using System.Collections.Generic;

// Token: 0x02000016 RID: 22
public class GameDbf
{
	// Token: 0x06000265 RID: 613 RVA: 0x0000BC6A File Offset: 0x00009E6A
	public static GameDbfIndex GetIndex()
	{
		if (GameDbf.s_index == null)
		{
			GameDbf.s_index = new GameDbfIndex();
		}
		return GameDbf.s_index;
	}

	// Token: 0x06000266 RID: 614 RVA: 0x0000BC88 File Offset: 0x00009E88
	public static void Load()
	{
		if (GameDbf.s_index == null)
		{
			GameDbf.s_index = new GameDbfIndex();
		}
		else
		{
			GameDbf.s_index.Initialize();
		}
		GameDbf.Achieve = Dbf<AchieveDbfRecord>.Load("ACHIEVE");
		GameDbf.Adventure = Dbf<AdventureDbfRecord>.Load("ADVENTURE");
		GameDbf.AdventureData = Dbf<AdventureDataDbfRecord>.Load("ADVENTURE_DATA");
		GameDbf.AdventureMission = Dbf<AdventureMissionDbfRecord>.Load("ADVENTURE_MISSION");
		GameDbf.AdventureMode = Dbf<AdventureModeDbfRecord>.Load("ADVENTURE_MODE");
		GameDbf.Banner = Dbf<BannerDbfRecord>.Load("BANNER");
		GameDbf.Booster = Dbf<BoosterDbfRecord>.Load("BOOSTER");
		GameDbf.Board = Dbf<BoardDbfRecord>.Load("BOARD");
		GameDbf.Card = Dbf<CardDbfRecord>.Load("CARD", new Dbf<CardDbfRecord>.RecordAddedListener(GameDbf.s_index.OnCardAdded), new Dbf<CardDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnCardRemoved));
		GameDbf.CardBack = Dbf<CardBackDbfRecord>.Load("CARD_BACK");
		GameDbf.Deck = Dbf<DeckDbfRecord>.Load("DECK");
		GameDbf.DeckRuleset = Dbf<DeckRulesetDbfRecord>.Load("DECK_RULESET");
		GameDbf.DeckRulesetRule = Dbf<DeckRulesetRuleDbfRecord>.Load("DECK_RULESET_RULE", new Dbf<DeckRulesetRuleDbfRecord>.RecordAddedListener(GameDbf.s_index.OnDeckRulesetRuleAdded), new Dbf<DeckRulesetRuleDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnDeckRulesetRuleRemoved));
		GameDbf.DeckRulesetRuleSubset = Dbf<DeckRulesetRuleSubsetDbfRecord>.Load("DECK_RULESET_RULE_SUBSET", new Dbf<DeckRulesetRuleSubsetDbfRecord>.RecordAddedListener(GameDbf.s_index.OnDeckRulesetRuleSubsetAdded), new Dbf<DeckRulesetRuleSubsetDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnDeckRulesetRuleSubsetRemoved));
		GameDbf.DeckCard = Dbf<DeckCardDbfRecord>.Load("DECK_CARD");
		GameDbf.DeckTemplate = Dbf<DeckTemplateDbfRecord>.Load("DECK_TEMPLATE");
		GameDbf.FixedReward = Dbf<FixedRewardDbfRecord>.Load("FIXED_REWARD");
		GameDbf.FixedRewardAction = Dbf<FixedRewardActionDbfRecord>.Load("FIXED_REWARD_ACTION", new Dbf<FixedRewardActionDbfRecord>.RecordAddedListener(GameDbf.s_index.OnFixedRewardActionAdded), new Dbf<FixedRewardActionDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnFixedRewardActionRemoved));
		GameDbf.FixedRewardMap = Dbf<FixedRewardMapDbfRecord>.Load("FIXED_REWARD_MAP", new Dbf<FixedRewardMapDbfRecord>.RecordAddedListener(GameDbf.s_index.OnFixedRewardMapAdded), new Dbf<FixedRewardMapDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnFixedRewardMapRemoved));
		GameDbf.Hero = Dbf<HeroDbfRecord>.Load("HERO");
		GameDbf.RotatedItem = Dbf<RotatedItemDbfRecord>.Load("ROTATED_ITEM");
		GameDbf.Scenario = Dbf<ScenarioDbfRecord>.Load("SCENARIO");
		GameDbf.Season = Dbf<SeasonDbfRecord>.Load("SEASON");
		GameDbf.Subset = Dbf<SubsetDbfRecord>.Load("SUBSET");
		GameDbf.SubsetCard = Dbf<SubsetCardDbfRecord>.Load("SUBSET_CARD", new Dbf<SubsetCardDbfRecord>.RecordAddedListener(GameDbf.s_index.OnSubsetCardAdded), new Dbf<SubsetCardDbfRecord>.RecordsRemovedListener(GameDbf.s_index.OnSubsetCardRemoved));
		GameDbf.Wing = Dbf<WingDbfRecord>.Load("WING");
	}

	// Token: 0x06000267 RID: 615 RVA: 0x0000BF00 File Offset: 0x0000A100
	public static void Reload(string name, string xml)
	{
		if (name != null)
		{
			if (GameDbf.<>f__switch$map89 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("ACHIEVE", 0);
				dictionary.Add("CARD_BACK", 1);
				GameDbf.<>f__switch$map89 = dictionary;
			}
			int num;
			if (GameDbf.<>f__switch$map89.TryGetValue(name, ref num))
			{
				if (num == 0)
				{
					GameDbf.Achieve = Dbf<AchieveDbfRecord>.Load(name, xml);
					if (AchieveManager.Get() != null)
					{
						AchieveManager.Get().InitAchieveManager();
					}
					return;
				}
				if (num == 1)
				{
					GameDbf.CardBack = Dbf<CardBackDbfRecord>.Load(name, xml);
					if (CardBackManager.Get() != null)
					{
						CardBackManager.Get().InitCardBackData();
					}
					return;
				}
			}
		}
		Error.AddDevFatal("Reloading {0} is unsupported", new object[]
		{
			name
		});
	}

	// Token: 0x040000A0 RID: 160
	public static Dbf<AchieveDbfRecord> Achieve;

	// Token: 0x040000A1 RID: 161
	public static Dbf<AdventureDbfRecord> Adventure;

	// Token: 0x040000A2 RID: 162
	public static Dbf<AdventureDataDbfRecord> AdventureData;

	// Token: 0x040000A3 RID: 163
	public static Dbf<AdventureMissionDbfRecord> AdventureMission;

	// Token: 0x040000A4 RID: 164
	public static Dbf<AdventureModeDbfRecord> AdventureMode;

	// Token: 0x040000A5 RID: 165
	public static Dbf<BannerDbfRecord> Banner;

	// Token: 0x040000A6 RID: 166
	public static Dbf<BoardDbfRecord> Board;

	// Token: 0x040000A7 RID: 167
	public static Dbf<BoosterDbfRecord> Booster;

	// Token: 0x040000A8 RID: 168
	public static Dbf<CardDbfRecord> Card;

	// Token: 0x040000A9 RID: 169
	public static Dbf<CardBackDbfRecord> CardBack;

	// Token: 0x040000AA RID: 170
	public static Dbf<DeckDbfRecord> Deck;

	// Token: 0x040000AB RID: 171
	public static Dbf<DeckRulesetDbfRecord> DeckRuleset;

	// Token: 0x040000AC RID: 172
	public static Dbf<DeckRulesetRuleDbfRecord> DeckRulesetRule;

	// Token: 0x040000AD RID: 173
	public static Dbf<DeckRulesetRuleSubsetDbfRecord> DeckRulesetRuleSubset;

	// Token: 0x040000AE RID: 174
	public static Dbf<DeckCardDbfRecord> DeckCard;

	// Token: 0x040000AF RID: 175
	public static Dbf<DeckTemplateDbfRecord> DeckTemplate;

	// Token: 0x040000B0 RID: 176
	public static Dbf<FixedRewardDbfRecord> FixedReward;

	// Token: 0x040000B1 RID: 177
	public static Dbf<FixedRewardActionDbfRecord> FixedRewardAction;

	// Token: 0x040000B2 RID: 178
	public static Dbf<FixedRewardMapDbfRecord> FixedRewardMap;

	// Token: 0x040000B3 RID: 179
	public static Dbf<HeroDbfRecord> Hero;

	// Token: 0x040000B4 RID: 180
	public static Dbf<RotatedItemDbfRecord> RotatedItem;

	// Token: 0x040000B5 RID: 181
	public static Dbf<ScenarioDbfRecord> Scenario;

	// Token: 0x040000B6 RID: 182
	public static Dbf<SeasonDbfRecord> Season;

	// Token: 0x040000B7 RID: 183
	public static Dbf<SubsetDbfRecord> Subset;

	// Token: 0x040000B8 RID: 184
	public static Dbf<SubsetCardDbfRecord> SubsetCard;

	// Token: 0x040000B9 RID: 185
	public static Dbf<WingDbfRecord> Wing;

	// Token: 0x040000BA RID: 186
	private static GameDbfIndex s_index;
}
