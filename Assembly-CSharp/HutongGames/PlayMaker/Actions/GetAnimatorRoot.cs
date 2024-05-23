using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D69 RID: 3433
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1036")]
	[ActionCategory("Animator")]
	[Tooltip("Gets the avatar body mass center position and rotation.Optionally accept a GameObject to get the body transform. \nThe position and rotation are local to the gameobject")]
	public class GetAnimatorRoot : FsmStateAction
	{
		// Token: 0x06006B9C RID: 27548 RVA: 0x001FA176 File Offset: 0x001F8376
		public override void Reset()
		{
			this.gameObject = null;
			this.rootPosition = null;
			this.rootRotation = null;
			this.bodyGameObject = null;
			this.everyFrame = false;
		}

		// Token: 0x06006B9D RID: 27549 RVA: 0x001FA19C File Offset: 0x001F839C
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

		// Token: 0x06006B9E RID: 27550 RVA: 0x001FA261 File Offset: 0x001F8461
		public void OnAnimatorMoveEvent()
		{
			if (this._animatorProxy != null)
			{
				this.DoGetBodyPosition();
			}
		}

		// Token: 0x06006B9F RID: 27551 RVA: 0x001FA27A File Offset: 0x001F847A
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.DoGetBodyPosition();
			}
		}

		// Token: 0x06006BA0 RID: 27552 RVA: 0x001FA294 File Offset: 0x001F8494
		private void DoGetBodyPosition()
		{
			if (this._animator == null)
			{
				return;
			}
			this.rootPosition.Value = this._animator.rootPosition;
			this.rootRotation.Value = this._animator.rootRotation;
			if (this._transform != null)
			{
				this._transform.position = this._animator.rootPosition;
				this._transform.rotation = this._animator.rootRotation;
			}
		}

		// Token: 0x06006BA1 RID: 27553 RVA: 0x001FA31C File Offset: 0x001F851C
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x0400541A RID: 21530
		[CheckForComponent(typeof(Animator))]
		[RequiredField]
		[Tooltip("The target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400541B RID: 21531
		[Tooltip("Repeat every frame. Useful when changing over time.")]
		public bool everyFrame;

		// Token: 0x0400541C RID: 21532
		[Tooltip("The avatar body mass center")]
		[ActionSection("Results")]
		[UIHint(10)]
		public FsmVector3 rootPosition;

		// Token: 0x0400541D RID: 21533
		[UIHint(10)]
		[Tooltip("The avatar body mass center")]
		public FsmQuaternion rootRotation;

		// Token: 0x0400541E RID: 21534
		[Tooltip("If set, apply the body mass center position and rotation to this gameObject")]
		public FsmGameObject bodyGameObject;

		// Token: 0x0400541F RID: 21535
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x04005420 RID: 21536
		private Animator _animator;

		// Token: 0x04005421 RID: 21537
		private Transform _transform;
	}
}
