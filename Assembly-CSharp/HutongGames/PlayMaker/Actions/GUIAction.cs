using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BBA RID: 3002
	[Tooltip("GUI base action - don't use!")]
	public abstract class GUIAction : FsmStateAction
	{
		// Token: 0x0600645C RID: 25692 RVA: 0x001DDA30 File Offset: 0x001DBC30
		public override void Reset()
		{
			this.screenRect = null;
			this.left = 0f;
			this.top = 0f;
			this.width = 1f;
			this.height = 1f;
			this.normalized = true;
		}

		// Token: 0x0600645D RID: 25693 RVA: 0x001DDA90 File Offset: 0x001DBC90
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
		}

		// Token: 0x04004BD8 RID: 19416
		[UIHint(10)]
		public FsmRect screenRect;

		// Token: 0x04004BD9 RID: 19417
		public FsmFloat left;

		// Token: 0x04004BDA RID: 19418
		public FsmFloat top;

		// Token: 0x04004BDB RID: 19419
		public FsmFloat width;

		// Token: 0x04004BDC RID: 19420
		public FsmFloat height;

		// Token: 0x04004BDD RID: 19421
		[RequiredField]
		public FsmBool normalized;

		// Token: 0x04004BDE RID: 19422
		internal Rect rect;
	}
}
