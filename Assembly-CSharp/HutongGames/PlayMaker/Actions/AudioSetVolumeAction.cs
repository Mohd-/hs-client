using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA9 RID: 3497
	[ActionCategory("Pegasus Audio")]
	[Tooltip("Sets the volume of an AudioSource on a Game Object.")]
	public class AudioSetVolumeAction : FsmStateAction
	{
		// Token: 0x06006CC6 RID: 27846 RVA: 0x001FFDC1 File Offset: 0x001FDFC1
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_Volume = 1f;
			this.m_EveryFrame = false;
		}

		// Token: 0x06006CC7 RID: 27847 RVA: 0x001FFDE1 File Offset: 0x001FDFE1
		public override void OnEnter()
		{
			this.UpdateVolume();
			if (!this.m_EveryFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006CC8 RID: 27848 RVA: 0x001FFDFA File Offset: 0x001FDFFA
		public override void OnUpdate()
		{
			this.UpdateVolume();
		}

		// Token: 0x06006CC9 RID: 27849 RVA: 0x001FFE04 File Offset: 0x001FE004
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
			if (this.m_Volume.IsNone)
			{
				return;
			}
			SoundManager.Get().SetVolume(component, this.m_Volume.Value);
		}

		// Token: 0x04005561 RID: 21857
		[CheckForComponent(typeof(AudioSource))]
		[RequiredField]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x04005562 RID: 21858
		[HasFloatSlider(0f, 1f)]
		public FsmFloat m_Volume;

		// Token: 0x04005563 RID: 21859
		public bool m_EveryFrame;
	}
}
