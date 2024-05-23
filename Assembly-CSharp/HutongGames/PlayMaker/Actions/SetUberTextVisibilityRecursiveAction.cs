using System;
using System.Collections.Generic;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DDF RID: 3551
	[ActionCategory("Pegasus")]
	[Tooltip("Sets the visibility on UberText objects and its UberText children.")]
	public class SetUberTextVisibilityRecursiveAction : FsmStateAction
	{
		// Token: 0x06006D9D RID: 28061 RVA: 0x002035FC File Offset: 0x002017FC
		public override void Reset()
		{
			this.gameObject = null;
			this.visible = false;
			this.resetOnExit = false;
			this.includeChildren = false;
			this.m_initialVisibility.Clear();
		}

		// Token: 0x06006D9E RID: 28062 RVA: 0x00203638 File Offset: 0x00201838
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
				UberText[] componentsInChildren = ownerDefaultTarget.GetComponentsInChildren<UberText>();
				if (componentsInChildren != null)
				{
					foreach (UberText uberText in componentsInChildren)
					{
						this.m_initialVisibility[uberText] = !uberText.isHidden();
						if (this.visible.Value)
						{
							uberText.Show();
						}
						else
						{
							uberText.Hide();
						}
					}
				}
			}
			else
			{
				UberText component = ownerDefaultTarget.GetComponent<UberText>();
				if (component != null)
				{
					this.m_initialVisibility[component] = !component.isHidden();
					if (this.visible.Value)
					{
						component.Show();
					}
					else
					{
						component.Hide();
					}
				}
			}
			base.Finish();
		}

		// Token: 0x06006D9F RID: 28063 RVA: 0x00203734 File Offset: 0x00201934
		public override void OnExit()
		{
			if (!this.resetOnExit)
			{
				return;
			}
			foreach (KeyValuePair<UberText, bool> keyValuePair in this.m_initialVisibility)
			{
				UberText key = keyValuePair.Key;
				if (!(key == null))
				{
					if (keyValuePair.Value)
					{
						key.Show();
					}
					else
					{
						key.Hide();
					}
				}
			}
		}

		// Token: 0x04005644 RID: 22084
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005645 RID: 22085
		[Tooltip("Should the objects be set to visible or invisible?")]
		public FsmBool visible;

		// Token: 0x04005646 RID: 22086
		[Tooltip("Resets to the initial visibility once\nit leaves the state")]
		public bool resetOnExit;

		// Token: 0x04005647 RID: 22087
		public bool includeChildren;

		// Token: 0x04005648 RID: 22088
		private Map<UberText, bool> m_initialVisibility = new Map<UberText, bool>();
	}
}
