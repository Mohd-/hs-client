using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CBE RID: 3262
	[ActionCategory(18)]
	[Tooltip("Sets the Texture used by the GUITexture attached to a Game Object.")]
	public class SetGUITexture : ComponentAction<GUITexture>
	{
		// Token: 0x06006894 RID: 26772 RVA: 0x001EBA91 File Offset: 0x001E9C91
		public override void Reset()
		{
			this.gameObject = null;
			this.texture = null;
		}

		// Token: 0x06006895 RID: 26773 RVA: 0x001EBAA4 File Offset: 0x001E9CA4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.guiTexture.texture = this.texture.Value;
			}
			base.Finish();
		}

		// Token: 0x0400506A RID: 20586
		[CheckForComponent(typeof(GUITexture))]
		[RequiredField]
		[Tooltip("The GameObject that owns the GUITexture.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400506B RID: 20587
		[Tooltip("Texture to apply.")]
		public FsmTexture texture;
	}
}
