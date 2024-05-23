using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C85 RID: 3205
	[ActionCategory(6)]
	[Tooltip("Perform a raycast into the scene using screen coordinates and stores the results. Use Ray Distance to set how close the camera must be to pick the object. NOTE: Uses the MainCamera!")]
	public class ScreenPick : FsmStateAction
	{
		// Token: 0x0600679C RID: 26524 RVA: 0x001E880C File Offset: 0x001E6A0C
		public override void Reset()
		{
			this.screenVector = new FsmVector3
			{
				UseVariable = true
			};
			this.screenX = new FsmFloat
			{
				UseVariable = true
			};
			this.screenY = new FsmFloat
			{
				UseVariable = true
			};
			this.normalized = false;
			this.rayDistance = 100f;
			this.storeDidPickObject = null;
			this.storeGameObject = null;
			this.storePoint = null;
			this.storeNormal = null;
			this.storeDistance = null;
			this.layerMask = new FsmInt[0];
			this.invertMask = false;
			this.everyFrame = false;
		}

		// Token: 0x0600679D RID: 26525 RVA: 0x001E88B3 File Offset: 0x001E6AB3
		public override void OnEnter()
		{
			this.DoScreenPick();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600679E RID: 26526 RVA: 0x001E88CC File Offset: 0x001E6ACC
		public override void OnUpdate()
		{
			this.DoScreenPick();
		}

		// Token: 0x0600679F RID: 26527 RVA: 0x001E88D4 File Offset: 0x001E6AD4
		private void DoScreenPick()
		{
			if (Camera.main == null)
			{
				this.LogError("No MainCamera defined!");
				base.Finish();
				return;
			}
			Vector3 vector = Vector3.zero;
			if (!this.screenVector.IsNone)
			{
				vector = this.screenVector.Value;
			}
			if (!this.screenX.IsNone)
			{
				vector.x = this.screenX.Value;
			}
			if (!this.screenY.IsNone)
			{
				vector.y = this.screenY.Value;
			}
			if (this.normalized.Value)
			{
				vector.x *= (float)Screen.width;
				vector.y *= (float)Screen.height;
			}
			Ray ray = Camera.main.ScreenPointToRay(vector);
			RaycastHit raycastHit;
			Physics.Raycast(ray, ref raycastHit, this.rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
			bool flag = raycastHit.collider != null;
			this.storeDidPickObject.Value = flag;
			if (flag)
			{
				this.storeGameObject.Value = raycastHit.collider.gameObject;
				this.storeDistance.Value = raycastHit.distance;
				this.storePoint.Value = raycastHit.point;
				this.storeNormal.Value = raycastHit.normal;
			}
			else
			{
				this.storeGameObject.Value = null;
				this.storeDistance = float.PositiveInfinity;
				this.storePoint.Value = Vector3.zero;
				this.storeNormal.Value = Vector3.zero;
			}
		}

		// Token: 0x04004F65 RID: 20325
		[Tooltip("A Vector3 screen position. Commonly stored by other actions.")]
		public FsmVector3 screenVector;

		// Token: 0x04004F66 RID: 20326
		[Tooltip("X position on screen.")]
		public FsmFloat screenX;

		// Token: 0x04004F67 RID: 20327
		[Tooltip("Y position on screen.")]
		public FsmFloat screenY;

		// Token: 0x04004F68 RID: 20328
		[Tooltip("Are the supplied screen coordinates normalized (0-1), or in pixels.")]
		public FsmBool normalized;

		// Token: 0x04004F69 RID: 20329
		[RequiredField]
		public FsmFloat rayDistance = 100f;

		// Token: 0x04004F6A RID: 20330
		[UIHint(10)]
		public FsmBool storeDidPickObject;

		// Token: 0x04004F6B RID: 20331
		[UIHint(10)]
		public FsmGameObject storeGameObject;

		// Token: 0x04004F6C RID: 20332
		[UIHint(10)]
		public FsmVector3 storePoint;

		// Token: 0x04004F6D RID: 20333
		[UIHint(10)]
		public FsmVector3 storeNormal;

		// Token: 0x04004F6E RID: 20334
		[UIHint(10)]
		public FsmFloat storeDistance;

		// Token: 0x04004F6F RID: 20335
		[Tooltip("Pick only from these layers.")]
		[UIHint(8)]
		public FsmInt[] layerMask;

		// Token: 0x04004F70 RID: 20336
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04004F71 RID: 20337
		public bool everyFrame;
	}
}
