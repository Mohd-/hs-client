using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C9A RID: 3226
	[ActionCategory(1)]
	[Tooltip("Sets the Pitch of the Audio Clip played by the AudioSource component on a Game Object.")]
	public class SetAudioPitch : ComponentAction<AudioSource>
	{
		// Token: 0x060067F0 RID: 26608 RVA: 0x001E9D57 File Offset: 0x001E7F57
		public override void Reset()
		{
			this.gameObject = null;
			this.pitch = 1f;
			this.everyFrame = false;
		}

		// Token: 0x060067F1 RID: 26609 RVA: 0x001E9D77 File Offset: 0x001E7F77
		public override void OnEnter()
		{
			this.DoSetAudioPitch();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067F2 RID: 26610 RVA: 0x001E9D90 File Offset: 0x001E7F90
		public override void OnUpdate()
		{
			this.DoSetAudioPitch();
		}

		// Token: 0x060067F3 RID: 26611 RVA: 0x001E9D98 File Offset: 0x001E7F98
		private void DoSetAudioPitch()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget) && !this.pitch.IsNone)
			{
				base.audio.pitch = this.pitch.Value;
			}
		}

		// Token: 0x04004FC2 RID: 20418
		[CheckForComponent(typeof(AudioSource))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004FC3 RID: 20419
		public FsmFloat pitch;

		// Token: 0x04004FC4 RID: 20420
		public bool everyFrame;
	}
}
