using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000F0E RID: 3854
public class Gryphon : MonoBehaviour
{
	// Token: 0x06007315 RID: 29461 RVA: 0x0021DEA4 File Offset: 0x0021C0A4
	private void Start()
	{
		this.m_Animator = base.GetComponent<Animator>();
		this.m_UniversalInputManager = UniversalInputManager.Get();
		this.m_ScreechSound = base.GetComponent<AudioSource>();
		this.m_SnapWaitTime = Random.Range(5f, 20f);
		this.m_Animator.SetLayerWeight(1, 1f);
	}

	// Token: 0x06007316 RID: 29462 RVA: 0x0021DEFC File Offset: 0x0021C0FC
	private void LateUpdate()
	{
		bool flag = false;
		this.m_CurrentBaseLayerState = this.m_Animator.GetCurrentAnimatorStateInfo(0);
		if (this.m_UniversalInputManager != null)
		{
			if (GameState.Get() != null && GameState.Get().IsMulliganManagerActive())
			{
				return;
			}
			if (this.m_UniversalInputManager.InputIsOver(base.gameObject))
			{
				flag = UniversalInputManager.Get().GetMouseButtonDown(0);
			}
			if (flag)
			{
				if (Time.time - this.m_lastScreech > 5f)
				{
					this.m_Animator.SetBool("Screech", true);
					SoundManager.Get().Play(this.m_ScreechSound);
					this.m_lastScreech = Time.time;
				}
			}
			else
			{
				this.m_Animator.SetBool("Screech", false);
			}
		}
		if (this.m_CurrentBaseLayerState.fullPathHash == Gryphon.lookState)
		{
			return;
		}
		if (this.m_CurrentBaseLayerState.fullPathHash == Gryphon.cleanState)
		{
			return;
		}
		if (this.m_CurrentBaseLayerState.fullPathHash == Gryphon.screechState)
		{
			return;
		}
		this.m_Animator.SetBool("Look", false);
		this.m_Animator.SetBool("Clean", false);
		this.PlayAniamtion();
	}

	// Token: 0x06007317 RID: 29463 RVA: 0x0021E034 File Offset: 0x0021C234
	private void FindEndTurnButton()
	{
		this.m_EndTurnButton = EndTurnButton.Get();
		if (this.m_EndTurnButton == null)
		{
			return;
		}
		this.m_EndTurnButtonTransform = this.m_EndTurnButton.transform;
	}

	// Token: 0x06007318 RID: 29464 RVA: 0x0021E070 File Offset: 0x0021C270
	private void FindSomethingToLookAt()
	{
		List<Vector3> list = new List<Vector3>();
		ZoneMgr zoneMgr = ZoneMgr.Get();
		if (zoneMgr == null)
		{
			this.PlayAniamtion();
			return;
		}
		List<ZonePlay> list2 = zoneMgr.FindZonesOfType<ZonePlay>();
		foreach (ZonePlay zonePlay in list2)
		{
			foreach (Card card in zonePlay.GetCards())
			{
				if (card.IsMousedOver())
				{
					this.m_LookAtPosition = card.transform.position;
					return;
				}
				list.Add(card.transform.position);
			}
		}
		if (Random.Range(0, 100) < this.m_LookAtHeroesPercent)
		{
			List<ZoneHero> list3 = ZoneMgr.Get().FindZonesOfType<ZoneHero>();
			foreach (ZoneHero zoneHero in list3)
			{
				foreach (Card card2 in zoneHero.GetCards())
				{
					if (card2.IsMousedOver())
					{
						this.m_LookAtPosition = card2.transform.position;
						return;
					}
					list.Add(card2.transform.position);
				}
			}
		}
		if (list.Count > 0)
		{
			int num = Random.Range(0, list.Count);
			this.m_LookAtPosition = list[num];
		}
		else
		{
			this.PlayAniamtion();
		}
	}

	// Token: 0x06007319 RID: 29465 RVA: 0x0021E270 File Offset: 0x0021C470
	private void PlayAniamtion()
	{
		if (Time.time < this.m_idleEndTime)
		{
			return;
		}
		if (Random.value > 0.5f)
		{
			this.m_idleEndTime = Time.time + 4f;
			this.m_Animator.SetBool("Look", false);
			this.m_Animator.SetBool("Clean", false);
		}
		else if (Random.value > 0.25f)
		{
			this.m_Animator.SetBool("Look", true);
		}
		else
		{
			this.m_Animator.SetBool("Clean", true);
		}
	}

	// Token: 0x0600731A RID: 29466 RVA: 0x0021E30C File Offset: 0x0021C50C
	private bool LookAtTurnButton()
	{
		if (this.m_EndTurnButton == null)
		{
			this.FindEndTurnButton();
		}
		if (this.m_EndTurnButton == null)
		{
			return false;
		}
		if (this.m_EndTurnButton.IsInNMPState() && this.m_EndTurnButtonTransform != null)
		{
			this.m_LookAtPosition = this.m_EndTurnButtonTransform.position;
			return true;
		}
		return false;
	}

	// Token: 0x0600731B RID: 29467 RVA: 0x0021E378 File Offset: 0x0021C578
	private void AniamteHead()
	{
		if (this.m_CurrentBaseLayerState.fullPathHash == Gryphon.lookState)
		{
			return;
		}
		if (this.m_CurrentBaseLayerState.fullPathHash == Gryphon.cleanState)
		{
			return;
		}
		if (this.m_CurrentBaseLayerState.fullPathHash == Gryphon.screechState)
		{
			return;
		}
		Vector3 vector = this.m_LookAtPosition - this.m_HeadBone.position;
		Quaternion quaternion = Quaternion.LookRotation(vector);
		this.m_HeadBone.rotation = Quaternion.Slerp(this.m_HeadBone.rotation, quaternion, Time.deltaTime * this.m_HeadRotationSpeed);
	}

	// Token: 0x04005D67 RID: 23911
	public float m_HeadRotationSpeed = 15f;

	// Token: 0x04005D68 RID: 23912
	public float m_MinFocusTime = 1.2f;

	// Token: 0x04005D69 RID: 23913
	public float m_MaxFocusTime = 5.5f;

	// Token: 0x04005D6A RID: 23914
	public int m_PlayAnimationPercent = 20;

	// Token: 0x04005D6B RID: 23915
	public int m_LookAtHeroesPercent = 20;

	// Token: 0x04005D6C RID: 23916
	public int m_LookAtTurnButtonPercent = 75;

	// Token: 0x04005D6D RID: 23917
	public float m_TurnButtonLookAwayTime = 0.5f;

	// Token: 0x04005D6E RID: 23918
	public float m_SnapWaitTime = 1f;

	// Token: 0x04005D6F RID: 23919
	public Transform m_HeadBone;

	// Token: 0x04005D70 RID: 23920
	public GameObject m_SnapCollider;

	// Token: 0x04005D71 RID: 23921
	private float m_WaitStartTime;

	// Token: 0x04005D72 RID: 23922
	private float m_RandomWeightsTotal;

	// Token: 0x04005D73 RID: 23923
	private Vector3 m_LookAtPosition;

	// Token: 0x04005D74 RID: 23924
	private Animator m_Animator;

	// Token: 0x04005D75 RID: 23925
	private EndTurnButton m_EndTurnButton;

	// Token: 0x04005D76 RID: 23926
	private Transform m_EndTurnButtonTransform;

	// Token: 0x04005D77 RID: 23927
	private UniversalInputManager m_UniversalInputManager;

	// Token: 0x04005D78 RID: 23928
	private AnimatorStateInfo m_CurrentBaseLayerState;

	// Token: 0x04005D79 RID: 23929
	private AudioSource m_ScreechSound;

	// Token: 0x04005D7A RID: 23930
	private float m_lastScreech;

	// Token: 0x04005D7B RID: 23931
	private float m_idleEndTime;

	// Token: 0x04005D7C RID: 23932
	private static int lookState = Animator.StringToHash("Base Layer.Look");

	// Token: 0x04005D7D RID: 23933
	private static int cleanState = Animator.StringToHash("Base Layer.Clean");

	// Token: 0x04005D7E RID: 23934
	private static int screechState = Animator.StringToHash("Base Layer.Screech");
}
