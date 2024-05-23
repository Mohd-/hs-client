using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200026A RID: 618
[CustomEditClass]
public class SpecialEventVisualMgr : MonoBehaviour
{
	// Token: 0x060022C5 RID: 8901 RVA: 0x000AB4C8 File Offset: 0x000A96C8
	private void Awake()
	{
		SpecialEventVisualMgr.s_instance = this;
	}

	// Token: 0x060022C6 RID: 8902 RVA: 0x000AB4D0 File Offset: 0x000A96D0
	private void OnDestroy()
	{
		SpecialEventVisualMgr.s_instance = null;
	}

	// Token: 0x060022C7 RID: 8903 RVA: 0x000AB4D8 File Offset: 0x000A96D8
	public static SpecialEventVisualMgr Get()
	{
		return SpecialEventVisualMgr.s_instance;
	}

	// Token: 0x060022C8 RID: 8904 RVA: 0x000AB4E0 File Offset: 0x000A96E0
	public bool LoadEvent(SpecialEventType eventType)
	{
		for (int i = 0; i < this.m_EventDefs.Count; i++)
		{
			SpecialEventVisualDef specialEventVisualDef = this.m_EventDefs[i];
			if (specialEventVisualDef.m_EventType == eventType)
			{
				string name = FileUtils.GameAssetPathToName(specialEventVisualDef.m_Prefab);
				AssetLoader.Get().LoadGameObject(name, null, null, false);
				return true;
			}
		}
		return false;
	}

	// Token: 0x060022C9 RID: 8905 RVA: 0x000AB540 File Offset: 0x000A9740
	public bool UnloadEvent(SpecialEventType eventType)
	{
		for (int i = 0; i < this.m_EventDefs.Count; i++)
		{
			SpecialEventVisualDef specialEventVisualDef = this.m_EventDefs[i];
			if (specialEventVisualDef.m_EventType == eventType)
			{
				string text = FileUtils.GameAssetPathToName(specialEventVisualDef.m_Prefab + "(Clone)");
				GameObject gameObject = GameObject.Find(text);
				if (gameObject != null)
				{
					Object.Destroy(gameObject);
				}
			}
		}
		return false;
	}

	// Token: 0x060022CA RID: 8906 RVA: 0x000AB5B2 File Offset: 0x000A97B2
	private void OnEventFinished(Spell spell, object userData)
	{
		if (spell.GetActiveState() != SpellStateType.NONE)
		{
			return;
		}
		Object.Destroy(spell.gameObject);
	}

	// Token: 0x060022CB RID: 8907 RVA: 0x000AB5CC File Offset: 0x000A97CC
	public static SpecialEventType GetActiveEventType()
	{
		if (SpecialEventManager.Get().IsEventActive(SpecialEventType.GVG_PROMOTION, false))
		{
			return SpecialEventType.GVG_PROMOTION;
		}
		if (SpecialEventManager.Get().IsEventActive(SpecialEventType.SPECIAL_EVENT_PRE_TAVERN_BRAWL, false))
		{
			return SpecialEventType.SPECIAL_EVENT_PRE_TAVERN_BRAWL;
		}
		return SpecialEventType.IGNORE;
	}

	// Token: 0x04001427 RID: 5159
	public List<SpecialEventVisualDef> m_EventDefs = new List<SpecialEventVisualDef>();

	// Token: 0x04001428 RID: 5160
	private static SpecialEventVisualMgr s_instance;
}
