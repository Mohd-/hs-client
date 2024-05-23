using System;
using UnityEngine;

// Token: 0x020006BA RID: 1722
public class ActiveFilterButton : MonoBehaviour
{
	// Token: 0x060047AF RID: 18351 RVA: 0x00158068 File Offset: 0x00156268
	protected void Awake()
	{
		if (this.m_inactiveFilterButton != null)
		{
			this.m_inactiveFilterButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.ShowFilters();
			});
		}
		if (this.m_activeFilterButton != null)
		{
			this.m_activeFilterButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.ClearFilters();
			});
		}
		if (this.m_doneButton != null)
		{
			this.m_doneButton.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
			{
				this.OffClickPressed();
			});
		}
		CollectionManagerDisplay.Get().RegisterManaFilterListener(new CollectionManagerDisplay.FilterStateListener(this.ManaFilterUpdate));
		CollectionManagerDisplay.Get().RegisterSearchFilterListener(new CollectionManagerDisplay.FilterStateListener(this.SearchFilterUpdate));
	}

	// Token: 0x060047B0 RID: 18352 RVA: 0x00158120 File Offset: 0x00156320
	protected void Start()
	{
		if (this.m_setFilterContainer != null)
		{
			this.m_setFilter = this.m_setFilterContainer.PrefabGameObject().GetComponent<SetFilterTray>();
			this.m_setFilter.m_toggleButton.transform.parent = base.transform;
			this.m_setFilterIconDefaultPos = this.m_setFilter.m_toggleButton.transform.localPosition;
		}
		this.m_manaFilterIconDefaultPos = this.m_manaFilterIcon.transform.localPosition;
		this.FiltersUpdated();
	}

	// Token: 0x060047B1 RID: 18353 RVA: 0x001581A8 File Offset: 0x001563A8
	public void OnDestroy()
	{
		CollectionManagerDisplay collectionManagerDisplay = CollectionManagerDisplay.Get();
		if (collectionManagerDisplay != null)
		{
			collectionManagerDisplay.UnregisterManaFilterListener(new CollectionManagerDisplay.FilterStateListener(this.ManaFilterUpdate));
			collectionManagerDisplay.UnregisterSearchFilterListener(new CollectionManagerDisplay.FilterStateListener(this.SearchFilterUpdate));
		}
	}

	// Token: 0x060047B2 RID: 18354 RVA: 0x001581EC File Offset: 0x001563EC
	public void ShowFilters()
	{
		CollectionManagerDisplay.Get().HideDeckHelpPopup();
		Navigation.Push(new Navigation.NavigateBackHandler(this.HideFilters));
		this.m_manaFilterTray.ToggleTraySlider(true, null, true);
		this.m_setFilterTray.ToggleTraySlider(true, null, true);
		this.m_setFilter.Show(true);
		this.m_manaFilter.m_manaCrystalContainer.UpdateSlices();
	}

	// Token: 0x060047B3 RID: 18355 RVA: 0x0015824C File Offset: 0x0015644C
	public bool HideFilters()
	{
		this.m_manaFilterTray.ToggleTraySlider(false, null, true);
		this.m_setFilterTray.ToggleTraySlider(false, null, true);
		return true;
	}

	// Token: 0x060047B4 RID: 18356 RVA: 0x0015826B File Offset: 0x0015646B
	public void OffClickPressed()
	{
		Navigation.GoBack();
		this.FiltersUpdated();
	}

	// Token: 0x060047B5 RID: 18357 RVA: 0x0015827C File Offset: 0x0015647C
	public void ClearFilters()
	{
		this.m_manaFilter.ClearFilter();
		this.m_setFilter.ClearFilter();
		this.m_search.ClearFilter(true);
	}

	// Token: 0x060047B6 RID: 18358 RVA: 0x001582AB File Offset: 0x001564AB
	public void SetEnabled(bool enabled)
	{
		this.m_inactiveFilterButton.SetEnabled(enabled);
		this.m_inactiveFilterButtonText.SetActive(enabled);
		this.m_inactiveFilterButtonRenderer.sharedMaterial = ((!enabled) ? this.m_disabledMaterial : this.m_enabledMaterial);
	}

	// Token: 0x060047B7 RID: 18359 RVA: 0x001582E7 File Offset: 0x001564E7
	public void ManaFilterUpdate(bool state, object description)
	{
		this.m_manaFilterActive = state;
		if (description == null)
		{
			this.m_manaFilterValue = string.Empty;
		}
		else
		{
			this.m_manaFilterValue = (string)description;
		}
		this.FiltersUpdated();
	}

	// Token: 0x060047B8 RID: 18360 RVA: 0x00158318 File Offset: 0x00156518
	public void SearchFilterUpdate(bool state, object description)
	{
		this.m_searchFilterActive = state;
		if (description == null)
		{
			this.m_searchFilterValue = string.Empty;
		}
		else
		{
			this.m_searchFilterValue = (string)description;
		}
		this.FiltersUpdated();
	}

	// Token: 0x060047B9 RID: 18361 RVA: 0x0015834C File Offset: 0x0015654C
	private void FiltersUpdated()
	{
		bool flag = this.m_manaFilterActive || this.m_searchFilterActive || this.m_setFilter.HasActiveFilter();
		if (this.m_inactiveFilterButton != null)
		{
			this.m_activeFilterButton.gameObject.SetActive(flag);
			this.m_inactiveFilterButton.gameObject.SetActive(!flag);
		}
		else
		{
			if (this.m_filtersShown != flag)
			{
				Vector3 vector = (!flag) ? new Vector3(0f, 0f, 0f) : new Vector3(180f, 0f, 0f);
				float num = (!flag) ? -0.5f : 0.5f;
				iTween.Stop(this.m_activeFilterButton.gameObject);
				this.m_activeFilterButton.gameObject.transform.localRotation = Quaternion.Euler(vector);
				iTween.RotateBy(this.m_activeFilterButton.gameObject, iTween.Hash(new object[]
				{
					"x",
					num,
					"time",
					0.25f,
					"easetype",
					iTween.EaseType.easeInOutExpo
				}));
			}
			this.m_filtersShown = flag;
		}
		this.m_searchText.Text = ((!this.m_searchFilterActive) ? string.Empty : this.m_searchFilterValue);
		this.m_manaFilterIcon.SetActive(this.m_manaFilterActive && !this.m_searchFilterActive);
		this.m_manaFilterText.Text = this.m_manaFilterValue;
		bool flag2 = this.m_setFilter.HasActiveFilter() && !this.m_searchFilterActive;
		this.m_setFilter.SetButtonShown(flag2);
		if (this.m_manaFilterIcon.activeSelf && !flag2)
		{
			this.m_manaFilterIcon.transform.localPosition = this.m_manaFilterIconCenterBone.localPosition;
		}
		else if (!this.m_manaFilterIcon.activeSelf && flag2)
		{
			this.m_setFilter.m_toggleButton.gameObject.transform.localPosition = this.m_setFilterIconCenterBone.localPosition;
		}
		else
		{
			this.m_manaFilterIcon.transform.localPosition = this.m_manaFilterIconDefaultPos;
			this.m_setFilter.m_toggleButton.gameObject.transform.localPosition = this.m_setFilterIconDefaultPos;
		}
	}

	// Token: 0x04002F31 RID: 12081
	public SlidingTray m_manaFilterTray;

	// Token: 0x04002F32 RID: 12082
	public SlidingTray m_setFilterTray;

	// Token: 0x04002F33 RID: 12083
	public UberText m_searchText;

	// Token: 0x04002F34 RID: 12084
	public GameObject m_manaFilterIcon;

	// Token: 0x04002F35 RID: 12085
	public UberText m_manaFilterText;

	// Token: 0x04002F36 RID: 12086
	public PegUIElement m_activeFilterButton;

	// Token: 0x04002F37 RID: 12087
	public PegUIElement m_inactiveFilterButton;

	// Token: 0x04002F38 RID: 12088
	public ManaFilterTabManager m_manaFilter;

	// Token: 0x04002F39 RID: 12089
	public SetFilterTray m_setFilter;

	// Token: 0x04002F3A RID: 12090
	public NestedPrefab m_setFilterContainer;

	// Token: 0x04002F3B RID: 12091
	public CollectionSearch m_search;

	// Token: 0x04002F3C RID: 12092
	public PegUIElement m_offClickCatcher;

	// Token: 0x04002F3D RID: 12093
	public UIBButton m_doneButton;

	// Token: 0x04002F3E RID: 12094
	public Material m_enabledMaterial;

	// Token: 0x04002F3F RID: 12095
	public Material m_disabledMaterial;

	// Token: 0x04002F40 RID: 12096
	public MeshRenderer m_inactiveFilterButtonRenderer;

	// Token: 0x04002F41 RID: 12097
	public GameObject m_inactiveFilterButtonText;

	// Token: 0x04002F42 RID: 12098
	public Transform m_manaFilterIconCenterBone;

	// Token: 0x04002F43 RID: 12099
	public Transform m_setFilterIconCenterBone;

	// Token: 0x04002F44 RID: 12100
	private bool m_filtersShown;

	// Token: 0x04002F45 RID: 12101
	private bool m_manaFilterActive;

	// Token: 0x04002F46 RID: 12102
	private string m_manaFilterValue = string.Empty;

	// Token: 0x04002F47 RID: 12103
	private bool m_searchFilterActive;

	// Token: 0x04002F48 RID: 12104
	private string m_searchFilterValue = string.Empty;

	// Token: 0x04002F49 RID: 12105
	private Vector3 m_manaFilterIconDefaultPos;

	// Token: 0x04002F4A RID: 12106
	private Vector3 m_setFilterIconDefaultPos;
}
