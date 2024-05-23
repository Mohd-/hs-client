using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B5D RID: 2909
	[Tooltip("Turns a Game Object on/off in a regular repeating pattern.")]
	[ActionCategory(13)]
	public class Blink : ComponentAction<Renderer>
	{
		// Token: 0x060062DA RID: 25306 RVA: 0x001D8E08 File Offset: 0x001D7008
		public override void Reset()
		{
			this.gameObject = null;
			this.timeOff = 0.5f;
			this.timeOn = 0.5f;
			this.rendererOnly = true;
			this.startOn = false;
			this.realTime = false;
		}

		// Token: 0x060062DB RID: 25307 RVA: 0x001D8E56 File Offset: 0x001D7056
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.timer = 0f;
			this.UpdateBlinkState(this.startOn.Value);
		}

		// Token: 0x060062DC RID: 25308 RVA: 0x001D8E80 File Offset: 0x001D7080
		public override void OnUpdate()
		{
			if (this.realTime)
			{
				this.timer = FsmTime.RealtimeSinceStartup - this.startTime;
			}
			else
			{
				this.timer += Time.deltaTime;
			}
			if (this.blinkOn && this.timer > this.timeOn.Value)
			{
				this.UpdateBlinkState(false);
			}
			if (!this.blinkOn && this.timer > this.timeOff.Value)
			{
				this.UpdateBlinkState(true);
			}
		}

		// Token: 0x060062DD RID: 25309 RVA: 0x001D8F14 File Offset: 0x001D7114
		private void UpdateBlinkState(bool state)
		{
			GameObject gameObject = (this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner;
			if (gameObject == null)
			{
				return;
			}
			if (this.rendererOnly)
			{
				if (base.UpdateCache(gameObject))
				{
					base.renderer.enabled = state;
				}
			}
			else
			{
				gameObject.SetActive(state);
			}
			this.blinkOn = state;
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.timer = 0f;
		}

		// Token: 0x04004A4E RID: 19022
		[RequiredField]
		[Tooltip("The GameObject to blink on/off.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004A4F RID: 19023
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Time to stay off in seconds.")]
		public FsmFloat timeOff;

		// Token: 0x04004A50 RID: 19024
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Time to stay on in seconds.")]
		public FsmFloat timeOn;

		// Token: 0x04004A51 RID: 19025
		[Tooltip("Should the object start in the active/visible state?")]
		public FsmBool startOn;

		// Token: 0x04004A52 RID: 19026
		[Tooltip("Only effect the renderer, keeping other components active.")]
		public bool rendererOnly;

		// Token: 0x04004A53 RID: 19027
		[Tooltip("Ignore TimeScale. Useful if the game is paused.")]
		public bool realTime;

		// Token: 0x04004A54 RID: 19028
		private float startTime;

		// Token: 0x04004A55 RID: 19029
		private float timer;

		// Token: 0x04004A56 RID: 19030
		private bool blinkOn;
	}
}
