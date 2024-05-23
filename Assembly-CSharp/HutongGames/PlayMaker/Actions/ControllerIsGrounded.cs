using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B70 RID: 2928
	[Tooltip("Tests if a Character Controller on a Game Object was touching the ground during the last move.")]
	[ActionCategory(32)]
	public class ControllerIsGrounded : FsmStateAction
	{
		// Token: 0x06006336 RID: 25398 RVA: 0x001DA55D File Offset: 0x001D875D
		public override void Reset()
		{
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006337 RID: 25399 RVA: 0x001DA582 File Offset: 0x001D8782
		public override void OnEnter()
		{
			this.DoControllerIsGrounded();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006338 RID: 25400 RVA: 0x001DA59B File Offset: 0x001D879B
		public override void OnUpdate()
		{
			this.DoControllerIsGrounded();
		}

		// Token: 0x06006339 RID: 25401 RVA: 0x001DA5A4 File Offset: 0x001D87A4
		private void DoControllerIsGrounded()
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
			if (this.controller == null)
			{
				return;
			}
			bool isGrounded = this.controller.isGrounded;
			this.storeResult.Value = isGrounded;
			base.Fsm.Event((!isGrounded) ? this.falseEvent : this.trueEvent);
		}

		// Token: 0x04004AAE RID: 19118
		[Tooltip("The GameObject to check.")]
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004AAF RID: 19119
		[Tooltip("Event to send if touching the ground.")]
		public FsmEvent trueEvent;

		// Token: 0x04004AB0 RID: 19120
		[Tooltip("Event to send if not touching the ground.")]
		public FsmEvent falseEvent;

		// Token: 0x04004AB1 RID: 19121
		[Tooltip("Sore the result in a bool variable.")]
		[UIHint(10)]
		public FsmBool storeResult;

		// Token: 0x04004AB2 RID: 19122
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x04004AB3 RID: 19123
		private GameObject previousGo;

		// Token: 0x04004AB4 RID: 19124
		private CharacterController controller;
	}
}
