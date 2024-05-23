using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DAA RID: 3498
	[ActionCategory("Pegasus Audio")]
	[Tooltip("Stops an Audio Source on a Game Object.")]
	public class AudioStopAction : FsmStateAction
	{
		// Token: 0x06006CCB RID: 27851 RVA: 0x001FFE73 File Offset: 0x001FE073
		public override void Reset()
		{
			this.m_GameObject = null;
		}

		// Token: 0x06006CCC RID: 27852 RVA: 0x001FFE7C File Offset: 0x001FE07C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
				if (component != null)
				{
					SoundManager.Get().Stop(component);
				}
			}
			base.Finish();
		}

		// Token: 0x04005564 RID: 21860
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault m_GameObject;
	}
}
