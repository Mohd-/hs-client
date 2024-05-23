using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BEB RID: 3051
	[Tooltip("Gets the value of the specified Input Axis and stores it in a Float Variable. See Unity Input Manager docs.")]
	[ActionCategory(6)]
	public class GetAxis : FsmStateAction
	{
		// Token: 0x06006501 RID: 25857 RVA: 0x001E0244 File Offset: 0x001DE444
		public override void Reset()
		{
			this.axisName = string.Empty;
			this.multiplier = 1f;
			this.store = null;
			this.everyFrame = true;
		}

		// Token: 0x06006502 RID: 25858 RVA: 0x001E027F File Offset: 0x001DE47F
		public override void OnEnter()
		{
			this.DoGetAxis();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006503 RID: 25859 RVA: 0x001E0298 File Offset: 0x001DE498
		public override void OnUpdate()
		{
			this.DoGetAxis();
		}

		// Token: 0x06006504 RID: 25860 RVA: 0x001E02A0 File Offset: 0x001DE4A0
		private void DoGetAxis()
		{
			float num = Input.GetAxis(this.axisName.Value);
			if (!this.multiplier.IsNone)
			{
				num *= this.multiplier.Value;
			}
			this.store.Value = num;
		}

		// Token: 0x04004C98 RID: 19608
		[Tooltip("The name of the axis. Set in the Unity Input Manager.")]
		[RequiredField]
		public FsmString axisName;

		// Token: 0x04004C99 RID: 19609
		[Tooltip("Axis values are in the range -1 to 1. Use the multiplier to set a larger range.")]
		public FsmFloat multiplier;

		// Token: 0x04004C9A RID: 19610
		[UIHint(10)]
		[RequiredField]
		[Tooltip("Store the result in a float variable.")]
		public FsmFloat store;

		// Token: 0x04004C9B RID: 19611
		[Tooltip("Repeat every frame. Typically this would be set to True.")]
		public bool everyFrame;
	}
}
