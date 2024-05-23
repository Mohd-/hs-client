using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000975 RID: 2421
public class WeaponSocketMgr : MonoBehaviour
{
	// Token: 0x060057EE RID: 22510 RVA: 0x001A4DA4 File Offset: 0x001A2FA4
	public void UpdateSockets()
	{
		if (this.m_Decorations != null)
		{
			foreach (WeaponSocketDecoration weaponSocketDecoration in this.m_Decorations)
			{
				weaponSocketDecoration.UpdateVisibility();
			}
		}
	}

	// Token: 0x060057EF RID: 22511 RVA: 0x001A4E08 File Offset: 0x001A3008
	public static bool ShouldSeeWeaponSocket(TAG_CLASS tagVal)
	{
		switch (tagVal)
		{
		case TAG_CLASS.DRUID:
		case TAG_CLASS.MAGE:
		case TAG_CLASS.PRIEST:
		case TAG_CLASS.WARLOCK:
			return false;
		}
		return true;
	}

	// Token: 0x04003F17 RID: 16151
	public List<WeaponSocketDecoration> m_Decorations;
}
