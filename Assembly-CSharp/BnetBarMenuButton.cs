using System;
using System.Collections;
using UnityEngine;

// Token: 0x020004DE RID: 1246
public class BnetBarMenuButton : PegUIElement
{
	// Token: 0x06003A9C RID: 15004 RVA: 0x0011B15B File Offset: 0x0011935B
	public void LockHighlight(bool isLocked)
	{
		this.m_highlight.SetActive(isLocked);
	}

	// Token: 0x06003A9D RID: 15005 RVA: 0x0011B169 File Offset: 0x00119369
	protected override void Awake()
	{
		base.Awake();
		this.UpdateHighlight();
	}

	// Token: 0x06003A9E RID: 15006 RVA: 0x0011B177 File Offset: 0x00119377
	public bool IsSelected()
	{
		return this.m_selected;
	}

	// Token: 0x06003A9F RID: 15007 RVA: 0x0011B17F File Offset: 0x0011937F
	public void SetSelected(bool enable)
	{
		if (enable == this.m_selected)
		{
			return;
		}
		this.m_selected = enable;
		this.UpdateHighlight();
	}

	// Token: 0x06003AA0 RID: 15008 RVA: 0x0011B19C File Offset: 0x0011939C
	public void SetPhoneStatusBarState(int nElements)
	{
		if (nElements == this.m_phoneBarStatus)
		{
			return;
		}
		this.m_phoneBarStatus = nElements;
		switch (nElements)
		{
		case 0:
			this.m_phoneBar.SetActive(false);
			break;
		case 1:
		{
			this.m_phoneBar.SetActive(true);
			iTween.Stop(this.m_phoneBar);
			Hashtable args = iTween.Hash(new object[]
			{
				"position",
				this.m_phoneBarOneElementBone.position,
				"time",
				1f,
				"isLocal",
				false,
				"easetype",
				iTween.EaseType.easeOutExpo,
				"onupdate",
				"OnStatusBarUpdate",
				"onupdatetarget",
				base.gameObject
			});
			iTween.MoveTo(this.m_phoneBar, args);
			break;
		}
		case 2:
		{
			this.m_phoneBar.SetActive(true);
			iTween.Stop(this.m_phoneBar);
			Hashtable args = iTween.Hash(new object[]
			{
				"position",
				this.m_phoneBarTwoElementBone.position,
				"time",
				1f,
				"isLocal",
				false,
				"easetype",
				iTween.EaseType.easeOutExpo,
				"onupdate",
				"OnStatusBarUpdate",
				"onupdatetarget",
				base.gameObject
			});
			iTween.MoveTo(this.m_phoneBar, args);
			break;
		}
		default:
			Debug.LogError("Invalid phone status bar state " + nElements);
			break;
		}
	}

	// Token: 0x06003AA1 RID: 15009 RVA: 0x0011B359 File Offset: 0x00119559
	public void OnStatusBarUpdate()
	{
		BnetBar.Get().UpdateLayout();
	}

	// Token: 0x06003AA2 RID: 15010 RVA: 0x0011B365 File Offset: 0x00119565
	private bool ShouldBeHighlighted()
	{
		return this.m_selected || base.GetInteractionState() == PegUIElement.InteractionState.Over;
	}

	// Token: 0x06003AA3 RID: 15011 RVA: 0x0011B380 File Offset: 0x00119580
	protected virtual void UpdateHighlight()
	{
		bool flag = this.ShouldBeHighlighted();
		if (!flag && GameMenu.Get() != null && GameMenu.Get().IsShown())
		{
			flag = true;
		}
		if (this.m_highlight.activeSelf == flag)
		{
			return;
		}
		this.m_highlight.SetActive(flag);
	}

	// Token: 0x06003AA4 RID: 15012 RVA: 0x0011B3DC File Offset: 0x001195DC
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		KeywordHelpPanel keywordHelpPanel = base.GetComponent<TooltipZone>().ShowTooltip(GameStrings.Get("GLOBAL_TOOLTIP_MENU_HEADER"), GameStrings.Get("GLOBAL_TOOLTIP_MENU_DESC"), 0.7f, true);
		SceneUtils.SetLayer(keywordHelpPanel.gameObject, GameLayer.BattleNet);
		keywordHelpPanel.transform.localEulerAngles = new Vector3(270f, 0f, 0f);
		keywordHelpPanel.transform.localScale = new Vector3(82.35294f, 70f, 90.32258f);
		TransformUtil.SetPoint(keywordHelpPanel, Anchor.BOTTOM, base.gameObject, Anchor.TOP, new Vector3(-98.22766f, 0f, 0f));
		SoundManager.Get().LoadAndPlay("Small_Mouseover");
		this.UpdateHighlight();
	}

	// Token: 0x06003AA5 RID: 15013 RVA: 0x0011B491 File Offset: 0x00119691
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		base.GetComponent<TooltipZone>().HideTooltip();
		this.UpdateHighlight();
	}

	// Token: 0x04002558 RID: 9560
	public GameObject m_highlight;

	// Token: 0x04002559 RID: 9561
	public GameObject m_phoneBar;

	// Token: 0x0400255A RID: 9562
	public Transform m_phoneBarOneElementBone;

	// Token: 0x0400255B RID: 9563
	public Transform m_phoneBarTwoElementBone;

	// Token: 0x0400255C RID: 9564
	private int m_phoneBarStatus = -1;

	// Token: 0x0400255D RID: 9565
	private bool m_selected;
}
