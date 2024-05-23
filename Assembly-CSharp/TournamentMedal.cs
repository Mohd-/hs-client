using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200052E RID: 1326
public class TournamentMedal : PegUIElement
{
	// Token: 0x06003DAF RID: 15791 RVA: 0x0012A5F4 File Offset: 0x001287F4
	protected override void Awake()
	{
		base.Awake();
		if (base.gameObject.GetComponent<TooltipZone>() != null)
		{
			this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.MedalOver));
			this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.MedalOut));
		}
	}

	// Token: 0x06003DB0 RID: 15792 RVA: 0x0012A648 File Offset: 0x00128848
	private void OnDestroy()
	{
		foreach (GameObject gameObject in this.m_stars)
		{
			Object.Destroy(gameObject);
		}
	}

	// Token: 0x06003DB1 RID: 15793 RVA: 0x0012A6A4 File Offset: 0x001288A4
	public void SetMedal(NetCache.NetCacheMedalInfo medal, bool showStars)
	{
		this.SetMedal(new MedalInfoTranslator(medal), showStars);
	}

	// Token: 0x06003DB2 RID: 15794 RVA: 0x0012A6B3 File Offset: 0x001288B3
	public void SetMedal(NetCache.NetCacheMedalInfo medal)
	{
		this.SetMedal(new MedalInfoTranslator(medal), true);
	}

	// Token: 0x06003DB3 RID: 15795 RVA: 0x0012A6C4 File Offset: 0x001288C4
	public void SetMedal(MedalInfoTranslator medal, bool showStars)
	{
		this.m_banner.SetActive(false);
		this.m_medal = medal;
		this.m_showStars = showStars;
		this.m_rankMedal.GetComponent<Renderer>().enabled = false;
		this.m_standardMedalTexture = AssetLoader.Get().LoadTexture(this.m_medal.GetCurrentMedal(false).textureName, false);
		this.m_wildMedalTexture = AssetLoader.Get().LoadTexture(this.m_medal.GetCurrentMedal(true).textureName, false);
		this.UpdateMedal();
	}

	// Token: 0x06003DB4 RID: 15796 RVA: 0x0012A746 File Offset: 0x00128946
	public bool IsBestCurrentRankWild()
	{
		return this.m_medal != null && this.m_medal.IsBestCurrentRankWild();
	}

	// Token: 0x06003DB5 RID: 15797 RVA: 0x0012A760 File Offset: 0x00128960
	public TranslatedMedalInfo GetMedal()
	{
		return this.m_medal.GetCurrentMedal(this.m_isWild);
	}

	// Token: 0x06003DB6 RID: 15798 RVA: 0x0012A774 File Offset: 0x00128974
	private void UpdateStars(int numEarned, int numTotal)
	{
		foreach (GameObject gameObject in this.m_stars)
		{
			Object.Destroy(gameObject);
		}
		int num = 0;
		List<Transform> list;
		if (numTotal % 2 == 0)
		{
			list = this.m_evenStarBones;
			if (numTotal == 2)
			{
				num = 1;
			}
		}
		else
		{
			list = this.m_oddStarBones;
			if (numTotal == 3)
			{
				num = 1;
			}
			else if (numTotal == 1)
			{
				num = 2;
			}
		}
		for (int i = 0; i < numTotal; i++)
		{
			GameObject gameObject2 = Object.Instantiate<GameObject>(this.m_starPrefab);
			gameObject2.transform.parent = base.transform;
			gameObject2.layer = base.gameObject.layer;
			this.m_stars.Add(gameObject2);
			gameObject2.transform.localScale = new Vector3(0.6f, 0.6f, 1f);
			gameObject2.transform.position = list[i + num].position;
			if (i < numEarned)
			{
				gameObject2.GetComponent<Renderer>().material = this.m_starFilledMaterial;
			}
			else
			{
				gameObject2.GetComponent<Renderer>().material = this.m_starEmptyMaterial;
			}
		}
	}

	// Token: 0x06003DB7 RID: 15799 RVA: 0x0012A8D0 File Offset: 0x00128AD0
	public void MedalOver(UIEvent e)
	{
		string text = string.Empty;
		bool flag = SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB);
		TranslatedMedalInfo currentMedal = this.m_medal.GetCurrentMedal(this.m_isWild);
		if (Options.Get().GetBool(Option.IN_RANKED_PLAY_MODE) || flag)
		{
			string rankPercentile = SeasonEndDialog.GetRankPercentile(currentMedal.rank);
			if (this.m_medal.GetCurrentMedal(this.m_isWild).rank == 0)
			{
				text = GameStrings.Format("GLOBAL_MEDAL_TOOLTIP_BODY_LEGEND", new object[0]);
			}
			else if (rankPercentile.Length > 0)
			{
				text = GameStrings.Format("GLOBAL_MEDAL_TOURNAMENT_TOOLTIP_BODY", new object[]
				{
					rankPercentile
				});
			}
			else
			{
				text = GameStrings.Format("GLOBAL_MEDAL_TOOLTIP_BODY", new object[0]);
			}
			string text2 = (!this.IsBestCurrentRankWild()) ? GameStrings.Get("GLOBAL_MEDAL_TOOLTIP_BEST_RANK_STANDARD") : GameStrings.Get("GLOBAL_MEDAL_TOOLTIP_BEST_RANK_WILD");
			text = text + "\n\n" + text2;
		}
		base.gameObject.GetComponent<TooltipZone>().ShowLayerTooltip(currentMedal.name, text);
	}

	// Token: 0x06003DB8 RID: 15800 RVA: 0x0012A9D8 File Offset: 0x00128BD8
	public void SetFormat(bool isWild)
	{
		this.m_isWild = isWild;
		if (this.m_medal != null)
		{
			this.UpdateMedal();
		}
	}

	// Token: 0x06003DB9 RID: 15801 RVA: 0x0012A9F2 File Offset: 0x00128BF2
	private void MedalOut(UIEvent e)
	{
		base.gameObject.GetComponent<TooltipZone>().HideTooltip();
	}

	// Token: 0x06003DBA RID: 15802 RVA: 0x0012AA04 File Offset: 0x00128C04
	private void UpdateMedal()
	{
		this.m_rankMedal.GetComponent<Renderer>().enabled = true;
		this.m_rankMedal.GetComponent<Renderer>().material.mainTexture = ((!this.m_isWild) ? this.m_standardMedalTexture : this.m_wildMedalTexture);
		TranslatedMedalInfo currentMedal = this.m_medal.GetCurrentMedal(this.m_isWild);
		int rank = currentMedal.rank;
		if (rank == 0)
		{
			this.m_banner.SetActive(false);
			this.m_legendIndex.gameObject.SetActive(true);
			if (currentMedal.legendIndex == 0)
			{
				this.m_legendIndex.Text = string.Empty;
			}
			else if (currentMedal.legendIndex == -1)
			{
				this.m_legendIndex.Text = string.Empty;
			}
			else
			{
				this.m_legendIndex.Text = currentMedal.legendIndex.ToString();
			}
		}
		else
		{
			this.m_banner.SetActive(true);
			this.m_rankNumber.Text = rank.ToString();
			this.m_legendIndex.gameObject.SetActive(false);
			this.m_legendIndex.Text = string.Empty;
		}
		if (this.m_wildFlair != null)
		{
			this.m_wildFlair.SetActive(this.m_isWild);
		}
		if (this.m_showStars)
		{
			this.UpdateStars(currentMedal.earnedStars, currentMedal.totalStars);
		}
	}

	// Token: 0x04002740 RID: 10048
	public GameObject m_banner;

	// Token: 0x04002741 RID: 10049
	public UberText m_rankNumber;

	// Token: 0x04002742 RID: 10050
	public UberText m_legendIndex;

	// Token: 0x04002743 RID: 10051
	public GameObject m_rankMedal;

	// Token: 0x04002744 RID: 10052
	public GameObject m_starPrefab;

	// Token: 0x04002745 RID: 10053
	public GameObject m_wildFlair;

	// Token: 0x04002746 RID: 10054
	public Material m_starFilledMaterial;

	// Token: 0x04002747 RID: 10055
	public Material_MobileOverride m_starEmptyMaterial;

	// Token: 0x04002748 RID: 10056
	public GameObject m_glowPlane;

	// Token: 0x04002749 RID: 10057
	public List<Transform> m_evenStarBones;

	// Token: 0x0400274A RID: 10058
	public List<Transform> m_oddStarBones;

	// Token: 0x0400274B RID: 10059
	private MedalInfoTranslator m_medal;

	// Token: 0x0400274C RID: 10060
	private List<GameObject> m_stars = new List<GameObject>();

	// Token: 0x0400274D RID: 10061
	private bool m_isWild;

	// Token: 0x0400274E RID: 10062
	private Texture m_standardMedalTexture;

	// Token: 0x0400274F RID: 10063
	private Texture m_wildMedalTexture;

	// Token: 0x04002750 RID: 10064
	private bool m_showStars;
}
