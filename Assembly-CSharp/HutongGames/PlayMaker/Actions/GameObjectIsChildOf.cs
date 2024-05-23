using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE6 RID: 3046
	[ActionCategory(27)]
	[Tooltip("Tests if a GameObject is a Child of another GameObject.")]
	public class GameObjectIsChildOf : FsmStateAction
	{
		// Token: 0x060064EA RID: 25834 RVA: 0x001DFDE9 File Offset: 0x001DDFE9
		public override void Reset()
		{
			this.gameObject = null;
			this.isChildOf = null;
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
		}

		// Token: 0x060064EB RID: 25835 RVA: 0x001DFE0E File Offset: 0x001DE00E
		public override void OnEnter()
		{
			this.DoIsChildOf(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x060064EC RID: 25836 RVA: 0x001DFE30 File Offset: 0x001DE030
		private void DoIsChildOf(GameObject go)
		{
			if (go == null || this.isChildOf == null)
			{
				return;
			}
			bool flag = go.transform.IsChildOf(this.isChildOf.Value.transform);
			this.storeResult.Value = flag;
			base.Fsm.Event((!flag) ? this.falseEvent : this.trueEvent);
		}

		// Token: 0x04004C7F RID: 19583
		[RequiredField]
		[Tooltip("GameObject to test.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004C80 RID: 19584
		[Tooltip("Is it a child of this GameObject?")]
		[RequiredField]
		public FsmGameObject isChildOf;

		// Token: 0x04004C81 RID: 19585
		[Tooltip("Event to send if GameObject is a child.")]
		public FsmEvent trueEvent;

		// Token: 0x04004C82 RID: 19586
		[Tooltip("Event to send if GameObject is NOT a child.")]
		public FsmEvent falseEvent;

		// Token: 0x04004C83 RID: 19587
		[RequiredField]
		[UIHint(10)]
		[Tooltip("Store result in a bool variable")]
		public FsmBool storeResult;
	}
}
