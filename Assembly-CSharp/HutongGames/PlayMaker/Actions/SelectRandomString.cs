using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C89 RID: 3209
	[Tooltip("Select a Random String from an array of Strings.")]
	[ActionCategory(16)]
	public class SelectRandomString : FsmStateAction
	{
		// Token: 0x060067AE RID: 26542 RVA: 0x001E8E0C File Offset: 0x001E700C
		public override void Reset()
		{
			this.strings = new FsmString[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.storeString = null;
		}

		// Token: 0x060067AF RID: 26543 RVA: 0x001E8E5F File Offset: 0x001E705F
		public override void OnEnter()
		{
			this.DoSelectRandomString();
			base.Finish();
		}

		// Token: 0x060067B0 RID: 26544 RVA: 0x001E8E70 File Offset: 0x001E7070
		private void DoSelectRandomString()
		{
			if (this.strings == null)
			{
				return;
			}
			if (this.strings.Length == 0)
			{
				return;
			}
			if (this.storeString == null)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				this.storeString.Value = this.strings[randomWeightedIndex].Value;
			}
		}

		// Token: 0x04004F82 RID: 20354
		[CompoundArray("Strings", "String", "Weight")]
		public FsmString[] strings;

		// Token: 0x04004F83 RID: 20355
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		// Token: 0x04004F84 RID: 20356
		[RequiredField]
		[UIHint(10)]
		public FsmString storeString;
	}
}
