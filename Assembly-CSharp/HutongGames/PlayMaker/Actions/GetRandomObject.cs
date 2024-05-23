using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C26 RID: 3110
	[ActionCategory(4)]
	[Tooltip("Gets a Random Game Object from the scene.\nOptionally filter by Tag.")]
	public class GetRandomObject : FsmStateAction
	{
		// Token: 0x060065F6 RID: 26102 RVA: 0x001E3289 File Offset: 0x001E1489
		public override void Reset()
		{
			this.withTag = "Untagged";
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x060065F7 RID: 26103 RVA: 0x001E32A9 File Offset: 0x001E14A9
		public override void OnEnter()
		{
			this.DoGetRandomObject();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060065F8 RID: 26104 RVA: 0x001E32C2 File Offset: 0x001E14C2
		public override void OnUpdate()
		{
			this.DoGetRandomObject();
		}

		// Token: 0x060065F9 RID: 26105 RVA: 0x001E32CC File Offset: 0x001E14CC
		private void DoGetRandomObject()
		{
			GameObject[] array;
			if (this.withTag.Value != "Untagged")
			{
				array = GameObject.FindGameObjectsWithTag(this.withTag.Value);
			}
			else
			{
				array = (GameObject[])Object.FindObjectsOfType(typeof(GameObject));
			}
			if (array.Length > 0)
			{
				this.storeResult.Value = array[Random.Range(0, array.Length)];
				return;
			}
			this.storeResult.Value = null;
		}

		// Token: 0x04004DB4 RID: 19892
		[UIHint(7)]
		public FsmString withTag;

		// Token: 0x04004DB5 RID: 19893
		[UIHint(10)]
		[RequiredField]
		public FsmGameObject storeResult;

		// Token: 0x04004DB6 RID: 19894
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
