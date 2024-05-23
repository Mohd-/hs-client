using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C0F RID: 3087
	[Tooltip("Gets the pressed state of a Key.")]
	[ActionCategory(6)]
	public class GetKey : FsmStateAction
	{
		// Token: 0x0600659E RID: 26014 RVA: 0x001E25BB File Offset: 0x001E07BB
		public override void Reset()
		{
			this.key = 0;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x0600659F RID: 26015 RVA: 0x001E25D2 File Offset: 0x001E07D2
		public override void OnEnter()
		{
			this.DoGetKey();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060065A0 RID: 26016 RVA: 0x001E25EB File Offset: 0x001E07EB
		public override void OnUpdate()
		{
			this.DoGetKey();
		}

		// Token: 0x060065A1 RID: 26017 RVA: 0x001E25F3 File Offset: 0x001E07F3
		private void DoGetKey()
		{
			this.storeResult.Value = Input.GetKey(this.key);
		}

		// Token: 0x04004D70 RID: 19824
		[Tooltip("The key to test.")]
		[RequiredField]
		public KeyCode key;

		// Token: 0x04004D71 RID: 19825
		[RequiredField]
		[UIHint(10)]
		[Tooltip("Store if the key is down (True) or up (False).")]
		public FsmBool storeResult;

		// Token: 0x04004D72 RID: 19826
		[Tooltip("Repeat every frame. Useful if you're waiting for a key press/release.")]
		public bool everyFrame;
	}
}
