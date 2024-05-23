using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C35 RID: 3125
	[ActionCategory(4)]
	[Tooltip("Gets the number of Game Objects in the scene with the specified Tag.")]
	public class GetTagCount : FsmStateAction
	{
		// Token: 0x0600663B RID: 26171 RVA: 0x001E3C97 File Offset: 0x001E1E97
		public override void Reset()
		{
			this.tag = "Untagged";
			this.storeResult = null;
		}

		// Token: 0x0600663C RID: 26172 RVA: 0x001E3CB0 File Offset: 0x001E1EB0
		public override void OnEnter()
		{
			GameObject[] array = GameObject.FindGameObjectsWithTag(this.tag.Value);
			if (this.storeResult != null)
			{
				this.storeResult.Value = ((array == null) ? 0 : array.Length);
			}
			base.Finish();
		}

		// Token: 0x04004DEE RID: 19950
		[UIHint(7)]
		public FsmString tag;

		// Token: 0x04004DEF RID: 19951
		[RequiredField]
		[UIHint(10)]
		public FsmInt storeResult;
	}
}
