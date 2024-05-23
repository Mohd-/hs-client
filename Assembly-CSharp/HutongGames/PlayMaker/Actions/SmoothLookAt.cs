using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CF1 RID: 3313
	[ActionCategory(14)]
	[Tooltip("Smoothly Rotates a Game Object so its forward vector points at a Target. The target can be defined as a Game Object or a world Position. If you specify both, then the position will be used as a local offset from the object's position.")]
	public class SmoothLookAt : FsmStateAction
	{
		// Token: 0x06006975 RID: 26997 RVA: 0x001EE4F0 File Offset: 0x001EC6F0
		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.targetPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.upVector = new FsmVector3
			{
				UseVariable = true
			};
			this.keepVertical = true;
			this.debug = false;
			this.speed = 5f;
			this.finishTolerance = 1f;
			this.finishEvent = null;
		}

		// Token: 0x06006976 RID: 26998 RVA: 0x001EE572 File Offset: 0x001EC772
		public override void OnEnter()
		{
			this.previousGo = null;
		}

		// Token: 0x06006977 RID: 26999 RVA: 0x001EE57B File Offset: 0x001EC77B
		public override void OnLateUpdate()
		{
			this.DoSmoothLookAt();
		}

		// Token: 0x06006978 RID: 27000 RVA: 0x001EE584 File Offset: 0x001EC784
		private void DoSmoothLookAt()
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
			if (this.previousGo != ownerDefaultTarget)
			{
				this.lastRotation = ownerDefaultTarget.transform.rotation;
				this.desiredRotation = this.lastRotation;
				this.previousGo = ownerDefaultTarget;
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
			if (this.keepVertical.Value)
			{
				vector.y = ownerDefaultTarget.transform.position.y;
			}
			Vector3 vector2 = vector - ownerDefaultTarget.transform.position;
			if (vector2 != Vector3.zero && vector2.sqrMagnitude > 0f)
			{
				this.desiredRotation = Quaternion.LookRotation(vector2, (!this.upVector.IsNone) ? this.upVector.Value : Vector3.up);
			}
			this.lastRotation = Quaternion.Slerp(this.lastRotation, this.desiredRotation, this.speed.Value * Time.deltaTime);
			ownerDefaultTarget.transform.rotation = this.lastRotation;
			if (this.debug.Value)
			{
				Debug.DrawLine(ownerDefaultTarget.transform.position, vector, Color.grey);
			}
			if (this.finishEvent != null)
			{
				Vector3 vector3 = vector - ownerDefaultTarget.transform.position;
				float num = Vector3.Angle(vector3, ownerDefaultTarget.transform.forward);
				if (Mathf.Abs(num) <= this.finishTolerance.Value)
				{
					base.Fsm.Event(this.finishEvent);
				}
			}
		}

		// Token: 0x04005127 RID: 20775
		[RequiredField]
		[Tooltip("The GameObject to rotate to face a target.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005128 RID: 20776
		[Tooltip("A target GameObject.")]
		public FsmGameObject targetObject;

		// Token: 0x04005129 RID: 20777
		[Tooltip("A target position. If a Target Object is defined, this is used as a local offset.")]
		public FsmVector3 targetPosition;

		// Token: 0x0400512A RID: 20778
		[Tooltip("Used to keep the game object generally upright. If left undefined the world y axis is used.")]
		public FsmVector3 upVector;

		// Token: 0x0400512B RID: 20779
		[Tooltip("Force the game object to remain vertical. Useful for characters.")]
		public FsmBool keepVertical;

		// Token: 0x0400512C RID: 20780
		[Tooltip("How fast the look at moves.")]
		[HasFloatSlider(0.5f, 15f)]
		public FsmFloat speed;

		// Token: 0x0400512D RID: 20781
		[Tooltip("Draw a line in the Scene View to the look at position.")]
		public FsmBool debug;

		// Token: 0x0400512E RID: 20782
		[Tooltip("If the angle to the target is less than this, send the Finish Event below. Measured in degrees.")]
		public FsmFloat finishTolerance;

		// Token: 0x0400512F RID: 20783
		[Tooltip("Event to send if the angle to target is less than the Finish Tolerance.")]
		public FsmEvent finishEvent;

		// Token: 0x04005130 RID: 20784
		private GameObject previousGo;

		// Token: 0x04005131 RID: 20785
		private Quaternion lastRotation;

		// Token: 0x04005132 RID: 20786
		private Quaternion desiredRotation;
	}
}
