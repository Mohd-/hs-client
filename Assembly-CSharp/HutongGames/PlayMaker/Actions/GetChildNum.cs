using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF3 RID: 3059
	[Tooltip("Gets the Child of a GameObject by Index.\nE.g., O to get the first child. HINT: Use this with an integer variable to iterate through children.")]
	[ActionCategory(4)]
	public class GetChildNum : FsmStateAction
	{
		// Token: 0x0600651D RID: 25885 RVA: 0x001E08CE File Offset: 0x001DEACE
		public override void Reset()
		{
			this.gameObject = null;
			this.childIndex = 0;
			this.store = null;
		}

		// Token: 0x0600651E RID: 25886 RVA: 0x001E08EC File Offset: 0x001DEAEC
		public override void OnEnter()
		{
			this.store.Value = this.DoGetChildNum(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x0600651F RID: 25887 RVA: 0x001E0924 File Offset: 0x001DEB24
		private GameObject DoGetChildNum(GameObject go)
		{
			return (!(go == null)) ? go.transform.GetChild(this.childIndex.Value % go.transform.childCount).gameObject : null;
		}

		// Token: 0x04004CB6 RID: 19638
		[RequiredField]
		[Tooltip("The GameObject to search.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004CB7 RID: 19639
		[RequiredField]
		[Tooltip("The index of the child to find.")]
		public FsmInt childIndex;

		// Token: 0x04004CB8 RID: 19640
		[UIHint(10)]
		[Tooltip("Store the child in a GameObject variable.")]
		[RequiredField]
		public FsmGameObject store;
	}
}
