using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200024E RID: 590
public class BoxCamera : MonoBehaviour
{
	// Token: 0x060021B0 RID: 8624 RVA: 0x000A4CBD File Offset: 0x000A2EBD
	public void SetParent(Box parent)
	{
		this.m_parent = parent;
	}

	// Token: 0x060021B1 RID: 8625 RVA: 0x000A4CC6 File Offset: 0x000A2EC6
	public Box GetParent()
	{
		return this.m_parent;
	}

	// Token: 0x060021B2 RID: 8626 RVA: 0x000A4CCE File Offset: 0x000A2ECE
	public BoxCameraStateInfo GetInfo()
	{
		return this.m_info;
	}

	// Token: 0x060021B3 RID: 8627 RVA: 0x000A4CD6 File Offset: 0x000A2ED6
	public void SetInfo(BoxCameraStateInfo info)
	{
		this.m_info = info;
	}

	// Token: 0x060021B4 RID: 8628 RVA: 0x000A4CDF File Offset: 0x000A2EDF
	public BoxCameraEventTable GetEventTable()
	{
		return this.m_EventTable;
	}

	// Token: 0x060021B5 RID: 8629 RVA: 0x000A4CE8 File Offset: 0x000A2EE8
	public Vector3 GetCameraPosition(BoxCamera.State state)
	{
		Transform transform;
		Transform transform2;
		if (state == BoxCamera.State.CLOSED)
		{
			transform = this.m_info.m_ClosedMinAspectRatioBone.transform;
			transform2 = this.m_info.m_ClosedBone.transform;
		}
		else if (state == BoxCamera.State.CLOSED_WITH_DRAWER)
		{
			transform = this.m_info.m_ClosedWithDrawerMinAspectRatioBone.transform;
			transform2 = this.m_info.m_ClosedWithDrawerBone.transform;
		}
		else
		{
			transform = this.m_info.m_OpenedMinAspectRatioBone.transform;
			transform2 = this.m_info.m_OpenedBone.transform;
		}
		if (!UniversalInputManager.UsePhoneUI)
		{
			return transform2.position;
		}
		return TransformUtil.GetAspectRatioDependentPosition(transform.position, transform2.position);
	}

	// Token: 0x060021B6 RID: 8630 RVA: 0x000A4D99 File Offset: 0x000A2F99
	public BoxCamera.State GetState()
	{
		return this.m_state;
	}

	// Token: 0x060021B7 RID: 8631 RVA: 0x000A4DA4 File Offset: 0x000A2FA4
	public bool ChangeState(BoxCamera.State state)
	{
		if (this.m_state == state)
		{
			return false;
		}
		this.m_state = state;
		Vector3 cameraPosition = this.GetCameraPosition(state);
		this.m_parent.OnAnimStarted();
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_applyAccelerometer = false;
			this.m_basePosition = base.transform.parent.InverseTransformPoint(cameraPosition);
			this.m_lookAtPoint = base.transform.parent.InverseTransformPoint(new Vector3(cameraPosition.x, 1.5f, cameraPosition.z));
			if (cameraPosition == base.gameObject.transform.position)
			{
				this.OnAnimFinished();
				return true;
			}
		}
		Hashtable args = null;
		if (state == BoxCamera.State.CLOSED)
		{
			args = iTween.Hash(new object[]
			{
				"position",
				cameraPosition,
				"delay",
				this.m_info.m_ClosedDelaySec,
				"time",
				this.m_info.m_ClosedMoveSec,
				"easeType",
				this.m_info.m_ClosedMoveEaseType,
				"oncomplete",
				"OnAnimFinished",
				"oncompletetarget",
				base.gameObject
			});
		}
		else if (state == BoxCamera.State.CLOSED_WITH_DRAWER)
		{
			args = iTween.Hash(new object[]
			{
				"position",
				cameraPosition,
				"delay",
				this.m_info.m_ClosedWithDrawerDelaySec,
				"time",
				this.m_info.m_ClosedWithDrawerMoveSec,
				"easeType",
				this.m_info.m_ClosedWithDrawerMoveEaseType,
				"oncomplete",
				"OnAnimFinished",
				"oncompletetarget",
				base.gameObject
			});
		}
		else if (state == BoxCamera.State.OPENED)
		{
			args = iTween.Hash(new object[]
			{
				"position",
				cameraPosition,
				"delay",
				this.m_info.m_OpenedDelaySec,
				"time",
				this.m_info.m_OpenedMoveSec,
				"easeType",
				this.m_info.m_OpenedMoveEaseType,
				"oncomplete",
				"OnAnimFinished",
				"oncompletetarget",
				base.gameObject
			});
		}
		else if (state == BoxCamera.State.SET_ROTATION_OPENED)
		{
			args = iTween.Hash(new object[]
			{
				"position",
				cameraPosition,
				"delay",
				this.m_info.m_OpenedDelaySec,
				"time",
				1.5f,
				"easeType",
				this.m_info.m_OpenedMoveEaseType,
				"oncomplete",
				"OnAnimFinished",
				"oncompletetarget",
				base.gameObject
			});
		}
		iTween.MoveTo(base.gameObject, args);
		return true;
	}

	// Token: 0x060021B8 RID: 8632 RVA: 0x000A50D6 File Offset: 0x000A32D6
	public void EnableAccelerometer()
	{
		if (MobileCallbackManager.AreMotionEffectsEnabled())
		{
			this.m_disableAccelerometer = false;
		}
	}

	// Token: 0x060021B9 RID: 8633 RVA: 0x000A50EC File Offset: 0x000A32EC
	public void Update()
	{
		if (this.m_disableAccelerometer || base.transform.parent.gameObject.GetComponent<LoadingScreen>() != null)
		{
			return;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			if (this.m_applyAccelerometer)
			{
				this.m_gyroRotation.x = Input.gyro.rotationRateUnbiased.x;
				this.m_gyroRotation.y = -Input.gyro.rotationRateUnbiased.y;
				this.m_currentAngle.x = this.m_currentAngle.x + this.m_gyroRotation.y * this.ROTATION_SCALE;
				this.m_currentAngle.y = this.m_currentAngle.y + this.m_gyroRotation.x * this.ROTATION_SCALE;
				this.m_currentAngle.x = Mathf.Clamp(this.m_currentAngle.x, -this.MAX_GYRO_RANGE, this.MAX_GYRO_RANGE);
				this.m_currentAngle.y = Mathf.Clamp(this.m_currentAngle.y, -this.MAX_GYRO_RANGE, this.MAX_GYRO_RANGE);
				base.gameObject.transform.localPosition = new Vector3(this.m_basePosition.x, this.m_basePosition.y, this.m_basePosition.z + this.m_currentAngle.y);
			}
			Vector3 vector;
			vector..ctor(0f, 0f, 1f);
			Vector3 vector2 = base.gameObject.transform.parent.TransformPoint(this.m_lookAtPoint);
			base.gameObject.transform.LookAt(vector2, vector);
			if (this.m_applyAccelerometer)
			{
				this.m_IgnoreFullscreenEffectsCamera.transform.position = base.gameObject.transform.parent.TransformPoint(this.m_basePosition);
				this.m_IgnoreFullscreenEffectsCamera.transform.LookAt(vector2, vector);
				this.m_TooltipCamera.transform.position = base.gameObject.transform.parent.TransformPoint(this.m_basePosition);
				this.m_TooltipCamera.transform.LookAt(vector2, vector);
			}
			else
			{
				TransformUtil.Identity(this.m_TooltipCamera);
				TransformUtil.Identity(this.m_IgnoreFullscreenEffectsCamera);
			}
		}
	}

	// Token: 0x060021BA RID: 8634 RVA: 0x000A5338 File Offset: 0x000A3538
	public void OnAnimFinished()
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_applyAccelerometer = (this.m_state != BoxCamera.State.OPENED);
			this.m_currentAngle = new Vector2(0f, 0f);
		}
		this.m_parent.OnAnimFinished();
	}

	// Token: 0x060021BB RID: 8635 RVA: 0x000A5386 File Offset: 0x000A3586
	public void UpdateState(BoxCamera.State state)
	{
		this.m_state = state;
		base.transform.position = this.GetCameraPosition(state);
	}

	// Token: 0x0400130C RID: 4876
	public BoxCameraEventTable m_EventTable;

	// Token: 0x0400130D RID: 4877
	public GameObject m_IgnoreFullscreenEffectsCamera;

	// Token: 0x0400130E RID: 4878
	public GameObject m_TooltipCamera;

	// Token: 0x0400130F RID: 4879
	private Box m_parent;

	// Token: 0x04001310 RID: 4880
	private BoxCameraStateInfo m_info;

	// Token: 0x04001311 RID: 4881
	private BoxCamera.State m_state;

	// Token: 0x04001312 RID: 4882
	private bool m_disableAccelerometer = true;

	// Token: 0x04001313 RID: 4883
	private bool m_applyAccelerometer;

	// Token: 0x04001314 RID: 4884
	private Vector2 m_currentAngle;

	// Token: 0x04001315 RID: 4885
	private Vector3 m_basePosition;

	// Token: 0x04001316 RID: 4886
	private Vector2 m_gyroRotation;

	// Token: 0x04001317 RID: 4887
	private float m_offset;

	// Token: 0x04001318 RID: 4888
	private float MAX_GYRO_RANGE = 2.1f;

	// Token: 0x04001319 RID: 4889
	private float ROTATION_SCALE = 0.085f;

	// Token: 0x0400131A RID: 4890
	private Vector3 m_lookAtPoint;

	// Token: 0x0200024F RID: 591
	public enum State
	{
		// Token: 0x0400131C RID: 4892
		CLOSED,
		// Token: 0x0400131D RID: 4893
		CLOSED_WITH_DRAWER,
		// Token: 0x0400131E RID: 4894
		OPENED,
		// Token: 0x0400131F RID: 4895
		SET_ROTATION_OPENED
	}
}
