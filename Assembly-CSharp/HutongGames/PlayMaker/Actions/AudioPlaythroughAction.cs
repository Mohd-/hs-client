using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA3 RID: 3491
	[Tooltip("Plays the Audio Clip in the Audio Source on a Game Object or plays a one shot clip. Does not wait for the audio to finish.")]
	[ActionCategory("Pegasus Audio")]
	public class AudioPlaythroughAction : FsmStateAction
	{
		// Token: 0x06006CAB RID: 27819 RVA: 0x001FF890 File Offset: 0x001FDA90
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_VolumeScale = 1f;
			this.m_OneShotClip = null;
		}

		// Token: 0x06006CAC RID: 27820 RVA: 0x001FF8B0 File Offset: 0x001FDAB0
		public override void OnEnter()
		{
			AudioSource source = this.GetSource();
			if (source == null)
			{
				base.Fsm.Event(this.m_FinishedEvent);
				base.Finish();
				return;
			}
			AudioClip audioClip = this.m_OneShotClip.Value as AudioClip;
			if (audioClip == null)
			{
				if (!this.m_VolumeScale.IsNone)
				{
					SoundManager.Get().SetVolume(source, this.m_VolumeScale.Value);
				}
				SoundManager.Get().Play(source);
			}
			else
			{
				SoundPlayClipArgs soundPlayClipArgs = new SoundPlayClipArgs();
				soundPlayClipArgs.m_templateSource = source;
				soundPlayClipArgs.m_clip = audioClip;
				if (!this.m_VolumeScale.IsNone)
				{
					soundPlayClipArgs.m_volume = new float?(this.m_VolumeScale.Value);
				}
				soundPlayClipArgs.m_parentObject = source.gameObject;
				SoundManager.Get().PlayClip(soundPlayClipArgs);
			}
			base.Fsm.Event(this.m_FinishedEvent);
			base.Finish();
		}

		// Token: 0x06006CAD RID: 27821 RVA: 0x001FF9A8 File Offset: 0x001FDBA8
		private AudioSource GetSource()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget == null)
			{
				return null;
			}
			return ownerDefaultTarget.GetComponent<AudioSource>();
		}

		// Token: 0x0400554C RID: 21836
		[CheckForComponent(typeof(AudioSource))]
		[RequiredField]
		[Tooltip("The GameObject with the AudioSource component.")]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x0400554D RID: 21837
		[Tooltip("Scales the volume of the AudioSource just for this Play call.")]
		[HasFloatSlider(0f, 1f)]
		public FsmFloat m_VolumeScale;

		// Token: 0x0400554E RID: 21838
		[Tooltip("Optionally play a one shot AudioClip.")]
		[ObjectType(typeof(AudioClip))]
		public FsmObject m_OneShotClip;

		// Token: 0x0400554F RID: 21839
		[Tooltip("Event to send when the AudioClip finished playing.")]
		public FsmEvent m_FinishedEvent;
	}
}
