using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C2E RID: 3118
	[ActionCategory(9)]
	[Tooltip("Gets the Speed of a Game Object and stores it in a Float Variable. NOTE: The Game Object must have a rigid body.")]
	public class GetSpeed : ComponentAction<Rigidbody>
	{
		// Token: 0x06006619 RID: 26137 RVA: 0x001E3865 File Offset: 0x001E1A65
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
			this.everyFrame = false;
		}

		// Token: 0x0600661A RID: 26138 RVA: 0x001E387C File Offset: 0x001E1A7C
		public override void OnEnter()
		{
			this.DoGetSpeed();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600661B RID: 26139 RVA: 0x001E3895 File Offset: 0x001E1A95
		public override void OnUpdate()
		{
			this.DoGetSpeed();
		}

		// Token: 0x0600661C RID: 26140 RVA: 0x001E38A0 File Offset: 0x001E1AA0
		private void DoGetSpeed()
		{
			if (this.storeResult == null)
			{
				return;
			}
			GameObject go = (this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner;
			if (base.UpdateCache(go))
			{
				Vector3 velocity = base.rigidbody.velocity;
				this.storeResult.Value = velocity.magnitude;
			}
		}

		// Token: 0x04004DD5 RID: 19925
		[RequiredField]
		[Tooltip("The GameObject with a Rigidbody.")]
		[CheckForComponent(typeof(Rigidbody))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004DD6 RID: 19926
		[RequiredField]
		[Tooltip("Store the speed in a float variable.")]
		[UIHint(10)]
		public FsmFloat storeResult;

		// Token: 0x04004DD7 RID: 19927
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
