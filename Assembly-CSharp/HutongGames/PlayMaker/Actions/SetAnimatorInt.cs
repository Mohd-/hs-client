using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D75 RID: 3445
	[Tooltip("Sets the value of a int parameter")]
	[ActionCategory("Animator")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1068")]
	public class SetAnimatorInt : FsmStateAction
	{
		// Token: 0x06006BDF RID: 27615 RVA: 0x001FB2BD File Offset: 0x001F94BD
		public override void Reset()
		{
			this.gameObject = null;
			this.parameter = null;
			this.Value = null;
			this.everyFrame = false;
		}

		// Token: 0x06006BE0 RID: 27616 RVA: 0x001FB2DC File Offset: 0x001F94DC
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
			this._paramID = Animator.StringToHash(this.parameter.Value);
			this.SetParameter();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006BE1 RID: 27617 RVA: 0x001FB393 File Offset: 0x001F9593
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.SetParameter();
			}
		}

		// Token: 0x06006BE2 RID: 27618 RVA: 0x001FB3AC File Offset: 0x001F95AC
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.SetParameter();
			}
		}

		// Token: 0x06006BE3 RID: 27619 RVA: 0x001FB3C5 File Offset: 0x001F95C5
		private void SetParameter()
		{
			if (this._animator != null)
			{
				this._animator.SetInteger(this._paramID, this.Value.Value);
			}
		}

		// Token: 0x06006BE4 RID: 27620 RVA: 0x001FB3F4 File Offset: 0x001F95F4
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x04005460 RID: 21600
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005461 RID: 21601
		[Tooltip("The animator parameter")]
		public FsmString parameter;

		// Token: 0x04005462 RID: 21602
		[Tooltip("The Int value to assign to the animator parameter")]
		public FsmInt Value;

		// Token: 0x04005463 RID: 21603
		[Tooltip("Repeat every frame. Useful when changing over time.")]
		public bool everyFrame;

		// Token: 0x04005464 RID: 21604
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x04005465 RID: 21605
		private Animator _animator;

		// Token: 0x04005466 RID: 21606
		private int _paramID;
	}
}
