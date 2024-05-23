using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C2A RID: 3114
	[Tooltip("Gets the Rotation of a Game Object and stores it in a Vector3 Variable or each Axis in a Float Variable")]
	[ActionCategory(14)]
	public class GetRotation : FsmStateAction
	{
		// Token: 0x06006609 RID: 26121 RVA: 0x001E35B0 File Offset: 0x001E17B0
		public override void Reset()
		{
			this.gameObject = null;
			this.quaternion = null;
			this.vector = null;
			this.xAngle = null;
			this.yAngle = null;
			this.zAngle = null;
			this.space = 0;
			this.everyFrame = false;
		}

		// Token: 0x0600660A RID: 26122 RVA: 0x001E35F5 File Offset: 0x001E17F5
		public override void OnEnter()
		{
			this.DoGetRotation();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600660B RID: 26123 RVA: 0x001E360E File Offset: 0x001E180E
		public override void OnUpdate()
		{
			this.DoGetRotation();
		}

		// Token: 0x0600660C RID: 26124 RVA: 0x001E3618 File Offset: 0x001E1818
		private void DoGetRotation()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.space == null)
			{
				this.quaternion.Value = ownerDefaultTarget.transform.rotation;
				Vector3 eulerAngles = ownerDefaultTarget.transform.eulerAngles;
				this.vector.Value = eulerAngles;
				this.xAngle.Value = eulerAngles.x;
				this.yAngle.Value = eulerAngles.y;
				this.zAngle.Value = eulerAngles.z;
			}
			else
			{
				Vector3 localEulerAngles = ownerDefaultTarget.transform.localEulerAngles;
				this.quaternion.Value = Quaternion.Euler(localEulerAngles);
				this.vector.Value = localEulerAngles;
				this.xAngle.Value = localEulerAngles.x;
				this.yAngle.Value = localEulerAngles.y;
				this.zAngle.Value = localEulerAngles.z;
			}
		}

		// Token: 0x04004DC4 RID: 19908
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004DC5 RID: 19909
		[UIHint(10)]
		public FsmQuaternion quaternion;

		// Token: 0x04004DC6 RID: 19910
		[Title("Euler Angles")]
		[UIHint(10)]
		public FsmVector3 vector;

		// Token: 0x04004DC7 RID: 19911
		[UIHint(10)]
		public FsmFloat xAngle;

		// Token: 0x04004DC8 RID: 19912
		[UIHint(10)]
		public FsmFloat yAngle;

		// Token: 0x04004DC9 RID: 19913
		[UIHint(10)]
		public FsmFloat zAngle;

		// Token: 0x04004DCA RID: 19914
		public Space space;

		// Token: 0x04004DCB RID: 19915
		public bool everyFrame;
	}
}
