using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B58 RID: 2904
	[Tooltip("Pauses playing the Audio Clip played by an Audio Source component on a Game Object.")]
	[ActionCategory(1)]
	public class AudioPause : FsmStateAction
	{
		// Token: 0x060062C8 RID: 25288 RVA: 0x001D8831 File Offset: 0x001D6A31
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060062C9 RID: 25289 RVA: 0x001D883C File Offset: 0x001D6A3C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
				if (component != null)
				{
					component.Pause();
				}
			}
			base.Finish();
		}

		// Token: 0x04004A39 RID: 19001
		[Tooltip("The GameObject with an Audio Source component.")]
		[CheckForComponent(typeof(AudioSource))]
		[RequiredField]
		public FsmOwnerDefault gameObject;
	}
}
