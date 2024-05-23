using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C7F RID: 3199
	[ActionCategory(14)]
	[Tooltip("Rotates a Game Object around each Axis. Use a Vector3 Variable and/or XYZ components. To leave any axis unchanged, set variable to 'None'.")]
	public class Rotate : FsmStateAction
	{
		// Token: 0x06006770 RID: 26480 RVA: 0x001E7F48 File Offset: 0x001E6148
		public override void Reset()
		{
			this.gameObject = null;
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
			this.space = 1;
			this.perSecond = false;
			this.everyFrame = true;
			this.lateUpdate = false;
			this.fixedUpdate = false;
		}

		// Token: 0x06006771 RID: 26481 RVA: 0x001E7FC2 File Offset: 0x001E61C2
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006772 RID: 26482 RVA: 0x001E7FD0 File Offset: 0x001E61D0
		public override void OnEnter()
		{
			if (!this.everyFrame && !this.lateUpdate && !this.fixedUpdate)
			{
				this.DoRotate();
				base.Finish();
			}
		}

		// Token: 0x06006773 RID: 26483 RVA: 0x001E7FFF File Offset: 0x001E61FF
		public override void OnUpdate()
		{
			if (!this.lateUpdate && !this.fixedUpdate)
			{
				this.DoRotate();
			}
		}

		// Token: 0x06006774 RID: 26484 RVA: 0x001E8020 File Offset: 0x001E6220
		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoRotate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006775 RID: 26485 RVA: 0x001E8050 File Offset: 0x001E6250
		public override void OnFixedUpdate()
		{
			if (this.fixedUpdate)
			{
				this.DoRotate();
			}
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006776 RID: 26486 RVA: 0x001E8080 File Offset: 0x001E6280
		private void DoRotate()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			Vector3 vector = (!this.vector.IsNone) ? this.vector.Value : new Vector3(this.xAngle.Value, this.yAngle.Value, this.zAngle.Value);
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
			if (!this.perSecond)
			{
				ownerDefaultTarget.transform.Rotate(vector, this.space);
			}
			else
			{
				ownerDefaultTarget.transform.Rotate(vector * Time.deltaTime, this.space);
			}
		}

		// Token: 0x04004F43 RID: 20291
		[Tooltip("The game object to rotate.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004F44 RID: 20292
		[UIHint(10)]
		[Tooltip("A rotation vector. NOTE: You can override individual axis below.")]
		public FsmVector3 vector;

		// Token: 0x04004F45 RID: 20293
		[Tooltip("Rotation around x axis.")]
		public FsmFloat xAngle;

		// Token: 0x04004F46 RID: 20294
		[Tooltip("Rotation around y axis.")]
		public FsmFloat yAngle;

		// Token: 0x04004F47 RID: 20295
		[Tooltip("Rotation around z axis.")]
		public FsmFloat zAngle;

		// Token: 0x04004F48 RID: 20296
		[Tooltip("Rotate in local or world space.")]
		public Space space;

		// Token: 0x04004F49 RID: 20297
		[Tooltip("Rotate over one second")]
		public bool perSecond;

		// Token: 0x04004F4A RID: 20298
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;

		// Token: 0x04004F4B RID: 20299
		[Tooltip("Perform the rotation in LateUpdate. This is useful if you want to override the rotation of objects that are animated or otherwise rotated in Update.")]
		public bool lateUpdate;

		// Token: 0x04004F4C RID: 20300
		[Tooltip("Perform the rotation in FixedUpdate. This is useful when working with rigid bodies and physics.")]
		public bool fixedUpdate;
	}
}
