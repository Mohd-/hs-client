using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000B38 RID: 2872
	[ActionCategory(4)]
	[Tooltip("Adds a Component to a Game Object. Use this to change the behaviour of objects on the fly. Optionally remove the Component on exiting the state.")]
	public class AddComponent : FsmStateAction
	{
		// Token: 0x0600621B RID: 25115 RVA: 0x001D2F2B File Offset: 0x001D112B
		public override void Reset()
		{
			this.gameObject = null;
			this.component = null;
			this.storeComponent = null;
		}

		// Token: 0x0600621C RID: 25116 RVA: 0x001D2F42 File Offset: 0x001D1142
		public override void OnEnter()
		{
			this.DoAddComponent();
			base.Finish();
		}

		// Token: 0x0600621D RID: 25117 RVA: 0x001D2F50 File Offset: 0x001D1150
		public override void OnExit()
		{
			if (this.removeOnExit.Value && this.addedComponent != null)
			{
				Object.Destroy(this.addedComponent);
			}
		}

		// Token: 0x0600621E RID: 25118 RVA: 0x001D2F8C File Offset: 0x001D118C
		private void DoAddComponent()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			this.addedComponent = ownerDefaultTarget.AddComponent(AddComponent.GetType(this.component.Value));
			this.storeComponent.Value = this.addedComponent;
			if (this.addedComponent == null)
			{
				this.LogError("Can't add component: " + this.component.Value);
			}
		}

		// Token: 0x0600621F RID: 25119 RVA: 0x001D3014 File Offset: 0x001D1214
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

		// Token: 0x0400492F RID: 18735
		[Tooltip("The GameObject to add the Component to.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004930 RID: 18736
		[Title("Component Type")]
		[UIHint(11)]
		[RequiredField]
		[Tooltip("The type of Component to add to the Game Object.")]
		public FsmString component;

		// Token: 0x04004931 RID: 18737
		[UIHint(10)]
		[Tooltip("Store the component in an Object variable. E.g., to use with Set Property.")]
		[ObjectType(typeof(Component))]
		public FsmObject storeComponent;

		// Token: 0x04004932 RID: 18738
		[Tooltip("Remove the Component when this State is exited.")]
		public FsmBool removeOnExit;

		// Token: 0x04004933 RID: 18739
		private Component addedComponent;
	}
}
