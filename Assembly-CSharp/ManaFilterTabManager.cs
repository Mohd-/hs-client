using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006BB RID: 1723
public class ManaFilterTabManager : MonoBehaviour
{
	// Token: 0x060047BE RID: 18366 RVA: 0x001585F8 File Offset: 0x001567F8
	private void Awake()
	{
	}

	// Token: 0x060047BF RID: 18367 RVA: 0x001585FA File Offset: 0x001567FA
	public void ClearFilter()
	{
		this.UpdateCurrentFilterValue(ManaFilterTab.ALL_TAB_IDX);
	}

	// Token: 0x060047C0 RID: 18368 RVA: 0x00158608 File Offset: 0x00156808
	public void SetUpTabs()
	{
		for (int i = 0; i <= ManaFilterTab.SEVEN_PLUS_TAB_IDX - 1; i++)
		{
			this.CreateNewTab(this.m_singleManaFilterPrefab, i);
		}
		this.CreateNewTab(this.m_dynamicManaFilterPrefab, ManaFilterTab.SEVEN_PLUS_TAB_IDX);
		this.m_manaCrystalContainer.UpdateSlices();
	}

	// Token: 0x060047C1 RID: 18369 RVA: 0x00158656 File Offset: 0x00156856
	public void ActivateTabs(bool active)
	{
		this.m_tabsActive = active;
		this.UpdateFilterStates();
		if (active)
		{
			this.m_manaCrystalContainer.UpdateSlices();
		}
	}

	// Token: 0x060047C2 RID: 18370 RVA: 0x00158678 File Offset: 0x00156878
	public void Enable(bool enabled)
	{
		foreach (ManaFilterTab manaFilterTab in this.m_tabs)
		{
			manaFilterTab.SetEnabled(enabled);
			ManaFilterTab.FilterState filterState = ManaFilterTab.FilterState.DISABLED;
			if (enabled && this.m_tabsActive)
			{
				filterState = ((manaFilterTab.GetManaID() != this.m_currentFilterValue) ? ManaFilterTab.FilterState.OFF : ManaFilterTab.FilterState.ON);
			}
			manaFilterTab.SetFilterState(filterState);
			if (manaFilterTab.m_costText != null)
			{
				manaFilterTab.m_costText.gameObject.SetActive(enabled);
			}
		}
	}

	// Token: 0x060047C3 RID: 18371 RVA: 0x00158728 File Offset: 0x00156928
	private void CreateNewTab(ManaFilterTab tabPrefab, int index)
	{
		ManaFilterTab manaFilterTab = (ManaFilterTab)GameUtils.Instantiate(tabPrefab, this.m_manaCrystalContainer.gameObject, false);
		manaFilterTab.SetManaID(index);
		manaFilterTab.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnTabPressed));
		manaFilterTab.AddEventListener(UIEventType.ROLLOVER, new UIEvent.Handler(this.OnTabMousedOver));
		manaFilterTab.AddEventListener(UIEventType.ROLLOUT, new UIEvent.Handler(this.OnTabMousedOut));
		manaFilterTab.SetFilterState(ManaFilterTab.FilterState.DISABLED);
		if (UniversalInputManager.Get().IsTouchMode())
		{
			manaFilterTab.SetReceiveReleaseWithoutMouseDown(true);
		}
		this.m_tabs.Add(manaFilterTab);
		this.m_manaCrystalContainer.AddSlice(manaFilterTab.gameObject);
	}

	// Token: 0x060047C4 RID: 18372 RVA: 0x001587CC File Offset: 0x001569CC
	private void OnTabPressed(UIEvent e)
	{
		if (!this.m_tabsActive)
		{
			return;
		}
		ManaFilterTab manaFilterTab = (ManaFilterTab)e.GetElement();
		if (!UniversalInputManager.UsePhoneUI && !Options.Get().GetBool(Option.HAS_CLICKED_MANA_TAB, false) && UserAttentionManager.CanShowAttentionGrabber("ManaFilterTabManager.OnTabPressed:" + Option.HAS_CLICKED_MANA_TAB))
		{
			Options.Get().SetBool(Option.HAS_CLICKED_MANA_TAB, true);
			this.ShowManaTabHint(manaFilterTab);
		}
		if (manaFilterTab.GetManaID() == this.m_currentFilterValue)
		{
			this.UpdateCurrentFilterValue(ManaFilterTab.ALL_TAB_IDX);
			return;
		}
		this.UpdateCurrentFilterValue(manaFilterTab.GetManaID());
	}

	// Token: 0x060047C5 RID: 18373 RVA: 0x00158874 File Offset: 0x00156A74
	private void OnTabMousedOver(UIEvent e)
	{
		if (!this.m_tabsActive)
		{
			return;
		}
		ManaFilterTab manaFilterTab = (ManaFilterTab)e.GetElement();
		manaFilterTab.NotifyMousedOver();
	}

	// Token: 0x060047C6 RID: 18374 RVA: 0x001588A0 File Offset: 0x00156AA0
	private void OnTabMousedOut(UIEvent e)
	{
		if (!this.m_tabsActive)
		{
			return;
		}
		ManaFilterTab manaFilterTab = (ManaFilterTab)e.GetElement();
		manaFilterTab.NotifyMousedOut();
	}

	// Token: 0x060047C7 RID: 18375 RVA: 0x001588CB File Offset: 0x00156ACB
	private void UpdateCurrentFilterValue(int filterValue)
	{
		if (filterValue != this.m_currentFilterValue)
		{
			SoundManager.Get().LoadAndPlay("mana_crystal_refresh");
			CollectionManagerDisplay.Get().FilterByManaCost(filterValue);
		}
		this.m_currentFilterValue = filterValue;
		this.UpdateFilterStates();
	}

	// Token: 0x060047C8 RID: 18376 RVA: 0x00158900 File Offset: 0x00156B00
	private void UpdateFilterStates()
	{
		foreach (ManaFilterTab manaFilterTab in this.m_tabs)
		{
			ManaFilterTab.FilterState filterState = ManaFilterTab.FilterState.DISABLED;
			if (this.m_tabsActive)
			{
				filterState = ((manaFilterTab.GetManaID() != this.m_currentFilterValue) ? ManaFilterTab.FilterState.OFF : ManaFilterTab.FilterState.ON);
			}
			manaFilterTab.SetFilterState(filterState);
		}
	}

	// Token: 0x060047C9 RID: 18377 RVA: 0x00158980 File Offset: 0x00156B80
	private void ShowManaTabHint(ManaFilterTab tabButton)
	{
		Notification notification = NotificationManager.Get().CreatePopupText(UserAttentionBlocker.NONE, tabButton.transform.position + new Vector3(0f, 0f, 7f), TutorialEntity.HELP_POPUP_SCALE, GameStrings.Get("GLUE_COLLECTION_MANAGER_MANA_TAB_FIRST_CLICK"), true);
		if (notification == null)
		{
			return;
		}
		notification.ShowPopUpArrow(Notification.PopUpArrowDirection.Down);
		NotificationManager.Get().DestroyNotification(notification, 3f);
	}

	// Token: 0x04002F4B RID: 12107
	public ManaFilterTab m_singleManaFilterPrefab;

	// Token: 0x04002F4C RID: 12108
	public ManaFilterTab m_dynamicManaFilterPrefab;

	// Token: 0x04002F4D RID: 12109
	public MultiSliceElement m_manaCrystalContainer;

	// Token: 0x04002F4E RID: 12110
	private bool m_tabsActive;

	// Token: 0x04002F4F RID: 12111
	private List<ManaFilterTab> m_tabs = new List<ManaFilterTab>();

	// Token: 0x04002F50 RID: 12112
	private int m_currentFilterValue = ManaFilterTab.ALL_TAB_IDX;
}
