using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C9B RID: 3227
	[Tooltip("Sets the Volume of the Audio Clip played by the AudioSource component on a Game Object.")]
	[ActionCategory(1)]
	public class SetAudioVolume : ComponentAction<AudioSource>
	{
		// Token: 0x060067F5 RID: 26613 RVA: 0x001E9DF1 File Offset: 0x001E7FF1
		public override void Reset()
		{
			this.gameObject = null;
			this.volume = 1f;
			this.everyFrame = false;
		}

		// Token: 0x060067F6 RID: 26614 RVA: 0x001E9E11 File Offset: 0x001E8011
		public override void OnEnter()
		{
			this.DoSetAudioVolume();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060067F7 RID: 26615 RVA: 0x001E9E2A File Offset: 0x001E802A
		public override void OnUpdate()
		{
			this.DoSetAudioVolume();
		}

		// Token: 0x060067F8 RID: 26616 RVA: 0x001E9E34 File Offset: 0x001E8034
		private void DoSetAudioVolume()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget) && !this.volume.IsNone)
			{
				base.audio.volume = this.volume.Value;
			}
		}

		// Token: 0x04004FC5 RID: 20421
		[CheckForComponent(typeof(AudioSource))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004FC6 RID: 20422
		[HasFloatSlider(0f, 1f)]
		public FsmFloat volume;

		// Token: 0x04004FC7 RID: 20423
		public bool everyFrame;
	}
}
