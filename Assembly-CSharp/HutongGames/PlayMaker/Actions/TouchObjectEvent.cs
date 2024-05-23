using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D02 RID: 3330
	[Tooltip("Sends events when an object is touched. Optionally filter by a fingerID. NOTE: Uses the MainCamera!")]
	[ActionCategory(33)]
	public class TouchObjectEvent : FsmStateAction
	{
		// Token: 0x060069BD RID: 27069 RVA: 0x001EFA94 File Offset: 0x001EDC94
		public override void Reset()
		{
			this.gameObject = null;
			this.pickDistance = 100f;
			this.fingerId = new FsmInt
			{
				UseVariable = true
			};
			this.touchBegan = null;
			this.touchMoved = null;
			this.touchStationary = null;
			this.touchEnded = null;
			this.touchCanceled = null;
			this.storeFingerId = null;
			this.storeHitPoint = null;
			this.storeHitNormal = null;
		}

		// Token: 0x060069BE RID: 27070 RVA: 0x001EFB04 File Offset: 0x001EDD04
		public override void OnUpdate()
		{
			if (Camera.main == null)
			{
				this.LogError("No MainCamera defined!");
				base.Finish();
				return;
			}
			if (Input.touchCount > 0)
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				foreach (Touch touch in Input.touches)
				{
					if (this.fingerId.IsNone || touch.fingerId == this.fingerId.Value)
					{
						Vector2 position = touch.position;
						RaycastHit raycastHitInfo;
						Physics.Raycast(Camera.main.ScreenPointToRay(position), ref raycastHitInfo, this.pickDistance.Value);
						base.Fsm.RaycastHitInfo = raycastHitInfo;
						if (raycastHitInfo.transform != null && raycastHitInfo.transform.gameObject == ownerDefaultTarget)
						{
							this.storeFingerId.Value = touch.fingerId;
							this.storeHitPoint.Value = raycastHitInfo.point;
							this.storeHitNormal.Value = raycastHitInfo.normal;
							switch (touch.phase)
							{
							case 0:
								base.Fsm.Event(this.touchBegan);
								return;
							case 1:
								base.Fsm.Event(this.touchMoved);
								return;
							case 2:
								base.Fsm.Event(this.touchStationary);
								return;
							case 3:
								base.Fsm.Event(this.touchEnded);
								return;
							case 4:
								base.Fsm.Event(this.touchCanceled);
								return;
							}
						}
					}
				}
			}
		}

		// Token: 0x04005189 RID: 20873
		[Tooltip("The Game Object to detect touches on.")]
		[CheckForComponent(typeof(Collider))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400518A RID: 20874
		[Tooltip("How far from the camera is the Game Object pickable.")]
		[RequiredField]
		public FsmFloat pickDistance;

		// Token: 0x0400518B RID: 20875
		[Tooltip("Only detect touches that match this fingerID, or set to None.")]
		public FsmInt fingerId;

		// Token: 0x0400518C RID: 20876
		[Tooltip("Event to send on touch began.")]
		[ActionSection("Events")]
		public FsmEvent touchBegan;

		// Token: 0x0400518D RID: 20877
		[Tooltip("Event to send on touch moved.")]
		public FsmEvent touchMoved;

		// Token: 0x0400518E RID: 20878
		[Tooltip("Event to send on stationary touch.")]
		public FsmEvent touchStationary;

		// Token: 0x0400518F RID: 20879
		[Tooltip("Event to send on touch ended.")]
		public FsmEvent touchEnded;

		// Token: 0x04005190 RID: 20880
		[Tooltip("Event to send on touch cancel.")]
		public FsmEvent touchCanceled;

		// Token: 0x04005191 RID: 20881
		[UIHint(10)]
		[Tooltip("Store the fingerId of the touch.")]
		[ActionSection("Store Results")]
		public FsmInt storeFingerId;

		// Token: 0x04005192 RID: 20882
		[UIHint(10)]
		[Tooltip("Store the world position where the object was touched.")]
		public FsmVector3 storeHitPoint;

		// Token: 0x04005193 RID: 20883
		[Tooltip("Store the surface normal vector where the object was touched.")]
		[UIHint(10)]
		public FsmVector3 storeHitNormal;
	}
}
