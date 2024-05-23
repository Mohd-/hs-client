using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF2 RID: 3314
	[Tooltip("Smoothly Rotates a Game Object so its forward vector points in the specified Direction.")]
	[ActionCategory(14)]
	public class SmoothLookAtDirection : FsmStateAction
	{
		// Token: 0x0600697A RID: 27002 RVA: 0x001EE7B4 File Offset: 0x001EC9B4
		public override void Reset()
		{
			this.gameObject = null;
			this.targetDirection = new FsmVector3
			{
				UseVariable = true
			};
			this.minMagnitude = 0.1f;
			this.upVector = new FsmVector3
			{
				UseVariable = true
			};
			this.keepVertical = true;
			this.speed = 5f;
			this.lateUpdate = true;
		}

		// Token: 0x0600697B RID: 27003 RVA: 0x001EE823 File Offset: 0x001ECA23
		public override void OnEnter()
		{
			this.previousGo = null;
		}

		// Token: 0x0600697C RID: 27004 RVA: 0x001EE82C File Offset: 0x001ECA2C
		public override void OnUpdate()
		{
			if (!this.lateUpdate)
			{
				this.DoSmoothLookAtDirection();
			}
		}

		// Token: 0x0600697D RID: 27005 RVA: 0x001EE83F File Offset: 0x001ECA3F
		public override void OnLateUpdate()
		{
			if (this.lateUpdate)
			{
				this.DoSmoothLookAtDirection();
			}
		}

		// Token: 0x0600697E RID: 27006 RVA: 0x001EE854 File Offset: 0x001ECA54
		private void DoSmoothLookAtDirection()
		{
			if (this.targetDirection.IsNone)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.previousGo != ownerDefaultTarget)
			{
				this.lastRotation = ownerDefaultTarget.transform.rotation;
				this.desiredRotation = this.lastRotation;
				this.previousGo = ownerDefaultTarget;
			}
			Vector3 value = this.targetDirection.Value;
			if (this.keepVertical.Value)
			{
				value.y = 0f;
			}
			if (value.sqrMagnitude > this.minMagnitude.Value)
			{
				this.desiredRotation = Quaternion.LookRotation(value, (!this.upVector.IsNone) ? this.upVector.Value : Vector3.up);
			}
			this.lastRotation = Quaternion.Slerp(this.lastRotation, this.desiredRotation, this.speed.Value * Time.deltaTime);
			ownerDefaultTarget.transform.rotation = this.lastRotation;
		}

		// Token: 0x04005133 RID: 20787
		[Tooltip("The GameObject to rotate.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005134 RID: 20788
		[Tooltip("The direction to smoothly rotate towards.")]
		[RequiredField]
		public FsmVector3 targetDirection;

		// Token: 0x04005135 RID: 20789
		[Tooltip("Only rotate if Target Direction Vector length is greater than this threshold.")]
		public FsmFloat minMagnitude;

		// Token: 0x04005136 RID: 20790
		[Tooltip("Keep this vector pointing up as the GameObject rotates.")]
		public FsmVector3 upVector;

		// Token: 0x04005137 RID: 20791
		[RequiredField]
		[Tooltip("Eliminate any tilt up/down as the GameObject rotates.")]
		public FsmBool keepVertical;

		// Token: 0x04005138 RID: 20792
		[Tooltip("How quickly to rotate.")]
		[HasFloatSlider(0.5f, 15f)]
		[RequiredField]
		public FsmFloat speed;

		// Token: 0x04005139 RID: 20793
		[Tooltip("Perform in LateUpdate. This can help eliminate jitters in some situations.")]
		public bool lateUpdate;

		// Token: 0x0400513A RID: 20794
		private GameObject previousGo;

		// Token: 0x0400513B RID: 20795
		private Quaternion lastRotation;

		// Token: 0x0400513C RID: 20796
		private Quaternion desiredRotation;
	}
}
