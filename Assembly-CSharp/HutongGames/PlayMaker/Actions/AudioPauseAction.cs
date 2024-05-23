using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D9F RID: 3487
	[Tooltip("Pauses the Audio Source of a Game Object.")]
	[ActionCategory("Pegasus Audio")]
	public class AudioPauseAction : FsmStateAction
	{
		// Token: 0x06006C9A RID: 27802 RVA: 0x001FF273 File Offset: 0x001FD473
		public override void Reset()
		{
			this.m_GameObject = null;
		}

		// Token: 0x06006C9B RID: 27803 RVA: 0x001FF27C File Offset: 0x001FD47C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.m_GameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
				if (component != null)
				{
					SoundManager.Get().Pause(component);
				}
			}
			base.Finish();
		}

		// Token: 0x04005536 RID: 21814
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault m_GameObject;
	}
}
