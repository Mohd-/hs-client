using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB8 RID: 3000
	[Tooltip("Sends Events based on the current State of an FSM.")]
	[ActionCategory(27)]
	public class FsmStateSwitch : FsmStateAction
	{
		// Token: 0x06006452 RID: 25682 RVA: 0x001DD801 File Offset: 0x001DBA01
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = null;
			this.compareTo = new FsmString[1];
			this.sendEvent = new FsmEvent[1];
			this.everyFrame = false;
		}

		// Token: 0x06006453 RID: 25683 RVA: 0x001DD830 File Offset: 0x001DBA30
		public override void OnEnter()
		{
			this.DoFsmStateSwitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006454 RID: 25684 RVA: 0x001DD849 File Offset: 0x001DBA49
		public override void OnUpdate()
		{
			this.DoFsmStateSwitch();
		}

		// Token: 0x06006455 RID: 25685 RVA: 0x001DD854 File Offset: 0x001DBA54
		private void DoFsmStateSwitch()
		{
			GameObject value = this.gameObject.Value;
			if (value == null)
			{
				return;
			}
			if (value != this.previousGo)
			{
				this.fsm = ActionHelpers.GetGameObjectFsm(value, this.fsmName.Value);
				this.previousGo = value;
			}
			if (this.fsm == null)
			{
				return;
			}
			string activeStateName = this.fsm.ActiveStateName;
			for (int i = 0; i < this.compareTo.Length; i++)
			{
				if (activeStateName == this.compareTo[i].Value)
				{
					base.Fsm.Event(this.sendEvent[i]);
					return;
				}
			}
		}

		// Token: 0x04004BC8 RID: 19400
		[Tooltip("The GameObject that owns the FSM.")]
		[RequiredField]
		public FsmGameObject gameObject;

		// Token: 0x04004BC9 RID: 19401
		[UIHint(15)]
		[Tooltip("Optional name of Fsm on GameObject. Useful if there is more than one FSM on the GameObject.")]
		public FsmString fsmName;

		// Token: 0x04004BCA RID: 19402
		[CompoundArray("State Switches", "Compare State", "Send Event")]
		public FsmString[] compareTo;

		// Token: 0x04004BCB RID: 19403
		public FsmEvent[] sendEvent;

		// Token: 0x04004BCC RID: 19404
		[Tooltip("Repeat every frame. Useful if you're waiting for a particular result.")]
		public bool everyFrame;

		// Token: 0x04004BCD RID: 19405
		private GameObject previousGo;

		// Token: 0x04004BCE RID: 19406
		private PlayMakerFSM fsm;
	}
}
