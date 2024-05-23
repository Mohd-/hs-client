using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C19 RID: 3097
	[ActionCategory(6)]
	[Tooltip("Gets the pressed state of the specified Mouse Button and stores it in a Bool Variable. See Unity Input Manager doc.")]
	public class GetMouseButton : FsmStateAction
	{
		// Token: 0x060065C4 RID: 26052 RVA: 0x001E2C1B File Offset: 0x001E0E1B
		public override void Reset()
		{
			this.button = 0;
			this.storeResult = null;
		}

		// Token: 0x060065C5 RID: 26053 RVA: 0x001E2C2B File Offset: 0x001E0E2B
		public override void OnUpdate()
		{
			this.storeResult.Value = Input.GetMouseButton(this.button);
		}

		// Token: 0x04004D90 RID: 19856
		[RequiredField]
		public MouseButton button;

		// Token: 0x04004D91 RID: 19857
		[RequiredField]
		[UIHint(10)]
		public FsmBool storeResult;
	}
}
