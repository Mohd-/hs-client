using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF8 RID: 3064
	[Tooltip("Gets info on the last Character Controller collision and store in variables.")]
	[ActionCategory(32)]
	public class GetControllerHitInfo : FsmStateAction
	{
		// Token: 0x06006532 RID: 25906 RVA: 0x001E0D90 File Offset: 0x001DEF90
		public override void Reset()
		{
			this.gameObjectHit = null;
			this.contactPoint = null;
			this.contactNormal = null;
			this.moveDirection = null;
			this.moveLength = null;
			this.physicsMaterialName = null;
		}

		// Token: 0x06006533 RID: 25907 RVA: 0x001E0DC8 File Offset: 0x001DEFC8
		private void StoreTriggerInfo()
		{
			if (base.Fsm.ControllerCollider == null)
			{
				return;
			}
			this.gameObjectHit.Value = base.Fsm.ControllerCollider.gameObject;
			this.contactPoint.Value = base.Fsm.ControllerCollider.point;
			this.contactNormal.Value = base.Fsm.ControllerCollider.normal;
			this.moveDirection.Value = base.Fsm.ControllerCollider.moveDirection;
			this.moveLength.Value = base.Fsm.ControllerCollider.moveLength;
			this.physicsMaterialName.Value = base.Fsm.ControllerCollider.collider.material.name;
		}

		// Token: 0x06006534 RID: 25908 RVA: 0x001E0E92 File Offset: 0x001DF092
		public override void OnEnter()
		{
			this.StoreTriggerInfo();
			base.Finish();
		}

		// Token: 0x06006535 RID: 25909 RVA: 0x001E0EA0 File Offset: 0x001DF0A0
		public override string ErrorCheck()
		{
			return ActionHelpers.CheckOwnerPhysicsSetup(base.Owner);
		}

		// Token: 0x04004CD0 RID: 19664
		[UIHint(10)]
		public FsmGameObject gameObjectHit;

		// Token: 0x04004CD1 RID: 19665
		[UIHint(10)]
		public FsmVector3 contactPoint;

		// Token: 0x04004CD2 RID: 19666
		[UIHint(10)]
		public FsmVector3 contactNormal;

		// Token: 0x04004CD3 RID: 19667
		[UIHint(10)]
		public FsmVector3 moveDirection;

		// Token: 0x04004CD4 RID: 19668
		[UIHint(10)]
		public FsmFloat moveLength;

		// Token: 0x04004CD5 RID: 19669
		[UIHint(10)]
		[Tooltip("Useful for triggering different effects. Audio, particles...")]
		public FsmString physicsMaterialName;
	}
}
