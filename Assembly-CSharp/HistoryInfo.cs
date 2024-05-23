using System;

// Token: 0x02000913 RID: 2323
public class HistoryInfo
{
	// Token: 0x0600565F RID: 22111 RVA: 0x0019EE0C File Offset: 0x0019D00C
	public int GetSplatAmount()
	{
		return this.m_damageChangeAmount + this.m_armorChangeAmount;
	}

	// Token: 0x06005660 RID: 22112 RVA: 0x0019EE1B File Offset: 0x0019D01B
	public Entity GetDuplicatedEntity()
	{
		return this.m_duplicatedEntity;
	}

	// Token: 0x06005661 RID: 22113 RVA: 0x0019EE23 File Offset: 0x0019D023
	public Entity GetOriginalEntity()
	{
		return this.m_originalEntity;
	}

	// Token: 0x06005662 RID: 22114 RVA: 0x0019EE2B File Offset: 0x0019D02B
	public void SetOriginalEntity(Entity entity)
	{
		this.m_originalEntity = entity;
		this.DuplicateEntity();
	}

	// Token: 0x06005663 RID: 22115 RVA: 0x0019EE3A File Offset: 0x0019D03A
	public bool HasDied()
	{
		return this.m_died;
	}

	// Token: 0x06005664 RID: 22116 RVA: 0x0019EE42 File Offset: 0x0019D042
	public void SetDied(bool set)
	{
		this.m_died = set;
	}

	// Token: 0x06005665 RID: 22117 RVA: 0x0019EE4C File Offset: 0x0019D04C
	public bool CanDuplicateEntity()
	{
		return this.m_originalEntity != null && this.m_originalEntity.GetLoadState() == Entity.LoadState.DONE && (!this.m_originalEntity.IsHidden() || (this.m_originalEntity.IsSecret() && GameUtils.IsEntityHiddenAfterCurrentTasklist(this.m_originalEntity)));
	}

	// Token: 0x06005666 RID: 22118 RVA: 0x0019EEAC File Offset: 0x0019D0AC
	public void DuplicateEntity()
	{
		if (this.m_duplicatedEntity != null)
		{
			return;
		}
		this.CacheBonuses();
		if (!this.CanDuplicateEntity())
		{
			return;
		}
		this.m_duplicatedEntity = this.m_originalEntity.CloneForHistory(this.m_originalDamageBonus, this.m_originalDamageBonusDouble, this.m_originalHealingDouble);
	}

	// Token: 0x06005667 RID: 22119 RVA: 0x0019EEFC File Offset: 0x0019D0FC
	private void CacheBonuses()
	{
		if (!this.CanCacheBonuses())
		{
			return;
		}
		this.m_originalDamageBonus = this.m_originalEntity.GetDamageBonus();
		this.m_originalDamageBonusDouble = this.m_originalEntity.GetDamageBonusDouble();
		this.m_originalHealingDouble = this.m_originalEntity.GetHealingDouble();
		this.m_cachedBonuses = true;
	}

	// Token: 0x06005668 RID: 22120 RVA: 0x0019EF50 File Offset: 0x0019D150
	private bool CanCacheBonuses()
	{
		return !this.m_cachedBonuses && this.m_originalEntity != null && this.m_originalEntity.HasTag(GAME_TAG.CARDTYPE);
	}

	// Token: 0x04003D0A RID: 15626
	public HistoryInfoType m_infoType;

	// Token: 0x04003D0B RID: 15627
	public int m_damageChangeAmount;

	// Token: 0x04003D0C RID: 15628
	public int m_armorChangeAmount;

	// Token: 0x04003D0D RID: 15629
	private Entity m_originalEntity;

	// Token: 0x04003D0E RID: 15630
	private int m_originalDamageBonus;

	// Token: 0x04003D0F RID: 15631
	private int m_originalDamageBonusDouble;

	// Token: 0x04003D10 RID: 15632
	private int m_originalHealingDouble;

	// Token: 0x04003D11 RID: 15633
	private Entity m_duplicatedEntity;

	// Token: 0x04003D12 RID: 15634
	private bool m_died;

	// Token: 0x04003D13 RID: 15635
	private bool m_cachedBonuses;
}
