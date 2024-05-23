using System;
using UnityEngine;

// Token: 0x0200087D RID: 2173
[Serializable]
public class GamesWonCrown
{
	// Token: 0x06005300 RID: 21248 RVA: 0x0018BD90 File Offset: 0x00189F90
	public void Show()
	{
		this.m_crown.SetActive(true);
	}

	// Token: 0x06005301 RID: 21249 RVA: 0x0018BD9E File Offset: 0x00189F9E
	public void Hide()
	{
		this.m_crown.SetActive(false);
	}

	// Token: 0x06005302 RID: 21250 RVA: 0x0018BDAC File Offset: 0x00189FAC
	public void Animate()
	{
		this.Show();
		PlayMakerFSM component = this.m_crown.GetComponent<PlayMakerFSM>();
		component.SendEvent("Birth");
	}

	// Token: 0x0400393A RID: 14650
	public GameObject m_crown;
}
