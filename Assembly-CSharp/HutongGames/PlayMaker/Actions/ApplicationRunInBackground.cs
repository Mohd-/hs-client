using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B56 RID: 2902
	[ActionCategory(41)]
	[Tooltip("Sets if the Application should play in the background. Useful for servers or testing network games on one machine.")]
	public class ApplicationRunInBackground : FsmStateAction
	{
		// Token: 0x060062C2 RID: 25282 RVA: 0x001D8790 File Offset: 0x001D6990
		public override void Reset()
		{
			this.runInBackground = true;
		}

		// Token: 0x060062C3 RID: 25283 RVA: 0x001D879E File Offset: 0x001D699E
		public override void OnEnter()
		{
			Application.runInBackground = this.runInBackground.Value;
			base.Finish();
		}

		// Token: 0x04004A36 RID: 18998
		public FsmBool runInBackground;
	}
}
