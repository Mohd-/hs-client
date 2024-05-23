using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C64 RID: 3172
	[ActionCategory(1)]
	[Tooltip("Plays an Audio Clip at a position defined by a Game Object or Vector3. If a position is defined, it takes priority over the game object. This action doesn't require an Audio Source component, but offers less control than Audio actions.")]
	public class PlaySound : FsmStateAction
	{
		// Token: 0x0600670E RID: 26382 RVA: 0x001E6A24 File Offset: 0x001E4C24
		public override void Reset()
		{
			this.gameObject = null;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.clip = null;
			this.volume = 1f;
		}

		// Token: 0x0600670F RID: 26383 RVA: 0x001E6A63 File Offset: 0x001E4C63
		public override void OnEnter()
		{
			this.DoPlaySound();
			base.Finish();
		}

		// Token: 0x06006710 RID: 26384 RVA: 0x001E6A74 File Offset: 0x001E4C74
		private void DoPlaySound()
		{
			AudioClip audioClip = this.clip.Value as AudioClip;
			if (audioClip == null)
			{
				this.LogWarning("Missing Audio Clip!");
				return;
			}
			if (!this.position.IsNone)
			{
				AudioSource.PlayClipAtPoint(audioClip, this.position.Value, this.volume.Value);
			}
			else
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				AudioSource.PlayClipAtPoint(audioClip, ownerDefaultTarget.transform.position, this.volume.Value);
			}
		}

		// Token: 0x04004EE1 RID: 20193
		public FsmOwnerDefault gameObject;

		// Token: 0x04004EE2 RID: 20194
		public FsmVector3 position;

		// Token: 0x04004EE3 RID: 20195
		[ObjectType(typeof(AudioClip))]
		[Title("Audio Clip")]
		[RequiredField]
		public FsmObject clip;

		// Token: 0x04004EE4 RID: 20196
		[HasFloatSlider(0f, 1f)]
		public FsmFloat volume = 1f;
	}
}
