using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000332 RID: 818
[CustomEditClass]
public class SpellTable : MonoBehaviour
{
	// Token: 0x06002A7C RID: 10876 RVA: 0x000D0D98 File Offset: 0x000CEF98
	public SpellTableEntry FindEntry(SpellType type)
	{
		foreach (SpellTableEntry spellTableEntry in this.m_Table)
		{
			if (spellTableEntry.m_Type == type)
			{
				return spellTableEntry;
			}
		}
		return null;
	}

	// Token: 0x06002A7D RID: 10877 RVA: 0x000D0E04 File Offset: 0x000CF004
	public Spell GetSpell(SpellType spellType)
	{
		foreach (SpellTableEntry spellTableEntry in this.m_Table)
		{
			if (spellTableEntry.m_Type == spellType)
			{
				if (spellTableEntry.m_Spell == null && spellTableEntry.m_SpellPrefabName != null)
				{
					string name = FileUtils.GameAssetPathToName(spellTableEntry.m_SpellPrefabName);
					GameObject gameObject = AssetLoader.Get().LoadActor(name, true, true);
					Spell component = gameObject.GetComponent<Spell>();
					if (component != null)
					{
						spellTableEntry.m_Spell = component;
						TransformUtil.AttachAndPreserveLocalTransform(gameObject.transform, base.gameObject.transform);
					}
				}
				if (spellTableEntry.m_Spell == null)
				{
					Debug.LogError(string.Concat(new object[]
					{
						"Unable to load spell ",
						spellType,
						" from spell table ",
						base.gameObject.name
					}));
					return null;
				}
				GameObject gameObject2 = Object.Instantiate<GameObject>(spellTableEntry.m_Spell.gameObject);
				Spell component2 = gameObject2.GetComponent<Spell>();
				component2.SetSpellType(spellType);
				return component2;
			}
		}
		return null;
	}

	// Token: 0x06002A7E RID: 10878 RVA: 0x000D0F58 File Offset: 0x000CF158
	public void ReleaseSpell(GameObject spellObject)
	{
		Object.Destroy(spellObject);
	}

	// Token: 0x06002A7F RID: 10879 RVA: 0x000D0F60 File Offset: 0x000CF160
	public void ReleaseAllSpells()
	{
		foreach (SpellTableEntry spellTableEntry in this.m_Table)
		{
			if (spellTableEntry.m_Spell != null)
			{
				Object.DestroyImmediate(spellTableEntry.m_Spell.gameObject);
				Object.DestroyImmediate(spellTableEntry.m_Spell);
				spellTableEntry.m_Spell = null;
			}
		}
	}

	// Token: 0x06002A80 RID: 10880 RVA: 0x000D0FE8 File Offset: 0x000CF1E8
	public bool IsLoaded(SpellType spellType)
	{
		Spell spell;
		this.FindSpell(spellType, out spell);
		return spell != null;
	}

	// Token: 0x06002A81 RID: 10881 RVA: 0x000D1008 File Offset: 0x000CF208
	public void SetSpell(SpellType type, Spell spell)
	{
		foreach (SpellTableEntry spellTableEntry in this.m_Table)
		{
			if (spellTableEntry.m_Type == type)
			{
				if (spellTableEntry.m_Spell == null)
				{
					spellTableEntry.m_Spell = spell;
					TransformUtil.AttachAndPreserveLocalTransform(spell.gameObject.transform, base.gameObject.transform);
				}
				return;
			}
		}
		Debug.LogError(string.Concat(new object[]
		{
			"Set invalid spell type ",
			type,
			" in spell table ",
			base.gameObject.name
		}));
	}

	// Token: 0x06002A82 RID: 10882 RVA: 0x000D10D8 File Offset: 0x000CF2D8
	public bool FindSpell(SpellType spellType, out Spell spell)
	{
		foreach (SpellTableEntry spellTableEntry in this.m_Table)
		{
			if (spellTableEntry.m_Type == spellType)
			{
				spell = spellTableEntry.m_Spell;
				return true;
			}
		}
		spell = null;
		return false;
	}

	// Token: 0x06002A83 RID: 10883 RVA: 0x000D114C File Offset: 0x000CF34C
	public void Show()
	{
		foreach (SpellTableEntry spellTableEntry in this.m_Table)
		{
			if (!(spellTableEntry.m_Spell == null))
			{
				if (spellTableEntry.m_Type != SpellType.NONE)
				{
					spellTableEntry.m_Spell.Show();
				}
			}
		}
	}

	// Token: 0x06002A84 RID: 10884 RVA: 0x000D11CC File Offset: 0x000CF3CC
	public void Hide()
	{
		foreach (SpellTableEntry spellTableEntry in this.m_Table)
		{
			if (!(spellTableEntry.m_Spell == null))
			{
				spellTableEntry.m_Spell.Hide();
			}
		}
	}

	// Token: 0x0400196F RID: 6511
	public List<SpellTableEntry> m_Table = new List<SpellTableEntry>();
}
