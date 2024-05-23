using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE3 RID: 3299
	[Tooltip("Sets the Scale of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	[ActionCategory(14)]
	public class SetScale : FsmStateAction
	{
		// Token: 0x06006933 RID: 26931 RVA: 0x001ED628 File Offset: 0x001EB828
		public override void Reset()
		{
			this.gameObject = null;
			this.vector = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.z = new FsmFloat
			{
				UseVariable = true
			};
			this.everyFrame = false;
			this.lateUpdate = false;
		}

		// Token: 0x06006934 RID: 26932 RVA: 0x001ED68D File Offset: 0x001EB88D
		public override void OnEnter()
		{
			this.DoSetScale();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006935 RID: 26933 RVA: 0x001ED6A6 File Offset: 0x001EB8A6
		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoSetScale();
			}
		}

		// Token: 0x06006936 RID: 26934 RVA: 0x001ED6BC File Offset: 0x001EB8BC
		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoSetScale();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006937 RID: 26935 RVA: 0x001ED6EC File Offset: 0x001EB8EC
		private void DoSetScale()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 localScale = (!this.vector.IsNone) ? this.vector.Value : ownerDefaultTarget.transform.localScale;
			if (!this.x.IsNone)
			{
				localScale.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				localScale.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				localScale.z = this.z.Value;
			}
			ownerDefaultTarget.transform.localScale = localScale;
		}

		// Token: 0x040050E7 RID: 20711
		[RequiredField]
		[Tooltip("The GameObject to scale.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040050E8 RID: 20712
		[Tooltip("Use stored Vector3 value, and/or set each axis below.")]
		[UIHint(10)]
		public FsmVector3 vector;

		// Token: 0x040050E9 RID: 20713
		public FsmFloat x;

		// Token: 0x040050EA RID: 20714
		public FsmFloat y;

		// Token: 0x040050EB RID: 20715
		public FsmFloat z;

		// Token: 0x040050EC RID: 20716
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040050ED RID: 20717
		[Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;
	}
}
