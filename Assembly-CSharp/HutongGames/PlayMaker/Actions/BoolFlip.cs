using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B61 RID: 2913
	[Tooltip("Flips the value of a Bool Variable.")]
	[ActionCategory(7)]
	public class BoolFlip : FsmStateAction
	{
		// Token: 0x060062ED RID: 25325 RVA: 0x001D91BC File Offset: 0x001D73BC
		public override void Reset()
		{
			this.boolVariable = null;
		}

		// Token: 0x060062EE RID: 25326 RVA: 0x001D91C5 File Offset: 0x001D73C5
		public override void OnEnter()
		{
			this.boolVariable.Value = !this.boolVariable.Value;
			base.Finish();
		}

		// Token: 0x04004A63 RID: 19043
		[Tooltip("Bool variable to flip.")]
		[RequiredField]
		[UIHint(10)]
		public FsmBool boolVariable;
	}
}
