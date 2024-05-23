using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA7 RID: 3239
	[Tooltip("Sets the color of the Fog in the scene.")]
	[ActionCategory(23)]
	public class SetFogColor : FsmStateAction
	{
		// Token: 0x0600682B RID: 26667 RVA: 0x001EA60B File Offset: 0x001E880B
		public override void Reset()
		{
			this.fogColor = Color.white;
			this.everyFrame = false;
		}

		// Token: 0x0600682C RID: 26668 RVA: 0x001EA624 File Offset: 0x001E8824
		public override void OnEnter()
		{
			this.DoSetFogColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600682D RID: 26669 RVA: 0x001EA63D File Offset: 0x001E883D
		public override void OnUpdate()
		{
			this.DoSetFogColor();
		}

		// Token: 0x0600682E RID: 26670 RVA: 0x001EA645 File Offset: 0x001E8845
		private void DoSetFogColor()
		{
			RenderSettings.fogColor = this.fogColor.Value;
		}

		// Token: 0x04004FF4 RID: 20468
		[RequiredField]
		public FsmColor fogColor;

		// Token: 0x04004FF5 RID: 20469
		public bool everyFrame;
	}
}
