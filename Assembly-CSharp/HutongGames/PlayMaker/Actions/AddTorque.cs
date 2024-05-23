using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B3E RID: 2878
	[Tooltip("Adds torque (rotational force) to a Game Object.")]
	[ActionCategory(9)]
	public class AddTorque : ComponentAction<Rigidbody>
	{
		// Token: 0x06006242 RID: 25154 RVA: 0x001D36D4 File Offset: 0x001D18D4
		public override void Reset()
		{
			this.gameObject = null;
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

		// Token: 0x06006243 RID: 25155 RVA: 0x001D3739 File Offset: 0x001D1939
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006244 RID: 25156 RVA: 0x001D3747 File Offset: 0x001D1947
		public override void OnEnter()
		{
			this.DoAddTorque();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006245 RID: 25157 RVA: 0x001D3760 File Offset: 0x001D1960
		public override void OnFixedUpdate()
		{
			this.DoAddTorque();
		}

		// Token: 0x06006246 RID: 25158 RVA: 0x001D3768 File Offset: 0x001D1968
		private void DoAddTorque()
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
				base.rigidbody.AddTorque(vector, this.forceMode);
			}
			else
			{
				base.rigidbody.AddRelativeTorque(vector, this.forceMode);
			}
		}

		// Token: 0x0400494E RID: 18766
		[RequiredField]
		[Tooltip("The GameObject to add torque to.")]
		[CheckForComponent(typeof(Rigidbody))]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400494F RID: 18767
		[UIHint(10)]
		[Tooltip("A Vector3 torque. Optionally override any axis with the X, Y, Z parameters.")]
		public FsmVector3 vector;

		// Token: 0x04004950 RID: 18768
		[Tooltip("Torque around the X axis. To leave unchanged, set to 'None'.")]
		public FsmFloat x;

		// Token: 0x04004951 RID: 18769
		[Tooltip("Torque around the Y axis. To leave unchanged, set to 'None'.")]
		public FsmFloat y;

		// Token: 0x04004952 RID: 18770
		[Tooltip("Torque around the Z axis. To leave unchanged, set to 'None'.")]
		public FsmFloat z;

		// Token: 0x04004953 RID: 18771
		[Tooltip("Apply the force in world or local space.")]
		public Space space;

		// Token: 0x04004954 RID: 18772
		[Tooltip("The type of force to apply. See Unity Physics docs.")]
		public ForceMode forceMode;

		// Token: 0x04004955 RID: 18773
		[Tooltip("Repeat every frame while the state is active.")]
		public bool everyFrame;
	}
}
