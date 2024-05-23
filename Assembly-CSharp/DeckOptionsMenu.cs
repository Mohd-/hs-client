using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200075B RID: 1883
public class DeckOptionsMenu : MonoBehaviour
{
	// Token: 0x06004BC5 RID: 19397 RVA: 0x00169B10 File Offset: 0x00167D10
	public void Awake()
	{
		this.m_root.SetActive(false);
		if (this.m_renameButton != null)
		{
			this.m_renameButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnRenameButtonPressed));
		}
		if (this.m_deleteButton != null)
		{
			this.m_deleteButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDeleteButtonPressed));
		}
		if (this.m_switchFormatButton != null)
		{
			this.m_switchFormatButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnSwitchFormatButtonPressed));
		}
	}

	// Token: 0x06004BC6 RID: 19398 RVA: 0x00169BA8 File Offset: 0x00167DA8
	public void Show()
	{
		if (this.m_shown)
		{
			return;
		}
		iTween.Stop(base.gameObject);
		this.m_root.SetActive(true);
		this.SetSwitchFormatText(this.m_deck.IsWild);
		this.UpdateLayout();
		if (this.m_buttonCount == 0)
		{
			this.m_root.SetActive(false);
			return;
		}
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_showBone.transform.position,
			"time",
			0.35f,
			"easeType",
			iTween.EaseType.easeOutCubic,
			"oncomplete",
			"FinishShow",
			"oncompletetarget",
			base.gameObject
		});
		iTween.MoveTo(this.m_root, args);
		this.m_shown = true;
	}

	// Token: 0x06004BC7 RID: 19399 RVA: 0x00169C8F File Offset: 0x00167E8F
	private void FinishShow()
	{
	}

	// Token: 0x06004BC8 RID: 19400 RVA: 0x00169C94 File Offset: 0x00167E94
	public void Hide(bool animate = true)
	{
		if (!this.m_shown)
		{
			return;
		}
		iTween.Stop(base.gameObject);
		if (!animate)
		{
			this.m_root.SetActive(false);
			return;
		}
		this.m_root.SetActive(true);
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_hideBone.transform.position,
			"time",
			0.35f,
			"easeType",
			iTween.EaseType.easeOutCubic,
			"oncomplete",
			"FinishHide",
			"oncompletetarget",
			base.gameObject
		});
		iTween.MoveTo(this.m_root, args);
		this.m_shown = false;
	}

	// Token: 0x06004BC9 RID: 19401 RVA: 0x00169D5F File Offset: 0x00167F5F
	private void FinishHide()
	{
		if (!this.m_shown)
		{
			this.m_root.SetActive(false);
		}
	}

	// Token: 0x06004BCA RID: 19402 RVA: 0x00169D78 File Offset: 0x00167F78
	public void SetDeck(CollectionDeck deck)
	{
		this.m_deck = deck;
	}

	// Token: 0x06004BCB RID: 19403 RVA: 0x00169D81 File Offset: 0x00167F81
	public void SetDeckInfo(CollectionDeckInfo deckInfo)
	{
		this.m_deckInfo = deckInfo;
	}

	// Token: 0x06004BCC RID: 19404 RVA: 0x00169D8A File Offset: 0x00167F8A
	private void OnRenameButtonPressed(UIEvent e)
	{
		this.m_deckInfo.Hide();
		CollectionDeckTray.Get().GetDecksContent().RenameCurrentlyEditingDeck();
	}

	// Token: 0x06004BCD RID: 19405 RVA: 0x00169DA8 File Offset: 0x00167FA8
	private void OnDeleteButtonPressed(UIEvent e)
	{
		AlertPopup.PopupInfo popupInfo = new AlertPopup.PopupInfo();
		popupInfo.m_headerText = GameStrings.Get("GLUE_COLLECTION_DELETE_CONFIRM_HEADER");
		popupInfo.m_showAlertIcon = false;
		if (AchieveManager.Get().HasUnlockedFeature(Achievement.UnlockableFeature.VANILLA_HEROES))
		{
			popupInfo.m_text = GameStrings.Get("GLUE_COLLECTION_DELETE_CONFIRM_DESC");
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.CONFIRM_CANCEL;
			popupInfo.m_responseCallback = new AlertPopup.ResponseCallback(this.OnDeleteButtonConfirmationResponse);
		}
		else
		{
			popupInfo.m_text = GameStrings.Get("GLUE_COLLECTION_DELETE_UNAVAILABLE_DESC");
			popupInfo.m_responseDisplay = AlertPopup.ResponseDisplay.OK;
			popupInfo.m_responseCallback = null;
		}
		this.m_deckInfo.Hide();
		DialogManager.Get().ShowPopup(popupInfo);
	}

	// Token: 0x06004BCE RID: 19406 RVA: 0x00169E45 File Offset: 0x00168045
	private void OnDeleteButtonConfirmationResponse(AlertPopup.Response response, object userData)
	{
		if (response == AlertPopup.Response.CANCEL)
		{
			return;
		}
		CollectionDeckTray.Get().DeleteEditingDeck(true);
		if (CollectionManagerDisplay.Get())
		{
			CollectionManagerDisplay.Get().OnDoneEditingDeck();
		}
	}

	// Token: 0x06004BCF RID: 19407 RVA: 0x00169E73 File Offset: 0x00168073
	private void OnClosePressed(UIEvent e)
	{
		Navigation.GoBack();
	}

	// Token: 0x06004BD0 RID: 19408 RVA: 0x00169E7B File Offset: 0x0016807B
	private void OverOffClicker(UIEvent e)
	{
		Debug.Log("OverOffClicker");
		this.Hide(true);
	}

	// Token: 0x06004BD1 RID: 19409 RVA: 0x00169E8E File Offset: 0x0016808E
	private void OnSwitchFormatButtonPressed(UIEvent e)
	{
		base.StartCoroutine(this.SwitchFormat());
	}

	// Token: 0x06004BD2 RID: 19410 RVA: 0x00169EA0 File Offset: 0x001680A0
	private IEnumerator SwitchFormat()
	{
		CollectionManagerDisplay.Get().HideConvertTutorial();
		this.m_deckInfo.Hide();
		this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_OFF);
		TraySection editingTraySection = CollectionDeckTray.Get().GetDecksContent().GetEditingTraySection();
		if (this.m_deck.IsWild)
		{
			editingTraySection.m_deckFX.Play("DeckTraySectionCollectionDeck_WildGlowOut");
			yield return new WaitForSeconds(0.5f);
		}
		else
		{
			editingTraySection.m_deckFX.Play("DeckTraySectionCollectionDeck_StandardGlowOut");
			yield return new WaitForSeconds(0.5f);
		}
		this.SetDeckFormat(!this.m_deck.IsWild);
		yield break;
	}

	// Token: 0x06004BD3 RID: 19411 RVA: 0x00169EBC File Offset: 0x001680BC
	private void SetDeckFormat(bool isWild)
	{
		this.m_deck.IsWild = isWild;
		CollectionDeckTray.Get().GetEditingDeckBox().SetFormat(isWild);
		CollectionManager.Get().SetDeckRuleset((!isWild) ? DeckRuleset.GetStandardRuleset() : DeckRuleset.GetWildRuleset());
		CollectionManagerDisplay.Get().m_pageManager.RefreshCurrentPageContents(CollectionPageManager.PageTransitionType.SINGLE_PAGE_RIGHT);
		CollectionManagerDisplay.Get().UpdateSetFilters(isWild, true, false);
		CollectionDeckTray.Get().GetCardsContent().UpdateCardList(true, null);
		CollectionDeckTray.Get().GetCardsContent().UpdateTileVisuals();
		if (!isWild)
		{
			if (CollectionManager.Get().ShouldShowWildToStandardTutorial(true))
			{
				CollectionManagerDisplay.Get().ShowStandardInfoTutorial(UserAttentionBlocker.SET_ROTATION_CM_TUTORIALS);
			}
			base.StartCoroutine(CollectionManagerDisplay.Get().ShowDeckTemplateTipsIfNeeded());
		}
	}

	// Token: 0x06004BD4 RID: 19412 RVA: 0x00169F73 File Offset: 0x00168173
	private void SetSwitchFormatText(bool isWild)
	{
		this.m_convertText.Text = GameStrings.Get((!isWild) ? "GLUE_COLLECTION_TO_WILD" : "GLUE_COLLECTION_TO_STANDARD");
	}

	// Token: 0x06004BD5 RID: 19413 RVA: 0x00169F9C File Offset: 0x0016819C
	private void UpdateLayout()
	{
		int buttonCount = this.GetButtonCount();
		if (buttonCount != this.m_buttonCount)
		{
			this.m_buttonCount = buttonCount;
			this.UpdateBackground();
		}
		this.UpdateButtons();
	}

	// Token: 0x06004BD6 RID: 19414 RVA: 0x00169FD0 File Offset: 0x001681D0
	private void UpdateBackground()
	{
		float num = this.m_topScales[this.m_buttonCount - 1];
		this.m_top.transform.transform.localScale = new Vector3(1f, 1f, num);
		this.m_bottom.transform.transform.position = this.m_bottomPositions[this.m_buttonCount - 1].position;
	}

	// Token: 0x06004BD7 RID: 19415 RVA: 0x0016A03C File Offset: 0x0016823C
	private void UpdateButtons()
	{
		int num = 0;
		bool flag = this.ShowConvertButton();
		bool flag2 = this.ShowRenameButton();
		bool flag3 = this.ShowDeleteButton();
		this.m_switchFormatButton.gameObject.SetActive(flag);
		if (flag)
		{
			if (this.m_deck.IsWild && this.m_highlight != null && CollectionManager.Get().ShouldShowWildToStandardTutorial(true))
			{
				this.m_highlight.ChangeState(ActorStateType.HIGHLIGHT_PRIMARY_ACTIVE);
			}
			this.m_switchFormatButton.transform.position = this.m_buttonPositions[num].position;
			num++;
		}
		this.m_renameButton.gameObject.SetActive(flag2);
		if (flag2)
		{
			this.m_renameButton.transform.position = this.m_buttonPositions[num].position;
			num++;
		}
		this.m_deleteButton.gameObject.SetActive(flag3);
		if (flag3)
		{
			this.m_deleteButton.transform.position = this.m_buttonPositions[num].position;
			num++;
		}
	}

	// Token: 0x06004BD8 RID: 19416 RVA: 0x0016A148 File Offset: 0x00168348
	private int GetButtonCount()
	{
		int num = 0;
		num += ((!this.ShowRenameButton()) ? 0 : 1);
		num += ((!this.ShowDeleteButton()) ? 0 : 1);
		return num + ((!this.ShowConvertButton()) ? 0 : 1);
	}

	// Token: 0x06004BD9 RID: 19417 RVA: 0x0016A197 File Offset: 0x00168397
	private bool ShowRenameButton()
	{
		return UniversalInputManager.Get().IsTouchMode();
	}

	// Token: 0x06004BDA RID: 19418 RVA: 0x0016A1A3 File Offset: 0x001683A3
	private bool ShowDeleteButton()
	{
		return UniversalInputManager.Get().IsTouchMode();
	}

	// Token: 0x06004BDB RID: 19419 RVA: 0x0016A1B0 File Offset: 0x001683B0
	private bool ShowConvertButton()
	{
		return SceneMgr.Get().GetMode() != SceneMgr.Mode.TAVERN_BRAWL && CollectionManager.Get().ShouldAccountSeeStandardWild();
	}

	// Token: 0x0400329C RID: 12956
	public GameObject m_root;

	// Token: 0x0400329D RID: 12957
	public UberText m_convertText;

	// Token: 0x0400329E RID: 12958
	public PegUIElement m_renameButton;

	// Token: 0x0400329F RID: 12959
	public PegUIElement m_deleteButton;

	// Token: 0x040032A0 RID: 12960
	public PegUIElement m_switchFormatButton;

	// Token: 0x040032A1 RID: 12961
	public GameObject m_top;

	// Token: 0x040032A2 RID: 12962
	public GameObject m_bottom;

	// Token: 0x040032A3 RID: 12963
	public HighlightState m_highlight;

	// Token: 0x040032A4 RID: 12964
	public Transform m_showBone;

	// Token: 0x040032A5 RID: 12965
	public Transform m_hideBone;

	// Token: 0x040032A6 RID: 12966
	public Transform[] m_buttonPositions;

	// Token: 0x040032A7 RID: 12967
	public Transform[] m_bottomPositions;

	// Token: 0x040032A8 RID: 12968
	public float[] m_topScales;

	// Token: 0x040032A9 RID: 12969
	private int m_buttonCount;

	// Token: 0x040032AA RID: 12970
	private bool m_shown;

	// Token: 0x040032AB RID: 12971
	private CollectionDeck m_deck;

	// Token: 0x040032AC RID: 12972
	private CollectionDeckInfo m_deckInfo;
}
