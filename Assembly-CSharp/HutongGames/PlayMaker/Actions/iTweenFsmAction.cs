using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D20 RID: 3360
	[Tooltip("iTween base action - don't use!")]
	public abstract class iTweenFsmAction : FsmStateAction
	{
		// Token: 0x06006A39 RID: 27193 RVA: 0x001F17E8 File Offset: 0x001EF9E8
		public override void Reset()
		{
			this.startEvent = null;
			this.finishEvent = null;
			this.realTime = new FsmBool
			{
				Value = false
			};
			this.stopOnExit = new FsmBool
			{
				Value = true
			};
			this.loopDontFinish = new FsmBool
			{
				Value = true
			};
			this.itweenType = string.Empty;
		}

		// Token: 0x06006A3A RID: 27194 RVA: 0x001F184C File Offset: 0x001EFA4C
		protected void OnEnteriTween(FsmOwnerDefault anOwner)
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(anOwner);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.itweenEvents = ownerDefaultTarget.AddComponent<iTweenFSMEvents>();
			this.itweenEvents.itweenFSMAction = this;
			iTweenFSMEvents.itweenIDCount++;
			this.itweenID = iTweenFSMEvents.itweenIDCount;
			this.itweenEvents.itweenID = iTweenFSMEvents.itweenIDCount;
			this.itweenEvents.donotfinish = (!this.loopDontFinish.IsNone && this.loopDontFinish.Value);
		}

		// Token: 0x06006A3B RID: 27195 RVA: 0x001F18DE File Offset: 0x001EFADE
		protected void IsLoop(bool aValue)
		{
			if (this.itweenEvents != null)
			{
				this.itweenEvents.islooping = aValue;
			}
		}

		// Token: 0x06006A3C RID: 27196 RVA: 0x001F1900 File Offset: 0x001EFB00
		protected void OnExitiTween(FsmOwnerDefault anOwner)
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(anOwner);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.itweenEvents)
			{
				Object.Destroy(this.itweenEvents);
			}
			if (this.stopOnExit.IsNone)
			{
				iTween.Stop(ownerDefaultTarget, this.itweenType);
			}
			else if (this.stopOnExit.Value)
			{
				iTween.Stop(ownerDefaultTarget, this.itweenType);
			}
		}

		// Token: 0x04005216 RID: 21014
		[ActionSection("Events")]
		public FsmEvent startEvent;

		// Token: 0x04005217 RID: 21015
		public FsmEvent finishEvent;

		// Token: 0x04005218 RID: 21016
		[Tooltip("Setting this to true will allow the animation to continue independent of the current time which is helpful for animating menus after a game has been paused by setting Time.timeScale=0.")]
		public FsmBool realTime;

		// Token: 0x04005219 RID: 21017
		public FsmBool stopOnExit;

		// Token: 0x0400521A RID: 21018
		public FsmBool loopDontFinish;

		// Token: 0x0400521B RID: 21019
		internal iTweenFSMEvents itweenEvents;

		// Token: 0x0400521C RID: 21020
		protected string itweenType = string.Empty;

		// Token: 0x0400521D RID: 21021
		protected int itweenID = -1;

		// Token: 0x02000D21 RID: 3361
		public enum AxisRestriction
		{
			// Token: 0x0400521F RID: 21023
			none,
			// Token: 0x04005220 RID: 21024
			x,
			// Token: 0x04005221 RID: 21025
			y,
			// Token: 0x04005222 RID: 21026
			z,
			// Token: 0x04005223 RID: 21027
			xy,
			// Token: 0x04005224 RID: 21028
			xz,
			// Token: 0x04005225 RID: 21029
			yz
		}
	}
}
