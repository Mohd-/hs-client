using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D16 RID: 3350
	[Tooltip("Performs most possible operations on 2 Vector3: Dot product, Cross product, Distance, Angle, Project, Reflect, Add, Subtract, Multiply, Divide, Min, Max")]
	[ActionCategory(19)]
	public class Vector3Operator : FsmStateAction
	{
		// Token: 0x06006A13 RID: 27155 RVA: 0x001F0E1C File Offset: 0x001EF01C
		public override void Reset()
		{
			this.vector1 = null;
			this.vector2 = null;
			this.operation = Vector3Operator.Vector3Operation.Add;
			this.storeVector3Result = null;
			this.storeFloatResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006A14 RID: 27156 RVA: 0x001F0E53 File Offset: 0x001EF053
		public override void OnEnter()
		{
			this.DoVector3Operator();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A15 RID: 27157 RVA: 0x001F0E6C File Offset: 0x001EF06C
		public override void OnUpdate()
		{
			this.DoVector3Operator();
		}

		// Token: 0x06006A16 RID: 27158 RVA: 0x001F0E74 File Offset: 0x001EF074
		private void DoVector3Operator()
		{
			Vector3 value = this.vector1.Value;
			Vector3 value2 = this.vector2.Value;
			switch (this.operation)
			{
			case Vector3Operator.Vector3Operation.DotProduct:
				this.storeFloatResult.Value = Vector3.Dot(value, value2);
				break;
			case Vector3Operator.Vector3Operation.CrossProduct:
				this.storeVector3Result.Value = Vector3.Cross(value, value2);
				break;
			case Vector3Operator.Vector3Operation.Distance:
				this.storeFloatResult.Value = Vector3.Distance(value, value2);
				break;
			case Vector3Operator.Vector3Operation.Angle:
				this.storeFloatResult.Value = Vector3.Angle(value, value2);
				break;
			case Vector3Operator.Vector3Operation.Project:
				this.storeVector3Result.Value = Vector3.Project(value, value2);
				break;
			case Vector3Operator.Vector3Operation.Reflect:
				this.storeVector3Result.Value = Vector3.Reflect(value, value2);
				break;
			case Vector3Operator.Vector3Operation.Add:
				this.storeVector3Result.Value = value + value2;
				break;
			case Vector3Operator.Vector3Operation.Subtract:
				this.storeVector3Result.Value = value - value2;
				break;
			case Vector3Operator.Vector3Operation.Multiply:
			{
				Vector3 zero = Vector3.zero;
				zero.x = value.x * value2.x;
				zero.y = value.y * value2.y;
				zero.z = value.z * value2.z;
				this.storeVector3Result.Value = zero;
				break;
			}
			case Vector3Operator.Vector3Operation.Divide:
			{
				Vector3 zero2 = Vector3.zero;
				zero2.x = value.x / value2.x;
				zero2.y = value.y / value2.y;
				zero2.z = value.z / value2.z;
				this.storeVector3Result.Value = zero2;
				break;
			}
			case Vector3Operator.Vector3Operation.Min:
				this.storeVector3Result.Value = Vector3.Min(value, value2);
				break;
			case Vector3Operator.Vector3Operation.Max:
				this.storeVector3Result.Value = Vector3.Max(value, value2);
				break;
			}
		}

		// Token: 0x040051E0 RID: 20960
		[RequiredField]
		public FsmVector3 vector1;

		// Token: 0x040051E1 RID: 20961
		[RequiredField]
		public FsmVector3 vector2;

		// Token: 0x040051E2 RID: 20962
		public Vector3Operator.Vector3Operation operation = Vector3Operator.Vector3Operation.Add;

		// Token: 0x040051E3 RID: 20963
		[UIHint(10)]
		public FsmVector3 storeVector3Result;

		// Token: 0x040051E4 RID: 20964
		[UIHint(10)]
		public FsmFloat storeFloatResult;

		// Token: 0x040051E5 RID: 20965
		public bool everyFrame;

		// Token: 0x02000D17 RID: 3351
		public enum Vector3Operation
		{
			// Token: 0x040051E7 RID: 20967
			DotProduct,
			// Token: 0x040051E8 RID: 20968
			CrossProduct,
			// Token: 0x040051E9 RID: 20969
			Distance,
			// Token: 0x040051EA RID: 20970
			Angle,
			// Token: 0x040051EB RID: 20971
			Project,
			// Token: 0x040051EC RID: 20972
			Reflect,
			// Token: 0x040051ED RID: 20973
			Add,
			// Token: 0x040051EE RID: 20974
			Subtract,
			// Token: 0x040051EF RID: 20975
			Multiply,
			// Token: 0x040051F0 RID: 20976
			Divide,
			// Token: 0x040051F1 RID: 20977
			Min,
			// Token: 0x040051F2 RID: 20978
			Max
		}
	}
}
