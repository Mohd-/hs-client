using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B72 RID: 2930
	[ActionCategory(32)]
	[Tooltip("Modify various character controller settings.\n'None' leaves the setting unchanged.")]
	public class ControllerSettings : FsmStateAction
	{
		// Token: 0x0600633E RID: 25406 RVA: 0x001DA758 File Offset: 0x001D8958
		public override void Reset()
		{
			this.gameObject = null;
			this.height = new FsmFloat
			{
				UseVariable = true
			};
			this.radius = new FsmFloat
			{
				UseVariable = true
			};
			this.slopeLimit = new FsmFloat
			{
				UseVariable = true
			};
			this.stepOffset = new FsmFloat
			{
				UseVariable = true
			};
			this.center = new FsmVector3
			{
				UseVariable = true
			};
			this.detectCollisions = new FsmBool
			{
				UseVariable = true
			};
			this.everyFrame = false;
		}

		// Token: 0x0600633F RID: 25407 RVA: 0x001DA7EB File Offset: 0x001D89EB
		public override void OnEnter()
		{
			this.DoControllerSettings();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006340 RID: 25408 RVA: 0x001DA804 File Offset: 0x001D8A04
		public override void OnUpdate()
		{
			this.DoControllerSettings();
		}

		// Token: 0x06006341 RID: 25409 RVA: 0x001DA80C File Offset: 0x001D8A0C
		private void DoControllerSettings()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget != this.previousGo)
			{
				this.controller = ownerDefaultTarget.GetComponent<CharacterController>();
				this.previousGo = ownerDefaultTarget;
			}
			if (this.controller != null)
			{
				if (!this.height.IsNone)
				{
					this.controller.height = this.height.Value;
				}
				if (!this.radius.IsNone)
				{
					this.controller.radius = this.radius.Value;
				}
				if (!this.slopeLimit.IsNone)
				{
					this.controller.slopeLimit = this.slopeLimit.Value;
				}
				if (!this.stepOffset.IsNone)
				{
					this.controller.stepOffset = this.stepOffset.Value;
				}
				if (!this.center.IsNone)
				{
					this.controller.center = this.center.Value;
				}
				if (!this.detectCollisions.IsNone)
				{
					this.controller.detectCollisions = this.detectCollisions.Value;
				}
			}
		}

		// Token: 0x04004ABB RID: 19131
		[Tooltip("The GameObject that owns the CharacterController.")]
		[RequiredField]
		[CheckForComponent(typeof(CharacterController))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004ABC RID: 19132
		[Tooltip("The height of the character's capsule.")]
		public FsmFloat height;

		// Token: 0x04004ABD RID: 19133
		[Tooltip("The radius of the character's capsule.")]
		public FsmFloat radius;

		// Token: 0x04004ABE RID: 19134
		[Tooltip("The character controllers slope limit in degrees.")]
		public FsmFloat slopeLimit;

		// Token: 0x04004ABF RID: 19135
		[Tooltip("The character controllers step offset in meters.")]
		public FsmFloat stepOffset;

		// Token: 0x04004AC0 RID: 19136
		[Tooltip("The center of the character's capsule relative to the transform's position")]
		public FsmVector3 center;

		// Token: 0x04004AC1 RID: 19137
		[Tooltip("Should other rigidbodies or character controllers collide with this character controller (By default always enabled).")]
		public FsmBool detectCollisions;

		// Token: 0x04004AC2 RID: 19138
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;

		// Token: 0x04004AC3 RID: 19139
		private GameObject previousGo;

		// Token: 0x04004AC4 RID: 19140
		private CharacterController controller;
	}
}
