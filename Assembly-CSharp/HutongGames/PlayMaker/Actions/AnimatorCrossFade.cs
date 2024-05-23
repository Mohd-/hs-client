using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D3F RID: 3391
	[ActionCategory("Animator")]
	[Tooltip("Create a dynamic transition between the current state and the destination state.Both state as to be on the same layer. note: You cannot change the current state on a synchronized layer, you need to change it on the referenced layer.")]
	public class AnimatorCrossFade : FsmStateAction
	{
		// Token: 0x06006ACB RID: 27339 RVA: 0x001F7224 File Offset: 0x001F5424
		public override void Reset()
		{
			this.gameObject = null;
			this.stateName = null;
			this.transitionDuration = 1f;
			this.layer = new FsmInt
			{
				UseVariable = true
			};
			this.normalizedTime = new FsmFloat
			{
				UseVariable = true
			};
		}

		// Token: 0x06006ACC RID: 27340 RVA: 0x001F7278 File Offset: 0x001F5478
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
				this._animator.CrossFade(this.stateName.Value, this.transitionDuration.Value, num, num2);
			}
			base.Finish();
		}

		// Token: 0x04005327 RID: 21287
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005328 RID: 21288
		[Tooltip("The name of the state that will be played.")]
		public FsmString stateName;

		// Token: 0x04005329 RID: 21289
		[Tooltip("The duration of the transition. Value is in source state normalized time.")]
		public FsmFloat transitionDuration;

		// Token: 0x0400532A RID: 21290
		[Tooltip("Layer index containing the destination state. Leave to none to ignore")]
		public FsmInt layer;

		// Token: 0x0400532B RID: 21291
		[Tooltip("Start time of the current destination state. Value is in source state normalized time, should be between 0 and 1.")]
		public FsmFloat normalizedTime;

		// Token: 0x0400532C RID: 21292
		private Animator _animator;

		// Token: 0x0400532D RID: 21293
		private int _paramID;
	}
}
