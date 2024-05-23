using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

// Token: 0x020005F6 RID: 1526
public class CollectibleCardFilter
{
	// Token: 0x060042D7 RID: 17111 RVA: 0x0014160D File Offset: 0x0013F80D
	public void SetDeckRuleset(DeckRuleset deckRuleset)
	{
		this.m_deckRuleset = deckRuleset;
	}

	// Token: 0x060042D8 RID: 17112 RVA: 0x00141616 File Offset: 0x0013F816
	public void FilterTheseCardSets(params TAG_CARD_SET[] cardSets)
	{
		this.m_filterCardSets = null;
		if (cardSets != null && cardSets.Length > 0)
		{
			this.m_filterCardSets = cardSets;
		}
	}

	// Token: 0x060042D9 RID: 17113 RVA: 0x00141638 File Offset: 0x0013F838
	public bool CardSetFilterIncludesWild()
	{
		if (this.m_filterCardSets == null)
		{
			return true;
		}
		foreach (TAG_CARD_SET set in this.m_filterCardSets)
		{
			if (GameUtils.IsSetRotated(set))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060042DA RID: 17114 RVA: 0x0014167F File Offset: 0x0013F87F
	public void FilterTheseClasses(params TAG_CLASS[] classTypes)
	{
		this.m_filterClasses = null;
		if (classTypes != null && classTypes.Length > 0)
		{
			this.m_filterClasses = classTypes;
		}
	}

	// Token: 0x060042DB RID: 17115 RVA: 0x0014169E File Offset: 0x0013F89E
	public void FilterTheseCardTypes(params TAG_CARDTYPE[] cardTypes)
	{
		this.m_filterCardTypes = null;
		if (cardTypes != null && cardTypes.Length > 0)
		{
			this.m_filterCardTypes = cardTypes;
		}
	}

	// Token: 0x060042DC RID: 17116 RVA: 0x001416BD File Offset: 0x0013F8BD
	public void FilterManaCost(int? manaCost)
	{
		this.m_filterManaCost = manaCost;
	}

	// Token: 0x060042DD RID: 17117 RVA: 0x001416C8 File Offset: 0x0013F8C8
	public void FilterOnlyOwned(bool owned)
	{
		this.m_filterOwnedMinimum = default(int?);
		if (owned)
		{
			this.m_filterOwnedMinimum = new int?(1);
		}
	}

	// Token: 0x060042DE RID: 17118 RVA: 0x001416F6 File Offset: 0x0013F8F6
	public void FilterPremium(TAG_PREMIUM? premiumType)
	{
		this.m_filterPremium = premiumType;
	}

	// Token: 0x060042DF RID: 17119 RVA: 0x00141700 File Offset: 0x0013F900
	public void FilterOnlyCraftable(bool onlyCraftable)
	{
		this.m_filterOnlyCraftable = default(bool?);
		if (onlyCraftable)
		{
			this.m_filterOnlyCraftable = new bool?(true);
		}
	}

	// Token: 0x060042E0 RID: 17120 RVA: 0x0014172E File Offset: 0x0013F92E
	public void FilterSearchText(string searchText)
	{
		this.m_filterText = searchText;
	}

	// Token: 0x060042E1 RID: 17121 RVA: 0x00141737 File Offset: 0x0013F937
	public bool HasSearchText()
	{
		return !string.IsNullOrEmpty(this.m_filterText);
	}

	// Token: 0x060042E2 RID: 17122 RVA: 0x00141747 File Offset: 0x0013F947
	public void FilterHero(bool isHero)
	{
		this.m_filterIsHero = isHero;
	}

	// Token: 0x060042E3 RID: 17123 RVA: 0x00141750 File Offset: 0x0013F950
	public List<CollectibleCard> GenerateList()
	{
		CollectionManager collectionManager = CollectionManager.Get();
		int? filterManaCost = this.m_filterManaCost;
		bool? isHero = new bool?(this.m_filterIsHero);
		int? filterOwnedMinimum = this.m_filterOwnedMinimum;
		return collectionManager.FindOrderedCards(this.m_filterText, this.m_filterPremium, filterManaCost, this.m_filterCardSets, this.m_filterClasses, this.m_filterCardTypes, default(TAG_RARITY?), default(TAG_RACE?), isHero, filterOwnedMinimum, default(bool?), this.m_filterOnlyCraftable, null, this.m_deckRuleset);
	}

	// Token: 0x060042E4 RID: 17124 RVA: 0x001417D0 File Offset: 0x0013F9D0
	public static void AddSearchableTokensToSet(string str, HashSet<string> addToList, bool split = true)
	{
		string[] array;
		if (split)
		{
			array = str.Split(new char[]
			{
				' ',
				'\t'
			}, 1);
		}
		else
		{
			(array = new string[1])[0] = str;
		}
		string[] array2 = array;
		foreach (string token in array2)
		{
			CollectibleCardFilter.AddSingleSearchableTokenToSet(token, addToList);
		}
		if (array2.Length > 1)
		{
			CollectibleCardFilter.AddSingleSearchableTokenToSet(str, addToList);
		}
	}

	// Token: 0x060042E5 RID: 17125 RVA: 0x0014183B File Offset: 0x0013FA3B
	public static void AddSearchableTokensToSet<T>(T structType, Func<T, bool> hasTypeString, Func<T, string> getTypeString, HashSet<string> addToList) where T : struct
	{
		if (hasTypeString.Invoke(structType))
		{
			CollectibleCardFilter.AddSearchableTokensToSet(getTypeString.Invoke(structType), addToList, true);
		}
	}

	// Token: 0x060042E6 RID: 17126 RVA: 0x00141858 File Offset: 0x0013FA58
	private static void AddSingleSearchableTokenToSet(string token, HashSet<string> addToList)
	{
		string text = token.ToLower();
		string text2 = CollectibleCardFilter.ConvertEuropeanCharacters(text);
		string text3 = CollectibleCardFilter.RemoveDiacritics(text);
		addToList.Add(text);
		if (!text.Equals(text2))
		{
			addToList.Add(text2);
		}
		if (!text.Equals(text3))
		{
			addToList.Add(text3);
		}
	}

	// Token: 0x060042E7 RID: 17127 RVA: 0x001418AC File Offset: 0x0013FAAC
	public static List<CollectionManager.CollectibleCardFilterFunc> FiltersFromSearchString(string searchString)
	{
		char[] array = new char[]
		{
			'+'
		};
		char[] array2 = new char[]
		{
			'-'
		};
		List<CollectionManager.CollectibleCardFilterFunc> list = new List<CollectionManager.CollectibleCardFilterFunc>();
		if (!string.IsNullOrEmpty(searchString))
		{
			string text = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_GOLDEN");
			string text2 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_ARTIST");
			string text3 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_HEALTH");
			string text4 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_ATTACK");
			string text5 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_OWNED");
			string text6 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_MANA");
			string text7 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_MISSING");
			string text8 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_EXTRA");
			string text9 = GameStrings.Get("GLUE_COLLECTION_MANAGER_SEARCH_NEW");
			string text10 = searchString.ToLower();
			string[] array3 = text10.Split(new char[]
			{
				' '
			});
			StringBuilder regularTokens = new StringBuilder();
			for (int i = 0; i < array3.Length; i++)
			{
				if (array3[i] == text7)
				{
					array3[i] = string.Format("{0}:0", text5);
				}
				if (!(array3[i] == text8))
				{
					if (array3[i] == text9)
					{
						list.Add((CollectibleCard card) => card.IsNewCard);
					}
					else if (array3[i] == text)
					{
						list.Add((CollectibleCard card) => card.PremiumType == TAG_PREMIUM.GOLDEN);
					}
					else
					{
						bool flag = false;
						char[] array4 = new char[]
						{
							':',
							'：'
						};
						if (Enumerable.Any<char>(array4, new Func<char, bool>(array3[i].Contains)))
						{
							string[] array5 = array3[i].Split(array4);
							if (array5.Length == 2)
							{
								string text11 = array5[0].Trim();
								string val = array5[1].Trim();
								bool flag2 = false;
								bool flag3 = false;
								if (val.TrimEnd(array) != val)
								{
									val = val.TrimEnd(array);
									flag2 = true;
								}
								if (val.TrimEnd(array2) != val)
								{
									val = val.TrimEnd(array2);
									flag3 = true;
								}
								int minVal = -1;
								int maxVal = -1;
								bool flag4 = false;
								if (val.Contains("-"))
								{
									string[] array6 = val.Split(new char[]
									{
										'-'
									});
									if (array6.Length == 2)
									{
										flag4 = int.TryParse(array6[0].Trim(), ref minVal);
										flag4 = (flag4 && int.TryParse(array6[1].Trim(), ref maxVal));
									}
								}
								else
								{
									flag4 = int.TryParse(val, ref minVal);
									maxVal = minVal;
								}
								if (flag4)
								{
									if (flag3)
									{
										minVal = int.MinValue;
									}
									if (flag2)
									{
										maxVal = int.MaxValue;
									}
									if (text11 == text4)
									{
										list.Add((CollectibleCard card) => card.Attack >= minVal && card.Attack <= maxVal);
										list.Add((CollectibleCard card) => card.CardType == TAG_CARDTYPE.MINION || card.CardType == TAG_CARDTYPE.WEAPON);
										flag = true;
									}
									if (text11 == text3)
									{
										list.Add((CollectibleCard card) => card.Health >= minVal && card.Health <= maxVal);
										list.Add((CollectibleCard card) => card.CardType == TAG_CARDTYPE.MINION);
										flag = true;
									}
									if (text11 == text6)
									{
										list.Add((CollectibleCard card) => card.ManaCost >= minVal && card.ManaCost <= maxVal);
										flag = true;
									}
									if (text11 == text5)
									{
										list.Add((CollectibleCard card) => card.OwnedCount >= minVal && card.OwnedCount <= maxVal);
										flag = true;
									}
								}
								else if (text11 == text2)
								{
									list.Add((CollectibleCard card) => CollectibleCard.FindTextInternational(val, card.ArtistName));
									flag = true;
								}
							}
						}
						if (!flag)
						{
							regularTokens.Append(array3[i]);
							regularTokens.Append(" ");
						}
					}
				}
			}
			list.Add((CollectibleCard card) => card.FindTextInCard(regularTokens.ToString()));
		}
		return list;
	}

	// Token: 0x060042E8 RID: 17128 RVA: 0x00141D34 File Offset: 0x0013FF34
	public static string ConvertEuropeanCharacters(string input)
	{
		int length = input.Length;
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < length; i++)
		{
			string text;
			if (CollectibleCardFilter.s_europeanConversionTable.TryGetValue(input.get_Chars(i), out text))
			{
				stringBuilder.Append(text);
			}
			else
			{
				stringBuilder.Append(input.get_Chars(i));
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060042E9 RID: 17129 RVA: 0x00141D9C File Offset: 0x0013FF9C
	public static string RemoveDiacritics(string input)
	{
		string text = input.Normalize(2);
		int length = text.Length;
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < length; i++)
		{
			UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(text.get_Chars(i));
			if (unicodeCategory != 5)
			{
				stringBuilder.Append(text.get_Chars(i));
			}
		}
		return stringBuilder.ToString().Normalize(1);
	}

	// Token: 0x04002A86 RID: 10886
	private TAG_CARD_SET[] m_filterCardSets;

	// Token: 0x04002A87 RID: 10887
	private TAG_CLASS[] m_filterClasses;

	// Token: 0x04002A88 RID: 10888
	private TAG_CARDTYPE[] m_filterCardTypes;

	// Token: 0x04002A89 RID: 10889
	private int? m_filterManaCost;

	// Token: 0x04002A8A RID: 10890
	private int? m_filterOwnedMinimum = new int?(1);

	// Token: 0x04002A8B RID: 10891
	private TAG_PREMIUM? m_filterPremium;

	// Token: 0x04002A8C RID: 10892
	private bool? m_filterOnlyCraftable;

	// Token: 0x04002A8D RID: 10893
	private string m_filterText;

	// Token: 0x04002A8E RID: 10894
	private bool m_filterIsHero;

	// Token: 0x04002A8F RID: 10895
	private DeckRuleset m_deckRuleset;

	// Token: 0x04002A90 RID: 10896
	private static Map<char, string> s_europeanConversionTable = new Map<char, string>
	{
		{
			'œ',
			"oe"
		},
		{
			'æ',
			"ae"
		},
		{
			'’',
			"'"
		},
		{
			'«',
			"\""
		},
		{
			'»',
			"\""
		},
		{
			'ä',
			"ae"
		},
		{
			'ü',
			"ue"
		},
		{
			'ö',
			"oe"
		},
		{
			'ß',
			"ss"
		}
	};

	// Token: 0x0200070B RID: 1803
	// (Invoke) Token: 0x060049FD RID: 18941
	public delegate void OnResultsUpdated();
}
