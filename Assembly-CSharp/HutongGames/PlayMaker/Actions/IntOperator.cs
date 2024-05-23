using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C48 RID: 3144
	[ActionCategory(7)]
	[Tooltip("Performs math operation on 2 Integers: Add, Subtract, Multiply, Divide, Min, Max.")]
	public class IntOperator : FsmStateAction
	{
		// Token: 0x0600668D RID: 26253 RVA: 0x001E4AE8 File Offset: 0x001E2CE8
		public override void Reset()
		{
			this.integer1 = null;
			this.integer2 = null;
			this.operation = IntOperator.Operation.Add;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x0600668E RID: 26254 RVA: 0x001E4B0D File Offset: 0x001E2D0D
		public override void OnEnter()
		{
			this.DoIntOperator();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600668F RID: 26255 RVA: 0x001E4B26 File Offset: 0x001E2D26
		public override void OnUpdate()
		{
			this.DoIntOperator();
		}

		// Token: 0x06006690 RID: 26256 RVA: 0x001E4B30 File Offset: 0x001E2D30
		private void DoIntOperator()
		{
			int value = this.integer1.Value;
			int value2 = this.integer2.Value;
			switch (this.operation)
			{
			case IntOperator.Operation.Add:
				this.storeResult.Value = value + value2;
				break;
			case IntOperator.Operation.Subtract:
				this.storeResult.Value = value - value2;
				break;
			case IntOperator.Operation.Multiply:
				this.storeResult.Value = value * value2;
				break;
			case IntOperator.Operation.Divide:
				this.storeResult.Value = value / value2;
				break;
			case IntOperator.Operation.Min:
				this.storeResult.Value = Mathf.Min(value, value2);
				break;
			case IntOperator.Operation.Max:
				this.storeResult.Value = Mathf.Max(value, value2);
				break;
			}
		}

		// Token: 0x04004E3F RID: 20031
		[RequiredField]
		public FsmInt integer1;

		// Token: 0x04004E40 RID: 20032
		[RequiredField]
		public FsmInt integer2;

		// Token: 0x04004E41 RID: 20033
		public IntOperator.Operation operation;

		// Token: 0x04004E42 RID: 20034
		[UIHint(10)]
		[RequiredField]
		public FsmInt storeResult;

		// Token: 0x04004E43 RID: 20035
		public bool everyFrame;

		// Token: 0x02000C49 RID: 3145
		public enum Operation
		{
			// Token: 0x04004E45 RID: 20037
			Add,
			// Token: 0x04004E46 RID: 20038
			Subtract,
			// Token: 0x04004E47 RID: 20039
			Multiply,
			// Token: 0x04004E48 RID: 20040
			Divide,
			// Token: 0x04004E49 RID: 20041
			Min,
			// Token: 0x04004E4A RID: 20042
			Max
		}
	}
}
