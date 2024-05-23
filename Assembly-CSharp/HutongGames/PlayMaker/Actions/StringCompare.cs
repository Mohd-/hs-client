using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF9 RID: 3321
	[Tooltip("Compares 2 Strings and sends Events based on the result.")]
	[ActionCategory(27)]
	public class StringCompare : FsmStateAction
	{
		// Token: 0x06006997 RID: 27031 RVA: 0x001EEF85 File Offset: 0x001ED185
		public override void Reset()
		{
			this.stringVariable = null;
			this.compareTo = string.Empty;
			this.equalEvent = null;
			this.notEqualEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006998 RID: 27032 RVA: 0x001EEFBA File Offset: 0x001ED1BA
		public override void OnEnter()
		{
			this.DoStringCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006999 RID: 27033 RVA: 0x001EEFD3 File Offset: 0x001ED1D3
		public override void OnUpdate()
		{
			this.DoStringCompare();
		}

		// Token: 0x0600699A RID: 27034 RVA: 0x001EEFDC File Offset: 0x001ED1DC
		private void DoStringCompare()
		{
			if (this.stringVariable == null || this.compareTo == null)
			{
				return;
			}
			bool flag = this.stringVariable.Value == this.compareTo.Value;
			if (this.storeResult != null)
			{
				this.storeResult.Value = flag;
			}
			if (flag && this.equalEvent != null)
			{
				base.Fsm.Event(this.equalEvent);
				return;
			}
			if (!flag && this.notEqualEvent != null)
			{
				base.Fsm.Event(this.notEqualEvent);
			}
		}

		// Token: 0x0400514F RID: 20815
		[RequiredField]
		[UIHint(10)]
		public FsmString stringVariable;

		// Token: 0x04005150 RID: 20816
		public FsmString compareTo;

		// Token: 0x04005151 RID: 20817
		public FsmEvent equalEvent;

		// Token: 0x04005152 RID: 20818
		public FsmEvent notEqualEvent;

		// Token: 0x04005153 RID: 20819
		[UIHint(10)]
		[Tooltip("Store the true/false result in a bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04005154 RID: 20820
		[Tooltip("Repeat every frame. Useful if any of the strings are changing over time.")]
		public bool everyFrame;
	}
}
