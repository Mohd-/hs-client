using System;

// Token: 0x0200055D RID: 1373
public class TranslatedMedalInfo
{
	// Token: 0x06003F2D RID: 16173 RVA: 0x001337D3 File Offset: 0x001319D3
	public bool IsHighestRankThatCannotBeLost()
	{
		return this.canLoseStars && !this.canLoseLevel;
	}

	// Token: 0x06003F2E RID: 16174 RVA: 0x001337EC File Offset: 0x001319EC
	public bool CanGetRankedRewardChest()
	{
		return this.canLoseStars || this.IsLegendRank();
	}

	// Token: 0x06003F2F RID: 16175 RVA: 0x00133802 File Offset: 0x00131A02
	public bool IsLegendRank()
	{
		return this.rank == 0;
	}

	// Token: 0x06003F30 RID: 16176 RVA: 0x00133810 File Offset: 0x00131A10
	public override string ToString()
	{
		return string.Format("[{3} totalStars={0} earnedStars={1} rank={2} canLoseStars={4} canLoseLevel={5}]", new object[]
		{
			this.totalStars,
			this.earnedStars,
			this.rank,
			this.name,
			this.canLoseStars,
			this.canLoseLevel
		});
	}

	// Token: 0x0400286D RID: 10349
	public int totalStars;

	// Token: 0x0400286E RID: 10350
	public int earnedStars;

	// Token: 0x0400286F RID: 10351
	public int rank = -1;

	// Token: 0x04002870 RID: 10352
	public int bestRank = -1;

	// Token: 0x04002871 RID: 10353
	public int winStreak;

	// Token: 0x04002872 RID: 10354
	public int streakStars;

	// Token: 0x04002873 RID: 10355
	public int legendIndex;

	// Token: 0x04002874 RID: 10356
	public bool canLoseStars;

	// Token: 0x04002875 RID: 10357
	public bool canLoseLevel;

	// Token: 0x04002876 RID: 10358
	public string name;

	// Token: 0x04002877 RID: 10359
	public string nextMedalName;

	// Token: 0x04002878 RID: 10360
	public string textureName;
}
