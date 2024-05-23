using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D35 RID: 3381
	[ActionCategory("iTween")]
	[Tooltip("Similar to RotateTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a 'live' set of changing values. Does not utilize an EaseType.")]
	public class iTweenRotateUpdate : FsmStateAction
	{
		// Token: 0x06006A98 RID: 27288 RVA: 0x001F5794 File Offset: 0x001F3994
		public override void Reset()
		{
			this.transformRotation = new FsmGameObject
			{
				UseVariable = true
			};
			this.vectorRotation = new FsmVector3
			{
				UseVariable = true
			};
			this.time = 1f;
			this.space = 0;
		}

		// Token: 0x06006A99 RID: 27289 RVA: 0x001F57E0 File Offset: 0x001F39E0
		public override void OnEnter()
		{
			this.hash = new Hashtable();
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				base.Finish();
				return;
			}
			if (this.transformRotation.IsNone)
			{
				this.hash.Add("rotation", (!this.vectorRotation.IsNone) ? this.vectorRotation.Value : Vector3.zero);
			}
			else if (this.vectorRotation.IsNone)
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform);
			}
			else if (this.space == null)
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform.eulerAngles + this.vectorRotation.Value);
			}
			else
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform.localEulerAngles + this.vectorRotation.Value);
			}
			this.hash.Add("time", (!this.time.IsNone) ? this.time.Value : 1f);
			this.hash.Add("islocal", this.space == 1);
			this.DoiTween();
		}

		// Token: 0x06006A9A RID: 27290 RVA: 0x001F598D File Offset: 0x001F3B8D
		public override void OnExit()
		{
		}

		// Token: 0x06006A9B RID: 27291 RVA: 0x001F5990 File Offset: 0x001F3B90
		public override void OnUpdate()
		{
			this.hash.Remove("rotation");
			if (this.transformRotation.IsNone)
			{
				this.hash.Add("rotation", (!this.vectorRotation.IsNone) ? this.vectorRotation.Value : Vector3.zero);
			}
			else if (this.vectorRotation.IsNone)
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform);
			}
			else if (this.space == null)
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform.eulerAngles + this.vectorRotation.Value);
			}
			else
			{
				this.hash.Add("rotation", this.transformRotation.Value.transform.localEulerAngles + this.vectorRotation.Value);
			}
			this.DoiTween();
		}

		// Token: 0x06006A9C RID: 27292 RVA: 0x001F5ABB File Offset: 0x001F3CBB
		private void DoiTween()
		{
			iTween.RotateUpdate(this.go, this.hash);
		}

		// Token: 0x040052DE RID: 21214
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040052DF RID: 21215
		[Tooltip("Rotate to a transform rotation.")]
		public FsmGameObject transformRotation;

		// Token: 0x040052E0 RID: 21216
		[Tooltip("A rotation the GameObject will animate from.")]
		public FsmVector3 vectorRotation;

		// Token: 0x040052E1 RID: 21217
		[Tooltip("The time in seconds the animation will take to complete. If transformRotation is set, this is used as an offset.")]
		public FsmFloat time;

		// Token: 0x040052E2 RID: 21218
		[Tooltip("Whether to animate in local or world space.")]
		public Space space;

		// Token: 0x040052E3 RID: 21219
		private Hashtable hash;

		// Token: 0x040052E4 RID: 21220
		private GameObject go;
	}
}
