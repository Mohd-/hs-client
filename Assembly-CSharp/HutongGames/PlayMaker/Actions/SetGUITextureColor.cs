using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC0 RID: 3264
	[ActionCategory(18)]
	[Tooltip("Sets the Color of the GUITexture attached to a Game Object.")]
	public class SetGUITextureColor : ComponentAction<GUITexture>
	{
		// Token: 0x0600689C RID: 26780 RVA: 0x001EBBA3 File Offset: 0x001E9DA3
		public override void Reset()
		{
			this.gameObject = null;
			this.color = Color.white;
			this.everyFrame = false;
		}

		// Token: 0x0600689D RID: 26781 RVA: 0x001EBBC3 File Offset: 0x001E9DC3
		public override void OnEnter()
		{
			this.DoSetGUITextureColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600689E RID: 26782 RVA: 0x001EBBDC File Offset: 0x001E9DDC
		public override void OnUpdate()
		{
			this.DoSetGUITextureColor();
		}

		// Token: 0x0600689F RID: 26783 RVA: 0x001EBBE4 File Offset: 0x001E9DE4
		private void DoSetGUITextureColor()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.guiTexture.color = this.color.Value;
			}
		}

		// Token: 0x0400506F RID: 20591
		[CheckForComponent(typeof(GUITexture))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005070 RID: 20592
		[RequiredField]
		public FsmColor color;

		// Token: 0x04005071 RID: 20593
		public bool everyFrame;
	}
}
