using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C18 RID: 3096
	[ActionCategory(20)]
	[Tooltip("Get a texture from a material on a GameObject")]
	public class GetMaterialTexture : ComponentAction<Renderer>
	{
		// Token: 0x060065C0 RID: 26048 RVA: 0x001E2A35 File Offset: 0x001E0C35
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.namedTexture = "_MainTex";
			this.storedTexture = null;
			this.getFromSharedMaterial = false;
		}

		// Token: 0x060065C1 RID: 26049 RVA: 0x001E2A68 File Offset: 0x001E0C68
		public override void OnEnter()
		{
			this.DoGetMaterialTexture();
			base.Finish();
		}

		// Token: 0x060065C2 RID: 26050 RVA: 0x001E2A78 File Offset: 0x001E0C78
		private void DoGetMaterialTexture()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			string text = this.namedTexture.Value;
			if (text == string.Empty)
			{
				text = "_MainTex";
			}
			if (this.materialIndex.Value == 0 && !this.getFromSharedMaterial)
			{
				this.storedTexture.Value = base.renderer.material.GetTexture(text);
			}
			else if (this.materialIndex.Value == 0 && this.getFromSharedMaterial)
			{
				this.storedTexture.Value = base.renderer.sharedMaterial.GetTexture(text);
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value && !this.getFromSharedMaterial)
			{
				Material[] materials = base.renderer.materials;
				this.storedTexture.Value = base.renderer.materials[this.materialIndex.Value].GetTexture(text);
				base.renderer.materials = materials;
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value && this.getFromSharedMaterial)
			{
				Material[] sharedMaterials = base.renderer.sharedMaterials;
				this.storedTexture.Value = base.renderer.sharedMaterials[this.materialIndex.Value].GetTexture(text);
				base.renderer.materials = sharedMaterials;
			}
		}

		// Token: 0x04004D8B RID: 19851
		[CheckForComponent(typeof(Renderer))]
		[RequiredField]
		[Tooltip("The GameObject the Material is applied to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D8C RID: 19852
		[Tooltip("The index of the Material in the Materials array.")]
		public FsmInt materialIndex;

		// Token: 0x04004D8D RID: 19853
		[UIHint(14)]
		[Tooltip("The texture to get. See Unity Shader docs for names.")]
		public FsmString namedTexture;

		// Token: 0x04004D8E RID: 19854
		[Title("StoreTexture")]
		[RequiredField]
		[Tooltip("Store the texture in a variable.")]
		[UIHint(10)]
		public FsmTexture storedTexture;

		// Token: 0x04004D8F RID: 19855
		[Tooltip("Get the shared version of the texture.")]
		public bool getFromSharedMaterial;
	}
}
