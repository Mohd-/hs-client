using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D4C RID: 3404
	[Tooltip("Returns the culling of this Animator component. Optionnaly sends events.\nIf true ('AlwaysAnimate'): always animate the entire character. Object is animated even when offscreen.\nIf False ('BasedOnRenderers') animation is disabled when renderers are not visible.")]
	[ActionCategory("Animator")]
	public class GetAnimatorCullingMode : FsmStateAction
	{
		// Token: 0x06006B01 RID: 27393 RVA: 0x001F7E3D File Offset: 0x001F603D
		public override void Reset()
		{
			this.gameObject = null;
			this.alwaysAnimate = null;
			this.alwaysAnimateEvent = null;
			this.basedOnRenderersEvent = null;
		}

		// Token: 0x06006B02 RID: 27394 RVA: 0x001F7E5C File Offset: 0x001F605C
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
			this.DoCheckCulling();
			base.Finish();
		}

		// Token: 0x06006B03 RID: 27395 RVA: 0x001F7EC0 File Offset: 0x001F60C0
		private void DoCheckCulling()
		{
			if (this._animator == null)
			{
				return;
			}
			bool flag = this._animator.cullingMode == null;
			this.alwaysAnimate.Value = flag;
			if (flag)
			{
				base.Fsm.Event(this.alwaysAnimateEvent);
			}
			else
			{
				base.Fsm.Event(this.basedOnRenderersEvent);
			}
		}

		// Token: 0x04005363 RID: 21347
		[RequiredField]
		[Tooltip("The Target. An Animator component is required")]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005364 RID: 21348
		[UIHint(10)]
		[Tooltip("If true, always animate the entire character, else animation is disabled when renderers are not visible")]
		[RequiredField]
		[ActionSection("Results")]
		public FsmBool alwaysAnimate;

		// Token: 0x04005365 RID: 21349
		[Tooltip("Event send if culling mode is 'AlwaysAnimate'")]
		public FsmEvent alwaysAnimateEvent;

		// Token: 0x04005366 RID: 21350
		[Tooltip("Event send if culling mode is 'BasedOnRenders'")]
		public FsmEvent basedOnRenderersEvent;

		// Token: 0x04005367 RID: 21351
		private Animator _animator;
	}
}
