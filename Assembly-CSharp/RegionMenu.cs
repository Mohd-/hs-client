using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AC2 RID: 2754
public class RegionMenu : ButtonListMenu
{
	// Token: 0x06005F30 RID: 24368 RVA: 0x001C7D2C File Offset: 0x001C5F2C
	protected override void Awake()
	{
		Debug.Log("region menu awake!");
		this.m_menuDefPrefab = this.m_menuDefPrefabOverride;
		this.m_menuParent = this.m_menuBone;
		base.Awake();
		base.SetTransform();
		this.m_menu.m_headerText.Text = GameStrings.Get("GLUE_PICK_A_REGION");
	}

	// Token: 0x06005F31 RID: 24369 RVA: 0x001C7D81 File Offset: 0x001C5F81
	public void SetButtons(List<UIBButton> buttons)
	{
		this.m_buttons = buttons;
	}

	// Token: 0x06005F32 RID: 24370 RVA: 0x001C7D8A File Offset: 0x001C5F8A
	public override void Show()
	{
		base.Show();
		SplashScreen.Get().HideWebAuth();
	}

	// Token: 0x06005F33 RID: 24371 RVA: 0x001C7D9C File Offset: 0x001C5F9C
	public override void Hide()
	{
		base.Hide();
		SplashScreen.Get().UnHideWebAuth();
		Object.Destroy(base.gameObject);
	}

	// Token: 0x06005F34 RID: 24372 RVA: 0x001C7DC4 File Offset: 0x001C5FC4
	protected override List<UIBButton> GetButtons()
	{
		return this.m_buttons;
	}

	// Token: 0x040046A1 RID: 18081
	public Transform m_menuBone;

	// Token: 0x040046A2 RID: 18082
	private List<UIBButton> m_buttons;

	// Token: 0x040046A3 RID: 18083
	protected string m_menuDefPrefabOverride = "ButtonListMenuDef_RegionMenu";
}
