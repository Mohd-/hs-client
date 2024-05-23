using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B62 RID: 2914
	[Tooltip("Tests if all the Bool Variables are False.\nSend an event or store the result.")]
	[ActionCategory(27)]
	public class BoolNoneTrue : FsmStateAction
	{
		// Token: 0x060062F0 RID: 25328 RVA: 0x001D91EE File Offset: 0x001D73EE
		public override void Reset()
		{
			this.boolVariables = null;
			this.sendEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060062F1 RID: 25329 RVA: 0x001D920C File Offset: 0x001D740C
		public override void OnEnter()
		{
			this.DoNoneTrue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060062F2 RID: 25330 RVA: 0x001D9225 File Offset: 0x001D7425
		public override void OnUpdate()
		{
			this.DoNoneTrue();
		}

		// Token: 0x060062F3 RID: 25331 RVA: 0x001D9230 File Offset: 0x001D7430
		private void DoNoneTrue()
		{
			if (this.boolVariables.Length == 0)
			{
				return;
			}
			bool flag = true;
			for (int i = 0; i < this.boolVariables.Length; i++)
			{
				if (this.boolVariables[i].Value)
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

		// Token: 0x04004A64 RID: 19044
		[Tooltip("The Bool variables to check.")]
		[RequiredField]
		[UIHint(10)]
		public FsmBool[] boolVariables;

		// Token: 0x04004A65 RID: 19045
		[Tooltip("Event to send if none of the Bool variables are True.")]
		public FsmEvent sendEvent;

		// Token: 0x04004A66 RID: 19046
		[UIHint(10)]
		[Tooltip("Store the result in a Bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04004A67 RID: 19047
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
