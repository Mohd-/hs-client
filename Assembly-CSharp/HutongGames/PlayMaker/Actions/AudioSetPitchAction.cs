using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA6 RID: 3494
	[ActionCategory("Pegasus Audio")]
	[Tooltip("Sets the pitch of an AudioSource on a Game Object.")]
	public class AudioSetPitchAction : FsmStateAction
	{
		// Token: 0x06006CB5 RID: 27829 RVA: 0x001FFAC9 File Offset: 0x001FDCC9
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_Pitch = 1f;
			this.m_EveryFrame = false;
		}

		// Token: 0x06006CB6 RID: 27830 RVA: 0x001FFAE9 File Offset: 0x001FDCE9
		public override void OnEnter()
		{
			this.UpdatePitch();
			if (!this.m_EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CB7 RID: 27831 RVA: 0x001FFB02 File Offset: 0x001FDD02
		public override void OnUpdate()
		{
			this.UpdatePitch();
		}

		// Token: 0x06006CB8 RID: 27832 RVA: 0x001FFB0C File Offset: 0x001FDD0C
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
			if (this.m_Pitch.IsNone)
			{
				return;
			}
			SoundManager.Get().SetPitch(component, this.m_Pitch.Value);
		}

		// Token: 0x04005554 RID: 21844
		[CheckForComponent(typeof(AudioSource))]
		[RequiredField]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x04005555 RID: 21845
		[HasFloatSlider(-3f, 3f)]
		public FsmFloat m_Pitch;

		// Token: 0x04005556 RID: 21846
		public bool m_EveryFrame;
	}
}
