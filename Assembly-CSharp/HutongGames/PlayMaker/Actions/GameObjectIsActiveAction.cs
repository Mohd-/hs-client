using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB5 RID: 3509
	[Tooltip("Tests if a Game Object is active.")]
	[ActionCategory("Pegasus")]
	public class GameObjectIsActiveAction : FsmStateAction
	{
		// Token: 0x06006CEF RID: 27887 RVA: 0x00200D68 File Offset: 0x001FEF68
		public override void Reset()
		{
			this.gameObject = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x06006CF0 RID: 27888 RVA: 0x00200D8D File Offset: 0x001FEF8D
		public override void OnEnter()
		{
			this.DoIsActive();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CF1 RID: 27889 RVA: 0x00200DA6 File Offset: 0x001FEFA6
		public override void OnUpdate()
		{
			this.DoIsActive();
		}

		// Token: 0x06006CF2 RID: 27890 RVA: 0x00200DB0 File Offset: 0x001FEFB0
		private void DoIsActive()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				bool activeInHierarchy = ownerDefaultTarget.activeInHierarchy;
				this.storeResult.Value = activeInHierarchy;
				base.Fsm.Event((!activeInHierarchy) ? this.falseEvent : this.trueEvent);
			}
			else
			{
				Debug.LogError("FSM GameObjectIsActive Error: GameObject is Null!");
			}
		}

		// Token: 0x040055A7 RID: 21927
		[RequiredField]
		[Tooltip("The GameObject to test.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055A8 RID: 21928
		[Tooltip("Event to send if the GameObject is active.")]
		public FsmEvent trueEvent;

		// Token: 0x040055A9 RID: 21929
		[Tooltip("Event to send if the GameObject is NOT active.")]
		public FsmEvent falseEvent;

		// Token: 0x040055AA RID: 21930
		[UIHint(10)]
		[Tooltip("Store the result in a bool variable.")]
		public FsmBool storeResult;

		// Token: 0x040055AB RID: 21931
		public bool everyFrame;
	}
}
