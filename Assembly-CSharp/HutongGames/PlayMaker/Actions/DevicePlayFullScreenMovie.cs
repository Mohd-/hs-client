using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B92 RID: 2962
	[Tooltip("Plays a full-screen movie on a handheld device. Please consult the Unity docs for Handheld.PlayFullScreenMovie for proper usage.")]
	[ActionCategory(33)]
	public class DevicePlayFullScreenMovie : FsmStateAction
	{
		// Token: 0x060063B9 RID: 25529 RVA: 0x001DBB7E File Offset: 0x001D9D7E
		public override void Reset()
		{
			this.RemindMeAtRuntime = true;
		}

		// Token: 0x060063BA RID: 25530 RVA: 0x001DBB87 File Offset: 0x001D9D87
		public override void OnEnter()
		{
			if (this.RemindMeAtRuntime)
			{
				Debug.LogWarning("Current platform is not iOS or Android, DevicePlayFullScreenMovie action only works for iOS and Android");
			}
		}

		// Token: 0x04004B2D RID: 19245
		[Tooltip("Note that player will stream movie directly from the iPhone disc, therefore you have to provide movie as a separate files and not as an usual asset.\nYou will have to create a folder named StreamingAssets inside your Unity project (inside your Assets folder). Store your movies inside that folder. Unity will automatically copy contents of that folder into the iPhone application bundle.")]
		[RequiredField]
		public FsmString moviePath;

		// Token: 0x04004B2E RID: 19246
		[Tooltip("This action will initiate a transition that fades the screen from your current content to the designated background color of the player. When playback finishes, the player uses another fade effect to transition back to your content.")]
		[RequiredField]
		public FsmColor fadeColor;

		// Token: 0x04004B2F RID: 19247
		[ActionSection("Current platform is not iOS or Android")]
		public bool RemindMeAtRuntime;
	}
}
