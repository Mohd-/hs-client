using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020007D0 RID: 2000
public class CreditsDisplay : MonoBehaviour
{
	// Token: 0x06004E1F RID: 19999 RVA: 0x00173AAB File Offset: 0x00171CAB
	public static CreditsDisplay Get()
	{
		return CreditsDisplay.s_instance;
	}

	// Token: 0x06004E20 RID: 20000 RVA: 0x00173AB4 File Offset: 0x00171CB4
	private void Awake()
	{
		CreditsDisplay.s_instance = this;
		this.m_fakeCards = new List<Actor>();
		this.m_creditsDefs = new List<FullDef>();
		this.creditsRootStartLocalPosition = this.m_creditsRoot.transform.localPosition;
		this.creditsText1StartLocalPosition = this.m_creditsText1.transform.localPosition;
		this.creditsText2StartLocalPosition = this.m_creditsText2.transform.localPosition;
		this.m_doneButton.SetText(GameStrings.Get("GLOBAL_BACK"));
		this.m_doneButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnDonePressed));
		this.m_yearButton.SetText((!this.m_displayingLatestYear) ? "2015" : "2014");
		this.m_yearButton.AddEventListener(UIEventType.RELEASE, new UIEvent.Handler(this.OnYearPressed));
		if (UniversalInputManager.UsePhoneUI)
		{
			Box.Get().m_tableTop.SetActive(false);
			Box.Get().m_letterboxingContainer.SetActive(false);
			this.m_doneButton.SetText(string.Empty);
			this.m_doneArrowInButton.SetActive(true);
		}
		AssetLoader.Get().LoadActor("Card_Hand_Ally", new AssetLoader.GameObjectCallback(this.ActorLoadedCallback), null, false);
		AssetLoader.Get().LoadActor("Card_Hand_Ally", new AssetLoader.GameObjectCallback(this.ActorLoadedCallback), null, false);
		this.LoadAllCreditsCards();
		this.LoadCreditsText();
		Navigation.Push(new Navigation.NavigateBackHandler(this.EndCredits));
	}

	// Token: 0x06004E21 RID: 20001 RVA: 0x00173C2F File Offset: 0x00171E2F
	private void OnDestoy()
	{
		CreditsDisplay.s_instance = null;
	}

	// Token: 0x06004E22 RID: 20002 RVA: 0x00173C38 File Offset: 0x00171E38
	private void LoadAllCreditsCards()
	{
		if (this.m_displayingLatestYear)
		{
			DefLoader.Get().LoadFullDef("CRED_01", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_02", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_03", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_04", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_05", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_06", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_07", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_08", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_09", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_10", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_11", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_12", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_14", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_15", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_16", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_18", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_19", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_20", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_21", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_22", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_23", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_24", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_25", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_26", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_27", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_28", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_29", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_30", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_31", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_32", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_33", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_34", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_35", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_36", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_37", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_38", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_39", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_40", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_41", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_42", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_43", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_44", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_45", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_46", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
		}
		else
		{
			DefLoader.Get().LoadFullDef("CRED_01", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_02", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_03", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_04", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_05", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_06", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_07", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_08", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_09", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_10", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_11", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_12", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_13", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_14", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_15", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_16", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
			DefLoader.Get().LoadFullDef("CRED_17", new DefLoader.LoadDefCallback<FullDef>(this.OnFullDefLoaded));
		}
	}

	// Token: 0x06004E23 RID: 20003 RVA: 0x001742C4 File Offset: 0x001724C4
	private void LoadCreditsText()
	{
		this.m_creditsTextLoadSucceeded = false;
		string filePath = this.GetFilePath();
		if (filePath == null)
		{
			Error.AddDevWarning("Credits Error", "CreditsDisplay.LoadCreditsText() - Failed to find file for CREDITS.", new object[0]);
			this.m_creditsTextLoaded = true;
			return;
		}
		try
		{
			this.m_creditLines = File.ReadAllLines(filePath);
			this.m_creditsTextLoadSucceeded = true;
		}
		catch (Exception ex)
		{
			Error.AddDevWarning("Credits Error", "CreditsDisplay.LoadCreditsText() - Failed to read \"{0}\".\n\nException: {1}", new object[]
			{
				filePath,
				ex.Message
			});
		}
		this.m_creditsTextLoaded = true;
	}

	// Token: 0x06004E24 RID: 20004 RVA: 0x0017435C File Offset: 0x0017255C
	private string GetFilePath()
	{
		Locale[] loadOrder = Localization.GetLoadOrder(false);
		for (int i = 0; i < loadOrder.Length; i++)
		{
			string fileName = "CREDITS_" + ((!this.m_displayingLatestYear) ? "2014" : "2015") + ".txt";
			string assetPath = GameStrings.GetAssetPath(loadOrder[i], fileName);
			if (File.Exists(assetPath))
			{
				return assetPath;
			}
		}
		return null;
	}

	// Token: 0x06004E25 RID: 20005 RVA: 0x001743C8 File Offset: 0x001725C8
	private void FlopCredits()
	{
		if (this.m_currentText == this.m_creditsText1)
		{
			this.m_currentText = this.m_creditsText2;
		}
		else
		{
			this.m_currentText = this.m_creditsText1;
		}
		this.m_currentText.Text = this.GetNextCreditsChunk();
		this.DropText();
	}

	// Token: 0x06004E26 RID: 20006 RVA: 0x00174420 File Offset: 0x00172620
	private void DropText()
	{
		UberText uberText = this.m_creditsText1;
		if (this.m_currentText == this.m_creditsText1)
		{
			uberText = this.m_creditsText2;
		}
		float num = 1.8649f;
		TransformUtil.SetPoint(this.m_currentText.gameObject, Anchor.FRONT, uberText.gameObject, Anchor.BACK, new Vector3(0f, 0f, num));
	}

	// Token: 0x06004E27 RID: 20007 RVA: 0x00174484 File Offset: 0x00172684
	private string GetNextCreditsChunk()
	{
		string text = string.Empty;
		int currentLine = this.m_currentLine;
		int num = 70;
		for (int i = 0; i < num; i++)
		{
			if (this.m_creditLines.Length < i + currentLine + 1)
			{
				this.m_creditsDone = true;
				this.StartEndCreditsTimer();
				return text;
			}
			string text2 = this.m_creditLines[i + currentLine];
			if (text2.Length > 38)
			{
				num -= Mathf.CeilToInt((float)(text2.Length / 38));
				if (i > num && i > 60)
				{
					break;
				}
			}
			text += text2;
			text += Environment.NewLine;
			this.m_currentLine++;
		}
		return text;
	}

	// Token: 0x06004E28 RID: 20008 RVA: 0x00174539 File Offset: 0x00172739
	private void ActorLoadedCallback(string name, GameObject go, object callbackData)
	{
		this.m_fakeCards.Add(go.GetComponent<Actor>());
	}

	// Token: 0x06004E29 RID: 20009 RVA: 0x0017454C File Offset: 0x0017274C
	private void Start()
	{
		base.StartCoroutine(this.NotifySceneLoadedWhenReady());
	}

	// Token: 0x06004E2A RID: 20010 RVA: 0x0017455C File Offset: 0x0017275C
	private IEnumerator NotifySceneLoadedWhenReady()
	{
		while (this.m_fakeCards.Count < 2)
		{
			yield return null;
		}
		while (!this.m_creditsTextLoaded)
		{
			yield return null;
		}
		Box.Get().AddTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxOpened));
		SceneMgr.Get().NotifySceneLoaded();
		yield break;
	}

	// Token: 0x06004E2B RID: 20011 RVA: 0x00174578 File Offset: 0x00172778
	private void OnBoxOpened(object userData)
	{
		Box.Get().RemoveTransitionFinishedListener(new Box.TransitionFinishedCallback(this.OnBoxOpened));
		if (!this.m_creditsTextLoadSucceeded)
		{
			SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
			return;
		}
		MusicManager.Get().StartPlaylist(MusicPlaylistType.UI_Credits);
		base.StartCoroutine("StartCredits");
	}

	// Token: 0x06004E2C RID: 20012 RVA: 0x001745CC File Offset: 0x001727CC
	private IEnumerator StartCredits()
	{
		this.m_creditsText2.Text = this.GetNextCreditsChunk();
		this.m_currentText = this.m_creditsText2;
		this.FlopCredits();
		this.started = true;
		this.m_creditsRoot.SetActive(true);
		yield return new WaitForSeconds(4f);
		while (this.m_creditsDefs.Count > 0)
		{
			this.NewCard();
			yield return new WaitForSeconds(11f);
		}
		yield break;
	}

	// Token: 0x06004E2D RID: 20013 RVA: 0x001745E7 File Offset: 0x001727E7
	private void NewCard()
	{
		base.StartCoroutine("ShowNewCard");
	}

	// Token: 0x06004E2E RID: 20014 RVA: 0x001745F8 File Offset: 0x001727F8
	private IEnumerator ShowNewCard()
	{
		float CARD_MOVE_TIME = 1f;
		int index = 0;
		if (this.m_lastCard == 0)
		{
			index = 1;
		}
		this.m_lastCard = index;
		this.m_shownCreditsCard = this.m_fakeCards[index];
		int newDefIndex = Random.Range(0, this.m_creditsDefs.Count);
		this.m_shownCreditsCard.SetCardDef(this.m_creditsDefs[newDefIndex].GetCardDef());
		EntityDef entityDef = this.m_creditsDefs[newDefIndex].GetEntityDef();
		bool isSchweitz = entityDef.GetCardId() == "CRED_10";
		if (isSchweitz)
		{
			entityDef.SetTag<TAG_RACE>(GAME_TAG.CARDRACE, TAG_RACE.PIRATE);
		}
		this.m_shownCreditsCard.SetEntityDef(entityDef);
		this.m_creditsDefs.RemoveAt(newDefIndex);
		this.m_shownCreditsCard.UpdateAllComponents();
		this.m_shownCreditsCard.Show();
		if (isSchweitz)
		{
			this.m_shownCreditsCard.GetRaceText().Text = GameStrings.Get("GLUE_NINJA");
		}
		this.m_shownCreditsCard.transform.position = this.m_offscreenCardBone.position;
		this.m_shownCreditsCard.transform.localScale = this.m_offscreenCardBone.localScale;
		this.m_shownCreditsCard.transform.localEulerAngles = this.m_offscreenCardBone.localEulerAngles;
		SoundManager.Get().LoadAndPlay("credits_card_enter_" + Random.Range(1, 3).ToString());
		iTween.MoveTo(this.m_shownCreditsCard.gameObject, this.m_cardBone.position, CARD_MOVE_TIME);
		iTween.RotateTo(this.m_shownCreditsCard.gameObject, this.m_cardBone.localEulerAngles, CARD_MOVE_TIME);
		Actor oldActor = this.m_shownCreditsCard;
		yield return new WaitForSeconds(0.5f);
		SoundManager.Get().LoadAndPlay("tavern_crowd_play_reaction_positive_" + Random.Range(1, 5).ToString());
		yield return new WaitForSeconds(7.5f);
		this.m_shownCreditsCard.ActivateSpell(SpellType.BURN);
		SoundManager.Get().LoadAndPlay("credits_card_embers_" + Random.Range(1, 3).ToString());
		if (this.m_shownCreditsCard == oldActor)
		{
			this.m_shownCreditsCard = null;
		}
		yield break;
	}

	// Token: 0x06004E2F RID: 20015 RVA: 0x00174614 File Offset: 0x00172814
	private void Update()
	{
		Network.Get().ProcessNetwork();
		if (!this.started)
		{
			return;
		}
		this.m_creditsRoot.transform.localPosition += new Vector3(0f, 0f, 2.5f * Time.deltaTime);
		if (this.m_creditsDone)
		{
			return;
		}
		if (this.m_currentText == null)
		{
			return;
		}
		if (this.GetTopOfCurrentCredits() > this.m_flopPoint.position.z)
		{
			this.FlopCredits();
		}
	}

	// Token: 0x06004E30 RID: 20016 RVA: 0x001746B0 File Offset: 0x001728B0
	private float GetTopOfCurrentCredits()
	{
		Bounds textWorldSpaceBounds = this.m_currentText.GetTextWorldSpaceBounds();
		return textWorldSpaceBounds.center.z + textWorldSpaceBounds.extents.z;
	}

	// Token: 0x06004E31 RID: 20017 RVA: 0x001746E8 File Offset: 0x001728E8
	private void OnFullDefLoaded(string cardID, FullDef def, object userData)
	{
		this.m_creditsDefs.Add(def);
	}

	// Token: 0x06004E32 RID: 20018 RVA: 0x001746F6 File Offset: 0x001728F6
	private void OnDonePressed(UIEvent e)
	{
		if (UniversalInputManager.UsePhoneUI)
		{
			Box.Get().m_letterboxingContainer.SetActive(true);
		}
		Navigation.GoBack();
	}

	// Token: 0x06004E33 RID: 20019 RVA: 0x00174720 File Offset: 0x00172920
	private void OnYearPressed(UIEvent e)
	{
		base.StopCoroutine("StartCredits");
		base.StopCoroutine("ShowNewCard");
		if (this.m_shownCreditsCard != null)
		{
			this.m_shownCreditsCard.ActivateSpell(SpellType.BURN);
			SoundManager.Get().LoadAndPlay("credits_card_embers_" + Random.Range(1, 3).ToString());
			this.m_shownCreditsCard = null;
		}
		this.m_displayingLatestYear = !this.m_displayingLatestYear;
		base.StartCoroutine(this.ResetCredits());
	}

	// Token: 0x06004E34 RID: 20020 RVA: 0x001747A8 File Offset: 0x001729A8
	private IEnumerator ResetCredits()
	{
		this.m_currentText = null;
		this.m_creditsText1.Text = string.Empty;
		this.m_creditsText2.Text = string.Empty;
		this.started = false;
		this.m_creditsTextLoaded = false;
		this.m_creditsTextLoadSucceeded = false;
		this.m_currentLine = 0;
		this.m_creditLines = null;
		this.m_yearButton.SetText((!this.m_displayingLatestYear) ? "2015" : "2014");
		this.m_creditsText1.transform.localPosition = this.creditsText1StartLocalPosition;
		this.m_creditsText2.transform.localPosition = this.creditsText2StartLocalPosition;
		this.m_creditsRoot.transform.localPosition = this.creditsRootStartLocalPosition;
		this.m_lastCard = 1;
		this.m_creditsDefs = new List<FullDef>();
		this.LoadAllCreditsCards();
		this.LoadCreditsText();
		while (!this.m_creditsTextLoaded)
		{
			yield return null;
		}
		base.StartCoroutine("StartCredits");
		yield break;
	}

	// Token: 0x06004E35 RID: 20021 RVA: 0x001747C4 File Offset: 0x001729C4
	private bool EndCredits()
	{
		iTween.FadeTo(this.m_creditsText1.gameObject, 0f, 0.1f);
		iTween.FadeTo(this.m_creditsText2.gameObject, 0f, 0.1f);
		SceneMgr.Get().SetNextMode(SceneMgr.Mode.HUB);
		return true;
	}

	// Token: 0x06004E36 RID: 20022 RVA: 0x00174811 File Offset: 0x00172A11
	private void StartEndCreditsTimer()
	{
		base.StartCoroutine(this.EndCreditsTimer());
	}

	// Token: 0x06004E37 RID: 20023 RVA: 0x00174820 File Offset: 0x00172A20
	private IEnumerator EndCreditsTimer()
	{
		yield return new WaitForSeconds(300f);
		if (SceneMgr.Get().IsModeRequested(SceneMgr.Mode.HUB) || SceneMgr.Get().GetMode() != SceneMgr.Mode.CREDITS)
		{
			yield break;
		}
		Navigation.GoBack();
		yield break;
	}

	// Token: 0x04003522 RID: 13602
	private const float CREDITS_SCROLL_SPEED = 2.5f;

	// Token: 0x04003523 RID: 13603
	private const int MAX_LINES_PER_CHUNK = 70;

	// Token: 0x04003524 RID: 13604
	private const string START_CREDITS_COROUTINE = "StartCredits";

	// Token: 0x04003525 RID: 13605
	private const string SHOW_NEW_CARD_COROUTINE = "ShowNewCard";

	// Token: 0x04003526 RID: 13606
	public GameObject m_creditsRoot;

	// Token: 0x04003527 RID: 13607
	public UberText m_creditsText1;

	// Token: 0x04003528 RID: 13608
	public UberText m_creditsText2;

	// Token: 0x04003529 RID: 13609
	private UberText m_currentText;

	// Token: 0x0400352A RID: 13610
	public Transform m_offscreenCardBone;

	// Token: 0x0400352B RID: 13611
	public Transform m_cardBone;

	// Token: 0x0400352C RID: 13612
	public UIBButton m_doneButton;

	// Token: 0x0400352D RID: 13613
	public UIBButton m_yearButton;

	// Token: 0x0400352E RID: 13614
	public Transform m_flopPoint;

	// Token: 0x0400352F RID: 13615
	public GameObject m_doneArrowInButton;

	// Token: 0x04003530 RID: 13616
	private static CreditsDisplay s_instance;

	// Token: 0x04003531 RID: 13617
	private string[] m_creditLines;

	// Token: 0x04003532 RID: 13618
	private int m_currentLine;

	// Token: 0x04003533 RID: 13619
	private List<Actor> m_fakeCards;

	// Token: 0x04003534 RID: 13620
	private List<FullDef> m_creditsDefs;

	// Token: 0x04003535 RID: 13621
	private bool started;

	// Token: 0x04003536 RID: 13622
	private bool m_creditsTextLoaded;

	// Token: 0x04003537 RID: 13623
	private bool m_creditsTextLoadSucceeded;

	// Token: 0x04003538 RID: 13624
	private bool m_creditsDone;

	// Token: 0x04003539 RID: 13625
	private Actor m_shownCreditsCard;

	// Token: 0x0400353A RID: 13626
	private bool m_displayingLatestYear = true;

	// Token: 0x0400353B RID: 13627
	private Vector3 creditsRootStartLocalPosition;

	// Token: 0x0400353C RID: 13628
	private Vector3 creditsText1StartLocalPosition;

	// Token: 0x0400353D RID: 13629
	private Vector3 creditsText2StartLocalPosition;

	// Token: 0x0400353E RID: 13630
	private int m_lastCard = 1;
}
