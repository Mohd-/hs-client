using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DEB RID: 3563
	[Tooltip("Plays the Audio Source of a Game Object depending on a Spell's Card's ID.")]
	[ActionCategory("Pegasus Audio")]
	public class SpellCardIdAudioPlayAction : SpellCardIdAudioAction
	{
		// Token: 0x06006DC7 RID: 28103 RVA: 0x00203FCE File Offset: 0x002021CE
		protected override GameObject GetSpellOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_SpellObject);
		}

		// Token: 0x06006DC8 RID: 28104 RVA: 0x00203FE4 File Offset: 0x002021E4
		public override void Reset()
		{
			this.m_SpellObject = null;
			this.m_WhichCard = SpellAction.Which.SOURCE;
			this.m_AudioSourceObject = null;
			this.m_CardIds = new string[2];
			this.m_Clips = new AudioClip[2];
			this.m_DefaultClip = null;
			this.m_VolumeScale = 1f;
			this.m_WaitForFinish = false;
		}

		// Token: 0x06006DC9 RID: 28105 RVA: 0x00204044 File Offset: 0x00202244
		public override void OnEnter()
		{
			base.OnEnter();
			AudioSource audioSource = base.GetAudioSource(this.m_AudioSourceObject);
			if (audioSource == null)
			{
				base.Finish();
				return;
			}
			AudioClip clipMatchingCardId = base.GetClipMatchingCardId(this.m_WhichCard, this.m_CardIds, this.m_Clips, this.m_DefaultClip);
			if (clipMatchingCardId == null)
			{
				base.Finish();
				return;
			}
			if (this.m_VolumeScale.IsNone)
			{
				SoundManager.Get().PlayOneShot(audioSource, clipMatchingCardId, 1f);
			}
			else
			{
				SoundManager.Get().PlayOneShot(audioSource, clipMatchingCardId, this.m_VolumeScale.Value);
			}
			if (!this.m_WaitForFinish.Value || !SoundManager.Get().IsActive(audioSource))
			{
				base.Finish();
			}
		}

		// Token: 0x06006DCA RID: 28106 RVA: 0x00204110 File Offset: 0x00202310
		public override void OnUpdate()
		{
			AudioSource audioSource = base.GetAudioSource(this.m_AudioSourceObject);
			if (this.m_WaitForFinish.Value && SoundManager.Get().IsActive(audioSource))
			{
				return;
			}
			base.Finish();
		}

		// Token: 0x0400566E RID: 22126
		public FsmOwnerDefault m_SpellObject;

		// Token: 0x0400566F RID: 22127
		[Tooltip("Which Card to check on the Spell.")]
		public SpellAction.Which m_WhichCard;

		// Token: 0x04005670 RID: 22128
		[Tooltip("The GameObject with the AudioSource component.")]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault m_AudioSourceObject;

		// Token: 0x04005671 RID: 22129
		[CompoundArray("Clips", "Card Id", "Clip")]
		[RequiredField]
		public string[] m_CardIds;

		// Token: 0x04005672 RID: 22130
		public AudioClip[] m_Clips;

		// Token: 0x04005673 RID: 22131
		public AudioClip m_DefaultClip;

		// Token: 0x04005674 RID: 22132
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Scales the volume of the AudioSource just for this Play call.")]
		public FsmFloat m_VolumeScale;

		// Token: 0x04005675 RID: 22133
		[Tooltip("Wait for the Audio Source to finish playing before moving on.")]
		public FsmBool m_WaitForFinish;
	}
}
