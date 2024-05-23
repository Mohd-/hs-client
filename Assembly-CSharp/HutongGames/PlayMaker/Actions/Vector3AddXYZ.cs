using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D0D RID: 3341
	[Tooltip("Adds a XYZ values to Vector3 Variable.")]
	[ActionCategory(19)]
	public class Vector3AddXYZ : FsmStateAction
	{
		// Token: 0x060069EC RID: 27116 RVA: 0x001F0588 File Offset: 0x001EE788
		public override void Reset()
		{
			this.vector3Variable = null;
			this.addX = 0f;
			this.addY = 0f;
			this.addZ = 0f;
			this.everyFrame = false;
			this.perSecond = false;
		}

		// Token: 0x060069ED RID: 27117 RVA: 0x001F05DA File Offset: 0x001EE7DA
		public override void OnEnter()
		{
			this.DoVector3AddXYZ();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060069EE RID: 27118 RVA: 0x001F05F3 File Offset: 0x001EE7F3
		public override void OnUpdate()
		{
			this.DoVector3AddXYZ();
		}

		// Token: 0x060069EF RID: 27119 RVA: 0x001F05FC File Offset: 0x001EE7FC
		private void DoVector3AddXYZ()
		{
			Vector3 vector;
			vector..ctor(this.addX.Value, this.addY.Value, this.addZ.Value);
			if (this.perSecond)
			{
				this.vector3Variable.Value += vector * Time.deltaTime;
			}
			else
			{
				this.vector3Variable.Value += vector;
			}
		}

		// Token: 0x040051BC RID: 20924
		[UIHint(10)]
		[RequiredField]
		public FsmVector3 vector3Variable;

		// Token: 0x040051BD RID: 20925
		public FsmFloat addX;

		// Token: 0x040051BE RID: 20926
		public FsmFloat addY;

		// Token: 0x040051BF RID: 20927
		public FsmFloat addZ;

		// Token: 0x040051C0 RID: 20928
		public bool everyFrame;

		// Token: 0x040051C1 RID: 20929
		public bool perSecond;
	}
}
