using System;
using System.Collections.Generic;

// Token: 0x0200001A RID: 26
public class ScenarioDbfRecord : DbfRecord
{
	// Token: 0x17000023 RID: 35
	// (get) Token: 0x06000270 RID: 624 RVA: 0x0000C039 File Offset: 0x0000A239
	[DbfField("NOTE_DESC", "designer note")]
	public string NoteDesc
	{
		get
		{
			return this.m_NoteDesc;
		}
	}

	// Token: 0x17000024 RID: 36
	// (get) Token: 0x06000271 RID: 625 RVA: 0x0000C041 File Offset: 0x0000A241
	[DbfField("PLAYERS", "number of players we need to start this template")]
	public int Players
	{
		get
		{
			return this.m_Players;
		}
	}

	// Token: 0x17000025 RID: 37
	// (get) Token: 0x06000272 RID: 626 RVA: 0x0000C049 File Offset: 0x0000A249
	[DbfField("PLAYER1_HERO_CARD_ID", "if not null then the specific ASSET.CARD.ID to be used")]
	public int Player1HeroCardId
	{
		get
		{
			return this.m_Player1HeroCardId;
		}
	}

	// Token: 0x17000026 RID: 38
	// (get) Token: 0x06000273 RID: 627 RVA: 0x0000C051 File Offset: 0x0000A251
	[DbfField("PLAYER2_HERO_CARD_ID", "if not null then the specific ASSET.CARD.ID to be used")]
	public int Player2HeroCardId
	{
		get
		{
			return this.m_Player2HeroCardId;
		}
	}

	// Token: 0x17000027 RID: 39
	// (get) Token: 0x06000274 RID: 628 RVA: 0x0000C059 File Offset: 0x0000A259
	[DbfField("IS_TUTORIAL", "")]
	public bool IsTutorial
	{
		get
		{
			return this.m_IsTutorial;
		}
	}

	// Token: 0x17000028 RID: 40
	// (get) Token: 0x06000275 RID: 629 RVA: 0x0000C061 File Offset: 0x0000A261
	[DbfField("IS_EXPERT", "expert or normal AI?")]
	public bool IsExpert
	{
		get
		{
			return this.m_IsExpert;
		}
	}

	// Token: 0x17000029 RID: 41
	// (get) Token: 0x06000276 RID: 630 RVA: 0x0000C069 File Offset: 0x0000A269
	[DbfField("IS_COOP", "flags this scenario as a cooperative scenario, meaning players share the same win/loss conditions")]
	public bool IsCoop
	{
		get
		{
			return this.m_IsCoop;
		}
	}

	// Token: 0x1700002A RID: 42
	// (get) Token: 0x06000277 RID: 631 RVA: 0x0000C071 File Offset: 0x0000A271
	[DbfField("ADVENTURE_ID", "ASSET.ADVENTURE.ID")]
	public int AdventureId
	{
		get
		{
			return this.m_AdventureId;
		}
	}

	// Token: 0x1700002B RID: 43
	// (get) Token: 0x06000278 RID: 632 RVA: 0x0000C079 File Offset: 0x0000A279
	[DbfField("WING_ID", "ASSET.WING.ID")]
	public int WingId
	{
		get
		{
			return this.m_WingId;
		}
	}

	// Token: 0x1700002C RID: 44
	// (get) Token: 0x06000279 RID: 633 RVA: 0x0000C081 File Offset: 0x0000A281
	[DbfField("SORT_ORDER", "sort order of this scenario in its wing (or adventure)")]
	public int SortOrder
	{
		get
		{
			return this.m_SortOrder;
		}
	}

	// Token: 0x1700002D RID: 45
	// (get) Token: 0x0600027A RID: 634 RVA: 0x0000C089 File Offset: 0x0000A289
	[DbfField("MODE_ID", "ASSET.ADVENTURE_MODE.ID")]
	public int ModeId
	{
		get
		{
			return this.m_ModeId;
		}
	}

	// Token: 0x1700002E RID: 46
	// (get) Token: 0x0600027B RID: 635 RVA: 0x0000C091 File Offset: 0x0000A291
	[DbfField("CLIENT_PLAYER2_HERO_CARD_ID", "client-only: if not 0 then overrides PLAYER2_HERO_CARD_ID column for what the client displays as the boss hero")]
	public int ClientPlayer2HeroCardId
	{
		get
		{
			return this.m_ClientPlayer2HeroCardId;
		}
	}

	// Token: 0x1700002F RID: 47
	// (get) Token: 0x0600027C RID: 636 RVA: 0x0000C099 File Offset: 0x0000A299
	[DbfField("NAME", "FK to LOC_STR.ID for the user-facing name of this scenario")]
	public DbfLocValue Name
	{
		get
		{
			return this.m_Name;
		}
	}

	// Token: 0x17000030 RID: 48
	// (get) Token: 0x0600027D RID: 637 RVA: 0x0000C0A1 File Offset: 0x0000A2A1
	[DbfField("SHORT_NAME", "FK to LOC_STR.ID for the user-facing name of this scenario in places that the name won't fit.")]
	public DbfLocValue ShortName
	{
		get
		{
			return this.m_ShortName;
		}
	}

	// Token: 0x17000031 RID: 49
	// (get) Token: 0x0600027E RID: 638 RVA: 0x0000C0A9 File Offset: 0x0000A2A9
	[DbfField("DESCRIPTION", "FK to LOC_STR.ID for the user-facing description of this scenario.")]
	public DbfLocValue Description
	{
		get
		{
			return this.m_Description;
		}
	}

	// Token: 0x17000032 RID: 50
	// (get) Token: 0x0600027F RID: 639 RVA: 0x0000C0B1 File Offset: 0x0000A2B1
	[DbfField("OPPONENT_NAME", "FK to LOC_STR.ID for the user-facing name of the player's opponent.")]
	public DbfLocValue OpponentName
	{
		get
		{
			return this.m_OpponentName;
		}
	}

	// Token: 0x17000033 RID: 51
	// (get) Token: 0x06000280 RID: 640 RVA: 0x0000C0B9 File Offset: 0x0000A2B9
	[DbfField("COMPLETED_DESCRIPTION", "FK to LOC_STR.ID for the user-facing mesage displayed after beating this scenario.")]
	public DbfLocValue CompletedDescription
	{
		get
		{
			return this.m_CompletedDescription;
		}
	}

	// Token: 0x17000034 RID: 52
	// (get) Token: 0x06000281 RID: 641 RVA: 0x0000C0C1 File Offset: 0x0000A2C1
	[DbfField("PLAYER1_DECK_ID", "")]
	public int Player1DeckId
	{
		get
		{
			return this.m_Player1DeckId;
		}
	}

	// Token: 0x17000035 RID: 53
	// (get) Token: 0x06000282 RID: 642 RVA: 0x0000C0C9 File Offset: 0x0000A2C9
	[DbfField("DECK_BUILDER_ID", "deprecated -- use deck_ruleset_id.")]
	public int DeckBuilderId
	{
		get
		{
			return this.m_DeckBuilderId;
		}
	}

	// Token: 0x17000036 RID: 54
	// (get) Token: 0x06000283 RID: 643 RVA: 0x0000C0D1 File Offset: 0x0000A2D1
	[DbfField("DECK_RULESET_ID", "the DECK_RULESET.ID used to create/validate decks for this scenario.")]
	public int DeckRulesetId
	{
		get
		{
			return this.m_DeckRulesetId;
		}
	}

	// Token: 0x17000037 RID: 55
	// (get) Token: 0x06000284 RID: 644 RVA: 0x0000C0D9 File Offset: 0x0000A2D9
	[DbfField("TB_TEXTURE", "chalkboard texture that shows in the main TB screen (non-phone).")]
	public string TbTexture
	{
		get
		{
			return this.m_TbTexture;
		}
	}

	// Token: 0x17000038 RID: 56
	// (get) Token: 0x06000285 RID: 645 RVA: 0x0000C0E1 File Offset: 0x0000A2E1
	[DbfField("TB_TEXTURE_PHONE", "chalkboard texture that shows in the main TB screen (phones only).")]
	public string TbTexturePhone
	{
		get
		{
			return this.m_TbTexturePhone;
		}
	}

	// Token: 0x17000039 RID: 57
	// (get) Token: 0x06000286 RID: 646 RVA: 0x0000C0E9 File Offset: 0x0000A2E9
	[DbfField("TB_TEXTURE_PHONE_OFFSET_Y", "offset into the TB_TEXTURE_PHONE - used by client.")]
	public float TbTexturePhoneOffsetY
	{
		get
		{
			return this.m_TbTexturePhoneOffsetY;
		}
	}

	// Token: 0x06000287 RID: 647 RVA: 0x0000C0F1 File Offset: 0x0000A2F1
	public void SetNoteDesc(string v)
	{
		this.m_NoteDesc = v;
	}

	// Token: 0x06000288 RID: 648 RVA: 0x0000C0FA File Offset: 0x0000A2FA
	public void SetPlayers(int v)
	{
		this.m_Players = v;
	}

	// Token: 0x06000289 RID: 649 RVA: 0x0000C103 File Offset: 0x0000A303
	public void SetPlayer1HeroCardId(int v)
	{
		this.m_Player1HeroCardId = v;
	}

	// Token: 0x0600028A RID: 650 RVA: 0x0000C10C File Offset: 0x0000A30C
	public void SetPlayer2HeroCardId(int v)
	{
		this.m_Player2HeroCardId = v;
	}

	// Token: 0x0600028B RID: 651 RVA: 0x0000C115 File Offset: 0x0000A315
	public void SetIsTutorial(bool v)
	{
		this.m_IsTutorial = v;
	}

	// Token: 0x0600028C RID: 652 RVA: 0x0000C11E File Offset: 0x0000A31E
	public void SetIsExpert(bool v)
	{
		this.m_IsExpert = v;
	}

	// Token: 0x0600028D RID: 653 RVA: 0x0000C127 File Offset: 0x0000A327
	public void SetIsCoop(bool v)
	{
		this.m_IsCoop = v;
	}

	// Token: 0x0600028E RID: 654 RVA: 0x0000C130 File Offset: 0x0000A330
	public void SetAdventureId(int v)
	{
		this.m_AdventureId = v;
	}

	// Token: 0x0600028F RID: 655 RVA: 0x0000C139 File Offset: 0x0000A339
	public void SetWingId(int v)
	{
		this.m_WingId = v;
	}

	// Token: 0x06000290 RID: 656 RVA: 0x0000C142 File Offset: 0x0000A342
	public void SetSortOrder(int v)
	{
		this.m_SortOrder = v;
	}

	// Token: 0x06000291 RID: 657 RVA: 0x0000C14B File Offset: 0x0000A34B
	public void SetModeId(int v)
	{
		this.m_ModeId = v;
	}

	// Token: 0x06000292 RID: 658 RVA: 0x0000C154 File Offset: 0x0000A354
	public void SetClientPlayer2HeroCardId(int v)
	{
		this.m_ClientPlayer2HeroCardId = v;
	}

	// Token: 0x06000293 RID: 659 RVA: 0x0000C15D File Offset: 0x0000A35D
	public void SetName(DbfLocValue v)
	{
		this.m_Name = v;
		v.SetDebugInfo(base.ID, "NAME");
	}

	// Token: 0x06000294 RID: 660 RVA: 0x0000C177 File Offset: 0x0000A377
	public void SetShortName(DbfLocValue v)
	{
		this.m_ShortName = v;
		v.SetDebugInfo(base.ID, "SHORT_NAME");
	}

	// Token: 0x06000295 RID: 661 RVA: 0x0000C191 File Offset: 0x0000A391
	public void SetDescription(DbfLocValue v)
	{
		this.m_Description = v;
		v.SetDebugInfo(base.ID, "DESCRIPTION");
	}

	// Token: 0x06000296 RID: 662 RVA: 0x0000C1AB File Offset: 0x0000A3AB
	public void SetOpponentName(DbfLocValue v)
	{
		this.m_OpponentName = v;
		v.SetDebugInfo(base.ID, "OPPONENT_NAME");
	}

	// Token: 0x06000297 RID: 663 RVA: 0x0000C1C5 File Offset: 0x0000A3C5
	public void SetCompletedDescription(DbfLocValue v)
	{
		this.m_CompletedDescription = v;
		v.SetDebugInfo(base.ID, "COMPLETED_DESCRIPTION");
	}

	// Token: 0x06000298 RID: 664 RVA: 0x0000C1DF File Offset: 0x0000A3DF
	public void SetPlayer1DeckId(int v)
	{
		this.m_Player1DeckId = v;
	}

	// Token: 0x06000299 RID: 665 RVA: 0x0000C1E8 File Offset: 0x0000A3E8
	public void SetDeckBuilderId(int v)
	{
		this.m_DeckBuilderId = v;
	}

	// Token: 0x0600029A RID: 666 RVA: 0x0000C1F1 File Offset: 0x0000A3F1
	public void SetDeckRulesetId(int v)
	{
		this.m_DeckRulesetId = v;
	}

	// Token: 0x0600029B RID: 667 RVA: 0x0000C1FA File Offset: 0x0000A3FA
	public void SetTbTexture(string v)
	{
		this.m_TbTexture = v;
	}

	// Token: 0x0600029C RID: 668 RVA: 0x0000C203 File Offset: 0x0000A403
	public void SetTbTexturePhone(string v)
	{
		this.m_TbTexturePhone = v;
	}

	// Token: 0x0600029D RID: 669 RVA: 0x0000C20C File Offset: 0x0000A40C
	public void SetTbTexturePhoneOffsetY(float v)
	{
		this.m_TbTexturePhoneOffsetY = v;
	}

	// Token: 0x0600029E RID: 670 RVA: 0x0000C218 File Offset: 0x0000A418
	public override object GetVar(string name)
	{
		if (name != null)
		{
			if (ScenarioDbfRecord.<>f__switch$map40 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(24);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("PLAYERS", 2);
				dictionary.Add("PLAYER1_HERO_CARD_ID", 3);
				dictionary.Add("PLAYER2_HERO_CARD_ID", 4);
				dictionary.Add("IS_TUTORIAL", 5);
				dictionary.Add("IS_EXPERT", 6);
				dictionary.Add("IS_COOP", 7);
				dictionary.Add("ADVENTURE_ID", 8);
				dictionary.Add("WING_ID", 9);
				dictionary.Add("SORT_ORDER", 10);
				dictionary.Add("MODE_ID", 11);
				dictionary.Add("CLIENT_PLAYER2_HERO_CARD_ID", 12);
				dictionary.Add("NAME", 13);
				dictionary.Add("SHORT_NAME", 14);
				dictionary.Add("DESCRIPTION", 15);
				dictionary.Add("OPPONENT_NAME", 16);
				dictionary.Add("COMPLETED_DESCRIPTION", 17);
				dictionary.Add("PLAYER1_DECK_ID", 18);
				dictionary.Add("DECK_BUILDER_ID", 19);
				dictionary.Add("DECK_RULESET_ID", 20);
				dictionary.Add("TB_TEXTURE", 21);
				dictionary.Add("TB_TEXTURE_PHONE", 22);
				dictionary.Add("TB_TEXTURE_PHONE_OFFSET_Y", 23);
				ScenarioDbfRecord.<>f__switch$map40 = dictionary;
			}
			int num;
			if (ScenarioDbfRecord.<>f__switch$map40.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return base.ID;
				case 1:
					return this.NoteDesc;
				case 2:
					return this.Players;
				case 3:
					return this.Player1HeroCardId;
				case 4:
					return this.Player2HeroCardId;
				case 5:
					return this.IsTutorial;
				case 6:
					return this.IsExpert;
				case 7:
					return this.IsCoop;
				case 8:
					return this.AdventureId;
				case 9:
					return this.WingId;
				case 10:
					return this.SortOrder;
				case 11:
					return this.ModeId;
				case 12:
					return this.ClientPlayer2HeroCardId;
				case 13:
					return this.Name;
				case 14:
					return this.ShortName;
				case 15:
					return this.Description;
				case 16:
					return this.OpponentName;
				case 17:
					return this.CompletedDescription;
				case 18:
					return this.Player1DeckId;
				case 19:
					return this.DeckBuilderId;
				case 20:
					return this.DeckRulesetId;
				case 21:
					return this.TbTexture;
				case 22:
					return this.TbTexturePhone;
				case 23:
					return this.TbTexturePhoneOffsetY;
				}
			}
		}
		return null;
	}

	// Token: 0x0600029F RID: 671 RVA: 0x0000C4EC File Offset: 0x0000A6EC
	public override void SetVar(string name, object val)
	{
		if (name != null)
		{
			if (ScenarioDbfRecord.<>f__switch$map41 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(24);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("PLAYERS", 2);
				dictionary.Add("PLAYER1_HERO_CARD_ID", 3);
				dictionary.Add("PLAYER2_HERO_CARD_ID", 4);
				dictionary.Add("IS_TUTORIAL", 5);
				dictionary.Add("IS_EXPERT", 6);
				dictionary.Add("IS_COOP", 7);
				dictionary.Add("ADVENTURE_ID", 8);
				dictionary.Add("WING_ID", 9);
				dictionary.Add("SORT_ORDER", 10);
				dictionary.Add("MODE_ID", 11);
				dictionary.Add("CLIENT_PLAYER2_HERO_CARD_ID", 12);
				dictionary.Add("NAME", 13);
				dictionary.Add("SHORT_NAME", 14);
				dictionary.Add("DESCRIPTION", 15);
				dictionary.Add("OPPONENT_NAME", 16);
				dictionary.Add("COMPLETED_DESCRIPTION", 17);
				dictionary.Add("PLAYER1_DECK_ID", 18);
				dictionary.Add("DECK_BUILDER_ID", 19);
				dictionary.Add("DECK_RULESET_ID", 20);
				dictionary.Add("TB_TEXTURE", 21);
				dictionary.Add("TB_TEXTURE_PHONE", 22);
				dictionary.Add("TB_TEXTURE_PHONE_OFFSET_Y", 23);
				ScenarioDbfRecord.<>f__switch$map41 = dictionary;
			}
			int num;
			if (ScenarioDbfRecord.<>f__switch$map41.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					base.SetID((int)val);
					break;
				case 1:
					this.SetNoteDesc((string)val);
					break;
				case 2:
					this.SetPlayers((int)val);
					break;
				case 3:
					this.SetPlayer1HeroCardId((int)val);
					break;
				case 4:
					this.SetPlayer2HeroCardId((int)val);
					break;
				case 5:
					this.SetIsTutorial((bool)val);
					break;
				case 6:
					this.SetIsExpert((bool)val);
					break;
				case 7:
					this.SetIsCoop((bool)val);
					break;
				case 8:
					this.SetAdventureId((int)val);
					break;
				case 9:
					this.SetWingId((int)val);
					break;
				case 10:
					this.SetSortOrder((int)val);
					break;
				case 11:
					this.SetModeId((int)val);
					break;
				case 12:
					this.SetClientPlayer2HeroCardId((int)val);
					break;
				case 13:
					this.SetName((DbfLocValue)val);
					break;
				case 14:
					this.SetShortName((DbfLocValue)val);
					break;
				case 15:
					this.SetDescription((DbfLocValue)val);
					break;
				case 16:
					this.SetOpponentName((DbfLocValue)val);
					break;
				case 17:
					this.SetCompletedDescription((DbfLocValue)val);
					break;
				case 18:
					this.SetPlayer1DeckId((int)val);
					break;
				case 19:
					this.SetDeckBuilderId((int)val);
					break;
				case 20:
					this.SetDeckRulesetId((int)val);
					break;
				case 21:
					this.SetTbTexture((string)val);
					break;
				case 22:
					this.SetTbTexturePhone((string)val);
					break;
				case 23:
					this.SetTbTexturePhoneOffsetY((float)val);
					break;
				}
			}
		}
	}

	// Token: 0x060002A0 RID: 672 RVA: 0x0000C860 File Offset: 0x0000AA60
	public override Type GetVarType(string name)
	{
		if (name != null)
		{
			if (ScenarioDbfRecord.<>f__switch$map42 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(24);
				dictionary.Add("ID", 0);
				dictionary.Add("NOTE_DESC", 1);
				dictionary.Add("PLAYERS", 2);
				dictionary.Add("PLAYER1_HERO_CARD_ID", 3);
				dictionary.Add("PLAYER2_HERO_CARD_ID", 4);
				dictionary.Add("IS_TUTORIAL", 5);
				dictionary.Add("IS_EXPERT", 6);
				dictionary.Add("IS_COOP", 7);
				dictionary.Add("ADVENTURE_ID", 8);
				dictionary.Add("WING_ID", 9);
				dictionary.Add("SORT_ORDER", 10);
				dictionary.Add("MODE_ID", 11);
				dictionary.Add("CLIENT_PLAYER2_HERO_CARD_ID", 12);
				dictionary.Add("NAME", 13);
				dictionary.Add("SHORT_NAME", 14);
				dictionary.Add("DESCRIPTION", 15);
				dictionary.Add("OPPONENT_NAME", 16);
				dictionary.Add("COMPLETED_DESCRIPTION", 17);
				dictionary.Add("PLAYER1_DECK_ID", 18);
				dictionary.Add("DECK_BUILDER_ID", 19);
				dictionary.Add("DECK_RULESET_ID", 20);
				dictionary.Add("TB_TEXTURE", 21);
				dictionary.Add("TB_TEXTURE_PHONE", 22);
				dictionary.Add("TB_TEXTURE_PHONE_OFFSET_Y", 23);
				ScenarioDbfRecord.<>f__switch$map42 = dictionary;
			}
			int num;
			if (ScenarioDbfRecord.<>f__switch$map42.TryGetValue(name, ref num))
			{
				switch (num)
				{
				case 0:
					return typeof(int);
				case 1:
					return typeof(string);
				case 2:
					return typeof(int);
				case 3:
					return typeof(int);
				case 4:
					return typeof(int);
				case 5:
					return typeof(bool);
				case 6:
					return typeof(bool);
				case 7:
					return typeof(bool);
				case 8:
					return typeof(int);
				case 9:
					return typeof(int);
				case 10:
					return typeof(int);
				case 11:
					return typeof(int);
				case 12:
					return typeof(int);
				case 13:
					return typeof(DbfLocValue);
				case 14:
					return typeof(DbfLocValue);
				case 15:
					return typeof(DbfLocValue);
				case 16:
					return typeof(DbfLocValue);
				case 17:
					return typeof(DbfLocValue);
				case 18:
					return typeof(int);
				case 19:
					return typeof(int);
				case 20:
					return typeof(int);
				case 21:
					return typeof(string);
				case 22:
					return typeof(string);
				case 23:
					return typeof(float);
				}
			}
		}
		return null;
	}

	// Token: 0x040000D3 RID: 211
	private string m_NoteDesc;

	// Token: 0x040000D4 RID: 212
	private int m_Players;

	// Token: 0x040000D5 RID: 213
	private int m_Player1HeroCardId;

	// Token: 0x040000D6 RID: 214
	private int m_Player2HeroCardId;

	// Token: 0x040000D7 RID: 215
	private bool m_IsTutorial;

	// Token: 0x040000D8 RID: 216
	private bool m_IsExpert;

	// Token: 0x040000D9 RID: 217
	private bool m_IsCoop;

	// Token: 0x040000DA RID: 218
	private int m_AdventureId;

	// Token: 0x040000DB RID: 219
	private int m_WingId;

	// Token: 0x040000DC RID: 220
	private int m_SortOrder;

	// Token: 0x040000DD RID: 221
	private int m_ModeId;

	// Token: 0x040000DE RID: 222
	private int m_ClientPlayer2HeroCardId;

	// Token: 0x040000DF RID: 223
	private DbfLocValue m_Name;

	// Token: 0x040000E0 RID: 224
	private DbfLocValue m_ShortName;

	// Token: 0x040000E1 RID: 225
	private DbfLocValue m_Description;

	// Token: 0x040000E2 RID: 226
	private DbfLocValue m_OpponentName;

	// Token: 0x040000E3 RID: 227
	private DbfLocValue m_CompletedDescription;

	// Token: 0x040000E4 RID: 228
	private int m_Player1DeckId;

	// Token: 0x040000E5 RID: 229
	private int m_DeckBuilderId;

	// Token: 0x040000E6 RID: 230
	private int m_DeckRulesetId;

	// Token: 0x040000E7 RID: 231
	private string m_TbTexture;

	// Token: 0x040000E8 RID: 232
	private string m_TbTexturePhone;

	// Token: 0x040000E9 RID: 233
	private float m_TbTexturePhoneOffsetY;
}
