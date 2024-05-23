using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B5F RID: 2911
	[ActionCategory(27)]
	[Tooltip("Tests if any of the given Bool Variables are True.")]
	public class BoolAnyTrue : FsmStateAction
	{
		// Token: 0x060062E4 RID: 25316 RVA: 0x001D906A File Offset: 0x001D726A
		public override void Reset()
		{
			this.boolVariables = null;
			this.sendEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060062E5 RID: 25317 RVA: 0x001D9088 File Offset: 0x001D7288
		public override void OnEnter()
		{
			this.DoAnyTrue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060062E6 RID: 25318 RVA: 0x001D90A1 File Offset: 0x001D72A1
		public override void OnUpdate()
		{
			this.DoAnyTrue();
		}

		// Token: 0x060062E7 RID: 25319 RVA: 0x001D90AC File Offset: 0x001D72AC
		private void DoAnyTrue()
		{
			if (this.boolVariables.Length == 0)
			{
				return;
			}
			this.storeResult.Value = false;
			for (int i = 0; i < this.boolVariables.Length; i++)
			{
				if (this.boolVariables[i].Value)
				{
					base.Fsm.Event(this.sendEvent);
					this.storeResult.Value = true;
					return;
				}
			}
		}

		// Token: 0x04004A5B RID: 19035
		[UIHint(10)]
		[Tooltip("The Bool variables to check.")]
		[RequiredField]
		public FsmBool[] boolVariables;

		// Token: 0x04004A5C RID: 19036
		[Tooltip("Event to send if any of the Bool variables are True.")]
		public FsmEvent sendEvent;

		// Token: 0x04004A5D RID: 19037
		[Tooltip("Store the result in a Bool variable.")]
		[UIHint(10)]
		public FsmBool storeResult;

		// Token: 0x04004A5E RID: 19038
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
