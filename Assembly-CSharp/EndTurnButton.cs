using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000616 RID: 1558
public class EndTurnButton : MonoBehaviour
{
	// Token: 0x0600435C RID: 17244 RVA: 0x001426B0 File Offset: 0x001408B0
	private void Awake()
	{
		EndTurnButton.s_instance = this;
		this.m_MyTurnText.Text = GameStrings.Get("GAMEPLAY_END_TURN");
		this.m_WaitingText.Text = string.Empty;
		base.GetComponent<Collider>().enabled = false;
	}

	// Token: 0x0600435D RID: 17245 RVA: 0x001426F4 File Offset: 0x001408F4
	private void OnDestroy()
	{
		EndTurnButton.s_instance = null;
	}

	// Token: 0x0600435E RID: 17246 RVA: 0x001426FC File Offset: 0x001408FC
	private void Start()
	{
		base.StartCoroutine(this.WaitAFrameAndThenChangeState());
	}

	// Token: 0x0600435F RID: 17247 RVA: 0x0014270B File Offset: 0x0014090B
	public static EndTurnButton Get()
	{
		return EndTurnButton.s_instance;
	}

	// Token: 0x06004360 RID: 17248 RVA: 0x00142712 File Offset: 0x00140912
	public GameObject GetButtonContainer()
	{
		return base.transform.FindChild("ButtonContainer").gameObject;
	}

	// Token: 0x06004361 RID: 17249 RVA: 0x0014272C File Offset: 0x0014092C
	public void PlayPushDownAnimation()
	{
		if (this.m_inputBlocked || this.IsInWaitingState() || this.m_pressed)
		{
			return;
		}
		this.m_pressed = true;
		this.GetButtonContainer().GetComponent<Animation>().Play("ENDTURN_PRESSED_DOWN");
		SoundManager.Get().LoadAndPlay("FX_EndTurn_Down");
	}

	// Token: 0x06004362 RID: 17250 RVA: 0x00142788 File Offset: 0x00140988
	public void PlayButtonUpAnimation()
	{
		if (this.m_inputBlocked || this.IsInWaitingState() || !this.m_pressed)
		{
			return;
		}
		this.m_pressed = false;
		this.GetButtonContainer().GetComponent<Animation>().Play("ENDTURN_PRESSED_UP");
		SoundManager.Get().LoadAndPlay("FX_EndTurn_Up");
	}

	// Token: 0x06004363 RID: 17251 RVA: 0x001427E4 File Offset: 0x001409E4
	public bool IsInWaitingState()
	{
		ActorStateType activeStateType = this.m_ActorStateMgr.GetActiveStateType();
		return activeStateType == ActorStateType.ENDTURN_WAITING || activeStateType == ActorStateType.ENDTURN_NMP_2_WAITING || activeStateType == ActorStateType.ENDTURN_WAITING_TIMER;
	}

	// Token: 0x06004364 RID: 17252 RVA: 0x0014281C File Offset: 0x00140A1C
	public bool IsInNMPState()
	{
		ActorStateType activeStateType = this.m_ActorStateMgr.GetActiveStateType();
		return activeStateType == ActorStateType.ENDTURN_NO_MORE_PLAYS;
	}

	// Token: 0x06004365 RID: 17253 RVA: 0x00142840 File Offset: 0x00140A40
	public bool IsInYouHavePlaysState()
	{
		ActorStateType activeStateType = this.m_ActorStateMgr.GetActiveStateType();
		return activeStateType == ActorStateType.ENDTURN_YOUR_TURN;
	}

	// Token: 0x06004366 RID: 17254 RVA: 0x00142864 File Offset: 0x00140A64
	public bool HasNoMorePlays()
	{
		Network.Options optionsPacket = GameState.Get().GetOptionsPacket();
		return optionsPacket != null && optionsPacket.List != null && optionsPacket.List.Count <= 1;
	}

	// Token: 0x06004367 RID: 17255 RVA: 0x001428A5 File Offset: 0x00140AA5
	public bool IsInputBlocked()
	{
		return this.m_inputBlocked;
	}

	// Token: 0x06004368 RID: 17256 RVA: 0x001428AD File Offset: 0x00140AAD
	public void HandleMouseOver()
	{
		this.m_mousedOver = true;
		if (this.m_inputBlocked)
		{
			return;
		}
		this.PutInMouseOverState();
	}

	// Token: 0x06004369 RID: 17257 RVA: 0x001428C8 File Offset: 0x00140AC8
	public void HandleMouseOut()
	{
		this.m_mousedOver = false;
		if (this.m_inputBlocked)
		{
			return;
		}
		if (this.m_pressed)
		{
			this.PlayButtonUpAnimation();
		}
		this.PutInMouseOffState();
	}

	// Token: 0x0600436A RID: 17258 RVA: 0x00142900 File Offset: 0x00140B00
	private void PutInMouseOverState()
	{
		if (this.IsInNMPState())
		{
			this.m_WhiteHighlight.SetActive(false);
			this.m_GreenHighlight.SetActive(true);
			Hashtable args = iTween.Hash(new object[]
			{
				"from",
				this.m_GreenHighlight.GetComponent<Renderer>().material.GetFloat("_Intensity"),
				"to",
				1.4f,
				"time",
				0.15f,
				"easetype",
				iTween.EaseType.linear,
				"onupdate",
				"OnUpdateIntensityValue",
				"onupdatetarget",
				base.gameObject,
				"name",
				"ENDTURN_INTENSITY"
			});
			iTween.StopByName(base.gameObject, "ENDTURN_INTENSITY");
			iTween.ValueTo(base.gameObject, args);
		}
		else if (this.IsInYouHavePlaysState())
		{
			this.m_WhiteHighlight.SetActive(true);
			this.m_GreenHighlight.SetActive(false);
		}
		else
		{
			this.m_WhiteHighlight.SetActive(false);
			this.m_GreenHighlight.SetActive(false);
		}
	}

	// Token: 0x0600436B RID: 17259 RVA: 0x00142A3C File Offset: 0x00140C3C
	private void PutInMouseOffState()
	{
		this.m_WhiteHighlight.SetActive(false);
		if (this.IsInNMPState())
		{
			this.m_GreenHighlight.SetActive(true);
			Hashtable args = iTween.Hash(new object[]
			{
				"from",
				this.m_GreenHighlight.GetComponent<Renderer>().material.GetFloat("_Intensity"),
				"to",
				1.1f,
				"time",
				0.15f,
				"easetype",
				iTween.EaseType.linear,
				"onupdate",
				"OnUpdateIntensityValue",
				"onupdatetarget",
				base.gameObject,
				"name",
				"ENDTURN_INTENSITY"
			});
			iTween.StopByName(base.gameObject, "ENDTURN_INTENSITY");
			iTween.ValueTo(base.gameObject, args);
		}
		else
		{
			this.m_GreenHighlight.SetActive(false);
		}
	}

	// Token: 0x0600436C RID: 17260 RVA: 0x00142B42 File Offset: 0x00140D42
	private void OnUpdateIntensityValue(float newValue)
	{
		this.m_GreenHighlight.GetComponent<Renderer>().material.SetFloat("_Intensity", newValue);
	}

	// Token: 0x0600436D RID: 17261 RVA: 0x00142B60 File Offset: 0x00140D60
	private IEnumerator WaitAFrameAndThenChangeState()
	{
		yield return null;
		if (GameState.Get().IsGameCreated())
		{
			this.HandleGameStart();
		}
		else
		{
			this.m_ActorStateMgr.ChangeState(ActorStateType.ENDTURN_WAITING);
			GameState.Get().RegisterCreateGameListener(new GameState.CreateGameCallback(this.OnCreateGame));
		}
		yield break;
	}

	// Token: 0x0600436E RID: 17262 RVA: 0x00142B7C File Offset: 0x00140D7C
	private void HandleGameStart()
	{
		this.UpdateState();
		GameState gameState = GameState.Get();
		if (gameState.IsPastBeginPhase() && gameState.IsFriendlySidePlayerTurn())
		{
			base.GetComponent<Collider>().enabled = true;
			GameState.Get().RegisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(this.OnOptionsReceived));
		}
	}

	// Token: 0x0600436F RID: 17263 RVA: 0x00142BD0 File Offset: 0x00140DD0
	private void SetButtonState(ActorStateType stateType)
	{
		if (this.m_ActorStateMgr == null)
		{
			Debug.Log("End Turn Button Actor State Manager is missing!");
			return;
		}
		if (this.m_ActorStateMgr.GetActiveStateType() == stateType)
		{
			return;
		}
		if (this.m_inputBlocked)
		{
			return;
		}
		this.m_ActorStateMgr.ChangeState(stateType);
		if (stateType == ActorStateType.ENDTURN_YOUR_TURN || stateType == ActorStateType.ENDTURN_WAITING_TIMER)
		{
			this.m_inputBlocked = true;
			base.StartCoroutine(this.WaitUntilAnimationIsCompleteAndThenUnblockInput());
		}
	}

	// Token: 0x06004370 RID: 17264 RVA: 0x00142C54 File Offset: 0x00140E54
	private IEnumerator WaitUntilAnimationIsCompleteAndThenUnblockInput()
	{
		yield return new WaitForSeconds(this.m_ActorStateMgr.GetMaximumAnimationTimeOfActiveStates());
		this.m_inputBlocked = false;
		if (this.HasNoMorePlays())
		{
			this.SetStateToNoMorePlays();
		}
		yield break;
	}

	// Token: 0x06004371 RID: 17265 RVA: 0x00142C70 File Offset: 0x00140E70
	private void UpdateState()
	{
		if (!GameState.Get().IsFriendlySidePlayerTurn())
		{
			this.SetStateToWaiting();
			return;
		}
		if (GameState.Get().IsMulliganManagerActive())
		{
			return;
		}
		if (GameState.Get().IsTurnStartManagerBlockingInput())
		{
			return;
		}
		if (GameState.Get().GetResponseMode() == GameState.ResponseMode.NONE)
		{
			return;
		}
		this.SetStateToYourTurn();
	}

	// Token: 0x06004372 RID: 17266 RVA: 0x00142CCC File Offset: 0x00140ECC
	private void SetStateToYourTurn()
	{
		if (this.m_ActorStateMgr == null)
		{
			return;
		}
		if (this.HasNoMorePlays())
		{
			this.SetStateToNoMorePlays();
			return;
		}
		this.SetButtonState(ActorStateType.ENDTURN_YOUR_TURN);
		if (this.m_mousedOver)
		{
			this.PutInMouseOverState();
		}
		else
		{
			this.PutInMouseOffState();
		}
	}

	// Token: 0x06004373 RID: 17267 RVA: 0x00142D24 File Offset: 0x00140F24
	private void SetStateToNoMorePlays()
	{
		if (this.m_ActorStateMgr == null)
		{
			return;
		}
		if (this.IsInWaitingState())
		{
			this.SetButtonState(ActorStateType.ENDTURN_YOUR_TURN);
		}
		else
		{
			this.SetButtonState(ActorStateType.ENDTURN_NO_MORE_PLAYS);
			if (this.m_mousedOver)
			{
				this.PutInMouseOverState();
			}
			else
			{
				this.PutInMouseOffState();
			}
		}
		if (!this.m_playedNmpSoundThisTurn)
		{
			this.m_playedNmpSoundThisTurn = true;
			base.StartCoroutine(this.PlayEndTurnSound());
		}
	}

	// Token: 0x06004374 RID: 17268 RVA: 0x00142DA0 File Offset: 0x00140FA0
	private void SetStateToWaiting()
	{
		if (this.m_ActorStateMgr == null)
		{
			return;
		}
		if (this.IsInWaitingState())
		{
			return;
		}
		if (GameState.Get().IsGameOver())
		{
			return;
		}
		if (this.IsInNMPState())
		{
			this.SetButtonState(ActorStateType.ENDTURN_NMP_2_WAITING);
		}
		else
		{
			this.SetButtonState(ActorStateType.ENDTURN_WAITING);
		}
		this.PutInMouseOffState();
	}

	// Token: 0x06004375 RID: 17269 RVA: 0x00142E04 File Offset: 0x00141004
	private IEnumerator PlayEndTurnSound()
	{
		yield return new WaitForSeconds(1.5f);
		if (this.IsInNMPState())
		{
			SoundManager.Get().LoadAndPlay("VO_JobsDone", base.gameObject);
		}
		yield break;
	}

	// Token: 0x06004376 RID: 17270 RVA: 0x00142E1F File Offset: 0x0014101F
	private void OnCreateGame(GameState.CreateGamePhase phase, object userData)
	{
		if (phase != GameState.CreateGamePhase.CREATED)
		{
			return;
		}
		GameState.Get().UnregisterCreateGameListener(new GameState.CreateGameCallback(this.OnCreateGame));
		this.HandleGameStart();
	}

	// Token: 0x06004377 RID: 17271 RVA: 0x00142E46 File Offset: 0x00141046
	public void OnMulliganEnded()
	{
		this.m_WaitingText.Text = GameStrings.Get("GAMEPLAY_ENEMY_TURN");
	}

	// Token: 0x06004378 RID: 17272 RVA: 0x00142E60 File Offset: 0x00141060
	public void OnTurnStartManagerFinished()
	{
		PegCursor.Get().SetMode(PegCursor.Mode.STOPWAITING);
		this.m_playedNmpSoundThisTurn = false;
		this.SetStateToYourTurn();
		base.GetComponent<Collider>().enabled = true;
		GameState.Get().RegisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(this.OnOptionsReceived));
	}

	// Token: 0x06004379 RID: 17273 RVA: 0x00142EA8 File Offset: 0x001410A8
	public void OnEndTurnRequested()
	{
		PegCursor.Get().SetMode(PegCursor.Mode.WAITING);
		this.SetStateToWaiting();
		base.GetComponent<Collider>().enabled = false;
		GameState.Get().UnregisterOptionsReceivedListener(new GameState.OptionsReceivedCallback(this.OnOptionsReceived));
	}

	// Token: 0x0600437A RID: 17274 RVA: 0x00142EE9 File Offset: 0x001410E9
	private void OnOptionsReceived(object userData)
	{
		this.UpdateState();
	}

	// Token: 0x0600437B RID: 17275 RVA: 0x00142EF1 File Offset: 0x001410F1
	public void OnTurnTimerStart()
	{
		if (this.m_inputBlocked)
		{
			return;
		}
		if (this.m_mousedOver)
		{
			return;
		}
	}

	// Token: 0x0600437C RID: 17276 RVA: 0x00142F0B File Offset: 0x0014110B
	public void OnTurnTimerEnded(bool isFriendlyPlayerTurnTimer)
	{
		if (!isFriendlyPlayerTurnTimer)
		{
			return;
		}
		this.SetButtonState(ActorStateType.ENDTURN_WAITING_TIMER);
	}

	// Token: 0x04002AAF RID: 10927
	public ActorStateMgr m_ActorStateMgr;

	// Token: 0x04002AB0 RID: 10928
	public UberText m_MyTurnText;

	// Token: 0x04002AB1 RID: 10929
	public UberText m_WaitingText;

	// Token: 0x04002AB2 RID: 10930
	public GameObject m_GreenHighlight;

	// Token: 0x04002AB3 RID: 10931
	public GameObject m_WhiteHighlight;

	// Token: 0x04002AB4 RID: 10932
	public GameObject m_EndTurnButtonMesh;

	// Token: 0x04002AB5 RID: 10933
	private static EndTurnButton s_instance;

	// Token: 0x04002AB6 RID: 10934
	private bool m_inputBlocked;

	// Token: 0x04002AB7 RID: 10935
	private bool m_pressed;

	// Token: 0x04002AB8 RID: 10936
	private bool m_playedNmpSoundThisTurn;

	// Token: 0x04002AB9 RID: 10937
	private bool m_mousedOver;
}
