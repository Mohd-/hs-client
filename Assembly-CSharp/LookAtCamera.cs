using System;
using UnityEngine;

// Token: 0x02000F16 RID: 3862
[ExecuteInEditMode]
public class LookAtCamera : MonoBehaviour
{
	// Token: 0x06007339 RID: 29497 RVA: 0x0021F2A0 File Offset: 0x0021D4A0
	private void Awake()
	{
		this.CreateLookAtTarget();
	}

	// Token: 0x0600733A RID: 29498 RVA: 0x0021F2A8 File Offset: 0x0021D4A8
	private void Start()
	{
		this.m_MainCamera = Camera.main;
	}

	// Token: 0x0600733B RID: 29499 RVA: 0x0021F2B8 File Offset: 0x0021D4B8
	private void Update()
	{
		if (this.m_MainCamera == null)
		{
			this.m_MainCamera = Camera.main;
			if (this.m_MainCamera == null)
			{
				return;
			}
		}
		if (this.m_LookAtTarget == null)
		{
			this.CreateLookAtTarget();
			if (this.m_LookAtTarget == null)
			{
				return;
			}
		}
		this.m_LookAtTarget.transform.position = this.m_MainCamera.transform.position + this.m_LookAtPositionOffset;
		base.transform.LookAt(this.m_LookAtTarget.transform, this.Z_VECTOR);
		base.transform.Rotate(this.X_VECTOR, 90f);
		base.transform.Rotate(this.Y_VECTOR, 180f);
	}

	// Token: 0x0600733C RID: 29500 RVA: 0x0021F38F File Offset: 0x0021D58F
	private void OnDestroy()
	{
		if (this.m_LookAtTarget)
		{
			Object.Destroy(this.m_LookAtTarget);
		}
	}

	// Token: 0x0600733D RID: 29501 RVA: 0x0021F3AC File Offset: 0x0021D5AC
	private void CreateLookAtTarget()
	{
		this.m_LookAtTarget = new GameObject();
		this.m_LookAtTarget.name = "LookAtCamera Target";
	}

	// Token: 0x04005DB8 RID: 23992
	private readonly Vector3 X_VECTOR = new Vector3(1f, 0f, 0f);

	// Token: 0x04005DB9 RID: 23993
	private readonly Vector3 Y_VECTOR = new Vector3(0f, 1f, 0f);

	// Token: 0x04005DBA RID: 23994
	private readonly Vector3 Z_VECTOR = new Vector3(0f, 0f, 1f);

	// Token: 0x04005DBB RID: 23995
	public Vector3 m_LookAtPositionOffset = Vector3.zero;

	// Token: 0x04005DBC RID: 23996
	private Camera m_MainCamera;

	// Token: 0x04005DBD RID: 23997
	private GameObject m_LookAtTarget;
}
