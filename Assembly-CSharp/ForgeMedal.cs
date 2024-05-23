using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F6A RID: 3946
public class ForgeMedal : PegUIElement
{
	// Token: 0x0600751B RID: 29979 RVA: 0x00229234 File Offset: 0x00227434
	protected override void Awake()
	{
		base.Awake();
		this.m_forgeMedalInfos.Add(new ForgeMedal.ForgeMedalInfo(new Medal(Medal.Type.MEDAL_NOVICE), new Vector2(0f, 0f)));
		this.m_forgeMedalInfos.Add(new ForgeMedal.ForgeMedalInfo(new Medal(Medal.Type.MEDAL_APPRENTICE), new Vector2(0.25f, 0f)));
		this.m_forgeMedalInfos.Add(new ForgeMedal.ForgeMedalInfo(new Medal(Medal.Type.MEDAL_JOURNEYMAN), new Vector2(0.5f, 0f)));
		this.m_forgeMedalInfos.Add(new ForgeMedal.ForgeMedalInfo(new Medal(Medal.Type.MEDAL_COPPER_A), new Vector2(0.75f, 0f)));
		this.m_forgeMedalInfos.Add(new ForgeMedal.ForgeMedalInfo(new Medal(Medal.Type.MEDAL_SILVER_A), new Vector2(0f, -0.25f)));
		this.m_forgeMedalInfos.Add(new ForgeMedal.ForgeMedalInfo(new Medal(Medal.Type.MEDAL_GOLD_A), new Vector2(0.25f, -0.25f)));
		this.m_forgeMedalInfos.Add(new ForgeMedal.ForgeMedalInfo(new Medal(Medal.Type.MEDAL_PLATINUM_A), new Vector2(0.5f, -0.25f)));
		this.m_forgeMedalInfos.Add(new ForgeMedal.ForgeMedalInfo(new Medal(Medal.Type.MEDAL_DIAMOND_A), new Vector2(0.75f, -0.25f)));
		this.m_forgeMedalInfos.Add(new ForgeMedal.ForgeMedalInfo(new Medal(Medal.Type.MEDAL_MASTER_A), new Vector2(0f, -0.5f)));
		this.m_forgeMedalInfos.Add(new ForgeMedal.ForgeMedalInfo(new Medal(1), new Vector2(0.25f, -0.5f)));
		this.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.MedalOver));
		this.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.MedalOut));
	}

	// Token: 0x0600751C RID: 29980 RVA: 0x002293E8 File Offset: 0x002275E8
	public void SetMedal(int wins)
	{
		ForgeMedal.ForgeMedalInfo forgeMedalInfo = this.m_forgeMedalInfos[wins];
		this.m_medal = forgeMedalInfo.ForgeMedal;
		int num = wins + 1;
		this.m_nextMedal = ((this.m_forgeMedalInfos.Count <= num) ? null : this.m_forgeMedalInfos[num].ForgeMedal);
		base.GetComponent<Renderer>().material.SetTextureOffset("_MainTex", forgeMedalInfo.TextureCoords);
	}

	// Token: 0x0600751D RID: 29981 RVA: 0x0022945C File Offset: 0x0022765C
	private void MedalOver(UIEvent e)
	{
		string headline = GameStrings.Format("GLOBAL_MEDAL_TOOLTIP_HEADER", new object[]
		{
			this.m_medal.GetBaseMedalName()
		});
		string bodytext = string.Empty;
		if (this.m_nextMedal == null)
		{
			bodytext = GameStrings.Get("GLOBAL_MEDAL_TOOLTIP_BODY_ARENA_GRAND_MASTER");
		}
		else
		{
			bodytext = GameStrings.Format("GLOBAL_MEDAL_TOOLTIP_BODY", new object[]
			{
				this.m_nextMedal.GetBaseMedalName()
			});
		}
		float scale;
		if (SceneMgr.Get().IsInGame())
		{
			scale = 0.3f;
		}
		else if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB))
		{
			scale = 6f;
		}
		else
		{
			scale = 3f;
		}
		base.gameObject.GetComponent<TooltipZone>().ShowTooltip(headline, bodytext, scale, true);
	}

	// Token: 0x0600751E RID: 29982 RVA: 0x00229518 File Offset: 0x00227718
	private void MedalOut(UIEvent e)
	{
		base.gameObject.GetComponent<TooltipZone>().HideTooltip();
	}

	// Token: 0x04005FA8 RID: 24488
	private List<ForgeMedal.ForgeMedalInfo> m_forgeMedalInfos = new List<ForgeMedal.ForgeMedalInfo>();

	// Token: 0x04005FA9 RID: 24489
	private Medal m_medal;

	// Token: 0x04005FAA RID: 24490
	private Medal m_nextMedal;

	// Token: 0x02000F6B RID: 3947
	private class ForgeMedalInfo
	{
		// Token: 0x0600751F RID: 29983 RVA: 0x0022952A File Offset: 0x0022772A
		public ForgeMedalInfo(Medal forgeMedal, Vector2 textureCoords)
		{
			this.ForgeMedal = forgeMedal;
			this.TextureCoords = textureCoords;
		}

		// Token: 0x17000A11 RID: 2577
		// (get) Token: 0x06007520 RID: 29984 RVA: 0x00229540 File Offset: 0x00227740
		// (set) Token: 0x06007521 RID: 29985 RVA: 0x00229548 File Offset: 0x00227748
		public Vector2 TextureCoords { get; private set; }

		// Token: 0x17000A12 RID: 2578
		// (get) Token: 0x06007522 RID: 29986 RVA: 0x00229551 File Offset: 0x00227751
		// (set) Token: 0x06007523 RID: 29987 RVA: 0x00229559 File Offset: 0x00227759
		public Medal ForgeMedal { get; private set; }
	}
}
