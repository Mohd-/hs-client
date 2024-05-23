using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C27 RID: 3111
	[ActionCategory(9)]
	[Tooltip("Gets info on the last Raycast and store in variables.")]
	public class GetRaycastHitInfo : FsmStateAction
	{
		// Token: 0x060065FB RID: 26107 RVA: 0x001E3352 File Offset: 0x001E1552
		public override void Reset()
		{
			this.gameObjectHit = null;
			this.point = null;
			this.normal = null;
			this.distance = null;
			this.everyFrame = false;
		}

		// Token: 0x060065FC RID: 26108 RVA: 0x001E3378 File Offset: 0x001E1578
		private void StoreRaycastInfo()
		{
			if (base.Fsm.RaycastHitInfo.collider != null)
			{
				this.gameObjectHit.Value = base.Fsm.RaycastHitInfo.collider.gameObject;
				this.point.Value = base.Fsm.RaycastHitInfo.point;
				this.normal.Value = base.Fsm.RaycastHitInfo.normal;
				this.distance.Value = base.Fsm.RaycastHitInfo.distance;
			}
		}

		// Token: 0x060065FD RID: 26109 RVA: 0x001E3421 File Offset: 0x001E1621
		public override void OnEnter()
		{
			this.StoreRaycastInfo();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060065FE RID: 26110 RVA: 0x001E343A File Offset: 0x001E163A
		public override void OnUpdate()
		{
			this.StoreRaycastInfo();
		}

		// Token: 0x04004DB7 RID: 19895
		[Tooltip("Get the GameObject hit by the last Raycast and store it in a variable.")]
		[UIHint(10)]
		public FsmGameObject gameObjectHit;

		// Token: 0x04004DB8 RID: 19896
		[UIHint(10)]
		[Title("Hit Point")]
		[Tooltip("Get the world position of the ray hit point and store it in a variable.")]
		public FsmVector3 point;

		// Token: 0x04004DB9 RID: 19897
		[UIHint(10)]
		[Tooltip("Get the normal at the hit point and store it in a variable.")]
		public FsmVector3 normal;

		// Token: 0x04004DBA RID: 19898
		[Tooltip("Get the distance along the ray to the hit point and store it in a variable.")]
		[UIHint(10)]
		public FsmFloat distance;

		// Token: 0x04004DBB RID: 19899
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
