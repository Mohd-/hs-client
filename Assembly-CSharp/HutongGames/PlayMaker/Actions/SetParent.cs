using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CDB RID: 3291
	[Tooltip("Sets the Parent of a Game Object.")]
	[ActionCategory(4)]
	public class SetParent : FsmStateAction
	{
		// Token: 0x0600690E RID: 26894 RVA: 0x001ECCF9 File Offset: 0x001EAEF9
		public override void Reset()
		{
			this.gameObject = null;
			this.parent = null;
			this.resetLocalPosition = null;
			this.resetLocalRotation = null;
		}

		// Token: 0x0600690F RID: 26895 RVA: 0x001ECD18 File Offset: 0x001EAF18
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				ownerDefaultTarget.transform.parent = ((!(this.parent.Value == null)) ? this.parent.Value.transform : null);
				if (this.resetLocalPosition.Value)
				{
					ownerDefaultTarget.transform.localPosition = Vector3.zero;
				}
				if (this.resetLocalRotation.Value)
				{
					ownerDefaultTarget.transform.localRotation = Quaternion.identity;
				}
			}
			base.Finish();
		}

		// Token: 0x040050C0 RID: 20672
		[Tooltip("The Game Object to parent.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040050C1 RID: 20673
		[Tooltip("The new parent for the Game Object.")]
		public FsmGameObject parent;

		// Token: 0x040050C2 RID: 20674
		[Tooltip("Set the local position to 0,0,0 after parenting.")]
		public FsmBool resetLocalPosition;

		// Token: 0x040050C3 RID: 20675
		[Tooltip("Set the local rotation to 0,0,0 after parenting.")]
		public FsmBool resetLocalRotation;
	}
}
