using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA5 RID: 3237
	[ActionCategory(23)]
	[Tooltip("Sets the intensity of all Flares in the scene.")]
	public class SetFlareStrength : FsmStateAction
	{
		// Token: 0x06006822 RID: 26658 RVA: 0x001EA557 File Offset: 0x001E8757
		public override void Reset()
		{
			this.flareStrength = 0.2f;
			this.everyFrame = false;
		}

		// Token: 0x06006823 RID: 26659 RVA: 0x001EA570 File Offset: 0x001E8770
		public override void OnEnter()
		{
			this.DoSetFlareStrength();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006824 RID: 26660 RVA: 0x001EA589 File Offset: 0x001E8789
		public override void OnUpdate()
		{
			this.DoSetFlareStrength();
		}

		// Token: 0x06006825 RID: 26661 RVA: 0x001EA591 File Offset: 0x001E8791
		private void DoSetFlareStrength()
		{
			RenderSettings.flareStrength = this.flareStrength.Value;
		}

		// Token: 0x04004FEF RID: 20463
		[RequiredField]
		public FsmFloat flareStrength;

		// Token: 0x04004FF0 RID: 20464
		public bool everyFrame;
	}
}
