using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C84 RID: 3204
	[Tooltip("Scales time: 1 = normal, 0.5 = half speed, 2 = double speed.")]
	[ActionCategory(31)]
	public class ScaleTime : FsmStateAction
	{
		// Token: 0x06006797 RID: 26519 RVA: 0x001E877E File Offset: 0x001E697E
		public override void Reset()
		{
			this.timeScale = 1f;
			this.adjustFixedDeltaTime = true;
			this.everyFrame = false;
		}

		// Token: 0x06006798 RID: 26520 RVA: 0x001E87A3 File Offset: 0x001E69A3
		public override void OnEnter()
		{
			this.DoTimeScale();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006799 RID: 26521 RVA: 0x001E87BC File Offset: 0x001E69BC
		public override void OnUpdate()
		{
			this.DoTimeScale();
		}

		// Token: 0x0600679A RID: 26522 RVA: 0x001E87C4 File Offset: 0x001E69C4
		private void DoTimeScale()
		{
			Time.timeScale = this.timeScale.Value;
			Time.fixedDeltaTime = 0.02f * Time.timeScale;
		}

		// Token: 0x04004F62 RID: 20322
		[RequiredField]
		[HasFloatSlider(0f, 4f)]
		[Tooltip("Scales time: 1 = normal, 0.5 = half speed, 2 = double speed.")]
		public FsmFloat timeScale;

		// Token: 0x04004F63 RID: 20323
		[Tooltip("Adjust the fixed physics time step to match the time scale.")]
		public FsmBool adjustFixedDeltaTime;

		// Token: 0x04004F64 RID: 20324
		[Tooltip("Repeat every frame. Useful when animating the value.")]
		public bool everyFrame;
	}
}
