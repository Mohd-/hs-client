using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D18 RID: 3352
	[ActionCategory(19)]
	[Tooltip("Multiplies a Vector3 variable by Time.deltaTime. Useful for frame rate independent motion.")]
	public class Vector3PerSecond : FsmStateAction
	{
		// Token: 0x06006A18 RID: 27160 RVA: 0x001F107D File Offset: 0x001EF27D
		public override void Reset()
		{
			this.vector3Variable = null;
			this.everyFrame = false;
		}

		// Token: 0x06006A19 RID: 27161 RVA: 0x001F108D File Offset: 0x001EF28D
		public override void OnEnter()
		{
			this.vector3Variable.Value = this.vector3Variable.Value * Time.deltaTime;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006A1A RID: 27162 RVA: 0x001F10C0 File Offset: 0x001EF2C0
		public override void OnUpdate()
		{
			this.vector3Variable.Value = this.vector3Variable.Value * Time.deltaTime;
		}

		// Token: 0x040051F3 RID: 20979
		[RequiredField]
		[UIHint(10)]
		public FsmVector3 vector3Variable;

		// Token: 0x040051F4 RID: 20980
		public bool everyFrame;
	}
}
