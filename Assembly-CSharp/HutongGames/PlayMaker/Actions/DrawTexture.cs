using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B9A RID: 2970
	[ActionCategory(5)]
	[Tooltip("Draws a GUI Texture. NOTE: Uses OnGUI so you need a PlayMakerGUI component in the scene.")]
	public class DrawTexture : FsmStateAction
	{
		// Token: 0x060063D1 RID: 25553 RVA: 0x001DBE60 File Offset: 0x001DA060
		public override void Reset()
		{
			this.texture = null;
			this.screenRect = null;
			this.left = 0f;
			this.top = 0f;
			this.width = 1f;
			this.height = 1f;
			this.scaleMode = 0;
			this.alphaBlend = true;
			this.imageAspect = 0f;
			this.normalized = true;
		}

		// Token: 0x060063D2 RID: 25554 RVA: 0x001DBEEC File Offset: 0x001DA0EC
		public override void OnGUI()
		{
			if (this.texture.Value == null)
			{
				return;
			}
			this.rect = (this.screenRect.IsNone ? default(Rect) : this.screenRect.Value);
			if (!this.left.IsNone)
			{
				this.rect.x = this.left.Value;
			}
			if (!this.top.IsNone)
			{
				this.rect.y = this.top.Value;
			}
			if (!this.width.IsNone)
			{
				this.rect.width = this.width.Value;
			}
			if (!this.height.IsNone)
			{
				this.rect.height = this.height.Value;
			}
			if (this.normalized.Value)
			{
				this.rect.x = this.rect.x * (float)Screen.width;
				this.rect.width = this.rect.width * (float)Screen.width;
				this.rect.y = this.rect.y * (float)Screen.height;
				this.rect.height = this.rect.height * (float)Screen.height;
			}
			GUI.DrawTexture(this.rect, this.texture.Value, this.scaleMode, this.alphaBlend.Value, this.imageAspect.Value);
		}

		// Token: 0x04004B3E RID: 19262
		[RequiredField]
		[Tooltip("Texture to draw.")]
		public FsmTexture texture;

		// Token: 0x04004B3F RID: 19263
		[UIHint(10)]
		[Title("Position")]
		[Tooltip("Rectangle on the screen to draw the texture within. Alternatively, set or override individual properties below.")]
		public FsmRect screenRect;

		// Token: 0x04004B40 RID: 19264
		[Tooltip("Left screen coordinate.")]
		public FsmFloat left;

		// Token: 0x04004B41 RID: 19265
		[Tooltip("Top screen coordinate.")]
		public FsmFloat top;

		// Token: 0x04004B42 RID: 19266
		[Tooltip("Width of texture on screen.")]
		public FsmFloat width;

		// Token: 0x04004B43 RID: 19267
		[Tooltip("Height of texture on screen.")]
		public FsmFloat height;

		// Token: 0x04004B44 RID: 19268
		[Tooltip("How to scale the image when the aspect ratio of it doesn't fit the aspect ratio to be drawn within.")]
		public ScaleMode scaleMode;

		// Token: 0x04004B45 RID: 19269
		[Tooltip("Whether to alpha blend the image on to the display (the default). If false, the picture is drawn on to the display.")]
		public FsmBool alphaBlend;

		// Token: 0x04004B46 RID: 19270
		[Tooltip("Aspect ratio to use for the source image. If 0 (the default), the aspect ratio from the image is used. Pass in w/h for the desired aspect ratio. This allows the aspect ratio of the source image to be adjusted without changing the pixel width and height.")]
		public FsmFloat imageAspect;

		// Token: 0x04004B47 RID: 19271
		[Tooltip("Use normalized screen coordinates (0-1)")]
		public FsmBool normalized;

		// Token: 0x04004B48 RID: 19272
		private Rect rect;
	}
}
