using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BB9 RID: 3001
	[ActionCategory(27)]
	[Tooltip("Tests if an FSM is in the specified State.")]
	public class FsmStateTest : FsmStateAction
	{
		// Token: 0x06006457 RID: 25687 RVA: 0x001DD914 File Offset: 0x001DBB14
		public override void Reset()
		{
			this.gameObject = null;
			this.fsmName = null;
			this.stateName = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006458 RID: 25688 RVA: 0x001DD947 File Offset: 0x001DBB47
		public override void OnEnter()
		{
			this.DoFsmStateTest();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006459 RID: 25689 RVA: 0x001DD960 File Offset: 0x001DBB60
		public override void OnUpdate()
		{
			this.DoFsmStateTest();
		}

		// Token: 0x0600645A RID: 25690 RVA: 0x001DD968 File Offset: 0x001DBB68
		private void DoFsmStateTest()
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
			bool value2 = false;
			if (this.fsm.ActiveStateName == this.stateName.Value)
			{
				base.Fsm.Event(this.trueEvent);
				value2 = true;
			}
			else
			{
				base.Fsm.Event(this.falseEvent);
			}
			this.storeResult.Value = value2;
		}

		// Token: 0x04004BCF RID: 19407
		[Tooltip("The GameObject that owns the FSM.")]
		[RequiredField]
		public FsmGameObject gameObject;

		// Token: 0x04004BD0 RID: 19408
		[UIHint(15)]
		[Tooltip("Optional name of Fsm on Game Object. Useful if there is more than one FSM on the GameObject.")]
		public FsmString fsmName;

		// Token: 0x04004BD1 RID: 19409
		[Tooltip("Check to see if the FSM is in this state.")]
		[RequiredField]
		public FsmString stateName;

		// Token: 0x04004BD2 RID: 19410
		[Tooltip("Event to send if the FSM is in the specified state.")]
		public FsmEvent trueEvent;

		// Token: 0x04004BD3 RID: 19411
		[Tooltip("Event to send if the FSM is NOT in the specified state.")]
		public FsmEvent falseEvent;

		// Token: 0x04004BD4 RID: 19412
		[UIHint(10)]
		[Tooltip("Store the result of this test in a bool variable. Useful if other actions depend on this test.")]
		public FsmBool storeResult;

		// Token: 0x04004BD5 RID: 19413
		[Tooltip("Repeat every frame. Useful if you're waiting for a particular state.")]
		public bool everyFrame;

		// Token: 0x04004BD6 RID: 19414
		private GameObject previousGo;

		// Token: 0x04004BD7 RID: 19415
		private PlayMakerFSM fsm;
	}
}
