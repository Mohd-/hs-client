using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000E01 RID: 3585
	[ActionCategory("Pegasus")]
	[Tooltip("Changes a GameObject's alpha over time, if it supports alpha changes.")]
	public class iTweenFadeToAction : iTweenFsmAction
	{
		// Token: 0x06006E1D RID: 28189 RVA: 0x00205034 File Offset: 0x00203234
		public override void Reset()
		{
			base.Reset();
			this.id = new FsmString
			{
				UseVariable = true
			};
			this.m_Alpha = 0f;
			this.time = 1f;
			this.delay = 0f;
			this.easeType = iTween.EaseType.linear;
			this.loopType = iTween.LoopType.none;
		}

		// Token: 0x06006E1E RID: 28190 RVA: 0x0020509C File Offset: 0x0020329C
		public override void OnEnter()
		{
			base.OnEnteriTween(this.gameObject);
			if (this.loopType != iTween.LoopType.none)
			{
				base.IsLoop(true);
			}
			this.DoiTween();
		}

		// Token: 0x06006E1F RID: 28191 RVA: 0x002050CD File Offset: 0x002032CD
		public override void OnExit()
		{
			base.OnExitiTween(this.gameObject);
		}

		// Token: 0x06006E20 RID: 28192 RVA: 0x002050DC File Offset: 0x002032DC
		private void DoiTween()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.itweenType = "color";
			if (this.m_IncludeChildren.Value)
			{
				foreach (object obj in ownerDefaultTarget.transform)
				{
					Transform transform = (Transform)obj;
					this.DoiTweenOnChild(transform.gameObject);
				}
			}
			this.DoiTweenOnParent(ownerDefaultTarget);
		}

		// Token: 0x06006E21 RID: 28193 RVA: 0x00205188 File Offset: 0x00203388
		private void DoiTweenOnChild(GameObject go)
		{
			Hashtable hashtable = this.InitTweenArgTable();
			if (this.time.Value <= 0f)
			{
				hashtable.Add("time", 0f);
				iTween.FadeUpdate(go, hashtable);
				return;
			}
			hashtable["time"] = ((!this.time.IsNone) ? this.time.Value : 1f);
			iTween.FadeTo(go, hashtable);
		}

		// Token: 0x06006E22 RID: 28194 RVA: 0x0020520C File Offset: 0x0020340C
		private void DoiTweenOnParent(GameObject go)
		{
			Hashtable hashtable = this.InitTweenArgTable();
			if (this.time.Value <= 0f)
			{
				hashtable.Add("time", 0f);
				iTween.FadeUpdate(go, hashtable);
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
			iTween.FadeTo(go, hashtable);
		}

		// Token: 0x06006E23 RID: 28195 RVA: 0x00205304 File Offset: 0x00203504
		private Hashtable InitTweenArgTable()
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("alpha", this.m_Alpha.Value);
			hashtable.Add("name", (!this.id.IsNone) ? this.id.Value : string.Empty);
			hashtable.Add("delay", (!this.delay.IsNone) ? this.delay.Value : 0f);
			hashtable.Add("easetype", this.easeType);
			hashtable.Add("looptype", this.loopType);
			hashtable.Add("ignoretimescale", !this.realTime.IsNone && this.realTime.Value);
			return hashtable;
		}

		// Token: 0x040056BD RID: 22205
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040056BE RID: 22206
		[Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		// Token: 0x040056BF RID: 22207
		[Tooltip("An alpha value the GameObject will animate To.")]
		public FsmFloat m_Alpha;

		// Token: 0x040056C0 RID: 22208
		[Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		// Token: 0x040056C1 RID: 22209
		[Tooltip("The time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		// Token: 0x040056C2 RID: 22210
		[Tooltip("The shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;

		// Token: 0x040056C3 RID: 22211
		[Tooltip("The type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;

		// Token: 0x040056C4 RID: 22212
		[Tooltip("Run this action on all child objects.")]
		public FsmBool m_IncludeChildren;
	}
}
