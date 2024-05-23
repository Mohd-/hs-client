using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200025E RID: 606
public class RibbonButtonsUI : MonoBehaviour
{
	// Token: 0x0600223A RID: 8762 RVA: 0x000A84E0 File Offset: 0x000A66E0
	public void Awake()
	{
		this.m_rootObject.SetActive(false);
		float num = 1f - TransformUtil.PhoneAspectRatioScale();
		float num2 = num * this.m_minAspectRatioAdjustment;
		TransformUtil.SetLocalPosX(this.m_LeftBones, this.m_LeftBones.localPosition.x + num2);
		TransformUtil.SetLocalPosX(this.m_RightBones, this.m_RightBones.localPosition.x - num2);
	}

	// Token: 0x0600223B RID: 8763 RVA: 0x000A8550 File Offset: 0x000A6750
	public void Toggle(bool show)
	{
		this.m_shown = show;
		if (show)
		{
			base.StartCoroutine(this.ShowRibbons());
		}
		else
		{
			base.StartCoroutine(this.HideRibbons());
		}
	}

	// Token: 0x0600223C RID: 8764 RVA: 0x000A858C File Offset: 0x000A678C
	private IEnumerator ShowRibbons()
	{
		this.m_rootObject.SetActive(false);
		float startDelay = 1f;
		foreach (RibbonButtonsUI.RibbonButtonObject ribbon in this.m_Ribbons)
		{
			if (ribbon.m_AnimateInDelay < startDelay)
			{
				startDelay = ribbon.m_AnimateInDelay;
			}
		}
		yield return new WaitForSeconds(startDelay);
		this.m_rootObject.SetActive(true);
		foreach (RibbonButtonsUI.RibbonButtonObject ribbon2 in this.m_Ribbons)
		{
			ribbon2.m_Ribbon.transform.position = ribbon2.m_HiddenBone.position;
			iTween.Stop(ribbon2.m_Ribbon.gameObject);
			Hashtable args = iTween.Hash(new object[]
			{
				"position",
				ribbon2.m_ShownBone.position,
				"delay",
				ribbon2.m_AnimateInDelay - startDelay,
				"time",
				this.m_EaseInTime,
				"easeType",
				iTween.EaseType.easeOutBack
			});
			iTween.MoveTo(ribbon2.m_Ribbon.gameObject, args);
		}
		yield break;
	}

	// Token: 0x0600223D RID: 8765 RVA: 0x000A85A8 File Offset: 0x000A67A8
	private IEnumerator HideRibbons()
	{
		foreach (RibbonButtonsUI.RibbonButtonObject ribbon in this.m_Ribbons)
		{
			ribbon.m_Ribbon.transform.position = ribbon.m_ShownBone.position;
			iTween.Stop(ribbon.m_Ribbon.gameObject);
			Hashtable args = iTween.Hash(new object[]
			{
				"position",
				ribbon.m_HiddenBone.position,
				"delay",
				0f,
				"time",
				this.m_EaseOutTime,
				"easeType",
				iTween.EaseType.easeInOutBack
			});
			iTween.MoveTo(ribbon.m_Ribbon.gameObject, args);
		}
		yield return new WaitForSeconds(this.m_EaseOutTime);
		if (!this.m_shown)
		{
			this.m_rootObject.SetActive(false);
		}
		yield break;
	}

	// Token: 0x0600223E RID: 8766 RVA: 0x000A85C4 File Offset: 0x000A67C4
	public void SetPackCount(int packs)
	{
		if (packs <= 0)
		{
			this.m_packCount.Text = string.Empty;
			this.m_packCountFrame.SetActive(false);
		}
		else
		{
			this.m_packCount.Text = GameStrings.Format("GLUE_PACK_OPENING_BOOSTER_COUNT", new object[]
			{
				packs
			});
			this.m_packCountFrame.SetActive(true);
		}
	}

	// Token: 0x04001393 RID: 5011
	public List<RibbonButtonsUI.RibbonButtonObject> m_Ribbons;

	// Token: 0x04001394 RID: 5012
	public Transform m_LeftBones;

	// Token: 0x04001395 RID: 5013
	public Transform m_RightBones;

	// Token: 0x04001396 RID: 5014
	public float m_EaseInTime = 1f;

	// Token: 0x04001397 RID: 5015
	public float m_EaseOutTime = 0.4f;

	// Token: 0x04001398 RID: 5016
	public GameObject m_rootObject;

	// Token: 0x04001399 RID: 5017
	public PegUIElement m_collectionManagerRibbon;

	// Token: 0x0400139A RID: 5018
	public PegUIElement m_questLogRibbon;

	// Token: 0x0400139B RID: 5019
	public PegUIElement m_packOpeningRibbon;

	// Token: 0x0400139C RID: 5020
	public PegUIElement m_storeRibbon;

	// Token: 0x0400139D RID: 5021
	public UberText m_packCount;

	// Token: 0x0400139E RID: 5022
	public GameObject m_packCountFrame;

	// Token: 0x0400139F RID: 5023
	public float m_minAspectRatioAdjustment = 0.24f;

	// Token: 0x040013A0 RID: 5024
	private bool m_shown = true;

	// Token: 0x0200067F RID: 1663
	[Serializable]
	public class RibbonButtonObject
	{
		// Token: 0x04002DD4 RID: 11732
		public PegUIElement m_Ribbon;

		// Token: 0x04002DD5 RID: 11733
		public Transform m_HiddenBone;

		// Token: 0x04002DD6 RID: 11734
		public Transform m_ShownBone;

		// Token: 0x04002DD7 RID: 11735
		public bool m_LeftSide;

		// Token: 0x04002DD8 RID: 11736
		public float m_AnimateInDelay;
	}
}
