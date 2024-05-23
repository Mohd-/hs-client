using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF5 RID: 3317
	[Tooltip("Stops all playing Animations on a Game Object. Optionally, specify a single Animation to Stop.")]
	[ActionCategory(0)]
	public class StopAnimation : ComponentAction<Animation>
	{
		// Token: 0x06006989 RID: 27017 RVA: 0x001EEE01 File Offset: 0x001ED001
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
		}

		// Token: 0x0600698A RID: 27018 RVA: 0x001EEE11 File Offset: 0x001ED011
		public override void OnEnter()
		{
			this.DoStopAnimation();
			base.Finish();
		}

		// Token: 0x0600698B RID: 27019 RVA: 0x001EEE20 File Offset: 0x001ED020
		private void DoStopAnimation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			if (this.animName == null || string.IsNullOrEmpty(this.animName.Value))
			{
				base.animation.Stop();
			}
			else
			{
				base.animation.Stop(this.animName.Value);
			}
		}

		// Token: 0x04005148 RID: 20808
		[CheckForComponent(typeof(Animation))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005149 RID: 20809
		[UIHint(6)]
		[Tooltip("Leave empty to stop all playing animations.")]
		public FsmString animName;
	}
}
