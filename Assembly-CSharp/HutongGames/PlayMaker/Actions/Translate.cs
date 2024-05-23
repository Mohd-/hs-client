using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000D08 RID: 3336
	[Tooltip("Translates a Game Object. Use a Vector3 variable and/or XYZ components. To leave any axis unchanged, set variable to 'None'.")]
	[ActionCategory(14)]
	public class Translate : FsmStateAction
	{
		// Token: 0x060069D0 RID: 27088 RVA: 0x001F0014 File Offset: 0x001EE214
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
			this.space = 1;
			this.perSecond = true;
			this.everyFrame = true;
			this.lateUpdate = false;
			this.fixedUpdate = false;
		}

		// Token: 0x060069D1 RID: 27089 RVA: 0x001F008E File Offset: 0x001EE28E
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x060069D2 RID: 27090 RVA: 0x001F009C File Offset: 0x001EE29C
		public override void OnEnter()
		{
			if (!this.everyFrame && !this.lateUpdate && !this.fixedUpdate)
			{
				this.DoTranslate();
				base.Finish();
			}
		}

		// Token: 0x060069D3 RID: 27091 RVA: 0x001F00CB File Offset: 0x001EE2CB
		public override void OnUpdate()
		{
			if (!this.lateUpdate && !this.fixedUpdate)
			{
				this.DoTranslate();
			}
		}

		// Token: 0x060069D4 RID: 27092 RVA: 0x001F00EC File Offset: 0x001EE2EC
		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoTranslate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060069D5 RID: 27093 RVA: 0x001F011C File Offset: 0x001EE31C
		public override void OnFixedUpdate()
		{
			if (this.fixedUpdate)
			{
				this.DoTranslate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060069D6 RID: 27094 RVA: 0x001F014C File Offset: 0x001EE34C
		private void DoTranslate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector = (!this.vector.IsNone) ? this.vector.Value : new Vector3(this.x.Value, this.y.Value, this.z.Value);
			if (!this.x.IsNone)
			{
				vector.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				vector.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				vector.z = this.z.Value;
			}
			if (!this.perSecond)
			{
				ownerDefaultTarget.transform.Translate(vector, this.space);
			}
			else
			{
				ownerDefaultTarget.transform.Translate(vector * Time.deltaTime, this.space);
			}
		}

		// Token: 0x040051A7 RID: 20903
		[RequiredField]
		[Tooltip("The game object to translate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040051A8 RID: 20904
		[Tooltip("A translation vector. NOTE: You can override individual axis below.")]
		[UIHint(10)]
		public FsmVector3 vector;

		// Token: 0x040051A9 RID: 20905
		[Tooltip("Translation along x axis.")]
		public FsmFloat x;

		// Token: 0x040051AA RID: 20906
		[Tooltip("Translation along y axis.")]
		public FsmFloat y;

		// Token: 0x040051AB RID: 20907
		[Tooltip("Translation along z axis.")]
		public FsmFloat z;

		// Token: 0x040051AC RID: 20908
		[Tooltip("Translate in local or world space.")]
		public Space space;

		// Token: 0x040051AD RID: 20909
		[Tooltip("Translate over one second")]
		public bool perSecond;

		// Token: 0x040051AE RID: 20910
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040051AF RID: 20911
		[Tooltip("Perform the translate in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;

		// Token: 0x040051B0 RID: 20912
		[Tooltip("Perform the translate in FixedUpdate. This is useful when working with rigid bodies and physics.")]
		public bool fixedUpdate;
	}
}
