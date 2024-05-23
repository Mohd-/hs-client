using System;
using System.IO;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CFE RID: 3326
	[ActionCategory(41)]
	[Tooltip("Saves a Screenshot to the users MyPictures folder. TIP: Can be useful for automated testing and debugging.")]
	public class TakeScreenshot : FsmStateAction
	{
		// Token: 0x060069B0 RID: 27056 RVA: 0x001EF536 File Offset: 0x001ED736
		public override void Reset()
		{
			this.filename = string.Empty;
			this.autoNumber = false;
		}

		// Token: 0x060069B1 RID: 27057 RVA: 0x001EF550 File Offset: 0x001ED750
		public override void OnEnter()
		{
			if (string.IsNullOrEmpty(this.filename.Value))
			{
				return;
			}
			string text = Environment.GetFolderPath(39) + "/";
			string text2 = text + this.filename.Value + ".png";
			if (this.autoNumber)
			{
				while (File.Exists(text2))
				{
					this.screenshotCount++;
					text2 = string.Concat(new object[]
					{
						text,
						this.filename.Value,
						this.screenshotCount,
						".png"
					});
				}
			}
			Application.CaptureScreenshot(text2);
			base.Finish();
		}

		// Token: 0x0400516D RID: 20845
		[RequiredField]
		public FsmString filename;

		// Token: 0x0400516E RID: 20846
		public bool autoNumber;

		// Token: 0x0400516F RID: 20847
		private int screenshotCount;
	}
}
