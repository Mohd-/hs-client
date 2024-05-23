using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D48 RID: 3400
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1036")]
	[ActionCategory("Animator")]
	[Tooltip("Gets the avatar body mass center position and rotation.Optionally accept a GameObject to get the body transform. \nThe position and rotation are local to the gameobject")]
	public class GetAnimatorBody : FsmStateAction
	{
		// Token: 0x06006AE9 RID: 27369 RVA: 0x001F7987 File Offset: 0x001F5B87
		public override void Reset()
		{
			this.gameObject = null;
			this.bodyPosition = null;
			this.bodyRotation = null;
			this.bodyGameObject = null;
			this.everyFrame = false;
		}

		// Token: 0x06006AEA RID: 27370 RVA: 0x001F79AC File Offset: 0x001F5BAC
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
			GameObject value = this.bodyGameObject.Value;
			if (value != null)
			{
				this._transform = value.transform;
			}
			this.DoGetBodyPosition();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006AEB RID: 27371 RVA: 0x001F7A71 File Offset: 0x001F5C71
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.DoGetBodyPosition();
			}
		}

		// Token: 0x06006AEC RID: 27372 RVA: 0x001F7A8A File Offset: 0x001F5C8A
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.DoGetBodyPosition();
			}
		}

		// Token: 0x06006AED RID: 27373 RVA: 0x001F7AA4 File Offset: 0x001F5CA4
		private void DoGetBodyPosition()
		{
			if (this._animator == null)
			{
				return;
			}
			this.bodyPosition.Value = this._animator.bodyPosition;
			this.bodyRotation.Value = this._animator.bodyRotation;
			if (this._transform != null)
			{
				this._transform.position = this._animator.bodyPosition;
				this._transform.rotation = this._animator.bodyRotation;
			}
		}

		// Token: 0x06006AEE RID: 27374 RVA: 0x001F7B2C File Offset: 0x001F5D2C
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x0400534D RID: 21325
		[RequiredField]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400534E RID: 21326
		[Tooltip("Repeat every frame. Useful when changing over time.")]
		public bool everyFrame;

		// Token: 0x0400534F RID: 21327
		[UIHint(10)]
		[Tooltip("The avatar body mass center")]
		[ActionSection("Results")]
		public FsmVector3 bodyPosition;

		// Token: 0x04005350 RID: 21328
		[UIHint(10)]
		[Tooltip("The avatar body mass center")]
		public FsmQuaternion bodyRotation;

		// Token: 0x04005351 RID: 21329
		[Tooltip("If set, apply the body mass center position and rotation to this gameObject")]
		public FsmGameObject bodyGameObject;

		// Token: 0x04005352 RID: 21330
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x04005353 RID: 21331
		private Animator _animator;

		// Token: 0x04005354 RID: 21332
		private Transform _transform;
	}
}
