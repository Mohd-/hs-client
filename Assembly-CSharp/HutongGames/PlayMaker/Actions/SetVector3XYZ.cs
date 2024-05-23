using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CEC RID: 3308
	[Tooltip("Sets the XYZ channels of a Vector3 Variable. To leave any channel unchanged, set variable to 'None'.")]
	[ActionCategory(19)]
	public class SetVector3XYZ : FsmStateAction
	{
		// Token: 0x0600695D RID: 26973 RVA: 0x001EDE88 File Offset: 0x001EC088
		public override void Reset()
		{
			this.vector3Variable = null;
			this.vector3Value = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.z = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x0600695E RID: 26974 RVA: 0x001EDEE6 File Offset: 0x001EC0E6
		public override void OnEnter()
		{
			this.DoSetVector3XYZ();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600695F RID: 26975 RVA: 0x001EDEFF File Offset: 0x001EC0FF
		public override void OnUpdate()
		{
			this.DoSetVector3XYZ();
		}

		// Token: 0x06006960 RID: 26976 RVA: 0x001EDF08 File Offset: 0x001EC108
		private void DoSetVector3XYZ()
		{
			if (this.vector3Variable == null)
			{
				return;
			}
			Vector3 value = this.vector3Variable.Value;
			if (!this.vector3Value.IsNone)
			{
				value = this.vector3Value.Value;
			}
			if (!this.x.IsNone)
			{
				value.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				value.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				value.z = this.z.Value;
			}
			this.vector3Variable.Value = value;
		}

		// Token: 0x0400510B RID: 20747
		[RequiredField]
		[UIHint(10)]
		public FsmVector3 vector3Variable;

		// Token: 0x0400510C RID: 20748
		[UIHint(10)]
		public FsmVector3 vector3Value;

		// Token: 0x0400510D RID: 20749
		public FsmFloat x;

		// Token: 0x0400510E RID: 20750
		public FsmFloat y;

		// Token: 0x0400510F RID: 20751
		public FsmFloat z;

		// Token: 0x04005110 RID: 20752
		public bool everyFrame;
	}
}
