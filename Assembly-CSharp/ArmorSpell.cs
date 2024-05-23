using System;

// Token: 0x0200032F RID: 815
public class ArmorSpell : Spell
{
	// Token: 0x06002A49 RID: 10825 RVA: 0x000CEF26 File Offset: 0x000CD126
	public int GetArmor()
	{
		return this.m_armor;
	}

	// Token: 0x06002A4A RID: 10826 RVA: 0x000CEF2E File Offset: 0x000CD12E
	public void SetArmor(int armor)
	{
		this.m_armor = armor;
		this.UpdateArmorText();
	}

	// Token: 0x06002A4B RID: 10827 RVA: 0x000CEF40 File Offset: 0x000CD140
	private void UpdateArmorText()
	{
		if (this.m_ArmorText == null)
		{
			return;
		}
		string text = this.m_armor.ToString();
		if (this.m_armor == 0)
		{
			text = string.Empty;
		}
		this.m_ArmorText.Text = text;
	}

	// Token: 0x04001930 RID: 6448
	public UberText m_ArmorText;

	// Token: 0x04001931 RID: 6449
	private int m_armor;
}
