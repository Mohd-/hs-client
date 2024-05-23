using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD3 RID: 3283
	[ActionCategory(20)]
	[Tooltip("Sets the material on a game object.")]
	public class SetMaterial : ComponentAction<Renderer>
	{
		// Token: 0x060068ED RID: 26861 RVA: 0x001EC4B1 File Offset: 0x001EA6B1
		public override void Reset()
		{
			this.gameObject = null;
			this.material = null;
			this.materialIndex = 0;
		}

		// Token: 0x060068EE RID: 26862 RVA: 0x001EC4CD File Offset: 0x001EA6CD
		public override void OnEnter()
		{
			this.DoSetMaterial();
			base.Finish();
		}

		// Token: 0x060068EF RID: 26863 RVA: 0x001EC4DC File Offset: 0x001EA6DC
		private void DoSetMaterial()
		{
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (!base.UpdateCache(ownerDefaultTarget))
			{
				return;
			}
			if (this.materialIndex.Value == 0)
			{
				base.renderer.material = this.material.Value;
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[this.materialIndex.Value] = this.material.Value;
				base.renderer.materials = materials;
			}
		}

		// Token: 0x0400509F RID: 20639
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040050A0 RID: 20640
		public FsmInt materialIndex;

		// Token: 0x040050A1 RID: 20641
		[RequiredField]
		public FsmMaterial material;
	}
}
