using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C15 RID: 3093
	[Tooltip("Gets the camera tagged MainCamera from the scene")]
	[ActionCategory(22)]
	public class GetMainCamera : FsmStateAction
	{
		// Token: 0x060065B5 RID: 26037 RVA: 0x001E27F7 File Offset: 0x001E09F7
		public override void Reset()
		{
			this.storeGameObject = null;
		}

		// Token: 0x060065B6 RID: 26038 RVA: 0x001E2800 File Offset: 0x001E0A00
		public override void OnEnter()
		{
			this.storeGameObject.Value = ((!(Camera.main != null)) ? null : Camera.main.gameObject);
			base.Finish();
		}

		// Token: 0x04004D84 RID: 19844
		[UIHint(10)]
		[RequiredField]
		public FsmGameObject storeGameObject;
	}
}
