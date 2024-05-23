using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DCB RID: 3531
	[ActionCategory("Pegasus")]
	[Tooltip("Turns a Particle Emitter on and off with optional delay.")]
	public class particleEmitterOnOff : FsmStateAction
	{
		// Token: 0x06006D4D RID: 27981 RVA: 0x00202318 File Offset: 0x00200518
		public override void Reset()
		{
			this.gameObject = null;
			this.emitOnOff = false;
			this.delay = 0f;
			this.finishEvent = null;
			this.realTime = false;
			this.go = null;
		}

		// Token: 0x06006D4E RID: 27982 RVA: 0x00202360 File Offset: 0x00200560
		public override void OnEnter()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			this.go.GetComponent<ParticleEmitter>().emit = this.emitOnOff.Value;
			if (this.delay.Value <= 0f)
			{
				base.Finish();
				return;
			}
			this.startTime = Time.realtimeSinceStartup;
			this.timer = 0f;
		}

		// Token: 0x06006D4F RID: 27983 RVA: 0x002023D4 File Offset: 0x002005D4
		public override void OnUpdate()
		{
			if (this.realTime)
			{
				this.timer = Time.realtimeSinceStartup - this.startTime;
			}
			else
			{
				this.timer += Time.deltaTime;
			}
			if (this.timer > this.delay.Value)
			{
				this.go.GetComponent<ParticleEmitter>().emit = !this.emitOnOff.Value;
				base.Finish();
				if (this.finishEvent != null)
				{
					base.Fsm.Event(this.finishEvent);
				}
			}
		}

		// Token: 0x040055FA RID: 22010
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055FB RID: 22011
		[Tooltip("Set to True to turn it on and False to turn it off.")]
		public FsmBool emitOnOff;

		// Token: 0x040055FC RID: 22012
		[Tooltip("If 0 it just acts like a switch. Values cause it to Toggle value after delay time (sec).")]
		public FsmFloat delay;

		// Token: 0x040055FD RID: 22013
		public FsmEvent finishEvent;

		// Token: 0x040055FE RID: 22014
		public bool realTime;

		// Token: 0x040055FF RID: 22015
		private float startTime;

		// Token: 0x04005600 RID: 22016
		private float timer;

		// Token: 0x04005601 RID: 22017
		private GameObject go;
	}
}
