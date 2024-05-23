using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C9F RID: 3231
	[ActionCategory(22)]
	[Tooltip("Sets Field of View used by the Camera.")]
	public class SetCameraFOV : ComponentAction<Camera>
	{
		// Token: 0x06006808 RID: 26632 RVA: 0x001EA024 File Offset: 0x001E8224
		public override void Reset()
		{
			this.gameObject = null;
			this.fieldOfView = 50f;
			this.everyFrame = false;
		}

		// Token: 0x06006809 RID: 26633 RVA: 0x001EA044 File Offset: 0x001E8244
		public override void OnEnter()
		{
			this.DoSetCameraFOV();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600680A RID: 26634 RVA: 0x001EA05D File Offset: 0x001E825D
		public override void OnUpdate()
		{
			this.DoSetCameraFOV();
		}

		// Token: 0x0600680B RID: 26635 RVA: 0x001EA068 File Offset: 0x001E8268
		private void DoSetCameraFOV()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.camera.fieldOfView = this.fieldOfView.Value;
			}
		}

		// Token: 0x04004FD2 RID: 20434
		[RequiredField]
		[CheckForComponent(typeof(Camera))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004FD3 RID: 20435
		[RequiredField]
		public FsmFloat fieldOfView;

		// Token: 0x04004FD4 RID: 20436
		public bool everyFrame;
	}
}
