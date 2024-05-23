using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE9 RID: 3049
	[Tooltip("Sends an Event based on a Game Object's Tag.")]
	[ActionCategory(27)]
	public class GameObjectTagSwitch : FsmStateAction
	{
		// Token: 0x060064F8 RID: 25848 RVA: 0x001E0008 File Offset: 0x001DE208
		public override void Reset()
		{
			this.gameObject = null;
			this.compareTo = new FsmString[1];
			this.sendEvent = new FsmEvent[1];
			this.everyFrame = false;
		}

		// Token: 0x060064F9 RID: 25849 RVA: 0x001E003B File Offset: 0x001DE23B
		public override void OnEnter()
		{
			this.DoTagSwitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060064FA RID: 25850 RVA: 0x001E0054 File Offset: 0x001DE254
		public override void OnUpdate()
		{
			this.DoTagSwitch();
		}

		// Token: 0x060064FB RID: 25851 RVA: 0x001E005C File Offset: 0x001DE25C
		private void DoTagSwitch()
		{
			GameObject value = this.gameObject.Value;
			if (value == null)
			{
				return;
			}
			for (int i = 0; i < this.compareTo.Length; i++)
			{
				if (value.tag == this.compareTo[i].Value)
				{
					base.Fsm.Event(this.sendEvent[i]);
					return;
				}
			}
		}

		// Token: 0x04004C8E RID: 19598
		[Tooltip("The GameObject to test.")]
		[RequiredField]
		[UIHint(10)]
		public FsmGameObject gameObject;

		// Token: 0x04004C8F RID: 19599
		[CompoundArray("Tag Switches", "Compare Tag", "Send Event")]
		[UIHint(7)]
		public FsmString[] compareTo;

		// Token: 0x04004C90 RID: 19600
		public FsmEvent[] sendEvent;

		// Token: 0x04004C91 RID: 19601
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
