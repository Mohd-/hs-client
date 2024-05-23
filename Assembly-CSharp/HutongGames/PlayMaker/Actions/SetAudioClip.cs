using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C98 RID: 3224
	[Tooltip("Sets the Audio Clip played by the AudioSource component on a Game Object.")]
	[ActionCategory(1)]
	public class SetAudioClip : ComponentAction<AudioSource>
	{
		// Token: 0x060067EA RID: 26602 RVA: 0x001E9C8A File Offset: 0x001E7E8A
		public override void Reset()
		{
			this.gameObject = null;
			this.audioClip = null;
		}

		// Token: 0x060067EB RID: 26603 RVA: 0x001E9C9C File Offset: 0x001E7E9C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.audio.clip = (this.audioClip.Value as AudioClip);
			}
			base.Finish();
		}

		// Token: 0x04004FBE RID: 20414
		[RequiredField]
		[Tooltip("The GameObject with the AudioSource component.")]
		[CheckForComponent(typeof(AudioSource))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004FBF RID: 20415
		[ObjectType(typeof(AudioClip))]
		[Tooltip("The AudioClip to set.")]
		public FsmObject audioClip;
	}
}
