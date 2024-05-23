using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA2 RID: 3490
	[Tooltip("Plays a random AudioClip. An AudioSource for the clip is created automatically based on the parameters.")]
	[ActionCategory("Pegasus Audio")]
	public class AudioPlayRandomClipAction : FsmStateAction
	{
		// Token: 0x06006CA5 RID: 27813 RVA: 0x001FF5D8 File Offset: 0x001FD7D8
		public override void Reset()
		{
			this.m_ParentObject = null;
			this.m_Clips = new AudioClip[3];
			this.m_Weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.m_MinVolume = 1f;
			this.m_MaxVolume = 1f;
			this.m_MinPitch = 1f;
			this.m_MaxPitch = 1f;
			this.m_SpatialBlend = 1f;
			this.m_Category = SoundCategory.FX;
			this.m_TemplateSource = null;
		}

		// Token: 0x06006CA6 RID: 27814 RVA: 0x001FF68C File Offset: 0x001FD88C
		public override void OnEnter()
		{
			if (this.m_Clips == null || this.m_Clips.Length == 0)
			{
				base.Finish();
				return;
			}
			SoundPlayClipArgs soundPlayClipArgs = new SoundPlayClipArgs();
			soundPlayClipArgs.m_templateSource = this.m_TemplateSource;
			soundPlayClipArgs.m_clip = this.ChooseClip();
			soundPlayClipArgs.m_volume = new float?(this.ChooseVolume());
			soundPlayClipArgs.m_pitch = new float?(this.ChoosePitch());
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

		// Token: 0x06006CA7 RID: 27815 RVA: 0x001FF763 File Offset: 0x001FD963
		private AudioClip ChooseClip()
		{
			if (this.m_Weights == null || this.m_Weights.Length == 0)
			{
				return this.m_Clips[0];
			}
			return this.m_Clips[ActionHelpers.GetRandomWeightedIndex(this.m_Weights)];
		}

		// Token: 0x06006CA8 RID: 27816 RVA: 0x001FF798 File Offset: 0x001FD998
		private float ChooseVolume()
		{
			float num = (!this.m_MinVolume.IsNone) ? this.m_MinVolume.Value : 1f;
			float num2 = (!this.m_MaxVolume.IsNone) ? this.m_MaxVolume.Value : 1f;
			if (object.Equals(num, num2))
			{
				return num;
			}
			return Random.Range(num, num2);
		}

		// Token: 0x06006CA9 RID: 27817 RVA: 0x001FF810 File Offset: 0x001FDA10
		private float ChoosePitch()
		{
			float num = (!this.m_MinPitch.IsNone) ? this.m_MinPitch.Value : 1f;
			float num2 = (!this.m_MaxPitch.IsNone) ? this.m_MaxPitch.Value : 1f;
			if (object.Equals(num, num2))
			{
				return num;
			}
			return Random.Range(num, num2);
		}

		// Token: 0x04005542 RID: 21826
		[Tooltip("Optional. If specified, the generated Audio Source will use the same transform as this object.")]
		public FsmOwnerDefault m_ParentObject;

		// Token: 0x04005543 RID: 21827
		[CompoundArray("Audio Clips", "Audio Clip", "Weight")]
		[RequiredField]
		public AudioClip[] m_Clips;

		// Token: 0x04005544 RID: 21828
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] m_Weights;

		// Token: 0x04005545 RID: 21829
		[HasFloatSlider(0f, 1f)]
		public FsmFloat m_MinVolume;

		// Token: 0x04005546 RID: 21830
		[HasFloatSlider(0f, 1f)]
		public FsmFloat m_MaxVolume;

		// Token: 0x04005547 RID: 21831
		[HasFloatSlider(-3f, 3f)]
		public FsmFloat m_MinPitch;

		// Token: 0x04005548 RID: 21832
		[HasFloatSlider(-3f, 3f)]
		public FsmFloat m_MaxPitch;

		// Token: 0x04005549 RID: 21833
		[HasFloatSlider(0f, 1f)]
		public FsmFloat m_SpatialBlend;

		// Token: 0x0400554A RID: 21834
		public SoundCategory m_Category;

		// Token: 0x0400554B RID: 21835
		[Tooltip("If specified, this Audio Source will be used as a template for the generated Audio Source, otherwise the one in the SoundConfig will be the template.")]
		public AudioSource m_TemplateSource;
	}
}
