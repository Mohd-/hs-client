using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BEA RID: 3050
	[ActionCategory(14)]
	[Tooltip("Gets the Angle between a GameObject's forward axis and a Target. The Target can be defined as a GameObject or a world Position. If you specify both, then the Position will be used as a local offset from the Target Object's position.")]
	public class GetAngleToTarget : FsmStateAction
	{
		// Token: 0x060064FD RID: 25853 RVA: 0x001E00D4 File Offset: 0x001DE2D4
		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.targetPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.ignoreHeight = true;
			this.storeAngle = null;
			this.everyFrame = false;
		}

		// Token: 0x060064FE RID: 25854 RVA: 0x001E011D File Offset: 0x001DE31D
		public override void OnLateUpdate()
		{
			this.DoGetAngleToTarget();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060064FF RID: 25855 RVA: 0x001E0138 File Offset: 0x001DE338
		private void DoGetAngleToTarget()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			GameObject value = this.targetObject.Value;
			if (value == null && this.targetPosition.IsNone)
			{
				return;
			}
			Vector3 vector;
			if (value != null)
			{
				vector = (this.targetPosition.IsNone ? value.transform.position : value.transform.TransformPoint(this.targetPosition.Value));
			}
			else
			{
				vector = this.targetPosition.Value;
			}
			if (this.ignoreHeight.Value)
			{
				vector.y = ownerDefaultTarget.transform.position.y;
			}
			Vector3 vector2 = vector - ownerDefaultTarget.transform.position;
			this.storeAngle.Value = Vector3.Angle(vector2, ownerDefaultTarget.transform.forward);
		}

		// Token: 0x04004C92 RID: 19602
		[RequiredField]
		[Tooltip("The game object whose forward axis we measure from. If the target is dead ahead the angle will be 0.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004C93 RID: 19603
		[Tooltip("The target object to measure the angle to. Or use target position.")]
		public FsmGameObject targetObject;

		// Token: 0x04004C94 RID: 19604
		[Tooltip("The world position to measure an angle to. If Target Object is also specified, this vector is used as an offset from that object's position.")]
		public FsmVector3 targetPosition;

		// Token: 0x04004C95 RID: 19605
		[Tooltip("Ignore height differences when calculating the angle.")]
		public FsmBool ignoreHeight;

		// Token: 0x04004C96 RID: 19606
		[Tooltip("Store the angle in a float variable.")]
		[UIHint(10)]
		[RequiredField]
		public FsmFloat storeAngle;

		// Token: 0x04004C97 RID: 19607
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
