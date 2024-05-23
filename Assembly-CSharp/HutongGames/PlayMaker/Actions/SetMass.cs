using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD2 RID: 3282
	[Tooltip("Sets the Mass of a Game Object's Rigid Body.")]
	[ActionCategory(9)]
	public class SetMass : ComponentAction<Rigidbody>
	{
		// Token: 0x060068E9 RID: 26857 RVA: 0x001EC43E File Offset: 0x001EA63E
		public override void Reset()
		{
			this.gameObject = null;
			this.mass = 1f;
		}

		// Token: 0x060068EA RID: 26858 RVA: 0x001EC457 File Offset: 0x001EA657
		public override void OnEnter()
		{
			this.DoSetMass();
			base.Finish();
		}

		// Token: 0x060068EB RID: 26859 RVA: 0x001EC468 File Offset: 0x001EA668
		private void DoSetMass()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.rigidbody.mass = this.mass.Value;
			}
		}

		// Token: 0x0400509D RID: 20637
		[CheckForComponent(typeof(Rigidbody))]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400509E RID: 20638
		[RequiredField]
		[HasFloatSlider(0.1f, 10f)]
		public FsmFloat mass;
	}
}
