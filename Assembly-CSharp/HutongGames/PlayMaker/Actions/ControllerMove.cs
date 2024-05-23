using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B71 RID: 2929
	[Tooltip("Moves a Game Object with a Character Controller. See also Controller Simple Move. NOTE: It is recommended that you make only one call to Move or SimpleMove per frame.")]
	[ActionCategory(32)]
	public class ControllerMove : FsmStateAction
	{
		// Token: 0x0600633B RID: 25403 RVA: 0x001DA648 File Offset: 0x001D8848
		public override void Reset()
		{
			this.gameObject = null;
			this.moveVector = new FsmVector3
			{
				UseVariable = true
			};
			this.space = 0;
			this.perSecond = true;
		}

		// Token: 0x0600633C RID: 25404 RVA: 0x001DA684 File Offset: 0x001D8884
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
				if (this.perSecond.Value)
				{
					this.controller.Move(vector * Time.deltaTime);
				}
				else
				{
					this.controller.Move(vector);
				}
			}
		}

		// Token: 0x04004AB5 RID: 19125
		[Tooltip("The GameObject to move.")]
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004AB6 RID: 19126
		[Tooltip("The movement vector.")]
		[RequiredField]
		public FsmVector3 moveVector;

		// Token: 0x04004AB7 RID: 19127
		[Tooltip("Move in local or word space.")]
		public Space space;

		// Token: 0x04004AB8 RID: 19128
		[Tooltip("Movement vector is defined in units per second. Makes movement frame rate independent.")]
		public FsmBool perSecond;

		// Token: 0x04004AB9 RID: 19129
		private GameObject previousGo;

		// Token: 0x04004ABA RID: 19130
		private CharacterController controller;
	}
}
