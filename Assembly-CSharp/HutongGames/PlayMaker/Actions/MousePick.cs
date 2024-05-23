using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C58 RID: 3160
	[Tooltip("Perform a Mouse Pick on the scene and stores the results. Use Ray Distance to set how close the camera must be to pick the object.")]
	[ActionCategory(6)]
	public class MousePick : FsmStateAction
	{
		// Token: 0x060066D1 RID: 26321 RVA: 0x001E5B78 File Offset: 0x001E3D78
		public override void Reset()
		{
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

		// Token: 0x060066D2 RID: 26322 RVA: 0x001E5BD7 File Offset: 0x001E3DD7
		public override void OnEnter()
		{
			this.DoMousePick();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066D3 RID: 26323 RVA: 0x001E5BF0 File Offset: 0x001E3DF0
		public override void OnUpdate()
		{
			this.DoMousePick();
		}

		// Token: 0x060066D4 RID: 26324 RVA: 0x001E5BF8 File Offset: 0x001E3DF8
		private void DoMousePick()
		{
			RaycastHit raycastHit = ActionHelpers.MousePick(this.rayDistance.Value, ActionHelpers.LayerArrayToLayerMask(this.layerMask, this.invertMask.Value));
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
				this.storeDistance.Value = float.PositiveInfinity;
				this.storePoint.Value = Vector3.zero;
				this.storeNormal.Value = Vector3.zero;
			}
		}

		// Token: 0x04004E9D RID: 20125
		[RequiredField]
		public FsmFloat rayDistance = 100f;

		// Token: 0x04004E9E RID: 20126
		[UIHint(10)]
		public FsmBool storeDidPickObject;

		// Token: 0x04004E9F RID: 20127
		[UIHint(10)]
		public FsmGameObject storeGameObject;

		// Token: 0x04004EA0 RID: 20128
		[UIHint(10)]
		public FsmVector3 storePoint;

		// Token: 0x04004EA1 RID: 20129
		[UIHint(10)]
		public FsmVector3 storeNormal;

		// Token: 0x04004EA2 RID: 20130
		[UIHint(10)]
		public FsmFloat storeDistance;

		// Token: 0x04004EA3 RID: 20131
		[UIHint(8)]
		[Tooltip("Pick only from these layers.")]
		public FsmInt[] layerMask;

		// Token: 0x04004EA4 RID: 20132
		[Tooltip("Invert the mask, so you pick from all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04004EA5 RID: 20133
		public bool everyFrame;
	}
}
