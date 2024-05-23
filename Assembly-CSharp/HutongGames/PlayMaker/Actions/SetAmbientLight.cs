using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C94 RID: 3220
	[Tooltip("Sets the Ambient Light Color for the scene.")]
	[ActionCategory(23)]
	public class SetAmbientLight : FsmStateAction
	{
		// Token: 0x060067D6 RID: 26582 RVA: 0x001E984B File Offset: 0x001E7A4B
		public override void Reset()
		{
			this.ambientColor = Color.gray;
			this.everyFrame = false;
		}

		// Token: 0x060067D7 RID: 26583 RVA: 0x001E9864 File Offset: 0x001E7A64
		public override void OnEnter()
		{
			this.DoSetAmbientColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067D8 RID: 26584 RVA: 0x001E987D File Offset: 0x001E7A7D
		public override void OnUpdate()
		{
			this.DoSetAmbientColor();
		}

		// Token: 0x060067D9 RID: 26585 RVA: 0x001E9885 File Offset: 0x001E7A85
		private void DoSetAmbientColor()
		{
			RenderSettings.ambientLight = this.ambientColor.Value;
		}

		// Token: 0x04004FAF RID: 20399
		[RequiredField]
		public FsmColor ambientColor;

		// Token: 0x04004FB0 RID: 20400
		public bool everyFrame;
	}
}
