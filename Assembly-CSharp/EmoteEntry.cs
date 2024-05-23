using System;
using UnityEngine;

// Token: 0x02000844 RID: 2116
public class EmoteEntry
{
	// Token: 0x0600511B RID: 20763 RVA: 0x00182733 File Offset: 0x00180933
	public EmoteEntry(EmoteType type, string path, string stringKey, Card owner)
	{
		this.m_emoteType = type;
		this.m_emoteSoundSpellPath = path;
		this.m_emoteGameStringKey = stringKey;
		this.m_owner = owner;
	}

	// Token: 0x0600511C RID: 20764 RVA: 0x00182758 File Offset: 0x00180958
	public EmoteType GetEmoteType()
	{
		return this.m_emoteType;
	}

	// Token: 0x0600511D RID: 20765 RVA: 0x00182760 File Offset: 0x00180960
	public string GetGameStringKey()
	{
		return this.m_emoteGameStringKey;
	}

	// Token: 0x0600511E RID: 20766 RVA: 0x00182768 File Offset: 0x00180968
	private void LoadSpell()
	{
		if (string.IsNullOrEmpty(this.m_emoteSoundSpellPath))
		{
			return;
		}
		GameObject gameObject = AssetLoader.Get().LoadSpell(this.m_emoteSoundSpellPath, true, false);
		if (gameObject == null)
		{
			if (AssetLoader.DOWNLOADABLE_LANGUAGE_PACKS)
			{
				return;
			}
			string text = string.Format("EmoteEntry.LoadSpell() - Failed to load \"{0}\"", this.m_emoteSoundSpellPath);
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
		else
		{
			this.m_emoteSoundSpell = gameObject.GetComponent<CardSoundSpell>();
			if (this.m_emoteSoundSpell == null)
			{
				Object.Destroy(gameObject);
				string text2 = string.Format("EmoteEntry.LoadSpell() - \"{0}\" does not have a Spell component.", this.m_emoteSoundSpellPath);
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
				SpellUtils.SetupSoundSpell(this.m_emoteSoundSpell, this.m_owner);
			}
			return;
		}
	}

	// Token: 0x0600511F RID: 20767 RVA: 0x00182860 File Offset: 0x00180A60
	public CardSoundSpell GetSpell(bool loadIfNeeded = true)
	{
		if (this.m_emoteSoundSpell == null && loadIfNeeded)
		{
			this.LoadSpell();
		}
		return this.m_emoteSoundSpell;
	}

	// Token: 0x06005120 RID: 20768 RVA: 0x00182885 File Offset: 0x00180A85
	public CardSoundSpell GetSpellIfLoaded()
	{
		return this.m_emoteSoundSpell;
	}

	// Token: 0x06005121 RID: 20769 RVA: 0x00182890 File Offset: 0x00180A90
	public void Clear()
	{
		if (this.m_emoteSoundSpell != null)
		{
			Object.Destroy(this.m_emoteSoundSpell.gameObject);
			this.m_emoteSoundSpell = null;
		}
	}

	// Token: 0x040037F0 RID: 14320
	private EmoteType m_emoteType;

	// Token: 0x040037F1 RID: 14321
	private CardSoundSpell m_emoteSoundSpell;

	// Token: 0x040037F2 RID: 14322
	private string m_emoteGameStringKey;

	// Token: 0x040037F3 RID: 14323
	private string m_emoteSoundSpellPath;

	// Token: 0x040037F4 RID: 14324
	private Card m_owner;
}
