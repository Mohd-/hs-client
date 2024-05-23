using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B99 RID: 2969
	[ActionCategory(2)]
	[Tooltip("Draws a state label for this FSM in the Game View. The label is drawn on the game object that owns the FSM. Use this to override the global setting in the PlayMaker Debug menu.")]
	public class DrawStateLabel : FsmStateAction
	{
		// Token: 0x060063CE RID: 25550 RVA: 0x001DBE2A File Offset: 0x001DA02A
		public override void Reset()
		{
			this.showLabel = true;
		}

		// Token: 0x060063CF RID: 25551 RVA: 0x001DBE38 File Offset: 0x001DA038
		public override void OnEnter()
		{
			base.Fsm.ShowStateLabel = this.showLabel.Value;
			base.Finish();
		}

		// Token: 0x04004B3D RID: 19261
		[RequiredField]
		[Tooltip("Set to True to show State labels, or Fals to hide them.")]
		public FsmBool showLabel;
	}
}
