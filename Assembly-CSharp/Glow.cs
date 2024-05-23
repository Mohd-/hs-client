using System;
using UnityEngine;

// Token: 0x02000899 RID: 2201
public class Glow : MonoBehaviour
{
	// Token: 0x060053D5 RID: 21461 RVA: 0x00191458 File Offset: 0x0018F658
	public void UpdateAlpha(float alpha)
	{
		base.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(1f, 1f, 1f, alpha));
	}
}
