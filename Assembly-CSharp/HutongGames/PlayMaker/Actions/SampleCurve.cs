using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C82 RID: 3202
	[Tooltip("Gets the value of a curve at a given time and stores it in a Float Variable. NOTE: This can be used for more than just animation! It's a general way to transform an input number into an output number using a curve (e.g., linear input -> bell curve).")]
	[ActionCategory(7)]
	public class SampleCurve : FsmStateAction
	{
		// Token: 0x0600678E RID: 26510 RVA: 0x001E8573 File Offset: 0x001E6773
		public override void Reset()
		{
			this.curve = null;
			this.sampleAt = null;
			this.storeValue = null;
			this.everyFrame = false;
		}

		// Token: 0x0600678F RID: 26511 RVA: 0x001E8591 File Offset: 0x001E6791
		public override void OnEnter()
		{
			this.DoSampleCurve();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006790 RID: 26512 RVA: 0x001E85AA File Offset: 0x001E67AA
		public override void OnUpdate()
		{
			this.DoSampleCurve();
		}

		// Token: 0x06006791 RID: 26513 RVA: 0x001E85B4 File Offset: 0x001E67B4
		private void DoSampleCurve()
		{
			if (this.curve == null || this.curve.curve == null || this.storeValue == null)
			{
				return;
			}
			this.storeValue.Value = this.curve.curve.Evaluate(this.sampleAt.Value);
		}

		// Token: 0x04004F57 RID: 20311
		[RequiredField]
		public FsmAnimationCurve curve;

		// Token: 0x04004F58 RID: 20312
		[RequiredField]
		public FsmFloat sampleAt;

		// Token: 0x04004F59 RID: 20313
		[RequiredField]
		[UIHint(10)]
		public FsmFloat storeValue;

		// Token: 0x04004F5A RID: 20314
		public bool everyFrame;
	}
}
