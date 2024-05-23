using System;

// Token: 0x02000E29 RID: 3625
[Serializable]
public class SpellImpactInfo
{
	// Token: 0x04005761 RID: 22369
	public bool m_Enabled = true;

	// Token: 0x04005762 RID: 22370
	public Spell m_Prefab;

	// Token: 0x04005763 RID: 22371
	public SpellImpactPrefabs[] m_DamageAmountImpacts;

	// Token: 0x04005764 RID: 22372
	public bool m_UseSuperSpellLocation;

	// Token: 0x04005765 RID: 22373
	public SpellLocation m_Location = SpellLocation.TARGET;

	// Token: 0x04005766 RID: 22374
	public bool m_SetParentToLocation;

	// Token: 0x04005767 RID: 22375
	public float m_SpawnDelaySecMin;

	// Token: 0x04005768 RID: 22376
	public float m_SpawnDelaySecMax;

	// Token: 0x04005769 RID: 22377
	public float m_GameDelaySecMin = 0.5f;

	// Token: 0x0400576A RID: 22378
	public float m_GameDelaySecMax = 0.5f;
}
