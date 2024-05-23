using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C87 RID: 3207
	[Tooltip("Select a random Color from an array of Colors.")]
	[ActionCategory(24)]
	public class SelectRandomColor : FsmStateAction
	{
		// Token: 0x060067A6 RID: 26534 RVA: 0x001E8C74 File Offset: 0x001E6E74
		public override void Reset()
		{
			this.colors = new FsmColor[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.storeColor = null;
		}

		// Token: 0x060067A7 RID: 26535 RVA: 0x001E8CC7 File Offset: 0x001E6EC7
		public override void OnEnter()
		{
			this.DoSelectRandomColor();
			base.Finish();
		}

		// Token: 0x060067A8 RID: 26536 RVA: 0x001E8CD8 File Offset: 0x001E6ED8
		private void DoSelectRandomColor()
		{
			if (this.colors == null)
			{
				return;
			}
			if (this.colors.Length == 0)
			{
				return;
			}
			if (this.storeColor == null)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				this.storeColor.Value = this.colors[randomWeightedIndex].Value;
			}
		}

		// Token: 0x04004F7C RID: 20348
		[CompoundArray("Colors", "Color", "Weight")]
		public FsmColor[] colors;

		// Token: 0x04004F7D RID: 20349
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		// Token: 0x04004F7E RID: 20350
		[RequiredField]
		[UIHint(10)]
		public FsmColor storeColor;
	}
}
