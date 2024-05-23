using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D0A RID: 3338
	[ActionCategory(26)]
	[Tooltip("Turn GUILayout on/off. If you don't use GUILayout actions you can get some performace back by turning GUILayout off. This can make a difference on iOS platforms.")]
	public class UseGUILayout : FsmStateAction
	{
		// Token: 0x060069E0 RID: 27104 RVA: 0x001F0412 File Offset: 0x001EE612
		public override void Reset()
		{
			this.turnOffGUIlayout = true;
		}

		// Token: 0x060069E1 RID: 27105 RVA: 0x001F041B File Offset: 0x001EE61B
		public override void OnEnter()
		{
			base.Fsm.Owner.useGUILayout = !this.turnOffGUIlayout;
			base.Finish();
		}

		// Token: 0x040051B5 RID: 20917
		[RequiredField]
		public bool turnOffGUIlayout;
	}
}
