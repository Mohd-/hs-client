using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF0 RID: 3312
	[Tooltip("Action version of Unity's Smooth Follow script.")]
	[ActionCategory(14)]
	public class SmoothFollowAction : FsmStateAction
	{
		// Token: 0x06006972 RID: 26994 RVA: 0x001EE2D4 File Offset: 0x001EC4D4
		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.distance = 10f;
			this.height = 5f;
			this.heightDamping = 2f;
			this.rotationDamping = 3f;
		}

		// Token: 0x06006973 RID: 26995 RVA: 0x001EE330 File Offset: 0x001EC530
		public override void OnLateUpdate()
		{
			if (this.targetObject.Value == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (this.cachedObect != ownerDefaultTarget)
			{
				this.cachedObect = ownerDefaultTarget;
				this.myTransform = ownerDefaultTarget.transform;
				this.targetTransform = this.targetObject.Value.transform;
			}
			float y = this.targetTransform.eulerAngles.y;
			float num = this.targetTransform.position.y + this.height.Value;
			float num2 = this.myTransform.eulerAngles.y;
			float num3 = this.myTransform.position.y;
			num2 = Mathf.LerpAngle(num2, y, this.rotationDamping.Value * Time.deltaTime);
			num3 = Mathf.Lerp(num3, num, this.heightDamping.Value * Time.deltaTime);
			Quaternion quaternion = Quaternion.Euler(0f, num2, 0f);
			this.myTransform.position = this.targetTransform.position;
			this.myTransform.position -= quaternion * Vector3.forward * this.distance.Value;
			this.myTransform.position = new Vector3(this.myTransform.position.x, num3, this.myTransform.position.z);
			this.myTransform.LookAt(this.targetTransform);
		}

		// Token: 0x0400511E RID: 20766
		[Tooltip("The game object to control. E.g. The camera.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400511F RID: 20767
		[Tooltip("The GameObject to follow.")]
		public FsmGameObject targetObject;

		// Token: 0x04005120 RID: 20768
		[RequiredField]
		[Tooltip("The distance in the x-z plane to the target.")]
		public FsmFloat distance;

		// Token: 0x04005121 RID: 20769
		[RequiredField]
		[Tooltip("The height we want the camera to be above the target")]
		public FsmFloat height;

		// Token: 0x04005122 RID: 20770
		[RequiredField]
		[Tooltip("How much to dampen height movement.")]
		public FsmFloat heightDamping;

		// Token: 0x04005123 RID: 20771
		[Tooltip("How much to dampen rotation changes.")]
		[RequiredField]
		public FsmFloat rotationDamping;

		// Token: 0x04005124 RID: 20772
		private GameObject cachedObect;

		// Token: 0x04005125 RID: 20773
		private Transform myTransform;

		// Token: 0x04005126 RID: 20774
		private Transform targetTransform;
	}
}
