using System;
using System.Collections;
using UnityEngine;

// Token: 0x020002A3 RID: 675
public class AdventureClassChallengeChestButton : PegUIElement
{
	// Token: 0x060024A8 RID: 9384 RVA: 0x000B3EF4 File Offset: 0x000B20F4
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		SoundManager.Get().LoadAndPlay("collection_manager_hero_mouse_over", base.gameObject);
		this.ShowHighlight(true);
		base.StartCoroutine(this.ShowRewardCard());
	}

	// Token: 0x060024A9 RID: 9385 RVA: 0x000B3F2A File Offset: 0x000B212A
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		this.ShowHighlight(false);
		this.HideRewardCard();
	}

	// Token: 0x060024AA RID: 9386 RVA: 0x000B3F3C File Offset: 0x000B213C
	public void Press()
	{
		SoundManager.Get().LoadAndPlay("collection_manager_hero_mouse_over", base.gameObject);
		this.Depress();
		this.ShowHighlight(true);
		base.StartCoroutine(this.ShowRewardCard());
	}

	// Token: 0x060024AB RID: 9387 RVA: 0x000B3F78 File Offset: 0x000B2178
	public void Release()
	{
		this.Raise();
		this.ShowHighlight(false);
		this.HideRewardCard();
	}

	// Token: 0x060024AC RID: 9388 RVA: 0x000B3F90 File Offset: 0x000B2190
	private void Raise()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_UpBone.localPosition,
			"time",
			0.1f,
			"easeType",
			iTween.EaseType.linear,
			"isLocal",
			true
		});
		iTween.MoveTo(this.m_RootObject, args);
	}

	// Token: 0x060024AD RID: 9389 RVA: 0x000B4008 File Offset: 0x000B2208
	private void Depress()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_DownBone.localPosition,
			"time",
			0.1f,
			"easeType",
			iTween.EaseType.linear,
			"isLocal",
			true
		});
		iTween.MoveTo(this.m_RootObject, args);
	}

	// Token: 0x060024AE RID: 9390 RVA: 0x000B4080 File Offset: 0x000B2280
	private void ShowHighlight(bool show)
	{
		this.m_HighlightPlane.GetComponent<Renderer>().enabled = show;
	}

	// Token: 0x060024AF RID: 9391 RVA: 0x000B4094 File Offset: 0x000B2294
	private IEnumerator ShowRewardCard()
	{
		while (this.m_IsRewardLoading)
		{
			yield return null;
		}
		SceneUtils.SetLayer(base.gameObject, GameLayer.IgnoreFullScreenEffects);
		FullScreenFXMgr fx = FullScreenFXMgr.Get();
		fx.SetBlurBrightness(1f);
		fx.SetBlurDesaturation(0f);
		fx.Vignette(0.4f, 0.2f, iTween.EaseType.easeOutCirc, null);
		fx.Blur(1f, 0.2f, iTween.EaseType.easeOutCirc, null);
		this.m_RewardBone.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		iTween.ScaleTo(this.m_RewardBone, new Vector3(10f, 10f, 10f), 0.2f);
		this.m_RewardCard.SetActive(true);
		yield break;
	}

	// Token: 0x060024B0 RID: 9392 RVA: 0x000B40B0 File Offset: 0x000B22B0
	private void HideRewardCard()
	{
		iTween.ScaleTo(this.m_RewardBone, new Vector3(0.1f, 0.1f, 0.1f), 0.2f);
		FullScreenFXMgr fullScreenFXMgr = FullScreenFXMgr.Get();
		fullScreenFXMgr.StopVignette(0.2f, iTween.EaseType.easeOutCirc, null);
		fullScreenFXMgr.StopBlur(0.2f, iTween.EaseType.easeOutCirc, new FullScreenFXMgr.EffectListener(this.EffectFadeOutFinished));
	}

	// Token: 0x060024B1 RID: 9393 RVA: 0x000B410E File Offset: 0x000B230E
	private void EffectFadeOutFinished()
	{
		SceneUtils.SetLayer(base.gameObject, GameLayer.Default);
		if (this.m_RewardCard != null)
		{
			this.m_RewardCard.SetActive(false);
		}
	}

	// Token: 0x040015AA RID: 5546
	public GameObject m_RootObject;

	// Token: 0x040015AB RID: 5547
	public Transform m_UpBone;

	// Token: 0x040015AC RID: 5548
	public Transform m_DownBone;

	// Token: 0x040015AD RID: 5549
	public GameObject m_HighlightPlane;

	// Token: 0x040015AE RID: 5550
	public GameObject m_RewardBone;

	// Token: 0x040015AF RID: 5551
	public GameObject m_RewardCard;

	// Token: 0x040015B0 RID: 5552
	public bool m_IsRewardLoading;
}
