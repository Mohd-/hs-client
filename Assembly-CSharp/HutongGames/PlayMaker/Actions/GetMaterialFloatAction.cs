using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DB9 RID: 3513
	[ActionCategory("Pegasus")]
	[Tooltip("Gets a named float value from a game object's material.")]
	public class GetMaterialFloatAction : FsmStateAction
	{
		// Token: 0x06006CFE RID: 27902 RVA: 0x00200FF8 File Offset: 0x001FF1F8
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.material = null;
			this.namedFloat = "_Intensity";
			this.floatValue = null;
			this.fail = null;
		}

		// Token: 0x06006CFF RID: 27903 RVA: 0x0020103D File Offset: 0x001FF23D
		public override void OnEnter()
		{
			this.DoGetMaterialfloatValue();
			base.Finish();
		}

		// Token: 0x06006D00 RID: 27904 RVA: 0x0020104C File Offset: 0x001FF24C
		private void DoGetMaterialfloatValue()
		{
			if (this.floatValue.IsNone)
			{
				return;
			}
			string text = this.namedFloat.Value;
			if (text == string.Empty)
			{
				text = "_Intensity";
			}
			if (this.material.Value != null)
			{
				if (!this.material.Value.HasProperty(text))
				{
					base.Fsm.Event(this.fail);
					return;
				}
				this.floatValue.Value = this.material.Value.GetFloat(text);
				return;
			}
			else
			{
				GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
				if (ownerDefaultTarget == null)
				{
					return;
				}
				if (ownerDefaultTarget.GetComponent<Renderer>() == null)
				{
					this.LogError("Missing Renderer!");
					return;
				}
				if (ownerDefaultTarget.GetComponent<Renderer>().material == null)
				{
					this.LogError("Missing Material!");
					return;
				}
				if (this.materialIndex.Value == 0)
				{
					if (!ownerDefaultTarget.GetComponent<Renderer>().material.HasProperty(text))
					{
						base.Fsm.Event(this.fail);
						return;
					}
					this.floatValue.Value = ownerDefaultTarget.GetComponent<Renderer>().material.GetFloat(text);
				}
				else if (ownerDefaultTarget.GetComponent<Renderer>().materials.Length > this.materialIndex.Value)
				{
					Material[] materials = ownerDefaultTarget.GetComponent<Renderer>().materials;
					if (!materials[this.materialIndex.Value].HasProperty(text))
					{
						base.Fsm.Event(this.fail);
						return;
					}
					this.floatValue.Value = materials[this.materialIndex.Value].GetFloat(text);
				}
				return;
			}
		}

		// Token: 0x040055B0 RID: 21936
		[Tooltip("The GameObject that the material is applied to.")]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040055B1 RID: 21937
		[Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
		public FsmInt materialIndex;

		// Token: 0x040055B2 RID: 21938
		[Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
		public FsmMaterial material;

		// Token: 0x040055B3 RID: 21939
		[Tooltip("The named float parameter in the shader.")]
		[UIHint(17)]
		public FsmString namedFloat;

		// Token: 0x040055B4 RID: 21940
		[UIHint(10)]
		[Tooltip("Get the parameter value.")]
		[RequiredField]
		public FsmFloat floatValue;

		// Token: 0x040055B5 RID: 21941
		public FsmEvent fail;
	}
}
