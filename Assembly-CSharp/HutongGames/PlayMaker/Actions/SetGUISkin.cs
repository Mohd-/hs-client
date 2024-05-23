using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CBC RID: 3260
	[Tooltip("Sets the GUISkin used by GUI elements.")]
	[ActionCategory(5)]
	public class SetGUISkin : FsmStateAction
	{
		// Token: 0x0600688C RID: 26764 RVA: 0x001EB9AF File Offset: 0x001E9BAF
		public override void Reset()
		{
			this.skin = null;
			this.applyGlobally = true;
		}

		// Token: 0x0600688D RID: 26765 RVA: 0x001EB9C4 File Offset: 0x001E9BC4
		public override void OnGUI()
		{
			if (this.skin != null)
			{
				GUI.skin = this.skin;
			}
			if (this.applyGlobally.Value)
			{
				PlayMakerGUI.GUISkin = this.skin;
				base.Finish();
			}
		}

		// Token: 0x04005065 RID: 20581
		[RequiredField]
		public GUISkin skin;

		// Token: 0x04005066 RID: 20582
		public FsmBool applyGlobally;
	}
}
