using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020007BB RID: 1979
public class DisenchantButton : CraftingButton
{
	// Token: 0x06004D96 RID: 19862 RVA: 0x00171874 File Offset: 0x0016FA74
	public override void EnableButton()
	{
		if (CraftingManager.Get().GetNumTransactions() > 0)
		{
			base.EnterUndoMode();
			return;
		}
		this.labelText.Text = GameStrings.Get("GLUE_CRAFTING_DISENCHANT");
		base.EnableButton();
	}

	// Token: 0x06004D97 RID: 19863 RVA: 0x001718B4 File Offset: 0x0016FAB4
	protected override void OnRelease()
	{
		if (CraftingManager.Get().GetPendingTransaction() != null)
		{
			return;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			base.GetComponent<Animation>().Play("CardExchange_ButtonPress1_phone");
		}
		else
		{
			base.GetComponent<Animation>().Play("CardExchange_ButtonPress1");
		}
		if (CraftingManager.Get().GetNumTransactions() > 0)
		{
			this.DoDisenchant();
			return;
		}
		CollectionManager.Get().RequestDeckContentsForDecksWithoutContentsLoaded(new CollectionManager.DelOnAllDeckContents(this.OnReadyToStartDisenchant));
	}

	// Token: 0x06004D98 RID: 19864 RVA: 0x00171938 File Offset: 0x0016FB38
	private void OnReadyToStartDisenchant()
	{
		List<string> postDisenchantInvalidDeckNames = this.GetPostDisenchantInvalidDeckNames();
		if (postDisenchantInvalidDeckNames.Count != 0)
		{
			string text = GameStrings.Get("GLUE_CRAFTING_DISENCHANT_CONFIRM_DESC");
			foreach (string text2 in postDisenchantInvalidDeckNames)
			{
				text = text + "\n" + text2;
			}
			AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
			popupInfo.m_headerText = GameStrings.Get("GLUE_CRAFTING_DISENCHANT_CONFIRM_HEADER");
			popupInfo.m_text = text;
			popupInfo.m_showAlertIcon = false;
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
			popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.OnConfirmDisenchantResponse);
			DialogManager.Get().ShowPopup(popupInfo);
			return;
		}
		EntityDef entityDef = CraftingManager.Get().GetShownActor().GetEntityDef();
		string cardId = entityDef.GetCardId();
		int numOwnedCopies = CraftingManager.Get().GetNumOwnedCopies(cardId, TAG_PREMIUM.GOLDEN, false);
		int numOwnedCopies2 = CraftingManager.Get().GetNumOwnedCopies(cardId, TAG_PREMIUM.NORMAL, true);
		int num = numOwnedCopies + numOwnedCopies2;
		if (CraftingManager.Get().GetNumTransactions() <= 0 && this.m_lastwarnedCard != cardId && ((!entityDef.IsElite() && num <= 2) || (entityDef.IsElite() && num <= 1)))
		{
			AlertPopup.PopupInfo popupInfo2 = new AlertPopup.PopupInfo();
			popupInfo2.m_headerText = GameStrings.Get("GLUE_CRAFTING_DISENCHANT_CONFIRM_HEADER");
			popupInfo2.m_text = GameStrings.Get("GLUE_CRAFTING_DISENCHANT_CONFIRM2_DESC");
			popupInfo2.m_showAlertIcon = true;
			popupInfo2.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
			popupInfo2.m_responseCallback = new AlertPopup.ResponseCallback(this.OnConfirmDisenchantResponse);
			this.m_lastwarnedCard = cardId;
			DialogManager.Get().ShowPopup(popupInfo2);
			return;
		}
		this.DoDisenchant();
	}

	// Token: 0x06004D99 RID: 19865 RVA: 0x00171AF0 File Offset: 0x0016FCF0
	private void OnConfirmDisenchantResponse(AlertPopup.Response response, object userData)
	{
		if (response == AlertPopup.Response.CANCEL)
		{
			return;
		}
		this.DoDisenchant();
	}

	// Token: 0x06004D9A RID: 19866 RVA: 0x00171B00 File Offset: 0x0016FD00
	private void DoDisenchant()
	{
		CraftingManager.Get().DisenchantButtonPressed();
	}

	// Token: 0x06004D9B RID: 19867 RVA: 0x00171B0C File Offset: 0x0016FD0C
	private List<string> GetPostDisenchantInvalidDeckNames()
	{
		Actor shownActor = CraftingManager.Get().GetShownActor();
		string cardId = shownActor.GetEntityDef().GetCardId();
		TAG_PREMIUM premium = shownActor.GetPremium();
		CollectibleCard card = CollectionManager.Get().GetCard(cardId, premium);
		int num = Mathf.Max(0, card.OwnedCount - 1);
		SortedDictionary<long, CollectionDeck> decks = CollectionManager.Get().GetDecks();
		List<string> list = new List<string>();
		foreach (CollectionDeck collectionDeck in decks.Values)
		{
			int cardCount = collectionDeck.GetCardCount(cardId, premium);
			if (cardCount > num)
			{
				list.Add(collectionDeck.Name);
				Log.Rachelle.Print(string.Format("Disenchanting will invalidate deck '{0}'", collectionDeck.Name), new object[0]);
			}
		}
		return list;
	}

	// Token: 0x040034D6 RID: 13526
	private string m_lastwarnedCard;
}
