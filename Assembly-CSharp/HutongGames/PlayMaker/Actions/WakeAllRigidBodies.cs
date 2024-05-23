using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D1D RID: 3357
	[ActionCategory(9)]
	[Tooltip("Rigid bodies start sleeping when they come to rest. This action wakes up all rigid bodies in the scene. E.g., if you Set Gravity and want objects at rest to respond.")]
	public class WakeAllRigidBodies : FsmStateAction
	{
		// Token: 0x06006A2B RID: 27179 RVA: 0x001F14F9 File Offset: 0x001EF6F9
		public override void Reset()
		{
			this.everyFrame = false;
		}

		// Token: 0x06006A2C RID: 27180 RVA: 0x001F1502 File Offset: 0x001EF702
		public override void OnEnter()
		{
			this.bodies = (Object.FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[]);
			this.DoWakeAll();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A2D RID: 27181 RVA: 0x001F1535 File Offset: 0x001EF735
		public override void OnUpdate()
		{
			this.DoWakeAll();
		}

		// Token: 0x06006A2E RID: 27182 RVA: 0x001F1540 File Offset: 0x001EF740
		private void DoWakeAll()
		{
			this.bodies = (Object.FindObjectsOfType(typeof(Rigidbody)) as Rigidbody[]);
			if (this.bodies != null)
			{
				foreach (Rigidbody rigidbody in this.bodies)
				{
					rigidbody.WakeUp();
				}
			}
		}

		// Token: 0x0400520A RID: 21002
		public bool everyFrame;

		// Token: 0x0400520B RID: 21003
		private Rigidbody[] bodies;
	}
}
