using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D42 RID: 3394
	[Tooltip("Plays a state. This could be used to synchronize your animation with audio or synchronize an Animator over the network.")]
	[ActionCategory("Animator")]
	public class AnimatorPlay : FsmStateAction
	{
		// Token: 0x06006AD6 RID: 27350 RVA: 0x001F75C0 File Offset: 0x001F57C0
		public override void Reset()
		{
			this.gameObject = null;
			this.stateName = null;
			this.layer = new FsmInt
			{
				UseVariable = true
			};
			this.normalizedTime = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06006AD7 RID: 27351 RVA: 0x001F7604 File Offset: 0x001F5804
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			this._animator = ownerDefaultTarget.GetComponent<Animator>();
			if (this._animator != null)
			{
				int num = (!this.layer.IsNone) ? this.layer.Value : -1;
				float num2 = (!this.normalizedTime.IsNone) ? this.normalizedTime.Value : float.NegativeInfinity;
				this._animator.Play(this.stateName.Value, num, num2);
			}
			base.Finish();
		}

		// Token: 0x0400533C RID: 21308
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		[Tooltip("The target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400533D RID: 21309
		[Tooltip("The name of the state that will be played.")]
		public FsmString stateName;

		// Token: 0x0400533E RID: 21310
		[Tooltip("The layer where the state is.")]
		public FsmInt layer;

		// Token: 0x0400533F RID: 21311
		[Tooltip("The normalized time at which the state will play")]
		public FsmFloat normalizedTime;

		// Token: 0x04005340 RID: 21312
		private Animator _animator;
	}
}
