using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B5A RID: 2906
	[ActionCategory(1)]
	[Tooltip("Stops playing the Audio Clip played by an Audio Source component on a Game Object.")]
	public class AudioStop : FsmStateAction
	{
		// Token: 0x060062CF RID: 25295 RVA: 0x001D8A2E File Offset: 0x001D6C2E
		public override void Reset()
		{
			this.gameObject = null;
		}

		// Token: 0x060062D0 RID: 25296 RVA: 0x001D8A38 File Offset: 0x001D6C38
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget != null)
			{
				AudioSource component = ownerDefaultTarget.GetComponent<AudioSource>();
				if (component != null)
				{
					component.Stop();
				}
			}
			base.Finish();
		}

		// Token: 0x04004A3F RID: 19007
		[RequiredField]
		[Tooltip("The GameObject with an AudioSource component.")]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault gameObject;
	}
}
