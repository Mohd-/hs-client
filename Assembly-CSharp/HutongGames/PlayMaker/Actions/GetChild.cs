using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000BF1 RID: 3057
	[ActionCategory(4)]
	[Tooltip("Finds the Child of a GameObject by Name and/or Tag. Use this to find attach points etc. NOTE: This action will search recursively through all children and return the first match; To find a specific child use Find Child.")]
	public class GetChild : FsmStateAction
	{
		// Token: 0x06006514 RID: 25876 RVA: 0x001E068C File Offset: 0x001DE88C
		public override void Reset()
		{
			this.gameObject = null;
			this.childName = string.Empty;
			this.withTag = "Untagged";
			this.storeResult = null;
		}

		// Token: 0x06006515 RID: 25877 RVA: 0x001E06C8 File Offset: 0x001DE8C8
		public override void OnEnter()
		{
			this.storeResult.Value = GetChild.DoGetChildByName(base.Fsm.GetOwnerDefaultTarget(this.gameObject), this.childName.Value, this.withTag.Value);
			base.Finish();
		}

		// Token: 0x06006516 RID: 25878 RVA: 0x001E0714 File Offset: 0x001DE914
		private static GameObject DoGetChildByName(GameObject root, string name, string tag)
		{
			if (root == null)
			{
				return null;
			}
			foreach (object obj in root.transform)
			{
				Transform transform = (Transform)obj;
				if (!string.IsNullOrEmpty(name))
				{
					if (transform.name == name)
					{
						if (string.IsNullOrEmpty(tag))
						{
							return transform.gameObject;
						}
						if (transform.tag.Equals(tag))
						{
							return transform.gameObject;
						}
					}
				}
				else if (!string.IsNullOrEmpty(tag) && transform.tag == tag)
				{
					return transform.gameObject;
				}
				GameObject gameObject = GetChild.DoGetChildByName(transform.gameObject, name, tag);
				if (gameObject != null)
				{
					return gameObject;
				}
			}
			return null;
		}

		// Token: 0x06006517 RID: 25879 RVA: 0x001E0828 File Offset: 0x001DEA28
		public override string ErrorCheck()
		{
			if (string.IsNullOrEmpty(this.childName.Value) && string.IsNullOrEmpty(this.withTag.Value))
			{
				return "Specify Child Name, Tag, or both.";
			}
			return null;
		}

		// Token: 0x04004CB0 RID: 19632
		[Tooltip("The GameObject to search.")]
		[RequiredField]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004CB1 RID: 19633
		[Tooltip("The name of the child to search for.")]
		public FsmString childName;

		// Token: 0x04004CB2 RID: 19634
		[UIHint(7)]
		[Tooltip("The Tag to search for. If Child Name is set, both name and Tag need to match.")]
		public FsmString withTag;

		// Token: 0x04004CB3 RID: 19635
		[RequiredField]
		[Tooltip("Store the result in a GameObject variable.")]
		[UIHint(10)]
		public FsmGameObject storeResult;
	}
}
