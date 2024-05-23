using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD4 RID: 3284
	[Tooltip("Sets a named color value in a game object's material.")]
	[ActionCategory(20)]
	public class SetMaterialColor : FsmStateAction
	{
		// Token: 0x060068F1 RID: 26865 RVA: 0x001EC588 File Offset: 0x001EA788
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.material = null;
			this.namedColor = "_Color";
			this.color = Color.black;
			this.everyFrame = false;
		}

		// Token: 0x060068F2 RID: 26866 RVA: 0x001EC5D6 File Offset: 0x001EA7D6
		public override void OnEnter()
		{
			this.DoSetMaterialColor();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068F3 RID: 26867 RVA: 0x001EC5EF File Offset: 0x001EA7EF
		public override void OnUpdate()
		{
			this.DoSetMaterialColor();
		}

		// Token: 0x060068F4 RID: 26868 RVA: 0x001EC5F8 File Offset: 0x001EA7F8
		private void DoSetMaterialColor()
		{
			if (this.color.IsNone)
			{
				return;
			}
			string text = this.namedColor.Value;
			if (text == string.Empty)
			{
				text = "_Color";
			}
			if (this.material.Value != null)
			{
				this.material.Value.SetColor(text, this.color.Value);
				return;
			}
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
				ownerDefaultTarget.GetComponent<Renderer>().material.SetColor(text, this.color.Value);
			}
			else if (ownerDefaultTarget.GetComponent<Renderer>().materials.Length > this.materialIndex.Value)
			{
				Material[] materials = ownerDefaultTarget.GetComponent<Renderer>().materials;
				materials[this.materialIndex.Value].SetColor(text, this.color.Value);
				ownerDefaultTarget.GetComponent<Renderer>().materials = materials;
			}
		}

		// Token: 0x040050A2 RID: 20642
		[CheckForComponent(typeof(Renderer))]
		[Tooltip("The GameObject that the material is applied to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040050A3 RID: 20643
		[Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
		public FsmInt materialIndex;

		// Token: 0x040050A4 RID: 20644
		[Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
		public FsmMaterial material;

		// Token: 0x040050A5 RID: 20645
		[Tooltip("A named color parameter in the shader.")]
		[UIHint(13)]
		public FsmString namedColor;

		// Token: 0x040050A6 RID: 20646
		[Tooltip("Set the parameter value.")]
		[RequiredField]
		public FsmColor color;

		// Token: 0x040050A7 RID: 20647
		[Tooltip("Repeat every frame. Useful if the value is animated.")]
		public bool everyFrame;
	}
}
