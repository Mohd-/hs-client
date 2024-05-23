using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004FC RID: 1276
public class EndGameTwoScoop : MonoBehaviour
{
	// Token: 0x06003B83 RID: 15235 RVA: 0x00120BC4 File Offset: 0x0011EDC4
	private void Awake()
	{
		base.gameObject.SetActive(false);
		AssetLoader.Get().LoadActor("Card_Play_Hero", new AssetLoader.GameObjectCallback(this.OnHeroActorLoaded), null, false);
	}

	// Token: 0x06003B84 RID: 15236 RVA: 0x00120BFB File Offset: 0x0011EDFB
	private void Start()
	{
		SceneUtils.SetLayer(base.gameObject, GameLayer.IgnoreFullScreenEffects);
		this.ResetPositions();
	}

	// Token: 0x06003B85 RID: 15237 RVA: 0x00120C10 File Offset: 0x0011EE10
	public bool IsShown()
	{
		return this.m_isShown;
	}

	// Token: 0x06003B86 RID: 15238 RVA: 0x00120C18 File Offset: 0x0011EE18
	public void Show()
	{
		this.m_isShown = true;
		base.gameObject.SetActive(true);
		this.ShowImpl();
		if (!GameMgr.Get().IsTutorial() && !GameMgr.Get().IsSpectator())
		{
			Entity startingHero = GameState.Get().GetFriendlySidePlayer().GetStartingHero();
			TAG_CLASS @class = startingHero.GetClass();
			NetCache.HeroLevel heroLevel = GameUtils.GetHeroLevel(@class);
			if (heroLevel == null)
			{
				this.HideXpBar();
			}
			else
			{
				this.m_xpBar = Object.Instantiate<HeroXPBar>(this.m_xpBarPrefab);
				this.m_xpBar.transform.parent = this.m_heroActor.transform;
				this.m_xpBar.transform.localScale = new Vector3(0.88f, 0.88f, 0.88f);
				this.m_xpBar.transform.localPosition = new Vector3(-0.1886583f, 0.2122119f, -0.7446293f);
				this.m_xpBar.m_soloLevelLimit = NetCache.Get().GetNetObject<NetCache.NetCacheRewardProgress>().XPSoloLimit;
				this.m_xpBar.m_isAnimated = true;
				this.m_xpBar.m_delay = EndGameTwoScoop.BAR_ANIMATION_DELAY;
				this.m_xpBar.m_heroLevel = heroLevel;
				this.m_xpBar.m_levelUpCallback = new HeroXPBar.PlayLevelUpEffectCallback(this.PlayLevelUpEffect);
				this.m_xpBar.UpdateDisplay();
			}
		}
	}

	// Token: 0x06003B87 RID: 15239 RVA: 0x00120D63 File Offset: 0x0011EF63
	public void Hide()
	{
		this.HideAll();
	}

	// Token: 0x06003B88 RID: 15240 RVA: 0x00120D6B File Offset: 0x0011EF6B
	public bool IsLoaded()
	{
		return this.m_heroActorLoaded;
	}

	// Token: 0x06003B89 RID: 15241 RVA: 0x00120D74 File Offset: 0x0011EF74
	public void HideXpBar()
	{
		if (this.m_xpBar != null)
		{
			this.m_xpBar.gameObject.SetActive(false);
		}
	}

	// Token: 0x06003B8A RID: 15242 RVA: 0x00120DA3 File Offset: 0x0011EFA3
	public virtual void StopAnimating()
	{
	}

	// Token: 0x06003B8B RID: 15243 RVA: 0x00120DA5 File Offset: 0x0011EFA5
	protected virtual void ShowImpl()
	{
	}

	// Token: 0x06003B8C RID: 15244 RVA: 0x00120DA7 File Offset: 0x0011EFA7
	protected virtual void ResetPositions()
	{
	}

	// Token: 0x06003B8D RID: 15245 RVA: 0x00120DA9 File Offset: 0x0011EFA9
	protected void SaveBannerText(string bannerText)
	{
		this.m_bannerText = bannerText;
	}

	// Token: 0x06003B8E RID: 15246 RVA: 0x00120DB2 File Offset: 0x0011EFB2
	protected void SetBannerLabel(string label)
	{
		this.m_bannerLabel.Text = label;
	}

	// Token: 0x06003B8F RID: 15247 RVA: 0x00120DC0 File Offset: 0x0011EFC0
	protected void EnableBannerLabel(bool enable)
	{
		this.m_bannerLabel.gameObject.SetActive(enable);
	}

	// Token: 0x06003B90 RID: 15248 RVA: 0x00120DD4 File Offset: 0x0011EFD4
	protected void PunchEndGameTwoScoop()
	{
		EndGameScreen.Get().NotifyOfAnimComplete();
		iTween.ScaleTo(base.gameObject, new Vector3(EndGameTwoScoop.AFTER_PUNCH_SCALE_VAL, EndGameTwoScoop.AFTER_PUNCH_SCALE_VAL, EndGameTwoScoop.AFTER_PUNCH_SCALE_VAL), 0.15f);
	}

	// Token: 0x06003B91 RID: 15249 RVA: 0x00120E10 File Offset: 0x0011F010
	private void HideAll()
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"scale",
			new Vector3(EndGameTwoScoop.START_SCALE_VAL, EndGameTwoScoop.START_SCALE_VAL, EndGameTwoScoop.START_SCALE_VAL),
			"time",
			0.25f,
			"oncomplete",
			"OnAllHidden",
			"oncompletetarget",
			base.gameObject
		});
		iTween.FadeTo(base.gameObject, 0f, 0.25f);
		iTween.ScaleTo(base.gameObject, args);
		this.m_isShown = false;
	}

	// Token: 0x06003B92 RID: 15250 RVA: 0x00120EAC File Offset: 0x0011F0AC
	private void OnAllHidden()
	{
		iTween.FadeTo(base.gameObject, 0f, 0f);
		base.gameObject.SetActive(false);
		this.ResetPositions();
	}

	// Token: 0x06003B93 RID: 15251 RVA: 0x00120EE0 File Offset: 0x0011F0E0
	private void OnHeroActorLoaded(string name, GameObject go, object callbackData)
	{
		go.transform.parent = base.transform;
		go.transform.localPosition = this.m_heroBone.transform.localPosition;
		go.transform.localScale = this.m_heroBone.transform.localScale;
		this.m_heroActor = go.GetComponent<Actor>();
		this.m_heroActor.TurnOffCollider();
		this.m_heroActor.m_healthObject.SetActive(false);
		this.m_heroActorLoaded = true;
		this.m_heroActor.SetPremium(GameState.Get().GetFriendlySidePlayer().GetHeroCard().GetPremium());
		this.m_heroActor.UpdateAllComponents();
	}

	// Token: 0x06003B94 RID: 15252 RVA: 0x00120F90 File Offset: 0x0011F190
	protected void PlayLevelUpEffect()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.m_levelUpTier1);
		if (gameObject)
		{
			gameObject.transform.parent = base.transform;
			gameObject.GetComponent<PlayMakerFSM>().SendEvent("Birth");
		}
	}

	// Token: 0x0400260E RID: 9742
	public UberText m_bannerLabel;

	// Token: 0x0400260F RID: 9743
	public GameObject m_heroBone;

	// Token: 0x04002610 RID: 9744
	public Actor m_heroActor;

	// Token: 0x04002611 RID: 9745
	public HeroXPBar m_xpBarPrefab;

	// Token: 0x04002612 RID: 9746
	public GameObject m_levelUpTier1;

	// Token: 0x04002613 RID: 9747
	public GameObject m_levelUpTier2;

	// Token: 0x04002614 RID: 9748
	public GameObject m_levelUpTier3;

	// Token: 0x04002615 RID: 9749
	protected string m_bannerText;

	// Token: 0x04002616 RID: 9750
	protected bool m_heroActorLoaded;

	// Token: 0x04002617 RID: 9751
	protected HeroXPBar m_xpBar;

	// Token: 0x04002618 RID: 9752
	private bool m_isShown;

	// Token: 0x04002619 RID: 9753
	private static readonly float AFTER_PUNCH_SCALE_VAL = 2.3f;

	// Token: 0x0400261A RID: 9754
	protected static readonly float START_SCALE_VAL = 0.01f;

	// Token: 0x0400261B RID: 9755
	protected static readonly float END_SCALE_VAL = 2.5f;

	// Token: 0x0400261C RID: 9756
	protected static readonly Vector3 START_POSITION = new Vector3(-7.8f, 8.2f, -5f);

	// Token: 0x0400261D RID: 9757
	protected static readonly float BAR_ANIMATION_DELAY = 1f;
}
