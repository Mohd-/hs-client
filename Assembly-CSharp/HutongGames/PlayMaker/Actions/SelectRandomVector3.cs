using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C8A RID: 3210
	[ActionCategory(19)]
	[Tooltip("Select a Random Vector3 from a Vector3 array.")]
	public class SelectRandomVector3 : FsmStateAction
	{
		// Token: 0x060067B2 RID: 26546 RVA: 0x001E8ED8 File Offset: 0x001E70D8
		public override void Reset()
		{
			this.vector3Array = new FsmVector3[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.storeVector3 = null;
		}

		// Token: 0x060067B3 RID: 26547 RVA: 0x001E8F2B File Offset: 0x001E712B
		public override void OnEnter()
		{
			this.DoSelectRandomColor();
			base.Finish();
		}

		// Token: 0x060067B4 RID: 26548 RVA: 0x001E8F3C File Offset: 0x001E713C
		private void DoSelectRandomColor()
		{
			if (this.vector3Array == null)
			{
				return;
			}
			if (this.vector3Array.Length == 0)
			{
				return;
			}
			if (this.storeVector3 == null)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				this.storeVector3.Value = this.vector3Array[randomWeightedIndex].Value;
			}
		}

		// Token: 0x04004F85 RID: 20357
		[CompoundArray("Vectors", "Vector", "Weight")]
		public FsmVector3[] vector3Array;

		// Token: 0x04004F86 RID: 20358
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		// Token: 0x04004F87 RID: 20359
		[UIHint(10)]
		[RequiredField]
		public FsmVector3 storeVector3;
	}
}
