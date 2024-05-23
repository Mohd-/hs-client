using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C7E RID: 3198
	[Tooltip("Rewinds the named animation.")]
	[ActionCategory(0)]
	public class RewindAnimation : ComponentAction<Animation>
	{
		// Token: 0x0600676C RID: 26476 RVA: 0x001E7EC8 File Offset: 0x001E60C8
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
		}

		// Token: 0x0600676D RID: 26477 RVA: 0x001E7ED8 File Offset: 0x001E60D8
		public override void OnEnter()
		{
			this.DoRewindAnimation();
			base.Finish();
		}

		// Token: 0x0600676E RID: 26478 RVA: 0x001E7EE8 File Offset: 0x001E60E8
		private void DoRewindAnimation()
		{
			if (string.IsNullOrEmpty(this.animName.Value))
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.animation.Rewind(this.animName.Value);
			}
		}

		// Token: 0x04004F41 RID: 20289
		[CheckForComponent(typeof(Animation))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004F42 RID: 20290
		[UIHint(6)]
		public FsmString animName;
	}
}
