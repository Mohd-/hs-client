using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE2 RID: 3298
	[Tooltip("Sets the Rotation of a Game Object. To leave any axis unchanged, set variable to 'None'.")]
	[ActionCategory(14)]
	public class SetRotation : FsmStateAction
	{
		// Token: 0x0600692D RID: 26925 RVA: 0x001ED408 File Offset: 0x001EB608
		public override void Reset()
		{
			this.gameObject = null;
			this.quaternion = null;
			this.vector = null;
			this.xAngle = new FsmFloat
			{
				UseVariable = true
			};
			this.yAngle = new FsmFloat
			{
				UseVariable = true
			};
			this.zAngle = new FsmFloat
			{
				UseVariable = true
			};
			this.space = 0;
			this.everyFrame = false;
			this.lateUpdate = false;
		}

		// Token: 0x0600692E RID: 26926 RVA: 0x001ED47C File Offset: 0x001EB67C
		public override void OnEnter()
		{
			if (!this.everyFrame && !this.lateUpdate)
			{
				this.DoSetRotation();
				base.Finish();
			}
		}

		// Token: 0x0600692F RID: 26927 RVA: 0x001ED4AB File Offset: 0x001EB6AB
		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoSetRotation();
			}
		}

		// Token: 0x06006930 RID: 26928 RVA: 0x001ED4C0 File Offset: 0x001EB6C0
		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoSetRotation();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006931 RID: 26929 RVA: 0x001ED4F0 File Offset: 0x001EB6F0
		private void DoSetRotation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector;
			if (!this.quaternion.IsNone)
			{
				vector = this.quaternion.Value.eulerAngles;
			}
			else if (!this.vector.IsNone)
			{
				vector = this.vector.Value;
			}
			else
			{
				vector = ((this.space != 1) ? ownerDefaultTarget.transform.eulerAngles : ownerDefaultTarget.transform.localEulerAngles);
			}
			if (!this.xAngle.IsNone)
			{
				vector.x = this.xAngle.Value;
			}
			if (!this.yAngle.IsNone)
			{
				vector.y = this.yAngle.Value;
			}
			if (!this.zAngle.IsNone)
			{
				vector.z = this.zAngle.Value;
			}
			if (this.space == 1)
			{
				ownerDefaultTarget.transform.localEulerAngles = vector;
			}
			else
			{
				ownerDefaultTarget.transform.eulerAngles = vector;
			}
		}

		// Token: 0x040050DE RID: 20702
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040050DF RID: 20703
		[UIHint(10)]
		[Tooltip("Use a stored quaternion, or vector angles below.")]
		public FsmQuaternion quaternion;

		// Token: 0x040050E0 RID: 20704
		[Title("Euler Angles")]
		[UIHint(10)]
		[Tooltip("Use euler angles stored in a Vector3 variable, and/or set each axis below.")]
		public FsmVector3 vector;

		// Token: 0x040050E1 RID: 20705
		public FsmFloat xAngle;

		// Token: 0x040050E2 RID: 20706
		public FsmFloat yAngle;

		// Token: 0x040050E3 RID: 20707
		public FsmFloat zAngle;

		// Token: 0x040050E4 RID: 20708
		[Tooltip("Use local or world space.")]
		public Space space;

		// Token: 0x040050E5 RID: 20709
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x040050E6 RID: 20710
		[Tooltip("Perform in LateUpdate. This is useful if you want to override the position of objects that are animated or otherwise positioned in Update.")]
		public bool lateUpdate;
	}
}
