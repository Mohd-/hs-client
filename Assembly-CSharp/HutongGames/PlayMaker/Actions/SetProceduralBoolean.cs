using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C6F RID: 3183
	[Tooltip("Set a named bool property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
	[ActionCategory("Substance")]
	public class SetProceduralBoolean : FsmStateAction
	{
		// Token: 0x06006732 RID: 26418 RVA: 0x001E7148 File Offset: 0x001E5348
		public override void Reset()
		{
			this.substanceMaterial = null;
			this.boolProperty = string.Empty;
			this.boolValue = false;
			this.everyFrame = false;
		}

		// Token: 0x06006733 RID: 26419 RVA: 0x001E717F File Offset: 0x001E537F
		public override void OnEnter()
		{
			this.DoSetProceduralFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x06006734 RID: 26420 RVA: 0x001E7198 File Offset: 0x001E5398
		public override void OnUpdate()
		{
			this.DoSetProceduralFloat();
		}

		// Token: 0x06006735 RID: 26421 RVA: 0x001E71A0 File Offset: 0x001E53A0
		private void DoSetProceduralFloat()
		{
			ProceduralMaterial proceduralMaterial = this.substanceMaterial.Value as ProceduralMaterial;
			if (proceduralMaterial == null)
			{
				this.LogError("Not a substance material!");
				return;
			}
			proceduralMaterial.SetProceduralBoolean(this.boolProperty.Value, this.boolValue.Value);
		}

		// Token: 0x04004EF9 RID: 20217
		[RequiredField]
		public FsmMaterial substanceMaterial;

		// Token: 0x04004EFA RID: 20218
		[RequiredField]
		public FsmString boolProperty;

		// Token: 0x04004EFB RID: 20219
		[RequiredField]
		public FsmBool boolValue;

		// Token: 0x04004EFC RID: 20220
		[Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
		public bool everyFrame;
	}
}
