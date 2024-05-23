using System;
using UnityEngine;

// Token: 0x020001AC RID: 428
[CustomEditClass]
public class AdventureBossCoin : PegUIElement
{
	// Token: 0x06001C1B RID: 7195 RVA: 0x000847AC File Offset: 0x000829AC
	public void SetPortraitMaterial(Material mat)
	{
		if (this.m_PortraitRenderer != null && this.m_PortraitMaterialIndex < this.m_PortraitRenderer.materials.Length)
		{
			Material[] materials = this.m_PortraitRenderer.materials;
			materials[this.m_PortraitMaterialIndex] = mat;
			this.m_PortraitRenderer.materials = materials;
		}
	}

	// Token: 0x06001C1C RID: 7196 RVA: 0x00084803 File Offset: 0x00082A03
	public void ShowConnector(bool show)
	{
		if (this.m_Connector != null)
		{
			this.m_Connector.SetActive(show);
		}
	}

	// Token: 0x06001C1D RID: 7197 RVA: 0x00084824 File Offset: 0x00082A24
	public void Enable(bool flag, bool animate = true)
	{
		base.GetComponent<Collider>().enabled = flag;
		if (this.m_DisabledCollider != null)
		{
			this.m_DisabledCollider.gameObject.SetActive(!flag);
		}
		if (this.m_Enabled == flag)
		{
			return;
		}
		this.m_Enabled = flag;
		if (animate && flag)
		{
			this.ShowCoin(false);
			this.m_CoinStateTable.TriggerState("Flip", true, null);
		}
		else
		{
			this.ShowCoin(flag);
		}
	}

	// Token: 0x06001C1E RID: 7198 RVA: 0x000848A8 File Offset: 0x00082AA8
	public void Select(bool selected)
	{
		UIBHighlight component = base.GetComponent<UIBHighlight>();
		if (component == null)
		{
			return;
		}
		component.AlwaysOver = selected;
		if (selected)
		{
			this.EnableFancyHighlight(false);
		}
	}

	// Token: 0x06001C1F RID: 7199 RVA: 0x000848E0 File Offset: 0x00082AE0
	public void HighlightOnce()
	{
		UIBHighlight component = base.GetComponent<UIBHighlight>();
		if (component == null)
		{
			return;
		}
		component.HighlightOnce();
	}

	// Token: 0x06001C20 RID: 7200 RVA: 0x00084907 File Offset: 0x00082B07
	public void ShowNewLookGlow()
	{
		this.EnableFancyHighlight(true);
	}

	// Token: 0x06001C21 RID: 7201 RVA: 0x00084910 File Offset: 0x00082B10
	private void EnableFancyHighlight(bool enable)
	{
		UIBHighlightStateControl component = base.GetComponent<UIBHighlightStateControl>();
		if (component == null)
		{
			return;
		}
		component.Select(enable, false);
	}

	// Token: 0x06001C22 RID: 7202 RVA: 0x0008493C File Offset: 0x00082B3C
	private void ShowCoin(bool show)
	{
		if (this.m_Coin == null)
		{
			return;
		}
		TransformUtil.SetEulerAngleZ(this.m_Coin, (!show) ? -180f : 0f);
	}

	// Token: 0x04000EBA RID: 3770
	private const string s_EventCoinFlip = "Flip";

	// Token: 0x04000EBB RID: 3771
	public GameObject m_Coin;

	// Token: 0x04000EBC RID: 3772
	public MeshRenderer m_PortraitRenderer;

	// Token: 0x04000EBD RID: 3773
	public int m_PortraitMaterialIndex = 1;

	// Token: 0x04000EBE RID: 3774
	public GameObject m_Connector;

	// Token: 0x04000EBF RID: 3775
	public StateEventTable m_CoinStateTable;

	// Token: 0x04000EC0 RID: 3776
	public PegUIElement m_DisabledCollider;

	// Token: 0x04000EC1 RID: 3777
	private bool m_Enabled;
}
