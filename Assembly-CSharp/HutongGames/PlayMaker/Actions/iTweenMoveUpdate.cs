using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D2A RID: 3370
	[Tooltip("Similar to MoveTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a 'live' set of changing values. Does not utilize an EaseType.")]
	[ActionCategory("iTween")]
	public class iTweenMoveUpdate : FsmStateAction
	{
		// Token: 0x06006A67 RID: 27239 RVA: 0x001F3C7C File Offset: 0x001F1E7C
		public override void Reset()
		{
			this.transformPosition = new FsmGameObject
			{
				UseVariable = true
			};
			this.vectorPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.time = 1f;
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

		// Token: 0x06006A68 RID: 27240 RVA: 0x001F3D1C File Offset: 0x001F1F1C
		public override void OnEnter()
		{
			this.hash = new Hashtable();
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				base.Finish();
				return;
			}
			if (this.transformPosition.IsNone)
			{
				this.hash.Add("position", (!this.vectorPosition.IsNone) ? this.vectorPosition.Value : Vector3.zero);
			}
			else if (this.vectorPosition.IsNone)
			{
				this.hash.Add("position", this.transformPosition.Value.transform);
			}
			else if (this.space == null || this.go.transform.parent == null)
			{
				this.hash.Add("position", this.transformPosition.Value.transform.position + this.vectorPosition.Value);
			}
			else
			{
				this.hash.Add("position", this.go.transform.parent.InverseTransformPoint(this.transformPosition.Value.transform.position) + this.vectorPosition.Value);
			}
			this.hash.Add("time", (!this.time.IsNone) ? this.time.Value : 1f);
			this.hash.Add("islocal", this.space == 1);
			this.hash.Add("axis", (this.axis != iTweenFsmAction.AxisRestriction.none) ? Enum.GetName(typeof(iTweenFsmAction.AxisRestriction), this.axis) : string.Empty);
			if (!this.orientToPath.IsNone)
			{
				this.hash.Add("orienttopath", this.orientToPath.Value);
			}
			if (this.lookAtObject.IsNone)
			{
				if (!this.lookAtVector.IsNone)
				{
					this.hash.Add("looktarget", this.lookAtVector.Value);
				}
			}
			else
			{
				this.hash.Add("looktarget", this.lookAtObject.Value.transform);
			}
			if (!this.lookAtObject.IsNone || !this.lookAtVector.IsNone)
			{
				this.hash.Add("looktime", (!this.lookTime.IsNone) ? this.lookTime.Value : 0f);
			}
			this.DoiTween();
		}

		// Token: 0x06006A69 RID: 27241 RVA: 0x001F4028 File Offset: 0x001F2228
		public override void OnUpdate()
		{
			this.hash.Remove("position");
			if (this.transformPosition.IsNone)
			{
				this.hash.Add("position", (!this.vectorPosition.IsNone) ? this.vectorPosition.Value : Vector3.zero);
			}
			else if (this.vectorPosition.IsNone)
			{
				this.hash.Add("position", this.transformPosition.Value.transform);
			}
			else if (this.space == null)
			{
				this.hash.Add("position", this.transformPosition.Value.transform.position + this.vectorPosition.Value);
			}
			else
			{
				this.hash.Add("position", this.transformPosition.Value.transform.localPosition + this.vectorPosition.Value);
			}
			this.DoiTween();
		}

		// Token: 0x06006A6A RID: 27242 RVA: 0x001F4153 File Offset: 0x001F2353
		public override void OnExit()
		{
		}

		// Token: 0x06006A6B RID: 27243 RVA: 0x001F4155 File Offset: 0x001F2355
		private void DoiTween()
		{
			iTween.MoveUpdate(this.go, this.hash);
		}

		// Token: 0x04005286 RID: 21126
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005287 RID: 21127
		[Tooltip("Move From a transform rotation.")]
		public FsmGameObject transformPosition;

		// Token: 0x04005288 RID: 21128
		[Tooltip("The position the GameObject will animate from.  If transformPosition is set, this is used as an offset.")]
		public FsmVector3 vectorPosition;

		// Token: 0x04005289 RID: 21129
		[Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		// Token: 0x0400528A RID: 21130
		[Tooltip("Whether to animate in local or world space.")]
		public Space space;

		// Token: 0x0400528B RID: 21131
		[ActionSection("LookAt")]
		[Tooltip("Whether or not the GameObject will orient to its direction of travel. False by default.")]
		public FsmBool orientToPath;

		// Token: 0x0400528C RID: 21132
		[Tooltip("A target object the GameObject will look at.")]
		public FsmGameObject lookAtObject;

		// Token: 0x0400528D RID: 21133
		[Tooltip("A target position the GameObject will look at.")]
		public FsmVector3 lookAtVector;

		// Token: 0x0400528E RID: 21134
		[Tooltip("The time in seconds the object will take to look at either the Look At Target or Orient To Path. 0 by default")]
		public FsmFloat lookTime;

		// Token: 0x0400528F RID: 21135
		[Tooltip("Restricts rotation to the supplied axis only.")]
		public iTweenFsmAction.AxisRestriction axis;

		// Token: 0x04005290 RID: 21136
		private Hashtable hash;

		// Token: 0x04005291 RID: 21137
		private GameObject go;
	}
}
