using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D76 RID: 3446
	[ActionCategory("Animator")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1069")]
	[Tooltip("Sets the layer's current weight")]
	public class SetAnimatorLayerWeight : FsmStateAction
	{
		// Token: 0x06006BE6 RID: 27622 RVA: 0x001FB431 File Offset: 0x001F9631
		public override void Reset()
		{
			this.gameObject = null;
			this.layerIndex = null;
			this.layerWeight = null;
			this.everyFrame = false;
		}

		// Token: 0x06006BE7 RID: 27623 RVA: 0x001FB450 File Offset: 0x001F9650
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
			this.DoLayerWeight();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006BE8 RID: 27624 RVA: 0x001FB4BD File Offset: 0x001F96BD
		public override void OnUpdate()
		{
			this.DoLayerWeight();
		}

		// Token: 0x06006BE9 RID: 27625 RVA: 0x001FB4C8 File Offset: 0x001F96C8
		private void DoLayerWeight()
		{
			if (this._animator == null)
			{
				return;
			}
			this._animator.SetLayerWeight(this.layerIndex.Value, this.layerWeight.Value);
		}

		// Token: 0x04005467 RID: 21607
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The Target. An Animator component is required")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005468 RID: 21608
		[Tooltip("The layer's index")]
		[RequiredField]
		public FsmInt layerIndex;

		// Token: 0x04005469 RID: 21609
		[Tooltip("Sets the layer's current weight")]
		[RequiredField]
		public FsmFloat layerWeight;

		// Token: 0x0400546A RID: 21610
		[Tooltip("Repeat every frame. Useful for changing over time.")]
		public bool everyFrame;

		// Token: 0x0400546B RID: 21611
		private Animator _animator;
	}
}
