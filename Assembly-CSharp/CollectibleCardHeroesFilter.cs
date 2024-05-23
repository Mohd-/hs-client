using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000706 RID: 1798
public class CollectibleCardHeroesFilter : CollectibleCardFilter
{
	// Token: 0x060049DB RID: 18907 RVA: 0x0016194A File Offset: 0x0015FB4A
	public void Init(int heroesPerPage)
	{
		this.m_heroesPerPage = heroesPerPage;
		base.FilterHero(true);
		base.FilterOnlyOwned(false);
	}

	// Token: 0x060049DC RID: 18908 RVA: 0x00161964 File Offset: 0x0015FB64
	public void UpdateResults()
	{
		this.m_allResults = Enumerable.ToList<CollectibleCard>(Enumerable.ThenBy<CollectibleCard, TAG_CLASS>(Enumerable.OrderByDescending<CollectibleCard, int>(base.GenerateList(), (CollectibleCard c) => c.OwnedCount), (CollectibleCard c) => c.Class));
		this.m_allResults.RemoveAll((CollectibleCard c) => c.PremiumType == TAG_PREMIUM.GOLDEN && c.OwnedCount == 0);
		this.m_allResults.RemoveAll(delegate(CollectibleCard c)
		{
			if (c.PremiumType == TAG_PREMIUM.GOLDEN)
			{
				return false;
			}
			CollectibleCard card = CollectionManager.Get().GetCard(c.CardId, TAG_PREMIUM.GOLDEN);
			return card != null && card.OwnedCount > 0;
		});
	}

	// Token: 0x060049DD RID: 18909 RVA: 0x00161A18 File Offset: 0x0015FC18
	public List<CollectibleCard> GetHeroesContents(int currentPage)
	{
		currentPage = Mathf.Min(currentPage, this.GetTotalNumPages());
		return Enumerable.ToList<CollectibleCard>(Enumerable.Take<CollectibleCard>(Enumerable.Skip<CollectibleCard>(this.m_allResults, this.m_heroesPerPage * (currentPage - 1)), this.m_heroesPerPage));
	}

	// Token: 0x060049DE RID: 18910 RVA: 0x00161A50 File Offset: 0x0015FC50
	public int GetTotalNumPages()
	{
		int count = this.m_allResults.Count;
		return count / this.m_heroesPerPage + ((count % this.m_heroesPerPage <= 0) ? 0 : 1);
	}

	// Token: 0x04003105 RID: 12549
	private int m_heroesPerPage = 6;

	// Token: 0x04003106 RID: 12550
	private List<CollectibleCard> m_allResults = new List<CollectibleCard>();
}
