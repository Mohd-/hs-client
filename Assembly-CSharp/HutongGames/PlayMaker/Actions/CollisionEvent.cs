using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B6C RID: 2924
	[ActionCategory(9)]
	[Tooltip("Detect collisions between the Owner of this FSM and other Game Objects that have RigidBody components.\nNOTE: The system events, COLLISION ENTER, COLLISION STAY, and COLLISION EXIT are sent automatically on collisions with any object. Use this action to filter collisions by Tag.")]
	public class CollisionEvent : FsmStateAction
	{
		// Token: 0x0600631F RID: 25375 RVA: 0x001D9F48 File Offset: 0x001D8148
		public override void Reset()
		{
			this.collision = 0;
			this.collideTag = "Untagged";
			this.sendEvent = null;
			this.storeCollider = null;
			this.storeForce = null;
		}

		// Token: 0x06006320 RID: 25376 RVA: 0x001D9F84 File Offset: 0x001D8184
		public override void Awake()
		{
			switch (this.collision)
			{
			case 0:
				base.Fsm.HandleCollisionEnter = true;
				break;
			case 1:
				base.Fsm.HandleCollisionStay = true;
				break;
			case 2:
				base.Fsm.HandleCollisionExit = true;
				break;
			}
		}

		// Token: 0x06006321 RID: 25377 RVA: 0x001D9FE4 File Offset: 0x001D81E4
		private void StoreCollisionInfo(Collision collisionInfo)
		{
			this.storeCollider.Value = collisionInfo.gameObject;
			this.storeForce.Value = collisionInfo.relativeVelocity.magnitude;
		}

		// Token: 0x06006322 RID: 25378 RVA: 0x001DA01C File Offset: 0x001D821C
		public override void DoCollisionEnter(Collision collisionInfo)
		{
			if (this.collision == null && collisionInfo.collider.gameObject.tag == this.collideTag.Value)
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06006323 RID: 25379 RVA: 0x001DA074 File Offset: 0x001D8274
		public override void DoCollisionStay(Collision collisionInfo)
		{
			if (this.collision == 1 && collisionInfo.collider.gameObject.tag == this.collideTag.Value)
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06006324 RID: 25380 RVA: 0x001DA0CC File Offset: 0x001D82CC
		public override void DoCollisionExit(Collision collisionInfo)
		{
			if (this.collision == 2 && collisionInfo.collider.gameObject.tag == this.collideTag.Value)
			{
				this.StoreCollisionInfo(collisionInfo);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06006325 RID: 25381 RVA: 0x001DA124 File Offset: 0x001D8324
		public override void DoControllerColliderHit(ControllerColliderHit collisionInfo)
		{
			if (this.collision == 3 && collisionInfo.collider.gameObject.tag == this.collideTag.Value)
			{
				if (this.storeCollider != null)
				{
					this.storeCollider.Value = collisionInfo.gameObject;
				}
				this.storeForce.Value = 0f;
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x06006326 RID: 25382 RVA: 0x001DA19F File Offset: 0x001D839F
		public override string ErrorCheck()
		{
			return ActionHelpers.CheckOwnerPhysicsSetup(base.Owner);
		}

		// Token: 0x04004A9D RID: 19101
		[Tooltip("The type of collision to detect.")]
		public CollisionType collision;

		// Token: 0x04004A9E RID: 19102
		[Tooltip("Filter by Tag.")]
		[UIHint(7)]
		public FsmString collideTag;

		// Token: 0x04004A9F RID: 19103
		[Tooltip("Event to send if a collision is detected.")]
		public FsmEvent sendEvent;

		// Token: 0x04004AA0 RID: 19104
		[Tooltip("Store the GameObject that collided with the Owner of this FSM.")]
		[UIHint(10)]
		public FsmGameObject storeCollider;

		// Token: 0x04004AA1 RID: 19105
		[UIHint(10)]
		[Tooltip("Store the force of the collision. NOTE: Use Get Collision Info to get more info about the collision.")]
		public FsmFloat storeForce;
	}
}
