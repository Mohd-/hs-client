using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE7 RID: 3303
	[ActionCategory(4)]
	[Tooltip("Sets a Game Object's Tag.")]
	public class SetTag : FsmStateAction
	{
		// Token: 0x06006947 RID: 26951 RVA: 0x001ED90B File Offset: 0x001EBB0B
		public override void Reset()
		{
			this.gameObject = null;
			this.tag = "Untagged";
		}

		// Token: 0x06006948 RID: 26952 RVA: 0x001ED924 File Offset: 0x001EBB24
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				ownerDefaultTarget.tag = this.tag.Value;
			}
			base.Finish();
		}

		// Token: 0x040050F6 RID: 20726
		public FsmOwnerDefault gameObject;

		// Token: 0x040050F7 RID: 20727
		[UIHint(7)]
		public FsmString tag;
	}
}
