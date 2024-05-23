using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D0B RID: 3339
	[Tooltip("Sets whether a Game Object's Rigidy Body is affected by Gravity.")]
	[ActionCategory(9)]
	public class UseGravity : ComponentAction<Rigidbody>
	{
		// Token: 0x060069E3 RID: 27107 RVA: 0x001F0444 File Offset: 0x001EE644
		public override void Reset()
		{
			this.gameObject = null;
			this.useGravity = true;
		}

		// Token: 0x060069E4 RID: 27108 RVA: 0x001F0459 File Offset: 0x001EE659
		public override void OnEnter()
		{
			this.DoUseGravity();
			base.Finish();
		}

		// Token: 0x060069E5 RID: 27109 RVA: 0x001F0468 File Offset: 0x001EE668
		private void DoUseGravity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.rigidbody.useGravity = this.useGravity.Value;
			}
		}

		// Token: 0x040051B6 RID: 20918
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040051B7 RID: 20919
		[RequiredField]
		public FsmBool useGravity;
	}
}
