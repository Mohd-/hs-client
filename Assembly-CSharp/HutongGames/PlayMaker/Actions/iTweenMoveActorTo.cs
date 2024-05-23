using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E02 RID: 3586
	[ActionCategory("Pegasus")]
	[Tooltip("Move an object's actor.  Used for spells that are dynamically loaded.")]
	public class iTweenMoveActorTo : iTweenFsmAction
	{
		// Token: 0x06006E25 RID: 28197 RVA: 0x00205408 File Offset: 0x00203608
		public override void Reset()
		{
			base.Reset();
			this.id = new FsmString
			{
				UseVariable = true
			};
			this.vectorPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.time = 1f;
			this.delay = 0f;
			this.easeType = iTween.EaseType.linear;
			this.loopType = iTween.LoopType.none;
		}

		// Token: 0x06006E26 RID: 28198 RVA: 0x00205474 File Offset: 0x00203674
		public override void OnEnter()
		{
			base.OnEnteriTween(this.gameObject);
			if (this.loopType != iTween.LoopType.none)
			{
				base.IsLoop(true);
			}
			this.DoiTween();
		}

		// Token: 0x06006E27 RID: 28199 RVA: 0x002054A5 File Offset: 0x002036A5
		public override void OnExit()
		{
			base.OnExitiTween(this.gameObject);
		}

		// Token: 0x06006E28 RID: 28200 RVA: 0x002054B4 File Offset: 0x002036B4
		private void DoiTween()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Actor actor = SceneUtils.FindComponentInParents<Actor>(ownerDefaultTarget);
			if (actor == null)
			{
				return;
			}
			GameObject gameObject = actor.gameObject;
			if (gameObject == null)
			{
				return;
			}
			this.itweenType = "move";
			Hashtable hashtable = new Hashtable();
			hashtable.Add("position", this.vectorPosition);
			hashtable.Add("name", (!this.id.IsNone) ? this.id.Value : string.Empty);
			hashtable.Add("delay", (!this.delay.IsNone) ? this.delay.Value : 0f);
			hashtable.Add("easetype", this.easeType);
			hashtable.Add("looptype", this.loopType);
			hashtable.Add("ignoretimescale", !this.realTime.IsNone && this.realTime.Value);
			if (this.time.Value <= 0f)
			{
				hashtable.Add("time", 0f);
				iTween.FadeUpdate(gameObject, hashtable);
				base.Fsm.Event(this.startEvent);
				base.Fsm.Event(this.finishEvent);
				base.Finish();
				return;
			}
			hashtable["time"] = ((!this.time.IsNone) ? this.time.Value : 1f);
			hashtable.Add("oncomplete", "iTweenOnComplete");
			hashtable.Add("oncompleteparams", this.itweenID);
			hashtable.Add("onstart", "iTweenOnStart");
			hashtable.Add("onstartparams", this.itweenID);
			hashtable.Add("oncompletetarget", ownerDefaultTarget);
			iTween.MoveTo(gameObject, hashtable);
		}

		// Token: 0x040056C5 RID: 22213
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056C6 RID: 22214
		[Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		// Token: 0x040056C7 RID: 22215
		[Tooltip("Position the GameObject will animate to.")]
		public FsmVector3 vectorPosition;

		// Token: 0x040056C8 RID: 22216
		[Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		// Token: 0x040056C9 RID: 22217
		[Tooltip("The time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		// Token: 0x040056CA RID: 22218
		[Tooltip("The shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;

		// Token: 0x040056CB RID: 22219
		[Tooltip("The type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;
	}
}
