using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C1F RID: 3103
	[Tooltip("Each time this action is called it gets the next child of a GameObject. This lets you quickly loop through all the children of an object to perform actions on them. NOTE: To find a specific child use Find Child.")]
	[ActionCategory(4)]
	public class GetNextChild : FsmStateAction
	{
		// Token: 0x060065DC RID: 26076 RVA: 0x001E2E7C File Offset: 0x001E107C
		public override void Reset()
		{
			this.gameObject = null;
			this.storeNextChild = null;
			this.loopEvent = null;
			this.finishedEvent = null;
		}

		// Token: 0x060065DD RID: 26077 RVA: 0x001E2E9A File Offset: 0x001E109A
		public override void OnEnter()
		{
			this.DoGetNextChild(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x060065DE RID: 26078 RVA: 0x001E2EBC File Offset: 0x001E10BC
		private void DoGetNextChild(GameObject parent)
		{
			if (parent == null)
			{
				return;
			}
			if (this.go != parent)
			{
				this.go = parent;
				this.nextChildIndex = 0;
			}
			if (this.nextChildIndex >= this.go.transform.childCount)
			{
				this.nextChildIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			this.storeNextChild.Value = parent.transform.GetChild(this.nextChildIndex).gameObject;
			if (this.nextChildIndex >= this.go.transform.childCount)
			{
				this.nextChildIndex = 0;
				base.Fsm.Event(this.finishedEvent);
				return;
			}
			this.nextChildIndex++;
			if (this.loopEvent != null)
			{
				base.Fsm.Event(this.loopEvent);
			}
		}

		// Token: 0x04004D9F RID: 19871
		[Tooltip("The parent GameObject. Note, if GameObject changes, this action will reset and start again at the first child.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004DA0 RID: 19872
		[UIHint(10)]
		[RequiredField]
		[Tooltip("Store the next child in a GameObject variable.")]
		public FsmGameObject storeNextChild;

		// Token: 0x04004DA1 RID: 19873
		[Tooltip("Event to send to get the next child.")]
		public FsmEvent loopEvent;

		// Token: 0x04004DA2 RID: 19874
		[Tooltip("Event to send when there are no more children.")]
		public FsmEvent finishedEvent;

		// Token: 0x04004DA3 RID: 19875
		private GameObject go;

		// Token: 0x04004DA4 RID: 19876
		private int nextChildIndex;
	}
}
