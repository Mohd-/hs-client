using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000F74 RID: 3956
public class CardTextTool : MonoBehaviour
{
	// Token: 0x06007545 RID: 30021 RVA: 0x00229B8C File Offset: 0x00227D8C
	private void Start()
	{
		GraphicsManager.Get().RenderQualityLevel = GraphicsQuality.Medium;
		Application.targetFrameRate = 20;
		Application.runInBackground = false;
		base.StartCoroutine(this.Initialize());
	}

	// Token: 0x06007546 RID: 30022 RVA: 0x00229BBE File Offset: 0x00227DBE
	private void OnApplicationQuit()
	{
		PlayerPrefs.SetString("CARD_TEXT_NAME", this.m_nameText);
		PlayerPrefs.SetString("CARD_TEXT_DESCRIPTION", this.m_descriptionText);
		PlayerPrefs.Save();
	}

	// Token: 0x06007547 RID: 30023 RVA: 0x00229BE8 File Offset: 0x00227DE8
	public void UpdateDescriptionText()
	{
		string text = this.m_DescriptionInputFiled.text;
		this.m_descriptionText = text;
		text = this.FixedNewline(text);
		this.m_AbilityCardDescription.Text = text;
		this.m_AllyCardDescription.Text = text;
		this.m_WeaponCardDescription.Text = text;
	}

	// Token: 0x06007548 RID: 30024 RVA: 0x00229C34 File Offset: 0x00227E34
	public void UpdateNameText()
	{
		string text = this.m_NameInputFiled.text;
		this.m_nameText = text;
		this.m_AbilityCardName.Text = text;
		this.m_AllyCardName.Text = text;
		this.m_WeaponCardName.Text = text;
	}

	// Token: 0x06007549 RID: 30025 RVA: 0x00229C78 File Offset: 0x00227E78
	public void PasteClipboard()
	{
		Type typeFromHandle = typeof(GUIUtility);
		PropertyInfo property = typeFromHandle.GetProperty("systemCopyBuffer", 40);
		this.m_descriptionText = (string)property.GetValue(null, null);
		this.m_DescriptionInputFiled.text = this.m_descriptionText;
		this.UpdateDescriptionText();
	}

	// Token: 0x0600754A RID: 30026 RVA: 0x00229CC8 File Offset: 0x00227EC8
	public void CopyToClipboard()
	{
		Type typeFromHandle = typeof(GUIUtility);
		PropertyInfo property = typeFromHandle.GetProperty("systemCopyBuffer", 40);
		property.SetValue(null, this.m_descriptionText, null);
	}

	// Token: 0x0600754B RID: 30027 RVA: 0x00229CFC File Offset: 0x00227EFC
	private IEnumerator Initialize()
	{
		if (!AssetLoader.Get().IsReady())
		{
			yield return null;
		}
		if (PlayerPrefs.HasKey("CARD_TEXT_LOCALE"))
		{
			this.m_locale = (Locale)PlayerPrefs.GetInt("CARD_TEXT_LOCALE");
		}
		Localization.SetLocale(this.m_locale);
		Localization.Initialize();
		FontTable.Get().Initialize();
		if (!FontTable.Get().IsInitialized())
		{
			yield return null;
		}
		this.SetupLocaleDropDown();
		this.SetLocale();
		this.m_AbilityActor.SetPortraitTexture(this.m_AbilityPortraitTexture);
		this.m_AllyActor.SetPortraitTexture(this.m_AllyPortraitTexture);
		this.m_WeaponActor.SetPortraitTexture(this.m_WeaponPortraitTexture);
		if (PlayerPrefs.HasKey("CARD_TEXT_NAME"))
		{
			this.m_NameInputFiled.text = PlayerPrefs.GetString("CARD_TEXT_NAME");
		}
		if (PlayerPrefs.HasKey("CARD_TEXT_DESCRIPTION"))
		{
			this.m_DescriptionInputFiled.text = PlayerPrefs.GetString("CARD_TEXT_DESCRIPTION");
		}
		foreach (UberText ut in this.m_CardsRoot.GetComponentsInChildren<UberText>())
		{
			ut.Cache = false;
		}
		this.UpdateDescriptionText();
		this.UpdateNameText();
		yield break;
	}

	// Token: 0x0600754C RID: 30028 RVA: 0x00229D18 File Offset: 0x00227F18
	private string FixedNewline(string text)
	{
		if (text.Length < 2)
		{
			return text;
		}
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < text.Length; i++)
		{
			if (i + 1 < text.Length && text.get_Chars(i) == '\\' && text.get_Chars(i + 1) == 'n')
			{
				stringBuilder.Append('\n');
				i++;
			}
			else
			{
				stringBuilder.Append(text.get_Chars(i));
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x0600754D RID: 30029 RVA: 0x00229DA4 File Offset: 0x00227FA4
	private void SetupLocaleDropDown()
	{
		GameObject gameObject = this.m_LocaleDropDownSelectionButton.transform.parent.gameObject;
		gameObject.SetActive(true);
		foreach (object obj in Enum.GetValues(typeof(Locale)))
		{
			Locale locale = (Locale)((int)obj);
			if (locale != Locale.UNKNOWN)
			{
				GameObject gameObject2 = Object.Instantiate<GameObject>(this.m_LocaleDropDownSelectionButton.gameObject);
				gameObject2.transform.parent = this.m_LocaleDropDownSelectionButton.transform.parent;
				Button component = gameObject2.GetComponent<Button>();
				Text componentInChildren = component.GetComponentInChildren<Text>();
				componentInChildren.text = locale.ToString();
				Locale locSet = locale;
				component.onClick.AddListener(delegate()
				{
					this.OnClick_LocaleSetButton(locSet);
				});
			}
		}
		Object.Destroy(this.m_LocaleDropDownSelectionButton.gameObject);
		this.SetLocaleButtonText(this.m_locale);
		gameObject.SetActive(false);
	}

	// Token: 0x0600754E RID: 30030 RVA: 0x00229ED8 File Offset: 0x002280D8
	private void OnClick_LocaleSetButton(Locale locale)
	{
		Text componentInChildren = this.m_LocaleDropDownMainButton.GetComponentInChildren<Text>();
		componentInChildren.text = locale.ToString();
		this.m_locale = locale;
		this.SaveLocale(this.m_locale);
		this.SetLocale();
	}

	// Token: 0x0600754F RID: 30031 RVA: 0x00229F1C File Offset: 0x0022811C
	private void SetLocaleButtonText(Locale loc)
	{
		Text componentInChildren = this.m_LocaleDropDownMainButton.GetComponentInChildren<Text>();
		componentInChildren.text = loc.ToString();
	}

	// Token: 0x06007550 RID: 30032 RVA: 0x00229F46 File Offset: 0x00228146
	private void SaveLocale(Locale loc)
	{
		PlayerPrefs.SetInt("CARD_TEXT_LOCALE", (int)this.m_locale);
		PlayerPrefs.Save();
	}

	// Token: 0x06007551 RID: 30033 RVA: 0x00229F5D File Offset: 0x0022815D
	private void SetLocale()
	{
		base.StartCoroutine(this.SetLocaleCoroutine());
	}

	// Token: 0x06007552 RID: 30034 RVA: 0x00229F6C File Offset: 0x0022816C
	private IEnumerator SetLocaleCoroutine()
	{
		Localization.SetLocale(this.m_locale);
		yield return null;
		this.UpdateCardFonts(Locale.enUS);
		this.UpdateCardFonts(this.m_locale);
		yield break;
	}

	// Token: 0x06007553 RID: 30035 RVA: 0x00229F88 File Offset: 0x00228188
	private void UpdateCardFonts(Locale loc)
	{
		foreach (CardTextTool.LocalizedFonts localizedFonts in this.m_LocalizedFonts)
		{
			if (localizedFonts.m_Locale == loc)
			{
				if (localizedFonts.m_FontDef.name == "FranklinGothic")
				{
					this.m_AbilityCardDescription.SetFontWithoutLocalization(localizedFonts.m_FontDef);
					this.m_AllyCardDescription.SetFontWithoutLocalization(localizedFonts.m_FontDef);
					this.m_WeaponCardDescription.SetFontWithoutLocalization(localizedFonts.m_FontDef);
				}
				if (localizedFonts.m_FontDef.name == "Belwe_Outline")
				{
					this.m_AbilityCardName.SetFontWithoutLocalization(localizedFonts.m_FontDef);
					this.m_AllyCardName.SetFontWithoutLocalization(localizedFonts.m_FontDef);
					this.m_WeaponCardName.SetFontWithoutLocalization(localizedFonts.m_FontDef);
				}
			}
		}
	}

	// Token: 0x04005FC1 RID: 24513
	private const string PREFS_LOCALE = "CARD_TEXT_LOCALE";

	// Token: 0x04005FC2 RID: 24514
	private const string PREFS_NAME = "CARD_TEXT_NAME";

	// Token: 0x04005FC3 RID: 24515
	private const string PREFS_DESCRIPTION = "CARD_TEXT_DESCRIPTION";

	// Token: 0x04005FC4 RID: 24516
	public GameObject m_CardsRoot;

	// Token: 0x04005FC5 RID: 24517
	public Actor m_AbilityActor;

	// Token: 0x04005FC6 RID: 24518
	public Actor m_AllyActor;

	// Token: 0x04005FC7 RID: 24519
	public Actor m_WeaponActor;

	// Token: 0x04005FC8 RID: 24520
	public Texture2D m_AbilityPortraitTexture;

	// Token: 0x04005FC9 RID: 24521
	public Texture2D m_AllyPortraitTexture;

	// Token: 0x04005FCA RID: 24522
	public Texture2D m_WeaponPortraitTexture;

	// Token: 0x04005FCB RID: 24523
	public UberText m_AbilityCardDescription;

	// Token: 0x04005FCC RID: 24524
	public UberText m_AllyCardDescription;

	// Token: 0x04005FCD RID: 24525
	public UberText m_WeaponCardDescription;

	// Token: 0x04005FCE RID: 24526
	public InputField m_DescriptionInputFiled;

	// Token: 0x04005FCF RID: 24527
	public UberText m_AbilityCardName;

	// Token: 0x04005FD0 RID: 24528
	public UberText m_AllyCardName;

	// Token: 0x04005FD1 RID: 24529
	public UberText m_WeaponCardName;

	// Token: 0x04005FD2 RID: 24530
	public InputField m_NameInputFiled;

	// Token: 0x04005FD3 RID: 24531
	public Button m_LocaleDropDownMainButton;

	// Token: 0x04005FD4 RID: 24532
	public Button m_LocaleDropDownSelectionButton;

	// Token: 0x04005FD5 RID: 24533
	public List<CardTextTool.LocalizedFonts> m_LocalizedFonts;

	// Token: 0x04005FD6 RID: 24534
	private string m_nameText;

	// Token: 0x04005FD7 RID: 24535
	private string m_descriptionText;

	// Token: 0x04005FD8 RID: 24536
	private Locale m_locale;

	// Token: 0x02000F75 RID: 3957
	[Serializable]
	public class LocalizedFonts
	{
		// Token: 0x04005FD9 RID: 24537
		public Locale m_Locale;

		// Token: 0x04005FDA RID: 24538
		public FontDef m_FontDef;
	}
}
