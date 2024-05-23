using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E91 RID: 3729
public class SpawnToDeckSpell : SuperSpell
{
	// Token: 0x060070B7 RID: 28855 RVA: 0x00213380 File Offset: 0x00211580
	public override bool AddPowerTargets()
	{
		this.m_visualToTargetIndexMap.Clear();
		this.m_targetToMetaDataMap.Clear();
		this.m_targets.Clear();
		List<PowerTask> taskList = this.m_taskList.GetTaskList();
		for (int i = 0; i < taskList.Count; i++)
		{
			PowerTask task = taskList[i];
			Card targetCardFromPowerTask = this.GetTargetCardFromPowerTask(i, task);
			if (!(targetCardFromPowerTask == null))
			{
				this.AddTarget(targetCardFromPowerTask.gameObject);
			}
		}
		return this.m_targets.Count > 0;
	}

	// Token: 0x060070B8 RID: 28856 RVA: 0x00213414 File Offset: 0x00211614
	protected override Card GetTargetCardFromPowerTask(int index, PowerTask task)
	{
		Network.PowerHistory power = task.GetPower();
		if (power.Type != Network.PowerType.FULL_ENTITY)
		{
			return null;
		}
		Network.HistFullEntity histFullEntity = power as Network.HistFullEntity;
		Network.Entity entity = histFullEntity.Entity;
		Entity entity2 = GameState.Get().GetEntity(entity.ID);
		if (entity2 == null)
		{
			string text = string.Format("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", this, entity.ID);
			Debug.LogWarning(text);
			return null;
		}
		if (entity2.GetZone() != TAG_ZONE.DECK)
		{
			return null;
		}
		return entity2.GetCard();
	}

	// Token: 0x060070B9 RID: 28857 RVA: 0x00213490 File Offset: 0x00211690
	protected override void OnAction(SpellStateType prevStateType)
	{
		this.m_effectsPendingFinish++;
		base.OnAction(prevStateType);
		base.StartCoroutine(this.DoActionWithTiming());
	}

	// Token: 0x060070BA RID: 28858 RVA: 0x002134C0 File Offset: 0x002116C0
	private IEnumerator DoActionWithTiming()
	{
		List<Actor> actors = new List<Actor>(this.m_targets.Count);
		yield return base.StartCoroutine(this.LoadAssets(actors));
		yield return base.StartCoroutine(this.DoEffects(actors));
		yield break;
	}

	// Token: 0x060070BB RID: 28859 RVA: 0x002134DC File Offset: 0x002116DC
	private IEnumerator LoadAssets(List<Actor> actors)
	{
		bool loadingOverrideCardDef = false;
		if (!string.IsNullOrEmpty(this.m_OverrideCardId) && !this.m_overrideCardDef)
		{
			loadingOverrideCardDef = true;
			DefLoader.LoadDefCallback<CardDef> cardDefCallback = delegate(string cardId, CardDef def, object userData)
			{
				loadingOverrideCardDef = false;
				if (def == null)
				{
					Error.AddDevFatal("SpawnToDeckSpell.LoadAssets() - FAILED to load CardDef for {0}", new object[]
					{
						cardId
					});
					return;
				}
				this.m_overrideCardDef = def;
			};
			DefLoader.Get().LoadCardDef(this.m_OverrideCardId, cardDefCallback, null, null);
		}
		while (loadingOverrideCardDef)
		{
			yield return null;
		}
		int assetsLoading = this.m_targets.Count;
		AssetLoader.GameObjectCallback actorCallback = delegate(string name, GameObject go, object userData)
		{
			assetsLoading--;
			int num = (int)userData;
			if (go == null)
			{
				Error.AddDevFatal("SpawnToDeckSpell.LoadAssets() - FAILED to load actor {0} (targetIndex {1})", new object[]
				{
					name,
					num
				});
				return;
			}
			Actor component = go.GetComponent<Actor>();
			GameObject gameObject = this.m_targets[num];
			Card component2 = gameObject.GetComponent<Card>();
			Entity entity2 = component2.GetEntity();
			component.SetEntityDef(this.GetEntityDef(entity2));
			component.SetCardDef(this.GetCardDef(component2));
			component.SetPremium(this.GetPremium(entity2));
			component.SetCardBackSideOverride(new Player.Side?(entity2.GetControllerSide()));
			component.UpdateAllComponents();
			component.Hide();
			actors[num] = component;
		};
		for (int i = 0; i < this.m_targets.Count; i++)
		{
			GameObject targetObject = this.m_targets[i];
			Card card = targetObject.GetComponent<Card>();
			Entity entity = card.GetEntity();
			EntityDef entityDef = this.GetEntityDef(entity);
			TAG_PREMIUM premium = this.GetPremium(entity);
			actors.Add(null);
			string name2 = ActorNames.GetHandActor(entityDef, premium);
			AssetLoader.Get().LoadActor(name2, actorCallback, i, false);
		}
		while (assetsLoading > 0)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x060070BC RID: 28860 RVA: 0x00213508 File Offset: 0x00211708
	private IEnumerator DoEffects(List<Actor> actors)
	{
		base.StartCoroutine(this.AnimateSpread(actors));
		Actor livingActor = null;
		do
		{
			livingActor = actors.Find((Actor currActor) => currActor);
			if (livingActor)
			{
				yield return null;
			}
		}
		while (livingActor);
		this.m_effectsPendingFinish--;
		base.FinishIfPossible();
		yield break;
	}

	// Token: 0x060070BD RID: 28861 RVA: 0x00213534 File Offset: 0x00211734
	private IEnumerator AnimateSpread(List<Actor> actors)
	{
		if (this.m_SpreadType == SpawnToDeckSpell.SpreadType.SEQUENCE)
		{
			List<Vector3> revealPositions = new List<Vector3>();
			float startOffset = -0.5f * (float)(actors.Count - 1) * this.m_SequenceData.m_Spacing;
			for (int i = 0; i < actors.Count; i++)
			{
				float currOffset = (float)i * this.m_SequenceData.m_Spacing;
				Vector3 offset = new Vector3(startOffset + currOffset, 0f, 0f);
				Vector3 revealPos = this.ComputeRevealPosition(offset);
				revealPositions.Add(revealPos);
			}
			this.PreventDeckOverlap(actors, revealPositions);
			this.PreventHandOverlapPhone(actors, revealPositions);
			List<float> revealSecs = this.RandomizeRevealTimes(actors.Count, this.m_SequenceData.m_RevealTime, this.m_SequenceData.m_NextCardRevealTimeMin, this.m_SequenceData.m_NextCardRevealTimeMax);
			float maxRevealSec = Mathf.Max(revealSecs.ToArray());
			for (int j = 0; j < actors.Count; j++)
			{
				Vector3 revealPos2 = revealPositions[j];
				float revealSec = revealSecs[j];
				float currHoldSec = (float)(actors.Count - 1 - j) * this.m_SequenceData.m_NextCardHoldTime;
				float holdSec = this.m_SequenceData.m_HoldTime + currHoldSec;
				float waitSec = maxRevealSec + holdSec;
				base.StartCoroutine(this.AnimateActor(actors, j, revealSec, revealPos2, waitSec));
			}
		}
		else
		{
			for (int k = 0; k < actors.Count; k++)
			{
				Vector3 revealPos3 = this.ComputeRevealPosition(Vector3.zero);
				base.StartCoroutine(this.AnimateActor(actors, k, this.m_StackData.m_RevealTime, revealPos3, this.m_StackData.m_RevealTime));
				if (k < actors.Count - 1)
				{
					yield return new WaitForSeconds(this.m_StackData.m_StaggerTime);
				}
			}
		}
		yield break;
	}

	// Token: 0x060070BE RID: 28862 RVA: 0x00213560 File Offset: 0x00211760
	private Vector3 ComputeRevealPosition(Vector3 offset)
	{
		Vector3 vector = base.transform.position;
		float num = Random.Range(this.m_RevealYOffsetMin, this.m_RevealYOffsetMax);
		vector.y += num;
		Card sourceCard = base.GetSourceCard();
		Player.Side controllerSide = sourceCard.GetControllerSide();
		Player.Side side = controllerSide;
		if (side != Player.Side.FRIENDLY)
		{
			if (side == Player.Side.OPPOSING)
			{
				vector.z += this.m_RevealOpponentSideZOffset;
			}
		}
		else
		{
			vector.z += this.m_RevealFriendlySideZOffset;
		}
		vector += offset;
		return vector;
	}

	// Token: 0x060070BF RID: 28863 RVA: 0x002135FC File Offset: 0x002117FC
	private void PreventHandOverlapPhone(List<Actor> actors, List<Vector3> revealPositions)
	{
		if (!UniversalInputManager.UsePhoneUI)
		{
			return;
		}
		Entity powerTarget = base.GetPowerTarget();
		if (powerTarget != null)
		{
			if (powerTarget.GetControllerSide() == Player.Side.OPPOSING)
			{
				return;
			}
		}
		else
		{
			Card sourceCard = base.GetSourceCard();
			if (sourceCard != null && sourceCard.GetControllerSide() == Player.Side.OPPOSING)
			{
				return;
			}
		}
		for (int i = 0; i < revealPositions.Count; i++)
		{
			Vector3 vector = revealPositions[i];
			vector..ctor(vector.x, vector.y, vector.z + 1.5f);
			revealPositions[i] = vector;
		}
	}

	// Token: 0x060070C0 RID: 28864 RVA: 0x002136A4 File Offset: 0x002118A4
	private void PreventDeckOverlap(List<Actor> actors, List<Vector3> revealPositions)
	{
		float num = 0f;
		for (int i = 0; i < revealPositions.Count; i++)
		{
			GameObject gameObject = this.m_targets[i];
			Card component = gameObject.GetComponent<Card>();
			Entity entity = component.GetEntity();
			Player controller = entity.GetController();
			ZoneDeck deckZone = controller.GetDeckZone();
			float num2 = 0f;
			GameObject activeThickness = deckZone.GetActiveThickness();
			if (activeThickness)
			{
				num2 = activeThickness.GetComponent<Renderer>().bounds.extents.x;
			}
			Vector3 position = deckZone.transform.position;
			position.x -= num2;
			Vector3 vector = revealPositions[i];
			vector.x += actors[i].GetMeshRenderer().bounds.extents.x;
			Vector3 vector2 = Camera.main.WorldToScreenPoint(position);
			float num3 = Camera.main.WorldToScreenPoint(vector).x - vector2.x;
			if (num3 > num)
			{
				num = num3;
			}
		}
		if (num <= 0f)
		{
			return;
		}
		for (int j = 0; j < revealPositions.Count; j++)
		{
			GameObject gameObject2 = this.m_targets[j];
			Card component2 = gameObject2.GetComponent<Card>();
			Entity entity2 = component2.GetEntity();
			Player controller2 = entity2.GetController();
			ZoneDeck deckZone2 = controller2.GetDeckZone();
			float num4 = CameraUtils.ScreenToWorldDist(Camera.main, num, deckZone2.transform.position);
			Vector3 vector3 = revealPositions[j];
			vector3..ctor(vector3.x - num4, vector3.y, vector3.z);
			revealPositions[j] = vector3;
		}
	}

	// Token: 0x060070C1 RID: 28865 RVA: 0x00213874 File Offset: 0x00211A74
	private List<float> RandomizeRevealTimes(int count, float revealSec, float nextRevealSecMin, float nextRevealSecMax)
	{
		List<float> list = new List<float>(count);
		List<int> list2 = new List<int>(count);
		for (int i = 0; i < count; i++)
		{
			list.Add(0f);
			list2.Add(i);
		}
		float num = revealSec;
		for (int j = 0; j < count; j++)
		{
			int num2 = Random.Range(0, list2.Count);
			int num3 = list2[num2];
			list2.RemoveAt(num2);
			list[num3] = num;
			float num4 = Random.Range(nextRevealSecMin, nextRevealSecMax);
			num += num4;
		}
		return list;
	}

	// Token: 0x060070C2 RID: 28866 RVA: 0x00213904 File Offset: 0x00211B04
	private IEnumerator AnimateActor(List<Actor> actors, int index, float revealSec, Vector3 revealPos, float waitSec)
	{
		Actor actor = actors[index];
		GameObject targetObject = this.m_targets[index];
		Card card = targetObject.GetComponent<Card>();
		Entity entity = card.GetEntity();
		Player controller = entity.GetController();
		ZonePlay play = controller.GetBattlefieldZone();
		ZoneDeck deck = controller.GetDeckZone();
		actor.transform.localScale = new Vector3(this.m_RevealStartScale, this.m_RevealStartScale, this.m_RevealStartScale);
		actor.transform.rotation = base.transform.rotation;
		actor.transform.position = base.transform.position;
		actor.Show();
		Vector3 revealScale = Vector3.one;
		Vector3 revealAngles = play.transform.rotation.eulerAngles;
		iTween.MoveTo(actor.gameObject, iTween.Hash(new object[]
		{
			"position",
			revealPos,
			"time",
			revealSec,
			"easetype",
			iTween.EaseType.easeOutExpo
		}));
		iTween.RotateTo(actor.gameObject, iTween.Hash(new object[]
		{
			"rotation",
			revealAngles,
			"time",
			revealSec,
			"easetype",
			iTween.EaseType.easeOutExpo
		}));
		iTween.ScaleTo(actor.gameObject, iTween.Hash(new object[]
		{
			"scale",
			revealScale,
			"time",
			revealSec,
			"easetype",
			iTween.EaseType.easeOutExpo
		}));
		if (waitSec > 0f)
		{
			yield return new WaitForSeconds(waitSec);
		}
		EntityDef entityDef = this.GetEntityDef(entity);
		bool hiddenActor = entityDef.GetCardType() == TAG_CARDTYPE.INVALID;
		yield return base.StartCoroutine(card.AnimatePlayToDeck(actor.gameObject, deck, hiddenActor));
		actor.Destroy();
		yield break;
	}

	// Token: 0x060070C3 RID: 28867 RVA: 0x0021396C File Offset: 0x00211B6C
	private TAG_PREMIUM GetPremium(Entity entity)
	{
		Card sourceCard = base.GetSourceCard();
		Entity entity2 = sourceCard.GetEntity();
		TAG_PREMIUM premiumType = entity2.GetPremiumType();
		if (premiumType == TAG_PREMIUM.GOLDEN)
		{
			return TAG_PREMIUM.GOLDEN;
		}
		SpawnToDeckSpell.HandActorSource handActorSource = this.m_HandActorSource;
		if (handActorSource == SpawnToDeckSpell.HandActorSource.CHOSEN_TARGET)
		{
			Entity powerTarget = base.GetPowerTarget();
			return powerTarget.GetPremiumType();
		}
		if (handActorSource != SpawnToDeckSpell.HandActorSource.OVERRIDE)
		{
			return entity.GetPremiumType();
		}
		return premiumType;
	}

	// Token: 0x060070C4 RID: 28868 RVA: 0x002139CC File Offset: 0x00211BCC
	private EntityDef GetEntityDef(Entity entity)
	{
		SpawnToDeckSpell.HandActorSource handActorSource = this.m_HandActorSource;
		if (handActorSource == SpawnToDeckSpell.HandActorSource.CHOSEN_TARGET)
		{
			Entity powerTarget = base.GetPowerTarget();
			return powerTarget.GetEntityDef();
		}
		if (handActorSource != SpawnToDeckSpell.HandActorSource.OVERRIDE)
		{
			return entity.GetEntityDef();
		}
		return DefLoader.Get().GetEntityDef(this.m_OverrideCardId);
	}

	// Token: 0x060070C5 RID: 28869 RVA: 0x00213A18 File Offset: 0x00211C18
	private CardDef GetCardDef(Card card)
	{
		SpawnToDeckSpell.HandActorSource handActorSource = this.m_HandActorSource;
		if (handActorSource == SpawnToDeckSpell.HandActorSource.CHOSEN_TARGET)
		{
			Card powerTargetCard = base.GetPowerTargetCard();
			return powerTargetCard.GetCardDef();
		}
		if (handActorSource != SpawnToDeckSpell.HandActorSource.OVERRIDE)
		{
			return card.GetCardDef();
		}
		return this.m_overrideCardDef;
	}

	// Token: 0x04005A03 RID: 23043
	private const float PHONE_HAND_OFFSET = 1.5f;

	// Token: 0x04005A04 RID: 23044
	public SpawnToDeckSpell.HandActorSource m_HandActorSource;

	// Token: 0x04005A05 RID: 23045
	public string m_OverrideCardId;

	// Token: 0x04005A06 RID: 23046
	public float m_RevealStartScale = 0.1f;

	// Token: 0x04005A07 RID: 23047
	public float m_RevealYOffsetMin = 5f;

	// Token: 0x04005A08 RID: 23048
	public float m_RevealYOffsetMax = 5f;

	// Token: 0x04005A09 RID: 23049
	public float m_RevealFriendlySideZOffset;

	// Token: 0x04005A0A RID: 23050
	public float m_RevealOpponentSideZOffset;

	// Token: 0x04005A0B RID: 23051
	public SpawnToDeckSpell.SpreadType m_SpreadType;

	// Token: 0x04005A0C RID: 23052
	public SpawnToDeckSpell.StackData m_StackData = new SpawnToDeckSpell.StackData();

	// Token: 0x04005A0D RID: 23053
	public SpawnToDeckSpell.SequenceData m_SequenceData = new SpawnToDeckSpell.SequenceData();

	// Token: 0x04005A0E RID: 23054
	private CardDef m_overrideCardDef;

	// Token: 0x02000E92 RID: 3730
	public enum HandActorSource
	{
		// Token: 0x04005A10 RID: 23056
		CHOSEN_TARGET,
		// Token: 0x04005A11 RID: 23057
		OVERRIDE,
		// Token: 0x04005A12 RID: 23058
		SPELL_TARGET
	}

	// Token: 0x02000E93 RID: 3731
	public enum SpreadType
	{
		// Token: 0x04005A14 RID: 23060
		STACK,
		// Token: 0x04005A15 RID: 23061
		SEQUENCE
	}

	// Token: 0x02000E94 RID: 3732
	[Serializable]
	public class StackData
	{
		// Token: 0x04005A16 RID: 23062
		public float m_RevealTime = 1f;

		// Token: 0x04005A17 RID: 23063
		public float m_StaggerTime = 1.2f;
	}

	// Token: 0x02000E95 RID: 3733
	[Serializable]
	public class SequenceData
	{
		// Token: 0x04005A18 RID: 23064
		public float m_Spacing = 2.1f;

		// Token: 0x04005A19 RID: 23065
		public float m_RevealTime = 0.6f;

		// Token: 0x04005A1A RID: 23066
		public float m_NextCardRevealTimeMin = 0.1f;

		// Token: 0x04005A1B RID: 23067
		public float m_NextCardRevealTimeMax = 0.2f;

		// Token: 0x04005A1C RID: 23068
		public float m_HoldTime = 0.3f;

		// Token: 0x04005A1D RID: 23069
		public float m_NextCardHoldTime = 0.4f;
	}
}
