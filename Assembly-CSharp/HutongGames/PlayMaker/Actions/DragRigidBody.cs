using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB3 RID: 3507
	[Tooltip("Drag a Rigid body with the mouse. If draggingPlaneTransform is defined, it will use the UP axis of this gameObject as the dragging plane normal \nThat is select the ground Plane, if you want to drag object on the ground instead of from the camera point of view.")]
	[ActionCategory("Pegasus")]
	public class DragRigidBody : FsmStateAction
	{
		// Token: 0x06006CE3 RID: 27875 RVA: 0x002007F8 File Offset: 0x001FE9F8
		public override void Reset()
		{
			this.spring = 50f;
			this.damper = 5f;
			this.drag = 10f;
			this.angularDrag = 5f;
			this.distance = 0.2f;
			this.attachToCenterOfMass = false;
			this.draggingPlaneTransform = null;
			this.moveUp = true;
		}

		// Token: 0x06006CE4 RID: 27876 RVA: 0x00200874 File Offset: 0x001FEA74
		public override void OnEnter()
		{
			this._cam = Camera.main;
			this._goPlane = base.Fsm.GetOwnerDefaultTarget(this.draggingPlaneTransform);
		}

		// Token: 0x06006CE5 RID: 27877 RVA: 0x002008A4 File Offset: 0x001FEAA4
		public override void OnUpdate()
		{
			if (!this.isDragging && UniversalInputManager.Get().GetMouseButtonDown(0))
			{
				RaycastHit hit;
				if (!Physics.Raycast(this._cam.ScreenPointToRay(UniversalInputManager.Get().GetMousePosition()), ref hit, 100f))
				{
					return;
				}
				if (!hit.rigidbody || hit.rigidbody.isKinematic)
				{
					return;
				}
				this.StartDragging(hit);
			}
			if (this.isDragging)
			{
				this.Drag();
			}
		}

		// Token: 0x06006CE6 RID: 27878 RVA: 0x00200930 File Offset: 0x001FEB30
		private void StartDragging(RaycastHit hit)
		{
			this.isDragging = true;
			if (!this.springJoint)
			{
				GameObject gameObject = new GameObject("__Rigidbody dragger__");
				Rigidbody rigidbody = gameObject.AddComponent<Rigidbody>();
				this.springJoint = gameObject.AddComponent<SpringJoint>();
				rigidbody.isKinematic = true;
			}
			this.springJoint.transform.position = hit.point;
			if (this.attachToCenterOfMass.Value)
			{
				Vector3 vector = this._cam.transform.TransformDirection(hit.rigidbody.centerOfMass) + hit.rigidbody.transform.position;
				vector = this.springJoint.transform.InverseTransformPoint(vector);
				this.springJoint.anchor = vector;
			}
			else
			{
				this.springJoint.anchor = Vector3.zero;
			}
			this._dragStartPos = hit.point;
			this.springJoint.spring = this.spring.Value;
			this.springJoint.damper = this.damper.Value;
			this.springJoint.maxDistance = this.distance.Value;
			this.springJoint.connectedBody = hit.rigidbody;
			this.oldDrag = this.springJoint.connectedBody.drag;
			this.oldAngularDrag = this.springJoint.connectedBody.angularDrag;
			this.springJoint.connectedBody.drag = this.drag.Value;
			this.springJoint.connectedBody.angularDrag = this.angularDrag.Value;
			this.dragDistance = hit.distance;
		}

		// Token: 0x06006CE7 RID: 27879 RVA: 0x00200AD8 File Offset: 0x001FECD8
		private void Drag()
		{
			if (!UniversalInputManager.Get().GetMouseButton(0))
			{
				this.StopDragging();
				return;
			}
			Ray ray = this._cam.ScreenPointToRay(UniversalInputManager.Get().GetMousePosition());
			if (this._goPlane != null)
			{
				Plane plane = default(Plane);
				if (this.moveUp.Value)
				{
					plane..ctor(this._goPlane.transform.forward, this._dragStartPos);
				}
				else
				{
					plane..ctor(this._goPlane.transform.up, this._dragStartPos);
				}
				float num;
				if (plane.Raycast(ray, ref num))
				{
					this.springJoint.transform.position = ray.GetPoint(num);
				}
			}
			else
			{
				this.springJoint.transform.position = ray.GetPoint(this.dragDistance);
			}
		}

		// Token: 0x06006CE8 RID: 27880 RVA: 0x00200BC4 File Offset: 0x001FEDC4
		private void StopDragging()
		{
			this.isDragging = false;
			if (this.springJoint == null)
			{
				return;
			}
			if (this.springJoint.connectedBody)
			{
				this.springJoint.connectedBody.drag = this.oldDrag;
				this.springJoint.connectedBody.angularDrag = this.oldAngularDrag;
				this.springJoint.connectedBody = null;
			}
		}

		// Token: 0x06006CE9 RID: 27881 RVA: 0x00200C37 File Offset: 0x001FEE37
		public override void OnExit()
		{
			this.StopDragging();
		}

		// Token: 0x04005592 RID: 21906
		[Tooltip("the springness of the drag")]
		public FsmFloat spring;

		// Token: 0x04005593 RID: 21907
		[Tooltip("the damping of the drag")]
		public FsmFloat damper;

		// Token: 0x04005594 RID: 21908
		[Tooltip("the drag during dragging")]
		public FsmFloat drag;

		// Token: 0x04005595 RID: 21909
		[Tooltip("the angular drag during dragging")]
		public FsmFloat angularDrag;

		// Token: 0x04005596 RID: 21910
		[Tooltip("The Max Distance between the dragging target and the RigidBody being dragged")]
		public FsmFloat distance;

		// Token: 0x04005597 RID: 21911
		[Tooltip("If TRUE, dragging will have close to no effect on the Rigidbody rotation ( except if it hits other bodies as you drag it)")]
		public FsmBool attachToCenterOfMass;

		// Token: 0x04005598 RID: 21912
		[Tooltip("Move th object forward and back or up and down")]
		public FsmBool moveUp;

		// Token: 0x04005599 RID: 21913
		[Tooltip("If Defined. Use this transform Up axis as the dragging plane normal. Typically, set it to the ground plane if you want to drag objects around on the floor..")]
		public FsmOwnerDefault draggingPlaneTransform;

		// Token: 0x0400559A RID: 21914
		private SpringJoint springJoint;

		// Token: 0x0400559B RID: 21915
		private bool isDragging;

		// Token: 0x0400559C RID: 21916
		private float oldDrag;

		// Token: 0x0400559D RID: 21917
		private float oldAngularDrag;

		// Token: 0x0400559E RID: 21918
		private Camera _cam;

		// Token: 0x0400559F RID: 21919
		private GameObject _goPlane;

		// Token: 0x040055A0 RID: 21920
		private Vector3 _dragStartPos;

		// Token: 0x040055A1 RID: 21921
		private float dragDistance;
	}
}
