using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C9C RID: 3228
	[ActionCategory(22)]
	[Tooltip("Sets the Background Color used by the Camera.")]
	public class SetBackgroundColor : ComponentAction<Camera>
	{
		// Token: 0x060067FA RID: 26618 RVA: 0x001E9E8D File Offset: 0x001E808D
		public override void Reset()
		{
			this.gameObject = null;
			this.backgroundColor = Color.black;
			this.everyFrame = false;
		}

		// Token: 0x060067FB RID: 26619 RVA: 0x001E9EAD File Offset: 0x001E80AD
		public override void OnEnter()
		{
			this.DoSetBackgroundColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067FC RID: 26620 RVA: 0x001E9EC6 File Offset: 0x001E80C6
		public override void OnUpdate()
		{
			this.DoSetBackgroundColor();
		}

		// Token: 0x060067FD RID: 26621 RVA: 0x001E9ED0 File Offset: 0x001E80D0
		private void DoSetBackgroundColor()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.camera.backgroundColor = this.backgroundColor.Value;
			}
		}

		// Token: 0x04004FC8 RID: 20424
		[CheckForComponent(typeof(Camera))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004FC9 RID: 20425
		[RequiredField]
		public FsmColor backgroundColor;

		// Token: 0x04004FCA RID: 20426
		public bool everyFrame;
	}
}
