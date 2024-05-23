using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C3C RID: 3132
	[ActionCategory(19)]
	[Tooltip("Get the XYZ channels of a Vector3 Variable and storew them in Float Variables.")]
	public class GetVector3XYZ : FsmStateAction
	{
		// Token: 0x06006656 RID: 26198 RVA: 0x001E4259 File Offset: 0x001E2459
		public override void Reset()
		{
			this.vector3Variable = null;
			this.storeX = null;
			this.storeY = null;
			this.storeZ = null;
			this.everyFrame = false;
		}

		// Token: 0x06006657 RID: 26199 RVA: 0x001E427E File Offset: 0x001E247E
		public override void OnEnter()
		{
			this.DoGetVector3XYZ();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006658 RID: 26200 RVA: 0x001E4297 File Offset: 0x001E2497
		public override void OnUpdate()
		{
			this.DoGetVector3XYZ();
		}

		// Token: 0x06006659 RID: 26201 RVA: 0x001E42A0 File Offset: 0x001E24A0
		private void DoGetVector3XYZ()
		{
			if (this.vector3Variable == null)
			{
				return;
			}
			if (this.storeX != null)
			{
				this.storeX.Value = this.vector3Variable.Value.x;
			}
			if (this.storeY != null)
			{
				this.storeY.Value = this.vector3Variable.Value.y;
			}
			if (this.storeZ != null)
			{
				this.storeZ.Value = this.vector3Variable.Value.z;
			}
		}

		// Token: 0x04004E10 RID: 19984
		[RequiredField]
		[UIHint(10)]
		public FsmVector3 vector3Variable;

		// Token: 0x04004E11 RID: 19985
		[UIHint(10)]
		public FsmFloat storeX;

		// Token: 0x04004E12 RID: 19986
		[UIHint(10)]
		public FsmFloat storeY;

		// Token: 0x04004E13 RID: 19987
		[UIHint(10)]
		public FsmFloat storeZ;

		// Token: 0x04004E14 RID: 19988
		public bool everyFrame;
	}
}
