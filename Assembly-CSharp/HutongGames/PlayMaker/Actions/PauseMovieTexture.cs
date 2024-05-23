using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C5E RID: 3166
	[ActionCategory(30)]
	[Tooltip("Pauses a Movie Texture.")]
	public class PauseMovieTexture : FsmStateAction
	{
		// Token: 0x060066F0 RID: 26352 RVA: 0x001E624F File Offset: 0x001E444F
		public override void Reset()
		{
			this.movieTexture = null;
		}

		// Token: 0x060066F1 RID: 26353 RVA: 0x001E6258 File Offset: 0x001E4458
		public override void OnEnter()
		{
			MovieTexture movieTexture = this.movieTexture.Value as MovieTexture;
			if (movieTexture != null)
			{
				movieTexture.Pause();
			}
			base.Finish();
		}

		// Token: 0x04004EC3 RID: 20163
		[ObjectType(typeof(MovieTexture))]
		[RequiredField]
		public FsmObject movieTexture;
	}
}
