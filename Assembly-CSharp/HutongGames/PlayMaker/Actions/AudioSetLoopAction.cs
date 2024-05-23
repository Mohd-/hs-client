using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DA4 RID: 3492
	[Tooltip("Sets looping on the AudioSource on a Game Object.")]
	[ActionCategory("Pegasus Audio")]
	public class AudioSetLoopAction : FsmStateAction
	{
		// Token: 0x06006CAF RID: 27823 RVA: 0x001FF9E3 File Offset: 0x001FDBE3
		public override void Reset()
		{
			this.m_GameObject = null;
			this.m_Loop = false;
		}

		// Token: 0x06006CB0 RID: 27824 RVA: 0x001FF9F8 File Offset: 0x001FDBF8
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
				if (component != null)
				{
					component.loop = this.m_Loop.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04005550 RID: 21840
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault m_GameObject;

		// Token: 0x04005551 RID: 21841
		public FsmBool m_Loop;
	}
}
