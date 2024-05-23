using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE1 RID: 3297
	[Tooltip("Sets the value of a Rect Variable.")]
	[ActionCategory(39)]
	public class SetRectValue : FsmStateAction
	{
		// Token: 0x06006929 RID: 26921 RVA: 0x001ED3A6 File Offset: 0x001EB5A6
		public override void Reset()
		{
			this.rectVariable = null;
			this.rectValue = null;
			this.everyFrame = false;
		}

		// Token: 0x0600692A RID: 26922 RVA: 0x001ED3BD File Offset: 0x001EB5BD
		public override void OnEnter()
		{
			this.rectVariable.Value = this.rectValue.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600692B RID: 26923 RVA: 0x001ED3E6 File Offset: 0x001EB5E6
		public override void OnUpdate()
		{
			this.rectVariable.Value = this.rectValue.Value;
		}

		// Token: 0x040050DB RID: 20699
		[UIHint(10)]
		[RequiredField]
		public FsmRect rectVariable;

		// Token: 0x040050DC RID: 20700
		[RequiredField]
		public FsmRect rectValue;

		// Token: 0x040050DD RID: 20701
		public bool everyFrame;
	}
}
