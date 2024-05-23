using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D29 RID: 3369
	[ActionCategory("iTween")]
	[Tooltip("Changes a GameObject's position over time to a supplied destination.")]
	public class iTweenMoveTo : iTweenFsmAction
	{
		// Token: 0x06006A61 RID: 27233 RVA: 0x001F3220 File Offset: 0x001F1420
		public override void OnDrawGizmos()
		{
			if (this.transforms.Length >= 2)
			{
				this.tempVct3 = new Vector3[this.transforms.Length];
				for (int i = 0; i < this.transforms.Length; i++)
				{
					if (this.transforms[i].IsNone)
					{
						this.tempVct3[i] = ((!this.vectors[i].IsNone) ? this.vectors[i].Value : Vector3.zero);
					}
					else if (this.transforms[i].Value == null)
					{
						this.tempVct3[i] = ((!this.vectors[i].IsNone) ? this.vectors[i].Value : Vector3.zero);
					}
					else
					{
						this.tempVct3[i] = this.transforms[i].Value.transform.position + ((!this.vectors[i].IsNone) ? this.vectors[i].Value : Vector3.zero);
					}
				}
				iTween.DrawPathGizmos(this.tempVct3, Color.yellow);
			}
		}

		// Token: 0x06006A62 RID: 27234 RVA: 0x001F3378 File Offset: 0x001F1578
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
			this.lookTime = new FsmFloat
			{
				UseVariable = true
			};
			this.moveToPath = true;
			this.lookAhead = new FsmFloat
			{
				UseVariable = true
			};
			this.transforms = new FsmGameObject[0];
			this.vectors = new FsmVector3[0];
			this.tempVct3 = new Vector3[0];
			this.axis = iTweenFsmAction.AxisRestriction.none;
			this.reverse = false;
		}

		// Token: 0x06006A63 RID: 27235 RVA: 0x001F34B4 File Offset: 0x001F16B4
		public override void OnEnter()
		{
			base.OnEnteriTween(this.gameObject);
			if (this.loopType != iTween.LoopType.none)
			{
				base.IsLoop(true);
			}
			this.DoiTween();
		}

		// Token: 0x06006A64 RID: 27236 RVA: 0x001F34E5 File Offset: 0x001F16E5
		public override void OnExit()
		{
			base.OnExitiTween(this.gameObject);
		}

		// Token: 0x06006A65 RID: 27237 RVA: 0x001F34F4 File Offset: 0x001F16F4
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
			hashtable.Add("name", (!this.id.IsNone) ? this.id.Value : string.Empty);
			hashtable.Add("islocal", this.space == 1);
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
			if (this.transforms.Length >= 2)
			{
				this.tempVct3 = new Vector3[this.transforms.Length];
				if (!this.reverse.IsNone && this.reverse.Value)
				{
					for (int i = 0; i < this.transforms.Length; i++)
					{
						if (this.transforms[i].IsNone)
						{
							this.tempVct3[this.tempVct3.Length - 1 - i] = ((!this.vectors[i].IsNone) ? this.vectors[i].Value : Vector3.zero);
						}
						else if (this.transforms[i].Value == null)
						{
							this.tempVct3[this.tempVct3.Length - 1 - i] = ((!this.vectors[i].IsNone) ? this.vectors[i].Value : Vector3.zero);
						}
						else
						{
							this.tempVct3[this.tempVct3.Length - 1 - i] = ((this.space != null) ? this.transforms[i].Value.transform.localPosition : this.transforms[i].Value.transform.position) + ((!this.vectors[i].IsNone) ? this.vectors[i].Value : Vector3.zero);
						}
					}
				}
				else
				{
					for (int j = 0; j < this.transforms.Length; j++)
					{
						if (this.transforms[j].IsNone)
						{
							this.tempVct3[j] = ((!this.vectors[j].IsNone) ? this.vectors[j].Value : Vector3.zero);
						}
						else if (this.transforms[j].Value == null)
						{
							this.tempVct3[j] = ((!this.vectors[j].IsNone) ? this.vectors[j].Value : Vector3.zero);
						}
						else
						{
							this.tempVct3[j] = ((this.space != null) ? ownerDefaultTarget.transform.parent.InverseTransformPoint(this.transforms[j].Value.transform.position) : this.transforms[j].Value.transform.position) + ((!this.vectors[j].IsNone) ? this.vectors[j].Value : Vector3.zero);
						}
					}
				}
				hashtable.Add("path", this.tempVct3);
				hashtable.Add("movetopath", this.moveToPath.IsNone || this.moveToPath.Value);
				hashtable.Add("lookahead", (!this.lookAhead.IsNone) ? this.lookAhead.Value : 1f);
			}
			this.itweenType = "move";
			iTween.MoveTo(ownerDefaultTarget, hashtable);
		}

		// Token: 0x04005271 RID: 21105
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005272 RID: 21106
		[Tooltip("iTween ID. If set you can use iTween Stop action to stop it by its id.")]
		public FsmString id;

		// Token: 0x04005273 RID: 21107
		[Tooltip("Move To a transform position.")]
		public FsmGameObject transformPosition;

		// Token: 0x04005274 RID: 21108
		[Tooltip("Position the GameObject will animate to. If Transform Position is defined this is used as a local offset.")]
		public FsmVector3 vectorPosition;

		// Token: 0x04005275 RID: 21109
		[Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		// Token: 0x04005276 RID: 21110
		[Tooltip("The time in seconds the animation will wait before beginning.")]
		public FsmFloat delay;

		// Token: 0x04005277 RID: 21111
		[Tooltip("Can be used instead of time to allow animation based on speed. When you define speed the time variable is ignored.")]
		public FsmFloat speed;

		// Token: 0x04005278 RID: 21112
		[Tooltip("Whether to animate in local or world space.")]
		public Space space;

		// Token: 0x04005279 RID: 21113
		[Tooltip("The shape of the easing curve applied to the animation.")]
		public iTween.EaseType easeType = iTween.EaseType.linear;

		// Token: 0x0400527A RID: 21114
		[Tooltip("The type of loop to apply once the animation has completed.")]
		public iTween.LoopType loopType;

		// Token: 0x0400527B RID: 21115
		[Tooltip("Whether or not the GameObject will orient to its direction of travel. False by default.")]
		[ActionSection("LookAt")]
		public FsmBool orientToPath;

		// Token: 0x0400527C RID: 21116
		[Tooltip("A target object the GameObject will look at.")]
		public FsmGameObject lookAtObject;

		// Token: 0x0400527D RID: 21117
		[Tooltip("A target position the GameObject will look at.")]
		public FsmVector3 lookAtVector;

		// Token: 0x0400527E RID: 21118
		[Tooltip("The time in seconds the object will take to look at either the Look Target or Orient To Path. 0 by default")]
		public FsmFloat lookTime;

		// Token: 0x0400527F RID: 21119
		[Tooltip("Restricts rotation to the supplied axis only.")]
		public iTweenFsmAction.AxisRestriction axis;

		// Token: 0x04005280 RID: 21120
		[ActionSection("Path")]
		[Tooltip("Whether to automatically generate a curve from the GameObject's current position to the beginning of the path. True by default.")]
		public FsmBool moveToPath;

		// Token: 0x04005281 RID: 21121
		[Tooltip("How much of a percentage (from 0 to 1) to look ahead on a path to influence how strict Orient To Path is and how much the object will anticipate each curve.")]
		public FsmFloat lookAhead;

		// Token: 0x04005282 RID: 21122
		[CompoundArray("Path Nodes", "Transform", "Vector")]
		[Tooltip("A list of objects to draw a Catmull-Rom spline through for a curved animation path.")]
		public FsmGameObject[] transforms;

		// Token: 0x04005283 RID: 21123
		[Tooltip("A list of positions to draw a Catmull-Rom through for a curved animation path. If Transform is defined, this value is added as a local offset.")]
		public FsmVector3[] vectors;

		// Token: 0x04005284 RID: 21124
		[Tooltip("Reverse the path so object moves from End to Start node.")]
		public FsmBool reverse;

		// Token: 0x04005285 RID: 21125
		private Vector3[] tempVct3;
	}
}
