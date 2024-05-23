using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BAF RID: 2991
	[Tooltip("Performs math operations on 2 Floats: Add, Subtract, Multiply, Divide, Min, Max.")]
	[ActionCategory(7)]
	public class FloatOperator : FsmStateAction
	{
		// Token: 0x0600642F RID: 25647 RVA: 0x001DD248 File Offset: 0x001DB448
		public override void Reset()
		{
			this.float1 = null;
			this.float2 = null;
			this.operation = FloatOperator.Operation.Add;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006430 RID: 25648 RVA: 0x001DD26D File Offset: 0x001DB46D
		public override void OnEnter()
		{
			this.DoFloatOperator();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006431 RID: 25649 RVA: 0x001DD286 File Offset: 0x001DB486
		public override void OnUpdate()
		{
			this.DoFloatOperator();
		}

		// Token: 0x06006432 RID: 25650 RVA: 0x001DD290 File Offset: 0x001DB490
		private void DoFloatOperator()
		{
			float value = this.float1.Value;
			float value2 = this.float2.Value;
			switch (this.operation)
			{
			case FloatOperator.Operation.Add:
				this.storeResult.Value = value + value2;
				break;
			case FloatOperator.Operation.Subtract:
				this.storeResult.Value = value - value2;
				break;
			case FloatOperator.Operation.Multiply:
				this.storeResult.Value = value * value2;
				break;
			case FloatOperator.Operation.Divide:
				this.storeResult.Value = value / value2;
				break;
			case FloatOperator.Operation.Min:
				this.storeResult.Value = Mathf.Min(value, value2);
				break;
			case FloatOperator.Operation.Max:
				this.storeResult.Value = Mathf.Max(value, value2);
				break;
			}
		}

		// Token: 0x04004BA0 RID: 19360
		[RequiredField]
		[Tooltip("The first float.")]
		public FsmFloat float1;

		// Token: 0x04004BA1 RID: 19361
		[RequiredField]
		[Tooltip("The second float.")]
		public FsmFloat float2;

		// Token: 0x04004BA2 RID: 19362
		[Tooltip("The math operation to perform on the floats.")]
		public FloatOperator.Operation operation;

		// Token: 0x04004BA3 RID: 19363
		[Tooltip("Store the result of the operation in a float variable.")]
		[UIHint(10)]
		[RequiredField]
		public FsmFloat storeResult;

		// Token: 0x04004BA4 RID: 19364
		[Tooltip("Repeat every frame. Useful if the variables are changing.")]
		public bool everyFrame;

		// Token: 0x02000BB0 RID: 2992
		public enum Operation
		{
			// Token: 0x04004BA6 RID: 19366
			Add,
			// Token: 0x04004BA7 RID: 19367
			Subtract,
			// Token: 0x04004BA8 RID: 19368
			Multiply,
			// Token: 0x04004BA9 RID: 19369
			Divide,
			// Token: 0x04004BAA RID: 19370
			Min,
			// Token: 0x04004BAB RID: 19371
			Max
		}
	}
}
