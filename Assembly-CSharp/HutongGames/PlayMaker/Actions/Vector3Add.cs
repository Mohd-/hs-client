using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D0C RID: 3340
	[Tooltip("Adds a value to Vector3 Variable.")]
	[ActionCategory(19)]
	public class Vector3Add : FsmStateAction
	{
		// Token: 0x060069E7 RID: 27111 RVA: 0x001F04B4 File Offset: 0x001EE6B4
		public override void Reset()
		{
			this.vector3Variable = null;
			this.addVector = new FsmVector3
			{
				UseVariable = true
			};
			this.everyFrame = false;
			this.perSecond = false;
		}

		// Token: 0x060069E8 RID: 27112 RVA: 0x001F04EA File Offset: 0x001EE6EA
		public override void OnEnter()
		{
			this.DoVector3Add();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060069E9 RID: 27113 RVA: 0x001F0503 File Offset: 0x001EE703
		public override void OnUpdate()
		{
			this.DoVector3Add();
		}

		// Token: 0x060069EA RID: 27114 RVA: 0x001F050C File Offset: 0x001EE70C
		private void DoVector3Add()
		{
			if (this.perSecond)
			{
				this.vector3Variable.Value = this.vector3Variable.Value + this.addVector.Value * Time.deltaTime;
			}
			else
			{
				this.vector3Variable.Value = this.vector3Variable.Value + this.addVector.Value;
			}
		}

		// Token: 0x040051B8 RID: 20920
		[UIHint(10)]
		[RequiredField]
		public FsmVector3 vector3Variable;

		// Token: 0x040051B9 RID: 20921
		[RequiredField]
		public FsmVector3 addVector;

		// Token: 0x040051BA RID: 20922
		public bool everyFrame;

		// Token: 0x040051BB RID: 20923
		public bool perSecond;
	}
}
