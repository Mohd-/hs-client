using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA5 RID: 3493
	[Tooltip("Mute/unmute the Audio Source on a Game Object.")]
	[ActionCategory("Pegasus Audio")]
	public class AudioSetMuteAction : FsmStateAction
	{
		// Token: 0x06006CB2 RID: 27826 RVA: 0x001FFA55 File Offset: 0x001FDC55
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_Mute = false;
		}

		// Token: 0x06006CB3 RID: 27827 RVA: 0x001FFA6C File Offset: 0x001FDC6C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
				if (component != null)
				{
					component.mute = this.m_Mute.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04005552 RID: 21842
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x04005553 RID: 21843
		public FsmBool m_Mute;
	}
}
