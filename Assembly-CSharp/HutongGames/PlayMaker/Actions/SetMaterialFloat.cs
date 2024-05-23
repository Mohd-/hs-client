using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CD5 RID: 3285
	[ActionCategory(20)]
	[Tooltip("Sets a named float in a game object's material.")]
	public class SetMaterialFloat : ComponentAction<Renderer>
	{
		// Token: 0x060068F6 RID: 26870 RVA: 0x001EC758 File Offset: 0x001EA958
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.material = null;
			this.namedFloat = string.Empty;
			this.floatValue = 0f;
			this.everyFrame = false;
		}

		// Token: 0x060068F7 RID: 26871 RVA: 0x001EC7A6 File Offset: 0x001EA9A6
		public override void OnEnter()
		{
			this.DoSetMaterialFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x060068F8 RID: 26872 RVA: 0x001EC7BF File Offset: 0x001EA9BF
		public override void OnUpdate()
		{
			this.DoSetMaterialFloat();
		}

		// Token: 0x060068F9 RID: 26873 RVA: 0x001EC7C8 File Offset: 0x001EA9C8
		private void DoSetMaterialFloat()
		{
			if (this.material.Value != null)
			{
				this.material.Value.SetFloat(this.namedFloat.Value, this.floatValue.Value);
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
				base.renderer.material.SetFloat(this.namedFloat.Value, this.floatValue.Value);
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[this.materialIndex.Value].SetFloat(this.namedFloat.Value, this.floatValue.Value);
				base.renderer.materials = materials;
			}
		}

		// Token: 0x040050A8 RID: 20648
		[Tooltip("The GameObject that the material is applied to.")]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040050A9 RID: 20649
		[Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
		public FsmInt materialIndex;

		// Token: 0x040050AA RID: 20650
		[Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
		public FsmMaterial material;

		// Token: 0x040050AB RID: 20651
		[Tooltip("A named float parameter in the shader.")]
		[RequiredField]
		public FsmString namedFloat;

		// Token: 0x040050AC RID: 20652
		[RequiredField]
		[Tooltip("Set the parameter value.")]
		public FsmFloat floatValue;

		// Token: 0x040050AD RID: 20653
		[Tooltip("Repeat every frame. Useful if the value is animated.")]
		public bool everyFrame;
	}
}
