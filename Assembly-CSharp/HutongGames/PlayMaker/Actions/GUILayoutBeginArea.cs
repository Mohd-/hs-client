using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC2 RID: 3010
	[Tooltip("Begin a GUILayout block of GUI controls in a fixed screen area. NOTE: Block must end with a corresponding GUILayoutEndArea.")]
	[ActionCategory(26)]
	public class GUILayoutBeginArea : FsmStateAction
	{
		// Token: 0x06006474 RID: 25716 RVA: 0x001DE180 File Offset: 0x001DC380
		public override void Reset()
		{
			this.screenRect = null;
			this.left = 0f;
			this.top = 0f;
			this.width = 1f;
			this.height = 1f;
			this.normalized = true;
			this.style = string.Empty;
		}

		// Token: 0x06006475 RID: 25717 RVA: 0x001DE1F0 File Offset: 0x001DC3F0
		public override void OnGUI()
		{
			this.rect = (this.screenRect.IsNone ? default(Rect) : this.screenRect.Value);
			if (!this.left.IsNone)
			{
				this.rect.x = this.left.Value;
			}
			if (!this.top.IsNone)
			{
				this.rect.y = this.top.Value;
			}
			if (!this.width.IsNone)
			{
				this.rect.width = this.width.Value;
			}
			if (!this.height.IsNone)
			{
				this.rect.height = this.height.Value;
			}
			if (this.normalized.Value)
			{
				this.rect.x = this.rect.x * (float)Screen.width;
				this.rect.width = this.rect.width * (float)Screen.width;
				this.rect.y = this.rect.y * (float)Screen.height;
				this.rect.height = this.rect.height * (float)Screen.height;
			}
			GUILayout.BeginArea(this.rect, GUIContent.none, this.style.Value);
		}

		// Token: 0x04004BF8 RID: 19448
		[UIHint(10)]
		public FsmRect screenRect;

		// Token: 0x04004BF9 RID: 19449
		public FsmFloat left;

		// Token: 0x04004BFA RID: 19450
		public FsmFloat top;

		// Token: 0x04004BFB RID: 19451
		public FsmFloat width;

		// Token: 0x04004BFC RID: 19452
		public FsmFloat height;

		// Token: 0x04004BFD RID: 19453
		public FsmBool normalized;

		// Token: 0x04004BFE RID: 19454
		public FsmString style;

		// Token: 0x04004BFF RID: 19455
		private Rect rect;
	}
}
