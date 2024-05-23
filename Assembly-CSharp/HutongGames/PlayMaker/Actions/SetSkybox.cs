using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE5 RID: 3301
	[ActionCategory(23)]
	[Tooltip("Sets the global Skybox.")]
	public class SetSkybox : FsmStateAction
	{
		// Token: 0x0600693E RID: 26942 RVA: 0x001ED849 File Offset: 0x001EBA49
		public override void Reset()
		{
			this.skybox = null;
		}

		// Token: 0x0600693F RID: 26943 RVA: 0x001ED852 File Offset: 0x001EBA52
		public override void OnEnter()
		{
			RenderSettings.skybox = this.skybox.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006940 RID: 26944 RVA: 0x001ED875 File Offset: 0x001EBA75
		public override void OnUpdate()
		{
			RenderSettings.skybox = this.skybox.Value;
		}

		// Token: 0x040050F1 RID: 20721
		public FsmMaterial skybox;

		// Token: 0x040050F2 RID: 20722
		[Tooltip("Repeat every frame. Useful if the Skybox is changing.")]
		public bool everyFrame;
	}
}
