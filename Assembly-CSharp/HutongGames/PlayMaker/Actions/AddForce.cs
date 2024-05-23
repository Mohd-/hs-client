using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B3B RID: 2875
	[Tooltip("Adds a force to a Game Object. Use Vector3 variable and/or Float variables for each axis.")]
	[ActionCategory(9)]
	public class AddForce : ComponentAction<Rigidbody>
	{
		// Token: 0x06006232 RID: 25138 RVA: 0x001D32C4 File Offset: 0x001D14C4
		public override void Reset()
		{
			this.gameObject = null;
			this.atPosition = new FsmVector3
			{
				UseVariable = true
			};
			this.vector = null;
			this.x = new FsmFloat
			{
				UseVariable = true
			};
			this.y = new FsmFloat
			{
				UseVariable = true
			};
			this.z = new FsmFloat
			{
				UseVariable = true
			};
			this.space = 0;
			this.forceMode = 0;
			this.everyFrame = false;
		}

		// Token: 0x06006233 RID: 25139 RVA: 0x001D3344 File Offset: 0x001D1544
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006234 RID: 25140 RVA: 0x001D3352 File Offset: 0x001D1552
		public override void OnEnter()
		{
			this.DoAddForce();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006235 RID: 25141 RVA: 0x001D336B File Offset: 0x001D156B
		public override void OnFixedUpdate()
		{
			this.DoAddForce();
		}

		// Token: 0x06006236 RID: 25142 RVA: 0x001D3374 File Offset: 0x001D1574
		private void DoAddForce()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector3 vector = (!this.vector.IsNone) ? this.vector.Value : new Vector3(this.x.Value, this.y.Value, this.z.Value);
			if (!this.x.IsNone)
			{
				vector.x = this.x.Value;
			}
			if (!this.y.IsNone)
			{
				vector.y = this.y.Value;
			}
			if (!this.z.IsNone)
			{
				vector.z = this.z.Value;
			}
			if (this.space == null)
			{
				if (!this.atPosition.IsNone)
				{
					base.rigidbody.AddForceAtPosition(vector, this.atPosition.Value, this.forceMode);
				}
				else
				{
					base.rigidbody.AddForce(vector, this.forceMode);
				}
			}
			else
			{
				base.rigidbody.AddRelativeForce(vector, this.forceMode);
			}
		}

		// Token: 0x0400493D RID: 18749
		[Tooltip("The GameObject to apply the force to.")]
		[CheckForComponent(typeof(Rigidbody))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400493E RID: 18750
		[Tooltip("Optionally apply the force at a position on the object. This will also add some torque. The position is often returned from MousePick or GetCollisionInfo actions.")]
		[UIHint(10)]
		public FsmVector3 atPosition;

		// Token: 0x0400493F RID: 18751
		[Tooltip("A Vector3 force to add. Optionally override any axis with the X, Y, Z parameters.")]
		[UIHint(10)]
		public FsmVector3 vector;

		// Token: 0x04004940 RID: 18752
		[Tooltip("Force along the X axis. To leave unchanged, set to 'None'.")]
		public FsmFloat x;

		// Token: 0x04004941 RID: 18753
		[Tooltip("Force along the Y axis. To leave unchanged, set to 'None'.")]
		public FsmFloat y;

		// Token: 0x04004942 RID: 18754
		[Tooltip("Force along the Z axis. To leave unchanged, set to 'None'.")]
		public FsmFloat z;

		// Token: 0x04004943 RID: 18755
		[Tooltip("Apply the force in world or local space.")]
		public Space space;

		// Token: 0x04004944 RID: 18756
		[Tooltip("The type of force to apply. See Unity Physics docs.")]
		public ForceMode forceMode;

		// Token: 0x04004945 RID: 18757
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
