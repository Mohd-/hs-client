using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D61 RID: 3425
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1052")]
	[Tooltip("Gets the layer's current weight")]
	[ActionCategory("Animator")]
	public class GetAnimatorLayerWeight : FsmStateAction
	{
		// Token: 0x06006B6F RID: 27503 RVA: 0x001F9823 File Offset: 0x001F7A23
		public override void Reset()
		{
			this.gameObject = null;
			this.layerIndex = null;
			this.layerWeight = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B70 RID: 27504 RVA: 0x001F9844 File Offset: 0x001F7A44
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
			this._animatorProxy = ownerDefaultTarget.GetComponent<PlayMakerAnimatorMoveProxy>();
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent += new Action(this.OnAnimatorMoveEvent);
			}
			this.GetLayerWeight();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006B71 RID: 27505 RVA: 0x001F98E5 File Offset: 0x001F7AE5
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.GetLayerWeight();
			}
		}

		// Token: 0x06006B72 RID: 27506 RVA: 0x001F98FE File Offset: 0x001F7AFE
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.GetLayerWeight();
			}
		}

		// Token: 0x06006B73 RID: 27507 RVA: 0x001F9917 File Offset: 0x001F7B17
		private void GetLayerWeight()
		{
			if (this._animator != null)
			{
				this.layerWeight.Value = this._animator.GetLayerWeight(this.layerIndex.Value);
			}
		}

		// Token: 0x06006B74 RID: 27508 RVA: 0x001F994C File Offset: 0x001F7B4C
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x040053EC RID: 21484
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053ED RID: 21485
		[RequiredField]
		[Tooltip("The layer's index")]
		public FsmInt layerIndex;

		// Token: 0x040053EE RID: 21486
		[Tooltip("Repeat every frame. Useful when value is subject to change over time.")]
		public bool everyFrame;

		// Token: 0x040053EF RID: 21487
		[RequiredField]
		[UIHint(10)]
		[Tooltip("The layer's current weight")]
		[ActionSection("Results")]
		public FsmFloat layerWeight;

		// Token: 0x040053F0 RID: 21488
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x040053F1 RID: 21489
		private Animator _animator;
	}
}
