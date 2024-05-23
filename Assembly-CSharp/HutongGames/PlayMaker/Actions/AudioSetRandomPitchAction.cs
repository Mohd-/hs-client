using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA7 RID: 3495
	[ActionCategory("Pegasus Audio")]
	[Tooltip("Randomly sets the pitch of an AudioSource on a Game Object.")]
	public class AudioSetRandomPitchAction : FsmStateAction
	{
		// Token: 0x06006CBA RID: 27834 RVA: 0x001FFB7C File Offset: 0x001FDD7C
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_MinPitch = 1f;
			this.m_MaxPitch = 1f;
			this.m_EveryFrame = false;
		}

		// Token: 0x06006CBB RID: 27835 RVA: 0x001FFBB7 File Offset: 0x001FDDB7
		public override void OnEnter()
		{
			this.ChoosePitch();
			this.UpdatePitch();
			if (!this.m_EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CBC RID: 27836 RVA: 0x001FFBD6 File Offset: 0x001FDDD6
		public override void OnUpdate()
		{
			this.UpdatePitch();
		}

		// Token: 0x06006CBD RID: 27837 RVA: 0x001FFBE0 File Offset: 0x001FDDE0
		private void ChoosePitch()
		{
			float num = (!this.m_MinPitch.IsNone) ? this.m_MinPitch.Value : 1f;
			float num2 = (!this.m_MaxPitch.IsNone) ? this.m_MaxPitch.Value : 1f;
			this.m_pitch = Random.Range(num, num2);
		}

		// Token: 0x06006CBE RID: 27838 RVA: 0x001FFC48 File Offset: 0x001FDE48
		private void UpdatePitch()
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
			SoundManager.Get().SetPitch(component, this.m_pitch);
		}

		// Token: 0x04005557 RID: 21847
		[CheckForComponent(typeof(AudioSource))]
		[RequiredField]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x04005558 RID: 21848
		[HasFloatSlider(-3f, 3f)]
		public FsmFloat m_MinPitch;

		// Token: 0x04005559 RID: 21849
		[HasFloatSlider(-3f, 3f)]
		public FsmFloat m_MaxPitch;

		// Token: 0x0400555A RID: 21850
		public bool m_EveryFrame;

		// Token: 0x0400555B RID: 21851
		private float m_pitch;
	}
}
