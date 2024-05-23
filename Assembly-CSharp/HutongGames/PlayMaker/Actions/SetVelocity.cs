using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CED RID: 3309
	[ActionCategory(9)]
	[Tooltip("Sets the Velocity of a Game Object. To leave any axis unchanged, set variable to 'None'. NOTE: Game object must have a rigidbody.")]
	public class SetVelocity : ComponentAction<Rigidbody>
	{
		// Token: 0x06006962 RID: 26978 RVA: 0x001EDFC4 File Offset: 0x001EC1C4
		public override void Reset()
		{
			this.gameObject = null;
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
			this.space = 1;
			this.everyFrame = false;
		}

		// Token: 0x06006963 RID: 26979 RVA: 0x001EE029 File Offset: 0x001EC229
		public override void Awake()
		{
			base.Fsm.HandleFixedUpdate = true;
		}

		// Token: 0x06006964 RID: 26980 RVA: 0x001EE037 File Offset: 0x001EC237
		public override void OnEnter()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006965 RID: 26981 RVA: 0x001EE050 File Offset: 0x001EC250
		public override void OnFixedUpdate()
		{
			this.DoSetVelocity();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006966 RID: 26982 RVA: 0x001EE06C File Offset: 0x001EC26C
		private void DoSetVelocity()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			Vector3 vector;
			if (this.vector.IsNone)
			{
				vector = ((this.space != null) ? ownerDefaultTarget.transform.InverseTransformDirection(base.rigidbody.velocity) : base.rigidbody.velocity);
			}
			else
			{
				vector = this.vector.Value;
			}
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
			base.rigidbody.velocity = ((this.space != null) ? ownerDefaultTarget.transform.TransformDirection(vector) : vector);
		}

		// Token: 0x04005111 RID: 20753
		[CheckForComponent(typeof(Rigidbody))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005112 RID: 20754
		[UIHint(10)]
		public FsmVector3 vector;

		// Token: 0x04005113 RID: 20755
		public FsmFloat x;

		// Token: 0x04005114 RID: 20756
		public FsmFloat y;

		// Token: 0x04005115 RID: 20757
		public FsmFloat z;

		// Token: 0x04005116 RID: 20758
		public Space space;

		// Token: 0x04005117 RID: 20759
		public bool everyFrame;
	}
}
