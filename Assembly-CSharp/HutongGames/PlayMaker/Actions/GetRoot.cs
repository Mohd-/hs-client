using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C29 RID: 3113
	[ActionCategory(4)]
	[Tooltip("Gets the top most parent of the Game Object.\nIf the game object has no parent, returns itself.")]
	public class GetRoot : FsmStateAction
	{
		// Token: 0x06006605 RID: 26117 RVA: 0x001E3542 File Offset: 0x001E1742
		public override void Reset()
		{
			this.gameObject = null;
			this.storeRoot = null;
		}

		// Token: 0x06006606 RID: 26118 RVA: 0x001E3552 File Offset: 0x001E1752
		public override void OnEnter()
		{
			this.DoGetRoot();
			base.Finish();
		}

		// Token: 0x06006607 RID: 26119 RVA: 0x001E3560 File Offset: 0x001E1760
		private void DoGetRoot()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeRoot.Value = ownerDefaultTarget.transform.root.gameObject;
		}

		// Token: 0x04004DC2 RID: 19906
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004DC3 RID: 19907
		[UIHint(10)]
		[RequiredField]
		public FsmGameObject storeRoot;
	}
}
