using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C16 RID: 3094
	[Tooltip("Gets the Mass of a Game Object's Rigid Body.")]
	[ActionCategory(9)]
	public class GetMass : ComponentAction<Rigidbody>
	{
		// Token: 0x060065B8 RID: 26040 RVA: 0x001E2846 File Offset: 0x001E0A46
		public override void Reset()
		{
			this.gameObject = null;
			this.storeResult = null;
		}

		// Token: 0x060065B9 RID: 26041 RVA: 0x001E2856 File Offset: 0x001E0A56
		public override void OnEnter()
		{
			this.DoGetMass();
			base.Finish();
		}

		// Token: 0x060065BA RID: 26042 RVA: 0x001E2864 File Offset: 0x001E0A64
		private void DoGetMass()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				this.storeResult.Value = base.rigidbody.mass;
			}
		}

		// Token: 0x04004D85 RID: 19845
		[Tooltip("The GameObject that owns the Rigidbody")]
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D86 RID: 19846
		[UIHint(10)]
		[Tooltip("Store the mass in a float variable.")]
		[RequiredField]
		public FsmFloat storeResult;
	}
}
