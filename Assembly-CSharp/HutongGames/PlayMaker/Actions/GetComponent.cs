using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF6 RID: 3062
	[Tooltip("Gets a Component attached to a GameObject and stores it in an Object variable. NOTE: Set the Object variable's Object Type to get a component of that type. E.g., set Object Type to UnityEngine.AudioListener to get the AudioListener component on the camera.")]
	[ActionCategory(40)]
	public class GetComponent : FsmStateAction
	{
		// Token: 0x0600652A RID: 25898 RVA: 0x001E0BCE File Offset: 0x001DEDCE
		public override void Reset()
		{
			this.gameObject = null;
			this.storeComponent = null;
			this.everyFrame = false;
		}

		// Token: 0x0600652B RID: 25899 RVA: 0x001E0BE5 File Offset: 0x001DEDE5
		public override void OnEnter()
		{
			this.DoGetComponent();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600652C RID: 25900 RVA: 0x001E0BFE File Offset: 0x001DEDFE
		public override void OnUpdate()
		{
			this.DoGetComponent();
		}

		// Token: 0x0600652D RID: 25901 RVA: 0x001E0C08 File Offset: 0x001DEE08
		private void DoGetComponent()
		{
			if (this.storeComponent == null)
			{
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.storeComponent.Value = ownerDefaultTarget.GetComponent(this.storeComponent.ObjectType);
		}

		// Token: 0x04004CC5 RID: 19653
		[Tooltip("The GameObject that owns the component.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004CC6 RID: 19654
		[UIHint(10)]
		[Tooltip("Store the component in an Object variable.\nNOTE: Set theObject variable's Object Type to get a component of that type. E.g., set Object Type to UnityEngine.AudioListener to get the AudioListener component on the camera.")]
		public FsmObject storeComponent;

		// Token: 0x04004CC7 RID: 19655
		[Tooltip("Repeat every frame.")]
		public bool everyFrame;
	}
}
