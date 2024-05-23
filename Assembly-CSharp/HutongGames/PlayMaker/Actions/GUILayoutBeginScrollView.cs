using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BC6 RID: 3014
	[Tooltip("Begins a ScrollView. Use GUILayoutEndScrollView at the end of the block.")]
	[ActionCategory(26)]
	public class GUILayoutBeginScrollView : GUILayoutAction
	{
		// Token: 0x06006481 RID: 25729 RVA: 0x001DE636 File Offset: 0x001DC836
		public override void Reset()
		{
			base.Reset();
			this.scrollPosition = null;
			this.horizontalScrollbar = null;
			this.verticalScrollbar = null;
			this.useCustomStyle = null;
			this.horizontalStyle = null;
			this.verticalStyle = null;
			this.backgroundStyle = null;
		}

		// Token: 0x06006482 RID: 25730 RVA: 0x001DE670 File Offset: 0x001DC870
		public override void OnGUI()
		{
			if (this.useCustomStyle.Value)
			{
				this.scrollPosition.Value = GUILayout.BeginScrollView(this.scrollPosition.Value, this.horizontalScrollbar.Value, this.verticalScrollbar.Value, this.horizontalStyle.Value, this.verticalStyle.Value, this.backgroundStyle.Value, base.LayoutOptions);
			}
			else
			{
				this.scrollPosition.Value = GUILayout.BeginScrollView(this.scrollPosition.Value, this.horizontalScrollbar.Value, this.verticalScrollbar.Value, base.LayoutOptions);
			}
		}

		// Token: 0x04004C0B RID: 19467
		[UIHint(10)]
		[Tooltip("Assign a Vector2 variable to store the scroll position of this view.")]
		[RequiredField]
		public FsmVector2 scrollPosition;

		// Token: 0x04004C0C RID: 19468
		[Tooltip("Always show the horizontal scrollbars.")]
		public FsmBool horizontalScrollbar;

		// Token: 0x04004C0D RID: 19469
		[Tooltip("Always show the vertical scrollbars.")]
		public FsmBool verticalScrollbar;

		// Token: 0x04004C0E RID: 19470
		[Tooltip("Define custom styles below. NOTE: You have to define all the styles if you check this option.")]
		public FsmBool useCustomStyle;

		// Token: 0x04004C0F RID: 19471
		[Tooltip("Named style in the active GUISkin for the horizontal scrollbars.")]
		public FsmString horizontalStyle;

		// Token: 0x04004C10 RID: 19472
		[Tooltip("Named style in the active GUISkin for the vertical scrollbars.")]
		public FsmString verticalStyle;

		// Token: 0x04004C11 RID: 19473
		[Tooltip("Named style in the active GUISkin for the background.")]
		public FsmString backgroundStyle;
	}
}
