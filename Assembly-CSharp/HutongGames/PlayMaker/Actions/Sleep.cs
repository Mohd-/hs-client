using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CEF RID: 3311
	[Tooltip("Forces a Game Object's Rigid Body to Sleep at least one frame.")]
	[ActionCategory(9)]
	public class Sleep : ComponentAction<Rigidbody>
	{
		// Token: 0x0600696E RID: 26990 RVA: 0x001EE27A File Offset: 0x001EC47A
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x0600696F RID: 26991 RVA: 0x001EE283 File Offset: 0x001EC483
		public override void OnEnter()
		{
			this.DoSleep();
			base.Finish();
		}

		// Token: 0x06006970 RID: 26992 RVA: 0x001EE294 File Offset: 0x001EC494
		private void DoSleep()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.rigidbody.Sleep();
			}
		}

		// Token: 0x0400511D RID: 20765
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		public FsmOwnerDefault gameObject;
	}
}
