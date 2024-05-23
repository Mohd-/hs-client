using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B6A RID: 2922
	[Tooltip("Fade to a fullscreen Color. NOTE: Uses OnGUI so requires a PlayMakerGUI component in the scene.")]
	[ActionCategory(22)]
	public class CameraFadeOut : FsmStateAction
	{
		// Token: 0x06006311 RID: 25361 RVA: 0x001D9AD1 File Offset: 0x001D7CD1
		public override void Reset()
		{
			this.color = Color.black;
			this.time = 1f;
			this.finishEvent = null;
		}

		// Token: 0x06006312 RID: 25362 RVA: 0x001D9AFA File Offset: 0x001D7CFA
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.currentTime = 0f;
			this.colorLerp = Color.clear;
		}

		// Token: 0x06006313 RID: 25363 RVA: 0x001D9B20 File Offset: 0x001D7D20
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
			this.colorLerp = Color.Lerp(Color.clear, this.color.Value, this.currentTime / this.time.Value);
			if (this.currentTime > this.time.Value && this.finishEvent != null)
			{
				base.Fsm.Event(this.finishEvent);
			}
		}

		// Token: 0x06006314 RID: 25364 RVA: 0x001D9BC0 File Offset: 0x001D7DC0
		public override void OnGUI()
		{
			Color color = GUI.color;
			GUI.color = this.colorLerp;
			GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), ActionHelpers.WhiteTexture);
			GUI.color = color;
		}

		// Token: 0x04004A91 RID: 19089
		[RequiredField]
		[Tooltip("Color to fade to. E.g., Fade to black.")]
		public FsmColor color;

		// Token: 0x04004A92 RID: 19090
		[RequiredField]
		[Tooltip("Fade out time in seconds.")]
		[HasFloatSlider(0f, 10f)]
		public FsmFloat time;

		// Token: 0x04004A93 RID: 19091
		[Tooltip("Event to send when finished.")]
		public FsmEvent finishEvent;

		// Token: 0x04004A94 RID: 19092
		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		// Token: 0x04004A95 RID: 19093
		private float startTime;

		// Token: 0x04004A96 RID: 19094
		private float currentTime;

		// Token: 0x04004A97 RID: 19095
		private Color colorLerp;
	}
}
