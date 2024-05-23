using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B69 RID: 2921
	[ActionCategory(22)]
	[Tooltip("Fade from a fullscreen Color. NOTE: Uses OnGUI so requires a PlayMakerGUI component in the scene.")]
	public class CameraFadeIn : FsmStateAction
	{
		// Token: 0x0600630C RID: 25356 RVA: 0x001D9983 File Offset: 0x001D7B83
		public override void Reset()
		{
			this.color = Color.black;
			this.time = 1f;
			this.finishEvent = null;
		}

		// Token: 0x0600630D RID: 25357 RVA: 0x001D99AC File Offset: 0x001D7BAC
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.currentTime = 0f;
			this.colorLerp = this.color.Value;
		}

		// Token: 0x0600630E RID: 25358 RVA: 0x001D99D8 File Offset: 0x001D7BD8
		public override void OnUpdate()
		{
			if (this.realTime)
			{
				this.currentTime = FsmTime.RealtimeSinceStartup - this.startTime;
			}
			else
			{
				this.currentTime += Time.deltaTime;
			}
			this.colorLerp = Color.Lerp(this.color.Value, Color.clear, this.currentTime / this.time.Value);
			if (this.currentTime > this.time.Value)
			{
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
				base.Finish();
			}
		}

		// Token: 0x0600630F RID: 25359 RVA: 0x001D9A80 File Offset: 0x001D7C80
		public override void OnGUI()
		{
			Color color = GUI.color;
			GUI.color = this.colorLerp;
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ActionHelpers.WhiteTexture);
			GUI.color = color;
		}

		// Token: 0x04004A8A RID: 19082
		[RequiredField]
		[Tooltip("Color to fade from. E.g., Fade up from black.")]
		public FsmColor color;

		// Token: 0x04004A8B RID: 19083
		[RequiredField]
		[HasFloatSlider(0f, 10f)]
		[Tooltip("Fade in time in seconds.")]
		public FsmFloat time;

		// Token: 0x04004A8C RID: 19084
		[Tooltip("Event to send when finished.")]
		public FsmEvent finishEvent;

		// Token: 0x04004A8D RID: 19085
		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		// Token: 0x04004A8E RID: 19086
		private float startTime;

		// Token: 0x04004A8F RID: 19087
		private float currentTime;

		// Token: 0x04004A90 RID: 19088
		private Color colorLerp;
	}
}
