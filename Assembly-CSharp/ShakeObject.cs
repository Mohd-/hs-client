using System;
using UnityEngine;

// Token: 0x02000F3F RID: 3903
public class ShakeObject : MonoBehaviour
{
	// Token: 0x060073FC RID: 29692 RVA: 0x0022250B File Offset: 0x0022070B
	private void Start()
	{
		this.orgPos = base.transform.position;
	}

	// Token: 0x060073FD RID: 29693 RVA: 0x00222520 File Offset: 0x00220720
	private void Update()
	{
		float num = Random.value * this.amount;
		float num2 = Random.value * this.amount;
		float num3 = Random.value * this.amount;
		num *= this.amount;
		num2 *= this.amount;
		num3 *= this.amount;
		base.transform.position = this.orgPos + new Vector3(num, num2, num3);
	}

	// Token: 0x04005E8D RID: 24205
	public float amount = 1f;

	// Token: 0x04005E8E RID: 24206
	private Vector3 orgPos;
}
