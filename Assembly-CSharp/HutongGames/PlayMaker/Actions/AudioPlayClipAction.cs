using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA1 RID: 3489
	[ActionCategory("Pegasus Audio")]
	[Tooltip("Generates an AudioSource based on a template, then plays that source.")]
	public class AudioPlayClipAction : FsmStateAction
	{
		// Token: 0x06006CA2 RID: 27810 RVA: 0x001FF474 File Offset: 0x001FD674
		public override void Reset()
		{
			this.m_ParentObject = null;
			this.m_Clip = null;
			this.m_Volume = 1f;
			this.m_Pitch = 1f;
			this.m_SpatialBlend = 1f;
			this.m_Category = SoundCategory.FX;
			this.m_TemplateSource = null;
		}

		// Token: 0x06006CA3 RID: 27811 RVA: 0x001FF4D0 File Offset: 0x001FD6D0
		public override void OnEnter()
		{
			if (this.m_Clip == null)
			{
				base.Finish();
				return;
			}
			SoundPlayClipArgs soundPlayClipArgs = new SoundPlayClipArgs();
			soundPlayClipArgs.m_templateSource = this.m_TemplateSource;
			soundPlayClipArgs.m_clip = (this.m_Clip.Value as AudioClip);
			if (!this.m_Volume.IsNone)
			{
				soundPlayClipArgs.m_volume = new float?(this.m_Volume.Value);
			}
			if (!this.m_Pitch.IsNone)
			{
				soundPlayClipArgs.m_pitch = new float?(this.m_Pitch.Value);
			}
			if (!this.m_SpatialBlend.IsNone)
			{
				soundPlayClipArgs.m_spatialBlend = new float?(this.m_SpatialBlend.Value);
			}
			if (this.m_Category != SoundCategory.NONE)
			{
				soundPlayClipArgs.m_category = new SoundCategory?(this.m_Category);
			}
			soundPlayClipArgs.m_parentObject = base.Fsm.GetOwnerDefaultTarget(this.m_ParentObject);
			SoundManager.Get().PlayClip(soundPlayClipArgs);
			base.Finish();
		}

		// Token: 0x0400553B RID: 21819
		[Tooltip("Optional. If specified, the generated Audio Source will be placed at the same location as this object.")]
		public FsmOwnerDefault m_ParentObject;

		// Token: 0x0400553C RID: 21820
		[ObjectType(typeof(AudioClip))]
		[RequiredField]
		public FsmObject m_Clip;

		// Token: 0x0400553D RID: 21821
		[HasFloatSlider(0f, 1f)]
		public FsmFloat m_Volume;

		// Token: 0x0400553E RID: 21822
		[HasFloatSlider(-3f, 3f)]
		public FsmFloat m_Pitch;

		// Token: 0x0400553F RID: 21823
		[HasFloatSlider(0f, 1f)]
		public FsmFloat m_SpatialBlend;

		// Token: 0x04005540 RID: 21824
		public SoundCategory m_Category;

		// Token: 0x04005541 RID: 21825
		[Tooltip("If specified, this Audio Source will be used as a template for the generated Audio Source, otherwise the one in the SoundConfig will be the template.")]
		public AudioSource m_TemplateSource;
	}
}
