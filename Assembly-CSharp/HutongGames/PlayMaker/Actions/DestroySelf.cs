using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B8F RID: 2959
	[Tooltip("Destroys the Owner of the Fsm! Useful for spawned Prefabs that need to kill themselves, e.g., a projectile that explodes on impact.")]
	[ActionCategory(4)]
	public class DestroySelf : FsmStateAction
	{
		// Token: 0x060063AD RID: 25517 RVA: 0x001DBA6D File Offset: 0x001D9C6D
		public override void Reset()
		{
			this.detachChildren = false;
		}

		// Token: 0x060063AE RID: 25518 RVA: 0x001DBA7C File Offset: 0x001D9C7C
		public override void OnEnter()
		{
			if (base.Owner != null)
			{
				if (this.detachChildren.Value)
				{
					base.Owner.transform.DetachChildren();
				}
				Object.Destroy(base.Owner);
			}
			base.Finish();
		}

		// Token: 0x04004B28 RID: 19240
		[Tooltip("Detach children before destroying the Owner.")]
		public FsmBool detachChildren;
	}
}
