using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C5F RID: 3167
	[ActionCategory(31)]
	[Tooltip("Multiplies a Float by Time.deltaTime to use in frame-rate independent operations. E.g., 10 becomes 10 units per second.")]
	public class PerSecond : FsmStateAction
	{
		// Token: 0x060066F3 RID: 26355 RVA: 0x001E6296 File Offset: 0x001E4496
		public override void Reset()
		{
			this.floatValue = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060066F4 RID: 26356 RVA: 0x001E62AD File Offset: 0x001E44AD
		public override void OnEnter()
		{
			this.DoPerSecond();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066F5 RID: 26357 RVA: 0x001E62C6 File Offset: 0x001E44C6
		public override void OnUpdate()
		{
			this.DoPerSecond();
		}

		// Token: 0x060066F6 RID: 26358 RVA: 0x001E62D0 File Offset: 0x001E44D0
		private void DoPerSecond()
		{
			if (this.storeResult == null)
			{
				return;
			}
			this.storeResult.Value = this.floatValue.Value * Time.deltaTime;
		}

		// Token: 0x04004EC4 RID: 20164
		[RequiredField]
		public FsmFloat floatValue;

		// Token: 0x04004EC5 RID: 20165
		[RequiredField]
		[UIHint(10)]
		public FsmFloat storeResult;

		// Token: 0x04004EC6 RID: 20166
		public bool everyFrame;
	}
}
