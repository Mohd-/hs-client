using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD6 RID: 3286
	[ActionCategory(20)]
	[Tooltip("Sets a named texture in a game object's material to a movie texture.")]
	public class SetMaterialMovieTexture : ComponentAction<Renderer>
	{
		// Token: 0x060068FB RID: 26875 RVA: 0x001EC8F3 File Offset: 0x001EAAF3
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.material = null;
			this.namedTexture = "_MainTex";
			this.movieTexture = null;
		}

		// Token: 0x060068FC RID: 26876 RVA: 0x001EC926 File Offset: 0x001EAB26
		public override void OnEnter()
		{
			this.DoSetMaterialTexture();
			base.Finish();
		}

		// Token: 0x060068FD RID: 26877 RVA: 0x001EC934 File Offset: 0x001EAB34
		private void DoSetMaterialTexture()
		{
			MovieTexture movieTexture = this.movieTexture.Value as MovieTexture;
			string text = this.namedTexture.Value;
			if (text == string.Empty)
			{
				text = "_MainTex";
			}
			if (this.material.Value != null)
			{
				this.material.Value.SetTexture(text, movieTexture);
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
				base.renderer.material.SetTexture(text, movieTexture);
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[this.materialIndex.Value].SetTexture(text, movieTexture);
				base.renderer.materials = materials;
			}
		}

		// Token: 0x040050AE RID: 20654
		[CheckForComponent(typeof(Renderer))]
		[Tooltip("The GameObject that the material is applied to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x040050AF RID: 20655
		[Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
		public FsmInt materialIndex;

		// Token: 0x040050B0 RID: 20656
		[Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
		public FsmMaterial material;

		// Token: 0x040050B1 RID: 20657
		[UIHint(14)]
		[Tooltip("A named texture in the shader.")]
		public FsmString namedTexture;

		// Token: 0x040050B2 RID: 20658
		[RequiredField]
		[ObjectType(typeof(MovieTexture))]
		public FsmObject movieTexture;
	}
}
