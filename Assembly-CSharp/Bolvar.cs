using System;

// Token: 0x02000E3E RID: 3646
public class Bolvar : SuperSpell
{
	// Token: 0x06006F00 RID: 28416 RVA: 0x00208EA4 File Offset: 0x002070A4
	protected override void OnAction(SpellStateType prevStateType)
	{
		this.m_effectsPendingFinish++;
		base.OnAction(prevStateType);
		Card sourceCard = base.GetSourceCard();
		Entity entity = sourceCard.GetEntity();
		Spell prefab = this.DetermineRangePrefab(entity.GetATK());
		Spell spell = base.CloneSpell(prefab);
		spell.SetSource(sourceCard.gameObject);
		spell.Activate();
		this.m_effectsPendingFinish--;
		base.FinishIfPossible();
	}

	// Token: 0x06006F01 RID: 28417 RVA: 0x00208F10 File Offset: 0x00207110
	private int GetRangePrefabMin()
	{
		if (this.m_atkPrefabs.Length == 0)
		{
			return 0;
		}
		int minAtk = this.m_atkPrefabs[0].m_MinAtk;
		for (int i = 1; i < this.m_atkPrefabs.Length; i++)
		{
			if (this.m_atkPrefabs[i].m_MinAtk < minAtk)
			{
				minAtk = this.m_atkPrefabs[i].m_MinAtk;
			}
		}
		return minAtk;
	}

	// Token: 0x06006F02 RID: 28418 RVA: 0x00208F78 File Offset: 0x00207178
	private int GetRangePrefabMax()
	{
		if (this.m_atkPrefabs.Length == 0)
		{
			return 0;
		}
		int maxAtk = this.m_atkPrefabs[0].m_MaxAtk;
		for (int i = 1; i < this.m_atkPrefabs.Length; i++)
		{
			if (this.m_atkPrefabs[i].m_MaxAtk > maxAtk)
			{
				maxAtk = this.m_atkPrefabs[i].m_MaxAtk;
			}
		}
		return maxAtk;
	}

	// Token: 0x06006F03 RID: 28419 RVA: 0x00208FE0 File Offset: 0x002071E0
	private Spell DetermineRangePrefab(int atk)
	{
		Spell result = null;
		if (this.m_atkPrefabs.Length == 0)
		{
			return result;
		}
		if (atk > this.GetRangePrefabMax())
		{
			return this.m_atkPrefabs[this.m_atkPrefabs.Length - 1].m_Prefab;
		}
		if (atk < this.GetRangePrefabMin())
		{
			return this.m_atkPrefabs[0].m_Prefab;
		}
		for (int i = 0; i < this.m_atkPrefabs.Length; i++)
		{
			if (atk >= this.m_atkPrefabs[i].m_MinAtk && atk <= this.m_atkPrefabs[i].m_MaxAtk)
			{
				return this.m_atkPrefabs[i].m_Prefab;
			}
		}
		return result;
	}

	// Token: 0x040057FD RID: 22525
	public AttackRangePrefabs[] m_atkPrefabs;
}
