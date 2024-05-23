using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CE9 RID: 3305
	[Tooltip("Sets the Offset of a named texture in a Game Object's Material. Useful for scrolling texture effects.")]
	[ActionCategory(20)]
	public class SetTextureOffset : ComponentAction<Renderer>
	{
		// Token: 0x0600694F RID: 26959 RVA: 0x001EDB18 File Offset: 0x001EBD18
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.namedTexture = "_MainTex";
			this.offsetX = 0f;
			this.offsetY = 0f;
			this.everyFrame = false;
		}

		// Token: 0x06006950 RID: 26960 RVA: 0x001EDB6F File Offset: 0x001EBD6F
		public override void OnEnter()
		{
			this.DoSetTextureOffset();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006951 RID: 26961 RVA: 0x001EDB88 File Offset: 0x001EBD88
		public override void OnUpdate()
		{
			this.DoSetTextureOffset();
		}

		// Token: 0x06006952 RID: 26962 RVA: 0x001EDB90 File Offset: 0x001EBD90
		private void DoSetTextureOffset()
		{
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
				base.renderer.material.SetTextureOffset(this.namedTexture.Value, new Vector2(this.offsetX.Value, this.offsetY.Value));
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[this.materialIndex.Value].SetTextureOffset(this.namedTexture.Value, new Vector2(this.offsetX.Value, this.offsetY.Value));
				base.renderer.materials = materials;
			}
		}

		// Token: 0x040050FC RID: 20732
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		// Token: 0x040050FD RID: 20733
		public FsmInt materialIndex;

		// Token: 0x040050FE RID: 20734
		[UIHint(13)]
		[RequiredField]
		public FsmString namedTexture;

		// Token: 0x040050FF RID: 20735
		[RequiredField]
		public FsmFloat offsetX;

		// Token: 0x04005100 RID: 20736
		[RequiredField]
		public FsmFloat offsetY;

		// Token: 0x04005101 RID: 20737
		public bool everyFrame;
	}
}
