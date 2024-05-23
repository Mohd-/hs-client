using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000818 RID: 2072
public class GameToast : MonoBehaviour
{
	// Token: 0x06004FE7 RID: 20455 RVA: 0x0017B224 File Offset: 0x00179424
	private void Start()
	{
		this.UpdateIntensity(16f);
		Hashtable args = iTween.Hash(new object[]
		{
			"time",
			0.5f,
			"from",
			16f,
			"to",
			1f,
			"delay",
			0.25f,
			"easetype",
			iTween.EaseType.easeOutCubic,
			"onupdate",
			"UpdateIntensity"
		});
		iTween.ValueTo(base.gameObject, args);
	}

	// Token: 0x06004FE8 RID: 20456 RVA: 0x0017B2D0 File Offset: 0x001794D0
	private void UpdateIntensity(float intensity)
	{
		foreach (Material material in this.m_intensityMaterials)
		{
			material.SetFloat("_Intensity", intensity);
		}
	}

	// Token: 0x040036B7 RID: 14007
	public List<Material> m_intensityMaterials = new List<Material>();
}
