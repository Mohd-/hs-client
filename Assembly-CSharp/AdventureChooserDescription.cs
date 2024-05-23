using System;
using UnityEngine;

// Token: 0x020001E2 RID: 482
[CustomEditClass]
public class AdventureChooserDescription : MonoBehaviour
{
	// Token: 0x17000328 RID: 808
	// (get) Token: 0x06001DB1 RID: 7601 RVA: 0x0008A84B File Offset: 0x00088A4B
	// (set) Token: 0x06001DB2 RID: 7602 RVA: 0x0008A858 File Offset: 0x00088A58
	[CustomEditField(Sections = "Description")]
	public Color WarningTextColor
	{
		get
		{
			return this.m_WarningTextColor;
		}
		set
		{
			this.m_WarningTextColor = value;
			this.RefreshText();
		}
	}

	// Token: 0x06001DB3 RID: 7603 RVA: 0x0008A86C File Offset: 0x00088A6C
	public string GetText()
	{
		return this.m_DescriptionObject.Text;
	}

	// Token: 0x06001DB4 RID: 7604 RVA: 0x0008A879 File Offset: 0x00088A79
	public void SetText(string requiredText, string descText)
	{
		this.m_RequiredText = requiredText;
		this.m_DescText = descText;
		this.RefreshText();
	}

	// Token: 0x06001DB5 RID: 7605 RVA: 0x0008A890 File Offset: 0x00088A90
	private void RefreshText()
	{
		string text2;
		if (!string.IsNullOrEmpty(this.m_RequiredText))
		{
			string text = this.m_WarningTextColor.r.ToString("X2") + this.m_WarningTextColor.g.ToString("X2") + this.m_WarningTextColor.b.ToString("X2");
			text2 = string.Concat(new string[]
			{
				"<color=#",
				text,
				">• ",
				this.m_RequiredText,
				" •</color>\n",
				this.m_DescText
			});
		}
		else
		{
			text2 = this.m_DescText;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			this.m_DescriptionObject.CharacterSize = 70f;
		}
		this.m_DescriptionObject.Text = text2;
	}

	// Token: 0x04001063 RID: 4195
	[SerializeField]
	private Color32 m_WarningTextColor = new Color32(byte.MaxValue, 210, 23, byte.MaxValue);

	// Token: 0x04001064 RID: 4196
	[CustomEditField(Sections = "Description")]
	public UberText m_DescriptionObject;

	// Token: 0x04001065 RID: 4197
	private string m_RequiredText;

	// Token: 0x04001066 RID: 4198
	private string m_DescText;
}
