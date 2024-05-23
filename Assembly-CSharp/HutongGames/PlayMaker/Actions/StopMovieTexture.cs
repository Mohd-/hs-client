using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF7 RID: 3319
	[ActionCategory(30)]
	[Tooltip("Stops playing the Movie Texture, and rewinds it to the beginning.")]
	public class StopMovieTexture : FsmStateAction
	{
		// Token: 0x06006990 RID: 27024 RVA: 0x001EEEAC File Offset: 0x001ED0AC
		public override void Reset()
		{
			this.movieTexture = null;
		}

		// Token: 0x06006991 RID: 27025 RVA: 0x001EEEB8 File Offset: 0x001ED0B8
		public override void OnEnter()
		{
			MovieTexture movieTexture = this.movieTexture.Value as MovieTexture;
			if (movieTexture != null)
			{
				movieTexture.Stop();
			}
			base.Finish();
		}

		// Token: 0x0400514A RID: 20810
		[ObjectType(typeof(MovieTexture))]
		[RequiredField]
		public FsmObject movieTexture;
	}
}
