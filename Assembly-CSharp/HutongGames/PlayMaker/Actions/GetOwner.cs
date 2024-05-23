using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C20 RID: 3104
	[Tooltip("Gets the Game Object that owns the FSM and stores it in a game object variable.")]
	[ActionCategory(4)]
	public class GetOwner : FsmStateAction
	{
		// Token: 0x060065E0 RID: 26080 RVA: 0x001E2FB0 File Offset: 0x001E11B0
		public override void Reset()
		{
			this.storeGameObject = null;
		}

		// Token: 0x060065E1 RID: 26081 RVA: 0x001E2FB9 File Offset: 0x001E11B9
		public override void OnEnter()
		{
			this.storeGameObject.Value = base.Owner;
			base.Finish();
		}

		// Token: 0x04004DA5 RID: 19877
		[RequiredField]
		[UIHint(10)]
		public FsmGameObject storeGameObject;
	}
}
