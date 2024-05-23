using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D70 RID: 3440
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1063")]
	[Tooltip("Sets the value of a bool parameter")]
	[ActionCategory("Animator")]
	public class SetAnimatorBool : FsmStateAction
	{
		// Token: 0x06006BC3 RID: 27587 RVA: 0x001FAAC8 File Offset: 0x001F8CC8
		public override void Reset()
		{
			this.gameObject = null;
			this.parameter = null;
			this.Value = null;
			this.everyFrame = false;
		}

		// Token: 0x06006BC4 RID: 27588 RVA: 0x001FAAE8 File Offset: 0x001F8CE8
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

		// Token: 0x06006BC5 RID: 27589 RVA: 0x001FAB9F File Offset: 0x001F8D9F
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.SetParameter();
			}
		}

		// Token: 0x06006BC6 RID: 27590 RVA: 0x001FABB8 File Offset: 0x001F8DB8
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.SetParameter();
			}
		}

		// Token: 0x06006BC7 RID: 27591 RVA: 0x001FABD1 File Offset: 0x001F8DD1
		private void SetParameter()
		{
			if (this._animator != null)
			{
				this._animator.SetBool(this._paramID, this.Value.Value);
			}
		}

		// Token: 0x06006BC8 RID: 27592 RVA: 0x001FAC00 File Offset: 0x001F8E00
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x04005440 RID: 21568
		[RequiredField]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005441 RID: 21569
		[Tooltip("The animator parameter")]
		public FsmString parameter;

		// Token: 0x04005442 RID: 21570
		[Tooltip("The Bool value to assign to the animator parameter")]
		public FsmBool Value;

		// Token: 0x04005443 RID: 21571
		[Tooltip("Repeat every frame. Useful when value is changing over time.")]
		public bool everyFrame;

		// Token: 0x04005444 RID: 21572
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x04005445 RID: 21573
		private Animator _animator;

		// Token: 0x04005446 RID: 21574
		private int _paramID;
	}
}
