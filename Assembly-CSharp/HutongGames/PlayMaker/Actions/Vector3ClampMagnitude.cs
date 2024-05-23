using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D0E RID: 3342
	[ActionCategory(19)]
	[Tooltip("Clamps the Magnitude of Vector3 Variable.")]
	public class Vector3ClampMagnitude : FsmStateAction
	{
		// Token: 0x060069F1 RID: 27121 RVA: 0x001F0681 File Offset: 0x001EE881
		public override void Reset()
		{
			this.vector3Variable = null;
			this.maxLength = null;
			this.everyFrame = false;
		}

		// Token: 0x060069F2 RID: 27122 RVA: 0x001F0698 File Offset: 0x001EE898
		public override void OnEnter()
		{
			this.DoVector3ClampMagnitude();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060069F3 RID: 27123 RVA: 0x001F06B1 File Offset: 0x001EE8B1
		public override void OnUpdate()
		{
			this.DoVector3ClampMagnitude();
		}

		// Token: 0x060069F4 RID: 27124 RVA: 0x001F06B9 File Offset: 0x001EE8B9
		private void DoVector3ClampMagnitude()
		{
			this.vector3Variable.Value = Vector3.ClampMagnitude(this.vector3Variable.Value, this.maxLength.Value);
		}

		// Token: 0x040051C2 RID: 20930
		[RequiredField]
		[UIHint(10)]
		public FsmVector3 vector3Variable;

		// Token: 0x040051C3 RID: 20931
		[RequiredField]
		public FsmFloat maxLength;

		// Token: 0x040051C4 RID: 20932
		public bool everyFrame;
	}
}
