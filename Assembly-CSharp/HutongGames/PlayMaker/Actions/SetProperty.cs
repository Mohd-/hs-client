using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CDD RID: 3293
	[Tooltip("Sets the value of any public property or field on the targeted Unity Object. E.g., Drag and drop any component attached to a Game Object to access its properties.")]
	[ActionCategory(40)]
	public class SetProperty : FsmStateAction
	{
		// Token: 0x06006917 RID: 26903 RVA: 0x001ECFB4 File Offset: 0x001EB1B4
		public override void Reset()
		{
			this.targetProperty = new FsmProperty
			{
				setProperty = true
			};
			this.everyFrame = false;
		}

		// Token: 0x06006918 RID: 26904 RVA: 0x001ECFDC File Offset: 0x001EB1DC
		public override void OnEnter()
		{
			this.SetValue(this.targetProperty);
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006919 RID: 26905 RVA: 0x001ECFFB File Offset: 0x001EB1FB
		public override void OnUpdate()
		{
			this.SetValue(this.targetProperty);
		}

		// Token: 0x0600691A RID: 26906 RVA: 0x001ED009 File Offset: 0x001EB209
		private void SetValue(FsmProperty property)
		{
			property.SetValue();
		}

		// Token: 0x040050CC RID: 20684
		public FsmProperty targetProperty;

		// Token: 0x040050CD RID: 20685
		public bool everyFrame;
	}
}
