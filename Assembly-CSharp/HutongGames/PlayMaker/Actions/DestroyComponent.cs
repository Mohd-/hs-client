using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B8D RID: 2957
	[Tooltip("Destroys a Component of an Object.")]
	[ActionCategory(4)]
	public class DestroyComponent : FsmStateAction
	{
		// Token: 0x060063A5 RID: 25509 RVA: 0x001DB90D File Offset: 0x001D9B0D
		public override void Reset()
		{
			this.aComponent = null;
			this.gameObject = null;
			this.component = null;
		}

		// Token: 0x060063A6 RID: 25510 RVA: 0x001DB924 File Offset: 0x001D9B24
		public override void OnEnter()
		{
			this.DoDestroyComponent((this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner);
			base.Finish();
		}

		// Token: 0x060063A7 RID: 25511 RVA: 0x001DB968 File Offset: 0x001D9B68
		private void DoDestroyComponent(GameObject go)
		{
			this.aComponent = go.GetComponent(this.component.Value);
			if (this.aComponent == null)
			{
				this.LogError("No such component: " + this.component.Value);
			}
			else
			{
				Object.Destroy(this.aComponent);
			}
		}

		// Token: 0x04004B22 RID: 19234
		[Tooltip("The GameObject that owns the Component.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004B23 RID: 19235
		[UIHint(11)]
		[RequiredField]
		[Tooltip("The name of the Component to destroy.")]
		public FsmString component;

		// Token: 0x04004B24 RID: 19236
		private Component aComponent;
	}
}
