using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D25 RID: 3365
	[Tooltip("Rotates a GameObject to look at a supplied Transform or Vector3 over time.")]
	[ActionCategory("iTween")]
	public class iTweenLookUpdate : FsmStateAction
	{
		// Token: 0x06006A4C RID: 27212 RVA: 0x001F2114 File Offset: 0x001F0314
		public override void Reset()
		{
			this.transformTarget = new FsmGameObject
			{
				UseVariable = true
			};
			this.vectorTarget = new FsmVector3
			{
				UseVariable = true
			};
			this.time = 1f;
			this.axis = iTweenFsmAction.AxisRestriction.none;
		}

		// Token: 0x06006A4D RID: 27213 RVA: 0x001F2160 File Offset: 0x001F0360
		public override void OnEnter()
		{
			this.hash = new Hashtable();
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				base.Finish();
				return;
			}
			if (this.transformTarget.IsNone)
			{
				this.hash.Add("looktarget", (!this.vectorTarget.IsNone) ? this.vectorTarget.Value : Vector3.zero);
			}
			else if (this.vectorTarget.IsNone)
			{
				this.hash.Add("looktarget", this.transformTarget.Value.transform);
			}
			else
			{
				this.hash.Add("looktarget", this.transformTarget.Value.transform.position + this.vectorTarget.Value);
			}
			this.hash.Add("time", (!this.time.IsNone) ? this.time.Value : 1f);
			this.hash.Add("axis", (this.axis != iTweenFsmAction.AxisRestriction.none) ? Enum.GetName(typeof(iTweenFsmAction.AxisRestriction), this.axis) : string.Empty);
			this.DoiTween();
		}

		// Token: 0x06006A4E RID: 27214 RVA: 0x001F22E4 File Offset: 0x001F04E4
		public override void OnExit()
		{
		}

		// Token: 0x06006A4F RID: 27215 RVA: 0x001F22E8 File Offset: 0x001F04E8
		public override void OnUpdate()
		{
			this.hash.Remove("looktarget");
			if (this.transformTarget.IsNone)
			{
				this.hash.Add("looktarget", (!this.vectorTarget.IsNone) ? this.vectorTarget.Value : Vector3.zero);
			}
			else if (this.vectorTarget.IsNone)
			{
				this.hash.Add("looktarget", this.transformTarget.Value.transform);
			}
			else
			{
				this.hash.Add("looktarget", this.transformTarget.Value.transform.position + this.vectorTarget.Value);
			}
			this.DoiTween();
		}

		// Token: 0x06006A50 RID: 27216 RVA: 0x001F23C9 File Offset: 0x001F05C9
		private void DoiTween()
		{
			iTween.LookUpdate(this.go, this.hash);
		}

		// Token: 0x0400523F RID: 21055
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005240 RID: 21056
		[Tooltip("Look at a transform position.")]
		public FsmGameObject transformTarget;

		// Token: 0x04005241 RID: 21057
		[Tooltip("A target position the GameObject will look at. If Transform Target is defined this is used as a look offset.")]
		public FsmVector3 vectorTarget;

		// Token: 0x04005242 RID: 21058
		[Tooltip("The time in seconds the animation will take to complete.")]
		public FsmFloat time;

		// Token: 0x04005243 RID: 21059
		[Tooltip("Restricts rotation to the supplied axis only. Just put there strinc like 'x' or 'xz'")]
		public iTweenFsmAction.AxisRestriction axis;

		// Token: 0x04005244 RID: 21060
		private Hashtable hash;

		// Token: 0x04005245 RID: 21061
		private GameObject go;
	}
}
