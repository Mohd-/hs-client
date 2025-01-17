﻿using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D28 RID: 3368
	[Tooltip("Instantly changes a GameObject's position to a supplied destination then returns it to it's starting position over time.")]
	[ActionCategory("iTween")]
	public class iTweenMoveFrom : iTweenFsmAction
	{
		// Token: 0x06006A5C RID: 27228 RVA: 0x001F2CEC File Offset: 0x001F0EEC
		public override void Reset()
		{
			base.Reset();
			this.id = new FsmString
			{
				UseVariable = true
			};
			this.transformPosition = new FsmGameObject
			{
				UseVariable = true
			};
			this.vectorPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.time = 1f;
			this.delay = 0f;
			this.loopType = iTween.LoopType.none;
			this.speed = new FsmFloat
			{
				UseVariable = true
			};
			this.space = 0;
			this.orientToPath = new FsmBool
			{
				Value = true
			};
			this.lookAtObject = new FsmGameObject
			{
				UseVariable = true
			};
			this.lookAtVector = new FsmVector3
			{
				UseVariable = true
			};
			this.lookTime = 0f;
			this.axis = iTweenFsmAction.AxisRestriction.none;
		}

		// Token: 0x06006A5D RID: 27229 RVA: 0x001F2DD4 File Offset: 0x001F0FD4
		public override void OnEnter()
		{
			base.OnEnteriTween(this.gameObject);
			if (this.loopType != iTween.LoopType.none)
			{
				base.IsLoop(true);
			}
			this.DoiTween();
		}

		// Token: 0x06006A5E RID: 27230 RVA: 0x001F2E05 File Offset: 0x001F1005
		public override void OnExit()
		{
			base.OnExitiTween(this.gameObject);
		}

		// Token: 0x06006A5F RID: 27231 RVA: 0x001F2E14 File Offset: 0x001F1014
		private void DoiTween()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector = (!this.vectorPosition.IsNone) ? this.vectorPosition.Value : Vector3.zero;
			if (!this.transformPosition.IsNone && this.transformPosition.Value)
			{
				vector = ((this.space != null && !(ownerDefaultTarget.transform.parent == null)) ? (ownerDefaultTarget.transform.parent.InverseTransformPoint(this.transformPosition.Value.transform.position) + vector) : (this.transformPosition.Value.transform.position + vector));
			}
			Hashtable hashtable = new Hashtable();
			hashtable.Add("position", vector);
			hashtable.Add((!this.speed.IsNone) ? "speed" : "time", (!this.speed.IsNone) ? this.speed.Value : ((!this.time.IsNone) ? this.time.Value : 1f));
			hashtable.Add("delay", (!this.delay.IsNone) ? this.delay.Value : 0f);
			hashtable.Add("easetype", this.easeType);
			hashtable.Add("looptype", this.loopType);
			hashtable.Add("oncomplete", "iTweenOnComplete");
			hashtable.Add("oncompleteparams", this.itweenID);
			hashtable.Add("onstart", "iTweenOnStart");
			hashtable.Add("onstartparams", this.itweenID);
			hashtable.Add("ignoretimescale", !this.realTime.IsNone && this.realTime.Value);
			hashtable.Add("islocal", this.space == 1);
			hashtable.Add("name", (!this.id.IsNone) ? this.id.Value : string.Empty);
			hashtable.Add("axis", (this.axis != iTweenFsmAction.AxisRestriction.none) ? Enum.GetName(typeof(iTweenFsmAction.AxisRestriction), this.axis) : string.Empty);
			if (!this.orientToPath.IsNone)
			{
				hashtable.Add("orienttopath", this.orientToPath.Value);
			}
			if (!this.lookAtObject.IsNone)
			{
				hashtable.Add("looktarget", (!this.lookAtVector.IsNone) ? (this.lookAtObject.Value.transform.position + this.lookAtVector.Value) : this.lookAtObject.Value.transform.position);
			}
			else if (!this.lookAtVector.IsNone)
			{
				hashtable.Add("looktarget", this.lookAtVector.Value);
			}
			if (!this.lookAtObject.IsNone || !this.lookAtVector.IsNone)
			{
				hashtable.Add("looktime", (!this.lookTime.IsNone) ? this.lookTime.Value : 0f);
			}
			this.itweenType = "move";
			iTween.MoveFrom(ownerDefaultTarget, hashtable);
		}

		// Token: 0x04005262 RID: 21090
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005263 RID: 21091
		[Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		// Token: 0x04005264 RID: 21092
		[Tooltip("Move From a transform rotation.")]
		public FsmGameObject transformPosition;

		// Token: 0x04005265 RID: 21093
		[Tooltip("The position the GameObject will animate from. If Transform Position is defined this is used as a local offset.")]
		public FsmVector3 vectorPosition;

		// Token: 0x04005266 RID: 21094
		[Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		// Token: 0x04005267 RID: 21095
		[Tooltip("The time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		// Token: 0x04005268 RID: 21096
		[Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
		public FsmFloat speed;

		// Token: 0x04005269 RID: 21097
		[Tooltip("The shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;

		// Token: 0x0400526A RID: 21098
		[Tooltip("The type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;

		// Token: 0x0400526B RID: 21099
		[Tooltip("Whether to animate in local or world space.")]
		public Space space;

		// Token: 0x0400526C RID: 21100
		[ActionSection("LookAt")]
		[Tooltip("Whether or not the GameObject will orient to its direction of travel. False by default.")]
		public FsmBool orientToPath;

		// Token: 0x0400526D RID: 21101
		[Tooltip("A target object the GameObject will look at.")]
		public FsmGameObject lookAtObject;

		// Token: 0x0400526E RID: 21102
		[Tooltip("A target position the GameObject will look at.")]
		public FsmVector3 lookAtVector;

		// Token: 0x0400526F RID: 21103
		[Tooltip("The time in seconds the object will take to look at either the Look At Target or Orient To Path. 0 by default")]
		public FsmFloat lookTime;

		// Token: 0x04005270 RID: 21104
		[Tooltip("Restricts rotation to the supplied axis only.")]
		public iTweenFsmAction.AxisRestriction axis;
	}
}
