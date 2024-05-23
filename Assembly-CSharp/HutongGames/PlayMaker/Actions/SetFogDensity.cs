using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CA8 RID: 3240
	[ActionCategory(23)]
	[Tooltip("Sets the density of the Fog in the scene.")]
	public class SetFogDensity : FsmStateAction
	{
		// Token: 0x06006830 RID: 26672 RVA: 0x001EA65F File Offset: 0x001E885F
		public override void Reset()
		{
			this.fogDensity = 0.5f;
			this.everyFrame = false;
		}

		// Token: 0x06006831 RID: 26673 RVA: 0x001EA678 File Offset: 0x001E8878
		public override void OnEnter()
		{
			this.DoSetFogDensity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006832 RID: 26674 RVA: 0x001EA691 File Offset: 0x001E8891
		public override void OnUpdate()
		{
			this.DoSetFogDensity();
		}

		// Token: 0x06006833 RID: 26675 RVA: 0x001EA699 File Offset: 0x001E8899
		private void DoSetFogDensity()
		{
			RenderSettings.fogDensity = this.fogDensity.Value;
		}

		// Token: 0x04004FF6 RID: 20470
		[RequiredField]
		public FsmFloat fogDensity;

		// Token: 0x04004FF7 RID: 20471
		public bool everyFrame;
	}
}
