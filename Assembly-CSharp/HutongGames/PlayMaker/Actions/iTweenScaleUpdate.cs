using System;
using System.Collections;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D3A RID: 3386
	[Tooltip("CSimilar to ScaleTo but incredibly less expensive for usage inside the Update function or similar looping situations involving a 'live' set of changing values. Does not utilize an EaseType.")]
	[ActionCategory("iTween")]
	public class iTweenScaleUpdate : FsmStateAction
	{
		// Token: 0x06006AB2 RID: 27314 RVA: 0x001F66D0 File Offset: 0x001F48D0
		public override void Reset()
		{
			this.transformScale = new FsmGameObject
			{
				UseVariable = true
			};
			this.vectorScale = new FsmVector3
			{
				UseVariable = true
			};
			this.time = 1f;
		}

		// Token: 0x06006AB3 RID: 27315 RVA: 0x001F6718 File Offset: 0x001F4918
		public override void OnEnter()
		{
			this.hash = new Hashtable();
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				base.Finish();
				return;
			}
			if (this.transformScale.IsNone)
			{
				this.hash.Add("scale", (!this.vectorScale.IsNone) ? this.vectorScale.Value : Vector3.zero);
			}
			else if (this.vectorScale.IsNone)
			{
				this.hash.Add("scale", this.transformScale.Value.transform);
			}
			else
			{
				this.hash.Add("scale", this.transformScale.Value.transform.localScale + this.vectorScale.Value);
			}
			this.hash.Add("time", (!this.time.IsNone) ? this.time.Value : 1f);
			this.DoiTween();
		}

		// Token: 0x06006AB4 RID: 27316 RVA: 0x001F685D File Offset: 0x001F4A5D
		public override void OnExit()
		{
		}

		// Token: 0x06006AB5 RID: 27317 RVA: 0x001F6860 File Offset: 0x001F4A60
		public override void OnUpdate()
		{
			this.hash.Remove("scale");
			if (this.transformScale.IsNone)
			{
				this.hash.Add("scale", (!this.vectorScale.IsNone) ? this.vectorScale.Value : Vector3.zero);
			}
			else if (this.vectorScale.IsNone)
			{
				this.hash.Add("scale", this.transformScale.Value.transform);
			}
			else
			{
				this.hash.Add("scale", this.transformScale.Value.transform.localScale + this.vectorScale.Value);
			}
			this.DoiTween();
		}

		// Token: 0x06006AB6 RID: 27318 RVA: 0x001F6941 File Offset: 0x001F4B41
		private void DoiTween()
		{
			iTween.ScaleUpdate(this.go, this.hash);
		}

		// Token: 0x04005307 RID: 21255
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005308 RID: 21256
		[Tooltip("Scale To a transform scale.")]
		public FsmGameObject transformScale;

		// Token: 0x04005309 RID: 21257
		[Tooltip("A scale vector the GameObject will animate To.")]
		public FsmVector3 vectorScale;

		// Token: 0x0400530A RID: 21258
		[Tooltip("The time in seconds the animation will take to complete. If transformScale is set, this is used as an offset.")]
		public FsmFloat time;

		// Token: 0x0400530B RID: 21259
		private Hashtable hash;

		// Token: 0x0400530C RID: 21260
		private GameObject go;
	}
}
