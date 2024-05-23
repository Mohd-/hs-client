using System;
using UnityEngine;

// Token: 0x020005BC RID: 1468
public class CraftCardCountTab : MonoBehaviour
{
	// Token: 0x0600417D RID: 16765 RVA: 0x0013BAB1 File Offset: 0x00139CB1
	private void Awake()
	{
		this.origPos = this.m_count.transform.localPosition;
	}

	// Token: 0x0600417E RID: 16766 RVA: 0x0013BACC File Offset: 0x00139CCC
	public void UpdateText(int numCopies)
	{
		if (numCopies > 9)
		{
			this.m_count.Text = "9";
			this.m_plus.gameObject.SetActive(true);
			this.m_count.transform.localPosition = new Vector3(0.08628464f, this.origPos.y, this.origPos.z);
			return;
		}
		if (numCopies >= 2)
		{
			this.m_shadow.SetActive(true);
			this.m_shadow.GetComponent<Animation>().Play("Crafting2ndCardShadow");
		}
		else
		{
			this.m_shadow.SetActive(false);
		}
		this.m_count.Text = numCopies.ToString();
		this.m_plus.gameObject.SetActive(false);
		this.m_count.transform.localPosition = this.origPos;
	}

	// Token: 0x040029A8 RID: 10664
	public UberText m_count;

	// Token: 0x040029A9 RID: 10665
	public UberText m_plus;

	// Token: 0x040029AA RID: 10666
	public GameObject m_shadow;

	// Token: 0x040029AB RID: 10667
	private Vector3 origPos = new Vector3(0f, 0f, 0f);
}
