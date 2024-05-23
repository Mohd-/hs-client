using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC2 RID: 3266
	[ActionCategory(1)]
	[Tooltip("Sets the global sound volume.")]
	public class SetGameVolume : FsmStateAction
	{
		// Token: 0x060068A5 RID: 26789 RVA: 0x001EBC8D File Offset: 0x001E9E8D
		public override void Reset()
		{
			this.volume = 1f;
			this.everyFrame = false;
		}

		// Token: 0x060068A6 RID: 26790 RVA: 0x001EBCA6 File Offset: 0x001E9EA6
		public override void OnEnter()
		{
			AudioListener.volume = this.volume.Value;
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068A7 RID: 26791 RVA: 0x001EBCC9 File Offset: 0x001E9EC9
		public override void OnUpdate()
		{
			AudioListener.volume = this.volume.Value;
		}

		// Token: 0x04005075 RID: 20597
		[HasFloatSlider(0f, 1f)]
		[RequiredField]
		public FsmFloat volume;

		// Token: 0x04005076 RID: 20598
		public bool everyFrame;
	}
}
