using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000656 RID: 1622
[CustomEditClass]
public class BRMAnvilWeapons : MonoBehaviour
{
	// Token: 0x0600457C RID: 17788 RVA: 0x0014D934 File Offset: 0x0014BB34
	public void RandomWeaponEvent()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < this.m_Weapons.Count; i++)
		{
			if (i != this.m_LastWeaponIndex)
			{
				list.Add(i);
			}
		}
		if (this.m_Weapons.Count > 0 && list.Count > 0)
		{
			int num = Random.Range(0, list.Count);
			BRMAnvilWeapons.AnvilWeapon anvilWeapon = this.m_Weapons[list[num]];
			this.m_LastWeaponIndex = list[num];
			anvilWeapon.m_FSM.SendEvent(anvilWeapon.m_Events[this.RandomSubWeapon(anvilWeapon)]);
		}
	}

	// Token: 0x0600457D RID: 17789 RVA: 0x0014D9E4 File Offset: 0x0014BBE4
	public int RandomSubWeapon(BRMAnvilWeapons.AnvilWeapon weapon)
	{
		List<int> list = new List<int>();
		for (int i = 0; i < weapon.m_Events.Count; i++)
		{
			if (i != weapon.m_CurrentWeaponIndex)
			{
				list.Add(i);
			}
		}
		int num = Random.Range(0, list.Count);
		weapon.m_CurrentWeaponIndex = list[num];
		return list[num];
	}

	// Token: 0x04002C73 RID: 11379
	public List<BRMAnvilWeapons.AnvilWeapon> m_Weapons;

	// Token: 0x04002C74 RID: 11380
	private int m_LastWeaponIndex;

	// Token: 0x02000657 RID: 1623
	[Serializable]
	public class AnvilWeapon
	{
		// Token: 0x04002C75 RID: 11381
		public PlayMakerFSM m_FSM;

		// Token: 0x04002C76 RID: 11382
		public List<string> m_Events;

		// Token: 0x04002C77 RID: 11383
		[HideInInspector]
		public int m_CurrentWeaponIndex;
	}
}
