using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD7 RID: 3287
	[ActionCategory(20)]
	[Tooltip("Sets a named texture in a game object's material.")]
	public class SetMaterialTexture : ComponentAction<Renderer>
	{
		// Token: 0x060068FF RID: 26879 RVA: 0x001ECA56 File Offset: 0x001EAC56
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.material = null;
			this.namedTexture = "_MainTex";
			this.texture = null;
		}

		// Token: 0x06006900 RID: 26880 RVA: 0x001ECA89 File Offset: 0x001EAC89
		public override void OnEnter()
		{
			this.DoSetMaterialTexture();
			base.Finish();
		}

		// Token: 0x06006901 RID: 26881 RVA: 0x001ECA98 File Offset: 0x001EAC98
		private void DoSetMaterialTexture()
		{
			string text = this.namedTexture.Value;
			if (text == string.Empty)
			{
				text = "_MainTex";
			}
			if (this.material.Value != null)
			{
				this.material.Value.SetTexture(text, this.texture.Value);
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			if (base.renderer.material == null)
			{
				this.LogError("Missing Material!");
				return;
			}
			if (this.materialIndex.Value == 0)
			{
				base.renderer.material.SetTexture(text, this.texture.Value);
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[this.materialIndex.Value].SetTexture(text, this.texture.Value);
				base.renderer.materials = materials;
			}
		}

		// Token: 0x040050B3 RID: 20659
		[Tooltip("The GameObject that the material is applied to.")]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040050B4 RID: 20660
		[Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
		public FsmInt materialIndex;

		// Token: 0x040050B5 RID: 20661
		[Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
		public FsmMaterial material;

		// Token: 0x040050B6 RID: 20662
		[Tooltip("A named parameter in the shader.")]
		[UIHint(14)]
		public FsmString namedTexture;

		// Token: 0x040050B7 RID: 20663
		public FsmTexture texture;
	}
}
