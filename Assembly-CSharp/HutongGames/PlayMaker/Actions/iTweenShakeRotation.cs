using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D3C RID: 3388
	[Tooltip("Randomly shakes a GameObject's rotation by a diminishing amount over time.")]
	[ActionCategory("iTween")]
	public class iTweenShakeRotation : iTweenFsmAction
	{
		// Token: 0x06006ABD RID: 27325 RVA: 0x001F6C24 File Offset: 0x001F4E24
		public override void Reset()
		{
			base.Reset();
			this.id = new FsmString
			{
				UseVariable = true
			};
			this.time = 1f;
			this.delay = 0f;
			this.loopType = iTween.LoopType.none;
			this.vector = new FsmVector3
			{
				UseVariable = true
			};
			this.space = 0;
		}

		// Token: 0x06006ABE RID: 27326 RVA: 0x001F6C90 File Offset: 0x001F4E90
		public override void OnEnter()
		{
			base.OnEnteriTween(this.gameObject);
			if (this.loopType != iTween.LoopType.none)
			{
				base.IsLoop(true);
			}
			this.DoiTween();
		}

		// Token: 0x06006ABF RID: 27327 RVA: 0x001F6CC1 File Offset: 0x001F4EC1
		public override void OnExit()
		{
			base.OnExitiTween(this.gameObject);
		}

		// Token: 0x06006AC0 RID: 27328 RVA: 0x001F6CD0 File Offset: 0x001F4ED0
		private void DoiTween()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector = Vector3.zero;
			if (!this.vector.IsNone)
			{
				vector = this.vector.Value;
			}
			this.itweenType = "shake";
			iTween.ShakeRotation(ownerDefaultTarget, iTween.Hash(new object[]
			{
				"amount",
				vector,
				"name",
				(!this.id.IsNone) ? this.id.Value : string.Empty,
				"time",
				(!this.time.IsNone) ? this.time.Value : 1f,
				"delay",
				(!this.delay.IsNone) ? this.delay.Value : 0f,
				"looptype",
				this.loopType,
				"oncomplete",
				"iTweenOnComplete",
				"oncompleteparams",
				this.itweenID,
				"onstart",
				"iTweenOnStart",
				"onstartparams",
				this.itweenID,
				"ignoretimescale",
				!this.realTime.IsNone && this.realTime.Value,
				"space",
				this.space
			}));
		}

		// Token: 0x04005315 RID: 21269
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005316 RID: 21270
		[Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		// Token: 0x04005317 RID: 21271
		[Tooltip("A vector shake range.")]
		[RequiredField]
		public FsmVector3 vector;

		// Token: 0x04005318 RID: 21272
		[Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		// Token: 0x04005319 RID: 21273
		[Tooltip("The time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		// Token: 0x0400531A RID: 21274
		[Tooltip("The type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;

		// Token: 0x0400531B RID: 21275
		public Space space;
	}
}
