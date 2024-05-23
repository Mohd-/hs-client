using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000AB3 RID: 2739
public class BrawlSpell : Spell
{
	// Token: 0x06005ED2 RID: 24274 RVA: 0x001C629C File Offset: 0x001C449C
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

	// Token: 0x06005ED3 RID: 24275 RVA: 0x001C6324 File Offset: 0x001C4524
	protected override void OnAction(SpellStateType prevStateType)
	{
		if (this.m_targets.Count > 0)
		{
			this.m_survivorCard = this.FindSurvivor();
			this.StartJumpIns();
		}
		else
		{
			this.OnSpellFinished();
			this.OnStateFinished();
		}
	}

	// Token: 0x06005ED4 RID: 24276 RVA: 0x001C6368 File Offset: 0x001C4568
	private Card FindSurvivor()
	{
		List<ZonePlay> list = ZoneMgr.Get().FindZonesOfType<ZonePlay>();
		foreach (ZonePlay zonePlay in list)
		{
			List<Card> cards = zonePlay.GetCards();
			Card playCard;
			foreach (Card playCard2 in cards)
			{
				playCard = playCard2;
				GameObject gameObject = this.m_targets.Find((GameObject testObject) => playCard == testObject.GetComponent<Card>());
				if (gameObject == null)
				{
					return playCard;
				}
			}
		}
		return null;
	}

	// Token: 0x06005ED5 RID: 24277 RVA: 0x001C6450 File Offset: 0x001C4650
	private void StartJumpIns()
	{
		this.m_jumpsPending = this.m_targets.Count + 1;
		List<Card> list = new List<Card>(this.m_jumpsPending);
		foreach (GameObject gameObject in this.m_targets)
		{
			Card component = gameObject.GetComponent<Card>();
			list.Add(component);
		}
		list.Add(this.m_survivorCard);
		float num = 0f;
		while (list.Count > 0)
		{
			int num2 = Random.Range(0, list.Count);
			Card card = list[num2];
			list.RemoveAt(num2);
			this.StartJumpIn(card, ref num);
		}
	}

	// Token: 0x06005ED6 RID: 24278 RVA: 0x001C6520 File Offset: 0x001C4720
	private void StartJumpIn(Card card, ref float startSec)
	{
		float num = Random.Range(this.m_MinJumpInDelay, this.m_MaxJumpInDelay);
		base.StartCoroutine(this.JumpIn(card, startSec + num));
		startSec += num;
	}

	// Token: 0x06005ED7 RID: 24279 RVA: 0x001C6558 File Offset: 0x001C4758
	private IEnumerator JumpIn(Card card, float delaySec)
	{
		yield return new WaitForSeconds(delaySec);
		Vector3[] path = new Vector3[]
		{
			card.transform.position,
			default(Vector3),
			base.transform.position
		};
		path[1] = 0.5f * (path[0] + path[2]);
		float jumpHeight = Random.Range(this.m_MinJumpHeight, this.m_MaxJumpHeight);
		Vector3[] array = path;
		int num = 1;
		array[num].y = array[num].y + jumpHeight;
		Hashtable argTable = iTween.Hash(new object[]
		{
			"path",
			path,
			"orienttopath",
			true,
			"time",
			this.m_JumpInDuration,
			"easetype",
			this.m_JumpInEaseType,
			"oncomplete",
			"OnJumpInComplete",
			"oncompletetarget",
			base.gameObject,
			"oncompleteparams",
			card
		});
		iTween.MoveTo(card.gameObject, argTable);
		if (this.m_JumpInSoundPrefab != null)
		{
			base.StartCoroutine(this.LoadAndPlaySound(this.m_JumpInSoundPrefab, this.m_JumpInSoundDelay));
		}
		yield break;
	}

	// Token: 0x06005ED8 RID: 24280 RVA: 0x001C6590 File Offset: 0x001C4790
	private void OnJumpInComplete(Card targetCard)
	{
		targetCard.HideCard();
		this.m_jumpsPending--;
		if (this.m_jumpsPending > 0)
		{
			return;
		}
		base.StartCoroutine(this.Hold());
	}

	// Token: 0x06005ED9 RID: 24281 RVA: 0x001C65CC File Offset: 0x001C47CC
	private IEnumerator Hold()
	{
		yield return new WaitForSeconds(this.m_HoldTime);
		this.StartJumpOuts();
		yield break;
	}

	// Token: 0x06005EDA RID: 24282 RVA: 0x001C65E8 File Offset: 0x001C47E8
	private void StartJumpOuts()
	{
		this.m_jumpsPending = this.m_targets.Count;
		List<int> list = new List<int>();
		List<int> list2 = new List<int>();
		float num = 0f;
		bool flag = true;
		for (int i = 0; i < this.m_targets.Count; i++)
		{
			GameObject gameObject = this.m_targets[i];
			Card component = gameObject.GetComponent<Card>();
			if (!(component == this.m_survivorCard))
			{
				GameObject freeBone;
				if (flag)
				{
					freeBone = this.GetFreeBone(this.m_LeftJumpOutBones, list);
					if (freeBone == null)
					{
						list.Clear();
						freeBone = this.GetFreeBone(this.m_LeftJumpOutBones, list);
					}
				}
				else
				{
					freeBone = this.GetFreeBone(this.m_RightJumpOutBones, list2);
					if (freeBone == null)
					{
						list2.Clear();
						freeBone = this.GetFreeBone(this.m_RightJumpOutBones, list2);
					}
				}
				float num2 = Random.Range(this.m_MinJumpOutDelay, this.m_MaxJumpOutDelay);
				base.StartCoroutine(this.JumpOut(component, num + num2, freeBone.transform.position));
				num += num2;
				flag = !flag;
			}
		}
	}

	// Token: 0x06005EDB RID: 24283 RVA: 0x001C6718 File Offset: 0x001C4918
	private GameObject GetFreeBone(List<GameObject> boneList, List<int> usedBoneIndexes)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < boneList.Count; i++)
		{
			if (!usedBoneIndexes.Contains(i))
			{
				list.Add(i);
			}
		}
		if (list.Count == 0)
		{
			return null;
		}
		int num = Random.Range(0, list.Count - 1);
		int num2 = list[num];
		usedBoneIndexes.Add(num2);
		return boneList[num2];
	}

	// Token: 0x06005EDC RID: 24284 RVA: 0x001C6788 File Offset: 0x001C4988
	private IEnumerator JumpOut(Card card, float delaySec, Vector3 destPos)
	{
		yield return new WaitForSeconds(delaySec);
		card.transform.rotation = Quaternion.identity;
		card.ShowCard();
		Vector3[] path = new Vector3[]
		{
			card.transform.position,
			default(Vector3),
			destPos
		};
		path[1] = 0.5f * (path[0] + path[2]);
		float jumpHeight = Random.Range(this.m_MinJumpHeight, this.m_MaxJumpHeight);
		Vector3[] array = path;
		int num = 1;
		array[num].y = array[num].y + jumpHeight;
		Hashtable argTable = iTween.Hash(new object[]
		{
			"path",
			path,
			"time",
			this.m_JumpOutDuration,
			"easetype",
			this.m_JumpOutEaseType,
			"oncomplete",
			"OnJumpOutComplete",
			"oncompletetarget",
			base.gameObject,
			"oncompleteparams",
			card
		});
		iTween.MoveTo(card.gameObject, argTable);
		if (this.m_JumpOutSoundPrefab != null)
		{
			base.StartCoroutine(this.LoadAndPlaySound(this.m_JumpOutSoundPrefab, this.m_JumpOutSoundDelay));
		}
		if (this.m_LandSoundPrefab != null)
		{
			base.StartCoroutine(this.LoadAndPlaySound(this.m_LandSoundPrefab, this.m_LandSoundDelay));
		}
		yield break;
	}

	// Token: 0x06005EDD RID: 24285 RVA: 0x001C67CD File Offset: 0x001C49CD
	private void OnJumpOutComplete(Card targetCard)
	{
		this.m_jumpsPending--;
		if (this.m_jumpsPending > 0)
		{
			return;
		}
		base.ActivateState(SpellStateType.DEATH);
		base.StartCoroutine(this.SurvivorHold());
	}

	// Token: 0x06005EDE RID: 24286 RVA: 0x001C6800 File Offset: 0x001C4A00
	private IEnumerator SurvivorHold()
	{
		this.m_survivorCard.transform.rotation = Quaternion.identity;
		this.m_survivorCard.ShowCard();
		yield return new WaitForSeconds(this.m_SurvivorHoldDuration);
		if (this.IsSurvivorAlone())
		{
			this.m_survivorCard.GetZone().UpdateLayout();
		}
		this.OnSpellFinished();
		this.OnStateFinished();
		yield break;
	}

	// Token: 0x06005EDF RID: 24287 RVA: 0x001C681C File Offset: 0x001C4A1C
	private bool IsSurvivorAlone()
	{
		Zone zone = this.m_survivorCard.GetZone();
		foreach (GameObject gameObject in this.m_targets)
		{
			Card component = gameObject.GetComponent<Card>();
			if (component.GetZone() == zone)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x06005EE0 RID: 24288 RVA: 0x001C68A0 File Offset: 0x001C4AA0
	private IEnumerator LoadAndPlaySound(AudioSource prefab, float delaySec)
	{
		AudioSource source = Object.Instantiate<AudioSource>(prefab);
		source.transform.parent = base.transform;
		TransformUtil.Identity(source);
		yield return new WaitForSeconds(delaySec);
		SoundManager.Get().PlayPreloaded(source);
		yield break;
	}

	// Token: 0x04004640 RID: 17984
	public float m_MinJumpHeight = 1.5f;

	// Token: 0x04004641 RID: 17985
	public float m_MaxJumpHeight = 2.5f;

	// Token: 0x04004642 RID: 17986
	public float m_MinJumpInDelay = 0.1f;

	// Token: 0x04004643 RID: 17987
	public float m_MaxJumpInDelay = 0.2f;

	// Token: 0x04004644 RID: 17988
	public float m_JumpInDuration = 1.5f;

	// Token: 0x04004645 RID: 17989
	public iTween.EaseType m_JumpInEaseType = iTween.EaseType.linear;

	// Token: 0x04004646 RID: 17990
	public float m_HoldTime = 0.1f;

	// Token: 0x04004647 RID: 17991
	public float m_MinJumpOutDelay = 0.1f;

	// Token: 0x04004648 RID: 17992
	public float m_MaxJumpOutDelay = 0.2f;

	// Token: 0x04004649 RID: 17993
	public float m_JumpOutDuration = 1.5f;

	// Token: 0x0400464A RID: 17994
	public iTween.EaseType m_JumpOutEaseType = iTween.EaseType.easeOutBounce;

	// Token: 0x0400464B RID: 17995
	public float m_SurvivorHoldDuration = 0.5f;

	// Token: 0x0400464C RID: 17996
	public List<GameObject> m_LeftJumpOutBones;

	// Token: 0x0400464D RID: 17997
	public List<GameObject> m_RightJumpOutBones;

	// Token: 0x0400464E RID: 17998
	public AudioSource m_JumpInSoundPrefab;

	// Token: 0x0400464F RID: 17999
	public float m_JumpInSoundDelay;

	// Token: 0x04004650 RID: 18000
	public AudioSource m_JumpOutSoundPrefab;

	// Token: 0x04004651 RID: 18001
	public float m_JumpOutSoundDelay;

	// Token: 0x04004652 RID: 18002
	public AudioSource m_LandSoundPrefab;

	// Token: 0x04004653 RID: 18003
	public float m_LandSoundDelay;

	// Token: 0x04004654 RID: 18004
	private int m_jumpsPending;

	// Token: 0x04004655 RID: 18005
	private Card m_survivorCard;
}
