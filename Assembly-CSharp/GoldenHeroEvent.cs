using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200087E RID: 2174
public class GoldenHeroEvent : MonoBehaviour
{
	// Token: 0x06005304 RID: 21252 RVA: 0x0018BDE9 File Offset: 0x00189FE9
	private void Awake()
	{
		this.LoadVanillaHeroCardDef();
	}

	// Token: 0x06005305 RID: 21253 RVA: 0x0018BDF4 File Offset: 0x00189FF4
	public void Show()
	{
		base.gameObject.SetActive(true);
		this.m_playmaker.SendEvent("Action");
		this.m_victoryTwoScoop.HideXpBar();
		this.m_victoryTwoScoop.m_bannerLabel.Text = string.Empty;
	}

	// Token: 0x06005306 RID: 21254 RVA: 0x0018BE3D File Offset: 0x0018A03D
	public void Hide()
	{
		this.m_playmaker.SendEvent("Done");
		SoundManager.Get().LoadAndPlay("rank_window_shrink");
	}

	// Token: 0x06005307 RID: 21255 RVA: 0x0018BE5E File Offset: 0x0018A05E
	public void SetHeroBurnAwayTexture(Texture heroTexture)
	{
		this.m_burningHero.GetComponent<Renderer>().material.mainTexture = heroTexture;
	}

	// Token: 0x06005308 RID: 21256 RVA: 0x0018BE76 File Offset: 0x0018A076
	public void HideTwoScoop()
	{
		this.m_victoryTwoScoop.Hide();
	}

	// Token: 0x06005309 RID: 21257 RVA: 0x0018BE83 File Offset: 0x0018A083
	public void SetVictoryTwoScoop(VictoryTwoScoop twoScoop)
	{
		this.m_victoryTwoScoop = twoScoop;
	}

	// Token: 0x0600530A RID: 21258 RVA: 0x0018BE8C File Offset: 0x0018A08C
	public void SwapHeroToVanilla()
	{
		if (this.m_VanillaHeroCardDef == null)
		{
			return;
		}
		this.m_victoryTwoScoop.m_heroActor.SetCardDef(this.m_VanillaHeroCardDef);
		this.m_victoryTwoScoop.m_heroActor.UpdateAllComponents();
	}

	// Token: 0x0600530B RID: 21259 RVA: 0x0018BED1 File Offset: 0x0018A0D1
	public void SwapMaterialToPremium()
	{
		this.m_victoryTwoScoop.m_heroActor.SetPremium(TAG_PREMIUM.GOLDEN);
		this.m_victoryTwoScoop.m_heroActor.UpdateAllComponents();
	}

	// Token: 0x0600530C RID: 21260 RVA: 0x0018BEF4 File Offset: 0x0018A0F4
	public void AnimationDone()
	{
		this.FireAnimationDoneEvent();
	}

	// Token: 0x0600530D RID: 21261 RVA: 0x0018BEFC File Offset: 0x0018A0FC
	private void FireAnimationDoneEvent()
	{
		GoldenHeroEvent.AnimationDoneListener[] array = this.m_animationDoneListeners.ToArray();
		foreach (GoldenHeroEvent.AnimationDoneListener animationDoneListener in array)
		{
			animationDoneListener();
		}
	}

	// Token: 0x0600530E RID: 21262 RVA: 0x0018BF35 File Offset: 0x0018A135
	public void RegisterAnimationDoneListener(GoldenHeroEvent.AnimationDoneListener listener)
	{
		if (this.m_animationDoneListeners.Contains(listener))
		{
			return;
		}
		this.m_animationDoneListeners.Add(listener);
	}

	// Token: 0x0600530F RID: 21263 RVA: 0x0018BF55 File Offset: 0x0018A155
	public void RemoveAnimationDoneListener(GoldenHeroEvent.AnimationDoneListener listener)
	{
		this.m_animationDoneListeners.Remove(listener);
	}

	// Token: 0x06005310 RID: 21264 RVA: 0x0018BF64 File Offset: 0x0018A164
	private void LoadVanillaHeroCardDef()
	{
		Player player = null;
		Map<int, Player> playerMap = GameState.Get().GetPlayerMap();
		foreach (Player player2 in playerMap.Values)
		{
			if (player2.GetSide() == Player.Side.FRIENDLY)
			{
				player = player2;
				break;
			}
		}
		if (player == null)
		{
			Debug.LogWarning("GoldenHeroEvent.LoadVanillaHeroCardDef() - currentPlayer == null");
			return;
		}
		EntityDef entityDef = player.GetEntityDef();
		TAG_CARD_SET cardSet = entityDef.GetCardSet();
		if (cardSet != TAG_CARD_SET.HERO_SKINS)
		{
			return;
		}
		string vanillaHeroCardID = CollectionManager.Get().GetVanillaHeroCardID(entityDef);
		CardPortraitQuality quality = new CardPortraitQuality(3, TAG_PREMIUM.NORMAL);
		DefLoader.Get().LoadCardDef(vanillaHeroCardID, new DefLoader.LoadDefCallback<CardDef>(this.OnVanillaHeroCardDefLoaded), new object(), quality);
	}

	// Token: 0x06005311 RID: 21265 RVA: 0x0018C03C File Offset: 0x0018A23C
	private void OnVanillaHeroCardDefLoaded(string cardId, CardDef def, object userData)
	{
		if (def == null)
		{
			Debug.LogError("GoldenHeroEvent.LoadDefaultHeroTexture() faild to load CardDef!");
			return;
		}
		this.m_VanillaHeroCardDef = def;
	}

	// Token: 0x0400393B RID: 14651
	public PlayMakerFSM m_playmaker;

	// Token: 0x0400393C RID: 14652
	public Transform m_heroBone;

	// Token: 0x0400393D RID: 14653
	public GameObject m_burningHero;

	// Token: 0x0400393E RID: 14654
	private VictoryTwoScoop m_victoryTwoScoop;

	// Token: 0x0400393F RID: 14655
	private List<GoldenHeroEvent.AnimationDoneListener> m_animationDoneListeners = new List<GoldenHeroEvent.AnimationDoneListener>();

	// Token: 0x04003940 RID: 14656
	private CardDef m_VanillaHeroCardDef;

	// Token: 0x0200087F RID: 2175
	// (Invoke) Token: 0x06005313 RID: 21267
	public delegate void AnimationDoneListener();
}
