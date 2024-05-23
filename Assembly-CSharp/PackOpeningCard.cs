using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000A90 RID: 2704
public class PackOpeningCard : MonoBehaviour
{
	// Token: 0x06005DF5 RID: 24053 RVA: 0x001C2539 File Offset: 0x001C0739
	private void Awake()
	{
		base.StartCoroutine("HackWaitThenDeactivateRarityInfo");
	}

	// Token: 0x06005DF6 RID: 24054 RVA: 0x001C2548 File Offset: 0x001C0748
	private IEnumerator HackWaitThenDeactivateRarityInfo()
	{
		yield return new WaitForEndOfFrame();
		yield return new WaitForEndOfFrame();
		yield break;
	}

	// Token: 0x06005DF7 RID: 24055 RVA: 0x001C255C File Offset: 0x001C075C
	public NetCache.BoosterCard GetCard()
	{
		return this.m_boosterCard;
	}

	// Token: 0x06005DF8 RID: 24056 RVA: 0x001C2564 File Offset: 0x001C0764
	public string GetCardId()
	{
		return (this.m_boosterCard != null) ? this.m_boosterCard.Def.Name : null;
	}

	// Token: 0x06005DF9 RID: 24057 RVA: 0x001C2587 File Offset: 0x001C0787
	public EntityDef GetEntityDef()
	{
		return this.m_entityDef;
	}

	// Token: 0x06005DFA RID: 24058 RVA: 0x001C258F File Offset: 0x001C078F
	public CardDef GetCardDef()
	{
		return this.m_cardDef;
	}

	// Token: 0x06005DFB RID: 24059 RVA: 0x001C2597 File Offset: 0x001C0797
	public Actor GetActor()
	{
		return this.m_actor;
	}

	// Token: 0x06005DFC RID: 24060 RVA: 0x001C25A0 File Offset: 0x001C07A0
	public void AttachBoosterCard(NetCache.BoosterCard boosterCard)
	{
		if (this.m_boosterCard == null && boosterCard == null)
		{
			return;
		}
		this.m_boosterCard = boosterCard;
		this.m_premium = this.m_boosterCard.Def.Premium;
		this.Destroy();
		if (this.m_boosterCard == null)
		{
			this.BecomeReady();
		}
		else
		{
			DefLoader.Get().LoadFullDef(this.m_boosterCard.Def.Name, new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
		}
	}

	// Token: 0x06005DFD RID: 24061 RVA: 0x001C261E File Offset: 0x001C081E
	public bool IsReady()
	{
		return this.m_ready;
	}

	// Token: 0x06005DFE RID: 24062 RVA: 0x001C2626 File Offset: 0x001C0826
	public bool IsRevealed()
	{
		return this.m_revealed;
	}

	// Token: 0x06005DFF RID: 24063 RVA: 0x001C2630 File Offset: 0x001C0830
	public void Destroy()
	{
		this.m_ready = false;
		if (this.m_actor != null)
		{
			this.m_actor.Destroy();
			this.m_actor = null;
		}
		this.m_rarityInfo = null;
		this.m_spell = null;
		this.m_revealButton = null;
		this.m_revealed = false;
	}

	// Token: 0x06005E00 RID: 24064 RVA: 0x001C2683 File Offset: 0x001C0883
	public bool IsInputEnabled()
	{
		return this.m_inputEnabled;
	}

	// Token: 0x06005E01 RID: 24065 RVA: 0x001C268B File Offset: 0x001C088B
	public void EnableInput(bool enable)
	{
		this.m_inputEnabled = enable;
		this.UpdateInput();
	}

	// Token: 0x06005E02 RID: 24066 RVA: 0x001C269A File Offset: 0x001C089A
	public bool IsRevealEnabled()
	{
		return this.m_revealEnabled;
	}

	// Token: 0x06005E03 RID: 24067 RVA: 0x001C26A2 File Offset: 0x001C08A2
	public void EnableReveal(bool enable)
	{
		this.m_revealEnabled = enable;
		this.UpdateActor();
	}

	// Token: 0x06005E04 RID: 24068 RVA: 0x001C26B1 File Offset: 0x001C08B1
	public void AddRevealedListener(PackOpeningCard.RevealedCallback callback)
	{
		this.AddRevealedListener(callback, null);
	}

	// Token: 0x06005E05 RID: 24069 RVA: 0x001C26BC File Offset: 0x001C08BC
	public void AddRevealedListener(PackOpeningCard.RevealedCallback callback, object userData)
	{
		PackOpeningCard.RevealedListener revealedListener = new PackOpeningCard.RevealedListener();
		revealedListener.SetCallback(callback);
		revealedListener.SetUserData(userData);
		this.m_revealedListeners.Add(revealedListener);
	}

	// Token: 0x06005E06 RID: 24070 RVA: 0x001C26E9 File Offset: 0x001C08E9
	public void RemoveRevealedListener(PackOpeningCard.RevealedCallback callback)
	{
		this.RemoveRevealedListener(callback, null);
	}

	// Token: 0x06005E07 RID: 24071 RVA: 0x001C26F4 File Offset: 0x001C08F4
	public void RemoveRevealedListener(PackOpeningCard.RevealedCallback callback, object userData)
	{
		PackOpeningCard.RevealedListener revealedListener = new PackOpeningCard.RevealedListener();
		revealedListener.SetCallback(callback);
		revealedListener.SetUserData(userData);
		this.m_revealedListeners.Remove(revealedListener);
	}

	// Token: 0x06005E08 RID: 24072 RVA: 0x001C2724 File Offset: 0x001C0924
	public void RemoveOnOverWhileFlippedListeners()
	{
		this.m_revealButton.RemoveEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnOverWhileFlipped));
		this.m_revealButton.RemoveEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnOutWhileFlipped));
	}

	// Token: 0x06005E09 RID: 24073 RVA: 0x001C2763 File Offset: 0x001C0963
	public void ForceReveal()
	{
		this.OnPress(null);
	}

	// Token: 0x06005E0A RID: 24074 RVA: 0x001C276C File Offset: 0x001C096C
	public void ShowRarityGlow()
	{
		if (this.IsRevealed())
		{
			return;
		}
		this.OnOver(null);
	}

	// Token: 0x06005E0B RID: 24075 RVA: 0x001C2781 File Offset: 0x001C0981
	public void HideRarityGlow()
	{
		if (this.IsRevealed())
		{
			return;
		}
		this.OnOut(null);
	}

	// Token: 0x06005E0C RID: 24076 RVA: 0x001C2798 File Offset: 0x001C0998
	private void OnFullDefLoaded(string cardId, FullDef fullDef, object userData)
	{
		if (fullDef == null)
		{
			this.BecomeReady();
			Debug.LogWarning(string.Format("PackOpeningCard.OnFullDefLoaded() - FAILED to load \"{0}\"", cardId));
			return;
		}
		this.m_entityDef = fullDef.GetEntityDef();
		this.m_cardDef = fullDef.GetCardDef();
		if (!this.DetermineRarityInfo())
		{
			this.BecomeReady();
			return;
		}
		string handActor = ActorNames.GetHandActor(this.m_entityDef, this.m_premium);
		AssetLoader.Get().LoadActor(handActor, new AssetLoader.GameObjectCallback(this.OnActorLoaded), null, false);
		if (Cheats.Get().IsYourMindFree())
		{
			CollectibleCard card = CollectionManager.Get().GetCard(this.m_entityDef.GetCardId(), this.m_premium);
			this.m_isNew = (card.SeenCount < 1 && card.OwnedCount < 2);
		}
	}

	// Token: 0x06005E0D RID: 24077 RVA: 0x001C2864 File Offset: 0x001C0A64
	private void OnActorLoaded(string name, GameObject actorObject, object userData)
	{
		if (actorObject == null)
		{
			this.BecomeReady();
			Debug.LogWarning(string.Format("PackOpeningCard.OnActorLoaded() - FAILED to load actor \"{0}\"", name));
			return;
		}
		Actor component = actorObject.GetComponent<Actor>();
		if (component == null)
		{
			this.BecomeReady();
			Debug.LogWarning(string.Format("PackOpeningCard.OnActorLoaded() - ERROR actor \"{0}\" has no Actor component", name));
			return;
		}
		this.m_actor = component;
		this.m_actor.TurnOffCollider();
		SceneUtils.SetLayer(component.gameObject, GameLayer.IgnoreFullScreenEffects);
		this.SetupActor();
		this.BecomeReady();
	}

	// Token: 0x06005E0E RID: 24078 RVA: 0x001C28EC File Offset: 0x001C0AEC
	private bool DetermineRarityInfo()
	{
		TAG_RARITY tag = (this.m_entityDef != null) ? this.m_entityDef.GetRarity() : TAG_RARITY.COMMON;
		PackOpeningRarity packOpeningRarity = GameUtils.GetPackOpeningRarity(tag);
		if (packOpeningRarity == PackOpeningRarity.NONE)
		{
			Debug.LogError(string.Format("PackOpeningCard.DetermineRarityInfo() - FAILED to determine rarity for {0}", this.GetCardId()));
			return false;
		}
		GameObject packOpeningCardEffects = SceneUtils.FindComponentInParents<PackOpening>(this).GetPackOpeningCardEffects();
		if (packOpeningCardEffects == null)
		{
			Debug.LogError("PackOpeningCard.DetermineRarityInfo() - Fail to get card effect from PackOpening");
			return false;
		}
		this.m_RarityInfos = packOpeningCardEffects.GetComponentsInChildren<PackOpeningCardRarityInfo>();
		if (this.m_RarityInfos == null)
		{
			Debug.LogError(string.Format("PackOpeningCard.DetermineRarityInfo() - {0} has no rarity info list. cardId={1}", this, this.GetCardId()));
			return false;
		}
		for (int i = 0; i < this.m_RarityInfos.Length; i++)
		{
			PackOpeningCardRarityInfo packOpeningCardRarityInfo = this.m_RarityInfos[i];
			if (packOpeningRarity == packOpeningCardRarityInfo.m_RarityType)
			{
				this.m_rarityInfo = packOpeningCardRarityInfo;
				this.SetupRarity();
				return true;
			}
		}
		Debug.LogError(string.Format("PackOpeningCard.DetermineRarityInfo() - {0} has no rarity info for {1}. cardId={2}", this, packOpeningRarity, this.GetCardId()));
		return false;
	}

	// Token: 0x06005E0F RID: 24079 RVA: 0x001C29EA File Offset: 0x001C0BEA
	private void SetupActor()
	{
		this.m_actor.SetEntityDef(this.m_entityDef);
		this.m_actor.SetCardDef(this.m_cardDef);
		this.m_actor.SetPremium(this.m_premium);
		this.m_actor.UpdateAllComponents();
	}

	// Token: 0x06005E10 RID: 24080 RVA: 0x001C2A2C File Offset: 0x001C0C2C
	private void UpdateActor()
	{
		if (this.m_actor == null)
		{
			return;
		}
		if (!this.IsRevealEnabled())
		{
			this.m_actor.Hide();
			return;
		}
		if (!this.IsRevealed())
		{
			this.m_actor.Hide();
		}
		Vector3 localScale = this.m_actor.transform.localScale;
		this.m_actor.transform.parent = this.m_rarityInfo.m_RevealedCardObject.transform;
		this.m_actor.transform.localPosition = Vector3.zero;
		this.m_actor.transform.localRotation = Quaternion.identity;
		this.m_actor.transform.localScale = localScale;
		if (this.m_isNew)
		{
			this.m_actor.SetActorState(ActorStateType.CARD_RECENTLY_ACQUIRED);
		}
	}

	// Token: 0x06005E11 RID: 24081 RVA: 0x001C2AFC File Offset: 0x001C0CFC
	private void SetupRarity()
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.m_rarityInfo.gameObject);
		if (gameObject == null)
		{
			return;
		}
		gameObject.transform.parent = this.m_CardParent.transform;
		this.m_rarityInfo = gameObject.GetComponent<PackOpeningCardRarityInfo>();
		this.m_rarityInfo.m_RarityObject.SetActive(true);
		this.m_rarityInfo.m_HiddenCardObject.SetActive(true);
		Vector3 localPosition = this.m_rarityInfo.m_HiddenCardObject.transform.localPosition;
		this.m_rarityInfo.m_HiddenCardObject.transform.parent = this.m_CardParent.transform;
		this.m_rarityInfo.m_HiddenCardObject.transform.localPosition = localPosition;
		this.m_rarityInfo.m_HiddenCardObject.transform.localRotation = Quaternion.identity;
		this.m_rarityInfo.m_HiddenCardObject.transform.localScale = new Vector3(7.646f, 7.646f, 7.646f);
		TransformUtil.AttachAndPreserveLocalTransform(this.m_rarityInfo.m_RarityObject.transform, this.m_CardParent.transform);
		this.m_spell = this.m_rarityInfo.m_RarityObject.GetComponent<Spell>();
		this.m_revealButton = this.m_rarityInfo.m_RarityObject.GetComponent<PegUIElement>();
		if (UniversalInputManager.Get().IsTouchMode())
		{
			this.m_revealButton.SetReceiveReleaseWithoutMouseDown(true);
		}
		this.m_SharedHiddenCardObject.transform.parent = this.m_rarityInfo.m_HiddenCardObject.transform;
		TransformUtil.Identity(this.m_SharedHiddenCardObject.transform);
	}

	// Token: 0x06005E12 RID: 24082 RVA: 0x001C2C94 File Offset: 0x001C0E94
	private void EnableRarityInfo(PackOpeningCardRarityInfo info, bool enable)
	{
		if (info.m_RarityObject != null)
		{
			info.m_RarityObject.SetActive(enable);
		}
		if (info.m_HiddenCardObject != null)
		{
			info.m_HiddenCardObject.SetActive(enable);
		}
	}

	// Token: 0x06005E13 RID: 24083 RVA: 0x001C2CDB File Offset: 0x001C0EDB
	private void OnOver(UIEvent e)
	{
		this.m_spell.ActivateState(SpellStateType.BIRTH);
	}

	// Token: 0x06005E14 RID: 24084 RVA: 0x001C2CE9 File Offset: 0x001C0EE9
	private void OnOut(UIEvent e)
	{
		this.m_spell.ActivateState(SpellStateType.CANCEL);
	}

	// Token: 0x06005E15 RID: 24085 RVA: 0x001C2CF8 File Offset: 0x001C0EF8
	private void OnOverWhileFlipped(UIEvent e)
	{
		if (this.m_isNew)
		{
			this.m_actor.SetActorState(ActorStateType.CARD_RECENTLY_ACQUIRED_MOUSE_OVER);
		}
		else
		{
			this.m_actor.SetActorState(ActorStateType.CARD_HISTORY);
		}
		KeywordHelpPanelManager.Get().UpdateKeywordHelpForPackOpening(this.m_actor.GetEntityDef(), this.m_actor);
	}

	// Token: 0x06005E16 RID: 24086 RVA: 0x001C2D4A File Offset: 0x001C0F4A
	private void OnOutWhileFlipped(UIEvent e)
	{
		if (this.m_isNew)
		{
			this.m_actor.SetActorState(ActorStateType.CARD_RECENTLY_ACQUIRED);
		}
		else
		{
			this.m_actor.SetActorState(ActorStateType.CARD_IDLE);
		}
		KeywordHelpPanelManager.Get().HideKeywordHelp();
	}

	// Token: 0x06005E17 RID: 24087 RVA: 0x001C2D7F File Offset: 0x001C0F7F
	private void OnPress(UIEvent e)
	{
		this.m_revealed = true;
		this.UpdateInput();
		this.m_spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnSpellFinished));
		this.m_spell.ActivateState(SpellStateType.ACTION);
		this.PlayCorrectSound();
	}

	// Token: 0x06005E18 RID: 24088 RVA: 0x001C2DB8 File Offset: 0x001C0FB8
	private void UpdateInput()
	{
		if (!this.IsReady())
		{
			return;
		}
		bool flag = !this.IsRevealed() && this.IsInputEnabled();
		if (this.m_revealButton != null && !UniversalInputManager.UsePhoneUI)
		{
			if (flag)
			{
				this.m_revealButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnOver));
				this.m_revealButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnOut));
				this.m_revealButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPress));
				PegUIElement pegUIElement = PegUI.Get().FindHitElement();
				if (pegUIElement == this.m_revealButton)
				{
					this.OnOver(null);
				}
			}
			else
			{
				this.m_revealButton.RemoveEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnOver));
				this.m_revealButton.RemoveEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnOut));
				this.m_revealButton.RemoveEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnPress));
			}
		}
	}

	// Token: 0x06005E19 RID: 24089 RVA: 0x001C2ECA File Offset: 0x001C10CA
	private void BecomeReady()
	{
		this.m_ready = true;
		this.UpdateInput();
		this.UpdateActor();
	}

	// Token: 0x06005E1A RID: 24090 RVA: 0x001C2EE0 File Offset: 0x001C10E0
	private void FireRevealedEvent()
	{
		PackOpeningCard.RevealedListener[] array = this.m_revealedListeners.ToArray();
		for (int i = 0; i < array.Length; i++)
		{
			array[i].Fire();
		}
	}

	// Token: 0x06005E1B RID: 24091 RVA: 0x001C2F18 File Offset: 0x001C1118
	private void OnSpellFinished(Spell spell, object userData)
	{
		this.FireRevealedEvent();
		this.UpdateInput();
		this.ShowClassName();
		this.ShowIsNew();
		this.m_revealButton.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnOverWhileFlipped));
		this.m_revealButton.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnOutWhileFlipped));
	}

	// Token: 0x06005E1C RID: 24092 RVA: 0x001C2F70 File Offset: 0x001C1170
	private void ShowClassName()
	{
		string className = this.GetClassName();
		UberText[] componentsInChildren = this.m_ClassNameSpell.GetComponentsInChildren<UberText>(true);
		foreach (UberText uberText in componentsInChildren)
		{
			uberText.Text = className;
		}
		this.m_ClassNameSpell.ActivateState(SpellStateType.BIRTH);
	}

	// Token: 0x06005E1D RID: 24093 RVA: 0x001C2FC4 File Offset: 0x001C11C4
	private void ShowIsNew()
	{
		if (this.m_isNew && this.m_IsNewSpell != null)
		{
			this.m_IsNewSpell.ActivateState(SpellStateType.BIRTH);
		}
	}

	// Token: 0x06005E1E RID: 24094 RVA: 0x001C2FFC File Offset: 0x001C11FC
	private string GetClassName()
	{
		TAG_CLASS @class = this.m_entityDef.GetClass();
		if (@class == TAG_CLASS.INVALID)
		{
			return GameStrings.Get("GLUE_PACK_OPENING_ALL_CLASSES");
		}
		return GameStrings.GetClassName(@class);
	}

	// Token: 0x06005E1F RID: 24095 RVA: 0x001C302C File Offset: 0x001C122C
	private void PlayCorrectSound()
	{
		switch (this.m_rarityInfo.m_RarityType)
		{
		case PackOpeningRarity.COMMON:
			if (this.m_premium == TAG_PREMIUM.GOLDEN)
			{
				SoundManager.Get().LoadAndPlay("VO_ANNOUNCER_FOIL_C_29");
			}
			break;
		case PackOpeningRarity.RARE:
			if (this.m_premium == TAG_PREMIUM.GOLDEN)
			{
				SoundManager.Get().LoadAndPlay("VO_ANNOUNCER_FOIL_R_30");
			}
			else
			{
				SoundManager.Get().LoadAndPlay("VO_ANNOUNCER_RARE_27");
			}
			break;
		case PackOpeningRarity.EPIC:
			if (this.m_premium == TAG_PREMIUM.GOLDEN)
			{
				SoundManager.Get().LoadAndPlay("VO_ANNOUNCER_FOIL_E_31");
			}
			else
			{
				SoundManager.Get().LoadAndPlay("VO_ANNOUNCER_EPIC_26");
			}
			break;
		case PackOpeningRarity.LEGENDARY:
			if (this.m_premium == TAG_PREMIUM.GOLDEN)
			{
				SoundManager.Get().LoadAndPlay("VO_ANNOUNCER_FOIL_L_32");
			}
			else
			{
				SoundManager.Get().LoadAndPlay("VO_ANNOUNCER_LEGENDARY_25");
			}
			break;
		}
	}

	// Token: 0x040045A2 RID: 17826
	private const TAG_RARITY FALLBACK_RARITY = TAG_RARITY.COMMON;

	// Token: 0x040045A3 RID: 17827
	public GameObject m_CardParent;

	// Token: 0x040045A4 RID: 17828
	public GameObject m_SharedHiddenCardObject;

	// Token: 0x040045A5 RID: 17829
	public Spell m_ClassNameSpell;

	// Token: 0x040045A6 RID: 17830
	public Spell m_IsNewSpell;

	// Token: 0x040045A7 RID: 17831
	private PackOpeningCardRarityInfo[] m_RarityInfos;

	// Token: 0x040045A8 RID: 17832
	private NetCache.BoosterCard m_boosterCard;

	// Token: 0x040045A9 RID: 17833
	private TAG_PREMIUM m_premium;

	// Token: 0x040045AA RID: 17834
	private EntityDef m_entityDef;

	// Token: 0x040045AB RID: 17835
	private CardDef m_cardDef;

	// Token: 0x040045AC RID: 17836
	private Actor m_actor;

	// Token: 0x040045AD RID: 17837
	private PackOpeningCardRarityInfo m_rarityInfo;

	// Token: 0x040045AE RID: 17838
	private Spell m_spell;

	// Token: 0x040045AF RID: 17839
	private PegUIElement m_revealButton;

	// Token: 0x040045B0 RID: 17840
	private bool m_ready;

	// Token: 0x040045B1 RID: 17841
	private bool m_inputEnabled;

	// Token: 0x040045B2 RID: 17842
	private bool m_revealEnabled;

	// Token: 0x040045B3 RID: 17843
	private bool m_revealed;

	// Token: 0x040045B4 RID: 17844
	private bool m_isNew;

	// Token: 0x040045B5 RID: 17845
	private List<PackOpeningCard.RevealedListener> m_revealedListeners = new List<PackOpeningCard.RevealedListener>();

	// Token: 0x02000A99 RID: 2713
	// (Invoke) Token: 0x06005E59 RID: 24153
	public delegate void RevealedCallback(object userData);

	// Token: 0x02000A9B RID: 2715
	private class RevealedListener : EventListener<PackOpeningCard.RevealedCallback>
	{
		// Token: 0x06005E5E RID: 24158 RVA: 0x001C3F1D File Offset: 0x001C211D
		public void Fire()
		{
			this.m_callback(this.m_userData);
		}
	}
}
