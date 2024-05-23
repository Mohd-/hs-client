using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C28 RID: 3112
	[Tooltip("Get the individual fields of a Rect Variable and store them in Float Variables.")]
	[ActionCategory(39)]
	public class GetRectFields : FsmStateAction
	{
		// Token: 0x06006600 RID: 26112 RVA: 0x001E344C File Offset: 0x001E164C
		public override void Reset()
		{
			this.rectVariable = null;
			this.storeX = null;
			this.storeY = null;
			this.storeWidth = null;
			this.storeHeight = null;
			this.everyFrame = false;
		}

		// Token: 0x06006601 RID: 26113 RVA: 0x001E3483 File Offset: 0x001E1683
		public override void OnEnter()
		{
			this.DoGetRectFields();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006602 RID: 26114 RVA: 0x001E349C File Offset: 0x001E169C
		public override void OnUpdate()
		{
			this.DoGetRectFields();
		}

		// Token: 0x06006603 RID: 26115 RVA: 0x001E34A4 File Offset: 0x001E16A4
		private void DoGetRectFields()
		{
			if (this.rectVariable.IsNone)
			{
				return;
			}
			this.storeX.Value = this.rectVariable.Value.x;
			this.storeY.Value = this.rectVariable.Value.y;
			this.storeWidth.Value = this.rectVariable.Value.width;
			this.storeHeight.Value = this.rectVariable.Value.height;
		}

		// Token: 0x04004DBC RID: 19900
		[UIHint(10)]
		[RequiredField]
		public FsmRect rectVariable;

		// Token: 0x04004DBD RID: 19901
		[UIHint(10)]
		public FsmFloat storeX;

		// Token: 0x04004DBE RID: 19902
		[UIHint(10)]
		public FsmFloat storeY;

		// Token: 0x04004DBF RID: 19903
		[UIHint(10)]
		public FsmFloat storeWidth;

		// Token: 0x04004DC0 RID: 19904
		[UIHint(10)]
		public FsmFloat storeHeight;

		// Token: 0x04004DC1 RID: 19905
		public bool everyFrame;
	}
}
