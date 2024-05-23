using System;
using UnityEngine;

// Token: 0x02000F19 RID: 3865
public class MoveToTarget : MonoBehaviour
{
	// Token: 0x06007348 RID: 29512 RVA: 0x0021F5DB File Offset: 0x0021D7DB
	private void Start()
	{
		if (this.m_AnimateOnStart)
		{
			this.StartAnimation();
		}
	}

	// Token: 0x06007349 RID: 29513 RVA: 0x0021F5EE File Offset: 0x0021D7EE
	private void Update()
	{
		if (this.m_MoveType == MoveToTarget.MoveType.MoveByTime)
		{
			this.MoveTime();
		}
		else
		{
			this.MoveSpeed();
		}
	}

	// Token: 0x0600734A RID: 29514 RVA: 0x0021F60C File Offset: 0x0021D80C
	private void MoveTime()
	{
		if (this.m_isDone)
		{
			base.transform.position = this.m_TargetObject.position;
		}
		if (!this.m_Animate)
		{
			return;
		}
		Vector3 position = this.m_TargetObject.position;
		float num = 1f / this.m_Time;
		this.m_LerpPosition += num * Time.deltaTime;
		if (this.m_LerpPosition > 1f)
		{
			this.m_isDone = true;
			base.transform.position = this.m_TargetObject.position;
			return;
		}
		Vector3 position2 = Vector3.Lerp(this.m_StartPosition.position, position, this.m_LerpPosition);
		base.transform.position = position2;
	}

	// Token: 0x0600734B RID: 29515 RVA: 0x0021F6C8 File Offset: 0x0021D8C8
	private void MoveSpeed()
	{
		if (this.m_isDone)
		{
			base.transform.position = this.m_TargetObject.position;
		}
		if (!this.m_Animate)
		{
			return;
		}
		float num = Vector3.Distance(base.transform.position, this.m_TargetObject.position);
		if (num < this.m_SnapDistance)
		{
			this.m_isDone = true;
			base.transform.position = this.m_TargetObject.position;
			return;
		}
		Vector3 vector = this.m_TargetObject.position - base.transform.position;
		vector.Normalize();
		float num2 = this.m_Speed * Time.deltaTime;
		base.transform.position = base.transform.position + vector * num2;
	}

	// Token: 0x0600734C RID: 29516 RVA: 0x0021F79C File Offset: 0x0021D99C
	private void StartAnimation()
	{
		if (this.m_StartPosition)
		{
			base.transform.position = this.m_StartPosition.position;
		}
		this.m_Animate = true;
		this.m_LerpPosition = 0f;
	}

	// Token: 0x04005DCA RID: 24010
	public Transform m_StartPosition;

	// Token: 0x04005DCB RID: 24011
	public Transform m_TargetObject;

	// Token: 0x04005DCC RID: 24012
	public MoveToTarget.MoveType m_MoveType;

	// Token: 0x04005DCD RID: 24013
	public float m_Time = 1f;

	// Token: 0x04005DCE RID: 24014
	public float m_Speed = 1f;

	// Token: 0x04005DCF RID: 24015
	public float m_SnapDistance = 0.1f;

	// Token: 0x04005DD0 RID: 24016
	public bool m_OrientToPath;

	// Token: 0x04005DD1 RID: 24017
	public bool m_AnimateOnStart;

	// Token: 0x04005DD2 RID: 24018
	private bool m_Animate;

	// Token: 0x04005DD3 RID: 24019
	private bool m_isDone;

	// Token: 0x04005DD4 RID: 24020
	private Vector3 m_LastTargetPosition;

	// Token: 0x04005DD5 RID: 24021
	private float m_LerpPosition;

	// Token: 0x02000F1A RID: 3866
	public enum MoveType
	{
		// Token: 0x04005DD7 RID: 24023
		MoveByTime,
		// Token: 0x04005DD8 RID: 24024
		MoveBySpeed
	}
}
