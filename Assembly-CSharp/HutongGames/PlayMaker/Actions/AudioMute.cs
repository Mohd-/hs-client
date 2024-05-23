using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B57 RID: 2903
	[ActionCategory(1)]
	[Tooltip("Mute/unmute the Audio Clip played by an Audio Source component on a Game Object.")]
	public class AudioMute : FsmStateAction
	{
		// Token: 0x060062C5 RID: 25285 RVA: 0x001D87BE File Offset: 0x001D69BE
		public override void Reset()
		{
			this.gameObject = null;
			this.mute = false;
		}

		// Token: 0x060062C6 RID: 25286 RVA: 0x001D87D4 File Offset: 0x001D69D4
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
				if (component != null)
				{
					component.mute = this.mute.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x04004A37 RID: 18999
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		[Tooltip("The GameObject with an Audio Source component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004A38 RID: 19000
		[Tooltip("Check to mute, uncheck to unmute.")]
		[RequiredField]
		public FsmBool mute;
	}
}
