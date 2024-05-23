using System;
using UnityEngine;

// Token: 0x02000F32 RID: 3890
public class RotateAnim : MonoBehaviour
{
	// Token: 0x060073BC RID: 29628 RVA: 0x00221605 File Offset: 0x0021F805
	private void Start()
	{
	}

	// Token: 0x060073BD RID: 29629 RVA: 0x00221608 File Offset: 0x0021F808
	private void Update()
	{
		if (!this.gogogo)
		{
			return;
		}
		this.timePassed += Time.deltaTime;
		float num = this.timePassed;
		float num2 = this.startingAngle;
		float num3 = num2 - Quaternion.Angle(base.transform.rotation, this.targetRotation);
		float num4 = this.timeValue;
		float num5 = num3 * (-Mathf.Pow(2f, -10f * num / num4) + 1f) + num2;
		base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, this.targetRotation, num5 * Time.deltaTime);
		if (Quaternion.Angle(base.transform.rotation, this.targetRotation) <= Mathf.Epsilon)
		{
			this.gogogo = false;
			Object.Destroy(this);
		}
	}

	// Token: 0x060073BE RID: 29630 RVA: 0x002216D8 File Offset: 0x0021F8D8
	public void SetTargetRotation(Vector3 target, float timeValueInput)
	{
		this.targetRotation = Quaternion.Euler(target);
		this.gogogo = true;
		this.timeValue = timeValueInput;
		this.startingAngle = Quaternion.Angle(base.transform.rotation, this.targetRotation);
	}

	// Token: 0x04005E51 RID: 24145
	private Quaternion targetRotation;

	// Token: 0x04005E52 RID: 24146
	private bool gogogo;

	// Token: 0x04005E53 RID: 24147
	private float timeValue;

	// Token: 0x04005E54 RID: 24148
	private float timePassed;

	// Token: 0x04005E55 RID: 24149
	private float startingAngle;
}
