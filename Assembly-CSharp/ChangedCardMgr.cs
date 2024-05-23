using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

// Token: 0x02000128 RID: 296
public class ChangedCardMgr
{
	// Token: 0x06000F44 RID: 3908 RVA: 0x000421D4 File Offset: 0x000403D4
	private ChangedCardMgr.TrackedCard FindCard(int dbId)
	{
		return this.m_cards.Find((ChangedCardMgr.TrackedCard card) => card.DbId == dbId);
	}

	// Token: 0x06000F45 RID: 3909 RVA: 0x00042208 File Offset: 0x00040408
	private void AddCard(ChangedCardMgr.TrackedCard card)
	{
		ChangedCardMgr.TrackedCard trackedCard = this.FindCard(card.DbId);
		if (trackedCard != null)
		{
			if (trackedCard.Index < card.Index)
			{
				trackedCard.Index = card.Index;
				trackedCard.Count = card.Count;
			}
			return;
		}
		this.m_cards.Add(card);
	}

	// Token: 0x06000F46 RID: 3910 RVA: 0x00042260 File Offset: 0x00040460
	private void Load()
	{
		this.m_isInitialized = true;
		string @string = Options.Get().GetString(Option.CHANGED_CARDS_DATA);
		if (string.IsNullOrEmpty(@string))
		{
			return;
		}
		string[] array = @string.Split(new char[]
		{
			'-'
		});
		foreach (string text in array)
		{
			if (!string.IsNullOrEmpty(text))
			{
				string[] array3 = text.Split(new char[]
				{
					','
				});
				if (array3.Length == 3)
				{
					for (int j = 0; j < 3; j++)
					{
						if (string.IsNullOrEmpty(array3[j]))
						{
						}
					}
					int index;
					if (GeneralUtils.TryParseInt(array3[0], out index))
					{
						int dbId;
						if (GeneralUtils.TryParseInt(array3[1], out dbId))
						{
							int count;
							if (GeneralUtils.TryParseInt(array3[2], out count))
							{
								this.AddCard(new ChangedCardMgr.TrackedCard
								{
									Index = index,
									DbId = dbId,
									Count = count
								});
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x06000F47 RID: 3911 RVA: 0x00042384 File Offset: 0x00040584
	private void Save()
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (ChangedCardMgr.TrackedCard trackedCard in this.m_cards)
		{
			stringBuilder.Append(trackedCard.ToString());
		}
		Options.Get().SetString(Option.CHANGED_CARDS_DATA, stringBuilder.ToString());
		Log.ChangedCards.Print("Saved CHANGED_CARDS_DATA " + stringBuilder.ToString(), new object[0]);
	}

	// Token: 0x06000F48 RID: 3912 RVA: 0x0004241C File Offset: 0x0004061C
	public void Initialize()
	{
		if (!this.m_isInitialized)
		{
			this.Load();
		}
	}

	// Token: 0x06000F49 RID: 3913 RVA: 0x00042430 File Offset: 0x00040630
	public bool AllowCard(int index, int dbId)
	{
		if (!this.m_isInitialized)
		{
			Debug.LogWarning("ChangedCardMgr.AllowCard called before Initialize!");
			return true;
		}
		ChangedCardMgr.TrackedCard trackedCard = this.FindCard(dbId);
		if (trackedCard == null)
		{
			trackedCard = new ChangedCardMgr.TrackedCard();
			trackedCard.Index = index;
			trackedCard.DbId = dbId;
			trackedCard.Count = 0;
			this.AddCard(trackedCard);
		}
		if (trackedCard.Index < index)
		{
			Log.ChangedCards.PrintWarning("Updating to a newer change version for card " + trackedCard, new object[0]);
			trackedCard.Index = index;
			trackedCard.Count = 0;
		}
		if (index == trackedCard.Index && trackedCard.Count < 1)
		{
			trackedCard.Count++;
			this.Save();
			return true;
		}
		return false;
	}

	// Token: 0x06000F4A RID: 3914 RVA: 0x000424E8 File Offset: 0x000406E8
	public static ChangedCardMgr Get()
	{
		return ChangedCardMgr.s_instance;
	}

	// Token: 0x0400082B RID: 2091
	public const int MaxViewCount = 1;

	// Token: 0x0400082C RID: 2092
	private static readonly ChangedCardMgr s_instance = new ChangedCardMgr();

	// Token: 0x0400082D RID: 2093
	private bool m_isInitialized;

	// Token: 0x0400082E RID: 2094
	private List<ChangedCardMgr.TrackedCard> m_cards = new List<ChangedCardMgr.TrackedCard>();

	// Token: 0x02000986 RID: 2438
	private class TrackedCard
	{
		// Token: 0x17000712 RID: 1810
		// (get) Token: 0x0600583D RID: 22589 RVA: 0x001A6FD0 File Offset: 0x001A51D0
		// (set) Token: 0x0600583E RID: 22590 RVA: 0x001A6FD8 File Offset: 0x001A51D8
		public int Index { get; set; }

		// Token: 0x17000713 RID: 1811
		// (get) Token: 0x0600583F RID: 22591 RVA: 0x001A6FE1 File Offset: 0x001A51E1
		// (set) Token: 0x06005840 RID: 22592 RVA: 0x001A6FE9 File Offset: 0x001A51E9
		public int DbId { get; set; }

		// Token: 0x17000714 RID: 1812
		// (get) Token: 0x06005841 RID: 22593 RVA: 0x001A6FF2 File Offset: 0x001A51F2
		// (set) Token: 0x06005842 RID: 22594 RVA: 0x001A6FFA File Offset: 0x001A51FA
		public int Count { get; set; }

		// Token: 0x06005843 RID: 22595 RVA: 0x001A7004 File Offset: 0x001A5204
		public override string ToString()
		{
			return string.Format("{0},{1},{2}-", this.Index, this.DbId, this.Count);
		}
	}
}
