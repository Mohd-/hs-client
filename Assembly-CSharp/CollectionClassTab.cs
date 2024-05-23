using System;
using System.Collections;
using UnityEngine;

// Token: 0x020006FA RID: 1786
public class CollectionClassTab : PegUIElement
{
	// Token: 0x06004984 RID: 18820 RVA: 0x0015FF68 File Offset: 0x0015E168
	public void Init(TAG_CLASS? classTag)
	{
		if (classTag != null)
		{
			this.m_classTag = classTag.Value;
		}
		this.SetClassIconsTextureOffset(base.gameObject.GetComponent<Renderer>());
		if (this.m_glowMesh != null)
		{
			this.SetClassIconsTextureOffset(this.m_glowMesh.GetComponent<Renderer>());
		}
		this.SetGlowActive(false);
		this.UpdateNewItemCount(0);
	}

	// Token: 0x06004985 RID: 18821 RVA: 0x0015FFCF File Offset: 0x0015E1CF
	public TAG_CLASS GetClass()
	{
		return this.m_classTag;
	}

	// Token: 0x06004986 RID: 18822 RVA: 0x0015FFD7 File Offset: 0x0015E1D7
	public void SetGlowActive(bool active)
	{
		if (this.m_selected)
		{
			active = true;
		}
		if (this.m_glowMesh != null)
		{
			this.m_glowMesh.SetActive(active);
		}
	}

	// Token: 0x06004987 RID: 18823 RVA: 0x00160004 File Offset: 0x0015E204
	public void SetSelected(bool selected)
	{
		if (this.m_selected == selected)
		{
			return;
		}
		this.m_selected = selected;
		this.SetGlowActive(this.m_selected);
	}

	// Token: 0x06004988 RID: 18824 RVA: 0x00160031 File Offset: 0x0015E231
	public void UpdateNewItemCount(int numNewItems)
	{
		this.m_numNewItems = numNewItems;
		this.UpdateNewItemCountVisuals();
	}

	// Token: 0x06004989 RID: 18825 RVA: 0x00160040 File Offset: 0x0015E240
	public void SetTargetLocalPosition(Vector3 targetLocalPos)
	{
		this.m_targetLocalPos = targetLocalPos;
	}

	// Token: 0x0600498A RID: 18826 RVA: 0x00160049 File Offset: 0x0015E249
	public void SetIsVisible(bool isVisible)
	{
		this.m_isVisible = isVisible;
		this.SetEnabled(this.m_isVisible);
	}

	// Token: 0x0600498B RID: 18827 RVA: 0x0016005E File Offset: 0x0015E25E
	public bool IsVisible()
	{
		return this.m_isVisible;
	}

	// Token: 0x0600498C RID: 18828 RVA: 0x00160066 File Offset: 0x0015E266
	public void SetTargetVisibility(bool visible)
	{
		this.m_shouldBeVisible = visible;
	}

	// Token: 0x0600498D RID: 18829 RVA: 0x0016006F File Offset: 0x0015E26F
	public bool ShouldBeVisible()
	{
		return this.m_shouldBeVisible;
	}

	// Token: 0x0600498E RID: 18830 RVA: 0x00160078 File Offset: 0x0015E278
	public bool WillSlide()
	{
		return Mathf.Abs(this.m_targetLocalPos.x - base.transform.localPosition.x) > 0.05f;
	}

	// Token: 0x0600498F RID: 18831 RVA: 0x001600B8 File Offset: 0x0015E2B8
	public void AnimateToTargetPosition(float animationTime, iTween.EaseType easeType)
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"position",
			this.m_targetLocalPos,
			"isLocal",
			true,
			"time",
			animationTime,
			"easetype",
			easeType,
			"name",
			"position",
			"oncomplete",
			"OnMovedToTargetPos",
			"onompletetarget",
			base.gameObject
		});
		iTween.StopByName(base.gameObject, "position");
		iTween.MoveTo(base.gameObject, args);
	}

	// Token: 0x06004990 RID: 18832 RVA: 0x00160170 File Offset: 0x0015E370
	public void SetLargeTab(bool large)
	{
		if (large == this.m_showLargeTab)
		{
			return;
		}
		if (large)
		{
			Vector3 localPosition = base.transform.localPosition;
			localPosition.y = this.m_SelectedLocalYPos;
			base.transform.localPosition = localPosition;
			Hashtable args = iTween.Hash(new object[]
			{
				"scale",
				this.m_SelectedLocalScale,
				"time",
				CollectionClassTab.SELECT_TAB_ANIM_TIME,
				"name",
				"scale"
			});
			iTween.ScaleTo(base.gameObject, args);
			SoundManager.Get().LoadAndPlay("class_tab_click", base.gameObject);
		}
		else
		{
			Vector3 localPosition2 = base.transform.localPosition;
			localPosition2.y = this.m_DeselectedLocalYPos;
			base.transform.localPosition = localPosition2;
			iTween.StopByName(base.gameObject, "scale");
			base.transform.localScale = this.m_DeselectedLocalScale;
		}
		this.m_showLargeTab = large;
	}

	// Token: 0x06004991 RID: 18833 RVA: 0x00160270 File Offset: 0x0015E470
	private static Vector2 GetTextureOffset(TAG_CLASS classTag)
	{
		if (CollectionPageManager.s_classTextureOffsets.ContainsKey(classTag))
		{
			return CollectionPageManager.s_classTextureOffsets[classTag];
		}
		Debug.LogWarning(string.Format("CollectionClassTab.GetTextureOffset(): No class texture offsets exist for class {0}", classTag));
		if (CollectionPageManager.s_classTextureOffsets.ContainsKey(TAG_CLASS.INVALID))
		{
			return CollectionPageManager.s_classTextureOffsets[TAG_CLASS.INVALID];
		}
		return Vector2.zero;
	}

	// Token: 0x06004992 RID: 18834 RVA: 0x001602D0 File Offset: 0x0015E4D0
	private void SetClassIconsTextureOffset(Renderer renderer)
	{
		if (renderer == null)
		{
			return;
		}
		Vector2 textureOffset = CollectionClassTab.GetTextureOffset(this.m_classTag);
		foreach (Material material in renderer.materials)
		{
			if (!(material.mainTexture == null))
			{
				if (material.mainTexture.name.Contains(CollectionClassTab.CLASS_ICONS_TEXTURE_NAME))
				{
					material.mainTextureOffset = textureOffset;
				}
			}
		}
	}

	// Token: 0x06004993 RID: 18835 RVA: 0x00160354 File Offset: 0x0015E554
	private void UpdateNewItemCountVisuals()
	{
		if (this.m_newItemCountText != null)
		{
			this.m_newItemCountText.Text = GameStrings.Format("GLUE_COLLECTION_NEW_CARD_CALLOUT", new object[]
			{
				this.m_numNewItems
			});
		}
		if (this.m_newItemCount != null)
		{
			this.m_newItemCount.SetActive(this.m_numNewItems > 0);
		}
	}

	// Token: 0x06004994 RID: 18836 RVA: 0x001603C0 File Offset: 0x0015E5C0
	private void OnMovedToTargetPos()
	{
		if (this.m_showLargeTab)
		{
			return;
		}
		Vector3 localPosition = base.transform.localPosition;
		localPosition.y = this.m_DeselectedLocalYPos;
		base.transform.localPosition = localPosition;
	}

	// Token: 0x040030B4 RID: 12468
	public GameObject m_glowMesh;

	// Token: 0x040030B5 RID: 12469
	public GameObject m_newItemCount;

	// Token: 0x040030B6 RID: 12470
	public UberText m_newItemCountText;

	// Token: 0x040030B7 RID: 12471
	public CollectionManagerDisplay.ViewMode m_tabViewMode;

	// Token: 0x040030B8 RID: 12472
	public Vector3 m_DeselectedLocalScale = new Vector3(0.44f, 0.44f, 0.44f);

	// Token: 0x040030B9 RID: 12473
	public Vector3 m_SelectedLocalScale = new Vector3(0.66f, 0.66f, 0.66f);

	// Token: 0x040030BA RID: 12474
	public float m_SelectedLocalYPos = 0.1259841f;

	// Token: 0x040030BB RID: 12475
	public float m_DeselectedLocalYPos;

	// Token: 0x040030BC RID: 12476
	private static readonly string CLASS_ICONS_TEXTURE_NAME = "ClassIcons";

	// Token: 0x040030BD RID: 12477
	private TAG_CLASS m_classTag;

	// Token: 0x040030BE RID: 12478
	private int m_numNewItems;

	// Token: 0x040030BF RID: 12479
	private bool m_selected;

	// Token: 0x040030C0 RID: 12480
	private Vector3 m_targetLocalPos;

	// Token: 0x040030C1 RID: 12481
	private bool m_shouldBeVisible = true;

	// Token: 0x040030C2 RID: 12482
	private bool m_isVisible = true;

	// Token: 0x040030C3 RID: 12483
	private bool m_showLargeTab;

	// Token: 0x040030C4 RID: 12484
	public static readonly float SELECT_TAB_ANIM_TIME = 0.2f;
}
