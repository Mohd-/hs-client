using System;
using System.Collections.Generic;
using System.Linq;

// Token: 0x02000705 RID: 1797
public class CollectibleCardClassFilter : CollectibleCardFilter
{
	// Token: 0x060049D0 RID: 18896 RVA: 0x00161540 File Offset: 0x0015F740
	public void Init(TAG_CLASS[] classTabOrder, int cardsPerPage)
	{
		this.m_classTabOrder = classTabOrder;
		this.m_cardsPerPage = cardsPerPage;
		for (int i = 0; i < classTabOrder.Length; i++)
		{
			this.m_currentResultsByClass[classTabOrder[i]] = new List<CollectibleCard>();
		}
	}

	// Token: 0x060049D1 RID: 18897 RVA: 0x00161584 File Offset: 0x0015F784
	public void UpdateResults()
	{
		List<CollectibleCard> list = base.GenerateList();
		foreach (KeyValuePair<TAG_CLASS, List<CollectibleCard>> keyValuePair in this.m_currentResultsByClass)
		{
			keyValuePair.Value.Clear();
		}
		foreach (CollectibleCard collectibleCard in list)
		{
			this.m_currentResultsByClass[collectibleCard.Class].Add(collectibleCard);
		}
	}

	// Token: 0x060049D2 RID: 18898 RVA: 0x00161640 File Offset: 0x0015F840
	public int GetNumPagesForClass(TAG_CLASS cardClass)
	{
		int count = this.m_currentResultsByClass[cardClass].Count;
		return count / this.m_cardsPerPage + ((count % this.m_cardsPerPage <= 0) ? 0 : 1);
	}

	// Token: 0x060049D3 RID: 18899 RVA: 0x0016167D File Offset: 0x0015F87D
	public int GetNumNewCardsForClass(TAG_CLASS cardClass)
	{
		return Enumerable.Count<CollectibleCard>(Enumerable.Where<CollectibleCard>(this.m_currentResultsByClass[cardClass], (CollectibleCard c) => c.SeenCount < c.OwnedCount));
	}

	// Token: 0x060049D4 RID: 18900 RVA: 0x001616B4 File Offset: 0x0015F8B4
	public int GetTotalNumPages()
	{
		int num = 0;
		foreach (TAG_CLASS cardClass in this.m_classTabOrder)
		{
			num += this.GetNumPagesForClass(cardClass);
		}
		return num;
	}

	// Token: 0x060049D5 RID: 18901 RVA: 0x001616F0 File Offset: 0x0015F8F0
	public List<CollectibleCard> GetPageContents(int page)
	{
		if (page < 0 || page > this.GetTotalNumPages())
		{
			return new List<CollectibleCard>();
		}
		int num = 0;
		for (int i = 0; i < this.m_classTabOrder.Length; i++)
		{
			int num2 = num;
			TAG_CLASS tag_CLASS = this.m_classTabOrder[i];
			num += this.GetNumPagesForClass(tag_CLASS);
			if (page <= num)
			{
				int pageWithinClass = page - num2;
				int num3;
				return this.GetPageContentsForClass(tag_CLASS, pageWithinClass, false, out num3);
			}
		}
		return new List<CollectibleCard>();
	}

	// Token: 0x060049D6 RID: 18902 RVA: 0x0016176C File Offset: 0x0015F96C
	public List<CollectibleCard> GetFirstNonEmptyPage(out int collectionPage)
	{
		collectionPage = 0;
		TAG_CLASS pageClass = TAG_CLASS.INVALID;
		for (int i = 0; i < this.m_classTabOrder.Length; i++)
		{
			if (this.m_currentResultsByClass[this.m_classTabOrder[i]].Count > 0)
			{
				pageClass = this.m_classTabOrder[i];
				break;
			}
		}
		return this.GetPageContentsForClass(pageClass, 1, true, out collectionPage);
	}

	// Token: 0x060049D7 RID: 18903 RVA: 0x001617D0 File Offset: 0x0015F9D0
	public List<CollectibleCard> GetPageContentsForClass(TAG_CLASS pageClass, int pageWithinClass, bool calculateCollectionPage, out int collectionPage)
	{
		collectionPage = 0;
		if (pageWithinClass <= 0 || pageWithinClass > this.GetNumPagesForClass(pageClass))
		{
			return new List<CollectibleCard>();
		}
		if (calculateCollectionPage)
		{
			for (int i = 0; i < this.m_classTabOrder.Length; i++)
			{
				TAG_CLASS tag_CLASS = this.m_classTabOrder[i];
				if (tag_CLASS == pageClass)
				{
					break;
				}
				collectionPage += this.GetNumPagesForClass(tag_CLASS);
			}
			collectionPage += pageWithinClass;
		}
		List<CollectibleCard> list = this.m_currentResultsByClass[pageClass];
		if (list == null)
		{
			return new List<CollectibleCard>();
		}
		IEnumerable<CollectibleCard> enumerable = Enumerable.Take<CollectibleCard>(Enumerable.Skip<CollectibleCard>(list, this.m_cardsPerPage * (pageWithinClass - 1)), this.m_cardsPerPage);
		return Enumerable.ToList<CollectibleCard>(enumerable);
	}

	// Token: 0x060049D8 RID: 18904 RVA: 0x00161880 File Offset: 0x0015FA80
	public List<CollectibleCard> GetPageContentsForCard(string cardID, TAG_PREMIUM premiumType, out int collectionPage)
	{
		EntityDef entityDef = DefLoader.Get().GetEntityDef(cardID);
		TAG_CLASS @class = entityDef.GetClass();
		collectionPage = 0;
		List<CollectibleCard> list = this.m_currentResultsByClass[@class];
		int num = list.FindIndex((CollectibleCard obj) => obj.CardId == cardID && obj.PremiumType == premiumType);
		if (num < 0)
		{
			return new List<CollectibleCard>();
		}
		int num2 = num + 1;
		int pageWithinClass = num2 / this.m_cardsPerPage + ((num2 % this.m_cardsPerPage <= 0) ? 0 : 1);
		return this.GetPageContentsForClass(@class, pageWithinClass, true, out collectionPage);
	}

	// Token: 0x04003101 RID: 12545
	private int m_cardsPerPage = 8;

	// Token: 0x04003102 RID: 12546
	private TAG_CLASS[] m_classTabOrder;

	// Token: 0x04003103 RID: 12547
	private Map<TAG_CLASS, List<CollectibleCard>> m_currentResultsByClass = new Map<TAG_CLASS, List<CollectibleCard>>();
}
