using System;
using UnityEngine;

// Token: 0x02000F51 RID: 3921
public class WeaponUVWorldspace : MonoBehaviour
{
	// Token: 0x060074AE RID: 29870 RVA: 0x00226F10 File Offset: 0x00225110
	private void Start()
	{
	}

	// Token: 0x060074AF RID: 29871 RVA: 0x00226F14 File Offset: 0x00225114
	private void Update()
	{
		Vector3 vector = base.transform.position * 0.7f;
		Material material = base.gameObject.GetComponent<Renderer>().material;
		material.SetFloat("_OffsetX", -vector.z - this.xOffset);
		material.SetFloat("_OffsetY", -vector.x - this.yOffset);
	}

	// Token: 0x04005F16 RID: 24342
	public float xOffset;

	// Token: 0x04005F17 RID: 24343
	public float yOffset;
}
