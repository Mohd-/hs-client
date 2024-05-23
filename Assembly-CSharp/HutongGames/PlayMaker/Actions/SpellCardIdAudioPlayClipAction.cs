using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DEC RID: 3564
	[Tooltip("Generates an AudioSource based on a template, then plays that source depending on a Spell's Card's ID.")]
	[ActionCategory("Pegasus Audio")]
	public class SpellCardIdAudioPlayClipAction : SpellCardIdAudioAction
	{
		// Token: 0x06006DCC RID: 28108 RVA: 0x00204159 File Offset: 0x00202359
		protected override GameObject GetSpellOwner()
		{
			return base.Fsm.GetOwnerDefaultTarget(this.m_SpellObject);
		}

		// Token: 0x06006DCD RID: 28109 RVA: 0x0020416C File Offset: 0x0020236C
		public override void Reset()
		{
			this.m_SpellObject = null;
			this.m_WhichCard = SpellAction.Which.SOURCE;
			this.m_TemplateSource = null;
			this.m_CardIds = new string[2];
			this.m_Clips = new AudioClip[2];
			this.m_DefaultClip = null;
			this.m_Volume = 1f;
			this.m_Pitch = 1f;
			this.m_Category = SoundCategory.FX;
		}

		// Token: 0x06006DCE RID: 28110 RVA: 0x002041D4 File Offset: 0x002023D4
		public override void OnEnter()
		{
			base.OnEnter();
			AudioClip clipMatchingCardId = base.GetClipMatchingCardId(this.m_WhichCard, this.m_CardIds, this.m_Clips, this.m_DefaultClip);
			if (clipMatchingCardId == null)
			{
				base.Finish();
				return;
			}
			SoundPlayClipArgs soundPlayClipArgs = new SoundPlayClipArgs();
			soundPlayClipArgs.m_templateSource = this.m_TemplateSource;
			soundPlayClipArgs.m_clip = clipMatchingCardId;
			if (!this.m_Volume.IsNone)
			{
				soundPlayClipArgs.m_volume = new float?(this.m_Volume.Value);
			}
			if (!this.m_Pitch.IsNone)
			{
				soundPlayClipArgs.m_pitch = new float?(this.m_Pitch.Value);
			}
			if (this.m_Category != SoundCategory.NONE)
			{
				soundPlayClipArgs.m_category = new SoundCategory?(this.m_Category);
			}
			soundPlayClipArgs.m_parentObject = base.Owner;
			SoundManager.Get().PlayClip(soundPlayClipArgs);
			base.Finish();
		}

		// Token: 0x04005676 RID: 22134
		public FsmOwnerDefault m_SpellObject;

		// Token: 0x04005677 RID: 22135
		[Tooltip("Which Card to check on the Spell.")]
		public SpellAction.Which m_WhichCard;

		// Token: 0x04005678 RID: 22136
		[Tooltip("If specified, this Audio Source will be used as a template for the generated Audio Source, otherwise the one in the SoundConfig will be the template.")]
		public AudioSource m_TemplateSource;

		// Token: 0x04005679 RID: 22137
		[CompoundArray("Clips", "Card Id", "Clip")]
		public string[] m_CardIds;

		// Token: 0x0400567A RID: 22138
		public AudioClip[] m_Clips;

		// Token: 0x0400567B RID: 22139
		public AudioClip m_DefaultClip;

		// Token: 0x0400567C RID: 22140
		[HasFloatSlider(0f, 1f)]
		public FsmFloat m_Volume;

		// Token: 0x0400567D RID: 22141
		[HasFloatSlider(-3f, 3f)]
		public FsmFloat m_Pitch;

		// Token: 0x0400567E RID: 22142
		[Tooltip("If you want the template Category the Category, change this so that it's not NONE.")]
		public SoundCategory m_Category;
	}
}
