using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000842 RID: 2114
public class CardEffect
{
	// Token: 0x0600510E RID: 20750 RVA: 0x00182294 File Offset: 0x00180494
	public CardEffect(CardEffectDef def, Card owner)
	{
		this.m_spellPath = def.m_SpellPath;
		this.m_soundSpellPaths = def.m_SoundSpellPaths;
		this.m_owner = owner;
		if (this.m_soundSpellPaths != null)
		{
			this.m_soundSpells = new List<CardSoundSpell>(this.m_soundSpellPaths.Count);
			for (int i = 0; i < this.m_soundSpellPaths.Count; i++)
			{
				this.m_soundSpells.Add(null);
			}
		}
	}

	// Token: 0x0600510F RID: 20751 RVA: 0x0018230F File Offset: 0x0018050F
	public CardEffect(string spellPath, Card owner)
	{
		this.m_spellPath = spellPath;
		this.m_owner = owner;
	}

	// Token: 0x06005110 RID: 20752 RVA: 0x00182325 File Offset: 0x00180525
	public Spell GetSpell(bool loadIfNeeded = true)
	{
		if (this.m_spell == null && !string.IsNullOrEmpty(this.m_spellPath) && loadIfNeeded)
		{
			this.LoadSpell();
		}
		return this.m_spell;
	}

	// Token: 0x06005111 RID: 20753 RVA: 0x0018235C File Offset: 0x0018055C
	public void LoadSoundSpell(int index)
	{
		if (index < 0 || this.m_soundSpellPaths == null || index >= this.m_soundSpellPaths.Count || string.IsNullOrEmpty(this.m_soundSpellPaths[index]))
		{
			return;
		}
		if (this.m_soundSpells[index] == null)
		{
			string name = this.m_soundSpellPaths[index];
			GameObject gameObject = AssetLoader.Get().LoadSpell(name, true, false);
			if (gameObject == null)
			{
				if (!AssetLoader.DOWNLOADABLE_LANGUAGE_PACKS)
				{
					Error.AddDevFatal("CardEffect.LoadSoundSpell() - FAILED TO LOAD \"{0}\" (index {1})", new object[]
					{
						this.m_spellPath,
						index
					});
				}
				return;
			}
			CardSoundSpell component = gameObject.GetComponent<CardSoundSpell>();
			this.m_soundSpells[index] = component;
			if (component == null)
			{
				if (!AssetLoader.DOWNLOADABLE_LANGUAGE_PACKS)
				{
					Error.AddDevFatal("CardEffect.LoadSoundSpell() - FAILED TO LOAD \"{0}\" (index {1})", new object[]
					{
						this.m_spellPath,
						index
					});
				}
			}
			else if (this.m_owner != null)
			{
				SpellUtils.SetupSoundSpell(component, this.m_owner);
			}
		}
	}

	// Token: 0x06005112 RID: 20754 RVA: 0x00182488 File Offset: 0x00180688
	public List<CardSoundSpell> GetSoundSpells(bool loadIfNeeded = true)
	{
		if (this.m_soundSpells == null)
		{
			return null;
		}
		if (loadIfNeeded)
		{
			for (int i = 0; i < this.m_soundSpells.Count; i++)
			{
				this.LoadSoundSpell(i);
			}
		}
		return this.m_soundSpells;
	}

	// Token: 0x06005113 RID: 20755 RVA: 0x001824D4 File Offset: 0x001806D4
	public void Clear()
	{
		if (this.m_spell != null)
		{
			Object.Destroy(this.m_spell.gameObject);
		}
		if (this.m_soundSpells != null)
		{
			for (int i = 0; i < this.m_soundSpells.Count; i++)
			{
				Spell spell = this.m_soundSpells[i];
				if (spell != null)
				{
					Object.Destroy(spell.gameObject);
				}
			}
		}
	}

	// Token: 0x06005114 RID: 20756 RVA: 0x00182550 File Offset: 0x00180750
	public void LoadAll()
	{
		this.GetSpell(true);
		if (this.m_soundSpellPaths != null)
		{
			for (int i = 0; i < this.m_soundSpellPaths.Count; i++)
			{
				this.LoadSoundSpell(i);
			}
		}
	}

	// Token: 0x06005115 RID: 20757 RVA: 0x00182593 File Offset: 0x00180793
	public void PurgeSpells()
	{
		SpellUtils.PurgeSpell(this.m_spell);
		SpellUtils.PurgeSpells<CardSoundSpell>(this.m_soundSpells);
	}

	// Token: 0x06005116 RID: 20758 RVA: 0x001825AC File Offset: 0x001807AC
	private void LoadSpell()
	{
		GameObject gameObject = AssetLoader.Get().LoadSpell(this.m_spellPath, true, false);
		if (gameObject == null)
		{
			string text = string.Format("CardEffect.LoadSpell() - Failed to load \"{0}\"", this.m_spellPath);
			if (ApplicationMgr.UseDevWorkarounds())
			{
				Debug.LogError(text);
			}
			else
			{
				Error.AddDevFatal(text, new object[0]);
			}
			return;
		}
		this.m_spell = gameObject.GetComponent<Spell>();
		if (this.m_spell == null)
		{
			Object.Destroy(gameObject);
			string text2 = string.Format("CardEffect.LoadSpell() - \"{0}\" does not have a Spell component.", this.m_spellPath);
			if (ApplicationMgr.UseDevWorkarounds())
			{
				Debug.LogError(text2);
			}
			else
			{
				Error.AddDevFatal(text2, new object[0]);
			}
			return;
		}
		if (this.m_owner != null)
		{
			SpellUtils.SetupSpell(this.m_spell, this.m_owner);
		}
	}

	// Token: 0x06005117 RID: 20759 RVA: 0x00182683 File Offset: 0x00180883
	private void DestroySpell(Spell spell)
	{
		if (spell == null)
		{
			return;
		}
		Object.Destroy(spell.gameObject);
	}

	// Token: 0x040037E9 RID: 14313
	private Spell m_spell;

	// Token: 0x040037EA RID: 14314
	private List<CardSoundSpell> m_soundSpells;

	// Token: 0x040037EB RID: 14315
	private string m_spellPath;

	// Token: 0x040037EC RID: 14316
	private List<string> m_soundSpellPaths;

	// Token: 0x040037ED RID: 14317
	private Card m_owner;
}
