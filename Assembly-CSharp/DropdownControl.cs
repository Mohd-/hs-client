using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200051E RID: 1310
[CustomEditClass]
public class DropdownControl : PegUIElement
{
	// Token: 0x06003CD4 RID: 15572 RVA: 0x001264FC File Offset: 0x001246FC
	public void Start()
	{
		this.m_button.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.onUserPressedButton();
		});
		this.m_selectedItem.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.onUserPressedSelection(this.m_selectedItem);
		});
		this.m_cancelCatcher.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.onUserCancelled();
		});
		this.hideMenu();
	}

	// Token: 0x06003CD5 RID: 15573 RVA: 0x0012655C File Offset: 0x0012475C
	public void addItem(object value)
	{
		DropdownMenuItem item = (DropdownMenuItem)GameUtils.Instantiate(this.m_menuItemTemplate, this.m_menuItemContainer.gameObject, false);
		item.gameObject.transform.localRotation = this.m_menuItemTemplate.transform.localRotation;
		item.gameObject.transform.localScale = this.m_menuItemTemplate.transform.localScale;
		this.m_items.Add(item);
		if (this.m_overrideFontNoLocalization != null)
		{
			item.m_text.SetFontWithoutLocalization(this.m_overrideFontNoLocalization);
		}
		else if (this.m_overrideFont != null)
		{
			item.m_text.TrueTypeFont = this.m_overrideFont;
		}
		item.SetValue(value, this.m_itemTextCallback(value));
		item.AddEventListener(UIEventType.RELEASE, delegate(UIEvent e)
		{
			this.onUserItemClicked(item);
		});
		item.gameObject.SetActive(true);
		this.layoutMenu();
	}

	// Token: 0x06003CD6 RID: 15574 RVA: 0x00126690 File Offset: 0x00124890
	public bool removeItem(object value)
	{
		int num = this.findItemIndex(value);
		if (num < 0)
		{
			return false;
		}
		DropdownMenuItem dropdownMenuItem = this.m_items[num];
		this.m_items.RemoveAt(num);
		Object.Destroy(dropdownMenuItem.gameObject);
		this.layoutMenu();
		return true;
	}

	// Token: 0x06003CD7 RID: 15575 RVA: 0x001266DC File Offset: 0x001248DC
	public void clearItems()
	{
		foreach (DropdownMenuItem dropdownMenuItem in this.m_items)
		{
			Object.Destroy(dropdownMenuItem.gameObject);
		}
		this.layoutMenu();
	}

	// Token: 0x06003CD8 RID: 15576 RVA: 0x00126740 File Offset: 0x00124940
	public void setSelectionToLastItem()
	{
		this.m_selectedItem.SetValue(null, string.Empty);
		if (this.m_items.Count == 0)
		{
			return;
		}
		for (int i = 0; i < this.m_items.Count - 1; i++)
		{
			this.m_items[i].SetSelected(false);
		}
		DropdownMenuItem dropdownMenuItem = this.m_items[this.m_items.Count - 1];
		dropdownMenuItem.SetSelected(true);
		this.m_selectedItem.SetValue(dropdownMenuItem.GetValue(), this.m_itemTextCallback(dropdownMenuItem.GetValue()));
	}

	// Token: 0x06003CD9 RID: 15577 RVA: 0x001267E1 File Offset: 0x001249E1
	public object getSelection()
	{
		return this.m_selectedItem.GetValue();
	}

	// Token: 0x06003CDA RID: 15578 RVA: 0x001267F0 File Offset: 0x001249F0
	public void setSelection(object val)
	{
		this.m_selectedItem.SetValue(null, string.Empty);
		for (int i = 0; i < this.m_items.Count; i++)
		{
			DropdownMenuItem dropdownMenuItem = this.m_items[i];
			object value = dropdownMenuItem.GetValue();
			if ((value == null && val == null) || value.Equals(val))
			{
				dropdownMenuItem.SetSelected(true);
				this.m_selectedItem.SetValue(value, this.m_itemTextCallback(value));
			}
			else
			{
				dropdownMenuItem.SetSelected(false);
			}
		}
	}

	// Token: 0x06003CDB RID: 15579 RVA: 0x00126881 File Offset: 0x00124A81
	public void onUserPressedButton()
	{
		this.showMenu();
	}

	// Token: 0x06003CDC RID: 15580 RVA: 0x00126889 File Offset: 0x00124A89
	public void onUserPressedSelection(DropdownMenuItem item)
	{
		this.showMenu();
	}

	// Token: 0x06003CDD RID: 15581 RVA: 0x00126894 File Offset: 0x00124A94
	public void onUserItemClicked(DropdownMenuItem item)
	{
		this.hideMenu();
		object selection = this.getSelection();
		object value = item.GetValue();
		this.setSelection(value);
		this.m_itemChosenCallback(value, selection);
	}

	// Token: 0x06003CDE RID: 15582 RVA: 0x001268C9 File Offset: 0x00124AC9
	public void onUserCancelled()
	{
		if (SoundManager.Get().IsInitialized())
		{
			SoundManager.Get().LoadAndPlay("Small_Click");
		}
		this.hideMenu();
	}

	// Token: 0x06003CDF RID: 15583 RVA: 0x001268EF File Offset: 0x00124AEF
	public DropdownControl.itemChosenCallback getItemChosenCallback()
	{
		return this.m_itemChosenCallback;
	}

	// Token: 0x06003CE0 RID: 15584 RVA: 0x001268F7 File Offset: 0x00124AF7
	public void setItemChosenCallback(DropdownControl.itemChosenCallback callback)
	{
		DropdownControl.itemChosenCallback itemChosenCallback = callback;
		if (callback == null)
		{
			itemChosenCallback = delegate(object A_0, object A_1)
			{
			};
		}
		this.m_itemChosenCallback = itemChosenCallback;
	}

	// Token: 0x06003CE1 RID: 15585 RVA: 0x00126924 File Offset: 0x00124B24
	public DropdownControl.itemTextCallback getItemTextCallback()
	{
		return this.m_itemTextCallback;
	}

	// Token: 0x06003CE2 RID: 15586 RVA: 0x0012692C File Offset: 0x00124B2C
	public void setItemTextCallback(DropdownControl.itemTextCallback callback)
	{
		this.m_itemTextCallback = (callback ?? new DropdownControl.itemTextCallback(DropdownControl.defaultItemTextCallback));
	}

	// Token: 0x06003CE3 RID: 15587 RVA: 0x00126948 File Offset: 0x00124B48
	public static string defaultItemTextCallback(object val)
	{
		return (val != null) ? val.ToString() : string.Empty;
	}

	// Token: 0x06003CE4 RID: 15588 RVA: 0x00126960 File Offset: 0x00124B60
	public bool isMenuShown()
	{
		return this.m_menu.gameObject.activeInHierarchy;
	}

	// Token: 0x06003CE5 RID: 15589 RVA: 0x00126972 File Offset: 0x00124B72
	public DropdownControl.menuShownCallback getMenuShownCallback()
	{
		return this.m_menuShownCallback;
	}

	// Token: 0x06003CE6 RID: 15590 RVA: 0x0012697A File Offset: 0x00124B7A
	public void setMenuShownCallback(DropdownControl.menuShownCallback callback)
	{
		this.m_menuShownCallback = callback;
	}

	// Token: 0x06003CE7 RID: 15591 RVA: 0x00126984 File Offset: 0x00124B84
	public void setFont(Font font)
	{
		this.m_overrideFont = font;
		this.m_overrideFontNoLocalization = null;
		this.m_selectedItem.m_text.TrueTypeFont = font;
		this.m_menuItemTemplate.m_text.TrueTypeFont = font;
	}

	// Token: 0x06003CE8 RID: 15592 RVA: 0x001269C4 File Offset: 0x00124BC4
	public void setFontWithoutLocalization(FontDef fontDef)
	{
		this.m_overrideFontNoLocalization = fontDef;
		this.m_overrideFont = null;
		this.m_selectedItem.m_text.SetFontWithoutLocalization(fontDef);
		this.m_menuItemTemplate.m_text.SetFontWithoutLocalization(fontDef);
	}

	// Token: 0x06003CE9 RID: 15593 RVA: 0x00126A04 File Offset: 0x00124C04
	private void showMenu()
	{
		this.m_cancelCatcher.gameObject.SetActive(true);
		this.m_menu.gameObject.SetActive(true);
		this.layoutMenu();
		this.m_menuShownCallback(true);
	}

	// Token: 0x06003CEA RID: 15594 RVA: 0x00126A48 File Offset: 0x00124C48
	private void hideMenu()
	{
		this.m_cancelCatcher.gameObject.SetActive(false);
		this.m_menu.gameObject.SetActive(false);
		this.m_menuShownCallback(false);
	}

	// Token: 0x06003CEB RID: 15595 RVA: 0x00126A84 File Offset: 0x00124C84
	private void layoutMenu()
	{
		if (!base.gameObject.activeSelf)
		{
			return;
		}
		this.m_menuItemTemplate.gameObject.SetActive(true);
		OrientedBounds orientedBounds = TransformUtil.ComputeOrientedWorldBounds(this.m_menuItemTemplate.gameObject, true);
		if (orientedBounds == null)
		{
			return;
		}
		float num = orientedBounds.Extents[1].magnitude * 2f;
		this.m_menuItemTemplate.gameObject.SetActive(false);
		this.m_menuItemContainer.ClearSlices();
		for (int i = 0; i < this.m_items.Count; i++)
		{
			this.m_menuItemContainer.AddSlice(this.m_items[i].gameObject);
		}
		this.m_menuItemContainer.UpdateSlices();
		if (this.m_items.Count <= 1)
		{
			TransformUtil.SetLocalScaleZ(this.m_menuMiddle, 0.001f);
		}
		else
		{
			TransformUtil.SetLocalScaleToWorldDimension(this.m_menuMiddle, new WorldDimensionIndex[]
			{
				new WorldDimensionIndex(num * (float)(this.m_items.Count - 1), 2)
			});
		}
		this.m_menu.UpdateSlices();
	}

	// Token: 0x06003CEC RID: 15596 RVA: 0x00126BA8 File Offset: 0x00124DA8
	private int findItemIndex(object value)
	{
		for (int i = 0; i < this.m_items.Count; i++)
		{
			DropdownMenuItem dropdownMenuItem = this.m_items[i];
			if (dropdownMenuItem.GetValue() == value)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06003CED RID: 15597 RVA: 0x00126BF0 File Offset: 0x00124DF0
	private DropdownMenuItem findItem(object value)
	{
		for (int i = 0; i < this.m_items.Count; i++)
		{
			DropdownMenuItem dropdownMenuItem = this.m_items[i];
			if (dropdownMenuItem.GetValue() == value)
			{
				return dropdownMenuItem;
			}
		}
		return null;
	}

	// Token: 0x040026B9 RID: 9913
	[CustomEditField(Sections = "Buttons")]
	public DropdownMenuItem m_selectedItem;

	// Token: 0x040026BA RID: 9914
	[CustomEditField(Sections = "Buttons")]
	public PegUIElement m_cancelCatcher;

	// Token: 0x040026BB RID: 9915
	[CustomEditField(Sections = "Buttons")]
	public UIBButton m_button;

	// Token: 0x040026BC RID: 9916
	[CustomEditField(Sections = "Menu")]
	public MultiSliceElement m_menu;

	// Token: 0x040026BD RID: 9917
	[CustomEditField(Sections = "Menu")]
	public GameObject m_menuMiddle;

	// Token: 0x040026BE RID: 9918
	[CustomEditField(Sections = "Menu")]
	public MultiSliceElement m_menuItemContainer;

	// Token: 0x040026BF RID: 9919
	[CustomEditField(Sections = "Menu Templates")]
	public DropdownMenuItem m_menuItemTemplate;

	// Token: 0x040026C0 RID: 9920
	private DropdownControl.itemChosenCallback m_itemChosenCallback = delegate(object A_0, object A_1)
	{
	};

	// Token: 0x040026C1 RID: 9921
	private DropdownControl.itemTextCallback m_itemTextCallback = new DropdownControl.itemTextCallback(DropdownControl.defaultItemTextCallback);

	// Token: 0x040026C2 RID: 9922
	private DropdownControl.menuShownCallback m_menuShownCallback = delegate(bool A_0)
	{
	};

	// Token: 0x040026C3 RID: 9923
	private List<DropdownMenuItem> m_items = new List<DropdownMenuItem>();

	// Token: 0x040026C4 RID: 9924
	private Font m_overrideFont;

	// Token: 0x040026C5 RID: 9925
	private FontDef m_overrideFontNoLocalization;

	// Token: 0x02000522 RID: 1314
	// (Invoke) Token: 0x06003D27 RID: 15655
	public delegate string itemTextCallback(object val);

	// Token: 0x02000523 RID: 1315
	// (Invoke) Token: 0x06003D2B RID: 15659
	public delegate void itemChosenCallback(object selection, object prevSelection);

	// Token: 0x0200064B RID: 1611
	// (Invoke) Token: 0x06004560 RID: 17760
	public delegate void menuShownCallback(bool shown);
}
