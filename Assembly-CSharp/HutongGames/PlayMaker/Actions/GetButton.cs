using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BEE RID: 3054
	[ActionCategory(6)]
	[Tooltip("Gets the pressed state of the specified Button and stores it in a Bool Variable. See Unity Input Manager docs.")]
	public class GetButton : FsmStateAction
	{
		// Token: 0x06006509 RID: 25865 RVA: 0x001E054D File Offset: 0x001DE74D
		public override void Reset()
		{
			this.buttonName = "Fire1";
			this.storeResult = null;
			this.everyFrame = true;
		}

		// Token: 0x0600650A RID: 25866 RVA: 0x001E056D File Offset: 0x001DE76D
		public override void OnEnter()
		{
			this.DoGetButton();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600650B RID: 25867 RVA: 0x001E0586 File Offset: 0x001DE786
		public override void OnUpdate()
		{
			this.DoGetButton();
		}

		// Token: 0x0600650C RID: 25868 RVA: 0x001E058E File Offset: 0x001DE78E
		private void DoGetButton()
		{
			this.storeResult.Value = Input.GetButton(this.buttonName.Value);
		}

		// Token: 0x04004CA7 RID: 19623
		[Tooltip("The name of the button. Set in the Unity Input Manager.")]
		[RequiredField]
		public FsmString buttonName;

		// Token: 0x04004CA8 RID: 19624
		[Tooltip("Store the result in a bool variable.")]
		[RequiredField]
		[UIHint(10)]
		public FsmBool storeResult;

		// Token: 0x04004CA9 RID: 19625
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
