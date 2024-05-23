using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B3D RID: 2877
	[Tooltip("Adds a Script to a Game Object. Use this to change the behaviour of objects on the fly. Optionally remove the Script on exiting the state.")]
	[ActionCategory(11)]
	public class AddScript : FsmStateAction
	{
		// Token: 0x0600623C RID: 25148 RVA: 0x001D3590 File Offset: 0x001D1790
		public override void Reset()
		{
			this.gameObject = null;
			this.script = null;
		}

		// Token: 0x0600623D RID: 25149 RVA: 0x001D35A0 File Offset: 0x001D17A0
		public override void OnEnter()
		{
			this.DoAddComponent((this.gameObject.OwnerOption != null) ? this.gameObject.GameObject.Value : base.Owner);
			base.Finish();
		}

		// Token: 0x0600623E RID: 25150 RVA: 0x001D35E4 File Offset: 0x001D17E4
		public override void OnExit()
		{
			if (this.removeOnExit.Value && this.addedComponent != null)
			{
				Object.Destroy(this.addedComponent);
			}
		}

		// Token: 0x0600623F RID: 25151 RVA: 0x001D3620 File Offset: 0x001D1820
		private void DoAddComponent(GameObject go)
		{
			if (go == null)
			{
				return;
			}
			this.addedComponent = go.AddComponent(AddScript.GetType(this.script.Value));
			if (this.addedComponent == null)
			{
				this.LogError("Can't add script: " + this.script.Value);
			}
		}

		// Token: 0x06006240 RID: 25152 RVA: 0x001D3684 File Offset: 0x001D1884
		private static Type GetType(string name)
		{
			Type globalType = ReflectionUtils.GetGlobalType(name);
			if (globalType != null)
			{
				return globalType;
			}
			globalType = ReflectionUtils.GetGlobalType("UnityEngine." + name);
			if (globalType != null)
			{
				return globalType;
			}
			return ReflectionUtils.GetGlobalType("HutongGames.PlayMaker." + name);
		}

		// Token: 0x0400494A RID: 18762
		[RequiredField]
		[Tooltip("The GameObject to add the script to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400494B RID: 18763
		[Tooltip("The Script to add to the GameObject.")]
		[RequiredField]
		[UIHint(11)]
		public FsmString script;

		// Token: 0x0400494C RID: 18764
		[Tooltip("Remove the script from the GameObject when this State is exited.")]
		public FsmBool removeOnExit;

		// Token: 0x0400494D RID: 18765
		private Component addedComponent;
	}
}
