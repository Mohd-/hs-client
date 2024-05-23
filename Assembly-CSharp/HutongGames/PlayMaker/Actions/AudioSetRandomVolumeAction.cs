using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA8 RID: 3496
	[Tooltip("Randomly sets the volume of an AudioSource on a Game Object.")]
	[ActionCategory("Pegasus Audio")]
	public class AudioSetRandomVolumeAction : FsmStateAction
	{
		// Token: 0x06006CC0 RID: 27840 RVA: 0x001FFCA4 File Offset: 0x001FDEA4
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_MinVolume = 1f;
			this.m_MaxVolume = 1f;
			this.m_EveryFrame = false;
		}

		// Token: 0x06006CC1 RID: 27841 RVA: 0x001FFCDF File Offset: 0x001FDEDF
		public override void OnEnter()
		{
			this.UpdateVolume();
			if (!this.m_EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CC2 RID: 27842 RVA: 0x001FFCF8 File Offset: 0x001FDEF8
		public override void OnUpdate()
		{
			this.UpdateVolume();
		}

		// Token: 0x06006CC3 RID: 27843 RVA: 0x001FFD00 File Offset: 0x001FDF00
		private void ChooseVolume()
		{
			float num = (!this.m_MinVolume.IsNone) ? this.m_MinVolume.Value : 1f;
			float num2 = (!this.m_MaxVolume.IsNone) ? this.m_MaxVolume.Value : 1f;
			this.m_volume = Random.Range(num, num2);
		}

		// Token: 0x06006CC4 RID: 27844 RVA: 0x001FFD68 File Offset: 0x001FDF68
		private void UpdateVolume()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
			if (component == null)
			{
				return;
			}
			SoundManager.Get().SetVolume(component, this.m_volume);
		}

		// Token: 0x0400555C RID: 21852
		[CheckForComponent(typeof(AudioSource))]
		[RequiredField]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x0400555D RID: 21853
		[HasFloatSlider(0f, 1f)]
		public FsmFloat m_MinVolume;

		// Token: 0x0400555E RID: 21854
		[HasFloatSlider(0f, 1f)]
		public FsmFloat m_MaxVolume;

		// Token: 0x0400555F RID: 21855
		public bool m_EveryFrame;

		// Token: 0x04005560 RID: 21856
		private float m_volume;
	}
}
