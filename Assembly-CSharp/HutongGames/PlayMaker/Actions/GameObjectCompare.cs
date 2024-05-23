using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BE3 RID: 3043
	[ActionCategory(27)]
	[Tooltip("Compares 2 Game Objects and sends Events based on the result.")]
	public class GameObjectCompare : FsmStateAction
	{
		// Token: 0x060064DB RID: 25819 RVA: 0x001DFB7C File Offset: 0x001DDD7C
		public override void Reset()
		{
			this.gameObjectVariable = null;
			this.compareTo = null;
			this.equalEvent = null;
			this.notEqualEvent = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060064DC RID: 25820 RVA: 0x001DFBB3 File Offset: 0x001DDDB3
		public override void OnEnter()
		{
			this.DoGameObjectCompare();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060064DD RID: 25821 RVA: 0x001DFBCC File Offset: 0x001DDDCC
		public override void OnUpdate()
		{
			this.DoGameObjectCompare();
		}

		// Token: 0x060064DE RID: 25822 RVA: 0x001DFBD4 File Offset: 0x001DDDD4
		private void DoGameObjectCompare()
		{
			bool flag = base.Fsm.GetOwnerDefaultTarget(this.gameObjectVariable) == this.compareTo.Value;
			this.storeResult.Value = flag;
			if (flag && this.equalEvent != null)
			{
				base.Fsm.Event(this.equalEvent);
			}
			else if (!flag && this.notEqualEvent != null)
			{
				base.Fsm.Event(this.notEqualEvent);
			}
		}

		// Token: 0x04004C6E RID: 19566
		[UIHint(10)]
		[Tooltip("A Game Object variable to compare.")]
		[RequiredField]
		[Title("Game Object")]
		public FsmOwnerDefault gameObjectVariable;

		// Token: 0x04004C6F RID: 19567
		[Tooltip("Compare the variable with this Game Object")]
		[RequiredField]
		public FsmGameObject compareTo;

		// Token: 0x04004C70 RID: 19568
		[Tooltip("Send this event if Game Objects are equal")]
		public FsmEvent equalEvent;

		// Token: 0x04004C71 RID: 19569
		[Tooltip("Send this event if Game Objects are not equal")]
		public FsmEvent notEqualEvent;

		// Token: 0x04004C72 RID: 19570
		[UIHint(10)]
		[Tooltip("Store the result of the check in a Bool Variable. (True if equal, false if not equal).")]
		public FsmBool storeResult;

		// Token: 0x04004C73 RID: 19571
		[Tooltip("Repeat every frame. Useful if you're waiting for a true or false result.")]
		public bool everyFrame;
	}
}
