using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C17 RID: 3095
	[ActionCategory(20)]
	[Tooltip("Get a material at index on a gameObject and store it in a variable")]
	public class GetMaterial : ComponentAction<Renderer>
	{
		// Token: 0x060065BC RID: 26044 RVA: 0x001E28AD File Offset: 0x001E0AAD
		public override void Reset()
		{
			this.gameObject = null;
			this.material = null;
			this.materialIndex = 0;
			this.getSharedMaterial = false;
		}

		// Token: 0x060065BD RID: 26045 RVA: 0x001E28D0 File Offset: 0x001E0AD0
		public override void OnEnter()
		{
			this.DoGetMaterial();
			base.Finish();
		}

		// Token: 0x060065BE RID: 26046 RVA: 0x001E28E0 File Offset: 0x001E0AE0
		private void DoGetMaterial()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			if (this.materialIndex.Value == 0 && !this.getSharedMaterial)
			{
				this.material.Value = base.renderer.material;
			}
			else if (this.materialIndex.Value == 0 && this.getSharedMaterial)
			{
				this.material.Value = base.renderer.sharedMaterial;
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value && !this.getSharedMaterial)
			{
				Material[] materials = base.renderer.materials;
				this.material.Value = materials[this.materialIndex.Value];
				base.renderer.materials = materials;
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value && this.getSharedMaterial)
			{
				Material[] sharedMaterials = base.renderer.sharedMaterials;
				this.material.Value = sharedMaterials[this.materialIndex.Value];
				base.renderer.sharedMaterials = sharedMaterials;
			}
		}

		// Token: 0x04004D87 RID: 19847
		[RequiredField]
		[Tooltip("The GameObject the Material is applied to.")]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04004D88 RID: 19848
		[Tooltip("The index of the Material in the Materials array.")]
		public FsmInt materialIndex;

		// Token: 0x04004D89 RID: 19849
		[Tooltip("Store the material in a variable.")]
		[RequiredField]
		[UIHint(10)]
		public FsmMaterial material;

		// Token: 0x04004D8A RID: 19850
		[Tooltip("Get the shared material of this object. NOTE: Modifying the shared material will change the appearance of all objects using this material, and change material settings that are stored in the project too.")]
		public bool getSharedMaterial;
	}
}
