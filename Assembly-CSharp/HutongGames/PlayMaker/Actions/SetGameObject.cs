using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC1 RID: 3265
	[ActionCategory(4)]
	[Tooltip("Sets the value of a Game Object Variable.")]
	public class SetGameObject : FsmStateAction
	{
		// Token: 0x060068A1 RID: 26785 RVA: 0x001EBC2D File Offset: 0x001E9E2D
		public override void Reset()
		{
			this.variable = null;
			this.gameObject = null;
			this.everyFrame = false;
		}

		// Token: 0x060068A2 RID: 26786 RVA: 0x001EBC44 File Offset: 0x001E9E44
		public override void OnEnter()
		{
			this.variable.Value = this.gameObject.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068A3 RID: 26787 RVA: 0x001EBC6D File Offset: 0x001E9E6D
		public override void OnUpdate()
		{
			this.variable.Value = this.gameObject.Value;
		}

		// Token: 0x04005072 RID: 20594
		[UIHint(10)]
		[RequiredField]
		public FsmGameObject variable;

		// Token: 0x04005073 RID: 20595
		public FsmGameObject gameObject;

		// Token: 0x04005074 RID: 20596
		public bool everyFrame;
	}
}
