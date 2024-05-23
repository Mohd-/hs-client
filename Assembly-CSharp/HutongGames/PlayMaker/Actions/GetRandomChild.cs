using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C25 RID: 3109
	[Tooltip("Gets a Random Child of a Game Object.")]
	[ActionCategory(4)]
	public class GetRandomChild : FsmStateAction
	{
		// Token: 0x060065F2 RID: 26098 RVA: 0x001E31FF File Offset: 0x001E13FF
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
		}

		// Token: 0x060065F3 RID: 26099 RVA: 0x001E320F File Offset: 0x001E140F
		public override void OnEnter()
		{
			this.DoGetRandomChild();
			base.Finish();
		}

		// Token: 0x060065F4 RID: 26100 RVA: 0x001E3220 File Offset: 0x001E1420
		private void DoGetRandomChild()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			int childCount = ownerDefaultTarget.transform.childCount;
			if (childCount == 0)
			{
				return;
			}
			this.storeResult.Value = ownerDefaultTarget.transform.GetChild(Random.Range(0, childCount)).gameObject;
		}

		// Token: 0x04004DB2 RID: 19890
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004DB3 RID: 19891
		[UIHint(10)]
		[RequiredField]
		public FsmGameObject storeResult;
	}
}
