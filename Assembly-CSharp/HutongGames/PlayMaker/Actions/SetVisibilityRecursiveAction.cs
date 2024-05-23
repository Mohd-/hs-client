using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DE0 RID: 3552
	[ActionCategory("Pegasus")]
	[Tooltip("Sets the visibility on a game object and its children.")]
	public class SetVisibilityRecursiveAction : FsmStateAction
	{
		// Token: 0x06006DA1 RID: 28065 RVA: 0x002037E0 File Offset: 0x002019E0
		public override void Reset()
		{
			this.gameObject = null;
			this.visible = false;
			this.resetOnExit = true;
			this.includeChildren = false;
			this.m_initialVisibility.Clear();
		}

		// Token: 0x06006DA2 RID: 28066 RVA: 0x0020381C File Offset: 0x00201A1C
		public override void OnEnter()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				base.Finish();
				return;
			}
			if (this.includeChildren)
			{
				Renderer[] componentsInChildren = ownerDefaultTarget.GetComponentsInChildren<Renderer>();
				if (componentsInChildren != null)
				{
					foreach (Renderer renderer in componentsInChildren)
					{
						this.m_initialVisibility[renderer] = renderer.enabled;
						renderer.enabled = this.visible.Value;
					}
				}
			}
			else
			{
				Renderer component = ownerDefaultTarget.GetComponent<Renderer>();
				if (component != null)
				{
					this.m_initialVisibility[component] = component.enabled;
					component.enabled = this.visible.Value;
				}
			}
			base.Finish();
		}

		// Token: 0x06006DA3 RID: 28067 RVA: 0x002038F0 File Offset: 0x00201AF0
		public override void OnExit()
		{
			if (!this.resetOnExit)
			{
				return;
			}
			foreach (KeyValuePair<Renderer, bool> keyValuePair in this.m_initialVisibility)
			{
				Renderer key = keyValuePair.Key;
				if (!(key == null))
				{
					key.enabled = keyValuePair.Value;
				}
			}
		}

		// Token: 0x04005649 RID: 22089
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400564A RID: 22090
		[Tooltip("Should the objects be set to visible or invisible?")]
		public FsmBool visible;

		// Token: 0x0400564B RID: 22091
		[Tooltip("Resets to the initial visibility once\nit leaves the state")]
		public bool resetOnExit;

		// Token: 0x0400564C RID: 22092
		public bool includeChildren;

		// Token: 0x0400564D RID: 22093
		private Dictionary<Renderer, bool> m_initialVisibility = new Dictionary<Renderer, bool>();
	}
}
