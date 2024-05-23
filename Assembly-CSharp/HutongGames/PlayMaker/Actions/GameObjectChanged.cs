using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE2 RID: 3042
	[ActionCategory(27)]
	[Tooltip("Tests if the value of a GameObject variable changed. Use this to send an event on change, or store a bool that can be used in other operations.")]
	public class GameObjectChanged : FsmStateAction
	{
		// Token: 0x060064D7 RID: 25815 RVA: 0x001DFADD File Offset: 0x001DDCDD
		public override void Reset()
		{
			this.gameObjectVariable = null;
			this.changedEvent = null;
			this.storeResult = null;
		}

		// Token: 0x060064D8 RID: 25816 RVA: 0x001DFAF4 File Offset: 0x001DDCF4
		public override void OnEnter()
		{
			if (this.gameObjectVariable.IsNone)
			{
				base.Finish();
				return;
			}
			this.previousValue = this.gameObjectVariable.Value;
		}

		// Token: 0x060064D9 RID: 25817 RVA: 0x001DFB20 File Offset: 0x001DDD20
		public override void OnUpdate()
		{
			this.storeResult.Value = false;
			if (this.gameObjectVariable.Value != this.previousValue)
			{
				this.storeResult.Value = true;
				base.Fsm.Event(this.changedEvent);
			}
		}

		// Token: 0x04004C6A RID: 19562
		[UIHint(10)]
		[RequiredField]
		[Tooltip("The GameObject variable to watch for a change.")]
		public FsmGameObject gameObjectVariable;

		// Token: 0x04004C6B RID: 19563
		[Tooltip("Event to send if the variable changes.")]
		public FsmEvent changedEvent;

		// Token: 0x04004C6C RID: 19564
		[Tooltip("Set to True if the variable changes.")]
		[UIHint(10)]
		public FsmBool storeResult;

		// Token: 0x04004C6D RID: 19565
		private GameObject previousValue;
	}
}
