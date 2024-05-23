using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B9B RID: 2971
	[Tooltip("Enables/Disables an Animation on a GameObject.\nAnimation time is paused while disabled. Animation must also have a non zero weight to play.")]
	[ActionCategory(0)]
	public class EnableAnimation : FsmStateAction
	{
		// Token: 0x060063D4 RID: 25556 RVA: 0x001DC084 File Offset: 0x001DA284
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.enable = true;
			this.resetOnExit = false;
		}

		// Token: 0x060063D5 RID: 25557 RVA: 0x001DC0B7 File Offset: 0x001DA2B7
		public override void OnEnter()
		{
			this.DoEnableAnimation(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x060063D6 RID: 25558 RVA: 0x001DC0D8 File Offset: 0x001DA2D8
		private void DoEnableAnimation(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			Animation component = go.GetComponent<Animation>();
			if (component == null)
			{
				this.LogError("Missing animation component!");
				return;
			}
			this.anim = component[this.animName.Value];
			if (this.anim != null)
			{
				this.anim.enabled = this.enable.Value;
			}
		}

		// Token: 0x060063D7 RID: 25559 RVA: 0x001DC150 File Offset: 0x001DA350
		public override void OnExit()
		{
			if (this.resetOnExit.Value && this.anim != null)
			{
				this.anim.enabled = !this.enable.Value;
			}
		}

		// Token: 0x04004B49 RID: 19273
		[Tooltip("The GameObject playing the animation.")]
		[CheckForComponent(typeof(Animation))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004B4A RID: 19274
		[RequiredField]
		[UIHint(6)]
		[Tooltip("The name of the animation to enable/disable.")]
		public FsmString animName;

		// Token: 0x04004B4B RID: 19275
		[RequiredField]
		[Tooltip("Set to True to enable, False to disable.")]
		public FsmBool enable;

		// Token: 0x04004B4C RID: 19276
		[Tooltip("Reset the initial enabled state when exiting the state.")]
		public FsmBool resetOnExit;

		// Token: 0x04004B4D RID: 19277
		private AnimationState anim;
	}
}
