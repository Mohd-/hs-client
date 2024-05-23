using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD1 RID: 3281
	[Tooltip("Sets the main camera.")]
	[ActionCategory(22)]
	public class SetMainCamera : FsmStateAction
	{
		// Token: 0x060068E6 RID: 26854 RVA: 0x001EC3C8 File Offset: 0x001EA5C8
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060068E7 RID: 26855 RVA: 0x001EC3D4 File Offset: 0x001EA5D4
		public override void OnEnter()
		{
			if (this.gameObject.Value != null)
			{
				if (Camera.main != null)
				{
					Camera.main.gameObject.tag = "Untagged";
				}
				this.gameObject.Value.tag = "MainCamera";
			}
			base.Finish();
		}

		// Token: 0x0400509C RID: 20636
		[RequiredField]
		[Tooltip("The GameObject to set as the main camera (should have a Camera component).")]
		[CheckForComponent(typeof(Camera))]
		public FsmGameObject gameObject;
	}
}
