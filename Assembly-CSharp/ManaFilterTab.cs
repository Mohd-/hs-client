using System;
using System.Collections;
using UnityEngine;

// Token: 0x020006C0 RID: 1728
public class ManaFilterTab : PegUIElement
{
	// Token: 0x06004809 RID: 18441 RVA: 0x00159C15 File Offset: 0x00157E15
	protected override void Awake()
	{
		this.m_crystal.MarkAsNotInGame();
		base.Awake();
	}

	// Token: 0x0600480A RID: 18442 RVA: 0x00159C28 File Offset: 0x00157E28
	public void SetFilterState(ManaFilterTab.FilterState state)
	{
		this.m_filterState = state;
		switch (this.m_filterState)
		{
		case ManaFilterTab.FilterState.ON:
			this.m_crystal.state = ManaCrystal.State.PROPOSED;
			break;
		case ManaFilterTab.FilterState.OFF:
			this.m_crystal.state = ManaCrystal.State.READY;
			break;
		case ManaFilterTab.FilterState.DISABLED:
			this.m_crystal.state = ManaCrystal.State.USED;
			break;
		}
	}

	// Token: 0x0600480B RID: 18443 RVA: 0x00159C90 File Offset: 0x00157E90
	public void NotifyMousedOver()
	{
		if (this.m_filterState == ManaFilterTab.FilterState.ON)
		{
			return;
		}
		this.m_crystal.state = ManaCrystal.State.PROPOSED;
		SoundManager.Get().LoadAndPlay("mana_crystal_highlight_lp", base.gameObject, 1f, new SoundManager.LoadedCallback(this.ManaCrystalSoundCallback));
	}

	// Token: 0x0600480C RID: 18444 RVA: 0x00159CDC File Offset: 0x00157EDC
	public void NotifyMousedOut()
	{
		Action<object> action = delegate(object amount)
		{
			SoundManager.Get().SetVolume(this.m_mouseOverSound, (float)amount);
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"from",
			1f,
			"to",
			0f,
			"time",
			0.5f,
			"easetype",
			iTween.EaseType.linear,
			"onupdate",
			action
		});
		iTween.Stop(base.gameObject);
		iTween.ValueTo(base.gameObject, args);
		if (this.m_filterState == ManaFilterTab.FilterState.ON)
		{
			return;
		}
		this.m_crystal.state = ManaCrystal.State.READY;
	}

	// Token: 0x0600480D RID: 18445 RVA: 0x00159D90 File Offset: 0x00157F90
	private void ManaCrystalSoundCallback(AudioSource source, object userData)
	{
		if (this.m_mouseOverSound != null)
		{
			SoundManager.Get().Stop(this.m_mouseOverSound);
		}
		this.m_mouseOverSound = source;
		SoundManager.Get().SetVolume(source, 0f);
		if (this.m_crystal.state != ManaCrystal.State.PROPOSED)
		{
			SoundManager.Get().Stop(this.m_mouseOverSound);
		}
		Action<object> action = delegate(object amount)
		{
			SoundManager.Get().SetVolume(source, (float)amount);
		};
		Hashtable args = iTween.Hash(new object[]
		{
			"from",
			0f,
			"to",
			1f,
			"time",
			0.5f,
			"easetype",
			iTween.EaseType.linear,
			"onupdate",
			action
		});
		iTween.Stop(base.gameObject);
		iTween.ValueTo(base.gameObject, args);
	}

	// Token: 0x0600480E RID: 18446 RVA: 0x00159E9E File Offset: 0x0015809E
	public void SetManaID(int manaID)
	{
		this.m_manaID = manaID;
		this.UpdateManaText();
	}

	// Token: 0x0600480F RID: 18447 RVA: 0x00159EAD File Offset: 0x001580AD
	public int GetManaID()
	{
		return this.m_manaID;
	}

	// Token: 0x06004810 RID: 18448 RVA: 0x00159EB8 File Offset: 0x001580B8
	private void UpdateManaText()
	{
		string text = string.Empty;
		string text2 = string.Empty;
		if (this.m_manaID == ManaFilterTab.ALL_TAB_IDX)
		{
			text2 = GameStrings.Get("GLUE_COLLECTION_ALL");
		}
		else
		{
			text = this.m_manaID.ToString();
			if (this.m_manaID == ManaFilterTab.SEVEN_PLUS_TAB_IDX)
			{
				if (UniversalInputManager.UsePhoneUI)
				{
					text += GameStrings.Get("GLUE_COLLECTION_PLUS");
				}
				else
				{
					text2 = GameStrings.Get("GLUE_COLLECTION_PLUS");
				}
			}
		}
		if (this.m_costText != null)
		{
			this.m_costText.Text = text;
		}
		if (this.m_otherText != null)
		{
			this.m_otherText.Text = text2;
		}
	}

	// Token: 0x04002F7A RID: 12154
	public static readonly int ALL_TAB_IDX = -1;

	// Token: 0x04002F7B RID: 12155
	public static readonly int SEVEN_PLUS_TAB_IDX = 7;

	// Token: 0x04002F7C RID: 12156
	public UberText m_costText;

	// Token: 0x04002F7D RID: 12157
	public UberText m_otherText;

	// Token: 0x04002F7E RID: 12158
	public ManaCrystal m_crystal;

	// Token: 0x04002F7F RID: 12159
	private int m_manaID;

	// Token: 0x04002F80 RID: 12160
	private ManaFilterTab.FilterState m_filterState;

	// Token: 0x04002F81 RID: 12161
	private AudioSource m_mouseOverSound;

	// Token: 0x020006C1 RID: 1729
	public enum FilterState
	{
		// Token: 0x04002F83 RID: 12163
		ON,
		// Token: 0x04002F84 RID: 12164
		OFF,
		// Token: 0x04002F85 RID: 12165
		DISABLED
	}
}
