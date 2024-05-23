using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000292 RID: 658
public class SpellCache : MonoBehaviour
{
	// Token: 0x06002404 RID: 9220 RVA: 0x000B089D File Offset: 0x000AEA9D
	private void Awake()
	{
		SpellCache.s_instance = this;
	}

	// Token: 0x06002405 RID: 9221 RVA: 0x000B08A5 File Offset: 0x000AEAA5
	private void Start()
	{
		if (SceneMgr.Get())
		{
			SceneMgr.Get().RegisterScenePreLoadEvent(new SceneMgr.ScenePreLoadCallback(this.OnScenePreLoad));
		}
	}

	// Token: 0x06002406 RID: 9222 RVA: 0x000B08CC File Offset: 0x000AEACC
	private void OnDestroy()
	{
		SpellCache.s_instance = null;
	}

	// Token: 0x06002407 RID: 9223 RVA: 0x000B08D4 File Offset: 0x000AEAD4
	public static SpellCache Get()
	{
		if (SpellCache.s_instance == null && !Application.isEditor)
		{
			Debug.LogError("Attempting to access null SpellCache");
			return null;
		}
		return SpellCache.s_instance;
	}

	// Token: 0x06002408 RID: 9224 RVA: 0x000B0904 File Offset: 0x000AEB04
	public SpellTable GetSpellTable(string tablePath)
	{
		string text = FileUtils.GameAssetPathToName(tablePath);
		SpellTable result;
		if (!this.m_spellTableCache.TryGetValue(text, out result))
		{
			result = this.LoadSpellTable(text);
		}
		return result;
	}

	// Token: 0x06002409 RID: 9225 RVA: 0x000B0934 File Offset: 0x000AEB34
	public void Clear()
	{
		foreach (KeyValuePair<string, SpellTable> keyValuePair in this.m_spellTableCache)
		{
			keyValuePair.Value.ReleaseAllSpells();
		}
	}

	// Token: 0x0600240A RID: 9226 RVA: 0x000B0994 File Offset: 0x000AEB94
	private SpellTable LoadSpellTable(string tableName)
	{
		GameObject gameObject = AssetLoader.Get().LoadActor(tableName, false, false);
		if (gameObject == null)
		{
			Error.AddDevFatal("SpellCache.LoadSpellTable() - {0} failed to load", new object[]
			{
				base.name
			});
			return null;
		}
		SpellTable component = gameObject.GetComponent<SpellTable>();
		if (component == null)
		{
			Error.AddDevFatal("SpellCache.LoadSpellTable() - {0} has no SpellTable component", new object[]
			{
				base.name
			});
			return null;
		}
		component.transform.parent = base.transform;
		this.m_spellTableCache.Add(tableName, component);
		return component;
	}

	// Token: 0x0600240B RID: 9227 RVA: 0x000B0A24 File Offset: 0x000AEC24
	private void OnScenePreLoad(SceneMgr.Mode prevMode, SceneMgr.Mode mode, object userData)
	{
		if (mode != SceneMgr.Mode.GAMEPLAY)
		{
			if (mode == SceneMgr.Mode.COLLECTIONMANAGER || mode == SceneMgr.Mode.TAVERN_BRAWL)
			{
				this.PreloadSpell("Card_Hand_Ally_SpellTable", SpellType.DEATHREVERSE);
				this.PreloadSpell("Card_Hand_Ability_SpellTable", SpellType.DEATHREVERSE);
				this.PreloadSpell("Card_Hand_Weapon_SpellTable", SpellType.DEATHREVERSE);
				this.PreloadSpell("Card_Hand_Ally_SpellTable", SpellType.GHOSTCARD);
				this.PreloadSpell("Card_Hand_Ability_SpellTable", SpellType.GHOSTCARD);
				this.PreloadSpell("Card_Hand_Weapon_SpellTable", SpellType.GHOSTCARD);
			}
		}
		else
		{
			this.PreloadSpell("Card_Hand_Ability_SpellTable", SpellType.SPELL_POWER_HINT_IDLE);
			this.PreloadSpell("Card_Hand_Ability_SpellTable", SpellType.SPELL_POWER_HINT_BURST);
			this.PreloadSpell("Card_Hand_Ability_SpellTable", SpellType.POWER_UP);
			this.PreloadSpell("Card_Hand_Ally_SpellTable", SpellType.SUMMON_OUT_MEDIUM);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.OPPONENT_ATTACK);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.STEALTH);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.DAMAGE);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.DEATH);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.SUMMON_OUT);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.FROZEN);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.FRIENDLY_ATTACK);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.SUMMON_IN_MEDIUM);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.SUMMON_IN);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.SUMMON_IN_OPPONENT);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.BATTLECRY);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.ENCHANT_POSITIVE);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.ENCHANT_NEGATIVE);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.ENCHANT_NEUTRAL);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.TAUNT_STEALTH);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.TRIGGER);
			this.PreloadSpell("Card_Play_Ally_SpellTable", SpellType.Zzz);
			this.PreloadSpell("Card_Hidden_SpellTable", SpellType.SUMMON_OUT);
			this.PreloadSpell("Card_Hidden_SpellTable", SpellType.SUMMON_IN);
			this.PreloadSpell("Card_Hidden_SpellTable", SpellType.SUMMON_OUT_WEAPON);
			this.PreloadSpell("Card_Play_Hero_SpellTable", SpellType.ENDGAME_WIN);
			this.PreloadSpell("Card_Play_Hero_SpellTable", SpellType.OPPONENT_ATTACK);
			this.PreloadSpell("Card_Play_Hero_SpellTable", SpellType.FRIENDLY_ATTACK);
			this.PreloadSpell("Card_Play_Hero_SpellTable", SpellType.FROZEN);
			this.PreloadSpell("Card_Play_Hero_SpellTable", SpellType.DAMAGE);
			this.PreloadSpell("Card_Play_Weapon_SpellTable", SpellType.ENCHANT_POSITIVE);
			this.PreloadSpell("Card_Play_Weapon_SpellTable", SpellType.ENCHANT_NEUTRAL);
			this.PreloadSpell("Card_Play_Weapon_SpellTable", SpellType.ENCHANT_NEGATIVE);
			this.PreloadSpell("Card_Play_Weapon_SpellTable", SpellType.DAMAGE);
			this.PreloadSpell("Card_Play_Weapon_SpellTable", SpellType.DEATH);
			this.PreloadSpell("Card_Play_Weapon_SpellTable", SpellType.SHEATHE);
			this.PreloadSpell("Card_Play_Weapon_SpellTable", SpellType.UNSHEATHE);
			this.PreloadSpell("Card_Play_Weapon_SpellTable", SpellType.SUMMON_IN_OPPONENT);
			this.PreloadSpell("Card_Play_Weapon_SpellTable", SpellType.SUMMON_IN_FRIENDLY);
			this.PreloadSpell("Card_Play_Hero_SpellTable", SpellType.FRIENDLY_ATTACK);
		}
	}

	// Token: 0x0600240C RID: 9228 RVA: 0x000B0C98 File Offset: 0x000AEE98
	private void PreloadSpell(string tableName, SpellType type)
	{
		SpellTable spellTable = this.GetSpellTable(tableName);
		if (spellTable == null)
		{
			Error.AddDevFatal("SpellCache.PreloadSpell() - Preloaded nonexistent SpellTable {0}", new object[]
			{
				tableName
			});
			return;
		}
		SpellTableEntry spellTableEntry = spellTable.FindEntry(type);
		if (spellTableEntry == null)
		{
			Error.AddDevFatal("SpellCache.PreloadSpell() - SpellTable {0} has no spell of type {1}", new object[]
			{
				tableName,
				type
			});
			return;
		}
		if (spellTableEntry.m_Spell != null)
		{
			return;
		}
		string text = FileUtils.GameAssetPathToName(spellTableEntry.m_SpellPrefabName);
		GameObject gameObject = AssetLoader.Get().LoadActor(text, true, true);
		if (gameObject == null)
		{
			Error.AddDevFatal("SpellCache.PreloadSpell() - Failed to load {0}", new object[]
			{
				text
			});
			return;
		}
		Spell component = gameObject.GetComponent<Spell>();
		if (component == null)
		{
			Error.AddDevFatal("SpellCache.PreloadSpell() - {0} does not have a Spell component", new object[]
			{
				text
			});
			return;
		}
		spellTable.SetSpell(type, component);
	}

	// Token: 0x040014FF RID: 5375
	private static SpellCache s_instance;

	// Token: 0x04001500 RID: 5376
	private Map<string, SpellTable> m_spellTableCache = new Map<string, SpellTable>();
}
