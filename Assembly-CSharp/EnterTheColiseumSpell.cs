using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000E62 RID: 3682
[CustomEditClass]
public class EnterTheColiseumSpell : Spell
{
	// Token: 0x06006FAC RID: 28588 RVA: 0x0020C46C File Offset: 0x0020A66C
	protected override void OnAction(SpellStateType prevStateType)
	{
		base.OnAction(prevStateType);
		this.m_survivorCards = this.FindSurvivors();
		base.StartCoroutine(this.PerformActions());
	}

	// Token: 0x06006FAD RID: 28589 RVA: 0x0020C49C File Offset: 0x0020A69C
	protected override Card GetTargetCardFromPowerTask(int index, PowerTask task)
	{
		Network.PowerHistory power = task.GetPower();
		if (power.Type != Network.PowerType.TAG_CHANGE)
		{
			return null;
		}
		Network.HistTagChange histTagChange = power as Network.HistTagChange;
		if (histTagChange.Tag != 360)
		{
			return null;
		}
		if (histTagChange.Value != 1)
		{
			return null;
		}
		Entity entity = GameState.Get().GetEntity(histTagChange.Entity);
		if (entity == null)
		{
			string text = string.Format("{0}.GetTargetCardFromPowerTask() - WARNING trying to target entity with id {1} but there is no entity with that id", this, histTagChange.Entity);
			Debug.LogWarning(text);
			return null;
		}
		return entity.GetCard();
	}

	// Token: 0x06006FAE RID: 28590 RVA: 0x0020C524 File Offset: 0x0020A724
	private IEnumerator PerformActions()
	{
		this.m_effectsPlaying = true;
		foreach (Card card in this.m_survivorCards)
		{
			card.SetDoNotSort(true);
			card.GetActor().SetUnlit();
			this.LiftCard(card);
			yield return new WaitForSeconds(this.m_LiftOffset);
		}
		FullScreenFXMgr.Get().Vignette(1f, this.m_LightingFadeTime, this.m_lightFadeEaseType, null);
		if (!string.IsNullOrEmpty(this.m_SpellStartSoundPrefab))
		{
			string showSoundName = FileUtils.GameAssetPathToName(this.m_SpellStartSoundPrefab);
			SoundManager.Get().LoadAndPlay(showSoundName);
		}
		this.PlayDustCloudSpell();
		yield return new WaitForSeconds(this.m_LiftTime);
		foreach (Card card2 in this.m_survivorCards)
		{
			this.PlaySurvivorSpell(card2);
		}
		yield return new WaitForSeconds(this.m_DestroyMinionDelay);
		this.OnSpellFinished();
		CameraShakeMgr.Shake(Camera.main, new Vector3(this.m_CameraShakeMagnitude, this.m_CameraShakeMagnitude, this.m_CameraShakeMagnitude), 0.75f);
		yield return new WaitForSeconds(this.m_LowerDelay);
		while (this.m_numSurvivorSpellsPlaying > 0)
		{
			yield return null;
		}
		foreach (Card card3 in this.m_survivorCards)
		{
			Zone cardZone = card3.GetZone();
			if (cardZone is ZonePlay)
			{
				ZonePlay zone = (ZonePlay)cardZone;
				this.LowerCard(card3.gameObject, zone.GetCardPosition(card3));
				yield return new WaitForSeconds(this.m_LowerOffset);
			}
		}
		FullScreenFXMgr.Get().StopVignette(this.m_LightingFadeTime, this.m_lightFadeEaseType, null);
		if (this.m_ImpactSpellPrefab != null)
		{
			foreach (Card card4 in this.m_survivorCards)
			{
				Spell spellInstance = Object.Instantiate<Spell>(this.m_ImpactSpellPrefab);
				spellInstance.transform.parent = card4.gameObject.transform;
				spellInstance.transform.localPosition = new Vector3(0f, 0f, 0f);
				spellInstance.AddStateFinishedCallback(delegate(Spell spell, SpellStateType prevStateType, object userData)
				{
					this.m_effectsPlaying = false;
					if (spell.GetActiveState() == SpellStateType.NONE)
					{
						Object.Destroy(spell.gameObject);
					}
				});
				spellInstance.Activate();
				yield return new WaitForSeconds(this.m_LowerOffset);
			}
		}
		yield return new WaitForSeconds(this.m_LowerTime);
		foreach (Card card5 in this.m_survivorCards)
		{
			card5.SetDoNotSort(false);
			card5.GetActor().SetLit();
		}
		List<ZonePlay> playZones = ZoneMgr.Get().FindZonesOfType<ZonePlay>();
		foreach (ZonePlay playZone in playZones)
		{
			playZone.UpdateLayout();
		}
		while (this.m_effectsPlaying)
		{
			yield return null;
		}
		this.OnStateFinished();
		yield break;
	}

	// Token: 0x06006FAF RID: 28591 RVA: 0x0020C540 File Offset: 0x0020A740
	private void LiftCard(Card card)
	{
		GameObject gameObject = card.gameObject;
		Vector3 position = gameObject.transform.position;
		Vector3 position2 = card.GetZone().gameObject.transform.position;
		Hashtable args = iTween.Hash(new object[]
		{
			"time",
			this.m_LiftTime,
			"position",
			new Vector3((!this.m_survivorsMeetInMiddle) ? position.x : position2.x, position.y + this.m_survivorLiftHeight, position.z),
			"onstart",
			delegate(object newVal)
			{
				SoundManager.Get().LoadAndPlay(this.m_RaiseSoundName);
			},
			"easetype",
			this.m_liftEaseType
		});
		iTween.MoveTo(gameObject, args);
	}

	// Token: 0x06006FB0 RID: 28592 RVA: 0x0020C614 File Offset: 0x0020A814
	private void LowerCard(GameObject target, Vector3 finalPosition)
	{
		Hashtable args = iTween.Hash(new object[]
		{
			"time",
			this.m_LowerTime,
			"position",
			finalPosition,
			"easetype",
			this.m_lowerEaseType
		});
		iTween.MoveTo(target, args);
	}

	// Token: 0x06006FB1 RID: 28593 RVA: 0x0020C674 File Offset: 0x0020A874
	private List<Card> FindSurvivors()
	{
		List<Card> list = new List<Card>();
		List<ZonePlay> list2 = ZoneMgr.Get().FindZonesOfType<ZonePlay>();
		foreach (ZonePlay zonePlay in list2)
		{
			List<Card> cards = zonePlay.GetCards();
			Card playCard;
			foreach (Card playCard2 in cards)
			{
				playCard = playCard2;
				GameObject gameObject = this.m_targets.Find((GameObject testObject) => playCard == testObject.GetComponent<Card>());
				if (gameObject == null)
				{
					list.Add(playCard);
				}
			}
		}
		return list;
	}

	// Token: 0x06006FB2 RID: 28594 RVA: 0x0020C760 File Offset: 0x0020A960
	private void PlaySurvivorSpell(Card card)
	{
		if (this.m_survivorSpellPrefab == null)
		{
			return;
		}
		this.m_numSurvivorSpellsPlaying++;
		Spell spell2 = Object.Instantiate<Spell>(this.m_survivorSpellPrefab);
		spell2.transform.parent = card.GetActor().transform;
		spell2.AddFinishedCallback(delegate(Spell spell, object spellUserData)
		{
			this.m_numSurvivorSpellsPlaying--;
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
	}

	// Token: 0x06006FB3 RID: 28595 RVA: 0x0020C7F8 File Offset: 0x0020A9F8
	private void PlayDustCloudSpell()
	{
		if (this.m_DustSpellPrefab == null)
		{
			return;
		}
		Spell spell2 = Object.Instantiate<Spell>(this.m_DustSpellPrefab);
		spell2.AddStateFinishedCallback(delegate(Spell spell, SpellStateType prevStateType, object userData)
		{
			if (spell.GetActiveState() == SpellStateType.NONE)
			{
				Object.Destroy(spell.gameObject);
			}
		});
		spell2.Activate();
	}

	// Token: 0x040058BD RID: 22717
	[CustomEditField(T = EditType.SOUND_PREFAB)]
	public string m_SpellStartSoundPrefab;

	// Token: 0x040058BE RID: 22718
	public float m_survivorLiftHeight = 2f;

	// Token: 0x040058BF RID: 22719
	public float m_LiftTime = 0.5f;

	// Token: 0x040058C0 RID: 22720
	public float m_LiftOffset = 0.1f;

	// Token: 0x040058C1 RID: 22721
	public float m_DestroyMinionDelay = 0.5f;

	// Token: 0x040058C2 RID: 22722
	public float m_LowerDelay = 1.5f;

	// Token: 0x040058C3 RID: 22723
	public float m_LowerOffset = 0.05f;

	// Token: 0x040058C4 RID: 22724
	public float m_LowerTime = 0.7f;

	// Token: 0x040058C5 RID: 22725
	public float m_LightingFadeTime = 0.5f;

	// Token: 0x040058C6 RID: 22726
	public float m_CameraShakeMagnitude = 0.075f;

	// Token: 0x040058C7 RID: 22727
	public iTween.EaseType m_liftEaseType = iTween.EaseType.easeInQuart;

	// Token: 0x040058C8 RID: 22728
	public iTween.EaseType m_lowerEaseType = iTween.EaseType.easeOutCubic;

	// Token: 0x040058C9 RID: 22729
	public iTween.EaseType m_lightFadeEaseType = iTween.EaseType.easeOutCubic;

	// Token: 0x040058CA RID: 22730
	public Spell m_survivorSpellPrefab;

	// Token: 0x040058CB RID: 22731
	public Spell m_DustSpellPrefab;

	// Token: 0x040058CC RID: 22732
	public bool m_survivorsMeetInMiddle = true;

	// Token: 0x040058CD RID: 22733
	public Spell m_ImpactSpellPrefab;

	// Token: 0x040058CE RID: 22734
	public string m_RaiseSoundName;

	// Token: 0x040058CF RID: 22735
	private List<Card> m_survivorCards;

	// Token: 0x040058D0 RID: 22736
	private bool m_effectsPlaying;

	// Token: 0x040058D1 RID: 22737
	private int m_numSurvivorSpellsPlaying;
}
