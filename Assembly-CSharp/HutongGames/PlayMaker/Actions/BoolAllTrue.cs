using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B5E RID: 2910
	[ActionCategory(27)]
	[Tooltip("Tests if all the given Bool Variables are True.")]
	public class BoolAllTrue : FsmStateAction
	{
		// Token: 0x060062DF RID: 25311 RVA: 0x001D8FAE File Offset: 0x001D71AE
		public override void Reset()
		{
			this.boolVariables = null;
			this.sendEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060062E0 RID: 25312 RVA: 0x001D8FCC File Offset: 0x001D71CC
		public override void OnEnter()
		{
			this.DoAllTrue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060062E1 RID: 25313 RVA: 0x001D8FE5 File Offset: 0x001D71E5
		public override void OnUpdate()
		{
			this.DoAllTrue();
		}

		// Token: 0x060062E2 RID: 25314 RVA: 0x001D8FF0 File Offset: 0x001D71F0
		private void DoAllTrue()
		{
			if (this.boolVariables.Length == 0)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < this.boolVariables.Length; i++)
			{
				if (!this.boolVariables[i].Value)
				{
					flag = false;
					break;
				}
			}
			if (flag)
			{
				base.Fsm.Event(this.sendEvent);
			}
			this.storeResult.Value = flag;
		}

		// Token: 0x04004A57 RID: 19031
		[RequiredField]
		[UIHint(10)]
		[Tooltip("The Bool variables to check.")]
		public FsmBool[] boolVariables;

		// Token: 0x04004A58 RID: 19032
		[Tooltip("Event to send if all the Bool variables are True.")]
		public FsmEvent sendEvent;

		// Token: 0x04004A59 RID: 19033
		[UIHint(10)]
		[Tooltip("Store the result in a Bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04004A5A RID: 19034
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
