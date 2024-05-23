using System;
using UnityEngine;

// Token: 0x020006F1 RID: 1777
public class CollectionCardLock : MonoBehaviour
{
	// Token: 0x06004942 RID: 18754 RVA: 0x0015E0D8 File Offset: 0x0015C2D8
	private void Start()
	{
	}

	// Token: 0x06004943 RID: 18755 RVA: 0x0015E0DC File Offset: 0x0015C2DC
	public void UpdateLockVisual(EntityDef entityDef, CollectionCardVisual.LockType lockType, string reason)
	{
		if (entityDef == null || lockType == CollectionCardVisual.LockType.NONE)
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		TAG_CARDTYPE cardType = entityDef.GetCardType();
		this.m_allyBg.SetActive(false);
		this.m_spellBg.SetActive(false);
		this.m_weaponBg.SetActive(false);
		GameObject gameObject;
		switch (cardType)
		{
		case TAG_CARDTYPE.MINION:
			gameObject = this.m_allyBg;
			this.m_lockPlate.transform.localPosition = this.m_lockPlateBone.transform.localPosition;
			goto IL_101;
		case TAG_CARDTYPE.SPELL:
			gameObject = this.m_spellBg;
			this.m_lockPlate.transform.localPosition = this.m_lockPlateBone.transform.localPosition;
			goto IL_101;
		case TAG_CARDTYPE.WEAPON:
			gameObject = this.m_weaponBg;
			this.m_lockPlate.transform.localPosition = this.m_weaponLockPlateBone.transform.localPosition;
			goto IL_101;
		}
		gameObject = this.m_spellBg;
		IL_101:
		float num = 0f;
		if (lockType != CollectionCardVisual.LockType.MAX_COPIES_IN_DECK)
		{
			if (lockType == CollectionCardVisual.LockType.NO_MORE_INSTANCES)
			{
				num = 1f;
				this.SetLockText(GameStrings.Get("GLUE_COLLECTION_LOCK_NO_MORE_INSTANCES"));
			}
		}
		else
		{
			num = 0f;
			int num2 = (!entityDef.IsElite()) ? 2 : 1;
			this.SetLockText(GameStrings.Format("GLUE_COLLECTION_LOCK_MAX_DECK_COPIES", new object[]
			{
				num2
			}));
		}
		this.SetLockText(reason);
		this.m_lockPlate.GetComponent<Renderer>().material.SetFloat("_Desaturate", num);
		gameObject.GetComponent<Renderer>().material.SetFloat("_Desaturate", num);
		gameObject.SetActive(true);
	}

	// Token: 0x06004944 RID: 18756 RVA: 0x0015E29F File Offset: 0x0015C49F
	public void SetLockText(string text)
	{
		this.m_lockText.Text = text;
	}

	// Token: 0x06004945 RID: 18757 RVA: 0x0015E2AD File Offset: 0x0015C4AD
	public void Hide()
	{
		base.gameObject.SetActive(false);
	}

	// Token: 0x0400305C RID: 12380
	public GameObject m_allyBg;

	// Token: 0x0400305D RID: 12381
	public GameObject m_spellBg;

	// Token: 0x0400305E RID: 12382
	public GameObject m_weaponBg;

	// Token: 0x0400305F RID: 12383
	public GameObject m_lockPlate;

	// Token: 0x04003060 RID: 12384
	public UberText m_lockText;

	// Token: 0x04003061 RID: 12385
	public GameObject m_lockPlateBone;

	// Token: 0x04003062 RID: 12386
	public GameObject m_weaponLockPlateBone;
}
