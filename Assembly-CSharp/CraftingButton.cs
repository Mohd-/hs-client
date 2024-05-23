using System;
using UnityEngine;

// Token: 0x020007B9 RID: 1977
public class CraftingButton : PegUIElement
{
	// Token: 0x06004D8A RID: 19850 RVA: 0x00171680 File Offset: 0x0016F880
	public virtual void DisableButton()
	{
		this.OnEnabled(false);
		this.buttonRenderer.material = this.disabledMaterial;
		this.labelText.Text = string.Empty;
	}

	// Token: 0x06004D8B RID: 19851 RVA: 0x001716B5 File Offset: 0x0016F8B5
	public virtual void EnterUndoMode()
	{
		this.OnEnabled(true);
		this.buttonRenderer.material = this.undoMaterial;
		this.labelText.Text = GameStrings.Get("GLUE_CRAFTING_UNDO");
	}

	// Token: 0x06004D8C RID: 19852 RVA: 0x001716E4 File Offset: 0x0016F8E4
	public virtual void EnableButton()
	{
		this.OnEnabled(true);
		this.buttonRenderer.material = this.enabledMaterial;
	}

	// Token: 0x06004D8D RID: 19853 RVA: 0x001716FE File Offset: 0x0016F8FE
	public bool IsButtonEnabled()
	{
		return this.isEnabled;
	}

	// Token: 0x06004D8E RID: 19854 RVA: 0x00171708 File Offset: 0x0016F908
	private void OnEnabled(bool enable)
	{
		this.isEnabled = enable;
		base.GetComponent<Collider>().enabled = enable;
		if (this.m_costObject != null)
		{
			if (this.m_enabledCostBone != null && this.m_disabledCostBone != null)
			{
				this.m_costObject.transform.position = ((!enable) ? this.m_disabledCostBone.position : this.m_enabledCostBone.position);
			}
			else
			{
				this.m_costObject.SetActive(enable);
			}
		}
	}

	// Token: 0x040034CA RID: 13514
	public Material undoMaterial;

	// Token: 0x040034CB RID: 13515
	public Material disabledMaterial;

	// Token: 0x040034CC RID: 13516
	public Material enabledMaterial;

	// Token: 0x040034CD RID: 13517
	public UberText labelText;

	// Token: 0x040034CE RID: 13518
	public MeshRenderer buttonRenderer;

	// Token: 0x040034CF RID: 13519
	public GameObject m_costObject;

	// Token: 0x040034D0 RID: 13520
	public Transform m_disabledCostBone;

	// Token: 0x040034D1 RID: 13521
	public Transform m_enabledCostBone;

	// Token: 0x040034D2 RID: 13522
	private bool isEnabled;
}
