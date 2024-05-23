using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF2 RID: 3058
	[Tooltip("Gets the number of children that a GameObject has.")]
	[ActionCategory(4)]
	public class GetChildCount : FsmStateAction
	{
		// Token: 0x06006519 RID: 25881 RVA: 0x001E0863 File Offset: 0x001DEA63
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
		}

		// Token: 0x0600651A RID: 25882 RVA: 0x001E0873 File Offset: 0x001DEA73
		public override void OnEnter()
		{
			this.DoGetChildCount();
			base.Finish();
		}

		// Token: 0x0600651B RID: 25883 RVA: 0x001E0884 File Offset: 0x001DEA84
		private void DoGetChildCount()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeResult.Value = ownerDefaultTarget.transform.childCount;
		}

		// Token: 0x04004CB4 RID: 19636
		[RequiredField]
		[Tooltip("The GameObject to test.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004CB5 RID: 19637
		[UIHint(10)]
		[RequiredField]
		[Tooltip("Store the number of children in an int variable.")]
		public FsmInt storeResult;
	}
}
