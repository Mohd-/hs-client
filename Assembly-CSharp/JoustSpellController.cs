using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020008CA RID: 2250
[CustomEditClass]
public class JoustSpellController : SpellController
{
	// Token: 0x060054D9 RID: 21721 RVA: 0x00196284 File Offset: 0x00194484
	protected override bool AddPowerSourceAndTargets(PowerTaskList taskList)
	{
		if (!this.HasSourceCard(taskList))
		{
			return false;
		}
		this.m_joustTaskIndex = -1;
		List<PowerTask> taskList2 = taskList.GetTaskList();
		for (int i = 0; i < taskList2.Count; i++)
		{
			PowerTask powerTask = taskList2[i];
			Network.PowerHistory power = powerTask.GetPower();
			Network.HistMetaData histMetaData = power as Network.HistMetaData;
			if (histMetaData != null)
			{
				if (histMetaData.MetaType == 3)
				{
					this.m_joustTaskIndex = i;
				}
			}
		}
		if (this.m_joustTaskIndex < 0)
		{
			return false;
		}
		Entity sourceEntity = taskList.GetSourceEntity();
		Card card = sourceEntity.GetCard();
		base.SetSource(card);
		return true;
	}

	// Token: 0x060054DA RID: 21722 RVA: 0x00196328 File Offset: 0x00194528
	protected override void OnProcessTaskList()
	{
		base.StartCoroutine(this.DoEffectWithTiming());
	}

	// Token: 0x060054DB RID: 21723 RVA: 0x00196338 File Offset: 0x00194538
	private IEnumerator DoEffectWithTiming()
	{
		yield return base.StartCoroutine(this.WaitForShowEntities());
		this.CreateJousters();
		yield return base.StartCoroutine(this.ShowJousters());
		yield return base.StartCoroutine(this.Joust());
		yield return base.StartCoroutine(this.HideJousters());
		this.DestroyJousters();
		base.OnProcessTaskList();
		yield break;
	}

	// Token: 0x060054DC RID: 21724 RVA: 0x00196354 File Offset: 0x00194554
	private IEnumerator WaitForShowEntities()
	{
		bool complete = false;
		PowerTaskList.CompleteCallback completeCallback = delegate(PowerTaskList taskList, int startIndex, int count, object userData)
		{
			complete = true;
		};
		this.m_taskList.DoTasks(0, this.m_joustTaskIndex, completeCallback);
		while (!complete)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x060054DD RID: 21725 RVA: 0x00196370 File Offset: 0x00194570
	private void CreateJousters()
	{
		PowerTask powerTask = this.m_taskList.GetTaskList()[this.m_joustTaskIndex];
		Network.HistMetaData metaData = (Network.HistMetaData)powerTask.GetPower();
		Player friendlySidePlayer = GameState.Get().GetFriendlySidePlayer();
		Player opposingSidePlayer = GameState.Get().GetOpposingSidePlayer();
		this.m_friendlyJouster = this.CreateJouster(friendlySidePlayer, metaData);
		this.m_opponentJouster = this.CreateJouster(opposingSidePlayer, metaData);
		this.DetermineWinner(metaData);
		this.DetermineSourceJouster();
	}

	// Token: 0x060054DE RID: 21726 RVA: 0x001963E0 File Offset: 0x001945E0
	private JoustSpellController.Jouster CreateJouster(Player player, Network.HistMetaData metaData)
	{
		Entity entity = null;
		foreach (int id in metaData.Info)
		{
			Entity entity2 = GameState.Get().GetEntity(id);
			if (entity2 != null)
			{
				Player controller = entity2.GetController();
				if (controller == player)
				{
					entity = entity2;
					break;
				}
			}
		}
		if (entity == null)
		{
			return null;
		}
		Card card = entity.GetCard();
		CardDef cardDef = card.GetCardDef();
		card.SetInputEnabled(false);
		GameObject gameObject = AssetLoader.Get().LoadActor("Card_Hidden", false, false);
		string handActor = ActorNames.GetHandActor(entity);
		GameObject gameObject2 = AssetLoader.Get().LoadActor(handActor, false, false);
		JoustSpellController.Jouster jouster = new JoustSpellController.Jouster();
		jouster.m_player = player;
		jouster.m_card = card;
		jouster.m_initialActor = gameObject.GetComponent<Actor>();
		jouster.m_revealedActor = gameObject2.GetComponent<Actor>();
		Action<Actor> action = delegate(Actor actor)
		{
			actor.SetEntity(entity);
			actor.SetCard(card);
			actor.SetCardDef(cardDef);
			actor.UpdateAllComponents();
			actor.Hide();
		};
		action.Invoke(jouster.m_initialActor);
		action.Invoke(jouster.m_revealedActor);
		return jouster;
	}

	// Token: 0x060054DF RID: 21727 RVA: 0x0019654C File Offset: 0x0019474C
	private void DetermineWinner(Network.HistMetaData metaData)
	{
		Card joustWinner = GameUtils.GetJoustWinner(metaData);
		if (!joustWinner)
		{
			return;
		}
		Player controller = joustWinner.GetController();
		if (controller.IsFriendlySide())
		{
			this.m_winningJouster = this.m_friendlyJouster;
		}
		else
		{
			this.m_winningJouster = this.m_opponentJouster;
		}
	}

	// Token: 0x060054E0 RID: 21728 RVA: 0x0019659C File Offset: 0x0019479C
	private void DetermineSourceJouster()
	{
		Card source = base.GetSource();
		Player controller = source.GetController();
		if (this.m_friendlyJouster != null && this.m_friendlyJouster.m_card.GetController() == controller)
		{
			this.m_sourceJouster = this.m_friendlyJouster;
		}
		else if (this.m_opponentJouster != null && this.m_opponentJouster.m_card.GetController() == controller)
		{
			this.m_sourceJouster = this.m_opponentJouster;
		}
	}

	// Token: 0x060054E1 RID: 21729 RVA: 0x00196618 File Offset: 0x00194818
	private IEnumerator ShowJousters()
	{
		if (!string.IsNullOrEmpty(this.m_DrawStingerPrefab))
		{
			string showSoundName = FileUtils.GameAssetPathToName(this.m_DrawStingerPrefab);
			SoundManager.Get().LoadAndPlay(showSoundName);
		}
		string friendlyBoneName = this.m_FriendlyBoneName;
		string opponentBoneName = this.m_OpponentBoneName;
		if (UniversalInputManager.UsePhoneUI)
		{
			friendlyBoneName += "_phone";
			opponentBoneName += "_phone";
		}
		Transform friendlyBone = Board.Get().FindBone(friendlyBoneName);
		Transform opponentBone = Board.Get().FindBone(opponentBoneName);
		Vector3 friendlyToOpponent = opponentBone.position - friendlyBone.position;
		Quaternion rotation = Quaternion.LookRotation(friendlyToOpponent);
		if (this.m_friendlyJouster != null)
		{
			Vector3 localScale = friendlyBone.localScale;
			Vector3 position = friendlyBone.position;
			float delaySec = this.GetRandomSec();
			float showSec = this.m_ShowTime + this.GetRandomSec();
			this.ShowJouster(this.m_friendlyJouster, localScale, rotation, position, delaySec, showSec);
		}
		else
		{
			this.PlayNoJousterSpell(GameState.Get().GetFriendlySidePlayer());
		}
		if (this.m_opponentJouster != null)
		{
			Vector3 localScale2 = opponentBone.localScale;
			Vector3 position2 = opponentBone.position;
			float delaySec2 = this.GetRandomSec();
			float showSec2 = this.m_ShowTime + this.GetRandomSec();
			this.ShowJouster(this.m_opponentJouster, localScale2, rotation, position2, delaySec2, showSec2);
		}
		else
		{
			this.PlayNoJousterSpell(GameState.Get().GetOpposingSidePlayer());
		}
		while (this.IsJousterBusy(this.m_friendlyJouster) || this.IsJousterBusy(this.m_opponentJouster))
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x060054E2 RID: 21730 RVA: 0x00196634 File Offset: 0x00194834
	private void ShowJouster(JoustSpellController.Jouster jouster, Vector3 localScale, Quaternion rotation, Vector3 position, float delaySec, float showSec)
	{
		jouster.m_effectsPendingFinish++;
		Card card = jouster.m_card;
		ZoneDeck deckZone = jouster.m_player.GetDeckZone();
		GameObject thicknessForLayout = deckZone.GetThicknessForLayout();
		jouster.m_deckIndex = deckZone.RemoveCard(card);
		deckZone.SetSuppressEmotes(true);
		deckZone.UpdateLayout();
		float num = 0.5f * showSec;
		Vector3 vector = thicknessForLayout.GetComponent<Renderer>().bounds.center + Card.IN_DECK_OFFSET;
		Vector3 vector2 = vector + Card.ABOVE_DECK_OFFSET;
		Vector3 eulerAngles = rotation.eulerAngles;
		Vector3[] array = new Vector3[]
		{
			vector,
			vector2,
			position
		};
		card.ShowCard();
		jouster.m_initialActor.Show();
		card.transform.position = vector;
		card.transform.rotation = Card.IN_DECK_HIDDEN_ROTATION;
		card.transform.localScale = Card.IN_DECK_SCALE;
		iTween.MoveTo(card.gameObject, iTween.Hash(new object[]
		{
			"path",
			array,
			"delay",
			delaySec,
			"time",
			showSec,
			"easetype",
			iTween.EaseType.easeInOutQuart
		}));
		iTween.RotateTo(card.gameObject, iTween.Hash(new object[]
		{
			"rotation",
			eulerAngles,
			"delay",
			delaySec + num,
			"time",
			num,
			"easetype",
			iTween.EaseType.easeInOutCubic
		}));
		iTween.ScaleTo(card.gameObject, iTween.Hash(new object[]
		{
			"scale",
			localScale,
			"delay",
			delaySec + num,
			"time",
			num,
			"easetype",
			iTween.EaseType.easeInOutQuint
		}));
		if (!string.IsNullOrEmpty(this.m_ShowSoundPrefab))
		{
			string soundName = FileUtils.GameAssetPathToName(this.m_ShowSoundPrefab);
			SoundManager.Get().LoadAndPlay(soundName);
		}
		Action<object> action = delegate(object tweenUserData)
		{
			jouster.m_effectsPendingFinish--;
			this.DriftJouster(jouster);
		};
		iTween.Timer(card.gameObject, iTween.Hash(new object[]
		{
			"delay",
			delaySec,
			"time",
			showSec,
			"oncomplete",
			action
		}));
	}

	// Token: 0x060054E3 RID: 21731 RVA: 0x00196908 File Offset: 0x00194B08
	private void PlayNoJousterSpell(Player player)
	{
		ZoneDeck deckZone = player.GetDeckZone();
		Spell spell2 = Object.Instantiate<Spell>(this.m_NoJousterSpellPrefab);
		spell2.SetPosition(deckZone.transform.position);
		spell2.AddStateFinishedCallback(delegate(Spell spell, SpellStateType prevStateType, object userData)
		{
			if (spell.GetActiveState() == SpellStateType.NONE)
			{
				Object.Destroy(spell.gameObject);
			}
		});
		spell2.Activate();
	}

	// Token: 0x060054E4 RID: 21732 RVA: 0x00196964 File Offset: 0x00194B64
	private void DriftJouster(JoustSpellController.Jouster jouster)
	{
		Card card = jouster.m_card;
		Vector3 position = card.transform.position;
		float z = jouster.m_initialActor.GetMeshRenderer().bounds.size.z;
		float num = 0.02f * z;
		Vector3 vector = GeneralUtils.RandomSign() * num * card.transform.up;
		Vector3 vector2 = -vector;
		Vector3 vector3 = GeneralUtils.RandomSign() * num * card.transform.right;
		Vector3 vector4 = -vector3;
		List<Vector3> list = new List<Vector3>();
		list.Add(position + vector + vector3);
		list.Add(position + vector2 + vector3);
		list.Add(position);
		list.Add(position + vector + vector4);
		list.Add(position + vector2 + vector4);
		list.Add(position);
		float num2 = this.m_DriftCycleTime + this.GetRandomSec();
		Hashtable args = iTween.Hash(new object[]
		{
			"path",
			list.ToArray(),
			"time",
			num2,
			"easetype",
			iTween.EaseType.linear,
			"looptype",
			iTween.LoopType.loop
		});
		iTween.MoveTo(card.gameObject, args);
	}

	// Token: 0x060054E5 RID: 21733 RVA: 0x00196AD4 File Offset: 0x00194CD4
	private IEnumerator Joust()
	{
		if (this.m_friendlyJouster != null)
		{
			float revealSec = this.m_RevealTime + this.GetRandomSec();
			this.RevealJouster(this.m_friendlyJouster, revealSec);
		}
		if (this.m_opponentJouster != null)
		{
			float revealSec2 = this.m_RevealTime + this.GetRandomSec();
			this.RevealJouster(this.m_opponentJouster, revealSec2);
		}
		if (this.m_sourceJouster != null)
		{
			while (this.IsJousterBusy(this.m_friendlyJouster) || this.IsJousterBusy(this.m_opponentJouster))
			{
				yield return null;
			}
			Spell resultSpellPrefab = (this.m_sourceJouster != this.m_winningJouster) ? this.m_LoserSpellPrefab : this.m_WinnerSpellPrefab;
			this.PlaySpellOnActor(this.m_sourceJouster, this.m_sourceJouster.m_revealedActor, resultSpellPrefab);
		}
		if (this.m_friendlyJouster != null || this.m_opponentJouster != null)
		{
			iTween.Timer(base.gameObject, iTween.Hash(new object[]
			{
				"time",
				this.m_HoldTime
			}));
		}
		while (this.IsJousterBusy(this.m_friendlyJouster) || this.IsJousterBusy(this.m_opponentJouster) || iTween.HasTween(base.gameObject))
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x060054E6 RID: 21734 RVA: 0x00196AF0 File Offset: 0x00194CF0
	private void RevealJouster(JoustSpellController.Jouster jouster, float revealSec)
	{
		jouster.m_effectsPendingFinish++;
		Card card = jouster.m_card;
		Actor hiddenActor = jouster.m_initialActor;
		Actor revealedActor = jouster.m_revealedActor;
		TransformUtil.SetEulerAngleZ(revealedActor.gameObject, -180f);
		iTween.RotateAdd(hiddenActor.gameObject, iTween.Hash(new object[]
		{
			"z",
			180f,
			"time",
			revealSec,
			"easetype",
			this.m_RevealEaseType
		}));
		iTween.RotateAdd(revealedActor.gameObject, iTween.Hash(new object[]
		{
			"z",
			180f,
			"time",
			revealSec,
			"easetype",
			this.m_RevealEaseType
		}));
		float startAngleZ = revealedActor.transform.rotation.eulerAngles.z;
		Action<object> action = delegate(object tweenUserData)
		{
			float z = revealedActor.transform.rotation.eulerAngles.z;
			float num = Mathf.DeltaAngle(startAngleZ, z);
			if (num >= 90f)
			{
				revealedActor.Show();
				hiddenActor.Hide();
			}
		};
		Action<object> action2 = delegate(object tweenUserData)
		{
			revealedActor.Show();
			hiddenActor.Hide();
			jouster.m_effectsPendingFinish--;
		};
		iTween.Timer(card.gameObject, iTween.Hash(new object[]
		{
			"time",
			revealSec,
			"onupdate",
			action,
			"oncomplete",
			action2
		}));
	}

	// Token: 0x060054E7 RID: 21735 RVA: 0x00196CA0 File Offset: 0x00194EA0
	private IEnumerator HideJousters()
	{
		if (!string.IsNullOrEmpty(this.m_HideStingerPrefab))
		{
			string hideSoundName = FileUtils.GameAssetPathToName(this.m_HideStingerPrefab);
			SoundManager.Get().LoadAndPlay(hideSoundName);
		}
		if (this.m_friendlyJouster != null)
		{
			float delaySec = this.GetRandomSec();
			float hideSec = this.m_HideTime + this.GetRandomSec();
			this.HideJouster(this.m_friendlyJouster, delaySec, hideSec);
		}
		if (this.m_opponentJouster != null)
		{
			float delaySec2 = this.GetRandomSec();
			float hideSec2 = this.m_HideTime + this.GetRandomSec();
			this.HideJouster(this.m_opponentJouster, delaySec2, hideSec2);
		}
		while (this.IsJousterBusy(this.m_friendlyJouster) || this.IsJousterBusy(this.m_opponentJouster))
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x060054E8 RID: 21736 RVA: 0x00196CBC File Offset: 0x00194EBC
	private void HideJouster(JoustSpellController.Jouster jouster, float delaySec, float hideSec)
	{
		jouster.m_effectsPendingFinish++;
		Card card = jouster.m_card;
		ZoneDeck deck = jouster.m_player.GetDeckZone();
		GameObject thicknessForLayout = deck.GetThicknessForLayout();
		Vector3 center = thicknessForLayout.GetComponent<Renderer>().bounds.center;
		float num = 0.5f * hideSec;
		Vector3 position = card.transform.position;
		Vector3 vector = center + Card.ABOVE_DECK_OFFSET;
		Vector3 vector2 = center + Card.IN_DECK_OFFSET;
		Vector3 in_DECK_ANGLES = Card.IN_DECK_ANGLES;
		Vector3 in_DECK_SCALE = Card.IN_DECK_SCALE;
		Vector3[] array = new Vector3[]
		{
			position,
			vector,
			vector2
		};
		iTween.MoveTo(card.gameObject, iTween.Hash(new object[]
		{
			"path",
			array,
			"delay",
			delaySec,
			"time",
			hideSec,
			"easetype",
			iTween.EaseType.easeInOutQuad
		}));
		iTween.RotateTo(card.gameObject, iTween.Hash(new object[]
		{
			"rotation",
			in_DECK_ANGLES,
			"delay",
			delaySec,
			"time",
			num,
			"easetype",
			iTween.EaseType.easeInOutCubic
		}));
		iTween.ScaleTo(card.gameObject, iTween.Hash(new object[]
		{
			"scale",
			in_DECK_SCALE,
			"delay",
			delaySec + num,
			"time",
			num,
			"easetype",
			iTween.EaseType.easeInOutQuint
		}));
		if (!string.IsNullOrEmpty(this.m_HideSoundPrefab))
		{
			string soundName = FileUtils.GameAssetPathToName(this.m_HideSoundPrefab);
			SoundManager.Get().LoadAndPlay(soundName);
		}
		Action<object> action = delegate(object userData)
		{
			jouster.m_effectsPendingFinish--;
			jouster.m_initialActor.GetCard().HideCard();
			deck.InsertCard(jouster.m_deckIndex, card);
			deck.UpdateLayout();
			deck.SetSuppressEmotes(false);
		};
		iTween.Timer(card.gameObject, iTween.Hash(new object[]
		{
			"delay",
			delaySec,
			"time",
			hideSec,
			"oncomplete",
			action
		}));
	}

	// Token: 0x060054E9 RID: 21737 RVA: 0x00196F54 File Offset: 0x00195154
	private void DestroyJousters()
	{
		if (this.m_friendlyJouster != null)
		{
			this.DestroyJouster(this.m_friendlyJouster);
			this.m_friendlyJouster = null;
		}
		if (this.m_opponentJouster != null)
		{
			this.DestroyJouster(this.m_opponentJouster);
			this.m_opponentJouster = null;
		}
	}

	// Token: 0x060054EA RID: 21738 RVA: 0x00196F9D File Offset: 0x0019519D
	private void DestroyJouster(JoustSpellController.Jouster jouster)
	{
		if (jouster == null)
		{
			return;
		}
		jouster.m_card.SetInputEnabled(true);
		jouster.m_initialActor.Destroy();
		jouster.m_revealedActor.Destroy();
	}

	// Token: 0x060054EB RID: 21739 RVA: 0x00196FC8 File Offset: 0x001951C8
	private float GetRandomSec()
	{
		return Random.Range(this.m_RandomSecMin, this.m_RandomSecMax);
	}

	// Token: 0x060054EC RID: 21740 RVA: 0x00196FDC File Offset: 0x001951DC
	private bool PlaySpellOnActor(JoustSpellController.Jouster jouster, Actor actor, Spell spellPrefab)
	{
		if (!spellPrefab)
		{
			return false;
		}
		jouster.m_effectsPendingFinish++;
		Card card = actor.GetCard();
		Spell spell2 = Object.Instantiate<Spell>(spellPrefab);
		spell2.transform.parent = actor.transform;
		spell2.AddFinishedCallback(delegate(Spell spell, object spellUserData)
		{
			jouster.m_effectsPendingFinish--;
		});
		spell2.AddStateFinishedCallback(delegate(Spell spell, SpellStateType prevStateType, object userData)
		{
			if (spell.GetActiveState() == SpellStateType.NONE)
			{
				Object.Destroy(spell.gameObject);
			}
		});
		spell2.SetSource(card.gameObject);
		spell2.Activate();
		return true;
	}

	// Token: 0x060054ED RID: 21741 RVA: 0x0019707D File Offset: 0x0019527D
	private bool IsJousterBusy(JoustSpellController.Jouster jouster)
	{
		return jouster != null && jouster.m_effectsPendingFinish > 0;
	}

	// Token: 0x04003B04 RID: 15108
	public Spell m_WinnerSpellPrefab;

	// Token: 0x04003B05 RID: 15109
	public Spell m_LoserSpellPrefab;

	// Token: 0x04003B06 RID: 15110
	public Spell m_NoJousterSpellPrefab;

	// Token: 0x04003B07 RID: 15111
	public float m_RandomSecMin = 0.1f;

	// Token: 0x04003B08 RID: 15112
	public float m_RandomSecMax = 0.25f;

	// Token: 0x04003B09 RID: 15113
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_ShowSoundPrefab;

	// Token: 0x04003B0A RID: 15114
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_DrawStingerPrefab;

	// Token: 0x04003B0B RID: 15115
	public float m_ShowTime = 1.2f;

	// Token: 0x04003B0C RID: 15116
	public float m_DriftCycleTime = 10f;

	// Token: 0x04003B0D RID: 15117
	public float m_RevealTime = 0.5f;

	// Token: 0x04003B0E RID: 15118
	public iTween.EaseType m_RevealEaseType = iTween.EaseType.easeOutBack;

	// Token: 0x04003B0F RID: 15119
	public float m_HoldTime = 1.2f;

	// Token: 0x04003B10 RID: 15120
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_HideSoundPrefab;

	// Token: 0x04003B11 RID: 15121
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_HideStingerPrefab;

	// Token: 0x04003B12 RID: 15122
	public float m_HideTime = 0.8f;

	// Token: 0x04003B13 RID: 15123
	public string m_FriendlyBoneName = "FriendlyJoust";

	// Token: 0x04003B14 RID: 15124
	public string m_OpponentBoneName = "OpponentJoust";

	// Token: 0x04003B15 RID: 15125
	private int m_joustTaskIndex;

	// Token: 0x04003B16 RID: 15126
	private JoustSpellController.Jouster m_friendlyJouster;

	// Token: 0x04003B17 RID: 15127
	private JoustSpellController.Jouster m_opponentJouster;

	// Token: 0x04003B18 RID: 15128
	private JoustSpellController.Jouster m_winningJouster;

	// Token: 0x04003B19 RID: 15129
	private JoustSpellController.Jouster m_sourceJouster;

	// Token: 0x02000967 RID: 2407
	private class Jouster
	{
		// Token: 0x04003ECB RID: 16075
		public Player m_player;

		// Token: 0x04003ECC RID: 16076
		public Card m_card;

		// Token: 0x04003ECD RID: 16077
		public int m_deckIndex;

		// Token: 0x04003ECE RID: 16078
		public Actor m_initialActor;

		// Token: 0x04003ECF RID: 16079
		public Actor m_revealedActor;

		// Token: 0x04003ED0 RID: 16080
		public int m_effectsPendingFinish;
	}
}
