using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE4 RID: 3044
	[Tooltip("Tests if a Game Object has a tag.")]
	[ActionCategory(27)]
	public class GameObjectCompareTag : FsmStateAction
	{
		// Token: 0x060064E0 RID: 25824 RVA: 0x001DFC60 File Offset: 0x001DDE60
		public override void Reset()
		{
			this.gameObject = null;
			this.tag = "Untagged";
			this.trueEvent = null;
			this.falseEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060064E1 RID: 25825 RVA: 0x001DFC95 File Offset: 0x001DDE95
		public override void OnEnter()
		{
			this.DoCompareTag();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060064E2 RID: 25826 RVA: 0x001DFCAE File Offset: 0x001DDEAE
		public override void OnUpdate()
		{
			this.DoCompareTag();
		}

		// Token: 0x060064E3 RID: 25827 RVA: 0x001DFCB8 File Offset: 0x001DDEB8
		private void DoCompareTag()
		{
			bool flag = false;
			if (this.gameObject.Value != null)
			{
				flag = this.gameObject.Value.CompareTag(this.tag.Value);
			}
			this.storeResult.Value = flag;
			base.Fsm.Event((!flag) ? this.falseEvent : this.trueEvent);
		}

		// Token: 0x04004C74 RID: 19572
		[RequiredField]
		[Tooltip("The GameObject to test.")]
		public FsmGameObject gameObject;

		// Token: 0x04004C75 RID: 19573
		[Tooltip("The Tag to check for.")]
		[UIHint(7)]
		[RequiredField]
		public FsmString tag;

		// Token: 0x04004C76 RID: 19574
		[Tooltip("Event to send if the GameObject has the Tag.")]
		public FsmEvent trueEvent;

		// Token: 0x04004C77 RID: 19575
		[Tooltip("Event to send if the GameObject does not have the Tag.")]
		public FsmEvent falseEvent;

		// Token: 0x04004C78 RID: 19576
		[UIHint(10)]
		[Tooltip("Store the result in a Bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04004C79 RID: 19577
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
