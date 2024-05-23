using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BEC RID: 3052
	[ActionCategory(6)]
	[Tooltip("Gets a world direction Vector from 2 Input Axis. Typically used for a third person controller with Relative To set to the camera.")]
	public class GetAxisVector : FsmStateAction
	{
		// Token: 0x06006506 RID: 25862 RVA: 0x001E02F0 File Offset: 0x001DE4F0
		public override void Reset()
		{
			this.horizontalAxis = "Horizontal";
			this.verticalAxis = "Vertical";
			this.multiplier = 1f;
			this.mapToPlane = GetAxisVector.AxisPlane.XZ;
			this.storeVector = null;
			this.storeMagnitude = null;
		}

		// Token: 0x06006507 RID: 25863 RVA: 0x001E0344 File Offset: 0x001DE544
		public override void OnUpdate()
		{
			Vector3 vector = default(Vector3);
			Vector3 vector2 = default(Vector3);
			if (this.relativeTo.Value == null)
			{
				switch (this.mapToPlane)
				{
				case GetAxisVector.AxisPlane.XZ:
					vector = Vector3.forward;
					vector2 = Vector3.right;
					break;
				case GetAxisVector.AxisPlane.XY:
					vector = Vector3.up;
					vector2 = Vector3.right;
					break;
				case GetAxisVector.AxisPlane.YZ:
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
				case GetAxisVector.AxisPlane.XZ:
					vector = transform.TransformDirection(Vector3.forward);
					vector.y = 0f;
					vector = vector.normalized;
					vector2..ctor(vector.z, 0f, -vector.x);
					break;
				case GetAxisVector.AxisPlane.XY:
				case GetAxisVector.AxisPlane.YZ:
					vector = Vector3.up;
					vector.z = 0f;
					vector = vector.normalized;
					vector2 = transform.TransformDirection(Vector3.right);
					break;
				}
			}
			float num = (!this.horizontalAxis.IsNone && !string.IsNullOrEmpty(this.horizontalAxis.Value)) ? Input.GetAxis(this.horizontalAxis.Value) : 0f;
			float num2 = (!this.verticalAxis.IsNone && !string.IsNullOrEmpty(this.verticalAxis.Value)) ? Input.GetAxis(this.verticalAxis.Value) : 0f;
			Vector3 vector3 = num * vector2 + num2 * vector;
			vector3 *= this.multiplier.Value;
			this.storeVector.Value = vector3;
			if (!this.storeMagnitude.IsNone)
			{
				this.storeMagnitude.Value = vector3.magnitude;
			}
		}

		// Token: 0x04004C9C RID: 19612
		[Tooltip("The name of the horizontal input axis. See Unity Input Manager.")]
		public FsmString horizontalAxis;

		// Token: 0x04004C9D RID: 19613
		[Tooltip("The name of the vertical input axis. See Unity Input Manager.")]
		public FsmString verticalAxis;

		// Token: 0x04004C9E RID: 19614
		[Tooltip("Input axis are reported in the range -1 to 1, this multiplier lets you set a new range.")]
		public FsmFloat multiplier;

		// Token: 0x04004C9F RID: 19615
		[Tooltip("The world plane to map the 2d input onto.")]
		[RequiredField]
		public GetAxisVector.AxisPlane mapToPlane;

		// Token: 0x04004CA0 RID: 19616
		[Tooltip("Make the result relative to a GameObject, typically the main camera.")]
		public FsmGameObject relativeTo;

		// Token: 0x04004CA1 RID: 19617
		[UIHint(10)]
		[Tooltip("Store the direction vector.")]
		[RequiredField]
		public FsmVector3 storeVector;

		// Token: 0x04004CA2 RID: 19618
		[Tooltip("Store the length of the direction vector.")]
		[UIHint(10)]
		public FsmFloat storeMagnitude;

		// Token: 0x02000BED RID: 3053
		public enum AxisPlane
		{
			// Token: 0x04004CA4 RID: 19620
			XZ,
			// Token: 0x04004CA5 RID: 19621
			XY,
			// Token: 0x04004CA6 RID: 19622
			YZ
		}
	}
}
