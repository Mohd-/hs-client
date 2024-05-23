using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C21 RID: 3105
	[ActionCategory(4)]
	[Tooltip("Gets the Parent of a Game Object.")]
	public class GetParent : FsmStateAction
	{
		// Token: 0x060065E3 RID: 26083 RVA: 0x001E2FDA File Offset: 0x001E11DA
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
		}

		// Token: 0x060065E4 RID: 26084 RVA: 0x001E2FEC File Offset: 0x001E11EC
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				this.storeResult.Value = ((!(ownerDefaultTarget.transform.parent == null)) ? ownerDefaultTarget.transform.parent.gameObject : null);
			}
			else
			{
				this.storeResult.Value = null;
			}
			base.Finish();
		}

		// Token: 0x04004DA6 RID: 19878
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004DA7 RID: 19879
		[UIHint(10)]
		public FsmGameObject storeResult;
	}
}
