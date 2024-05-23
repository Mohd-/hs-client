using System;
using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	// Token: 0x02000C6E RID: 3182
	[ActionCategory("Substance")]
	[Tooltip("Rebuilds all dirty textures. By default the rebuild is spread over multiple frames so it won't halt the game. Check Immediately to rebuild all textures in a single frame.")]
	public class RebuildTextures : FsmStateAction
	{
		// Token: 0x0600672D RID: 26413 RVA: 0x001E70A9 File Offset: 0x001E52A9
		public override void Reset()
		{
			this.substanceMaterial = null;
			this.immediately = false;
			this.everyFrame = false;
		}

		// Token: 0x0600672E RID: 26414 RVA: 0x001E70C5 File Offset: 0x001E52C5
		public override void OnEnter()
		{
			this.DoRebuildTextures();
			if (!this.everyFrame)
			{
				base.Finish();
			}
		}

		// Token: 0x0600672F RID: 26415 RVA: 0x001E70DE File Offset: 0x001E52DE
		public override void OnUpdate()
		{
			this.DoRebuildTextures();
		}

		// Token: 0x06006730 RID: 26416 RVA: 0x001E70E8 File Offset: 0x001E52E8
		private void DoRebuildTextures()
		{
			ProceduralMaterial proceduralMaterial = this.substanceMaterial.Value as ProceduralMaterial;
			if (proceduralMaterial == null)
			{
				this.LogError("Not a substance material!");
				return;
			}
			if (!this.immediately.Value)
			{
				proceduralMaterial.RebuildTextures();
			}
			else
			{
				proceduralMaterial.RebuildTexturesImmediately();
			}
		}

		// Token: 0x04004EF6 RID: 20214
		[RequiredField]
		public FsmMaterial substanceMaterial;

		// Token: 0x04004EF7 RID: 20215
		[RequiredField]
		public FsmBool immediately;

		// Token: 0x04004EF8 RID: 20216
		public bool everyFrame;
	}
}
