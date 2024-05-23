using System;
using UnityEngine;

// Token: 0x02000F17 RID: 3863
[RequireComponent(typeof(Material))]
public class MaterialSoftFlicker : MonoBehaviour
{
	// Token: 0x0600733F RID: 29503 RVA: 0x0021F41F File Offset: 0x0021D61F
	private void Start()
	{
		this.random = Random.Range(0f, 65535f);
	}

	// Token: 0x06007340 RID: 29504 RVA: 0x0021F438 File Offset: 0x0021D638
	private void Update()
	{
		float num = Mathf.PerlinNoise(this.random, Time.time * this.m_timeScale);
		base.gameObject.GetComponent<Renderer>().material.SetColor("_TintColor", new Color(this.m_color.r, this.m_color.g, this.m_color.b, Mathf.Lerp(this.minIntensity, this.maxIntensity, num)));
	}

	// Token: 0x04005DBE RID: 23998
	public float minIntensity = 0.25f;

	// Token: 0x04005DBF RID: 23999
	public float maxIntensity = 0.5f;

	// Token: 0x04005DC0 RID: 24000
	public float m_timeScale = 1f;

	// Token: 0x04005DC1 RID: 24001
	public Color m_color = new Color(1f, 1f, 1f, 1f);

	// Token: 0x04005DC2 RID: 24002
	private float random;
}
