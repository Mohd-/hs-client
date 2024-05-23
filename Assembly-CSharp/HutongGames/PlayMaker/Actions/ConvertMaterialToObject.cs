using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B7D RID: 2941
	[ActionCategory(17)]
	[Tooltip("Converts a Material variable to an Object variable. Useful if you want to use Set Property (which only works on Object variables).")]
	public class ConvertMaterialToObject : FsmStateAction
	{
		// Token: 0x0600636E RID: 25454 RVA: 0x001DAF96 File Offset: 0x001D9196
		public override void Reset()
		{
			this.materialVariable = null;
			this.objectVariable = null;
			this.everyFrame = false;
		}

		// Token: 0x0600636F RID: 25455 RVA: 0x001DAFAD File Offset: 0x001D91AD
		public override void OnEnter()
		{
			this.DoConvertMaterialToObject();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006370 RID: 25456 RVA: 0x001DAFC6 File Offset: 0x001D91C6
		public override void OnUpdate()
		{
			this.DoConvertMaterialToObject();
		}

		// Token: 0x06006371 RID: 25457 RVA: 0x001DAFCE File Offset: 0x001D91CE
		private void DoConvertMaterialToObject()
		{
			this.objectVariable.Value = this.materialVariable.Value;
		}

		// Token: 0x04004AF2 RID: 19186
		[UIHint(10)]
		[Tooltip("The Material variable to convert to an Object.")]
		[RequiredField]
		public FsmMaterial materialVariable;

		// Token: 0x04004AF3 RID: 19187
		[UIHint(10)]
		[Tooltip("Store the result in an Object variable.")]
		[RequiredField]
		public FsmObject objectVariable;

		// Token: 0x04004AF4 RID: 19188
		[Tooltip("Repeat every frame. Useful if the Material variable is changing.")]
		public bool everyFrame;
	}
}
