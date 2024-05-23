using System;
using UnityEngine;

// Token: 0x0200026B RID: 619
public class TooltipZone : MonoBehaviour
{
	// Token: 0x060022CD RID: 8909 RVA: 0x000AB60A File Offset: 0x000A980A
	public GameObject GetTooltipObject()
	{
		return this.m_tooltip;
	}

	// Token: 0x060022CE RID: 8910 RVA: 0x000AB612 File Offset: 0x000A9812
	public bool IsShowingTooltip()
	{
		return this.m_tooltip != null;
	}

	// Token: 0x060022CF RID: 8911 RVA: 0x000AB620 File Offset: 0x000A9820
	public KeywordHelpPanel ShowTooltip(string headline, string bodytext, float scale, bool enablePhoneScale = true)
	{
		if (this.m_tooltip != null)
		{
			return this.m_tooltip.GetComponent<KeywordHelpPanel>();
		}
		if (UniversalInputManager.UsePhoneUI && enablePhoneScale)
		{
			scale *= 2f;
		}
		this.m_tooltip = Object.Instantiate<GameObject>(this.tooltipPrefab);
		KeywordHelpPanel component = this.m_tooltip.GetComponent<KeywordHelpPanel>();
		component.Reset();
		component.Initialize(headline, bodytext);
		component.SetScale(scale);
		if (UniversalInputManager.Get().IsTouchMode() && this.touchTooltipLocation != null)
		{
			component.transform.position = this.touchTooltipLocation.position;
			component.transform.rotation = this.touchTooltipLocation.rotation;
		}
		else if (this.tooltipDisplayLocation != null)
		{
			component.transform.position = this.tooltipDisplayLocation.position;
			component.transform.rotation = this.tooltipDisplayLocation.rotation;
		}
		component.transform.parent = base.transform;
		return component;
	}

	// Token: 0x060022D0 RID: 8912 RVA: 0x000AB73C File Offset: 0x000A993C
	public KeywordHelpPanel ShowTooltip(string headline, string bodytext)
	{
		float scale;
		if (SceneMgr.Get().IsInGame())
		{
			scale = KeywordHelpPanel.GAMEPLAY_SCALE;
		}
		else
		{
			scale = KeywordHelpPanel.COLLECTION_MANAGER_SCALE;
		}
		return this.ShowTooltip(headline, bodytext, scale, true);
	}

	// Token: 0x060022D1 RID: 8913 RVA: 0x000AB783 File Offset: 0x000A9983
	public void ShowGameplayTooltip(string headline, string bodytext)
	{
		this.ShowTooltip(headline, bodytext, KeywordHelpPanel.GAMEPLAY_SCALE, true);
	}

	// Token: 0x060022D2 RID: 8914 RVA: 0x000AB799 File Offset: 0x000A9999
	public void ShowGameplayTooltipLarge(string headline, string bodytext)
	{
		this.ShowTooltip(headline, bodytext, KeywordHelpPanel.GAMEPLAY_SCALE_LARGE, false);
	}

	// Token: 0x060022D3 RID: 8915 RVA: 0x000AB7AF File Offset: 0x000A99AF
	public void ShowBoxTooltip(string headline, string bodytext)
	{
		this.ShowTooltip(headline, bodytext, KeywordHelpPanel.BOX_SCALE, true);
	}

	// Token: 0x060022D4 RID: 8916 RVA: 0x000AB7C8 File Offset: 0x000A99C8
	public KeywordHelpPanel ShowLayerTooltip(string headline, string bodytext)
	{
		KeywordHelpPanel keywordHelpPanel = this.ShowTooltip(headline, bodytext, 1f, true);
		if (this.tooltipDisplayLocation == null)
		{
			return keywordHelpPanel;
		}
		keywordHelpPanel.transform.parent = this.tooltipDisplayLocation.transform;
		keywordHelpPanel.transform.localScale = Vector3.one;
		SceneUtils.SetLayer(this.m_tooltip, this.tooltipDisplayLocation.gameObject.layer);
		return keywordHelpPanel;
	}

	// Token: 0x060022D5 RID: 8917 RVA: 0x000AB83C File Offset: 0x000A9A3C
	public void ShowSocialTooltip(Component target, string headline, string bodytext, float scale, GameLayer layer)
	{
		this.ShowSocialTooltip(target.gameObject, headline, bodytext, scale, layer);
	}

	// Token: 0x060022D6 RID: 8918 RVA: 0x000AB85C File Offset: 0x000A9A5C
	public void ShowSocialTooltip(GameObject targetObject, string headline, string bodytext, float scale, GameLayer layer)
	{
		this.ShowTooltip(headline, bodytext, scale, true);
		SceneUtils.SetLayer(this.m_tooltip, layer);
		Camera camera = CameraUtils.FindFirstByLayer(targetObject.layer);
		Camera camera2 = CameraUtils.FindFirstByLayer(this.m_tooltip.layer);
		if (camera != camera2)
		{
			Vector3 vector = camera.WorldToScreenPoint(this.m_tooltip.transform.position);
			Vector3 position = camera2.ScreenToWorldPoint(vector);
			this.m_tooltip.transform.position = position;
		}
	}

	// Token: 0x060022D7 RID: 8919 RVA: 0x000AB8DC File Offset: 0x000A9ADC
	public void AnchorTooltipTo(GameObject target, Anchor targetAnchorPoint, Anchor tooltipAnchorPoint)
	{
		if (this.m_tooltip == null)
		{
			return;
		}
		TransformUtil.SetPoint(this.m_tooltip, tooltipAnchorPoint, target, targetAnchorPoint);
	}

	// Token: 0x060022D8 RID: 8920 RVA: 0x000AB909 File Offset: 0x000A9B09
	public void HideTooltip()
	{
		if (this.m_tooltip != null)
		{
			Object.Destroy(this.m_tooltip);
		}
	}

	// Token: 0x04001429 RID: 5161
	public GameObject tooltipPrefab;

	// Token: 0x0400142A RID: 5162
	public Transform tooltipDisplayLocation;

	// Token: 0x0400142B RID: 5163
	public Transform touchTooltipLocation;

	// Token: 0x0400142C RID: 5164
	public GameObject targetObject;

	// Token: 0x0400142D RID: 5165
	private GameObject m_tooltip;
}
