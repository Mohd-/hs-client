using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF7 RID: 3063
	[Tooltip("Gets the Collision Flags from a Character Controller on a Game Object. Collision flags give you a broad overview of where the character collided with any other object.")]
	[ActionCategory(32)]
	public class GetControllerCollisionFlags : FsmStateAction
	{
		// Token: 0x0600652F RID: 25903 RVA: 0x001E0C64 File Offset: 0x001DEE64
		public override void Reset()
		{
			this.gameObject = null;
			this.isGrounded = null;
			this.none = null;
			this.sides = null;
			this.above = null;
			this.below = null;
		}

		// Token: 0x06006530 RID: 25904 RVA: 0x001E0C9C File Offset: 0x001DEE9C
		public override void OnUpdate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.previousGo)
			{
				this.controller = ownerDefaultTarget.GetComponent<CharacterController>();
				this.previousGo = ownerDefaultTarget;
			}
			if (this.controller != null)
			{
				this.isGrounded.Value = this.controller.isGrounded;
				FsmBool fsmBool = this.none;
				CollisionFlags collisionFlags = this.controller.collisionFlags;
				fsmBool.Value = false;
				this.sides.Value = ((this.controller.collisionFlags & 1) != 0);
				this.above.Value = ((this.controller.collisionFlags & 2) != 0);
				this.below.Value = ((this.controller.collisionFlags & 4) != 0);
			}
		}

		// Token: 0x04004CC8 RID: 19656
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject with a Character Controller component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004CC9 RID: 19657
		[UIHint(10)]
		[Tooltip("True if the Character Controller capsule is on the ground")]
		public FsmBool isGrounded;

		// Token: 0x04004CCA RID: 19658
		[Tooltip("True if no collisions in last move.")]
		[UIHint(10)]
		public FsmBool none;

		// Token: 0x04004CCB RID: 19659
		[Tooltip("True if the Character Controller capsule was hit on the sides.")]
		[UIHint(10)]
		public FsmBool sides;

		// Token: 0x04004CCC RID: 19660
		[Tooltip("True if the Character Controller capsule was hit from above.")]
		[UIHint(10)]
		public FsmBool above;

		// Token: 0x04004CCD RID: 19661
		[Tooltip("True if the Character Controller capsule was hit from below.")]
		[UIHint(10)]
		public FsmBool below;

		// Token: 0x04004CCE RID: 19662
		private GameObject previousGo;

		// Token: 0x04004CCF RID: 19663
		private CharacterController controller;
	}
}
