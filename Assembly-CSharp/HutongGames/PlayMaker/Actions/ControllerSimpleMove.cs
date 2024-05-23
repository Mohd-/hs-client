using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B73 RID: 2931
	[Tooltip("Moves a Game Object with a Character Controller. Velocity along the y-axis is ignored. Speed is in meters/s. Gravity is automatically applied.")]
	[ActionCategory(32)]
	public class ControllerSimpleMove : FsmStateAction
	{
		// Token: 0x06006343 RID: 25411 RVA: 0x001DA95C File Offset: 0x001D8B5C
		public override void Reset()
		{
			this.gameObject = null;
			this.moveVector = new FsmVector3
			{
				UseVariable = true
			};
			this.speed = 1f;
			this.space = 0;
		}

		// Token: 0x06006344 RID: 25412 RVA: 0x001DA99C File Offset: 0x001D8B9C
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
				Vector3 vector = (this.space != null) ? ownerDefaultTarget.transform.TransformDirection(this.moveVector.Value) : this.moveVector.Value;
				this.controller.SimpleMove(vector * this.speed.Value);
			}
		}

		// Token: 0x04004AC5 RID: 19141
		[CheckForComponent(typeof(CharacterController))]
		[Tooltip("The GameObject to move.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004AC6 RID: 19142
		[RequiredField]
		[Tooltip("The movement vector.")]
		public FsmVector3 moveVector;

		// Token: 0x04004AC7 RID: 19143
		[Tooltip("Multiply the movement vector by a speed factor.")]
		public FsmFloat speed;

		// Token: 0x04004AC8 RID: 19144
		[Tooltip("Move in local or word space.")]
		public Space space;

		// Token: 0x04004AC9 RID: 19145
		private GameObject previousGo;

		// Token: 0x04004ACA RID: 19146
		private CharacterController controller;
	}
}
