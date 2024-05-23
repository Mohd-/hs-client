using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF4 RID: 3060
	[Tooltip("Gets info on the last collision event and store in variables. See Unity Physics docs.")]
	[ActionCategory(9)]
	public class GetCollisionInfo : FsmStateAction
	{
		// Token: 0x06006521 RID: 25889 RVA: 0x001E0974 File Offset: 0x001DEB74
		public override void Reset()
		{
			this.gameObjectHit = null;
			this.relativeVelocity = null;
			this.relativeSpeed = null;
			this.contactPoint = null;
			this.contactNormal = null;
			this.physicsMaterialName = null;
		}

		// Token: 0x06006522 RID: 25890 RVA: 0x001E09AC File Offset: 0x001DEBAC
		private void StoreCollisionInfo()
		{
			if (base.Fsm.CollisionInfo == null)
			{
				return;
			}
			this.gameObjectHit.Value = base.Fsm.CollisionInfo.gameObject;
			this.relativeSpeed.Value = base.Fsm.CollisionInfo.relativeVelocity.magnitude;
			this.relativeVelocity.Value = base.Fsm.CollisionInfo.relativeVelocity;
			this.physicsMaterialName.Value = base.Fsm.CollisionInfo.collider.material.name;
			if (base.Fsm.CollisionInfo.contacts != null && base.Fsm.CollisionInfo.contacts.Length > 0)
			{
				this.contactPoint.Value = base.Fsm.CollisionInfo.contacts[0].point;
				this.contactNormal.Value = base.Fsm.CollisionInfo.contacts[0].normal;
			}
		}

		// Token: 0x06006523 RID: 25891 RVA: 0x001E0AC1 File Offset: 0x001DECC1
		public override void OnEnter()
		{
			this.StoreCollisionInfo();
			base.Finish();
		}

		// Token: 0x04004CB9 RID: 19641
		[Tooltip("Get the GameObject hit.")]
		[UIHint(10)]
		public FsmGameObject gameObjectHit;

		// Token: 0x04004CBA RID: 19642
		[UIHint(10)]
		[Tooltip("Get the relative velocity of the collision.")]
		public FsmVector3 relativeVelocity;

		// Token: 0x04004CBB RID: 19643
		[Tooltip("Get the relative speed of the collision. Useful for controlling reactions. E.g., selecting an appropriate sound fx.")]
		[UIHint(10)]
		public FsmFloat relativeSpeed;

		// Token: 0x04004CBC RID: 19644
		[Tooltip("Get the world position of the collision contact. Useful for spawning effects etc.")]
		[UIHint(10)]
		public FsmVector3 contactPoint;

		// Token: 0x04004CBD RID: 19645
		[Tooltip("Get the collision normal vector. Useful for aligning spawned effects etc.")]
		[UIHint(10)]
		public FsmVector3 contactNormal;

		// Token: 0x04004CBE RID: 19646
		[Tooltip("Get the name of the physics material of the colliding GameObject. Useful for triggering different effects. Audio, particles...")]
		[UIHint(10)]
		public FsmString physicsMaterialName;
	}
}
