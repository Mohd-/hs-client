using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006BC RID: 1724
public class SetFilterTray : MonoBehaviour
{
	// Token: 0x060047CB RID: 18379 RVA: 0x00158A18 File Offset: 0x00156C18
	private void Awake()
	{
		if (!UniversalInputManager.UsePhoneUI)
		{
			this.m_toggleButton.AddEventListener(UIEventType.PRESS, delegate(UIEvent e)
			{
				this.Show(true);
			});
			this.m_hideArea.AddEventListener(UIEventType.PRESS, delegate(UIEvent e)
			{
				this.Show(false);
			});
			this.m_trayObject.SetActive(false);
		}
		else
		{
			this.m_hideArea.gameObject.SetActive(false);
		}
		this.m_toggleButton.gameObject.SetActive(false);
	}

	// Token: 0x060047CC RID: 18380 RVA: 0x00158A99 File Offset: 0x00156C99
	public void SetButtonShown(bool isShown)
	{
		this.m_toggleButton.gameObject.SetActive(isShown);
	}

	// Token: 0x060047CD RID: 18381 RVA: 0x00158AAC File Offset: 0x00156CAC
	public void SetButtonEnabled(bool isEnabled)
	{
		this.m_toggleButton.SetEnabled(isEnabled);
		this.m_toggleButton.SetEnabledVisual(isEnabled);
	}

	// Token: 0x060047CE RID: 18382 RVA: 0x00158AC8 File Offset: 0x00156CC8
	public void AddHeader(string headerName, bool isWild)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.m_headerPrefab);
		GameUtils.SetParent(gameObject, this.m_contents, false);
		gameObject.SetActive(false);
		SetFilterItem component = gameObject.GetComponent<SetFilterItem>();
		UIBScrollableItem component2 = gameObject.GetComponent<UIBScrollableItem>();
		component.IsHeader = true;
		component.Text = headerName;
		component.Height = component2.m_size.z;
		component.IsWild = isWild;
		this.m_items.Add(component);
	}

	// Token: 0x060047CF RID: 18383 RVA: 0x00158B38 File Offset: 0x00156D38
	public void AddItem(string itemName, Vector2? iconOffset, SetFilterItem.ItemSelectedCallback callback, List<TAG_CARD_SET> data, bool isWild, bool isAllStandard = false)
	{
		GameObject gameObject = Object.Instantiate<GameObject>(this.m_itemPrefab);
		SetFilterItem item = gameObject.GetComponent<SetFilterItem>();
		GameUtils.SetParent(gameObject, this.m_contents, false);
		gameObject.SetActive(false);
		SetFilterItem component = gameObject.GetComponent<SetFilterItem>();
		UIBScrollableItem component2 = gameObject.GetComponent<UIBScrollableItem>();
		item.IsHeader = false;
		item.Text = itemName;
		item.Height = component2.m_size.z;
		item.IsWild = isWild;
		item.IsAllStandard = isAllStandard;
		item.CardSets = data;
		item.Callback = callback;
		item.IconOffset = iconOffset;
		this.m_items.Add(item);
		component.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.Select(item, true);
		});
	}

	// Token: 0x060047D0 RID: 18384 RVA: 0x00158C20 File Offset: 0x00156E20
	public void SelectFirstItem()
	{
		foreach (SetFilterItem setFilterItem in this.m_items)
		{
			if (!setFilterItem.IsHeader)
			{
				UIBScrollableItem component = setFilterItem.GetComponent<UIBScrollableItem>();
				if (component != null && component.m_active == UIBScrollableItem.ActiveState.Active)
				{
					this.Select(setFilterItem, true);
					break;
				}
			}
		}
	}

	// Token: 0x060047D1 RID: 18385 RVA: 0x00158CAC File Offset: 0x00156EAC
	public bool HasActiveFilter()
	{
		foreach (SetFilterItem setFilterItem in this.m_items)
		{
			if (!setFilterItem.IsHeader && setFilterItem.isActiveAndEnabled)
			{
				if (setFilterItem == this.m_selected)
				{
					return false;
				}
				return true;
			}
		}
		return false;
	}

	// Token: 0x060047D2 RID: 18386 RVA: 0x00158D3C File Offset: 0x00156F3C
	public void Select(SetFilterItem item, bool callCallback = true)
	{
		if (item == this.m_selected)
		{
			return;
		}
		if (this.m_selected != null)
		{
			this.m_selected.SetSelected(false);
			this.m_lastSelected = this.m_selected;
		}
		this.m_selected = item;
		item.SetSelected(true);
		if (callCallback)
		{
			item.Callback(item.CardSets, item.IsWild);
		}
		this.m_toggleButton.SetToggleIconOffset(new Vector2?(item.IconOffset.Value));
	}

	// Token: 0x060047D3 RID: 18387 RVA: 0x00158DCD File Offset: 0x00156FCD
	public void SelectPreviouslySelectedItem()
	{
		this.Select(this.m_lastSelected, false);
	}

	// Token: 0x060047D4 RID: 18388 RVA: 0x00158DDC File Offset: 0x00156FDC
	public void UpdateSetFilters(bool showWild, bool editingDeck, bool showUnownedSets)
	{
		if (this.m_showWild != showWild || this.m_editingDeck != editingDeck || this.m_showUnownedSets != showUnownedSets)
		{
			this.m_showWild = showWild;
			this.m_editingDeck = editingDeck;
			this.m_showUnownedSets = showUnownedSets;
			this.Arrange();
		}
	}

	// Token: 0x060047D5 RID: 18389 RVA: 0x00158E28 File Offset: 0x00157028
	public void ClearFilter()
	{
		this.SelectFirstItem();
		if (UniversalInputManager.UsePhoneUI)
		{
			this.SetButtonShown(false);
		}
	}

	// Token: 0x060047D6 RID: 18390 RVA: 0x00158E48 File Offset: 0x00157048
	public void Show(bool show)
	{
		if (!UniversalInputManager.UsePhoneUI)
		{
			if (this.m_isAnimating)
			{
				return;
			}
			this.m_shown = show;
			this.m_trayObject.SetActive(true);
			this.m_hideArea.gameObject.SetActive(true);
			UIBHighlight component = this.m_toggleButton.GetComponent<UIBHighlight>();
			if (component != null)
			{
				component.AlwaysOver = show;
			}
			this.m_isAnimating = true;
			if (show)
			{
				this.Arrange();
				this.m_trayObject.transform.localPosition = this.m_hideBone.transform.localPosition;
				Hashtable args = iTween.Hash(new object[]
				{
					"position",
					this.m_showBone.transform.localPosition,
					"time",
					0.35f,
					"easeType",
					iTween.EaseType.easeOutCubic,
					"isLocal",
					true,
					"oncomplete",
					"FinishFilterShown",
					"oncompletetarget",
					base.gameObject
				});
				iTween.MoveTo(this.m_trayObject, args);
				SoundManager.Get().LoadAndPlay("choose_opponent_panel_slide_on", base.gameObject);
			}
			else
			{
				this.m_trayObject.transform.localPosition = this.m_showBone.transform.localPosition;
				Hashtable args2 = iTween.Hash(new object[]
				{
					"position",
					this.m_hideBone.transform.localPosition,
					"time",
					0.25f,
					"easeType",
					iTween.EaseType.easeOutCubic,
					"isLocal",
					true,
					"oncomplete",
					"FinishFilterHidden",
					"oncompletetarget",
					base.gameObject
				});
				iTween.MoveTo(this.m_trayObject, args2);
				SoundManager.Get().LoadAndPlay("choose_opponent_panel_slide_off", base.gameObject);
			}
			this.m_hideArea.gameObject.SetActive(this.m_shown);
		}
		else
		{
			this.Arrange();
		}
	}

	// Token: 0x060047D7 RID: 18391 RVA: 0x0015907E File Offset: 0x0015727E
	private void FinishFilterShown()
	{
		this.m_isAnimating = false;
	}

	// Token: 0x060047D8 RID: 18392 RVA: 0x00159088 File Offset: 0x00157288
	private void FinishFilterHidden()
	{
		this.m_isAnimating = false;
		this.m_trayObject.SetActive(false);
		this.m_hideArea.gameObject.SetActive(false);
	}

	// Token: 0x060047D9 RID: 18393 RVA: 0x001590BC File Offset: 0x001572BC
	private void Arrange()
	{
		this.m_scroller.ClearVisibleAffectObjects();
		if (!this.m_showUnownedSets)
		{
			this.EvaluateOwnership();
		}
		Vector3 position = this.m_contentsBone.transform.position;
		bool flag = false;
		foreach (SetFilterItem setFilterItem in this.m_items)
		{
			UIBScrollableItem component = setFilterItem.GetComponent<UIBScrollableItem>();
			if (component == null)
			{
				Debug.LogWarning("SetFilterItem has no UIBScrollableItem component!");
			}
			else if ((setFilterItem.IsWild && !this.m_showWild) || (this.m_showWild && this.m_editingDeck && setFilterItem.IsAllStandard) || (!this.m_showUnownedSets && !this.OwnCardInSetsForItem(setFilterItem)))
			{
				if (setFilterItem == this.m_selected)
				{
					flag = true;
				}
				setFilterItem.gameObject.SetActive(false);
				component.m_active = UIBScrollableItem.ActiveState.Inactive;
			}
			else
			{
				setFilterItem.gameObject.SetActive(true);
				component.m_active = UIBScrollableItem.ActiveState.Active;
				setFilterItem.gameObject.transform.position = position;
				position.z -= setFilterItem.Height;
				this.m_scroller.AddVisibleAffectedObject(setFilterItem.gameObject, new Vector3(setFilterItem.Height, setFilterItem.Height, setFilterItem.Height), true, null);
			}
		}
		if (flag)
		{
			this.SelectFirstItem();
		}
		this.m_scroller.UpdateAndFireVisibleAffectedObjects();
	}

	// Token: 0x060047DA RID: 18394 RVA: 0x00159264 File Offset: 0x00157464
	private void EvaluateOwnership()
	{
		if (this.m_lastCollectionQueryTime > CollectionManager.Get().CollectionLastModifiedTime())
		{
			return;
		}
		this.m_setsWithOwnedCards.Clear();
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		List<CollectibleCard> allCards = CollectionManager.Get().GetAllCards();
		for (int i = 0; i < allCards.Count; i++)
		{
			CollectibleCard collectibleCard = allCards[i];
			if (collectibleCard.OwnedCount > 0)
			{
				this.m_setsWithOwnedCards.Add(collectibleCard.Set);
			}
		}
		Log.JMac.Print("SetFilterTray - Evaluating Ownership took {0} seconds.", new object[]
		{
			Time.realtimeSinceStartup - realtimeSinceStartup
		});
		this.m_lastCollectionQueryTime = Time.realtimeSinceStartup;
	}

	// Token: 0x060047DB RID: 18395 RVA: 0x00159310 File Offset: 0x00157510
	private bool OwnCardInSetsForItem(SetFilterItem item)
	{
		if (item.CardSets == null)
		{
			return true;
		}
		for (int i = 0; i < item.CardSets.Count; i++)
		{
			if (this.m_setsWithOwnedCards.Contains(item.CardSets[i]))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x04002F51 RID: 12113
	public UIBScrollable m_scroller;

	// Token: 0x04002F52 RID: 12114
	public GameObject m_contents;

	// Token: 0x04002F53 RID: 12115
	public CollectionSetFilterDropdownToggle m_toggleButton;

	// Token: 0x04002F54 RID: 12116
	public PegUIElement m_hideArea;

	// Token: 0x04002F55 RID: 12117
	public GameObject m_trayObject;

	// Token: 0x04002F56 RID: 12118
	public GameObject m_contentsBone;

	// Token: 0x04002F57 RID: 12119
	public GameObject m_headerPrefab;

	// Token: 0x04002F58 RID: 12120
	public GameObject m_itemPrefab;

	// Token: 0x04002F59 RID: 12121
	public GameObject m_showBone;

	// Token: 0x04002F5A RID: 12122
	public GameObject m_hideBone;

	// Token: 0x04002F5B RID: 12123
	private bool m_shown;

	// Token: 0x04002F5C RID: 12124
	private bool m_showWild = true;

	// Token: 0x04002F5D RID: 12125
	private bool m_editingDeck;

	// Token: 0x04002F5E RID: 12126
	private bool m_showUnownedSets;

	// Token: 0x04002F5F RID: 12127
	private bool m_isAnimating;

	// Token: 0x04002F60 RID: 12128
	private List<SetFilterItem> m_items = new List<SetFilterItem>();

	// Token: 0x04002F61 RID: 12129
	private float m_lastCollectionQueryTime;

	// Token: 0x04002F62 RID: 12130
	private HashSet<TAG_CARD_SET> m_setsWithOwnedCards = new HashSet<TAG_CARD_SET>();

	// Token: 0x04002F63 RID: 12131
	private SetFilterItem m_selected;

	// Token: 0x04002F64 RID: 12132
	private SetFilterItem m_lastSelected;
}
