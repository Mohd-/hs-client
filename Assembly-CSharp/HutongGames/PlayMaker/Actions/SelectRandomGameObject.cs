using System;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C88 RID: 3208
	[Tooltip("Selects a Random Game Object from an array of Game Objects.")]
	[ActionCategory(4)]
	public class SelectRandomGameObject : FsmStateAction
	{
		// Token: 0x060067AA RID: 26538 RVA: 0x001E8D40 File Offset: 0x001E6F40
		public override void Reset()
		{
			this.gameObjects = new FsmGameObject[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.storeGameObject = null;
		}

		// Token: 0x060067AB RID: 26539 RVA: 0x001E8D93 File Offset: 0x001E6F93
		public override void OnEnter()
		{
			this.DoSelectRandomGameObject();
			base.Finish();
		}

		// Token: 0x060067AC RID: 26540 RVA: 0x001E8DA4 File Offset: 0x001E6FA4
		private void DoSelectRandomGameObject()
		{
			if (this.gameObjects == null)
			{
				return;
			}
			if (this.gameObjects.Length == 0)
			{
				return;
			}
			if (this.storeGameObject == null)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				this.storeGameObject.Value = this.gameObjects[randomWeightedIndex].Value;
			}
		}

		// Token: 0x04004F7F RID: 20351
		[CompoundArray("Game Objects", "Game Object", "Weight")]
		public FsmGameObject[] gameObjects;

		// Token: 0x04004F80 RID: 20352
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		// Token: 0x04004F81 RID: 20353
		[UIHint(10)]
		[RequiredField]
		public FsmGameObject storeGameObject;
	}
}
