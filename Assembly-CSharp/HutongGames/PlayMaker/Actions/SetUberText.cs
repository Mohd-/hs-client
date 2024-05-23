using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DDE RID: 3550
	[Tooltip("Sets the text on an UberText object.")]
	[ActionCategory("Pegasus")]
	public class SetUberText : FsmStateAction
	{
		// Token: 0x06006D9A RID: 28058 RVA: 0x00203584 File Offset: 0x00201784
		public override void Reset()
		{
			this.uberTextObject = null;
			this.text = string.Empty;
		}

		// Token: 0x06006D9B RID: 28059 RVA: 0x00203598 File Offset: 0x00201798
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.uberTextObject);
			if (ownerDefaultTarget != null)
			{
				UberText component = ownerDefaultTarget.GetComponent<UberText>();
				if (component != null)
				{
					component.Text = this.text;
				}
			}
			base.Finish();
		}

		// Token: 0x04005642 RID: 22082
		[RequiredField]
		public FsmOwnerDefault uberTextObject;

		// Token: 0x04005643 RID: 22083
		public string text;
	}
}
