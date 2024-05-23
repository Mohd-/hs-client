using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D71 RID: 3441
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1064")]
	[Tooltip("Controls culling of this Animator component.\nIf true, set to 'AlwaysAnimate': always animate the entire character. Object is animated even when offscreen.\nIf False, set to 'BasedOnRenderes' animation is disabled when renderers are not visible.")]
	[ActionCategory("Animator")]
	public class SetAnimatorCullingMode : FsmStateAction
	{
		// Token: 0x06006BCA RID: 27594 RVA: 0x001FAC3D File Offset: 0x001F8E3D
		public override void Reset()
		{
			this.gameObject = null;
			this.alwaysAnimate = null;
		}

		// Token: 0x06006BCB RID: 27595 RVA: 0x001FAC50 File Offset: 0x001F8E50
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			this._animator = ownerDefaultTarget.GetComponent<Animator>();
			if (this._animator == null)
			{
				base.Finish();
				return;
			}
			this.SetCullingMode();
			base.Finish();
		}

		// Token: 0x06006BCC RID: 27596 RVA: 0x001FACB4 File Offset: 0x001F8EB4
		private void SetCullingMode()
		{
			if (this._animator == null)
			{
				return;
			}
			this._animator.cullingMode = ((!this.alwaysAnimate.Value) ? 1 : 0);
		}

		// Token: 0x04005447 RID: 21575
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The Target. An Animator component is required")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005448 RID: 21576
		[Tooltip("If true, always animate the entire character, else animation is disabled when renderers are not visible")]
		public FsmBool alwaysAnimate;

		// Token: 0x04005449 RID: 21577
		private Animator _animator;
	}
}
