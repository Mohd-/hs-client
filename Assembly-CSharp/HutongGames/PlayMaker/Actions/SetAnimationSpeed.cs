using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C95 RID: 3221
	[ActionCategory(0)]
	[Tooltip("Sets the Speed of an Animation. Check Every Frame to update the animation time continuosly, e.g., if you're manipulating a variable that controls animation speed.")]
	public class SetAnimationSpeed : ComponentAction<Animation>
	{
		// Token: 0x060067DB RID: 26587 RVA: 0x001E98AF File Offset: 0x001E7AAF
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.speed = 1f;
			this.everyFrame = false;
		}

		// Token: 0x060067DC RID: 26588 RVA: 0x001E98D8 File Offset: 0x001E7AD8
		public override void OnEnter()
		{
			this.DoSetAnimationSpeed((this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067DD RID: 26589 RVA: 0x001E9928 File Offset: 0x001E7B28
		public override void OnUpdate()
		{
			this.DoSetAnimationSpeed((this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner);
		}

		// Token: 0x060067DE RID: 26590 RVA: 0x001E9968 File Offset: 0x001E7B68
		private void DoSetAnimationSpeed(GameObject go)
		{
			if (!base.UpdateCache(go))
			{
				return;
			}
			AnimationState animationState = base.animation[this.animName.Value];
			if (animationState == null)
			{
				this.LogWarning("Missing animation: " + this.animName.Value);
				return;
			}
			animationState.speed = this.speed.Value;
		}

		// Token: 0x04004FB1 RID: 20401
		[CheckForComponent(typeof(Animation))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004FB2 RID: 20402
		[UIHint(6)]
		[RequiredField]
		public FsmString animName;

		// Token: 0x04004FB3 RID: 20403
		public FsmFloat speed = 1f;

		// Token: 0x04004FB4 RID: 20404
		public bool everyFrame;
	}
}
