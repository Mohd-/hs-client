using System;
using PegasusUtil;
using UnityEngine;

// Token: 0x0200055C RID: 1372
public class MedalInfoTranslator
{
	// Token: 0x06003F1F RID: 16159 RVA: 0x0013320C File Offset: 0x0013140C
	public MedalInfoTranslator()
	{
		new MedalInfoTranslator(-1, 0, -1, 0);
	}

	// Token: 0x06003F20 RID: 16160 RVA: 0x00133220 File Offset: 0x00131420
	public MedalInfoTranslator(int rank, int legendIndex, int wildRank = -1, int wildLegendIndex = 0)
	{
		TranslatedMedalInfo translatedMedalInfo = new TranslatedMedalInfo();
		rank = Mathf.Clamp(rank, 0, 25);
		legendIndex = Mathf.Clamp(legendIndex, -1, legendIndex);
		translatedMedalInfo.rank = rank;
		translatedMedalInfo.legendIndex = legendIndex;
		translatedMedalInfo.textureName = "Medal_Ranked_" + translatedMedalInfo.rank;
		translatedMedalInfo.name = GameStrings.Get("GLOBAL_MEDAL_" + translatedMedalInfo.rank);
		string text = "GLOBAL_MEDAL_" + (translatedMedalInfo.rank - 1);
		string text2 = GameStrings.Get(text);
		if (text2 != text)
		{
			translatedMedalInfo.nextMedalName = text2;
		}
		else
		{
			translatedMedalInfo.nextMedalName = string.Empty;
		}
		this.m_currMedalInfo = translatedMedalInfo;
		TranslatedMedalInfo translatedMedalInfo2 = new TranslatedMedalInfo();
		wildRank = Mathf.Clamp(wildRank, 0, 25);
		wildLegendIndex = Mathf.Clamp(wildLegendIndex, -1, wildLegendIndex);
		translatedMedalInfo2.rank = wildRank;
		translatedMedalInfo2.legendIndex = wildLegendIndex;
		translatedMedalInfo2.textureName = "Medal_Ranked_" + translatedMedalInfo2.rank;
		translatedMedalInfo2.name = GameStrings.Get("GLOBAL_MEDAL_" + translatedMedalInfo2.rank);
		text = "GLOBAL_MEDAL_" + (translatedMedalInfo2.rank - 1);
		text2 = GameStrings.Get(text);
		if (text2 != text)
		{
			translatedMedalInfo2.nextMedalName = text2;
		}
		else
		{
			translatedMedalInfo2.nextMedalName = string.Empty;
		}
		this.m_currWildMedalInfo = translatedMedalInfo2;
	}

	// Token: 0x06003F21 RID: 16161 RVA: 0x00133390 File Offset: 0x00131590
	public MedalInfoTranslator(NetCache.NetCacheMedalInfo currMedalInfo)
	{
		this.m_currMedalInfo = this.Translate(currMedalInfo.Standard);
		this.m_currWildMedalInfo = this.Translate(currMedalInfo.Wild);
	}

	// Token: 0x06003F22 RID: 16162 RVA: 0x001333C8 File Offset: 0x001315C8
	public MedalInfoTranslator(NetCache.NetCacheMedalInfo currMedalInfo, NetCache.NetCacheMedalInfo prevMedalInfo)
	{
		this.m_currMedalInfo = this.Translate(currMedalInfo.Standard);
		this.m_currWildMedalInfo = this.Translate(currMedalInfo.Wild);
		this.m_validPrevMedal = (prevMedalInfo != null);
		this.m_prevMedalInfo = ((!this.m_validPrevMedal) ? this.Translate(currMedalInfo.Standard) : this.Translate(prevMedalInfo.Standard));
		this.m_prevWildMedalInfo = ((!this.m_validPrevMedal) ? this.Translate(currMedalInfo.Wild) : this.Translate(prevMedalInfo.Wild));
	}

	// Token: 0x06003F23 RID: 16163 RVA: 0x00133468 File Offset: 0x00131668
	public bool IsDisplayable(bool useWildMedal)
	{
		TranslatedMedalInfo translatedMedalInfo = (!useWildMedal) ? this.m_currMedalInfo : this.m_currWildMedalInfo;
		return translatedMedalInfo != null && translatedMedalInfo.rank != -1;
	}

	// Token: 0x06003F24 RID: 16164 RVA: 0x001334A4 File Offset: 0x001316A4
	public TranslatedMedalInfo Translate(MedalInfoData medalInfoData)
	{
		TranslatedMedalInfo translatedMedalInfo = new TranslatedMedalInfo();
		if (medalInfoData == null)
		{
			return translatedMedalInfo;
		}
		translatedMedalInfo.rank = 26 - medalInfoData.StarLevel;
		translatedMedalInfo.bestRank = 26 - medalInfoData.BestStarLevel;
		translatedMedalInfo.legendIndex = ((!medalInfoData.HasLegendRank) ? 0 : medalInfoData.LegendRank);
		translatedMedalInfo.totalStars = medalInfoData.LevelEnd - medalInfoData.LevelStart;
		translatedMedalInfo.earnedStars = medalInfoData.Stars;
		if (medalInfoData.StarLevel != 1)
		{
			translatedMedalInfo.earnedStars -= medalInfoData.LevelStart - 1;
		}
		translatedMedalInfo.winStreak = medalInfoData.Streak;
		translatedMedalInfo.textureName = "Medal_Ranked_" + translatedMedalInfo.rank;
		translatedMedalInfo.name = GameStrings.Get("GLOBAL_MEDAL_" + translatedMedalInfo.rank);
		translatedMedalInfo.canLoseStars = medalInfoData.CanLoseStars;
		translatedMedalInfo.canLoseLevel = medalInfoData.CanLoseLevel;
		string text = "GLOBAL_MEDAL_" + (translatedMedalInfo.rank - 1);
		string text2 = GameStrings.Get(text);
		if (text2 != text)
		{
			translatedMedalInfo.nextMedalName = text2;
		}
		else
		{
			translatedMedalInfo.nextMedalName = string.Empty;
		}
		return translatedMedalInfo;
	}

	// Token: 0x06003F25 RID: 16165 RVA: 0x001335DC File Offset: 0x001317DC
	public TranslatedMedalInfo GetCurrentMedal(bool useWildMedal)
	{
		return (!useWildMedal) ? this.m_currMedalInfo : this.m_currWildMedalInfo;
	}

	// Token: 0x06003F26 RID: 16166 RVA: 0x001335F5 File Offset: 0x001317F5
	public TranslatedMedalInfo GetPreviousMedal(bool useWildMedal)
	{
		return (!useWildMedal) ? this.m_prevMedalInfo : this.m_prevWildMedalInfo;
	}

	// Token: 0x06003F27 RID: 16167 RVA: 0x0013360E File Offset: 0x0013180E
	public bool IsPreviousMedalValid()
	{
		return this.m_validPrevMedal;
	}

	// Token: 0x06003F28 RID: 16168 RVA: 0x00133618 File Offset: 0x00131818
	public bool IsBestCurrentRankWild()
	{
		if (this.m_currWildMedalInfo == null)
		{
			return false;
		}
		if (this.m_currMedalInfo == null)
		{
			return true;
		}
		if (this.m_currMedalInfo.rank > this.m_currWildMedalInfo.rank)
		{
			return true;
		}
		if (this.m_currMedalInfo.rank < this.m_currWildMedalInfo.rank)
		{
			return false;
		}
		if (this.m_currMedalInfo.IsLegendRank())
		{
			return this.m_currMedalInfo.legendIndex > this.m_currWildMedalInfo.legendIndex;
		}
		return this.m_currMedalInfo.earnedStars < this.m_currWildMedalInfo.earnedStars;
	}

	// Token: 0x06003F29 RID: 16169 RVA: 0x001336BC File Offset: 0x001318BC
	public bool ShowRewardChest(bool useWildMedal)
	{
		TranslatedMedalInfo translatedMedalInfo = (!useWildMedal) ? this.m_prevMedalInfo : this.m_prevWildMedalInfo;
		TranslatedMedalInfo translatedMedalInfo2 = (!useWildMedal) ? this.m_currMedalInfo : this.m_currWildMedalInfo;
		return translatedMedalInfo != null && translatedMedalInfo2 != null && translatedMedalInfo2.rank != 0 && (translatedMedalInfo2.bestRank < translatedMedalInfo.bestRank && translatedMedalInfo2.CanGetRankedRewardChest());
	}

	// Token: 0x06003F2A RID: 16170 RVA: 0x00133734 File Offset: 0x00131934
	public RankChangeType GetChangeType(bool useWildMedal)
	{
		TranslatedMedalInfo translatedMedalInfo = (!useWildMedal) ? this.m_prevMedalInfo : this.m_prevWildMedalInfo;
		TranslatedMedalInfo translatedMedalInfo2 = (!useWildMedal) ? this.m_currMedalInfo : this.m_currWildMedalInfo;
		if (translatedMedalInfo == null || translatedMedalInfo2 == null)
		{
			return RankChangeType.UNKNOWN;
		}
		if (translatedMedalInfo.rank < translatedMedalInfo2.rank)
		{
			return RankChangeType.RANK_DOWN;
		}
		if (translatedMedalInfo2.rank < translatedMedalInfo.rank)
		{
			return RankChangeType.RANK_UP;
		}
		return RankChangeType.RANK_SAME;
	}

	// Token: 0x06003F2B RID: 16171 RVA: 0x001337A6 File Offset: 0x001319A6
	public void TestSetMedalInfo(TranslatedMedalInfo currMedal, TranslatedMedalInfo prevMedal)
	{
		this.m_currMedalInfo = currMedal;
		this.m_prevMedalInfo = prevMedal;
		this.m_validPrevMedal = true;
	}

	// Token: 0x04002862 RID: 10338
	public const string MEDAL_TEXTURE_PREFIX = "Medal_Ranked_";

	// Token: 0x04002863 RID: 10339
	public const string MEDAL_NAME_PREFIX = "GLOBAL_MEDAL_";

	// Token: 0x04002864 RID: 10340
	public const int MAX_RANK = 26;

	// Token: 0x04002865 RID: 10341
	public const int WORST_DISPLAYABLE_RANK = 25;

	// Token: 0x04002866 RID: 10342
	public const int LEGEND_RANK_VALUE = 0;

	// Token: 0x04002867 RID: 10343
	public const int LOWEST_LEGEND_VALUE = -1;

	// Token: 0x04002868 RID: 10344
	private TranslatedMedalInfo m_currMedalInfo;

	// Token: 0x04002869 RID: 10345
	private TranslatedMedalInfo m_prevMedalInfo;

	// Token: 0x0400286A RID: 10346
	private TranslatedMedalInfo m_currWildMedalInfo;

	// Token: 0x0400286B RID: 10347
	private TranslatedMedalInfo m_prevWildMedalInfo;

	// Token: 0x0400286C RID: 10348
	private bool m_validPrevMedal;
}
