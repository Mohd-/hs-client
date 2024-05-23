using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C97 RID: 3223
	[ActionCategory(0)]
	[Tooltip("Sets the Blend Weight of an Animation. Check Every Frame to update the weight continuosly, e.g., if you're manipulating a variable that controls the weight.")]
	public class SetAnimationWeight : ComponentAction<Animation>
	{
		// Token: 0x060067E5 RID: 26597 RVA: 0x001E9B60 File Offset: 0x001E7D60
		public override void Reset()
		{
			this.gameObject = null;
			this.animName = null;
			this.weight = 1f;
			this.everyFrame = false;
		}

		// Token: 0x060067E6 RID: 26598 RVA: 0x001E9B88 File Offset: 0x001E7D88
		public override void OnEnter()
		{
			this.DoSetAnimationWeight((this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067E7 RID: 26599 RVA: 0x001E9BD8 File Offset: 0x001E7DD8
		public override void OnUpdate()
		{
			this.DoSetAnimationWeight((this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner);
		}

		// Token: 0x060067E8 RID: 26600 RVA: 0x001E9C18 File Offset: 0x001E7E18
		private void DoSetAnimationWeight(GameObject go)
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
			animationState.weight = this.weight.Value;
		}

		// Token: 0x04004FBA RID: 20410
		[CheckForComponent(typeof(Animation))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004FBB RID: 20411
		[RequiredField]
		[UIHint(6)]
		public FsmString animName;

		// Token: 0x04004FBC RID: 20412
		public FsmFloat weight = 1f;

		// Token: 0x04004FBD RID: 20413
		public bool everyFrame;
	}
}
