using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC4 RID: 3268
	[Tooltip("Sets the size of light halos.")]
	[ActionCategory(23)]
	public class SetHaloStrength : FsmStateAction
	{
		// Token: 0x060068AE RID: 26798 RVA: 0x001EBDE9 File Offset: 0x001E9FE9
		public override void Reset()
		{
			this.haloStrength = 0.5f;
			this.everyFrame = false;
		}

		// Token: 0x060068AF RID: 26799 RVA: 0x001EBE02 File Offset: 0x001EA002
		public override void OnEnter()
		{
			this.DoSetHaloStrength();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068B0 RID: 26800 RVA: 0x001EBE1B File Offset: 0x001EA01B
		public override void OnUpdate()
		{
			this.DoSetHaloStrength();
		}

		// Token: 0x060068B1 RID: 26801 RVA: 0x001EBE23 File Offset: 0x001EA023
		private void DoSetHaloStrength()
		{
			RenderSettings.haloStrength = this.haloStrength.Value;
		}

		// Token: 0x0400507C RID: 20604
		[RequiredField]
		public FsmFloat haloStrength;

		// Token: 0x0400507D RID: 20605
		public bool everyFrame;
	}
}
