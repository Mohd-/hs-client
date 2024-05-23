using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA5 RID: 2981
	[Tooltip("Randomly flickers a Game Object on/off.")]
	[ActionCategory(13)]
	public class Flicker : ComponentAction<Renderer>
	{
		// Token: 0x06006401 RID: 25601 RVA: 0x001DCA91 File Offset: 0x001DAC91
		public override void Reset()
		{
			this.gameObject = null;
			this.frequency = 0.1f;
			this.amountOn = 0.5f;
			this.rendererOnly = true;
			this.realTime = false;
		}

		// Token: 0x06006402 RID: 25602 RVA: 0x001DCAC8 File Offset: 0x001DACC8
		public override void OnEnter()
		{
			this.startTime = FsmTime.RealtimeSinceStartup;
			this.timer = 0f;
		}

		// Token: 0x06006403 RID: 25603 RVA: 0x001DCAE0 File Offset: 0x001DACE0
		public override void OnUpdate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.realTime)
			{
				this.timer = FsmTime.RealtimeSinceStartup - this.startTime;
			}
			else
			{
				this.timer += Time.deltaTime;
			}
			if (this.timer > this.frequency.Value)
			{
				bool flag = Random.Range(0f, 1f) < this.amountOn.Value;
				if (this.rendererOnly)
				{
					if (base.UpdateCache(ownerDefaultTarget))
					{
						base.renderer.enabled = flag;
					}
				}
				else
				{
					ownerDefaultTarget.SetActive(flag);
				}
				this.startTime = this.timer;
				this.timer = 0f;
			}
		}

		// Token: 0x04004B72 RID: 19314
		[Tooltip("The GameObject to flicker.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004B73 RID: 19315
		[HasFloatSlider(0f, 1f)]
		[Tooltip("The frequency of the flicker in seconds.")]
		public FsmFloat frequency;

		// Token: 0x04004B74 RID: 19316
		[HasFloatSlider(0f, 1f)]
		[Tooltip("Amount of time flicker is On (0-1). E.g. Use 0.95 for an occasional flicker.")]
		public FsmFloat amountOn;

		// Token: 0x04004B75 RID: 19317
		[Tooltip("Only effect the renderer, leaving other components active.")]
		public bool rendererOnly;

		// Token: 0x04004B76 RID: 19318
		[Tooltip("Ignore time scale. Useful if flickering UI when the game is paused.")]
		public bool realTime;

		// Token: 0x04004B77 RID: 19319
		private float startTime;

		// Token: 0x04004B78 RID: 19320
		private float timer;
	}
}
