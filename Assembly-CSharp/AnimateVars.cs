using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000EF0 RID: 3824
public class AnimateVars : MonoBehaviour
{
	// Token: 0x0600726B RID: 29291 RVA: 0x00219C78 File Offset: 0x00217E78
	public void AnimateValue()
	{
		foreach (Renderer renderer in this.m_renderers)
		{
			if (renderer != null)
			{
				renderer.material.SetFloat(this.varName, this.amount);
			}
		}
	}

	// Token: 0x0600726C RID: 29292 RVA: 0x00219CF0 File Offset: 0x00217EF0
	private void Start()
	{
		this.m_renderers = new List<Renderer>();
		foreach (GameObject gameObject in this.m_objects)
		{
			if (!(gameObject == null))
			{
				this.m_renderers.Add(gameObject.GetComponent<Renderer>());
			}
		}
	}

	// Token: 0x0600726D RID: 29293 RVA: 0x00219D70 File Offset: 0x00217F70
	private void Update()
	{
		this.AnimateValue();
	}

	// Token: 0x04005C74 RID: 23668
	public List<GameObject> m_objects;

	// Token: 0x04005C75 RID: 23669
	public float amount;

	// Token: 0x04005C76 RID: 23670
	public string varName;

	// Token: 0x04005C77 RID: 23671
	private List<Renderer> m_renderers;
}
