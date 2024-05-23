using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C61 RID: 3169
	[Tooltip("Plays a Movie Texture. Use the Movie Texture in a Material, or in the GUI.")]
	[ActionCategory(30)]
	public class PlayMovieTexture : FsmStateAction
	{
		// Token: 0x060066FF RID: 26367 RVA: 0x001E65A4 File Offset: 0x001E47A4
		public override void Reset()
		{
			this.movieTexture = null;
			this.loop = false;
		}

		// Token: 0x06006700 RID: 26368 RVA: 0x001E65BC File Offset: 0x001E47BC
		public override void OnEnter()
		{
			MovieTexture movieTexture = this.movieTexture.Value as MovieTexture;
			if (movieTexture != null)
			{
				movieTexture.loop = this.loop.Value;
				movieTexture.Play();
			}
			base.Finish();
		}

		// Token: 0x04004ED0 RID: 20176
		[ObjectType(typeof(MovieTexture))]
		[RequiredField]
		public FsmObject movieTexture;

		// Token: 0x04004ED1 RID: 20177
		public FsmBool loop;
	}
}
