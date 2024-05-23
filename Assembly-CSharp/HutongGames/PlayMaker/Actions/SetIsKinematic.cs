using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CC7 RID: 3271
	[ActionCategory(9)]
	[Tooltip("Controls whether physics affects the Game Object.")]
	public class SetIsKinematic : ComponentAction<Rigidbody>
	{
		// Token: 0x060068BB RID: 26811 RVA: 0x001EBF0A File Offset: 0x001EA10A
		public override void Reset()
		{
			this.gameObject = null;
			this.isKinematic = false;
		}

		// Token: 0x060068BC RID: 26812 RVA: 0x001EBF1F File Offset: 0x001EA11F
		public override void OnEnter()
		{
			this.DoSetIsKinematic();
			base.Finish();
		}

		// Token: 0x060068BD RID: 26813 RVA: 0x001EBF30 File Offset: 0x001EA130
		private void DoSetIsKinematic()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.rigidbody.isKinematic = this.isKinematic.Value;
			}
		}

		// Token: 0x04005084 RID: 20612
		[RequiredField]
		[CheckForComponent(typeof(Rigidbody))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005085 RID: 20613
		[RequiredField]
		public FsmBool isKinematic;
	}
}
