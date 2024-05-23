using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F4C RID: 3916
public class TentacleFX : MonoBehaviour
{
	// Token: 0x0600748C RID: 29836 RVA: 0x002256B4 File Offset: 0x002238B4
	private void Start()
	{
		foreach (GameObject gameObject in this.m_Tentacles)
		{
			Material material = gameObject.GetComponent<Renderer>().material;
			material.SetFloat("_Seed", Random.Range(0f, 2f));
		}
		if (this.doRotate)
		{
			this.m_TentacleRoot.transform.Rotate(Vector3.up, Random.Range(0f, 360f), 1);
		}
	}

	// Token: 0x04005EF2 RID: 24306
	public GameObject m_TentacleRoot;

	// Token: 0x04005EF3 RID: 24307
	public List<GameObject> m_Tentacles;

	// Token: 0x04005EF4 RID: 24308
	public bool doRotate = true;
}
