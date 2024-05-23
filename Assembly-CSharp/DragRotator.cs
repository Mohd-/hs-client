using System;
using UnityEngine;

// Token: 0x02000781 RID: 1921
public class DragRotator : MonoBehaviour
{
	// Token: 0x06004C64 RID: 19556 RVA: 0x0016B988 File Offset: 0x00169B88
	private void Awake()
	{
		this.m_prevPos = base.transform.position;
		this.m_originalAngles = base.transform.localRotation.eulerAngles;
	}

	// Token: 0x06004C65 RID: 19557 RVA: 0x0016B9C0 File Offset: 0x00169BC0
	private void Update()
	{
		Vector3 position = base.transform.position;
		Vector3 vector = position - this.m_prevPos;
		if (vector.sqrMagnitude > 0.0001f)
		{
			this.m_pitchDeg += vector.z * this.m_info.m_PitchInfo.m_ForceMultiplier;
			this.m_pitchDeg = Mathf.Clamp(this.m_pitchDeg, this.m_info.m_PitchInfo.m_MinDegrees, this.m_info.m_PitchInfo.m_MaxDegrees);
			this.m_rollDeg -= vector.x * this.m_info.m_RollInfo.m_ForceMultiplier;
			this.m_rollDeg = Mathf.Clamp(this.m_rollDeg, this.m_info.m_RollInfo.m_MinDegrees, this.m_info.m_RollInfo.m_MaxDegrees);
		}
		this.m_pitchDeg = Mathf.SmoothDamp(this.m_pitchDeg, 0f, ref this.m_pitchVel, this.m_info.m_PitchInfo.m_RestSeconds * 0.1f);
		this.m_rollDeg = Mathf.SmoothDamp(this.m_rollDeg, 0f, ref this.m_rollVel, this.m_info.m_RollInfo.m_RestSeconds * 0.1f);
		base.transform.localRotation = Quaternion.Euler(this.m_originalAngles.x + this.m_pitchDeg, this.m_originalAngles.y, this.m_originalAngles.z + this.m_rollDeg);
		this.m_prevPos = position;
	}

	// Token: 0x06004C66 RID: 19558 RVA: 0x0016BB4D File Offset: 0x00169D4D
	public DragRotatorInfo GetInfo()
	{
		return this.m_info;
	}

	// Token: 0x06004C67 RID: 19559 RVA: 0x0016BB55 File Offset: 0x00169D55
	public void SetInfo(DragRotatorInfo info)
	{
		this.m_info = info;
	}

	// Token: 0x06004C68 RID: 19560 RVA: 0x0016BB60 File Offset: 0x00169D60
	public void Reset()
	{
		this.m_prevPos = base.transform.position;
		base.transform.localRotation = Quaternion.Euler(this.m_originalAngles);
		this.m_rollDeg = 0f;
		this.m_rollVel = 0f;
		this.m_pitchDeg = 0f;
		this.m_pitchVel = 0f;
	}

	// Token: 0x0400333D RID: 13117
	private const float EPSILON = 0.0001f;

	// Token: 0x0400333E RID: 13118
	private const float SMOOTH_DAMP_SEC_FUDGE = 0.1f;

	// Token: 0x0400333F RID: 13119
	private DragRotatorInfo m_info;

	// Token: 0x04003340 RID: 13120
	private float m_pitchDeg;

	// Token: 0x04003341 RID: 13121
	private float m_rollDeg;

	// Token: 0x04003342 RID: 13122
	private float m_pitchVel;

	// Token: 0x04003343 RID: 13123
	private float m_rollVel;

	// Token: 0x04003344 RID: 13124
	private Vector3 m_prevPos;

	// Token: 0x04003345 RID: 13125
	private Vector3 m_originalAngles;
}
