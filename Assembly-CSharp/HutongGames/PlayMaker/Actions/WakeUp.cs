using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D1E RID: 3358
	[ActionCategory(9)]
	[Tooltip("Forces a Game Object's Rigid Body to wake up.")]
	public class WakeUp : ComponentAction<Rigidbody>
	{
		// Token: 0x06006A30 RID: 27184 RVA: 0x001F159F File Offset: 0x001EF79F
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x06006A31 RID: 27185 RVA: 0x001F15A8 File Offset: 0x001EF7A8
		public override void OnEnter()
		{
			this.DoWakeUp();
			base.Finish();
		}

		// Token: 0x06006A32 RID: 27186 RVA: 0x001F15B8 File Offset: 0x001EF7B8
		private void DoWakeUp()
		{
			GameObject go = (this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner;
			if (base.UpdateCache(go))
			{
				base.rigidbody.WakeUp();
			}
		}

		// Token: 0x0400520C RID: 21004
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		public FsmOwnerDefault gameObject;
	}
}
