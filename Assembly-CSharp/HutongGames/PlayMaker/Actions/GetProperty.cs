using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C24 RID: 3108
	[ActionCategory(40)]
	[Tooltip("Gets the value of any public property or field on the targeted Unity Object and stores it in a variable. E.g., Drag and drop any component attached to a Game Object to access its properties.")]
	public class GetProperty : FsmStateAction
	{
		// Token: 0x060065EE RID: 26094 RVA: 0x001E31B8 File Offset: 0x001E13B8
		public override void Reset()
		{
			this.targetProperty = new FsmProperty();
			this.everyFrame = false;
		}

		// Token: 0x060065EF RID: 26095 RVA: 0x001E31CC File Offset: 0x001E13CC
		public override void OnEnter()
		{
			this.targetProperty.GetValue();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060065F0 RID: 26096 RVA: 0x001E31EA File Offset: 0x001E13EA
		public override void OnUpdate()
		{
			this.targetProperty.GetValue();
		}

		// Token: 0x04004DB0 RID: 19888
		public FsmProperty targetProperty;

		// Token: 0x04004DB1 RID: 19889
		public bool everyFrame;
	}
}
