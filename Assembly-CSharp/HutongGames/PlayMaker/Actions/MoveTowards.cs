using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C5A RID: 3162
	[Tooltip("Moves a Game Object towards a Target. Optionally sends an event when successful. The Target can be specified as a Game Object or a world Position. If you specify both, then the Position is used as a local offset from the Object's Position.")]
	[ActionCategory(14)]
	public class MoveTowards : FsmStateAction
	{
		// Token: 0x060066DD RID: 26333 RVA: 0x001E5ED1 File Offset: 0x001E40D1
		public override void Reset()
		{
			this.gameObject = null;
			this.targetObject = null;
			this.maxSpeed = 10f;
			this.finishDistance = 1f;
			this.finishEvent = null;
		}

		// Token: 0x060066DE RID: 26334 RVA: 0x001E5F08 File Offset: 0x001E4108
		public override void OnUpdate()
		{
			this.DoMoveTowards();
		}

		// Token: 0x060066DF RID: 26335 RVA: 0x001E5F10 File Offset: 0x001E4110
		private void DoMoveTowards()
		{
			if (!this.UpdateTargetPos())
			{
				return;
			}
			this.go.transform.position = Vector3.MoveTowards(this.go.transform.position, this.targetPos, this.maxSpeed.Value * Time.deltaTime);
			float magnitude = (this.go.transform.position - this.targetPos).magnitude;
			if (magnitude < this.finishDistance.Value)
			{
				base.Fsm.Event(this.finishEvent);
				base.Finish();
			}
		}

		// Token: 0x060066E0 RID: 26336 RVA: 0x001E5FB4 File Offset: 0x001E41B4
		public bool UpdateTargetPos()
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
				this.targetPos = (this.targetPosition.IsNone ? this.goTarget.transform.position : this.goTarget.transform.TransformPoint(this.targetPosition.Value));
			}
			else
			{
				this.targetPos = this.targetPosition.Value;
			}
			this.targetPosWithVertical = this.targetPos;
			if (this.ignoreVertical.Value)
			{
				this.targetPos.y = this.go.transform.position.y;
			}
			return true;
		}

		// Token: 0x060066E1 RID: 26337 RVA: 0x001E60CC File Offset: 0x001E42CC
		public Vector3 GetTargetPos()
		{
			return this.targetPos;
		}

		// Token: 0x060066E2 RID: 26338 RVA: 0x001E60D4 File Offset: 0x001E42D4
		public Vector3 GetTargetPosWithVertical()
		{
			return this.targetPosWithVertical;
		}

		// Token: 0x04004EAF RID: 20143
		[RequiredField]
		[Tooltip("The GameObject to Move")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004EB0 RID: 20144
		[Tooltip("A target GameObject to move towards. Or use a world Target Position below.")]
		public FsmGameObject targetObject;

		// Token: 0x04004EB1 RID: 20145
		[Tooltip("A world position if no Target Object. Otherwise used as a local offset from the Target Object.")]
		public FsmVector3 targetPosition;

		// Token: 0x04004EB2 RID: 20146
		[Tooltip("Ignore any height difference in the target.")]
		public FsmBool ignoreVertical;

		// Token: 0x04004EB3 RID: 20147
		[HasFloatSlider(0f, 20f)]
		[Tooltip("The maximum movement speed. HINT: You can make this a variable to change it over time.")]
		public FsmFloat maxSpeed;

		// Token: 0x04004EB4 RID: 20148
		[HasFloatSlider(0f, 5f)]
		[Tooltip("Distance at which the move is considered finished, and the Finish Event is sent.")]
		public FsmFloat finishDistance;

		// Token: 0x04004EB5 RID: 20149
		[Tooltip("Event to send when the Finish Distance is reached.")]
		public FsmEvent finishEvent;

		// Token: 0x04004EB6 RID: 20150
		private GameObject go;

		// Token: 0x04004EB7 RID: 20151
		private GameObject goTarget;

		// Token: 0x04004EB8 RID: 20152
		private Vector3 targetPos;

		// Token: 0x04004EB9 RID: 20153
		private Vector3 targetPosWithVertical;
	}
}
