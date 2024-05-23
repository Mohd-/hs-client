using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C54 RID: 3156
	[Tooltip("Rotates a GameObject based on mouse movement. Minimum and Maximum values can be used to constrain the rotation.")]
	[ActionCategory(6)]
	public class MouseLook : FsmStateAction
	{
		// Token: 0x060066C1 RID: 26305 RVA: 0x001E55A4 File Offset: 0x001E37A4
		public override void Reset()
		{
			this.gameObject = null;
			this.axes = MouseLook.RotationAxes.MouseXAndY;
			this.sensitivityX = 15f;
			this.sensitivityY = 15f;
			this.minimumX = new FsmFloat
			{
				UseVariable = true
			};
			this.maximumX = new FsmFloat
			{
				UseVariable = true
			};
			this.minimumY = -60f;
			this.maximumY = 60f;
			this.everyFrame = true;
		}

		// Token: 0x060066C2 RID: 26306 RVA: 0x001E5630 File Offset: 0x001E3830
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			Rigidbody component = ownerDefaultTarget.GetComponent<Rigidbody>();
			if (component != null)
			{
				component.freezeRotation = true;
			}
			this.rotationX = ownerDefaultTarget.transform.localRotation.eulerAngles.y;
			this.rotationY = ownerDefaultTarget.transform.localRotation.eulerAngles.x;
			this.DoMouseLook();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066C3 RID: 26307 RVA: 0x001E56D7 File Offset: 0x001E38D7
		public override void OnUpdate()
		{
			this.DoMouseLook();
		}

		// Token: 0x060066C4 RID: 26308 RVA: 0x001E56E0 File Offset: 0x001E38E0
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
			case MouseLook.RotationAxes.MouseXAndY:
				transform.localEulerAngles = new Vector3(this.GetYRotation(), this.GetXRotation(), 0f);
				break;
			case MouseLook.RotationAxes.MouseX:
				transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, this.GetXRotation(), 0f);
				break;
			case MouseLook.RotationAxes.MouseY:
				transform.localEulerAngles = new Vector3(-this.GetYRotation(), transform.localEulerAngles.y, 0f);
				break;
			}
		}

		// Token: 0x060066C5 RID: 26309 RVA: 0x001E57A8 File Offset: 0x001E39A8
		private float GetXRotation()
		{
			this.rotationX += Input.GetAxis("Mouse X") * this.sensitivityX.Value;
			this.rotationX = MouseLook.ClampAngle(this.rotationX, this.minimumX, this.maximumX);
			return this.rotationX;
		}

		// Token: 0x060066C6 RID: 26310 RVA: 0x001E57FC File Offset: 0x001E39FC
		private float GetYRotation()
		{
			this.rotationY += Input.GetAxis("Mouse Y") * this.sensitivityY.Value;
			this.rotationY = MouseLook.ClampAngle(this.rotationY, this.minimumY, this.maximumY);
			return this.rotationY;
		}

		// Token: 0x060066C7 RID: 26311 RVA: 0x001E5850 File Offset: 0x001E3A50
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

		// Token: 0x04004E7F RID: 20095
		[Tooltip("The GameObject to rotate.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004E80 RID: 20096
		[Tooltip("The axes to rotate around.")]
		public MouseLook.RotationAxes axes;

		// Token: 0x04004E81 RID: 20097
		[RequiredField]
		public FsmFloat sensitivityX;

		// Token: 0x04004E82 RID: 20098
		[RequiredField]
		public FsmFloat sensitivityY;

		// Token: 0x04004E83 RID: 20099
		[Tooltip("Clamp rotation around X axis. Set to None for no clamping.")]
		[HasFloatSlider(-360f, 360f)]
		public FsmFloat minimumX;

		// Token: 0x04004E84 RID: 20100
		[Tooltip("Clamp rotation around X axis. Set to None for no clamping.")]
		[HasFloatSlider(-360f, 360f)]
		public FsmFloat maximumX;

		// Token: 0x04004E85 RID: 20101
		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Clamp rotation around Y axis. Set to None for no clamping.")]
		public FsmFloat minimumY;

		// Token: 0x04004E86 RID: 20102
		[HasFloatSlider(-360f, 360f)]
		[Tooltip("Clamp rotation around Y axis. Set to None for no clamping.")]
		public FsmFloat maximumY;

		// Token: 0x04004E87 RID: 20103
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04004E88 RID: 20104
		private float rotationX;

		// Token: 0x04004E89 RID: 20105
		private float rotationY;

		// Token: 0x02000C55 RID: 3157
		public enum RotationAxes
		{
			// Token: 0x04004E8B RID: 20107
			MouseXAndY,
			// Token: 0x04004E8C RID: 20108
			MouseX,
			// Token: 0x04004E8D RID: 20109
			MouseY
		}
	}
}
