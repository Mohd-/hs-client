using System;

// Token: 0x02000E2D RID: 3629
[Serializable]
public class SpellAreaEffectInfo
{
	// Token: 0x0400577E RID: 22398
	public bool m_Enabled = true;

	// Token: 0x0400577F RID: 22399
	public Spell m_Prefab;

	// Token: 0x04005780 RID: 22400
	public bool m_UseSuperSpellLocation;

	// Token: 0x04005781 RID: 22401
	public SpellLocation m_Location;

	// Token: 0x04005782 RID: 22402
	public bool m_SetParentToLocation;

	// Token: 0x04005783 RID: 22403
	public SpellFacing m_Facing;

	// Token: 0x04005784 RID: 22404
	public SpellFacingOptions m_FacingOptions;

	// Token: 0x04005785 RID: 22405
	public float m_SpawnDelaySecMin;

	// Token: 0x04005786 RID: 22406
	public float m_SpawnDelaySecMax;
}
