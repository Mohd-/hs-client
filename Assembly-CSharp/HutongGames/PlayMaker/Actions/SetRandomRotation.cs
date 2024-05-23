using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CDF RID: 3295
	[ActionCategory(14)]
	[Tooltip("Sets Random Rotation for a Game Object. Uncheck an axis to keep its current value.")]
	public class SetRandomRotation : FsmStateAction
	{
		// Token: 0x06006920 RID: 26912 RVA: 0x001ED14E File Offset: 0x001EB34E
		public override void Reset()
		{
			this.gameObject = null;
			this.x = true;
			this.y = true;
			this.z = true;
		}

		// Token: 0x06006921 RID: 26913 RVA: 0x001ED17B File Offset: 0x001EB37B
		public override void OnEnter()
		{
			this.DoRandomRotation();
			base.Finish();
		}

		// Token: 0x06006922 RID: 26914 RVA: 0x001ED18C File Offset: 0x001EB38C
		private void DoRandomRotation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 localEulerAngles = ownerDefaultTarget.transform.localEulerAngles;
			float num = localEulerAngles.x;
			float num2 = localEulerAngles.y;
			float num3 = localEulerAngles.z;
			if (this.x.Value)
			{
				num = (float)Random.Range(0, 360);
			}
			if (this.y.Value)
			{
				num2 = (float)Random.Range(0, 360);
			}
			if (this.z.Value)
			{
				num3 = (float)Random.Range(0, 360);
			}
			ownerDefaultTarget.transform.localEulerAngles = new Vector3(num, num2, num3);
		}

		// Token: 0x040050D1 RID: 20689
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040050D2 RID: 20690
		[RequiredField]
		public FsmBool x;

		// Token: 0x040050D3 RID: 20691
		[RequiredField]
		public FsmBool y;

		// Token: 0x040050D4 RID: 20692
		[RequiredField]
		public FsmBool z;
	}
}
