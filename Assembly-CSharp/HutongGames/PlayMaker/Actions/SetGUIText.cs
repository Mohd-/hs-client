using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CBD RID: 3261
	[Tooltip("Sets the Text used by the GUIText Component attached to a Game Object.")]
	[ActionCategory(18)]
	public class SetGUIText : ComponentAction<GUIText>
	{
		// Token: 0x0600688F RID: 26767 RVA: 0x001EBA0B File Offset: 0x001E9C0B
		public override void Reset()
		{
			this.gameObject = null;
			this.text = string.Empty;
		}

		// Token: 0x06006890 RID: 26768 RVA: 0x001EBA24 File Offset: 0x001E9C24
		public override void OnEnter()
		{
			this.DoSetGUIText();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006891 RID: 26769 RVA: 0x001EBA3D File Offset: 0x001E9C3D
		public override void OnUpdate()
		{
			this.DoSetGUIText();
		}

		// Token: 0x06006892 RID: 26770 RVA: 0x001EBA48 File Offset: 0x001E9C48
		private void DoSetGUIText()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (base.UpdateCache(ownerDefaultTarget))
			{
				base.guiText.text = this.text.Value;
			}
		}

		// Token: 0x04005067 RID: 20583
		[RequiredField]
		[CheckForComponent(typeof(GUIText))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005068 RID: 20584
		public FsmString text;

		// Token: 0x04005069 RID: 20585
		public bool everyFrame;
	}
}
