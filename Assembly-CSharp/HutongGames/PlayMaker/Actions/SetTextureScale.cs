using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000CEA RID: 3306
	[Tooltip("Sets the Scale of a named texture in a Game Object's Material. Useful for special effects.")]
	[ActionCategory(20)]
	public class SetTextureScale : ComponentAction<Renderer>
	{
		// Token: 0x06006954 RID: 26964 RVA: 0x001EDCA0 File Offset: 0x001EBEA0
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.namedTexture = "_MainTex";
			this.scaleX = 1f;
			this.scaleY = 1f;
			this.everyFrame = false;
		}

		// Token: 0x06006955 RID: 26965 RVA: 0x001EDCF7 File Offset: 0x001EBEF7
		public override void OnEnter()
		{
			this.DoSetTextureScale();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006956 RID: 26966 RVA: 0x001EDD10 File Offset: 0x001EBF10
		public override void OnUpdate()
		{
			this.DoSetTextureScale();
		}

		// Token: 0x06006957 RID: 26967 RVA: 0x001EDD18 File Offset: 0x001EBF18
		private void DoSetTextureScale()
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
				base.renderer.material.SetTextureScale(this.namedTexture.Value, new Vector2(this.scaleX.Value, this.scaleY.Value));
			}
			else if (base.renderer.materials.Length > this.materialIndex.Value)
			{
				Material[] materials = base.renderer.materials;
				materials[this.materialIndex.Value].SetTextureScale(this.namedTexture.Value, new Vector2(this.scaleX.Value, this.scaleY.Value));
				base.renderer.materials = materials;
			}
		}

		// Token: 0x04005102 RID: 20738
		[RequiredField]
		[CheckForComponent(typeof(Renderer))]
		public FsmOwnerDefault gameObject;

		// Token: 0x04005103 RID: 20739
		public FsmInt materialIndex;

		// Token: 0x04005104 RID: 20740
		[UIHint(13)]
		public FsmString namedTexture;

		// Token: 0x04005105 RID: 20741
		[RequiredField]
		public FsmFloat scaleX;

		// Token: 0x04005106 RID: 20742
		[RequiredField]
		public FsmFloat scaleY;

		// Token: 0x04005107 RID: 20743
		public bool everyFrame;
	}
}
