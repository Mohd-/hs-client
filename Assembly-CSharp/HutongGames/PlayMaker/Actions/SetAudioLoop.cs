using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C99 RID: 3225
	[ActionCategory(1)]
	[Tooltip("Sets looping on the AudioSource component on a Game Object.")]
	public class SetAudioLoop : ComponentAction<AudioSource>
	{
		// Token: 0x060067ED RID: 26605 RVA: 0x001E9CF0 File Offset: 0x001E7EF0
		public override void Reset()
		{
			this.gameObject = null;
			this.loop = false;
		}

		// Token: 0x060067EE RID: 26606 RVA: 0x001E9D08 File Offset: 0x001E7F08
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.audio.loop = this.loop.Value;
			}
			base.Finish();
		}

		// Token: 0x04004FC0 RID: 20416
		[RequiredField]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004FC1 RID: 20417
		public FsmBool loop;
	}
}
