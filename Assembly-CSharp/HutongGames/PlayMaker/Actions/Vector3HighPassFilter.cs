using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D0F RID: 3343
	[ActionCategory(19)]
	[Tooltip("Use a high pass filter to isolate sudden changes in a Vector3 Variable. Useful when working with Get Device Acceleration to remove the constant effect of gravity.")]
	public class Vector3HighPassFilter : FsmStateAction
	{
		// Token: 0x060069F6 RID: 27126 RVA: 0x001F06E9 File Offset: 0x001EE8E9
		public override void Reset()
		{
			this.vector3Variable = null;
			this.filteringFactor = 0.1f;
		}

		// Token: 0x060069F7 RID: 27127 RVA: 0x001F0704 File Offset: 0x001EE904
		public override void OnEnter()
		{
			this.filteredVector = new Vector3(this.vector3Variable.Value.x, this.vector3Variable.Value.y, this.vector3Variable.Value.z);
		}

		// Token: 0x060069F8 RID: 27128 RVA: 0x001F0758 File Offset: 0x001EE958
		public override void OnUpdate()
		{
			this.filteredVector.x = this.vector3Variable.Value.x - (this.vector3Variable.Value.x * this.filteringFactor.Value + this.filteredVector.x * (1f - this.filteringFactor.Value));
			this.filteredVector.y = this.vector3Variable.Value.y - (this.vector3Variable.Value.y * this.filteringFactor.Value + this.filteredVector.y * (1f - this.filteringFactor.Value));
			this.filteredVector.z = this.vector3Variable.Value.z - (this.vector3Variable.Value.z * this.filteringFactor.Value + this.filteredVector.z * (1f - this.filteringFactor.Value));
			this.vector3Variable.Value = new Vector3(this.filteredVector.x, this.filteredVector.y, this.filteredVector.z);
		}

		// Token: 0x040051C5 RID: 20933
		[UIHint(10)]
		[RequiredField]
		[Tooltip("Vector3 Variable to filter. Should generally come from some constantly updated input, e.g., acceleration.")]
		public FsmVector3 vector3Variable;

		// Token: 0x040051C6 RID: 20934
		[Tooltip("Determines how much influence new changes have.")]
		public FsmFloat filteringFactor;

		// Token: 0x040051C7 RID: 20935
		private Vector3 filteredVector;
	}
}
