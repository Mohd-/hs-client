using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C53 RID: 3155
	[Tooltip("Rotates a Game Object so its forward vector points at a Target. The Target can be specified as a GameObject or a world Position. If you specify both, then Position specifies a local offset from the target object's Position.")]
	[ActionCategory(14)]
	public class LookAt : FsmStateAction
	{
		// Token: 0x060066B9 RID: 26297 RVA: 0x001E5354 File Offset: 0x001E3554
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
			this.debugLineColor = Color.yellow;
			this.everyFrame = true;
		}

		// Token: 0x060066BA RID: 26298 RVA: 0x001E53C6 File Offset: 0x001E35C6
		public override void OnEnter()
		{
			this.DoLookAt();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060066BB RID: 26299 RVA: 0x001E53DF File Offset: 0x001E35DF
		public override void OnLateUpdate()
		{
			this.DoLookAt();
		}

		// Token: 0x060066BC RID: 26300 RVA: 0x001E53E8 File Offset: 0x001E35E8
		private void DoLookAt()
		{
			if (!this.UpdateLookAtPosition())
			{
				return;
			}
			this.go.transform.LookAt(this.lookAtPos, (!this.upVector.IsNone) ? this.upVector.Value : Vector3.up);
			if (this.debug.Value)
			{
				Debug.DrawLine(this.go.transform.position, this.lookAtPos, this.debugLineColor.Value);
			}
		}

		// Token: 0x060066BD RID: 26301 RVA: 0x001E5474 File Offset: 0x001E3674
		public bool UpdateLookAtPosition()
		{
			this.go = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (this.go == null)
			{
				return false;
			}
			this.goTarget = this.targetObject.Value;
			if (this.goTarget == null && this.targetPosition.IsNone)
			{
				return false;
			}
			if (this.goTarget != null)
			{
				this.lookAtPos = (this.targetPosition.IsNone ? this.goTarget.transform.position : this.goTarget.transform.TransformPoint(this.targetPosition.Value));
			}
			else
			{
				this.lookAtPos = this.targetPosition.Value;
			}
			this.lookAtPosWithVertical = this.lookAtPos;
			if (this.keepVertical.Value)
			{
				this.lookAtPos.y = this.go.transform.position.y;
			}
			return true;
		}

		// Token: 0x060066BE RID: 26302 RVA: 0x001E558C File Offset: 0x001E378C
		public Vector3 GetLookAtPosition()
		{
			return this.lookAtPos;
		}

		// Token: 0x060066BF RID: 26303 RVA: 0x001E5594 File Offset: 0x001E3794
		public Vector3 GetLookAtPositionWithVertical()
		{
			return this.lookAtPosWithVertical;
		}

		// Token: 0x04004E73 RID: 20083
		[RequiredField]
		[Tooltip("The GameObject to rotate.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004E74 RID: 20084
		[Tooltip("The GameObject to Look At.")]
		public FsmGameObject targetObject;

		// Token: 0x04004E75 RID: 20085
		[Tooltip("World position to look at, or local offset from Target Object if specified.")]
		public FsmVector3 targetPosition;

		// Token: 0x04004E76 RID: 20086
		[Tooltip("Rotate the GameObject to point its up direction vector in the direction hinted at by the Up Vector. See Unity Look At docs for more details.")]
		public FsmVector3 upVector;

		// Token: 0x04004E77 RID: 20087
		[Tooltip("Don't rotate vertically.")]
		public FsmBool keepVertical;

		// Token: 0x04004E78 RID: 20088
		[Title("Draw Debug Line")]
		[Tooltip("Draw a debug line from the GameObject to the Target.")]
		public FsmBool debug;

		// Token: 0x04004E79 RID: 20089
		[Tooltip("Color to use for the debug line.")]
		public FsmColor debugLineColor;

		// Token: 0x04004E7A RID: 20090
		[Tooltip("Repeat every frame.")]
		public bool everyFrame = true;

		// Token: 0x04004E7B RID: 20091
		private GameObject go;

		// Token: 0x04004E7C RID: 20092
		private GameObject goTarget;

		// Token: 0x04004E7D RID: 20093
		private Vector3 lookAtPos;

		// Token: 0x04004E7E RID: 20094
		private Vector3 lookAtPosWithVertical;
	}
}
