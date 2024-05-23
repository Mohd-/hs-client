using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200061F RID: 1567
public class ZoneWeapon : Zone
{
	// Token: 0x0600446D RID: 17517 RVA: 0x00148F75 File Offset: 0x00147175
	public override string ToString()
	{
		return string.Format("{0} (Weapon)", base.ToString());
	}

	// Token: 0x0600446E RID: 17518 RVA: 0x00148F87 File Offset: 0x00147187
	public override bool CanAcceptTags(int controllerId, TAG_ZONE zoneTag, TAG_CARDTYPE cardType)
	{
		return base.CanAcceptTags(controllerId, zoneTag, cardType) && cardType == TAG_CARDTYPE.WEAPON;
	}

	// Token: 0x0600446F RID: 17519 RVA: 0x00148FA4 File Offset: 0x001471A4
	public override int RemoveCard(Card card)
	{
		int num = base.RemoveCard(card);
		if (num >= 0 && !this.m_destroyedWeapons.Contains(card))
		{
			this.m_destroyedWeapons.Add(card);
		}
		return num;
	}

	// Token: 0x06004470 RID: 17520 RVA: 0x00148FE0 File Offset: 0x001471E0
	public override void UpdateLayout()
	{
		if (GameState.Get().IsMulliganManagerActive())
		{
			base.UpdateLayoutFinished();
			return;
		}
		this.m_updatingLayout = true;
		if (base.IsBlockingLayout())
		{
			base.UpdateLayoutFinished();
			return;
		}
		if (this.m_cards.Count == 0)
		{
			this.m_destroyedWeapons.Clear();
			base.UpdateLayoutFinished();
			return;
		}
		base.StartCoroutine(this.UpdateLayoutImpl());
	}

	// Token: 0x06004471 RID: 17521 RVA: 0x0014904C File Offset: 0x0014724C
	private IEnumerator UpdateLayoutImpl()
	{
		Card equippedWeapon = this.m_cards[0];
		while (equippedWeapon.IsDoNotSort())
		{
			yield return null;
		}
		equippedWeapon.ShowCard();
		equippedWeapon.EnableTransitioningZones(true);
		string tweenName = ZoneMgr.Get().GetTweenName<ZoneWeapon>();
		if (this.m_Side == Player.Side.OPPOSING)
		{
			iTween.StopOthersByName(equippedWeapon.gameObject, tweenName, false);
		}
		Vector3 intermediatePosition = base.transform.position;
		intermediatePosition.y += 1.5f;
		Hashtable moveArgs = iTween.Hash(new object[]
		{
			"name",
			tweenName,
			"position",
			intermediatePosition,
			"time",
			0.9f
		});
		iTween.MoveTo(equippedWeapon.gameObject, moveArgs);
		Hashtable rotateArgs = iTween.Hash(new object[]
		{
			"name",
			tweenName,
			"rotation",
			base.transform.localEulerAngles,
			"time",
			0.9f
		});
		iTween.RotateTo(equippedWeapon.gameObject, rotateArgs);
		Hashtable scaleArgs = iTween.Hash(new object[]
		{
			"name",
			tweenName,
			"scale",
			base.transform.localScale,
			"time",
			0.9f
		});
		iTween.ScaleTo(equippedWeapon.gameObject, scaleArgs);
		yield return new WaitForSeconds(0.9f);
		if (this.m_destroyedWeapons.Count > 0)
		{
			yield return new WaitForSeconds(1.75f);
		}
		this.m_destroyedWeapons.Clear();
		moveArgs = iTween.Hash(new object[]
		{
			"position",
			base.transform.position,
			"time",
			0.1f,
			"easetype",
			iTween.EaseType.easeOutCubic,
			"name",
			tweenName
		});
		iTween.MoveTo(equippedWeapon.gameObject, moveArgs);
		base.StartFinishLayoutTimer(0.1f);
		yield break;
	}

	// Token: 0x04002B65 RID: 11109
	private const float INTERMEDIATE_Y_OFFSET = 1.5f;

	// Token: 0x04002B66 RID: 11110
	private const float INTERMEDIATE_TRANSITION_SEC = 0.9f;

	// Token: 0x04002B67 RID: 11111
	private const float DESTROYED_WEAPON_WAIT_SEC = 1.75f;

	// Token: 0x04002B68 RID: 11112
	private const float FINAL_TRANSITION_SEC = 0.1f;

	// Token: 0x04002B69 RID: 11113
	private List<Card> m_destroyedWeapons = new List<Card>();
}
