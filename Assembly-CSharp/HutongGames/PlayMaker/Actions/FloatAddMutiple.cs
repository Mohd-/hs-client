using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA8 RID: 2984
	[Tooltip("Adds multipe float variables to float variable.")]
	[ActionCategory(7)]
	public class FloatAddMutiple : FsmStateAction
	{
		// Token: 0x0600640F RID: 25615 RVA: 0x001DCCBD File Offset: 0x001DAEBD
		public override void Reset()
		{
			this.floatVariables = null;
			this.addTo = null;
			this.everyFrame = false;
		}

		// Token: 0x06006410 RID: 25616 RVA: 0x001DCCD4 File Offset: 0x001DAED4
		public override void OnEnter()
		{
			this.DoFloatAdd();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006411 RID: 25617 RVA: 0x001DCCED File Offset: 0x001DAEED
		public override void OnUpdate()
		{
			this.DoFloatAdd();
		}

		// Token: 0x06006412 RID: 25618 RVA: 0x001DCCF8 File Offset: 0x001DAEF8
		private void DoFloatAdd()
		{
			for (int i = 0; i < this.floatVariables.Length; i++)
			{
				this.addTo.Value += this.floatVariables[i].Value;
			}
		}

		// Token: 0x04004B7F RID: 19327
		[UIHint(10)]
		[Tooltip("The float variables to add.")]
		public FsmFloat[] floatVariables;

		// Token: 0x04004B80 RID: 19328
		[UIHint(10)]
		[Tooltip("Add to this variable.")]
		[RequiredField]
		public FsmFloat addTo;

		// Token: 0x04004B81 RID: 19329
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
