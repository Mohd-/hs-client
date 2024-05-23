using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000DDD RID: 3549
	[Tooltip("Sets a named float in a game object's material.")]
	[ActionCategory("Pegasus")]
	public class SetMaterialFloatAction : FsmStateAction
	{
		// Token: 0x06006D95 RID: 28053 RVA: 0x00203314 File Offset: 0x00201514
		public override void Reset()
		{
			this.gameObject = null;
			this.materialIndex = 0;
			this.material = null;
			this.namedFloat = string.Empty;
			this.floatValue = 0f;
			this.everyFrame = false;
		}

		// Token: 0x06006D96 RID: 28054 RVA: 0x00203362 File Offset: 0x00201562
		public override void OnEnter()
		{
			this.DoSetMaterialFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006D97 RID: 28055 RVA: 0x0020337B File Offset: 0x0020157B
		public override void OnUpdate()
		{
			this.DoSetMaterialFloat();
		}

		// Token: 0x06006D98 RID: 28056 RVA: 0x00203384 File Offset: 0x00201584
		private void DoSetMaterialFloat()
		{
			if (this.material.Value != null)
			{
				this.material.Value.SetFloat(this.namedFloat.Value, this.floatValue.Value);
				return;
			}
			GameObject ownerDefaultTarget = base.Fsm.GetOwnerDefaultTarget(this.gameObject);
			if (ownerDefaultTarget == null)
			{
				return;
			}
			if (ownerDefaultTarget.GetComponent<Renderer>() == null)
			{
				SkinnedMeshRenderer component = ownerDefaultTarget.GetComponent<SkinnedMeshRenderer>();
				if (component == null)
				{
					this.LogError("Missing Renderer!");
					return;
				}
			}
			if (this.materialIndex.Value == 0)
			{
				if (ownerDefaultTarget.GetComponent<Renderer>() == null)
				{
					SkinnedMeshRenderer component2 = ownerDefaultTarget.GetComponent<SkinnedMeshRenderer>();
					if (component2 == null)
					{
						this.LogError("Missing Renderer!");
						return;
					}
					component2.material.SetFloat(this.namedFloat.Value, this.floatValue.Value);
				}
				else
				{
					ownerDefaultTarget.GetComponent<Renderer>().material.SetFloat(this.namedFloat.Value, this.floatValue.Value);
				}
			}
			else if (ownerDefaultTarget.GetComponent<Renderer>().materials.Length > this.materialIndex.Value)
			{
				if (ownerDefaultTarget.GetComponent<Renderer>() == null)
				{
					SkinnedMeshRenderer component3 = ownerDefaultTarget.GetComponent<SkinnedMeshRenderer>();
					if (component3 == null)
					{
						this.LogError("Missing Renderer!");
						return;
					}
					Material[] materials = component3.materials;
					materials[this.materialIndex.Value].SetFloat(this.namedFloat.Value, this.floatValue.Value);
					component3.materials = materials;
				}
				else
				{
					Material[] materials2 = ownerDefaultTarget.GetComponent<Renderer>().materials;
					materials2[this.materialIndex.Value].SetFloat(this.namedFloat.Value, this.floatValue.Value);
					ownerDefaultTarget.GetComponent<Renderer>().materials = materials2;
				}
			}
		}

		// Token: 0x0400563C RID: 22076
		[Tooltip("The GameObject that the material is applied to.")]
		public FsmOwnerDefault gameObject;

		// Token: 0x0400563D RID: 22077
		[Tooltip("GameObjects can have multiple materials. Specify an index to target a specific material.")]
		public FsmInt materialIndex;

		// Token: 0x0400563E RID: 22078
		[Tooltip("Alternatively specify a Material instead of a GameObject and Index.")]
		public FsmMaterial material;

		// Token: 0x0400563F RID: 22079
		[Tooltip("A named float parameter in the shader.")]
		[RequiredField]
		public FsmString namedFloat;

		// Token: 0x04005640 RID: 22080
		[Tooltip("Set the parameter value.")]
		[RequiredField]
		public FsmFloat floatValue;

		// Token: 0x04005641 RID: 22081
		[Tooltip("Repeat every frame. Useful if the value is animated.")]
		public bool everyFrame;
	}
}
