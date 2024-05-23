using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000EEF RID: 3823
public class AnimateTransitions : MonoBehaviour
{
	// Token: 0x06007267 RID: 29287 RVA: 0x00219B7C File Offset: 0x00217D7C
	public void StartTransitions()
	{
		foreach (Renderer renderer in this.rend)
		{
			renderer.material.SetFloat("_Transistion", this.amount);
		}
	}

	// Token: 0x06007268 RID: 29288 RVA: 0x00219BE8 File Offset: 0x00217DE8
	private void Start()
	{
		this.rend = new List<Renderer>();
		foreach (GameObject gameObject in this.m_TargetList)
		{
			if (!(gameObject == null))
			{
				this.rend.Add(gameObject.GetComponent<Renderer>());
			}
		}
	}

	// Token: 0x06007269 RID: 29289 RVA: 0x00219C68 File Offset: 0x00217E68
	private void Update()
	{
		this.StartTransitions();
	}

	// Token: 0x04005C71 RID: 23665
	public List<GameObject> m_TargetList;

	// Token: 0x04005C72 RID: 23666
	public float amount;

	// Token: 0x04005C73 RID: 23667
	private List<Renderer> rend;
}
