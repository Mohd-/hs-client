using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DC2 RID: 3522
	[ActionCategory("Pegasus")]
	[Tooltip("Is Editor Running sends Events based on the result.")]
	public class IsEditorRunningAction : FsmStateAction
	{
		// Token: 0x06006D1E RID: 27934 RVA: 0x002016A9 File Offset: 0x001FF8A9
		public override void Reset()
		{
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
		}

		// Token: 0x06006D1F RID: 27935 RVA: 0x002016C0 File Offset: 0x001FF8C0
		public override void OnEnter()
		{
			this.IsEditorRunning();
			base.Finish();
		}

		// Token: 0x06006D20 RID: 27936 RVA: 0x002016CE File Offset: 0x001FF8CE
		public override void OnUpdate()
		{
			this.IsEditorRunning();
		}

		// Token: 0x06006D21 RID: 27937 RVA: 0x002016D8 File Offset: 0x001FF8D8
		private void IsEditorRunning()
		{
			this.storeResult.Value = GeneralUtils.IsEditorPlaying();
			if (this.storeResult.Value)
			{
				base.Fsm.Event(this.trueEvent);
			}
			else
			{
				base.Fsm.Event(this.falseEvent);
			}
		}

		// Token: 0x040055C8 RID: 21960
		[RequiredField]
		[Tooltip("Event to use if Editor is running.")]
		public FsmEvent trueEvent;

		// Token: 0x040055C9 RID: 21961
		[Tooltip("Event to use if Editor is NOT running.")]
		public FsmEvent falseEvent;

		// Token: 0x040055CA RID: 21962
		[Tooltip("Store the true/false result in a bool variable.")]
		[UIHint(10)]
		public FsmBool storeResult;
	}
}
