using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CEB RID: 3307
	[Tooltip("Sets the value of a Vector3 Variable.")]
	[ActionCategory(19)]
	public class SetVector3Value : FsmStateAction
	{
		// Token: 0x06006959 RID: 26969 RVA: 0x001EDE26 File Offset: 0x001EC026
		public override void Reset()
		{
			this.vector3Variable = null;
			this.vector3Value = null;
			this.everyFrame = false;
		}

		// Token: 0x0600695A RID: 26970 RVA: 0x001EDE3D File Offset: 0x001EC03D
		public override void OnEnter()
		{
			this.vector3Variable.Value = this.vector3Value.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600695B RID: 26971 RVA: 0x001EDE66 File Offset: 0x001EC066
		public override void OnUpdate()
		{
			this.vector3Variable.Value = this.vector3Value.Value;
		}

		// Token: 0x04005108 RID: 20744
		[RequiredField]
		[UIHint(10)]
		public FsmVector3 vector3Variable;

		// Token: 0x04005109 RID: 20745
		[RequiredField]
		public FsmVector3 vector3Value;

		// Token: 0x0400510A RID: 20746
		public bool everyFrame;
	}
}
