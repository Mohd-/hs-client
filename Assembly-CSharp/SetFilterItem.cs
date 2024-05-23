using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006C4 RID: 1732
public class SetFilterItem : PegUIElement
{
	// Token: 0x170004F9 RID: 1273
	// (get) Token: 0x0600482B RID: 18475 RVA: 0x0015A70D File Offset: 0x0015890D
	// (set) Token: 0x0600482C RID: 18476 RVA: 0x0015A715 File Offset: 0x00158915
	public bool IsHeader
	{
		get
		{
			return this.m_isHeader;
		}
		set
		{
			this.m_isHeader = value;
		}
	}

	// Token: 0x170004FA RID: 1274
	// (get) Token: 0x0600482D RID: 18477 RVA: 0x0015A71E File Offset: 0x0015891E
	// (set) Token: 0x0600482E RID: 18478 RVA: 0x0015A72B File Offset: 0x0015892B
	public string Text
	{
		get
		{
			return this.m_uberText.Text;
		}
		set
		{
			this.m_uberText.Text = value;
		}
	}

	// Token: 0x170004FB RID: 1275
	// (get) Token: 0x0600482F RID: 18479 RVA: 0x0015A739 File Offset: 0x00158939
	// (set) Token: 0x06004830 RID: 18480 RVA: 0x0015A741 File Offset: 0x00158941
	public bool IsWild
	{
		get
		{
			return this.m_isWild;
		}
		set
		{
			this.m_isWild = value;
		}
	}

	// Token: 0x170004FC RID: 1276
	// (get) Token: 0x06004831 RID: 18481 RVA: 0x0015A74A File Offset: 0x0015894A
	// (set) Token: 0x06004832 RID: 18482 RVA: 0x0015A752 File Offset: 0x00158952
	public bool IsAllStandard
	{
		get
		{
			return this.m_isAllStandard;
		}
		set
		{
			this.m_isAllStandard = value;
		}
	}

	// Token: 0x170004FD RID: 1277
	// (get) Token: 0x06004833 RID: 18483 RVA: 0x0015A75B File Offset: 0x0015895B
	// (set) Token: 0x06004834 RID: 18484 RVA: 0x0015A763 File Offset: 0x00158963
	public List<TAG_CARD_SET> CardSets
	{
		get
		{
			return this.m_cardSets;
		}
		set
		{
			this.m_cardSets = value;
		}
	}

	// Token: 0x170004FE RID: 1278
	// (get) Token: 0x06004835 RID: 18485 RVA: 0x0015A76C File Offset: 0x0015896C
	// (set) Token: 0x06004836 RID: 18486 RVA: 0x0015A774 File Offset: 0x00158974
	public float Height
	{
		get
		{
			return this.m_height;
		}
		set
		{
			this.m_height = value;
		}
	}

	// Token: 0x170004FF RID: 1279
	// (get) Token: 0x06004837 RID: 18487 RVA: 0x0015A77D File Offset: 0x0015897D
	// (set) Token: 0x06004838 RID: 18488 RVA: 0x0015A785 File Offset: 0x00158985
	public SetFilterItem.ItemSelectedCallback Callback
	{
		get
		{
			return this.m_callback;
		}
		set
		{
			this.m_callback = value;
		}
	}

	// Token: 0x17000500 RID: 1280
	// (get) Token: 0x06004839 RID: 18489 RVA: 0x0015A78E File Offset: 0x0015898E
	// (set) Token: 0x0600483A RID: 18490 RVA: 0x0015A7AC File Offset: 0x001589AC
	public Vector2? IconOffset
	{
		get
		{
			return new Vector2?(this.m_icon.material.GetTextureOffset("_MainTex"));
		}
		set
		{
			if (value == null)
			{
				this.m_icon.gameObject.SetActive(false);
			}
			else
			{
				this.m_icon.gameObject.SetActive(true);
				this.m_icon.material.SetTextureOffset("_MainTex", value.Value);
			}
		}
	}

	// Token: 0x0600483B RID: 18491 RVA: 0x0015A808 File Offset: 0x00158A08
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		if (this.m_mouseOverGlow != null)
		{
			this.m_mouseOverGlow.SetActive(true);
			SoundManager.Get().LoadAndPlay("Small_Mouseover", base.gameObject);
		}
	}

	// Token: 0x0600483C RID: 18492 RVA: 0x0015A848 File Offset: 0x00158A48
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		if (this.m_mouseOverGlow != null)
		{
			this.m_mouseOverGlow.SetActive(false);
		}
		if (!this.m_isSelected && this.m_pressedShadow != null)
		{
			this.m_pressedShadow.SetActive(false);
		}
	}

	// Token: 0x0600483D RID: 18493 RVA: 0x0015A89C File Offset: 0x00158A9C
	public void SetSelected(bool selected)
	{
		this.m_selectedGlow.SetActive(selected);
		if (this.m_pressedShadow != null)
		{
			this.m_pressedShadow.SetActive(selected);
		}
		this.m_isSelected = selected;
	}

	// Token: 0x0600483E RID: 18494 RVA: 0x0015A8DC File Offset: 0x00158ADC
	protected override void OnPress()
	{
		if (this.m_pressedShadow != null)
		{
			this.m_pressedShadow.SetActive(true);
		}
		SoundManager.Get().LoadAndPlay("Small_Click");
	}

	// Token: 0x0600483F RID: 18495 RVA: 0x0015A918 File Offset: 0x00158B18
	protected override void OnRelease()
	{
		if (!this.m_isSelected && this.m_pressedShadow != null)
		{
			this.m_pressedShadow.SetActive(false);
		}
	}

	// Token: 0x04002F8D RID: 12173
	public UberText m_uberText;

	// Token: 0x04002F8E RID: 12174
	public GameObject m_selectedGlow;

	// Token: 0x04002F8F RID: 12175
	public MeshRenderer m_icon;

	// Token: 0x04002F90 RID: 12176
	public GameObject m_mouseOverGlow;

	// Token: 0x04002F91 RID: 12177
	public GameObject m_pressedShadow;

	// Token: 0x04002F92 RID: 12178
	private bool m_isHeader;

	// Token: 0x04002F93 RID: 12179
	private bool m_isWild;

	// Token: 0x04002F94 RID: 12180
	private bool m_isAllStandard;

	// Token: 0x04002F95 RID: 12181
	private List<TAG_CARD_SET> m_cardSets;

	// Token: 0x04002F96 RID: 12182
	private float m_height;

	// Token: 0x04002F97 RID: 12183
	private SetFilterItem.ItemSelectedCallback m_callback;

	// Token: 0x04002F98 RID: 12184
	private bool m_isSelected;

	// Token: 0x020006C5 RID: 1733
	// (Invoke) Token: 0x06004841 RID: 18497
	public delegate void ItemSelectedCallback(object data, bool isWild);
}
