using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008C9 RID: 2249
public class FatigueSpellController : SpellController
{
	// Token: 0x060054D1 RID: 21713 RVA: 0x00195D38 File Offset: 0x00193F38
	protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
	{
		if (!this.HasSourceCard(taskList))
		{
			return false;
		}
		this.m_fatigueTagChange = null;
		List<PowerTask> taskList2 = this.m_taskList.GetTaskList();
		for (int i = 0; i < taskList2.Count; i++)
		{
			PowerTask powerTask = taskList2[i];
			Network.PowerHistory power = powerTask.GetPower();
			if (power.Type == Network.PowerType.TAG_CHANGE)
			{
				Network.HistTagChange histTagChange = (Network.HistTagChange)power;
				if (histTagChange.Tag == 22)
				{
					this.m_fatigueTagChange = histTagChange;
				}
			}
		}
		if (this.m_fatigueTagChange == null)
		{
			return false;
		}
		Entity sourceEntity = taskList.GetSourceEntity();
		Card card = sourceEntity.GetCard();
		base.SetSource(card);
		return true;
	}

	// Token: 0x060054D2 RID: 21714 RVA: 0x00195DDD File Offset: 0x00193FDD
	protected override void OnProcessTaskList()
	{
		AssetLoader.Get().LoadActor("Card_Hand_Fatigue", new AssetLoader.GameObjectCallback(this.OnFatigueActorLoaded), null, false);
	}

	// Token: 0x060054D3 RID: 21715 RVA: 0x00195E00 File Offset: 0x00194000
	private void OnFatigueActorLoaded(string actorName, GameObject actorObject, object callbackData)
	{
		if (actorObject == null)
		{
			Debug.LogWarning(string.Format("FatigueSpellController.OnFatigueActorLoaded() - FAILED to load actor \"{0}\"", actorName));
			this.DoFinishFatigue();
			return;
		}
		Actor component = actorObject.GetComponent<Actor>();
		if (component == null)
		{
			Debug.LogWarning(string.Format("FatigueSpellController.OnFatigueActorLoaded() - ERROR actor \"{0}\" has no Actor component", actorName));
			this.DoFinishFatigue();
			return;
		}
		Player.Side controllerSide = base.GetSource().GetControllerSide();
		bool flag = controllerSide == Player.Side.FRIENDLY;
		this.m_fatigueActor = component;
		UberText nameText = this.m_fatigueActor.GetNameText();
		if (nameText != null)
		{
			nameText.Text = GameStrings.Get("GAMEPLAY_FATIGUE_TITLE");
		}
		UberText powersText = this.m_fatigueActor.GetPowersText();
		if (powersText != null)
		{
			powersText.Text = GameStrings.Format("GAMEPLAY_FATIGUE_TEXT", new object[]
			{
				this.m_fatigueTagChange.Value
			});
		}
		component.SetCardBackSideOverride(new Player.Side?(controllerSide));
		component.UpdateCardBack();
		ZoneDeck zoneDeck = (!flag) ? GameState.Get().GetOpposingSidePlayer().GetDeckZone() : GameState.Get().GetFriendlySidePlayer().GetDeckZone();
		zoneDeck.DoFatigueGlow();
		this.m_fatigueActor.transform.localEulerAngles = FatigueSpellController.FATIGUE_ACTOR_INITIAL_LOCAL_ROTATION;
		this.m_fatigueActor.transform.localScale = FatigueSpellController.FATIGUE_ACTOR_START_SCALE;
		this.m_fatigueActor.transform.position = zoneDeck.transform.position;
		Vector3[] array = new Vector3[]
		{
			this.m_fatigueActor.transform.position,
			new Vector3(this.m_fatigueActor.transform.position.x, this.m_fatigueActor.transform.position.y + 3.6f, this.m_fatigueActor.transform.position.z),
			Board.Get().FindBone("FatigueCardBone").position
		};
		iTween.MoveTo(this.m_fatigueActor.gameObject, iTween.Hash(new object[]
		{
			"path",
			array,
			"time",
			1.2f,
			"easetype",
			iTween.EaseType.easeInSineOutExpo
		}));
		iTween.RotateTo(this.m_fatigueActor.gameObject, iTween.Hash(new object[]
		{
			"rotation",
			FatigueSpellController.FATIGUE_ACTOR_FINAL_LOCAL_ROTATION,
			"time",
			1.2f,
			"delay",
			0.15f
		}));
		iTween.ScaleTo(this.m_fatigueActor.gameObject, FatigueSpellController.FATIGUE_ACTOR_FINAL_SCALE, 1f);
		base.StartCoroutine(this.WaitThenFinishFatigue(0.8f));
	}

	// Token: 0x060054D4 RID: 21716 RVA: 0x001960E8 File Offset: 0x001942E8
	private IEnumerator WaitThenFinishFatigue(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		this.DoFinishFatigue();
		yield break;
	}

	// Token: 0x060054D5 RID: 21717 RVA: 0x00196114 File Offset: 0x00194314
	private void DoFinishFatigue()
	{
		Spell spell = base.GetSource().GetActor().GetSpell(SpellType.FATIGUE_DEATH);
		spell.AddFinishedCallback(new Spell.FinishedCallback(this.OnFatigueDamageFinished));
		spell.ActivateState(SpellStateType.BIRTH);
	}

	// Token: 0x060054D6 RID: 21718 RVA: 0x00196150 File Offset: 0x00194350
	private void OnFatigueDamageFinished(Spell spell, object userData)
	{
		spell.RemoveFinishedCallback(new Spell.FinishedCallback(this.OnFatigueDamageFinished));
		if (this.m_fatigueActor == null)
		{
			this.OnFinishedTaskList();
			return;
		}
		Spell spell2 = this.m_fatigueActor.GetSpell(SpellType.DEATH);
		if (spell2 == null)
		{
			this.OnFinishedTaskList();
			return;
		}
		Actor fatigueActor = this.m_fatigueActor;
		this.m_fatigueActor = null;
		spell2.AddStateFinishedCallback(new Spell.StateFinishedCallback(this.OnFatigueDeathSpellFinished), fatigueActor);
		spell2.Activate();
		this.OnFinishedTaskList();
	}

	// Token: 0x060054D7 RID: 21719 RVA: 0x001961D8 File Offset: 0x001943D8
	private void OnFatigueDeathSpellFinished(Spell spell, SpellStateType prevStateType, object userData)
	{
		Actor actor = (Actor)userData;
		if (actor != null)
		{
			actor.Destroy();
		}
		this.OnFinished();
	}

	// Token: 0x04003AFB RID: 15099
	private const float FATIGUE_DRAW_ANIM_TIME = 1.2f;

	// Token: 0x04003AFC RID: 15100
	private const float FATIGUE_DRAW_SCALE_TIME = 1f;

	// Token: 0x04003AFD RID: 15101
	private const float FATIGUE_HOLD_TIME = 0.8f;

	// Token: 0x04003AFE RID: 15102
	private static readonly Vector3 FATIGUE_ACTOR_START_SCALE = new Vector3(0.88f, 0.88f, 0.88f);

	// Token: 0x04003AFF RID: 15103
	private static readonly Vector3 FATIGUE_ACTOR_FINAL_SCALE = Vector3.one;

	// Token: 0x04003B00 RID: 15104
	private static readonly Vector3 FATIGUE_ACTOR_INITIAL_LOCAL_ROTATION = new Vector3(270f, 270f, 0f);

	// Token: 0x04003B01 RID: 15105
	private static readonly Vector3 FATIGUE_ACTOR_FINAL_LOCAL_ROTATION = Vector3.zero;

	// Token: 0x04003B02 RID: 15106
	private Network.HistTagChange m_fatigueTagChange;

	// Token: 0x04003B03 RID: 15107
	private Actor m_fatigueActor;
}
