using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200085A RID: 2138
public class TurnTimer : MonoBehaviour
{
	// Token: 0x0600524D RID: 21069 RVA: 0x00189124 File Offset: 0x00187324
	private void Awake()
	{
		TurnTimer.s_instance = this;
		this.m_spell = base.GetComponent<Spell>();
		this.m_spell.AddStateStartedCallback(new Spell.StateStartedCallback(this.OnSpellStateStarted));
		if (GameState.Get() != null)
		{
			GameState.Get().RegisterCurrentPlayerChangedListener(new GameState.CurrentPlayerChangedCallback(this.OnCurrentPlayerChanged));
			GameState.Get().RegisterFriendlyTurnStartedListener(new GameState.FriendlyTurnStartedCallback(this.OnFriendlyTurnStarted), null);
			GameState.Get().RegisterTurnTimerUpdateListener(new GameState.TurnTimerUpdateCallback(this.OnTurnTimerUpdate));
			GameState.Get().RegisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver), null);
		}
	}

	// Token: 0x0600524E RID: 21070 RVA: 0x001891C2 File Offset: 0x001873C2
	private void OnDestroy()
	{
		TurnTimer.s_instance = null;
	}

	// Token: 0x0600524F RID: 21071 RVA: 0x001891CA File Offset: 0x001873CA
	public static TurnTimer Get()
	{
		return TurnTimer.s_instance;
	}

	// Token: 0x06005250 RID: 21072 RVA: 0x001891D1 File Offset: 0x001873D1
	public bool HasCountdownTimeout()
	{
		return this.m_countdownTimeoutSec > Mathf.Epsilon;
	}

	// Token: 0x06005251 RID: 21073 RVA: 0x001891E0 File Offset: 0x001873E0
	public void OnEndTurnRequested()
	{
		if (!this.HasCountdownTimeout())
		{
			return;
		}
		this.ChangeState(TurnTimerState.KILL);
	}

	// Token: 0x06005252 RID: 21074 RVA: 0x001891F5 File Offset: 0x001873F5
	private void ChangeState(TurnTimerState state)
	{
		this.ChangeSpellState(state);
	}

	// Token: 0x06005253 RID: 21075 RVA: 0x00189200 File Offset: 0x00187400
	private void ChangeStateImpl(TurnTimerState state)
	{
		if (state == TurnTimerState.START)
		{
			this.ChangeState_Start();
		}
		else if (state == TurnTimerState.COUNTDOWN)
		{
			this.ChangeState_Countdown();
		}
		else if (state == TurnTimerState.TIMEOUT)
		{
			this.ChangeState_Timeout();
		}
		else if (state == TurnTimerState.KILL)
		{
			this.ChangeState_Kill();
		}
	}

	// Token: 0x06005254 RID: 21076 RVA: 0x00189250 File Offset: 0x00187450
	private void ChangeState_Start()
	{
		this.m_state = TurnTimerState.START;
		if (GameState.Get() != null)
		{
			Card heroCard = GameState.Get().GetCurrentPlayer().GetHeroCard();
			if (heroCard != null)
			{
				heroCard.PlayEmote(EmoteType.TIMER);
			}
			this.m_currentTimerBelongsToFriendlySidePlayer = GameState.Get().IsFriendlySidePlayerTurn();
		}
	}

	// Token: 0x06005255 RID: 21077 RVA: 0x001892A3 File Offset: 0x001874A3
	private void ChangeState_Countdown()
	{
		this.m_state = TurnTimerState.COUNTDOWN;
		this.m_countdownTimeoutSec = this.ComputeCountdownRemainingSec();
		this.RestartCountdownAnims(this.m_countdownTimeoutSec);
	}

	// Token: 0x06005256 RID: 21078 RVA: 0x001892C4 File Offset: 0x001874C4
	private void ChangeState_Timeout()
	{
		this.m_state = TurnTimerState.TIMEOUT;
		this.m_countdownEndTimestamp = 0f;
		if (EndTurnButton.Get() != null)
		{
			EndTurnButton.Get().OnTurnTimerEnded(this.m_currentTimerBelongsToFriendlySidePlayer);
		}
		this.StopCountdownAnims();
		this.UpdateCountdownAnims(0f);
	}

	// Token: 0x06005257 RID: 21079 RVA: 0x00189318 File Offset: 0x00187518
	private void ChangeState_Kill()
	{
		this.m_state = TurnTimerState.KILL;
		this.m_countdownEndTimestamp = 0f;
		this.StopCountdownAnims();
		this.UpdateCountdownAnims(0f);
	}

	// Token: 0x06005258 RID: 21080 RVA: 0x0018934C File Offset: 0x0018754C
	private void ChangeSpellState(TurnTimerState timerState)
	{
		SpellStateType stateType = this.TranslateTimerStateToSpellState(timerState);
		this.m_spell.ActivateState(stateType);
		if (timerState == TurnTimerState.START)
		{
			base.StartCoroutine(this.TimerBirthAnimateMaterialValues());
		}
	}

	// Token: 0x06005259 RID: 21081 RVA: 0x00189384 File Offset: 0x00187584
	private IEnumerator TimerBirthAnimateMaterialValues()
	{
		float endTime = Time.timeSinceLevelLoad + 1f;
		while (Time.timeSinceLevelLoad < endTime)
		{
			this.OnUpdateFuseMatVal(this.m_FuseXamountAnimation);
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600525A RID: 21082 RVA: 0x001893A0 File Offset: 0x001875A0
	private void OnSpellStateStarted(Spell spell, SpellStateType prevStateType, object userData)
	{
		SpellStateType activeState = spell.GetActiveState();
		TurnTimerState state = this.TranslateSpellStateToTimerState(activeState);
		this.ChangeStateImpl(state);
	}

	// Token: 0x0600525B RID: 21083 RVA: 0x001893C3 File Offset: 0x001875C3
	private SpellStateType TranslateTimerStateToSpellState(TurnTimerState timerState)
	{
		if (timerState == TurnTimerState.START)
		{
			return SpellStateType.BIRTH;
		}
		if (timerState == TurnTimerState.COUNTDOWN)
		{
			return SpellStateType.IDLE;
		}
		if (timerState == TurnTimerState.TIMEOUT)
		{
			return SpellStateType.DEATH;
		}
		if (timerState == TurnTimerState.KILL)
		{
			return SpellStateType.CANCEL;
		}
		return SpellStateType.NONE;
	}

	// Token: 0x0600525C RID: 21084 RVA: 0x001893EA File Offset: 0x001875EA
	private TurnTimerState TranslateSpellStateToTimerState(SpellStateType spellState)
	{
		if (spellState == SpellStateType.BIRTH)
		{
			return TurnTimerState.START;
		}
		if (spellState == SpellStateType.IDLE)
		{
			return TurnTimerState.COUNTDOWN;
		}
		if (spellState == SpellStateType.DEATH)
		{
			return TurnTimerState.TIMEOUT;
		}
		if (spellState == SpellStateType.CANCEL)
		{
			return TurnTimerState.KILL;
		}
		return TurnTimerState.NONE;
	}

	// Token: 0x0600525D RID: 21085 RVA: 0x00189411 File Offset: 0x00187611
	private bool ShouldUpdateCountdownRemaining()
	{
		return this.m_state == TurnTimerState.COUNTDOWN;
	}

	// Token: 0x0600525E RID: 21086 RVA: 0x00189422 File Offset: 0x00187622
	private void StopCountdownAnims()
	{
		iTween.StopByName(this.m_SparksObject, this.GenerateMoveAnimName());
		iTween.StopByName(this.m_FuseWickObject, this.GenerateMatValAnimName());
	}

	// Token: 0x0600525F RID: 21087 RVA: 0x00189448 File Offset: 0x00187648
	private float UpdateCountdownAnims(float countdownRemainingSec)
	{
		float num = this.ComputeCountdownProgress(countdownRemainingSec);
		this.m_SparksObject.transform.position = Vector3.Lerp(this.m_SparksFinishBone.position, this.m_SparksStartBone.position, num);
		float num2 = Mathf.Lerp(this.m_FuseMatValFinish, this.m_FuseMatValStart, num);
		this.m_FuseWickObject.GetComponent<Renderer>().material.SetFloat(this.m_FuseMatValName, num2);
		this.m_FuseShadowObject.GetComponent<Renderer>().material.SetFloat(this.m_FuseMatValName, num2);
		return num2;
	}

	// Token: 0x06005260 RID: 21088 RVA: 0x001894D8 File Offset: 0x001876D8
	private void StartCountdownAnims(float startingMatVal, float countdownRemainingSec)
	{
		this.m_currentMoveAnimId += 1U;
		this.m_currentMatValAnimId += 1U;
		Hashtable args = iTween.Hash(new object[]
		{
			"name",
			this.GenerateMoveAnimName(),
			"time",
			countdownRemainingSec,
			"position",
			this.m_SparksFinishBone.position,
			"ignoretimescale",
			true,
			"easetype",
			iTween.EaseType.linear
		});
		iTween.MoveTo(this.m_SparksObject, args);
		Hashtable args2 = iTween.Hash(new object[]
		{
			"name",
			this.GenerateMatValAnimName(),
			"time",
			countdownRemainingSec,
			"from",
			startingMatVal,
			"to",
			this.m_FuseMatValFinish,
			"ignoretimescale",
			true,
			"easetype",
			iTween.EaseType.linear,
			"onupdate",
			"OnUpdateFuseMatVal",
			"onupdatetarget",
			base.gameObject
		});
		iTween.ValueTo(this.m_FuseWickObject, args2);
	}

	// Token: 0x06005261 RID: 21089 RVA: 0x00189628 File Offset: 0x00187828
	private string GenerateMoveAnimName()
	{
		return string.Format("SparksMove{0}", this.m_currentMoveAnimId);
	}

	// Token: 0x06005262 RID: 21090 RVA: 0x0018963F File Offset: 0x0018783F
	private string GenerateMatValAnimName()
	{
		return string.Format("FuseMatVal{0}", this.m_currentMatValAnimId);
	}

	// Token: 0x06005263 RID: 21091 RVA: 0x00189658 File Offset: 0x00187858
	private void OnUpdateFuseMatVal(float val)
	{
		this.m_FuseWickObject.GetComponent<Renderer>().material.SetFloat(this.m_FuseMatValName, val);
		this.m_FuseShadowObject.GetComponent<Renderer>().material.SetFloat(this.m_FuseMatValName, val);
	}

	// Token: 0x06005264 RID: 21092 RVA: 0x001896A0 File Offset: 0x001878A0
	private void RestartCountdownAnims(float countdownRemainingSec)
	{
		this.StopCountdownAnims();
		float startingMatVal = this.UpdateCountdownAnims(countdownRemainingSec);
		this.StartCountdownAnims(startingMatVal, countdownRemainingSec);
	}

	// Token: 0x06005265 RID: 21093 RVA: 0x001896C4 File Offset: 0x001878C4
	private void UpdateCountdownTimeout()
	{
		this.m_countdownTimeoutSec = 0f;
		if (GameState.Get() == null)
		{
			return;
		}
		Player currentPlayer = GameState.Get().GetCurrentPlayer();
		if (currentPlayer == null)
		{
			return;
		}
		if (!currentPlayer.HasTag(GAME_TAG.TIMEOUT))
		{
			return;
		}
		int tag = currentPlayer.GetTag(GAME_TAG.TIMEOUT);
		this.m_countdownTimeoutSec = (float)tag;
	}

	// Token: 0x06005266 RID: 21094 RVA: 0x00189718 File Offset: 0x00187918
	private float ComputeCountdownRemainingSec()
	{
		float num = this.m_countdownEndTimestamp - Time.realtimeSinceStartup;
		if (num < 0f)
		{
			return 0f;
		}
		return num;
	}

	// Token: 0x06005267 RID: 21095 RVA: 0x00189744 File Offset: 0x00187944
	private float ComputeCountdownProgress(float countdownRemainingSec)
	{
		if (countdownRemainingSec <= Mathf.Epsilon)
		{
			return 0f;
		}
		return countdownRemainingSec / this.m_countdownTimeoutSec;
	}

	// Token: 0x06005268 RID: 21096 RVA: 0x0018975F File Offset: 0x0018795F
	private void OnCurrentPlayerChanged(Player player, object userData)
	{
		if (this.m_state == TurnTimerState.COUNTDOWN || this.m_state == TurnTimerState.START)
		{
			this.ChangeState(TurnTimerState.KILL);
		}
		this.UpdateCountdownTimeout();
	}

	// Token: 0x06005269 RID: 21097 RVA: 0x00189786 File Offset: 0x00187986
	private void OnFriendlyTurnStarted(object userData)
	{
		if (!this.HasCountdownTimeout())
		{
			return;
		}
		if (this.m_waitingForTurnStartManagerFinish)
		{
			this.ChangeState(TurnTimerState.START);
		}
		this.m_waitingForTurnStartManagerFinish = false;
	}

	// Token: 0x0600526A RID: 21098 RVA: 0x001897B0 File Offset: 0x001879B0
	private void OnTurnTimerUpdate(TurnTimerUpdate update, object userData)
	{
		this.m_countdownEndTimestamp = update.GetEndTimestamp();
		if (!update.ShouldShow())
		{
			if (this.m_state == TurnTimerState.COUNTDOWN || this.m_state == TurnTimerState.START)
			{
				this.ChangeState(TurnTimerState.KILL);
			}
			return;
		}
		float secondsRemaining = update.GetSecondsRemaining();
		if (secondsRemaining <= Mathf.Epsilon)
		{
			this.OnTurnTimedOut();
		}
		else if (this.m_state == TurnTimerState.COUNTDOWN)
		{
			this.RestartCountdownAnims(secondsRemaining);
		}
		else if (GameState.Get().IsTurnStartManagerActive())
		{
			this.m_waitingForTurnStartManagerFinish = true;
		}
		else
		{
			this.ChangeState(TurnTimerState.START);
		}
	}

	// Token: 0x0600526B RID: 21099 RVA: 0x0018984B File Offset: 0x00187A4B
	private void OnTurnTimedOut()
	{
		if (!this.HasCountdownTimeout())
		{
			return;
		}
		this.ChangeState(TurnTimerState.TIMEOUT);
	}

	// Token: 0x0600526C RID: 21100 RVA: 0x00189860 File Offset: 0x00187A60
	private void OnGameOver(object userData)
	{
		if (this.m_state == TurnTimerState.COUNTDOWN || this.m_state == TurnTimerState.START)
		{
			this.ChangeState(TurnTimerState.KILL);
		}
	}

	// Token: 0x04003890 RID: 14480
	private const float BIRTH_ANIMATION_TIME = 1f;

	// Token: 0x04003891 RID: 14481
	public float m_DebugTimeout = 30f;

	// Token: 0x04003892 RID: 14482
	public float m_DebugTimeoutStart = 20f;

	// Token: 0x04003893 RID: 14483
	public GameObject m_SparksObject;

	// Token: 0x04003894 RID: 14484
	public Transform m_SparksStartBone;

	// Token: 0x04003895 RID: 14485
	public Transform m_SparksFinishBone;

	// Token: 0x04003896 RID: 14486
	public GameObject m_FuseWickObject;

	// Token: 0x04003897 RID: 14487
	public GameObject m_FuseShadowObject;

	// Token: 0x04003898 RID: 14488
	public string m_FuseMatValName = "_Xamount";

	// Token: 0x04003899 RID: 14489
	public float m_FuseMatValStart = 0.42f;

	// Token: 0x0400389A RID: 14490
	public float m_FuseMatValFinish = -1.5f;

	// Token: 0x0400389B RID: 14491
	public float m_FuseXamountAnimation = -1.5f;

	// Token: 0x0400389C RID: 14492
	private static TurnTimer s_instance;

	// Token: 0x0400389D RID: 14493
	private Spell m_spell;

	// Token: 0x0400389E RID: 14494
	private TurnTimerState m_state;

	// Token: 0x0400389F RID: 14495
	private float m_countdownTimeoutSec;

	// Token: 0x040038A0 RID: 14496
	private float m_countdownEndTimestamp;

	// Token: 0x040038A1 RID: 14497
	private uint m_currentMoveAnimId;

	// Token: 0x040038A2 RID: 14498
	private uint m_currentMatValAnimId;

	// Token: 0x040038A3 RID: 14499
	private bool m_currentTimerBelongsToFriendlySidePlayer;

	// Token: 0x040038A4 RID: 14500
	private bool m_waitingForTurnStartManagerFinish;
}
