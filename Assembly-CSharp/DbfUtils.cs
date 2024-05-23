using System;
using System.Collections.Generic;
using PegasusShared;

// Token: 0x020005D9 RID: 1497
public static class DbfUtils
{
	// Token: 0x06004287 RID: 17031 RVA: 0x00140DB8 File Offset: 0x0013EFB8
	public static ScenarioDbfRecord ConvertFromProtobuf(ScenarioDbRecord protoScenario)
	{
		if (protoScenario == null)
		{
			return null;
		}
		ScenarioDbfRecord scenarioDbfRecord = new ScenarioDbfRecord();
		scenarioDbfRecord.SetID(protoScenario.Id);
		scenarioDbfRecord.SetNoteDesc(protoScenario.NoteDesc);
		scenarioDbfRecord.SetPlayers(protoScenario.NumPlayers);
		scenarioDbfRecord.SetPlayer1HeroCardId((int)protoScenario.Player1HeroCardId);
		scenarioDbfRecord.SetPlayer2HeroCardId((int)protoScenario.Player2HeroCardId);
		scenarioDbfRecord.SetIsExpert(protoScenario.IsExpert);
		scenarioDbfRecord.SetIsCoop(protoScenario.HasIsCoop && protoScenario.IsCoop);
		scenarioDbfRecord.SetAdventureId(protoScenario.AdventureId);
		if (protoScenario.HasAdventureModeId)
		{
			scenarioDbfRecord.SetModeId(protoScenario.AdventureModeId);
		}
		scenarioDbfRecord.SetWingId(protoScenario.WingId);
		scenarioDbfRecord.SetSortOrder(protoScenario.SortOrder);
		if (protoScenario.HasClientPlayer2HeroCardId)
		{
			scenarioDbfRecord.SetClientPlayer2HeroCardId((int)protoScenario.ClientPlayer2HeroCardId);
		}
		scenarioDbfRecord.SetTbTexture(protoScenario.TavernBrawlTexture);
		scenarioDbfRecord.SetTbTexturePhone(protoScenario.TavernBrawlTexturePhone);
		if (protoScenario.HasTavernBrawlTexturePhoneOffset)
		{
			scenarioDbfRecord.SetTbTexturePhoneOffsetY(protoScenario.TavernBrawlTexturePhoneOffset.Y);
		}
		DbfUtils.AddLocStrings(scenarioDbfRecord, protoScenario.Strings);
		if (protoScenario.HasDeckRulesetId)
		{
			scenarioDbfRecord.SetDeckRulesetId(protoScenario.DeckRulesetId);
		}
		return scenarioDbfRecord;
	}

	// Token: 0x06004288 RID: 17032 RVA: 0x00140EE4 File Offset: 0x0013F0E4
	public static DeckRulesetDbfRecord ConvertFromProtobuf(DeckRulesetDbRecord proto)
	{
		if (proto == null)
		{
			return null;
		}
		DeckRulesetDbfRecord deckRulesetDbfRecord = new DeckRulesetDbfRecord();
		deckRulesetDbfRecord.SetID(proto.Id);
		return deckRulesetDbfRecord;
	}

	// Token: 0x06004289 RID: 17033 RVA: 0x00140F0C File Offset: 0x0013F10C
	public static DeckRulesetRuleDbfRecord ConvertFromProtobuf(DeckRulesetRuleDbRecord proto, out List<int> outTargetSubsetIds)
	{
		outTargetSubsetIds = null;
		if (proto == null)
		{
			return null;
		}
		DeckRulesetRuleDbfRecord deckRulesetRuleDbfRecord = new DeckRulesetRuleDbfRecord();
		deckRulesetRuleDbfRecord.SetID(proto.Id);
		deckRulesetRuleDbfRecord.SetDeckRulesetId(proto.DeckRulesetId);
		if (proto.HasAppliesToSubsetId)
		{
			deckRulesetRuleDbfRecord.SetAppliesToSubsetId(proto.AppliesToSubsetId);
		}
		if (proto.HasAppliesToIsNot)
		{
			deckRulesetRuleDbfRecord.SetAppliesToIsNot(proto.AppliesToIsNot);
		}
		deckRulesetRuleDbfRecord.SetRuleType(proto.RuleType);
		deckRulesetRuleDbfRecord.SetRuleIsNot(proto.RuleIsNot);
		if (proto.HasMinValue)
		{
			deckRulesetRuleDbfRecord.SetMinValue(proto.MinValue);
		}
		if (proto.HasMaxValue)
		{
			deckRulesetRuleDbfRecord.SetMaxValue(proto.MaxValue);
		}
		if (proto.HasTag)
		{
			deckRulesetRuleDbfRecord.SetTag(proto.Tag);
		}
		if (proto.HasTagMinValue)
		{
			deckRulesetRuleDbfRecord.SetTagMinValue(proto.TagMinValue);
		}
		if (proto.HasTagMaxValue)
		{
			deckRulesetRuleDbfRecord.SetTagMaxValue(proto.TagMaxValue);
		}
		if (proto.HasStringValue)
		{
			deckRulesetRuleDbfRecord.SetStringValue(proto.StringValue);
		}
		outTargetSubsetIds = proto.TargetSubsetIds;
		DbfUtils.AddLocStrings(deckRulesetRuleDbfRecord, proto.Strings);
		return deckRulesetRuleDbfRecord;
	}

	// Token: 0x0600428A RID: 17034 RVA: 0x00141028 File Offset: 0x0013F228
	private static void AddLocStrings(DbfRecord record, List<LocalizedString> protoStrings)
	{
		foreach (LocalizedString localizedString in protoStrings)
		{
			DbfLocValue dbfLocValue = new DbfLocValue();
			string key = localizedString.Key;
			for (int i = 0; i < localizedString.Values.Count; i++)
			{
				LocalizedStringValue localizedStringValue = localizedString.Values[i];
				Locale locale = (Locale)localizedStringValue.Locale;
				dbfLocValue.SetString(locale, TextUtils.DecodeWhitespaces(localizedStringValue.Value));
			}
			record.SetVar(key, dbfLocValue);
		}
	}
}
