using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D1B RID: 3355
	[Tooltip("Gets data from a url and store it in variables. See Unity WWW docs for more details.")]
	[ActionCategory("Web Player")]
	public class WWWObject : FsmStateAction
	{
		// Token: 0x06006A23 RID: 27171 RVA: 0x001F1254 File Offset: 0x001EF454
		public override void Reset()
		{
			this.url = null;
			this.storeText = null;
			this.storeTexture = null;
			this.errorString = null;
			this.progress = null;
			this.isDone = null;
		}

		// Token: 0x06006A24 RID: 27172 RVA: 0x001F128C File Offset: 0x001EF48C
		public override void OnEnter()
		{
			if (string.IsNullOrEmpty(this.url.Value))
			{
				base.Finish();
				return;
			}
			this.wwwObject = new WWW(this.url.Value);
		}

		// Token: 0x06006A25 RID: 27173 RVA: 0x001F12CC File Offset: 0x001EF4CC
		public override void OnUpdate()
		{
			if (this.wwwObject == null)
			{
				this.errorString.Value = "WWW Object is Null!";
				base.Finish();
				return;
			}
			this.errorString.Value = this.wwwObject.error;
			if (!string.IsNullOrEmpty(this.wwwObject.error))
			{
				base.Finish();
				base.Fsm.Event(this.isError);
				return;
			}
			this.progress.Value = this.wwwObject.progress;
			if (this.progress.Value.Equals(1f))
			{
				this.storeText.Value = this.wwwObject.text;
				this.storeTexture.Value = this.wwwObject.texture;
				this.storeMovieTexture.Value = this.wwwObject.movie;
				this.errorString.Value = this.wwwObject.error;
				base.Fsm.Event((!string.IsNullOrEmpty(this.errorString.Value)) ? this.isError : this.isDone);
				base.Finish();
			}
		}

		// Token: 0x040051FC RID: 20988
		[RequiredField]
		[Tooltip("Url to download data from.")]
		public FsmString url;

		// Token: 0x040051FD RID: 20989
		[ActionSection("Results")]
		[UIHint(10)]
		[Tooltip("Gets text from the url.")]
		public FsmString storeText;

		// Token: 0x040051FE RID: 20990
		[UIHint(10)]
		[Tooltip("Gets a Texture from the url.")]
		public FsmTexture storeTexture;

		// Token: 0x040051FF RID: 20991
		[ObjectType(typeof(MovieTexture))]
		[Tooltip("Gets a Texture from the url.")]
		[UIHint(10)]
		public FsmObject storeMovieTexture;

		// Token: 0x04005200 RID: 20992
		[UIHint(10)]
		[Tooltip("Error message if there was an error during the download.")]
		public FsmString errorString;

		// Token: 0x04005201 RID: 20993
		[UIHint(10)]
		[Tooltip("How far the download progressed (0-1).")]
		public FsmFloat progress;

		// Token: 0x04005202 RID: 20994
		[Tooltip("Event to send when the data has finished loading (progress = 1).")]
		[ActionSection("Events")]
		public FsmEvent isDone;

		// Token: 0x04005203 RID: 20995
		[Tooltip("Event to send if there was an error.")]
		public FsmEvent isError;

		// Token: 0x04005204 RID: 20996
		private WWW wwwObject;
	}
}
