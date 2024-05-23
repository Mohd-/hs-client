using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D60 RID: 3424
	[Tooltip("Returns the name of a layer from its index")]
	[ActionCategory("Animator")]
	public class GetAnimatorLayerName : FsmStateAction
	{
		// Token: 0x06006B6B RID: 27499 RVA: 0x001F976D File Offset: 0x001F796D
		public override void Reset()
		{
			this.gameObject = null;
			this.layerIndex = null;
			this.layerName = null;
		}

		// Token: 0x06006B6C RID: 27500 RVA: 0x001F9784 File Offset: 0x001F7984
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
			this.DoGetLayerName();
			base.Finish();
		}

		// Token: 0x06006B6D RID: 27501 RVA: 0x001F97E6 File Offset: 0x001F79E6
		private void DoGetLayerName()
		{
			if (this._animator == null)
			{
				return;
			}
			this.layerName.Value = this._animator.GetLayerName(this.layerIndex.Value);
		}

		// Token: 0x040053E8 RID: 21480
		[RequiredField]
		[Tooltip("The Target. An Animator component is required")]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053E9 RID: 21481
		[RequiredField]
		[Tooltip("The layer index")]
		public FsmInt layerIndex;

		// Token: 0x040053EA RID: 21482
		[ActionSection("Results")]
		[Tooltip("The layer name")]
		[RequiredField]
		[UIHint(10)]
		public FsmString layerName;

		// Token: 0x040053EB RID: 21483
		private Animator _animator;
	}
}
