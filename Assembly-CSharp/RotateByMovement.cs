using System;
using UnityEngine;

// Token: 0x02000F33 RID: 3891
public class RotateByMovement : MonoBehaviour
{
	// Token: 0x060073C0 RID: 29632 RVA: 0x00221724 File Offset: 0x0021F924
	private void Update()
	{
		Transform transform = this.mParent.transform;
		if (this.m_previousPos == transform.localPosition)
		{
			return;
		}
		if (this.m_previousPos == Vector3.zero)
		{
			this.m_previousPos = transform.localPosition;
			return;
		}
		Vector3 localPosition = transform.localPosition;
		float num = localPosition.z - this.m_previousPos.z;
		float num2 = localPosition.x - this.m_previousPos.x;
		float num3 = Mathf.Sqrt(Mathf.Pow(num2, 2f) + Mathf.Pow(num, 2f));
		float num4 = Mathf.Asin(num / num3) * 180f / 3.1415927f;
		num4 -= 90f;
		base.transform.localEulerAngles = new Vector3(90f, num4, 0f);
		this.m_previousPos = localPosition;
	}

	// Token: 0x04005E56 RID: 24150
	private Vector3 m_previousPos;

	// Token: 0x04005E57 RID: 24151
	public GameObject mParent;
}
