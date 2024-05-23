using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C9E RID: 3230
	[ActionCategory(22)]
	[Tooltip("Sets the Culling Mask used by the Camera.")]
	public class SetCameraCullingMask : ComponentAction<Camera>
	{
		// Token: 0x06006803 RID: 26627 RVA: 0x001E9F7C File Offset: 0x001E817C
		public override void Reset()
		{
			this.gameObject = null;
			this.cullingMask = new FsmInt[0];
			this.invertMask = false;
			this.everyFrame = false;
		}

		// Token: 0x06006804 RID: 26628 RVA: 0x001E9FAF File Offset: 0x001E81AF
		public override void OnEnter()
		{
			this.DoSetCameraCullingMask();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006805 RID: 26629 RVA: 0x001E9FC8 File Offset: 0x001E81C8
		public override void OnUpdate()
		{
			this.DoSetCameraCullingMask();
		}

		// Token: 0x06006806 RID: 26630 RVA: 0x001E9FD0 File Offset: 0x001E81D0
		private void DoSetCameraCullingMask()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.camera.cullingMask = ActionHelpers.LayerArrayToLayerMask(this.cullingMask, this.invertMask.Value);
			}
		}

		// Token: 0x04004FCE RID: 20430
		[CheckForComponent(typeof(Camera))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004FCF RID: 20431
		[UIHint(8)]
		[Tooltip("Cull these layers.")]
		public FsmInt[] cullingMask;

		// Token: 0x04004FD0 RID: 20432
		[Tooltip("Invert the mask, so you cull all layers except those defined above.")]
		public FsmBool invertMask;

		// Token: 0x04004FD1 RID: 20433
		public bool everyFrame;
	}
}
