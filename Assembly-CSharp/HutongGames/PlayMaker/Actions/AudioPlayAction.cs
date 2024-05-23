using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA0 RID: 3488
	[ActionCategory("Pegasus Audio")]
	[Tooltip("Plays the Audio Source of a Game Object or plays a one shot clip. Waits for the audio to finish.")]
	public class AudioPlayAction : FsmStateAction
	{
		// Token: 0x06006C9D RID: 27805 RVA: 0x001FF2D4 File Offset: 0x001FD4D4
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_VolumeScale = 1f;
			this.m_OneShotClip = null;
		}

		// Token: 0x06006C9E RID: 27806 RVA: 0x001FF2F4 File Offset: 0x001FD4F4
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
			if (!SoundManager.Get().IsActive(source))
			{
				base.Fsm.Event(this.m_FinishedEvent);
				base.Finish();
			}
		}

		// Token: 0x06006C9F RID: 27807 RVA: 0x001FF3FC File Offset: 0x001FD5FC
		public override void OnUpdate()
		{
			AudioSource source = this.GetSource();
			if (SoundManager.Get().IsActive(source))
			{
				return;
			}
			base.Fsm.Event(this.m_FinishedEvent);
			base.Finish();
		}

		// Token: 0x06006CA0 RID: 27808 RVA: 0x001FF438 File Offset: 0x001FD638
		private AudioSource GetSource()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget == null)
			{
				return null;
			}
			return ownerDefaultTarget.GetComponent<AudioSource>();
		}

		// Token: 0x04005537 RID: 21815
		[Tooltip("The GameObject with the AudioSource component.")]
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x04005538 RID: 21816
		[Tooltip("Scales the volume of the AudioSource just for this Play call.")]
		[HasFloatSlider(0f, 1f)]
		public FsmFloat m_VolumeScale;

		// Token: 0x04005539 RID: 21817
		[Tooltip("Optionally play a One Shot AudioClip.")]
		[ObjectType(typeof(AudioClip))]
		public FsmObject m_OneShotClip;

		// Token: 0x0400553A RID: 21818
		[Tooltip("Event to send when the AudioSource finishes playing.")]
		public FsmEvent m_FinishedEvent;
	}
}
