using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE7 RID: 3047
	[ActionCategory(27)]
	[Tooltip("Tests if a GameObject Variable has a null value. E.g., If the FindGameObject action failed to find an object.")]
	public class GameObjectIsNull : FsmStateAction
	{
		// Token: 0x060064EE RID: 25838 RVA: 0x001DFEA7 File Offset: 0x001DE0A7
		public override void Reset()
		{
			this.gameObject = null;
			this.isNull = null;
			this.isNotNull = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060064EF RID: 25839 RVA: 0x001DFECC File Offset: 0x001DE0CC
		public override void OnEnter()
		{
			this.DoIsGameObjectNull();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060064F0 RID: 25840 RVA: 0x001DFEE5 File Offset: 0x001DE0E5
		public override void OnUpdate()
		{
			this.DoIsGameObjectNull();
		}

		// Token: 0x060064F1 RID: 25841 RVA: 0x001DFEF0 File Offset: 0x001DE0F0
		private void DoIsGameObjectNull()
		{
			bool flag = this.gameObject.Value == null;
			if (this.storeResult != null)
			{
				this.storeResult.Value = flag;
			}
			base.Fsm.Event((!flag) ? this.isNotNull : this.isNull);
		}

		// Token: 0x04004C84 RID: 19588
		[RequiredField]
		[Tooltip("The GameObject variable to test.")]
		[UIHint(10)]
		public FsmGameObject gameObject;

		// Token: 0x04004C85 RID: 19589
		[Tooltip("Event to send if the GamObject is null.")]
		public FsmEvent isNull;

		// Token: 0x04004C86 RID: 19590
		[Tooltip("Event to send if the GamObject is NOT null.")]
		public FsmEvent isNotNull;

		// Token: 0x04004C87 RID: 19591
		[UIHint(10)]
		[Tooltip("Store the result in a bool variable.")]
		public FsmBool storeResult;

		// Token: 0x04004C88 RID: 19592
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
