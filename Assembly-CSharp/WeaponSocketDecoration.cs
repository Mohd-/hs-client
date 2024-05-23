using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000974 RID: 2420
public class WeaponSocketDecoration : MonoBehaviour
{
	// Token: 0x060057E8 RID: 22504 RVA: 0x001A4C24 File Offset: 0x001A2E24
	public bool IsShown()
	{
		return base.GetComponent<Renderer>().enabled;
	}

	// Token: 0x060057E9 RID: 22505 RVA: 0x001A4C31 File Offset: 0x001A2E31
	public void UpdateVisibility()
	{
		if (this.AreVisibilityRequirementsMet())
		{
			this.Show();
		}
		else
		{
			this.Hide();
		}
	}

	// Token: 0x060057EA RID: 22506 RVA: 0x001A4C50 File Offset: 0x001A2E50
	public bool AreVisibilityRequirementsMet()
	{
		Map<int, Player> playerMap = GameState.Get().GetPlayerMap();
		if (playerMap == null)
		{
			return false;
		}
		if (this.m_VisibilityRequirements == null)
		{
			return false;
		}
		foreach (WeaponSocketRequirement weaponSocketRequirement in this.m_VisibilityRequirements)
		{
			bool flag = false;
			foreach (Player player in playerMap.Values)
			{
				if (weaponSocketRequirement.m_Side == player.GetSide())
				{
					Entity hero = player.GetHero();
					if (hero == null)
					{
						Debug.LogWarning(string.Format("WeaponSocketDecoration.AreVisibilityRequirementsMet() - player {0} has no hero", player));
						return false;
					}
					if (weaponSocketRequirement.m_HasWeapon != WeaponSocketMgr.ShouldSeeWeaponSocket(hero.GetClass()))
					{
						return false;
					}
					flag = true;
				}
			}
			if (!flag)
			{
				return false;
			}
		}
		return true;
	}

	// Token: 0x060057EB RID: 22507 RVA: 0x001A4D80 File Offset: 0x001A2F80
	public void Show()
	{
		SceneUtils.EnableRenderers(base.gameObject, true);
	}

	// Token: 0x060057EC RID: 22508 RVA: 0x001A4D8E File Offset: 0x001A2F8E
	public void Hide()
	{
		SceneUtils.EnableRenderers(base.gameObject, false);
	}

	// Token: 0x04003F16 RID: 16150
	public List<WeaponSocketRequirement> m_VisibilityRequirements;
}
