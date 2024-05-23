using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D7F RID: 3455
	[ActionCategory("Animator")]
	[Tooltip("Follow a target")]
	[HelpUrl("https://hutonggames.fogbugz.com/default.asp?W1033")]
	public class AnimatorFollow : FsmStateAction
	{
		// Token: 0x06006C14 RID: 27668 RVA: 0x001FBE8C File Offset: 0x001FA08C
		public override void Reset()
		{
			this.gameObject = null;
			this.target = null;
			this.speedDampTime = 0.25f;
			this.directionDampTime = 0.25f;
			this.minimumDistance = 1f;
		}

		// Token: 0x06006C15 RID: 27669 RVA: 0x001FBED8 File Offset: 0x001FA0D8
		public override void OnEnter()
		{
			this._go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this._go == null)
			{
				base.Finish();
				return;
			}
			this._animatorProxy = this._go.GetComponent<PlayMakerAnimatorMoveProxy>();
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent += new Action(this.OnAnimatorMoveEvent);
			}
			this.avatar = this._go.GetComponent<Animator>();
			this.controller = this._go.GetComponent<CharacterController>();
			this.avatar.speed = 1f + Random.Range(-0.4f, 0.4f);
		}

		// Token: 0x06006C16 RID: 27670 RVA: 0x001FBF90 File Offset: 0x001FA190
		public override void OnUpdate()
		{
			GameObject value = this.target.Value;
			float value2 = this.speedDampTime.Value;
			float value3 = this.directionDampTime.Value;
			float value4 = this.minimumDistance.Value;
			if (this.avatar && value)
			{
				if (Vector3.Distance(value.transform.position, this.avatar.rootPosition) > value4)
				{
					this.avatar.SetFloat("Speed", 1f, value2, Time.deltaTime);
					Vector3 vector = this.avatar.rootRotation * Vector3.forward;
					Vector3 normalized = (value.transform.position - this.avatar.rootPosition).normalized;
					if (Vector3.Dot(vector, normalized) > 0f)
					{
						this.avatar.SetFloat("Direction", Vector3.Cross(vector, normalized).y, value3, Time.deltaTime);
					}
					else
					{
						this.avatar.SetFloat("Direction", (float)((Vector3.Cross(vector, normalized).y <= 0f) ? -1 : 1), value3, Time.deltaTime);
					}
				}
				else
				{
					this.avatar.SetFloat("Speed", 0f, value2, Time.deltaTime);
				}
				if (this._animatorProxy == null)
				{
					this.OnAnimatorMoveEvent();
				}
			}
		}

		// Token: 0x06006C17 RID: 27671 RVA: 0x001FC114 File Offset: 0x001FA314
		public override void OnExit()
		{
			if (this._animatorProxy != null)
			{
				this._animatorProxy.OnAnimatorMoveEvent -= new Action(this.OnAnimatorMoveEvent);
			}
		}

		// Token: 0x06006C18 RID: 27672 RVA: 0x001FC14C File Offset: 0x001FA34C
		public void OnAnimatorMoveEvent()
		{
			this.controller.Move(this.avatar.deltaPosition);
			this._go.transform.rotation = this.avatar.rootRotation;
		}

		// Token: 0x04005494 RID: 21652
		[Tooltip("The GameObject. An Animator component and a PlayMakerAnimatorProxy component are required")]
		[RequiredField]
		[CheckForComponent(typeof(Animator))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005495 RID: 21653
		[Tooltip("The Game Object to target.")]
		[RequiredField]
		public FsmGameObject target;

		// Token: 0x04005496 RID: 21654
		[Tooltip("The minimum distance to follow.")]
		public FsmFloat minimumDistance;

		// Token: 0x04005497 RID: 21655
		[Tooltip("The damping for following up.")]
		public FsmFloat speedDampTime;

		// Token: 0x04005498 RID: 21656
		[Tooltip("The damping for turning.")]
		public FsmFloat directionDampTime;

		// Token: 0x04005499 RID: 21657
		private GameObject _go;

		// Token: 0x0400549A RID: 21658
		private PlayMakerAnimatorMoveProxy _animatorProxy;

		// Token: 0x0400549B RID: 21659
		private Animator avatar;

		// Token: 0x0400549C RID: 21660
		private CharacterController controller;
	}
}
