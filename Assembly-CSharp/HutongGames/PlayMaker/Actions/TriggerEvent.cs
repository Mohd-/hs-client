using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D09 RID: 3337
	[ActionCategory(9)]
	[Tooltip("Detect collisions with objects that have RigidBody components. \nNOTE: The system events, TRIGGER ENTER, TRIGGER STAY, and TRIGGER EXIT are sent when any object collides with the trigger. Use this action to filter collisions by Tag.")]
	public class TriggerEvent : FsmStateAction
	{
		// Token: 0x060069D8 RID: 27096 RVA: 0x001F026B File Offset: 0x001EE46B
		public override void Reset()
		{
			this.trigger = 0;
			this.collideTag = "Untagged";
			this.sendEvent = null;
			this.storeCollider = null;
		}

		// Token: 0x060069D9 RID: 27097 RVA: 0x001F0294 File Offset: 0x001EE494
		public override void Awake()
		{
			switch (this.trigger)
			{
			case 0:
				base.Fsm.HandleTriggerEnter = true;
				break;
			case 1:
				base.Fsm.HandleTriggerStay = true;
				break;
			case 2:
				base.Fsm.HandleTriggerExit = true;
				break;
			}
		}

		// Token: 0x060069DA RID: 27098 RVA: 0x001F02F2 File Offset: 0x001EE4F2
		private void StoreCollisionInfo(Collider collisionInfo)
		{
			this.storeCollider.Value = collisionInfo.gameObject;
		}

		// Token: 0x060069DB RID: 27099 RVA: 0x001F0308 File Offset: 0x001EE508
		public override void DoTriggerEnter(Collider other)
		{
			if (this.trigger == null && other.gameObject.tag == this.collideTag.Value)
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x060069DC RID: 27100 RVA: 0x001F0358 File Offset: 0x001EE558
		public override void DoTriggerStay(Collider other)
		{
			if (this.trigger == 1 && other.gameObject.tag == this.collideTag.Value)
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x060069DD RID: 27101 RVA: 0x001F03AC File Offset: 0x001EE5AC
		public override void DoTriggerExit(Collider other)
		{
			if (this.trigger == 2 && other.gameObject.tag == this.collideTag.Value)
			{
				this.StoreCollisionInfo(other);
				base.Fsm.Event(this.sendEvent);
			}
		}

		// Token: 0x060069DE RID: 27102 RVA: 0x001F03FD File Offset: 0x001EE5FD
		public override string ErrorCheck()
		{
			return ActionHelpers.CheckOwnerPhysicsSetup(base.Owner);
		}

		// Token: 0x040051B1 RID: 20913
		public TriggerType trigger;

		// Token: 0x040051B2 RID: 20914
		[UIHint(7)]
		public FsmString collideTag;

		// Token: 0x040051B3 RID: 20915
		public FsmEvent sendEvent;

		// Token: 0x040051B4 RID: 20916
		[UIHint(10)]
		public FsmGameObject storeCollider;
	}
}
