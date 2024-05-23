using System;
using UnityEngine;

// Token: 0x020007BD RID: 1981
public class CreateButton : CraftingButton
{
	// Token: 0x06004DA3 RID: 19875 RVA: 0x00171CD0 File Offset: 0x0016FED0
	protected override void OnRelease()
	{
		if (CraftingManager.Get().GetPendingTransaction() != null)
		{
			return;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			base.GetComponent<Animation>().Play("CardExchange_ButtonPress2_phone");
		}
		else
		{
			base.GetComponent<Animation>().Play("CardExchange_ButtonPress2");
		}
		string cardId = CraftingManager.Get().GetShownActor().GetEntityDef().GetCardId();
		DeckRuleset deckRuleset = CollectionManager.Get().GetDeckRuleset();
		bool flag;
		if (deckRuleset != null)
		{
			flag = !deckRuleset.Filter(DefLoader.Get().GetEntityDef(cardId));
		}
		else
		{
			TAG_PREMIUM premium = CraftingManager.Get().GetShownActor().GetPremium();
			CollectibleCard card = CollectionManager.Get().GetCard(cardId, premium);
			flag = GameUtils.IsSetRotated(card.Set);
		}
		if (CraftingManager.Get().GetNumTransactions() != 0)
		{
			flag = false;
		}
		if (flag)
		{
			AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo
			{
				m_headerText = GameStrings.Get("GLUE_CRAFTING_WILD_CARD_HEADER"),
				m_cancelText = GameStrings.Get("GLUE_CRAFTING_WILD_CARD_WARNING_CANCEL"),
				m_confirmText = GameStrings.Get("GLUE_CRAFTING_WILD_CARD_WARNING_CONFIRM"),
				m_showAlertIcon = true,
				m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL,
				m_responseCallback = new AlertPopup.ResponseCallback(this.OnConfirmCreateResponse)
			};
			if (SceneMgr.Get().GetMode() == SceneMgr.Mode.TAVERN_BRAWL)
			{
				popupInfo.m_headerText = GameStrings.Get("GLUE_CRAFTING_WILD_CARD_TAVERN_BRAWL_HEADER");
				popupInfo.m_text = GameStrings.Get("GLUE_CRAFTING_WILD_CARD_TAVERN_BRAWL_DESC");
			}
			else if (CollectionManager.Get().AccountEverHadWildCards())
			{
				popupInfo.m_text = GameStrings.Get("GLUE_CRAFTING_WILD_CARD_DESC");
			}
			else
			{
				popupInfo.m_text = GameStrings.Get("GLUE_CRAFTING_WILD_CARD_FIRST_WILD_DESC");
			}
			DialogManager.Get().ShowPopup(popupInfo);
			return;
		}
		this.DoCreate();
	}

	// Token: 0x06004DA4 RID: 19876 RVA: 0x00171E8C File Offset: 0x0017008C
	private void OnConfirmCreateResponse(AlertPopup.Response response, object userData)
	{
		if (response == AlertPopup.Response.CONFIRM)
		{
			if (!CollectionManager.Get().AccountEverHadWildCards())
			{
				AlertPopup.PopupInfo info = new AlertPopup.PopupInfo
				{
					m_headerText = GameStrings.Get("GLUE_CRAFTING_WILD_CARD_HEADER"),
					m_text = GameStrings.Get("GLUE_CRAFTING_WILD_CARD_INTRO_DESC"),
					m_showAlertIcon = true,
					m_responseDisplay = AlertPopup.ResponseDisplay.OK,
					m_responseCallback = delegate(AlertPopup.Response r, object data)
					{
						this.DoCreate();
						Options.Get().SetBool(Option.HAS_SEEN_STANDARD_MODE_TUTORIAL, true);
						Options.Get().SetInt(Option.SET_ROTATION_INTRO_PROGRESS, 1);
						UserAttentionManager.StopBlocking(UserAttentionBlocker.SET_ROTATION_INTRO);
						Options.Get().SetBool(Option.SHOW_SWITCH_TO_WILD_ON_PLAY_SCREEN, true);
						Options.Get().SetBool(Option.SHOW_SWITCH_TO_WILD_ON_CREATE_DECK, true);
					}
				};
				DialogManager.Get().ShowPopup(info);
			}
			else
			{
				this.DoCreate();
			}
		}
	}

	// Token: 0x06004DA5 RID: 19877 RVA: 0x00171F10 File Offset: 0x00170110
	public override void EnableButton()
	{
		if (CraftingManager.Get().GetNumTransactions() < 0)
		{
			base.EnterUndoMode();
			return;
		}
		this.labelText.Text = GameStrings.Get("GLUE_CRAFTING_CREATE");
		base.EnableButton();
	}

	// Token: 0x06004DA6 RID: 19878 RVA: 0x00171F4F File Offset: 0x0017014F
	private void DoCreate()
	{
		CraftingManager.Get().CreateButtonPressed();
	}
}
