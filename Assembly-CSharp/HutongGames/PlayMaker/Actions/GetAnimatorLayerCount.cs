using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D5F RID: 3423
	[Tooltip("Returns the Animator controller layer count")]
	[ActionCategory("Animator")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1051")]
	public class GetAnimatorLayerCount : FsmStateAction
	{
		// Token: 0x06006B67 RID: 27495 RVA: 0x001F96BB File Offset: 0x001F78BB
		public override void Reset()
		{
			this.gameObject = null;
			this.layerCount = null;
		}

		// Token: 0x06006B68 RID: 27496 RVA: 0x001F96CC File Offset: 0x001F78CC
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
			this.DoGetLayerCount();
			base.Finish();
		}

		// Token: 0x06006B69 RID: 27497 RVA: 0x001F9730 File Offset: 0x001F7930
		private void DoGetLayerCount()
		{
			if (this._animator == null)
			{
				return;
			}
			this.layerCount.Value = this._animator.layerCount;
		}

		// Token: 0x040053E5 RID: 21477
		[Tooltip("The Target. An Animator component is required")]
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040053E6 RID: 21478
		[RequiredField]
		[ActionSection("Results")]
		[UIHint(10)]
		[Tooltip("The Animator controller layer count")]
		public FsmInt layerCount;

		// Token: 0x040053E7 RID: 21479
		private Animator _animator;
	}
}
