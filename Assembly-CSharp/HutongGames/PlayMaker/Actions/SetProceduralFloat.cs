using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C71 RID: 3185
	[Tooltip("Set a named float property in a Substance material. NOTE: Use Rebuild Textures after setting Substance properties.")]
	[ActionCategory("Substance")]
	public class SetProceduralFloat : FsmStateAction
	{
		// Token: 0x0600673C RID: 26428 RVA: 0x001E72B4 File Offset: 0x001E54B4
		public override void Reset()
		{
			this.substanceMaterial = null;
			this.floatProperty = string.Empty;
			this.floatValue = 0f;
			this.everyFrame = false;
		}

		// Token: 0x0600673D RID: 26429 RVA: 0x001E72EF File Offset: 0x001E54EF
		public override void OnEnter()
		{
			this.DoSetProceduralFloat();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600673E RID: 26430 RVA: 0x001E7308 File Offset: 0x001E5508
		public override void OnUpdate()
		{
			this.DoSetProceduralFloat();
		}

		// Token: 0x0600673F RID: 26431 RVA: 0x001E7310 File Offset: 0x001E5510
		private void DoSetProceduralFloat()
		{
			ProceduralMaterial proceduralMaterial = this.substanceMaterial.Value as ProceduralMaterial;
			if (proceduralMaterial == null)
			{
				this.LogError("Not a substance material!");
				return;
			}
			proceduralMaterial.SetProceduralFloat(this.floatProperty.Value, this.floatValue.Value);
		}

		// Token: 0x04004F01 RID: 20225
		[RequiredField]
		public FsmMaterial substanceMaterial;

		// Token: 0x04004F02 RID: 20226
		[RequiredField]
		public FsmString floatProperty;

		// Token: 0x04004F03 RID: 20227
		[RequiredField]
		public FsmFloat floatValue;

		// Token: 0x04004F04 RID: 20228
		[Tooltip("NOTE: Updating procedural materials every frame can be very slow!")]
		public bool everyFrame;
	}
}
