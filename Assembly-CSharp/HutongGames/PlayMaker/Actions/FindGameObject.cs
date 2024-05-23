using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BA3 RID: 2979
	[Tooltip("Finds a Game Object by Name and/or Tag.")]
	[ActionCategory(4)]
	public class FindGameObject : FsmStateAction
	{
		// Token: 0x060063FB RID: 25595 RVA: 0x001DC93C File Offset: 0x001DAB3C
		public override void Reset()
		{
			this.objectName = string.Empty;
			this.withTag = "Untagged";
			this.store = null;
		}

		// Token: 0x060063FC RID: 25596 RVA: 0x001DC968 File Offset: 0x001DAB68
		public override void OnEnter()
		{
			base.Finish();
			if (!(this.withTag.Value != "Untagged"))
			{
				this.store.Value = GameObject.Find(this.objectName.Value);
				return;
			}
			if (!string.IsNullOrEmpty(this.objectName.Value))
			{
				GameObject[] array = GameObject.FindGameObjectsWithTag(this.withTag.Value);
				foreach (GameObject gameObject in array)
				{
					if (gameObject.name == this.objectName.Value)
					{
						this.store.Value = gameObject;
						return;
					}
				}
				this.store.Value = null;
				return;
			}
			this.store.Value = GameObject.FindGameObjectWithTag(this.withTag.Value);
		}

		// Token: 0x060063FD RID: 25597 RVA: 0x001DCA41 File Offset: 0x001DAC41
		public override string ErrorCheck()
		{
			if (string.IsNullOrEmpty(this.objectName.Value) && string.IsNullOrEmpty(this.withTag.Value))
			{
				return "Specify Name, Tag, or both.";
			}
			return null;
		}

		// Token: 0x04004B6F RID: 19311
		[Tooltip("The name of the GameObject to find. You can leave this empty if you specify a Tag.")]
		public FsmString objectName;

		// Token: 0x04004B70 RID: 19312
		[UIHint(7)]
		[Tooltip("Find a GameObject with this tag. If Object Name is specified then both name and Tag must match.")]
		public FsmString withTag;

		// Token: 0x04004B71 RID: 19313
		[Tooltip("Store the result in a GameObject variable.")]
		[RequiredField]
		[UIHint(10)]
		public FsmGameObject store;
	}
}
