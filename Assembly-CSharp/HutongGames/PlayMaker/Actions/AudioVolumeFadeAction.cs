using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DAB RID: 3499
	[Tooltip("Fades an Audio Source component's volume towards a target value.")]
	[ActionCategory("Pegasus Audio")]
	public class AudioVolumeFadeAction : FsmStateAction
	{
		// Token: 0x06006CCE RID: 27854 RVA: 0x001FFED4 File Offset: 0x001FE0D4
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_FadeTime = 1f;
			this.m_TargetVolume = 0f;
			this.m_StopWhenDone = true;
			this.m_RealTime = false;
		}

		// Token: 0x06006CCF RID: 27855 RVA: 0x001FFF20 File Offset: 0x001FE120
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			this.m_audio = ownerDefaultTarget.GetComponent<AudioSource>();
			if (this.m_audio == null)
			{
				base.Finish();
				return;
			}
			if (this.m_FadeTime.Value <= Mathf.Epsilon)
			{
				base.Finish();
				return;
			}
			this.m_startVolume = SoundManager.Get().GetVolume(this.m_audio);
			this.m_startTime = FsmTime.RealtimeSinceStartup;
			this.m_currentTime = this.m_startTime;
			this.m_endTime = this.m_startTime + this.m_FadeTime.Value;
			SoundManager.Get().SetVolume(this.m_audio, this.m_startVolume);
		}

		// Token: 0x06006CD0 RID: 27856 RVA: 0x001FFFF0 File Offset: 0x001FE1F0
		public override void OnUpdate()
		{
			if (this.m_RealTime.Value)
			{
				this.m_currentTime = FsmTime.RealtimeSinceStartup - this.m_startTime;
			}
			else
			{
				this.m_currentTime += Time.deltaTime;
			}
			if (this.m_currentTime < this.m_endTime)
			{
				float num = (this.m_currentTime - this.m_startTime) / this.m_FadeTime.Value;
				float volume = Mathf.Lerp(this.m_startVolume, this.m_TargetVolume.Value, num);
				SoundManager.Get().SetVolume(this.m_audio, volume);
			}
			else
			{
				SoundManager.Get().SetVolume(this.m_audio, this.m_TargetVolume.Value);
				if (this.m_StopWhenDone.Value)
				{
					SoundManager.Get().Stop(this.m_audio);
				}
				base.Finish();
			}
		}

		// Token: 0x04005565 RID: 21861
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x04005566 RID: 21862
		[RequiredField]
		public FsmFloat m_FadeTime;

		// Token: 0x04005567 RID: 21863
		[RequiredField]
		public FsmFloat m_TargetVolume;

		// Token: 0x04005568 RID: 21864
		[Tooltip("Stop the audio source when the target volume is reached.")]
		public FsmBool m_StopWhenDone;

		// Token: 0x04005569 RID: 21865
		[Tooltip("Use real time. Useful if time is scaled and you don't want this action to scale.")]
		public FsmBool m_RealTime;

		// Token: 0x0400556A RID: 21866
		private AudioSource m_audio;

		// Token: 0x0400556B RID: 21867
		private float m_startVolume;

		// Token: 0x0400556C RID: 21868
		private float m_startTime;

		// Token: 0x0400556D RID: 21869
		private float m_currentTime;

		// Token: 0x0400556E RID: 21870
		private float m_endTime;
	}
}
