using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D6E RID: 3438
	[Tooltip("Sets the position and rotation of the body. A GameObject can be set to control the position and rotation, or it can be manually expressed.")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1062")]
	[ActionCategory("Animator")]
	public class SetAnimatorBody : FsmStateAction
	{
		// Token: 0x06006BB8 RID: 27576 RVA: 0x001FA7D0 File Offset: 0x001F89D0
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.rotation = new FsmQuaternion
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x06006BB9 RID: 27577 RVA: 0x001FA81C File Offset: 0x001F8A1C
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
			this._animatorProxy = ownerDefaultTarget.GetComponent<PlayMakerAnimatorIKProxy>();
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorIKEvent += new Action<int>(this.OnAnimatorIKEvent);
			}
			GameObject value = this.target.Value;
			if (value != null)
			{
				this._transform = value.transform;
			}
			this.DoSetBody();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006BBA RID: 27578 RVA: 0x001FA8E1 File Offset: 0x001F8AE1
		public void OnAnimatorIKEvent(int layer)
		{
			if (this._animatorProxy != null)
			{
				this.DoSetBody();
			}
		}

		// Token: 0x06006BBB RID: 27579 RVA: 0x001FA8FA File Offset: 0x001F8AFA
		public override void OnUpdate()
		{
			if (this._animatorProxy == null)
			{
				this.DoSetBody();
			}
		}

		// Token: 0x06006BBC RID: 27580 RVA: 0x001FA914 File Offset: 0x001F8B14
		private void DoSetBody()
		{
			if (this._animator == null)
			{
				return;
			}
			if (this._transform != null)
			{
				if (this.position.IsNone)
				{
					this._animator.bodyPosition = this._transform.position;
				}
				else
				{
					this._animator.bodyPosition = this._transform.position + this.position.Value;
				}
				if (this.rotation.IsNone)
				{
					this._animator.bodyRotation = this._transform.rotation;
				}
				else
				{
					this._animator.bodyRotation = this._transform.rotation * this.rotation.Value;
				}
			}
			else
			{
				if (!this.position.IsNone)
				{
					this._animator.bodyPosition = this.position.Value;
				}
				if (!this.rotation.IsNone)
				{
					this._animator.bodyRotation = this.rotation.Value;
				}
			}
		}

		// Token: 0x06006BBD RID: 27581 RVA: 0x001FAA38 File Offset: 0x001F8C38
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorIKEvent -= new Action<int>(this.OnAnimatorIKEvent);
			}
		}

		// Token: 0x04005436 RID: 21558
		[CheckForComponent(typeof(Animator))]
		[Tooltip("The target. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005437 RID: 21559
		[Tooltip("The gameObject target of the ik goal")]
		public FsmGameObject target;

		// Token: 0x04005438 RID: 21560
		[Tooltip("The position of the ik goal. If Goal GameObject set, position is used as an offset from Goal")]
		public FsmVector3 position;

		// Token: 0x04005439 RID: 21561
		[Tooltip("The rotation of the ik goal.If Goal GameObject set, rotation is used as an offset from Goal")]
		public FsmQuaternion rotation;

		// Token: 0x0400543A RID: 21562
		[Tooltip("Repeat every frame. Useful when changing over time.")]
		public bool everyFrame;

		// Token: 0x0400543B RID: 21563
		private PlayMakerAnimatorIKProxy _animatorProxy;

		// Token: 0x0400543C RID: 21564
		private Animator _animator;

		// Token: 0x0400543D RID: 21565
		private Transform _transform;
	}
}
