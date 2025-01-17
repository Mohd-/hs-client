﻿using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C63 RID: 3171
	[Tooltip("Plays a Random Audio Clip at a position defined by a Game Object or a Vector3. If a position is defined, it takes priority over the game object. You can set the relative weight of the clips to control how often they are selected.")]
	[ActionCategory(1)]
	public class PlayRandomSound : FsmStateAction
	{
		// Token: 0x0600670A RID: 26378 RVA: 0x001E68D4 File Offset: 0x001E4AD4
		public override void Reset()
		{
			this.gameObject = null;
			this.position = new FsmVector3
			{
				UseVariable = true
			};
			this.audioClips = new AudioClip[3];
			this.weights = new FsmFloat[]
			{
				1f,
				1f,
				1f
			};
			this.volume = 1f;
		}

		// Token: 0x0600670B RID: 26379 RVA: 0x001E694B File Offset: 0x001E4B4B
		public override void OnEnter()
		{
			this.DoPlayRandomClip();
			base.Finish();
		}

		// Token: 0x0600670C RID: 26380 RVA: 0x001E695C File Offset: 0x001E4B5C
		private void DoPlayRandomClip()
		{
			if (this.audioClips.Length == 0)
			{
				return;
			}
			int randomWeightedIndex = ActionHelpers.GetRandomWeightedIndex(this.weights);
			if (randomWeightedIndex != -1)
			{
				AudioClip audioClip = this.audioClips[randomWeightedIndex];
				if (audioClip != null)
				{
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
			}
		}

		// Token: 0x04004EDC RID: 20188
		public FsmOwnerDefault gameObject;

		// Token: 0x04004EDD RID: 20189
		public FsmVector3 position;

		// Token: 0x04004EDE RID: 20190
		[CompoundArray("Audio Clips", "Audio Clip", "Weight")]
		public AudioClip[] audioClips;

		// Token: 0x04004EDF RID: 20191
		[HasFloatSlider(0f, 1f)]
		public FsmFloat[] weights;

		// Token: 0x04004EE0 RID: 20192
		[HasFloatSlider(0f, 1f)]
		public FsmFloat volume = 1f;
	}
}
