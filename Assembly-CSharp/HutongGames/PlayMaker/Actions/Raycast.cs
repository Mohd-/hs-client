using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C78 RID: 3192
	[Tooltip("Casts a Ray against all Colliders in the scene. Use either a Game Object or Vector3 world position as the origin of the ray. Use GetRaycastInfo to get more detailed info.")]
	[ActionCategory(9)]
	public class Raycast : FsmStateAction
	{
		// Token: 0x06006757 RID: 26455 RVA: 0x001E7994 File Offset: 0x001E5B94
		public override void Reset()
		{
			this.fromGameObject = null;
			this.fromPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.direction = new FsmVector3
			{
				UseVariable = true
			};
			this.space = 1;
			this.distance = 100f;
			this.hitEvent = null;
			this.storeDidHit = null;
			this.storeHitObject = null;
			this.storeHitPoint = null;
			this.storeHitNormal = null;
			this.storeHitDistance = null;
			this.repeatInterval = 1;
			this.layerMask = new FsmInt[0];
			this.invertMask = false;
			this.debugColor = Color.yellow;
			this.debug = false;
		}

		// Token: 0x06006758 RID: 26456 RVA: 0x001E7A51 File Offset: 0x001E5C51
		public override void OnEnter()
		{
			this.DoRaycast();
			if (this.repeatInterval.Value == 0)
			{
				base.Finish();
			}
		}

		// Token: 0x06006759 RID: 26457 RVA: 0x001E7A6F File Offset: 0x001E5C6F
		public override void OnUpdate()
		{
			this.repeat--;
			if (this.repeat == 0)
			{
				this.DoRaycast();
			}
		}

		// Token: 0x0600675A RID: 26458 RVA: 0x001E7A90 File Offset: 0x001E5C90
		private void DoRaycast()
		{
			this.repeat = this.repeatInterval.Value;
			if (this.distance.Value == 0f)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.fromGameObject);
			Vector3 vector = (!(ownerDefaultTarget != null)) ? this.fromPosition.Value : ownerDefaultTarget.transform.position;
			float num = float.PositiveInfinity;
			if (this.distance.Value > 0f)
			{
				num = this.distance.Value;
			}
			Vector3 vector2 = this.direction.Value;
			if (ownerDefaultTarget != null && this.space == 1)
			{
				vector2 = ownerDefaultTarget.transform.TransformDirection(this.direction.Value);
			}
			RaycastHit raycastHitInfo;
			Physics.Raycast(vector, vector2, ref raycastHitInfo, num, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
			base.Fsm.RaycastHitInfo = raycastHitInfo;
			bool flag = raycastHitInfo.collider != null;
			this.storeDidHit.Value = flag;
			if (flag)
			{
				this.storeHitObject.Value = raycastHitInfo.collider.gameObject;
				this.storeHitPoint.Value = base.Fsm.RaycastHitInfo.point;
				this.storeHitNormal.Value = base.Fsm.RaycastHitInfo.normal;
				this.storeHitDistance.Value = base.Fsm.RaycastHitInfo.distance;
				base.Fsm.Event(this.hitEvent);
			}
			if (this.debug.Value)
			{
				float num2 = Mathf.Min(num, 1000f);
				Debug.DrawLine(vector, vector + vector2 * num2, this.debugColor.Value);
			}
		}

		// Token: 0x04004F25 RID: 20261
		[Tooltip("Start ray at game object position. \nOr use From Position parameter.")]
		public FsmOwnerDefault fromGameObject;

		// Token: 0x04004F26 RID: 20262
		[Tooltip("Start ray at a vector3 world position. \nOr use Game Object parameter.")]
		public FsmVector3 fromPosition;

		// Token: 0x04004F27 RID: 20263
		[Tooltip("A vector3 direction vector")]
		public FsmVector3 direction;

		// Token: 0x04004F28 RID: 20264
		[Tooltip("Cast the ray in world or local space. Note if no Game Object is specfied, the direction is in world space.")]
		public Space space;

		// Token: 0x04004F29 RID: 20265
		[Tooltip("The length of the ray. Set to -1 for infinity.")]
		public FsmFloat distance;

		// Token: 0x04004F2A RID: 20266
		[Tooltip("Event to send if the ray hits an object.")]
		[ActionSection("Result")]
		[UIHint(10)]
		public FsmEvent hitEvent;

		// Token: 0x04004F2B RID: 20267
		[UIHint(10)]
		[Tooltip("Set a bool variable to true if hit something, otherwise false.")]
		public FsmBool storeDidHit;

		// Token: 0x04004F2C RID: 20268
		[UIHint(10)]
		[Tooltip("Store the game object hit in a variable.")]
		public FsmGameObject storeHitObject;

		// Token: 0x04004F2D RID: 20269
		[Tooltip("Get the world position of the ray hit point and store it in a variable.")]
		[UIHint(10)]
		public FsmVector3 storeHitPoint;

		// Token: 0x04004F2E RID: 20270
		[UIHint(10)]
		[Tooltip("Get the normal at the hit point and store it in a variable.")]
		public FsmVector3 storeHitNormal;

		// Token: 0x04004F2F RID: 20271
		[Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
		[UIHint(10)]
		public FsmFloat storeHitDistance;

		// Token: 0x04004F30 RID: 20272
		[Tooltip("Set how often to cast a ray. 0 = once, don't repeat; 1 = everyFrame; 2 = every other frame... \nSince raycasts can get expensive use the highest repeat interval you can get away with.")]
		[ActionSection("Filter")]
		public FsmInt repeatInterval;

		// Token: 0x04004F31 RID: 20273
		[UIHint(8)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		// Token: 0x04004F32 RID: 20274
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04004F33 RID: 20275
		[ActionSection("Debug")]
		[Tooltip("The color to use for the debug line.")]
		public FsmColor debugColor;

		// Token: 0x04004F34 RID: 20276
		[Tooltip("Draw a debug line. Note: Check Gizmos in the Game View to see it in game.")]
		public FsmBool debug;

		// Token: 0x04004F35 RID: 20277
		private int repeat;
	}
}
