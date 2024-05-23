using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B9E RID: 2974
	[Tooltip("Enables/Disables Fog in the scene.")]
	[ActionCategory(23)]
	public class EnableFog : FsmStateAction
	{
		// Token: 0x060063E4 RID: 25572 RVA: 0x001DC4F8 File Offset: 0x001DA6F8
		public override void Reset()
		{
			this.enableFog = true;
			this.everyFrame = false;
		}

		// Token: 0x060063E5 RID: 25573 RVA: 0x001DC50D File Offset: 0x001DA70D
		public override void OnEnter()
		{
			RenderSettings.fog = this.enableFog.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060063E6 RID: 25574 RVA: 0x001DC530 File Offset: 0x001DA730
		public override void OnUpdate()
		{
			RenderSettings.fog = this.enableFog.Value;
		}

		// Token: 0x04004B59 RID: 19289
		[Tooltip("Set to True to enable, False to disable.")]
		public FsmBool enableFog;

		// Token: 0x04004B5A RID: 19290
		[Tooltip("Repeat every frame. Useful if the Enable Fog setting is changing.")]
		public bool everyFrame;
	}
}
