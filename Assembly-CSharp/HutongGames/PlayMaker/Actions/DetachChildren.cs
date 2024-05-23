using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B90 RID: 2960
	[ActionCategory(4)]
	[Tooltip("Unparents all children from the Game Object.")]
	public class DetachChildren : FsmStateAction
	{
		// Token: 0x060063B0 RID: 25520 RVA: 0x001DBAD3 File Offset: 0x001D9CD3
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060063B1 RID: 25521 RVA: 0x001DBADC File Offset: 0x001D9CDC
		public override void OnEnter()
		{
			DetachChildren.DoDetachChildren(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x060063B2 RID: 25522 RVA: 0x001DBAFA File Offset: 0x001D9CFA
		private static void DoDetachChildren(GameObject go)
		{
			if (go != null)
			{
				go.transform.DetachChildren();
			}
		}

		// Token: 0x04004B29 RID: 19241
		[Tooltip("GameObject to unparent children from.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;
	}
}
