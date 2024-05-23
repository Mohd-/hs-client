using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020005BD RID: 1469
public class CraftingUI : MonoBehaviour
{
	// Token: 0x06004180 RID: 16768 RVA: 0x0013BBBC File Offset: 0x00139DBC
	private void Update()
	{
		if (!this.m_enabled)
		{
			return;
		}
		if (this.m_isAnimating)
		{
			this.m_mousedOver = false;
			return;
		}
		Ray ray = Camera.main.ScreenPointToRay(UniversalInputManager.Get().GetMousePosition());
		LayerMask layerMask = 512;
		RaycastHit raycastHit;
		if (!Physics.Raycast(ray, ref raycastHit, Camera.main.farClipPlane, layerMask))
		{
			return;
		}
		if (raycastHit.collider == this.m_mouseOverCollider)
		{
			this.NotifyOfMouseOver();
		}
		else
		{
			this.NotifyOfMouseOut();
		}
	}

	// Token: 0x06004181 RID: 16769 RVA: 0x0013BC50 File Offset: 0x00139E50
	public void UpdateWildTheming()
	{
		if (this.m_wildTheming == null)
		{
			return;
		}
		EntityDef def;
		TAG_PREMIUM tag_PREMIUM;
		if (!CraftingManager.Get().GetShownCardInfo(out def, out tag_PREMIUM))
		{
			this.m_wildTheming.SetActive(false);
			return;
		}
		this.m_wildTheming.SetActive(GameUtils.IsCardRotated(def));
	}

	// Token: 0x06004182 RID: 16770 RVA: 0x0013BCA0 File Offset: 0x00139EA0
	public void UpdateText()
	{
		this.UpdateBankText();
		EntityDef entityDef;
		TAG_PREMIUM premium;
		if (!CraftingManager.Get().GetShownCardInfo(out entityDef, out premium))
		{
			this.m_buttonDisenchant.DisableButton();
			this.m_buttonCreate.DisableButton();
			return;
		}
		NetCache.CardDefinition cardDefinition = new NetCache.CardDefinition
		{
			Name = entityDef.GetCardId(),
			Premium = premium
		};
		int numOwnedCopies = CraftingManager.Get().GetNumOwnedCopies(cardDefinition.Name, cardDefinition.Premium);
		string text = string.Empty;
		string text2 = string.Empty;
		TAG_CARD_SET cardSet = entityDef.GetCardSet();
		string cardSetName = GameStrings.GetCardSetName(cardSet);
		NetCache.CardValue cardValue = CraftingManager.Get().GetCardValue(cardDefinition.Name, cardDefinition.Premium);
		text = GameStrings.Get("GLUE_CRAFTING_SOULBOUND");
		if (numOwnedCopies <= 0)
		{
			text = cardSetName;
			text2 = entityDef.GetHowToEarnText(cardDefinition.Premium);
		}
		else if (cardSet == TAG_CARD_SET.CORE)
		{
			text2 = GameStrings.Get("GLUE_CRAFTING_SOULBOUND_BASIC_DESC");
		}
		else if (cardSet == TAG_CARD_SET.REWARD || cardSet == TAG_CARD_SET.PROMO)
		{
			text2 = GameStrings.Get("GLUE_CRAFTING_SOULBOUND_REWARD_DESC");
		}
		else
		{
			text2 = GameStrings.Get("GLUE_CRAFTING_SOULBOUND_DESC");
		}
		bool flag;
		if (cardValue == null)
		{
			flag = false;
		}
		else if (this.IsCraftingEventForCardActive(cardDefinition.Name))
		{
			int numTransactions = CraftingManager.Get().GetNumTransactions();
			int num = cardValue.Buy;
			if (numTransactions < 0)
			{
				num = cardValue.Sell;
			}
			int num2 = cardValue.Sell;
			if (numTransactions > 0)
			{
				num2 = cardValue.Buy;
			}
			this.m_disenchantValue.Text = "+" + num2.ToString();
			this.m_craftValue.Text = "-" + num.ToString();
			flag = true;
		}
		else
		{
			text = GameStrings.Get("GLUE_CRAFTING_EVENT_NOT_ACTIVE_TITLE");
			text2 = GameStrings.Format("GLUE_CRAFTING_EVENT_NOT_ACTIVE_DESCRIPTION", new object[]
			{
				cardSetName
			});
			flag = false;
		}
		this.m_soulboundTitle.Text = text;
		this.m_soulboundDesc.Text = text2;
		if (!flag)
		{
			this.m_buttonDisenchant.DisableButton();
			this.m_buttonCreate.DisableButton();
			this.m_soulboundNotification.SetActive(true);
			this.m_activeObject = this.m_soulboundNotification;
			return;
		}
		if (!FixedRewardsMgr.Get().CanCraftCard(cardDefinition.Name, cardDefinition.Premium))
		{
			this.m_buttonDisenchant.DisableButton();
			this.m_buttonCreate.DisableButton();
			this.m_soulboundNotification.SetActive(true);
			this.m_activeObject = this.m_soulboundNotification;
			return;
		}
		this.m_soulboundNotification.SetActive(false);
		this.m_activeObject = base.gameObject;
		if (numOwnedCopies <= 0)
		{
			this.m_buttonDisenchant.DisableButton();
		}
		else
		{
			this.m_buttonDisenchant.EnableButton();
		}
		int num3 = (!entityDef.IsElite()) ? 2 : 1;
		long localArcaneDustBalance = CraftingManager.Get().GetLocalArcaneDustBalance();
		if (numOwnedCopies >= num3 || localArcaneDustBalance < (long)this.GetCardBuyValue(cardDefinition.Name, cardDefinition.Premium))
		{
			this.m_buttonCreate.DisableButton();
		}
		else
		{
			this.m_buttonCreate.EnableButton();
		}
	}

	// Token: 0x06004183 RID: 16771 RVA: 0x0013BFB8 File Offset: 0x0013A1B8
	public void DoDisenchant()
	{
		this.UpdateTips();
		Options.Get().SetBool(Option.HAS_DISENCHANTED, true);
		CraftingManager.Get().AdjustLocalArcaneDustBalance(this.GetCardSellValue(CraftingManager.Get().GetShownActor().GetEntityDef().GetCardId(), CraftingManager.Get().GetShownActor().GetPremium()));
		CraftingManager.Get().NotifyOfTransaction(-1);
		this.UpdateText();
		if (this.m_isAnimating)
		{
			CraftingManager.Get().FinishFlipCurrentActorEarly();
		}
		this.StopCurrentAnim();
		base.StartCoroutine(this.DoDisenchantAnims());
		CraftingManager.Get().StartCoroutine(this.StartCraftCooldown());
	}

	// Token: 0x06004184 RID: 16772 RVA: 0x0013C058 File Offset: 0x0013A258
	public void CleanUpEffects()
	{
		if (this.m_explodingActor != null)
		{
			Spell spell = this.m_explodingActor.GetSpell(SpellType.DECONSTRUCT);
			if (spell != null && spell.GetActiveState() != SpellStateType.NONE)
			{
				this.m_explodingActor.GetSpell(SpellType.DECONSTRUCT).GetComponent<PlayMakerFSM>().SendEvent("Cancel");
				this.m_explodingActor.Hide();
			}
		}
		if (this.m_constructingActor != null)
		{
			Spell spell2 = this.m_constructingActor.GetSpell(SpellType.CONSTRUCT);
			if (spell2 != null && spell2.GetActiveState() != SpellStateType.NONE)
			{
				this.m_constructingActor.GetSpell(SpellType.CONSTRUCT).GetComponent<PlayMakerFSM>().SendEvent("Cancel");
				this.m_constructingActor.Hide();
			}
		}
		base.GetComponent<PlayMakerFSM>().SendEvent("Cancel");
		this.m_isAnimating = false;
	}

	// Token: 0x06004185 RID: 16773 RVA: 0x0013C138 File Offset: 0x0013A338
	public void DoCreate()
	{
		this.UpdateTips();
		if (!Options.Get().GetBool(Option.HAS_CRAFTED))
		{
			Options.Get().SetBool(Option.HAS_CRAFTED, true);
		}
		string cardId = CraftingManager.Get().GetShownActor().GetEntityDef().GetCardId();
		TAG_PREMIUM premium = CraftingManager.Get().GetShownActor().GetPremium();
		CraftingManager.Get().AdjustLocalArcaneDustBalance(-this.GetCardBuyValue(cardId, premium));
		CraftingManager.Get().NotifyOfTransaction(1);
		if (CraftingManager.Get().GetNumOwnedCopies(cardId, premium) > 1)
		{
			CraftingManager.Get().ForceNonGhostFlagOn();
		}
		this.UpdateText();
		this.StopCurrentAnim();
		base.StartCoroutine(this.DoCreateAnims());
		CraftingManager.Get().StartCoroutine(this.StartDisenchantCooldown());
	}

	// Token: 0x06004186 RID: 16774 RVA: 0x0013C1F4 File Offset: 0x0013A3F4
	public void UpdateBankText()
	{
		long localArcaneDustBalance = CraftingManager.Get().GetLocalArcaneDustBalance();
		this.m_bankAmountText.Text = localArcaneDustBalance.ToString();
		BnetBar.Get().m_currencyFrame.RefreshContents();
		if (UniversalInputManager.UsePhoneUI && CraftingTray.Get() != null)
		{
			ArcaneDustAmount.Get().UpdateCurrentDustAmount();
		}
	}

	// Token: 0x06004187 RID: 16775 RVA: 0x0013C258 File Offset: 0x0013A458
	public void Disable(Vector3 hidePosition)
	{
		this.m_enabled = false;
		iTween.MoveTo(this.m_activeObject, iTween.Hash(new object[]
		{
			"time",
			0.4f,
			"position",
			hidePosition
		}));
		this.HideTips();
	}

	// Token: 0x06004188 RID: 16776 RVA: 0x0013C2AE File Offset: 0x0013A4AE
	public bool IsEnabled()
	{
		return this.m_enabled;
	}

	// Token: 0x06004189 RID: 16777 RVA: 0x0013C2B8 File Offset: 0x0013A4B8
	public void Enable(Vector3 showPosition, Vector3 hidePosition)
	{
		if (!this.m_initializedPositions)
		{
			base.transform.position = hidePosition;
			this.m_soulboundNotification.transform.position = base.transform.position;
			this.m_soulboundTitle.Text = GameStrings.Get("GLUE_CRAFTING_SOULBOUND");
			this.m_soulboundDesc.Text = GameStrings.Get("GLUE_CRAFTING_SOULBOUND_DESC");
			this.m_activeObject = base.gameObject;
			this.m_initializedPositions = true;
		}
		this.m_enabled = true;
		this.UpdateText();
		this.UpdateWildTheming();
		this.m_activeObject.SetActive(true);
		iTween.MoveTo(this.m_activeObject, iTween.Hash(new object[]
		{
			"time",
			0.5f,
			"position",
			showPosition
		}));
		this.ShowFirstTimeTips();
	}

	// Token: 0x0600418A RID: 16778 RVA: 0x0013C395 File Offset: 0x0013A595
	public void SetStartingActive()
	{
		this.m_soulboundNotification.SetActive(false);
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600418B RID: 16779 RVA: 0x0013C3B0 File Offset: 0x0013A5B0
	private void ShowFirstTimeTips()
	{
		if (this.m_activeObject == this.m_soulboundNotification)
		{
			return;
		}
		if (Options.Get().GetBool(Option.HAS_CRAFTED) || !UserAttentionManager.CanShowAttentionGrabber("CraftingUI.ShowFirstTimeTips"))
		{
			return;
		}
		this.CreateDisenchantNotification();
		this.CreateCraftNotification();
	}

	// Token: 0x0600418C RID: 16780 RVA: 0x0013C401 File Offset: 0x0013A601
	private void CreateDisenchantNotification()
	{
		if (!this.m_buttonDisenchant.IsButtonEnabled())
		{
			return;
		}
	}

	// Token: 0x0600418D RID: 16781 RVA: 0x0013C414 File Offset: 0x0013A614
	private void CreateCraftNotification()
	{
		if (!this.m_buttonCreate.IsButtonEnabled())
		{
			return;
		}
		Vector3 position;
		Notification.PopUpArrowDirection direction;
		if (UniversalInputManager.UsePhoneUI)
		{
			position..ctor(73.3f, 1f, 55.4f);
			direction = Notification.PopUpArrowDirection.Down;
		}
		else
		{
			position..ctor(55f, 1f, -56f);
			direction = Notification.PopUpArrowDirection.Left;
		}
		this.m_craftNotification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, position, 16f * Vector3.one, GameStrings.Get("GLUE_COLLECTION_TUTORIAL06"), false);
		if (this.m_craftNotification != null)
		{
			this.m_craftNotification.ShowPopUpArrow(direction);
		}
	}

	// Token: 0x0600418E RID: 16782 RVA: 0x0013C4C0 File Offset: 0x0013A6C0
	private void UpdateTips()
	{
		if (Options.Get().GetBool(Option.HAS_CRAFTED) || !UserAttentionManager.CanShowAttentionGrabber("CraftingUI.UpdateTips"))
		{
			this.HideTips();
			return;
		}
		if (this.m_craftNotification == null)
		{
			this.CreateCraftNotification();
		}
		else if (!this.m_buttonCreate.IsButtonEnabled())
		{
			NotificationManager.Get().DestroyNotification(this.m_craftNotification, 0f);
		}
	}

	// Token: 0x0600418F RID: 16783 RVA: 0x0013C535 File Offset: 0x0013A735
	private void HideTips()
	{
		if (this.m_craftNotification != null)
		{
			NotificationManager.Get().DestroyNotificationNowWithNoAnim(this.m_craftNotification);
		}
	}

	// Token: 0x06004190 RID: 16784 RVA: 0x0013C558 File Offset: 0x0013A758
	private void NotifyOfMouseOver()
	{
		if (this.m_mousedOver)
		{
			return;
		}
		this.m_mousedOver = true;
		base.GetComponent<PlayMakerFSM>().SendEvent("Idle");
	}

	// Token: 0x06004191 RID: 16785 RVA: 0x0013C57D File Offset: 0x0013A77D
	private void NotifyOfMouseOut()
	{
		if (!this.m_mousedOver)
		{
			return;
		}
		this.m_mousedOver = false;
		base.GetComponent<PlayMakerFSM>().SendEvent("IdleCancel");
	}

	// Token: 0x06004192 RID: 16786 RVA: 0x0013C5A4 File Offset: 0x0013A7A4
	private int GetCardBuyValue(string cardID, TAG_PREMIUM premium)
	{
		NetCache.CardValue cardValue = CraftingManager.Get().GetCardValue(cardID, premium);
		int numTransactions = CraftingManager.Get().GetNumTransactions();
		if (numTransactions >= 0)
		{
			return cardValue.Buy;
		}
		return cardValue.Sell;
	}

	// Token: 0x06004193 RID: 16787 RVA: 0x0013C5E0 File Offset: 0x0013A7E0
	private int GetCardSellValue(string cardID, TAG_PREMIUM premium)
	{
		NetCache.CardValue cardValue = CraftingManager.Get().GetCardValue(cardID, premium);
		int numTransactions = CraftingManager.Get().GetNumTransactions();
		if (numTransactions <= 0)
		{
			return cardValue.Sell;
		}
		return cardValue.Buy;
	}

	// Token: 0x06004194 RID: 16788 RVA: 0x0013C61C File Offset: 0x0013A81C
	private bool IsCraftingEventForCardActive(string cardID)
	{
		CardDbfRecord cardRecord = GameUtils.GetCardRecord(cardID);
		if (cardRecord == null)
		{
			Debug.LogWarning(string.Format("CraftingUI.IsCraftingEventForCardActive could not find DBF record for card {0}, assuming it cannot be crafted or disenchanted", cardID));
			return false;
		}
		string craftingEvent = cardRecord.CraftingEvent;
		return SpecialEventManager.Get().IsEventActive(craftingEvent, true);
	}

	// Token: 0x06004195 RID: 16789 RVA: 0x0013C65C File Offset: 0x0013A85C
	private void StopCurrentAnim()
	{
		if (!this.m_isAnimating)
		{
			return;
		}
		base.StopAllCoroutines();
		this.CleanUpEffects();
		foreach (GameObject gameObject in this.m_thingsToDestroy)
		{
			if (!(gameObject == null))
			{
				Object.Destroy(gameObject);
			}
		}
	}

	// Token: 0x06004196 RID: 16790 RVA: 0x0013C6E0 File Offset: 0x0013A8E0
	private IEnumerator StartDisenchantCooldown()
	{
		if (!this.m_buttonDisenchant.GetComponent<Collider>().enabled)
		{
			yield break;
		}
		this.m_buttonDisenchant.GetComponent<Collider>().enabled = false;
		yield return new WaitForSeconds(1f);
		this.m_buttonDisenchant.GetComponent<Collider>().enabled = true;
		yield break;
	}

	// Token: 0x06004197 RID: 16791 RVA: 0x0013C6FC File Offset: 0x0013A8FC
	private IEnumerator StartCraftCooldown()
	{
		if (!this.m_buttonCreate.GetComponent<Collider>().enabled)
		{
			yield break;
		}
		this.m_buttonCreate.GetComponent<Collider>().enabled = false;
		yield return new WaitForSeconds(1f);
		this.m_buttonCreate.GetComponent<Collider>().enabled = true;
		yield break;
	}

	// Token: 0x06004198 RID: 16792 RVA: 0x0013C718 File Offset: 0x0013A918
	private IEnumerator DoDisenchantAnims()
	{
		SoundManager.Get().Play(this.m_disenchantSound.GetComponent<AudioSource>());
		SoundManager.Get().Stop(this.m_craftingSound.GetComponent<AudioSource>());
		this.m_isAnimating = true;
		PlayMakerFSM playmaker = base.GetComponent<PlayMakerFSM>();
		playmaker.SendEvent("Birth");
		yield return new WaitForSeconds(this.m_disenchantDelayBeforeCardExplodes);
		while (CraftingManager.Get().GetShownActor() == null)
		{
			yield return null;
		}
		this.m_explodingActor = CraftingManager.Get().GetShownActor();
		this.UpdateBankText();
		if (CraftingManager.Get().IsCancelling())
		{
			yield break;
		}
		CraftingManager.Get().LoadGhostActorIfNecessary();
		this.m_explodingActor.ActivateSpell(SpellType.DECONSTRUCT);
		yield return new WaitForSeconds(this.m_disenchantDelayBeforeCardFlips);
		if (CraftingManager.Get().IsCancelling())
		{
			yield break;
		}
		CraftingManager.Get().FlipUpsideDownCard(this.m_explodingActor);
		Actor oldActor = this.m_explodingActor;
		this.m_thingsToDestroy.Add(this.m_explodingActor.gameObject);
		yield return new WaitForSeconds(this.m_disenchantDelayBeforeBallsComeOut);
		if (CraftingManager.Get().IsCancelling())
		{
			yield break;
		}
		playmaker.SendEvent("Action");
		yield return new WaitForSeconds(1f);
		this.m_isAnimating = false;
		yield return new WaitForSeconds(10f);
		if (oldActor != null)
		{
			Object.Destroy(oldActor.gameObject);
		}
		yield break;
	}

	// Token: 0x06004199 RID: 16793 RVA: 0x0013C734 File Offset: 0x0013A934
	private IEnumerator DoCreateAnims()
	{
		CardDef cardDef = CraftingManager.Get().GetShownActor().GetCardDef();
		SoundManager.Get().Play(this.m_craftingSound.GetComponent<AudioSource>());
		SoundManager.Get().Stop(this.m_disenchantSound.GetComponent<AudioSource>());
		this.m_isAnimating = true;
		CraftingManager.Get().FlipCurrentActor();
		PlayMakerFSM playmaker = base.GetComponent<PlayMakerFSM>();
		playmaker.SendEvent("Birth");
		yield return new WaitForSeconds(this.m_craftDelayBeforeConstructSpell);
		if (CraftingManager.Get().IsCancelling())
		{
			yield break;
		}
		this.m_constructingActor = CraftingManager.Get().LoadNewActorAndConstructIt();
		this.UpdateBankText();
		yield return new WaitForSeconds(this.m_craftDelayBeforeGhostDeath);
		if (CraftingManager.Get().IsCancelling())
		{
			yield break;
		}
		if (cardDef != null && cardDef.m_PlayEffectDef != null)
		{
			GameUtils.PlayCardEffectDefSounds(cardDef.m_PlayEffectDef);
		}
		CraftingManager.Get().FinishCreateAnims();
		yield return new WaitForSeconds(1f);
		this.m_isAnimating = false;
		yield break;
	}

	// Token: 0x040029AC RID: 10668
	public UberText m_bankAmountText;

	// Token: 0x040029AD RID: 10669
	public CreateButton m_buttonCreate;

	// Token: 0x040029AE RID: 10670
	public DisenchantButton m_buttonDisenchant;

	// Token: 0x040029AF RID: 10671
	public GameObject m_soulboundNotification;

	// Token: 0x040029B0 RID: 10672
	public UberText m_soulboundTitle;

	// Token: 0x040029B1 RID: 10673
	public UberText m_soulboundDesc;

	// Token: 0x040029B2 RID: 10674
	public UberText m_disenchantValue;

	// Token: 0x040029B3 RID: 10675
	public UberText m_craftValue;

	// Token: 0x040029B4 RID: 10676
	public GameObject m_wildTheming;

	// Token: 0x040029B5 RID: 10677
	public float m_disenchantDelayBeforeCardExplodes;

	// Token: 0x040029B6 RID: 10678
	public float m_disenchantDelayBeforeCardFlips;

	// Token: 0x040029B7 RID: 10679
	public float m_disenchantDelayBeforeBallsComeOut;

	// Token: 0x040029B8 RID: 10680
	public float m_craftDelayBeforeConstructSpell;

	// Token: 0x040029B9 RID: 10681
	public float m_craftDelayBeforeGhostDeath;

	// Token: 0x040029BA RID: 10682
	public GameObject m_glowballs;

	// Token: 0x040029BB RID: 10683
	public SoundDef m_craftingSound;

	// Token: 0x040029BC RID: 10684
	public SoundDef m_disenchantSound;

	// Token: 0x040029BD RID: 10685
	public Collider m_mouseOverCollider;

	// Token: 0x040029BE RID: 10686
	private Actor m_explodingActor;

	// Token: 0x040029BF RID: 10687
	private Actor m_constructingActor;

	// Token: 0x040029C0 RID: 10688
	private bool m_isAnimating;

	// Token: 0x040029C1 RID: 10689
	private List<GameObject> m_thingsToDestroy = new List<GameObject>();

	// Token: 0x040029C2 RID: 10690
	private GameObject m_activeObject;

	// Token: 0x040029C3 RID: 10691
	private bool m_enabled;

	// Token: 0x040029C4 RID: 10692
	private bool m_mousedOver;

	// Token: 0x040029C5 RID: 10693
	private Notification m_craftNotification;

	// Token: 0x040029C6 RID: 10694
	private bool m_initializedPositions;
}
