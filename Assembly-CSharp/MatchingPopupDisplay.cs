using System;
using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using UnityEngine;

// Token: 0x020002E0 RID: 736
public class MatchingPopupDisplay : TransitionPopup
{
	// Token: 0x060026A7 RID: 9895 RVA: 0x000BC294 File Offset: 0x000BA494
	protected override void Awake()
	{
		base.Awake();
		this.SetupSpinnerText();
		this.UpdateTipOfTheDay();
		this.GenerateRandomSpinnerTexts();
		this.m_title.Text = GameStrings.Get("GLUE_MATCHMAKER_FINDING_OPPONENT");
		this.m_nameContainer.SetActive(false);
		this.m_title.gameObject.SetActive(false);
		this.m_tipOfTheDay.gameObject.SetActive(false);
		this.m_wildVines.SetActive(false);
		SoundManager.Get().Load("FindOpponent_mechanism_start");
	}

	// Token: 0x060026A8 RID: 9896 RVA: 0x000BC318 File Offset: 0x000BA518
	protected override void OnGameConnecting(FindGameEventData eventData)
	{
		base.OnGameConnecting(eventData);
		this.IncreaseTooltipProgress();
	}

	// Token: 0x060026A9 RID: 9897 RVA: 0x000BC327 File Offset: 0x000BA527
	protected override void OnGameEntered(FindGameEventData eventData)
	{
		this.EnableCancelButtonIfPossible();
	}

	// Token: 0x060026AA RID: 9898 RVA: 0x000BC330 File Offset: 0x000BA530
	protected override void OnGameDelayed(FindGameEventData eventData)
	{
		this.EnableCancelButtonIfPossible();
	}

	// Token: 0x060026AB RID: 9899 RVA: 0x000BC339 File Offset: 0x000BA539
	protected override void OnAnimateShowFinished()
	{
		base.OnAnimateShowFinished();
		this.EnableCancelButtonIfPossible();
	}

	// Token: 0x060026AC RID: 9900 RVA: 0x000BC348 File Offset: 0x000BA548
	private void SetupSpinnerText()
	{
		for (int i = 1; i <= 10; i++)
		{
			GameObject gameObject = SceneUtils.FindChild(base.gameObject, "NAME_" + i).gameObject;
			this.m_spinnerTexts.Add(gameObject);
		}
	}

	// Token: 0x060026AD RID: 9901 RVA: 0x000BC398 File Offset: 0x000BA598
	private void GenerateRandomSpinnerTexts()
	{
		int i = 1;
		List<string> list = new List<string>();
		for (;;)
		{
			string text = GameStrings.Get("GLUE_SPINNER_" + i);
			if (text == "GLUE_SPINNER_" + i)
			{
				break;
			}
			list.Add(text);
			i++;
		}
		UberText component = SceneUtils.FindChild(base.gameObject, "NAME_PerfectOpponent").gameObject.GetComponent<UberText>();
		component.Text = GameStrings.Get("GLUE_MATCHMAKER_PERFECT_OPPONENT");
		for (i = 0; i < 10; i++)
		{
			int num = Mathf.FloorToInt(Random.value * (float)list.Count);
			UberText component2 = this.m_spinnerTexts[i].GetComponent<UberText>();
			component2.Text = list[num];
			list.RemoveAt(num);
		}
	}

	// Token: 0x060026AE RID: 9902 RVA: 0x000BC470 File Offset: 0x000BA670
	private IEnumerator StopSpinnerDelay()
	{
		yield return new WaitForSeconds(3.5f);
		this.AnimateHide();
		yield break;
	}

	// Token: 0x060026AF RID: 9903 RVA: 0x000BC48C File Offset: 0x000BA68C
	private bool OnNavigateBack()
	{
		if (!this.m_cancelButton.gameObject.activeSelf)
		{
			return false;
		}
		base.GetComponent<PlayMakerFSM>().SendEvent("Cancel");
		base.FireMatchCanceledEvent();
		return true;
	}

	// Token: 0x060026B0 RID: 9904 RVA: 0x000BC4C7 File Offset: 0x000BA6C7
	protected override void OnCancelButtonReleased(UIEvent e)
	{
		base.OnCancelButtonReleased(e);
		Navigation.GoBack();
	}

	// Token: 0x060026B1 RID: 9905 RVA: 0x000BC4D8 File Offset: 0x000BA6D8
	private void UpdateTipOfTheDay()
	{
		this.m_gameMode = SceneMgr.Get().GetMode();
		if (this.m_gameMode == SceneMgr.Mode.TOURNAMENT)
		{
			this.m_tipOfTheDay.Text = GameStrings.GetTip(TipCategory.PLAY, Options.Get().GetInt(Option.TIP_PLAY_PROGRESS, 0), TipCategory.DEFAULT);
		}
		else if (this.m_gameMode == SceneMgr.Mode.DRAFT)
		{
			this.m_tipOfTheDay.Text = GameStrings.GetTip(TipCategory.FORGE, Options.Get().GetInt(Option.TIP_FORGE_PROGRESS, 0), TipCategory.DEFAULT);
		}
		else if (this.m_gameMode == SceneMgr.Mode.TAVERN_BRAWL)
		{
			this.m_tipOfTheDay.Text = GameStrings.GetRandomTip(TipCategory.TAVERNBRAWL);
		}
		else
		{
			this.m_tipOfTheDay.Text = GameStrings.GetRandomTip(TipCategory.DEFAULT);
		}
	}

	// Token: 0x060026B2 RID: 9906 RVA: 0x000BC58C File Offset: 0x000BA78C
	private void IncreaseTooltipProgress()
	{
		if (this.m_gameMode == SceneMgr.Mode.TOURNAMENT)
		{
			Options.Get().SetInt(Option.TIP_PLAY_PROGRESS, Options.Get().GetInt(Option.TIP_PLAY_PROGRESS, 0) + 1);
		}
		else if (this.m_gameMode == SceneMgr.Mode.DRAFT)
		{
			Options.Get().SetInt(Option.TIP_FORGE_PROGRESS, Options.Get().GetInt(Option.TIP_FORGE_PROGRESS, 0) + 1);
		}
	}

	// Token: 0x060026B3 RID: 9907 RVA: 0x000BC5F0 File Offset: 0x000BA7F0
	protected override void ShowPopup()
	{
		SoundManager.Get().LoadAndPlay("FindOpponent_mechanism_start");
		base.ShowPopup();
		PlayMakerFSM component = base.GetComponent<PlayMakerFSM>();
		FsmBool fsmBool = component.FsmVariables.FindFsmBool("PlaySpinningMusic");
		if (fsmBool != null)
		{
			fsmBool.Value = (this.m_gameMode != SceneMgr.Mode.TAVERN_BRAWL);
		}
		component.SendEvent("Birth");
		SceneUtils.EnableRenderers(this.m_nameContainer, false);
		this.m_title.gameObject.SetActive(true);
		this.m_tipOfTheDay.gameObject.SetActive(true);
		bool active = SceneMgr.Get().GetMode() == SceneMgr.Mode.TOURNAMENT && Options.Get().GetBool(Option.IN_WILD_MODE);
		this.m_wildVines.SetActive(active);
		Navigation.Push(new Navigation.NavigateBackHandler(this.OnNavigateBack));
	}

	// Token: 0x060026B4 RID: 9908 RVA: 0x000BC6BC File Offset: 0x000BA8BC
	protected override void OnGameplaySceneLoaded()
	{
		this.m_nameContainer.SetActive(true);
		base.GetComponent<PlayMakerFSM>().SendEvent("Death");
		base.StartCoroutine(this.StopSpinnerDelay());
		Navigation.Clear();
	}

	// Token: 0x060026B5 RID: 9909 RVA: 0x000BC6F7 File Offset: 0x000BA8F7
	protected override void OnGameError(FindGameEventData eventData)
	{
		Navigation.PopUnique(new Navigation.NavigateBackHandler(this.OnNavigateBack));
	}

	// Token: 0x060026B6 RID: 9910 RVA: 0x000BC70A File Offset: 0x000BA90A
	protected override void OnGameCanceled(FindGameEventData eventData)
	{
		Navigation.PopUnique(new Navigation.NavigateBackHandler(this.OnNavigateBack));
	}

	// Token: 0x0400170B RID: 5899
	private const int NUM_SPINNER_ENTRIES = 10;

	// Token: 0x0400170C RID: 5900
	public UberText m_tipOfTheDay;

	// Token: 0x0400170D RID: 5901
	public GameObject m_nameContainer;

	// Token: 0x0400170E RID: 5902
	public GameObject m_wildVines;

	// Token: 0x0400170F RID: 5903
	private List<GameObject> m_spinnerTexts = new List<GameObject>();

	// Token: 0x04001710 RID: 5904
	private SceneMgr.Mode m_gameMode;
}
