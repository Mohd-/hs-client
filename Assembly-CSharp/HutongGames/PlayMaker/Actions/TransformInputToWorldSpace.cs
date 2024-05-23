using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D05 RID: 3333
	[ActionCategory(6)]
	[Tooltip("Transforms 2d input into a 3d world space vector. E.g., can be used to transform input from a touch joystick to a movement vector.")]
	public class TransformInputToWorldSpace : FsmStateAction
	{
		// Token: 0x060069C8 RID: 27080 RVA: 0x001EFD71 File Offset: 0x001EDF71
		public override void Reset()
		{
			this.horizontalInput = null;
			this.verticalInput = null;
			this.multiplier = 1f;
			this.mapToPlane = TransformInputToWorldSpace.AxisPlane.XZ;
			this.storeVector = null;
			this.storeMagnitude = null;
		}

		// Token: 0x060069C9 RID: 27081 RVA: 0x001EFDA8 File Offset: 0x001EDFA8
		public override void OnUpdate()
		{
			Vector3 vector = default(Vector3);
			Vector3 vector2 = default(Vector3);
			if (this.relativeTo.Value == null)
			{
				switch (this.mapToPlane)
				{
				case TransformInputToWorldSpace.AxisPlane.XZ:
					vector = Vector3.forward;
					vector2 = Vector3.right;
					break;
				case TransformInputToWorldSpace.AxisPlane.XY:
					vector = Vector3.up;
					vector2 = Vector3.right;
					break;
				case TransformInputToWorldSpace.AxisPlane.YZ:
					vector = Vector3.up;
					vector2 = Vector3.forward;
					break;
				}
			}
			else
			{
				Transform transform = this.relativeTo.Value.transform;
				switch (this.mapToPlane)
				{
				case TransformInputToWorldSpace.AxisPlane.XZ:
					vector = transform.TransformDirection(Vector3.forward);
					vector.y = 0f;
					vector = vector.normalized;
					vector2..ctor(vector.z, 0f, -vector.x);
					break;
				case TransformInputToWorldSpace.AxisPlane.XY:
				case TransformInputToWorldSpace.AxisPlane.YZ:
					vector = Vector3.up;
					vector.z = 0f;
					vector = vector.normalized;
					vector2 = transform.TransformDirection(Vector3.right);
					break;
				}
			}
			float num = (!this.horizontalInput.IsNone) ? this.horizontalInput.Value : 0f;
			float num2 = (!this.verticalInput.IsNone) ? this.verticalInput.Value : 0f;
			Vector3 vector3 = num * vector2 + num2 * vector;
			vector3 *= this.multiplier.Value;
			this.storeVector.Value = vector3;
			if (!this.storeMagnitude.IsNone)
			{
				this.storeMagnitude.Value = vector3.magnitude;
			}
		}

		// Token: 0x04005198 RID: 20888
		[Tooltip("The horizontal input.")]
		[UIHint(10)]
		public FsmFloat horizontalInput;

		// Token: 0x04005199 RID: 20889
		[UIHint(10)]
		[Tooltip("The vertical input.")]
		public FsmFloat verticalInput;

		// Token: 0x0400519A RID: 20890
		[Tooltip("Input axis are reported in the range -1 to 1, this multiplier lets you set a new range.")]
		public FsmFloat multiplier;

		// Token: 0x0400519B RID: 20891
		[Tooltip("The world plane to map the 2d input onto.")]
		[RequiredField]
		public TransformInputToWorldSpace.AxisPlane mapToPlane;

		// Token: 0x0400519C RID: 20892
		[Tooltip("Make the result relative to a GameObject, typically the main camera.")]
		public FsmGameObject relativeTo;

		// Token: 0x0400519D RID: 20893
		[UIHint(10)]
		[Tooltip("Store the direction vector.")]
		[RequiredField]
		public FsmVector3 storeVector;

		// Token: 0x0400519E RID: 20894
		[Tooltip("Store the length of the direction vector.")]
		[UIHint(10)]
		public FsmFloat storeMagnitude;

		// Token: 0x02000D06 RID: 3334
		public enum AxisPlane
		{
			// Token: 0x040051A0 RID: 20896
			XZ,
			// Token: 0x040051A1 RID: 20897
			XY,
			// Token: 0x040051A2 RID: 20898
			YZ
		}
	}
}
