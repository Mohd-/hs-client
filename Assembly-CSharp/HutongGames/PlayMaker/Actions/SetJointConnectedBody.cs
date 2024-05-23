using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC8 RID: 3272
	[Tooltip("Connect a joint to a game object.")]
	[ActionCategory(9)]
	public class SetJointConnectedBody : FsmStateAction
	{
		// Token: 0x060068BF RID: 26815 RVA: 0x001EBF79 File Offset: 0x001EA179
		public override void Reset()
		{
			this.joint = null;
			this.rigidBody = null;
		}

		// Token: 0x060068C0 RID: 26816 RVA: 0x001EBF8C File Offset: 0x001EA18C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.joint);
			if (ownerDefaultTarget != null)
			{
				Joint component = ownerDefaultTarget.GetComponent<Joint>();
				if (component != null)
				{
					component.connectedBody = ((!(this.rigidBody.Value == null)) ? this.rigidBody.Value.GetComponent<Rigidbody>() : null);
				}
			}
			base.Finish();
		}

		// Token: 0x04005086 RID: 20614
		[Tooltip("The joint to connect. Requires a Joint component.")]
		[RequiredField]
		[CheckForComponent(typeof(Joint))]
		public FsmOwnerDefault joint;

		// Token: 0x04005087 RID: 20615
		[CheckForComponent(typeof(Rigidbody))]
		[Tooltip("The game object to connect to the Joint. Set to none to connect the Joint to the world.")]
		public FsmGameObject rigidBody;
	}
}
