using System;
using UnityEngine;

// Token: 0x02000AA6 RID: 2726
[CustomEditClass]
public class AdventureCompleteReward : Reward
{
	// Token: 0x06005E8B RID: 24203 RVA: 0x001C4ABA File Offset: 0x001C2CBA
	protected override void InitData()
	{
		base.SetData(new AdventureCompleteRewardData(), false);
	}

	// Token: 0x06005E8C RID: 24204 RVA: 0x001C4AC8 File Offset: 0x001C2CC8
	protected override void ShowReward(bool updateCacheValues)
	{
		if (base.IsShown)
		{
			return;
		}
		AdventureCompleteRewardData adventureCompleteRewardData = base.Data as AdventureCompleteRewardData;
		if (this.m_StateTable != null)
		{
			string eventName = (!adventureCompleteRewardData.IsBadlyHurt || !this.m_StateTable.HasState("ShowBadlyHurt")) ? "ShowHurt" : "ShowBadlyHurt";
			this.m_StateTable.TriggerState(eventName, true, null);
		}
		if (this.m_BannerTextObject != null)
		{
			this.m_BannerTextObject.Text = adventureCompleteRewardData.BannerText;
		}
		if (this.m_BannerObject != null && this.m_BannerScaleOverride != null)
		{
			Vector3 vector = this.m_BannerScaleOverride;
			if (vector != Vector3.zero)
			{
				this.m_BannerObject.transform.localScale = vector;
			}
		}
		this.FadeFullscreenEffectsIn();
	}

	// Token: 0x06005E8D RID: 24205 RVA: 0x001C4BB0 File Offset: 0x001C2DB0
	protected override void HideReward()
	{
		if (!base.IsShown)
		{
			return;
		}
		base.HideReward();
		if (this.m_StateTable != null)
		{
			this.m_StateTable.TriggerState("Hide", true, null);
		}
		this.FadeFullscreenEffectsOut();
	}

	// Token: 0x06005E8E RID: 24206 RVA: 0x001C4BF8 File Offset: 0x001C2DF8
	private void FadeFullscreenEffectsIn()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr == null)
		{
			Debug.LogWarning("AdventureCompleteReward: FullScreenFXMgr.Get() returned null!");
			return;
		}
		fullScreenFXMgr.SetBlurBrightness(0.85f);
		fullScreenFXMgr.SetBlurDesaturation(0f);
		fullScreenFXMgr.Vignette(0.4f, 0.5f, iTween.EaseType.easeOutCirc, null);
		fullScreenFXMgr.Blur(1f, 0.5f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x06005E8F RID: 24207 RVA: 0x001C4C60 File Offset: 0x001C2E60
	private void FadeFullscreenEffectsOut()
	{
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		if (fullScreenFXMgr == null)
		{
			Debug.LogWarning("AdventureCompleteReward: FullScreenFXMgr.Get() returned null!");
			return;
		}
		fullScreenFXMgr.StopVignette(1f, iTween.EaseType.easeOutCirc, new FullScreenFXMgr.EffectListener(this.DestroyThis));
		fullScreenFXMgr.StopBlur(1f, iTween.EaseType.easeOutCirc, null);
	}

	// Token: 0x06005E90 RID: 24208 RVA: 0x001C4CB4 File Offset: 0x001C2EB4
	protected override void OnDataSet(bool updateVisuals)
	{
		if (!updateVisuals)
		{
			return;
		}
		if (!(base.Data is AdventureCompleteRewardData))
		{
			Debug.LogWarning(string.Format("AdventureCompleteReward.OnDataSet() - Data {0} is not AdventureCompleteRewardData", base.Data));
			return;
		}
		base.EnableClickCatcher(true);
		base.RegisterClickListener(delegate(Reward reward, object userData)
		{
			this.HideReward();
		});
		base.SetReady(true);
	}

	// Token: 0x06005E91 RID: 24209 RVA: 0x001C4D11 File Offset: 0x001C2F11
	private void DestroyThis()
	{
		Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x0400461B RID: 17947
	private const string s_EventShowHurt = "ShowHurt";

	// Token: 0x0400461C RID: 17948
	private const string s_EventShowBadlyHurt = "ShowBadlyHurt";

	// Token: 0x0400461D RID: 17949
	private const string s_EventHide = "Hide";

	// Token: 0x0400461E RID: 17950
	[CustomEditField(Sections = "State Event Table")]
	public StateEventTable m_StateTable;

	// Token: 0x0400461F RID: 17951
	[CustomEditField(Sections = "Banner")]
	public UberText m_BannerTextObject;

	// Token: 0x04004620 RID: 17952
	[CustomEditField(Sections = "Banner")]
	public GameObject m_BannerObject;

	// Token: 0x04004621 RID: 17953
	[CustomEditField(Sections = "Banner")]
	public Vector3_MobileOverride m_BannerScaleOverride;
}
