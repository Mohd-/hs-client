using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B63 RID: 2915
	[ActionCategory(7)]
	[Tooltip("Performs boolean operations on 2 Bool Variables.")]
	public class BoolOperator : FsmStateAction
	{
		// Token: 0x060062F5 RID: 25333 RVA: 0x001D92AA File Offset: 0x001D74AA
		public override void Reset()
		{
			this.bool1 = false;
			this.bool2 = false;
			this.operation = BoolOperator.Operation.AND;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060062F6 RID: 25334 RVA: 0x001D92D9 File Offset: 0x001D74D9
		public override void OnEnter()
		{
			this.DoBoolOperator();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060062F7 RID: 25335 RVA: 0x001D92F2 File Offset: 0x001D74F2
		public override void OnUpdate()
		{
			this.DoBoolOperator();
		}

		// Token: 0x060062F8 RID: 25336 RVA: 0x001D92FC File Offset: 0x001D74FC
		private void DoBoolOperator()
		{
			bool value = this.bool1.Value;
			bool value2 = this.bool2.Value;
			switch (this.operation)
			{
			case BoolOperator.Operation.AND:
				this.storeResult.Value = (value && value2);
				break;
			case BoolOperator.Operation.NAND:
				this.storeResult.Value = (!value || !value2);
				break;
			case BoolOperator.Operation.OR:
				this.storeResult.Value = (value || value2);
				break;
			case BoolOperator.Operation.XOR:
				this.storeResult.Value = (value ^ value2);
				break;
			}
		}

		// Token: 0x04004A68 RID: 19048
		[RequiredField]
		[Tooltip("The first Bool variable.")]
		public FsmBool bool1;

		// Token: 0x04004A69 RID: 19049
		[Tooltip("The second Bool variable.")]
		[RequiredField]
		public FsmBool bool2;

		// Token: 0x04004A6A RID: 19050
		[Tooltip("Boolean Operation.")]
		public BoolOperator.Operation operation;

		// Token: 0x04004A6B RID: 19051
		[Tooltip("Store the result in a Bool Variable.")]
		[RequiredField]
		[UIHint(10)]
		public FsmBool storeResult;

		// Token: 0x04004A6C RID: 19052
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x02000B64 RID: 2916
		public enum Operation
		{
			// Token: 0x04004A6E RID: 19054
			AND,
			// Token: 0x04004A6F RID: 19055
			NAND,
			// Token: 0x04004A70 RID: 19056
			OR,
			// Token: 0x04004A71 RID: 19057
			XOR
		}
	}
}
