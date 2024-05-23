using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

// Token: 0x0200014D RID: 333
public class EntityDef : EntityBase
{
	// Token: 0x06001198 RID: 4504 RVA: 0x0004C241 File Offset: 0x0004A441
	public override string ToString()
	{
		return this.GetDebugName();
	}

	// Token: 0x06001199 RID: 4505 RVA: 0x0004C24C File Offset: 0x0004A44C
	public static XmlElement LoadCardXmlFromAsset(string cardId, Object asset)
	{
		if (asset == null)
		{
			Debug.LogWarning(string.Format("EntityDef.LoadCardXmlFromAsset() - null cardAsset given for cardId \"{0}\"", cardId));
			return null;
		}
		TextAsset xmlAsset = asset as TextAsset;
		return EntityDef.LoadCardXmlFromAsset(cardId, xmlAsset);
	}

	// Token: 0x0600119A RID: 4506 RVA: 0x0004C288 File Offset: 0x0004A488
	public static XmlElement LoadCardXmlFromAsset(string cardId, TextAsset xmlAsset)
	{
		if (xmlAsset == null)
		{
			Debug.LogWarning(string.Format("EntityDef.LoadCardXmlFromAsset() - asset for cardId \"{0}\" was not a card xml", cardId));
			return null;
		}
		XmlDocument xmlDocument = XmlUtils.LoadXmlDocFromTextAsset(xmlAsset);
		return xmlDocument["Entity"];
	}

	// Token: 0x0600119B RID: 4507 RVA: 0x0004C2C8 File Offset: 0x0004A4C8
	public static EntityDef LoadFromAsset(string cardId, TextAsset xmlAsset, bool overrideCardId = false)
	{
		if (xmlAsset == null)
		{
			Debug.LogWarning(string.Format("EntityDef.LoadFromAsset() - asset for cardId \"{0}\" was not a card xml", cardId));
			return null;
		}
		return EntityDef.LoadFromString(cardId, xmlAsset.text, overrideCardId);
	}

	// Token: 0x0600119C RID: 4508 RVA: 0x0004C300 File Offset: 0x0004A500
	public static EntityDef LoadFromString(string cardId, string xmlText, bool overrideCardId = false)
	{
		EntityDef entityDef = new EntityDef();
		using (StringReader stringReader = new StringReader(xmlText))
		{
			using (XmlReader xmlReader = XmlReader.Create(stringReader))
			{
				entityDef.LoadDataFromCardXml(xmlReader);
			}
			if (overrideCardId)
			{
				entityDef.m_cardId = cardId;
			}
		}
		return entityDef;
	}

	// Token: 0x0600119D RID: 4509 RVA: 0x0004C378 File Offset: 0x0004A578
	public EntityDef Clone()
	{
		EntityDef entityDef = new EntityDef();
		entityDef.m_cardId = this.m_cardId;
		entityDef.ReplaceTags(this.m_tags);
		entityDef.m_referencedTags.Replace(this.m_referencedTags);
		foreach (KeyValuePair<int, string> keyValuePair in this.m_stringTags)
		{
			entityDef.m_stringTags.Add(keyValuePair.Key, keyValuePair.Value);
		}
		foreach (KeyValuePair<string, Power> keyValuePair2 in this.m_powers)
		{
			entityDef.m_powers.Add(keyValuePair2.Key, keyValuePair2.Value);
		}
		entityDef.m_masterPower = this.m_masterPower;
		return entityDef;
	}

	// Token: 0x0600119E RID: 4510 RVA: 0x0004C480 File Offset: 0x0004A680
	public string GetCardId()
	{
		return this.m_cardId;
	}

	// Token: 0x0600119F RID: 4511 RVA: 0x0004C488 File Offset: 0x0004A688
	public void SetCardId(string cardId)
	{
		this.m_cardId = cardId;
	}

	// Token: 0x060011A0 RID: 4512 RVA: 0x0004C491 File Offset: 0x0004A691
	public TagSet GetReferencedTags()
	{
		return this.m_referencedTags;
	}

	// Token: 0x060011A1 RID: 4513 RVA: 0x0004C499 File Offset: 0x0004A699
	public override int GetReferencedTag(int tag)
	{
		return this.m_referencedTags.GetTag(tag);
	}

	// Token: 0x060011A2 RID: 4514 RVA: 0x0004C4A7 File Offset: 0x0004A6A7
	public void SetReferencedTag(GAME_TAG enumTag, int val)
	{
		this.SetReferencedTag((int)enumTag, val);
	}

	// Token: 0x060011A3 RID: 4515 RVA: 0x0004C4B1 File Offset: 0x0004A6B1
	public void SetReferencedTag(int tag, int val)
	{
		this.m_referencedTags.SetTag(tag, val);
	}

	// Token: 0x060011A4 RID: 4516 RVA: 0x0004C4C0 File Offset: 0x0004A6C0
	public override string GetStringTag(int tag)
	{
		string result;
		this.m_stringTags.TryGetValue(tag, out result);
		return result;
	}

	// Token: 0x060011A5 RID: 4517 RVA: 0x0004C4DD File Offset: 0x0004A6DD
	public void SetStringTag(GAME_TAG enumTag, string val)
	{
		this.SetStringTag((int)enumTag, val);
	}

	// Token: 0x060011A6 RID: 4518 RVA: 0x0004C4E7 File Offset: 0x0004A6E7
	public void SetStringTag(int tag, string val)
	{
		this.m_stringTags[tag] = val;
	}

	// Token: 0x060011A7 RID: 4519 RVA: 0x0004C4F6 File Offset: 0x0004A6F6
	public TAG_CLASS GetClass()
	{
		return base.GetTag<TAG_CLASS>(GAME_TAG.CLASS);
	}

	// Token: 0x060011A8 RID: 4520 RVA: 0x0004C503 File Offset: 0x0004A703
	public TAG_RACE GetRace()
	{
		return base.GetTag<TAG_RACE>(GAME_TAG.CARDRACE);
	}

	// Token: 0x060011A9 RID: 4521 RVA: 0x0004C510 File Offset: 0x0004A710
	public TAG_ENCHANTMENT_VISUAL GetEnchantmentBirthVisual()
	{
		return base.GetTag<TAG_ENCHANTMENT_VISUAL>(GAME_TAG.ENCHANTMENT_BIRTH_VISUAL);
	}

	// Token: 0x060011AA RID: 4522 RVA: 0x0004C51D File Offset: 0x0004A71D
	public TAG_ENCHANTMENT_VISUAL GetEnchantmentIdleVisual()
	{
		return base.GetTag<TAG_ENCHANTMENT_VISUAL>(GAME_TAG.ENCHANTMENT_IDLE_VISUAL);
	}

	// Token: 0x060011AB RID: 4523 RVA: 0x0004C52A File Offset: 0x0004A72A
	public TAG_RARITY GetRarity()
	{
		return base.GetTag<TAG_RARITY>(GAME_TAG.RARITY);
	}

	// Token: 0x060011AC RID: 4524 RVA: 0x0004C538 File Offset: 0x0004A738
	public string GetName()
	{
		string stringTag = base.GetStringTag(GAME_TAG.CARDNAME);
		if (stringTag != null)
		{
			return stringTag;
		}
		return this.GetDebugName();
	}

	// Token: 0x060011AD RID: 4525 RVA: 0x0004C560 File Offset: 0x0004A760
	public string GetDebugName()
	{
		string stringTag = base.GetStringTag(GAME_TAG.CARDNAME);
		if (stringTag != null)
		{
			return string.Format("[name={0} cardId={1} type={2}]", stringTag, this.m_cardId, base.GetCardType());
		}
		if (this.m_cardId != null)
		{
			return string.Format("[cardId={0} type={1}]", this.m_cardId, base.GetCardType());
		}
		return string.Format("UNKNOWN ENTITY [cardType={0}]", base.GetCardType());
	}

	// Token: 0x060011AE RID: 4526 RVA: 0x0004C5D8 File Offset: 0x0004A7D8
	public string GetArtistName()
	{
		string stringTag = base.GetStringTag(GAME_TAG.ARTISTNAME);
		if (stringTag == null)
		{
			return "ERROR: NO ARTIST NAME";
		}
		return stringTag;
	}

	// Token: 0x060011AF RID: 4527 RVA: 0x0004C600 File Offset: 0x0004A800
	public string GetFlavorText()
	{
		string stringTag = base.GetStringTag(GAME_TAG.FLAVORTEXT);
		if (stringTag == null)
		{
			return string.Empty;
		}
		return stringTag;
	}

	// Token: 0x060011B0 RID: 4528 RVA: 0x0004C628 File Offset: 0x0004A828
	public string GetHowToEarnText(TAG_PREMIUM premium)
	{
		if (premium == TAG_PREMIUM.GOLDEN)
		{
			string stringTag = base.GetStringTag(GAME_TAG.HOW_TO_EARN_GOLDEN);
			if (stringTag == null)
			{
				return string.Empty;
			}
			return stringTag;
		}
		else
		{
			string stringTag = base.GetStringTag(GAME_TAG.HOW_TO_EARN);
			if (stringTag == null)
			{
				return string.Empty;
			}
			return stringTag;
		}
	}

	// Token: 0x060011B1 RID: 4529 RVA: 0x0004C66F File Offset: 0x0004A86F
	public string GetCardTextInHand()
	{
		return TextUtils.TransformCardText(this, GAME_TAG.CARDTEXT_INHAND);
	}

	// Token: 0x060011B2 RID: 4530 RVA: 0x0004C67C File Offset: 0x0004A87C
	public string GetRaceText()
	{
		if (!base.HasTag(GAME_TAG.CARDRACE))
		{
			return string.Empty;
		}
		return GameStrings.GetRaceName(this.GetRace());
	}

	// Token: 0x060011B3 RID: 4531 RVA: 0x0004C6A0 File Offset: 0x0004A8A0
	public Power GetMasterPower()
	{
		if (this.m_masterPower.Equals(string.Empty) || !this.m_powers.ContainsKey(this.m_masterPower))
		{
			return Power.GetDefaultMasterPower();
		}
		return this.m_powers[this.m_masterPower];
	}

	// Token: 0x060011B4 RID: 4532 RVA: 0x0004C6EF File Offset: 0x0004A8EF
	public Power GetAttackPower()
	{
		return Power.GetDefaultAttackPower();
	}

	// Token: 0x060011B5 RID: 4533 RVA: 0x0004C6F8 File Offset: 0x0004A8F8
	public PowerHistoryInfo GetPowerHistoryInfo(int effectIndex)
	{
		if (effectIndex < 0)
		{
			return null;
		}
		PowerHistoryInfo result = null;
		foreach (PowerHistoryInfo powerHistoryInfo in this.m_powerHistoryInfoList)
		{
			if (powerHistoryInfo.GetEffectIndex() == effectIndex)
			{
				result = powerHistoryInfo;
				break;
			}
		}
		return result;
	}

	// Token: 0x060011B6 RID: 4534 RVA: 0x0004C770 File Offset: 0x0004A970
	public List<string> GetEntourageCardIDs()
	{
		return this.m_entourageCardIDs;
	}

	// Token: 0x060011B7 RID: 4535 RVA: 0x0004C778 File Offset: 0x0004A978
	public bool LoadDataFromCardXml(XmlReader reader)
	{
		this.m_powerHistoryInfoList = new List<PowerHistoryInfo>();
		while (reader.NodeType != 1 || reader.Name != "Entity")
		{
			if (!reader.Read())
			{
				break;
			}
		}
		if (reader.EOF)
		{
			return false;
		}
		int depth = reader.Depth;
		this.m_cardId = reader["CardID"];
		bool flag = false;
		while (flag || reader.Read())
		{
			flag = false;
			if (reader.NodeType == 1)
			{
				if (reader.Depth <= depth || reader.EOF)
				{
					break;
				}
				if (this.m_currLoadingPowerDefinition != null && !reader.Name.Equals("PlayRequirement"))
				{
					this.FlushPower();
				}
				string name = reader.Name;
				if (name != null)
				{
					if (EntityDef.<>f__switch$map88 == null)
					{
						Dictionary<string, int> dictionary = new Dictionary<string, int>(7);
						dictionary.Add("Tag", 0);
						dictionary.Add("ReferencedTag", 1);
						dictionary.Add("MasterPower", 2);
						dictionary.Add("Power", 3);
						dictionary.Add("PlayRequirement", 4);
						dictionary.Add("EntourageCard", 5);
						dictionary.Add("TriggeredPowerHistoryInfo", 6);
						EntityDef.<>f__switch$map88 = dictionary;
					}
					int num;
					if (EntityDef.<>f__switch$map88.TryGetValue(name, ref num))
					{
						switch (num)
						{
						case 0:
							flag = this.ReadTag(reader);
							continue;
						case 1:
							this.ReadReferencedTag(reader);
							continue;
						case 2:
							this.ReadMasterPower(reader);
							continue;
						case 3:
							this.ReadPower(reader);
							continue;
						case 4:
							this.ReadPlayRequirement(reader);
							continue;
						case 5:
							this.ReadEntourage(reader);
							continue;
						case 6:
							this.ReadTriggeredPowerHistoryInfo(reader);
							continue;
						}
					}
				}
				Debug.LogError(string.Format("EntityDef.LoadDataFromCardXml() - Unrecognized element \"{0}\" in card xml {1}", reader.Name, this.m_cardId));
			}
		}
		this.FlushPower();
		return true;
	}

	// Token: 0x060011B8 RID: 4536 RVA: 0x0004C988 File Offset: 0x0004AB88
	private bool ReadTag(XmlReader reader)
	{
		string text = reader["enumID"];
		string text2 = reader["type"];
		bool result = false;
		int tag = Convert.ToInt32(text);
		if (text2.Equals("String"))
		{
			if (ApplicationMgr.UsingStandaloneLocalData())
			{
				result = true;
				this.ReadStringTagWithAllLocales(reader, tag);
			}
			else
			{
				this.ReadStringTagWithSingleLocale(reader, tag);
			}
		}
		else
		{
			int tagValue = Convert.ToInt32(reader["value"]);
			base.SetTag(tag, tagValue);
		}
		return result;
	}

	// Token: 0x060011B9 RID: 4537 RVA: 0x0004CA08 File Offset: 0x0004AC08
	private void ReadStringTagWithSingleLocale(XmlReader reader, int tag)
	{
		string value = reader.ReadElementContentAsString();
		this.m_stringTags[tag] = value;
	}

	// Token: 0x060011BA RID: 4538 RVA: 0x0004CA2C File Offset: 0x0004AC2C
	private void ReadStringTagWithAllLocales(XmlReader reader, int tag)
	{
		Map<string, string> map = new Map<string, string>();
		int depth = reader.Depth;
		while (reader.Read())
		{
			if (reader.Depth <= depth)
			{
				break;
			}
			if (reader.NodeType == 1)
			{
				string name = reader.Name;
				try
				{
					string value = reader.ReadElementContentAsString();
					map.Add(name, value);
				}
				catch (Exception ex)
				{
					Debug.LogError(string.Format("{0}: card={1} localeName={2} tag={3}\n{4}", new object[]
					{
						ex.GetType().Name,
						this.GetCardId(),
						name,
						tag,
						ex
					}));
				}
			}
		}
		string text = null;
		if (tag == 342)
		{
			map.TryGetValue(Localization.DEFAULT_LOCALE_NAME, out text);
		}
		else
		{
			foreach (Locale locale in Localization.GetLoadOrder(false))
			{
				string key = locale.ToString();
				map.TryGetValue(key, out text);
				if (text != null)
				{
					break;
				}
			}
		}
		if (text == null)
		{
			Debug.LogError(string.Format("EntityDef.ReadTag() - Error loading card xml {0}, could not find localized text for tag {1}", this.m_cardId, tag));
			text = string.Empty;
		}
		this.m_stringTags[tag] = TextUtils.DecodeWhitespaces(text);
	}

	// Token: 0x060011BB RID: 4539 RVA: 0x0004CB94 File Offset: 0x0004AD94
	private void ReadReferencedTag(XmlReader reader)
	{
		string text = reader["enumID"];
		int tag = Convert.ToInt32(text);
		int val = Convert.ToInt32(reader["value"]);
		this.SetReferencedTag(tag, val);
	}

	// Token: 0x060011BC RID: 4540 RVA: 0x0004CBD0 File Offset: 0x0004ADD0
	private void ReadMasterPower(XmlReader reader)
	{
		if (!string.IsNullOrEmpty(this.m_masterPower))
		{
			Debug.Log(string.Format("Error loading card xml {0}, multiple MasterPower definitions", this.m_cardId));
			return;
		}
		this.m_masterPower = reader.ReadElementContentAsString();
	}

	// Token: 0x060011BD RID: 4541 RVA: 0x0004CC10 File Offset: 0x0004AE10
	private void FlushPower()
	{
		if (this.m_currLoadingPowerDefinition != null)
		{
			Power power = Power.Create(this.m_currLoadingPowerDefinition, this.m_currLoadingPowerInfos);
			if (this.m_powers.ContainsKey(power.GetDefinition()))
			{
				Debug.LogError(string.Format("Error loading card xml {0}, already contains power definition {1}", this.m_cardId, power.GetDefinition()));
			}
			else
			{
				this.m_powers.Add(power.GetDefinition(), power);
			}
		}
		this.m_currLoadingPowerDefinition = null;
		this.m_currLoadingPowerInfos = null;
	}

	// Token: 0x060011BE RID: 4542 RVA: 0x0004CC90 File Offset: 0x0004AE90
	private void ReadPower(XmlReader reader)
	{
		string currLoadingPowerDefinition = reader["definition"];
		this.m_currLoadingPowerDefinition = currLoadingPowerDefinition;
		this.m_currLoadingPowerInfos = new List<Power.PowerInfo>();
	}

	// Token: 0x060011BF RID: 4543 RVA: 0x0004CCBC File Offset: 0x0004AEBC
	private void ReadPlayRequirement(XmlReader reader)
	{
		string text = reader["reqID"];
		string text2 = reader["param"];
		if (string.IsNullOrEmpty(text))
		{
			Debug.LogError("PlayRequirement is missing requirement ID");
			return;
		}
		int reqId = Convert.ToInt32(text);
		int param;
		if (string.IsNullOrEmpty(text2))
		{
			param = 0;
		}
		else
		{
			param = Convert.ToInt32(text2);
		}
		Power.PowerInfo powerInfo;
		powerInfo.reqId = (PlayErrors.ErrorType)reqId;
		powerInfo.param = param;
		this.m_currLoadingPowerInfos.Add(powerInfo);
	}

	// Token: 0x060011C0 RID: 4544 RVA: 0x0004CD34 File Offset: 0x0004AF34
	private void ReadEntourage(XmlReader reader)
	{
		this.m_entourageCardIDs.Add(reader["cardID"]);
	}

	// Token: 0x060011C1 RID: 4545 RVA: 0x0004CD4C File Offset: 0x0004AF4C
	private void ReadTriggeredPowerHistoryInfo(XmlReader reader)
	{
		string text = reader["effectIndex"];
		string text2 = reader["showInHistory"];
		int index = Convert.ToInt32(text);
		bool show = Convert.ToBoolean(text2);
		this.m_powerHistoryInfoList.Add(new PowerHistoryInfo(index, show));
	}

	// Token: 0x060011C2 RID: 4546 RVA: 0x0004CD94 File Offset: 0x0004AF94
	public static string GetTagString(XmlNode node, int tag, Locale loc)
	{
		XmlElement xmlElement = null;
		if (tag != 342)
		{
			foreach (Locale locale in Localization.GetLoadOrder(loc, false))
			{
				string text = locale.ToString();
				xmlElement = node[text];
				if (xmlElement != null)
				{
					break;
				}
			}
		}
		else
		{
			xmlElement = node[Localization.DEFAULT_LOCALE_NAME];
		}
		if (xmlElement != null)
		{
			return TextUtils.DecodeWhitespaces(xmlElement.InnerText);
		}
		return null;
	}

	// Token: 0x04000953 RID: 2387
	private string m_cardId;

	// Token: 0x04000954 RID: 2388
	protected TagSet m_referencedTags = new TagSet();

	// Token: 0x04000955 RID: 2389
	protected Map<int, string> m_stringTags = new Map<int, string>();

	// Token: 0x04000956 RID: 2390
	private Map<string, Power> m_powers = new Map<string, Power>();

	// Token: 0x04000957 RID: 2391
	private string m_masterPower = string.Empty;

	// Token: 0x04000958 RID: 2392
	private List<PowerHistoryInfo> m_powerHistoryInfoList = new List<PowerHistoryInfo>();

	// Token: 0x04000959 RID: 2393
	private List<string> m_entourageCardIDs = new List<string>();

	// Token: 0x0400095A RID: 2394
	private string m_currLoadingPowerDefinition;

	// Token: 0x0400095B RID: 2395
	private List<Power.PowerInfo> m_currLoadingPowerInfos;
}
