using System;
using UnityEngine;

// Token: 0x02000F23 RID: 3875
public class PremiumMaterialSwitcher : MonoBehaviour
{
	// Token: 0x0600737B RID: 29563 RVA: 0x0022033F File Offset: 0x0021E53F
	private void Start()
	{
	}

	// Token: 0x0600737C RID: 29564 RVA: 0x00220344 File Offset: 0x0021E544
	public void SetToPremium(int premium)
	{
		if (premium < 1)
		{
			if (base.GetComponent<Renderer>().materials == null || this.OrgMaterials == null)
			{
				return;
			}
			Material[] materials = base.GetComponent<Renderer>().materials;
			int num = 0;
			while (num < this.m_PremiumMaterials.Length && num < materials.Length)
			{
				if (!(this.m_PremiumMaterials[num] == null))
				{
					materials[num] = this.OrgMaterials[num];
				}
				num++;
			}
			base.GetComponent<Renderer>().materials = materials;
			this.OrgMaterials = null;
			return;
		}
		else
		{
			if (this.m_PremiumMaterials.Length < 1)
			{
				return;
			}
			if (this.OrgMaterials == null)
			{
				this.OrgMaterials = base.GetComponent<Renderer>().materials;
			}
			Material[] materials2 = base.GetComponent<Renderer>().materials;
			int num2 = 0;
			while (num2 < this.m_PremiumMaterials.Length && num2 < materials2.Length)
			{
				if (!(this.m_PremiumMaterials[num2] == null))
				{
					materials2[num2] = this.m_PremiumMaterials[num2];
				}
				num2++;
			}
			base.GetComponent<Renderer>().materials = materials2;
			return;
		}
	}

	// Token: 0x04005E07 RID: 24071
	public Material[] m_PremiumMaterials;

	// Token: 0x04005E08 RID: 24072
	private Material[] OrgMaterials;
}
