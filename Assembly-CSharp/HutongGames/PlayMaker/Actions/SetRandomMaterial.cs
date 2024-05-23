using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CDE RID: 3294
	[ActionCategory(20)]
	[Tooltip("Sets a Game Object's material randomly from an array of Materials.")]
	public class SetRandomMaterial : ComponentAction<Renderer>
	{
		// Token: 0x0600691C RID: 26908 RVA: 0x001ED019 File Offset: 0x001EB219
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.materials = new FsmMaterial[3];
		}

		// Token: 0x0600691D RID: 26909 RVA: 0x001ED03A File Offset: 0x001EB23A
		public override void OnEnter()
		{
			this.DoSetRandomMaterial();
			base.Finish();
		}

		// Token: 0x0600691E RID: 26910 RVA: 0x001ED048 File Offset: 0x001EB248
		private void DoSetRandomMaterial()
		{
			if (this.materials == null)
			{
				return;
			}
			if (this.materials.Length == 0)
			{
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
				base.renderer.material = this.materials[Random.Range(0, this.materials.Length)].Value;
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] array = base.renderer.materials;
				array[this.materialIndex.Value] = this.materials[Random.Range(0, this.materials.Length)].Value;
				base.renderer.materials = array;
			}
		}

		// Token: 0x040050CE RID: 20686
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040050CF RID: 20687
		public FsmInt materialIndex;

		// Token: 0x040050D0 RID: 20688
		public FsmMaterial[] materials;
	}
}
