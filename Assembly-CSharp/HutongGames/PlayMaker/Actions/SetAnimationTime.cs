using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C96 RID: 3222
	[ActionCategory(0)]
	[Tooltip("Sets the current Time of an Animation, Normalize time means 0 (start) to 1 (end); useful if you don't care about the exact time. Check Every Frame to update the time continuosly.")]
	public class SetAnimationTime : ComponentAction<Animation>
	{
		// Token: 0x060067E0 RID: 26592 RVA: 0x001E99DA File Offset: 0x001E7BDA
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.time = null;
			this.normalized = false;
			this.everyFrame = false;
		}

		// Token: 0x060067E1 RID: 26593 RVA: 0x001E9A00 File Offset: 0x001E7C00
		public override void OnEnter()
		{
			this.DoSetAnimationTime((this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067E2 RID: 26594 RVA: 0x001E9A50 File Offset: 0x001E7C50
		public override void OnUpdate()
		{
			this.DoSetAnimationTime((this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner);
		}

		// Token: 0x060067E3 RID: 26595 RVA: 0x001E9A90 File Offset: 0x001E7C90
		private void DoSetAnimationTime(GameObject go)
		{
			if (!base.UpdateCache(go))
			{
				return;
			}
			base.animation.Play(this.animName.Value);
			AnimationState animationState = base.animation[this.animName.Value];
			if (animationState == null)
			{
				this.LogWarning("Missing animation: " + this.animName.Value);
				return;
			}
			if (this.normalized)
			{
				animationState.normalizedTime = this.time.Value;
			}
			else
			{
				animationState.time = this.time.Value;
			}
			if (this.everyFrame)
			{
				animationState.speed = 0f;
			}
		}

		// Token: 0x04004FB5 RID: 20405
		[CheckForComponent(typeof(Animation))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004FB6 RID: 20406
		[UIHint(6)]
		[RequiredField]
		public FsmString animName;

		// Token: 0x04004FB7 RID: 20407
		public FsmFloat time;

		// Token: 0x04004FB8 RID: 20408
		public bool normalized;

		// Token: 0x04004FB9 RID: 20409
		public bool everyFrame;
	}
}
