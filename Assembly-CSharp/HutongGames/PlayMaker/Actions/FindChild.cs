using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA1 RID: 2977
	[ActionCategory(4)]
	[Tooltip("Finds the Child of a GameObject by Name.\nNote, you can specify a path to the child, e.g., LeftShoulder/Arm/Hand/Finger. If you need to specify a tag, use GetChild.")]
	public class FindChild : FsmStateAction
	{
		// Token: 0x060063F2 RID: 25586 RVA: 0x001DC6B8 File Offset: 0x001DA8B8
		public override void Reset()
		{
			this.gameObject = null;
			this.childName = string.Empty;
			this.storeResult = null;
		}

		// Token: 0x060063F3 RID: 25587 RVA: 0x001DC6D8 File Offset: 0x001DA8D8
		public override void OnEnter()
		{
			this.DoFindChild();
			base.Finish();
		}

		// Token: 0x060063F4 RID: 25588 RVA: 0x001DC6E8 File Offset: 0x001DA8E8
		private void DoFindChild()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Transform transform = ownerDefaultTarget.transform.FindChild(this.childName.Value);
			this.storeResult.Value = ((!(transform != null)) ? null : transform.gameObject);
		}

		// Token: 0x04004B65 RID: 19301
		[RequiredField]
		[Tooltip("The GameObject to search.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004B66 RID: 19302
		[Tooltip("The name of the child. Note, you can specify a path to the child, e.g., LeftShoulder/Arm/Hand/Finger")]
		[RequiredField]
		public FsmString childName;

		// Token: 0x04004B67 RID: 19303
		[Tooltip("Store the child in a GameObject variable.")]
		[UIHint(10)]
		[RequiredField]
		public FsmGameObject storeResult;
	}
}
