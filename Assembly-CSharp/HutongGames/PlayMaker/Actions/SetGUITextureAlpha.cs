using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CBF RID: 3263
	[ActionCategory(18)]
	[Tooltip("Sets the Alpha of the GUITexture attached to a Game Object. Useful for fading GUI elements in/out.")]
	public class SetGUITextureAlpha : ComponentAction<GUITexture>
	{
		// Token: 0x06006897 RID: 26775 RVA: 0x001EBAF3 File Offset: 0x001E9CF3
		public override void Reset()
		{
			this.gameObject = null;
			this.alpha = 1f;
			this.everyFrame = false;
		}

		// Token: 0x06006898 RID: 26776 RVA: 0x001EBB13 File Offset: 0x001E9D13
		public override void OnEnter()
		{
			this.DoGUITextureAlpha();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006899 RID: 26777 RVA: 0x001EBB2C File Offset: 0x001E9D2C
		public override void OnUpdate()
		{
			this.DoGUITextureAlpha();
		}

		// Token: 0x0600689A RID: 26778 RVA: 0x001EBB34 File Offset: 0x001E9D34
		private void DoGUITextureAlpha()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				Color color = base.guiTexture.color;
				base.guiTexture.color = new Color(color.r, color.g, color.b, this.alpha.Value);
			}
		}

		// Token: 0x0400506C RID: 20588
		[RequiredField]
		[CheckForComponent(typeof(GUITexture))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400506D RID: 20589
		[RequiredField]
		public FsmFloat alpha;

		// Token: 0x0400506E RID: 20590
		public bool everyFrame;
	}
}
