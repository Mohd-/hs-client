using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C70 RID: 3184
	[ActionCategory("Substance")]
	[Tooltip("Set a named color property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
	public class SetProceduralColor : FsmStateAction
	{
		// Token: 0x06006737 RID: 26423 RVA: 0x001E71FC File Offset: 0x001E53FC
		public override void Reset()
		{
			this.substanceMaterial = null;
			this.colorProperty = string.Empty;
			this.colorValue = Color.white;
			this.everyFrame = false;
		}

		// Token: 0x06006738 RID: 26424 RVA: 0x001E7237 File Offset: 0x001E5437
		public override void OnEnter()
		{
			this.DoSetProceduralFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006739 RID: 26425 RVA: 0x001E7250 File Offset: 0x001E5450
		public override void OnUpdate()
		{
			this.DoSetProceduralFloat();
		}

		// Token: 0x0600673A RID: 26426 RVA: 0x001E7258 File Offset: 0x001E5458
		private void DoSetProceduralFloat()
		{
			ProceduralMaterial proceduralMaterial = this.substanceMaterial.Value as ProceduralMaterial;
			if (proceduralMaterial == null)
			{
				this.LogError("Not a substance material!");
				return;
			}
			proceduralMaterial.SetProceduralColor(this.colorProperty.Value, this.colorValue.Value);
		}

		// Token: 0x04004EFD RID: 20221
		[RequiredField]
		public FsmMaterial substanceMaterial;

		// Token: 0x04004EFE RID: 20222
		[RequiredField]
		public FsmString colorProperty;

		// Token: 0x04004EFF RID: 20223
		[RequiredField]
		public FsmColor colorValue;

		// Token: 0x04004F00 RID: 20224
		[Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
		public bool everyFrame;
	}
}
