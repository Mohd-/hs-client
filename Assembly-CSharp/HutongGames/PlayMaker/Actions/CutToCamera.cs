using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B82 RID: 2946
	[Tooltip("Activates a Camera in the scene.")]
	[ActionCategory(22)]
	public class CutToCamera : FsmStateAction
	{
		// Token: 0x06006385 RID: 25477 RVA: 0x001DB390 File Offset: 0x001D9590
		public override void Reset()
		{
			this.camera = null;
			this.makeMainCamera = true;
			this.cutBackOnExit = false;
		}

		// Token: 0x06006386 RID: 25478 RVA: 0x001DB3A8 File Offset: 0x001D95A8
		public override void OnEnter()
		{
			if (this.camera == null)
			{
				this.LogError("Missing camera!");
				return;
			}
			this.oldCamera = Camera.main;
			CutToCamera.SwitchCamera(Camera.main, this.camera);
			if (this.makeMainCamera)
			{
				this.camera.tag = "MainCamera";
			}
			base.Finish();
		}

		// Token: 0x06006387 RID: 25479 RVA: 0x001DB40E File Offset: 0x001D960E
		public override void OnExit()
		{
			if (this.cutBackOnExit)
			{
				CutToCamera.SwitchCamera(this.camera, this.oldCamera);
			}
		}

		// Token: 0x06006388 RID: 25480 RVA: 0x001DB42C File Offset: 0x001D962C
		private static void SwitchCamera(Camera camera1, Camera camera2)
		{
			if (camera1 != null)
			{
				camera1.enabled = false;
			}
			if (camera2 != null)
			{
				camera2.enabled = true;
			}
		}

		// Token: 0x04004B04 RID: 19204
		[RequiredField]
		public Camera camera;

		// Token: 0x04004B05 RID: 19205
		public bool makeMainCamera;

		// Token: 0x04004B06 RID: 19206
		public bool cutBackOnExit;

		// Token: 0x04004B07 RID: 19207
		private Camera oldCamera;
	}
}
