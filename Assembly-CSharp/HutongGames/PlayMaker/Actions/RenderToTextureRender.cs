using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DD4 RID: 3540
	[Tooltip("Triggers a render to texture to render.")]
	[ActionCategory("Pegasus")]
	public class RenderToTextureRender : FsmStateAction
	{
		// Token: 0x06006D6B RID: 28011 RVA: 0x00202AA0 File Offset: 0x00200CA0
		public override void Reset()
		{
			this.r2tObject = null;
			this.now = false;
		}

		// Token: 0x06006D6C RID: 28012 RVA: 0x00202AB0 File Offset: 0x00200CB0
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.r2tObject);
			if (ownerDefaultTarget != null)
			{
				RenderToTexture component = ownerDefaultTarget.GetComponent<RenderToTexture>();
				if (component != null)
				{
					if (this.now)
					{
						component.RenderNow();
					}
					else
					{
						component.Render();
					}
				}
			}
			base.Finish();
		}

		// Token: 0x0400561A RID: 22042
		[RequiredField]
		public FsmOwnerDefault r2tObject;

		// Token: 0x0400561B RID: 22043
		public bool now;
	}
}
