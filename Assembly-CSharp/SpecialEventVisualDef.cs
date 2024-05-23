using System;

// Token: 0x0200068F RID: 1679
[Serializable]
public class SpecialEventVisualDef
{
	// Token: 0x04002E13 RID: 11795
	[CustomEditField]
	public SpecialEventType m_EventType;

	// Token: 0x04002E14 RID: 11796
	[CustomEditField(T = EditType.GAME_OBJECT)]
	public string m_Prefab;
}
