using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE8 RID: 3304
	[ActionCategory(4)]
	[Tooltip("Set the Tag on all children of a GameObject. Optionally filter by component.")]
	public class SetTagsOnChildren : FsmStateAction
	{
		// Token: 0x0600694A RID: 26954 RVA: 0x001ED96E File Offset: 0x001EBB6E
		public override void Reset()
		{
			this.gameObject = null;
			this.tag = null;
			this.filterByComponent = null;
		}

		// Token: 0x0600694B RID: 26955 RVA: 0x001ED985 File Offset: 0x001EBB85
		public override void OnEnter()
		{
			this.SetTag(base.Fsm.GetOwnerDefaultTarget(this.gameObject));
			base.Finish();
		}

		// Token: 0x0600694C RID: 26956 RVA: 0x001ED9A4 File Offset: 0x001EBBA4
		private void SetTag(GameObject parent)
		{
			if (parent == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(this.filterByComponent.Value))
			{
				foreach (object obj in parent.transform)
				{
					Transform transform = (Transform)obj;
					transform.gameObject.tag = this.tag.Value;
				}
			}
			else
			{
				this.UpdateComponentFilter();
				if (this.componentFilter != null)
				{
					Component[] componentsInChildren = parent.GetComponentsInChildren(this.componentFilter);
					foreach (Component component in componentsInChildren)
					{
						component.gameObject.tag = this.tag.Value;
					}
				}
			}
			base.Finish();
		}

		// Token: 0x0600694D RID: 26957 RVA: 0x001EDA9C File Offset: 0x001EBC9C
		private void UpdateComponentFilter()
		{
			this.componentFilter = ReflectionUtils.GetGlobalType(this.filterByComponent.Value);
			if (this.componentFilter == null)
			{
				this.componentFilter = ReflectionUtils.GetGlobalType("UnityEngine." + this.filterByComponent.Value);
			}
			if (this.componentFilter == null)
			{
				Debug.LogWarning("Couldn't get type: " + this.filterByComponent.Value);
			}
		}

		// Token: 0x040050F8 RID: 20728
		[Tooltip("GameObject Parent")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x040050F9 RID: 20729
		[Tooltip("Set Tag To...")]
		[RequiredField]
		[UIHint(7)]
		public FsmString tag;

		// Token: 0x040050FA RID: 20730
		[UIHint(11)]
		[Tooltip("Only set the Tag on children with this component.")]
		public FsmString filterByComponent;

		// Token: 0x040050FB RID: 20731
		private Type componentFilter;
	}
}
