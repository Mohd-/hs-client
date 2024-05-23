using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C56 RID: 3158
	[ActionCategory(6)]
	[Tooltip("Rotates a GameObject based on mouse movement. Minimum and Maximum values can be used to constrain the rotation.")]
	public class MouseLook2 : ComponentAction<Rigidbody>
	{
		// Token: 0x060066C9 RID: 26313 RVA: 0x001E58A4 File Offset: 0x001E3AA4
		public override void Reset()
		{
			this.gameObject = null;
			this.axes = MouseLook2.RotationAxes.MouseXAndY;
			this.sensitivityX = 15f;
			this.sensitivityY = 15f;
			this.minimumX = -360f;
			this.maximumX = 360f;
			this.minimumY = -60f;
			this.maximumY = 60f;
			this.everyFrame = true;
		}

		// Token: 0x060066CA RID: 26314 RVA: 0x001E5928 File Offset: 0x001E3B28
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			if (!base.UpdateCache(ownerDefaultTarget) && base.rigidbody)
			{
				base.rigidbody.freezeRotation = true;
			}
			this.DoMouseLook();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066CB RID: 26315 RVA: 0x001E5999 File Offset: 0x001E3B99
		public override void OnUpdate()
		{
			this.DoMouseLook();
		}

		// Token: 0x060066CC RID: 26316 RVA: 0x001E59A4 File Offset: 0x001E3BA4
		private void DoMouseLook()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Transform transform = ownerDefaultTarget.transform;
			switch (this.axes)
			{
			case MouseLook2.RotationAxes.MouseXAndY:
				transform.localEulerAngles = new Vector3(this.GetYRotation(), this.GetXRotation(), 0f);
				break;
			case MouseLook2.RotationAxes.MouseX:
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, this.GetXRotation(), 0f);
				break;
			case MouseLook2.RotationAxes.MouseY:
				transform.localEulerAngles = new Vector3(-this.GetYRotation(), transform.localEulerAngles.y, 0f);
				break;
			}
		}

		// Token: 0x060066CD RID: 26317 RVA: 0x001E5A6C File Offset: 0x001E3C6C
		private float GetXRotation()
		{
			this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX.Value;
			this.rotationX = MouseLook2.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
			return this.rotationX;
		}

		// Token: 0x060066CE RID: 26318 RVA: 0x001E5AC0 File Offset: 0x001E3CC0
		private float GetYRotation()
		{
			this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY.Value;
			this.rotationY = MouseLook2.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
			return this.rotationY;
		}

		// Token: 0x060066CF RID: 26319 RVA: 0x001E5B14 File Offset: 0x001E3D14
		private static float ClampAngle(float angle, FsmFloat min, FsmFloat max)
		{
			if (!min.IsNone && angle < min.Value)
			{
				angle = min.Value;
			}
			if (!max.IsNone && angle > max.Value)
			{
				angle = max.Value;
			}
			return angle;
		}

		// Token: 0x04004E8E RID: 20110
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004E8F RID: 20111
		[Tooltip("The axes to rotate around.")]
		public MouseLook2.RotationAxes axes;

		// Token: 0x04004E90 RID: 20112
		[RequiredField]
		public FsmFloat sensitivityX;

		// Token: 0x04004E91 RID: 20113
		[RequiredField]
		public FsmFloat sensitivityY;

		// Token: 0x04004E92 RID: 20114
		[HasFloatSlider(-360f, 360f)]
		[RequiredField]
		public FsmFloat minimumX;

		// Token: 0x04004E93 RID: 20115
		[HasFloatSlider(-360f, 360f)]
		[RequiredField]
		public FsmFloat maximumX;

		// Token: 0x04004E94 RID: 20116
		[HasFloatSlider(-360f, 360f)]
		[RequiredField]
		public FsmFloat minimumY;

		// Token: 0x04004E95 RID: 20117
		[HasFloatSlider(-360f, 360f)]
		[RequiredField]
		public FsmFloat maximumY;

		// Token: 0x04004E96 RID: 20118
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04004E97 RID: 20119
		private float rotationX;

		// Token: 0x04004E98 RID: 20120
		private float rotationY;

		// Token: 0x02000C57 RID: 3159
		public enum RotationAxes
		{
			// Token: 0x04004E9A RID: 20122
			MouseXAndY,
			// Token: 0x04004E9B RID: 20123
			MouseX,
			// Token: 0x04004E9C RID: 20124
			MouseY
		}
	}
}
