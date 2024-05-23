using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C5B RID: 3163
	[ActionCategory(30)]
	[Tooltip("Sets the Game Object as the Audio Source associated with the Movie Texture. The Game Object must have an AudioSource Component.")]
	public class MovieTextureAudioSettings : FsmStateAction
	{
		// Token: 0x060066E4 RID: 26340 RVA: 0x001E60E4 File Offset: 0x001E42E4
		public override void Reset()
		{
			this.movieTexture = null;
			this.gameObject = null;
		}

		// Token: 0x060066E5 RID: 26341 RVA: 0x001E60F4 File Offset: 0x001E42F4
		public override void OnEnter()
		{
			MovieTexture movieTexture = this.movieTexture.Value as MovieTexture;
			if (movieTexture != null && this.gameObject.Value != null)
			{
				AudioSource component = this.gameObject.Value.GetComponent<AudioSource>();
				if (component != null)
				{
					component.clip = movieTexture.audioClip;
				}
			}
			base.Finish();
		}

		// Token: 0x04004EBA RID: 20154
		[RequiredField]
		[ObjectType(typeof(MovieTexture))]
		public FsmObject movieTexture;

		// Token: 0x04004EBB RID: 20155
		[CheckForComponent(typeof(AudioSource))]
		[RequiredField]
		public FsmGameObject gameObject;
	}
}
