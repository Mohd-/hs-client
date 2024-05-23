using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200024A RID: 586
public class BoxDisk : MonoBehaviour
{
	// Token: 0x0600219E RID: 8606 RVA: 0x000A4572 File Offset: 0x000A2772
	public void SetParent(Box parent)
	{
		this.m_parent = parent;
	}

	// Token: 0x0600219F RID: 8607 RVA: 0x000A457B File Offset: 0x000A277B
	public Box GetParent()
	{
		return this.m_parent;
	}

	// Token: 0x060021A0 RID: 8608 RVA: 0x000A4583 File Offset: 0x000A2783
	public BoxDiskStateInfo GetInfo()
	{
		return this.m_info;
	}

	// Token: 0x060021A1 RID: 8609 RVA: 0x000A458B File Offset: 0x000A278B
	public void SetInfo(BoxDiskStateInfo info)
	{
		this.m_info = info;
	}

	// Token: 0x060021A2 RID: 8610 RVA: 0x000A4594 File Offset: 0x000A2794
	public bool ChangeState(BoxDisk.State state)
	{
		if (this.m_state == state)
		{
			return false;
		}
		this.m_state = state;
		if (state == BoxDisk.State.LOADING)
		{
			this.m_parent.OnAnimStarted();
			Vector3 vector = this.m_info.m_LoadingRotation - base.transform.localRotation.eulerAngles;
			Hashtable args = iTween.Hash(new object[]
			{
				"amount",
				vector,
				"delay",
				this.m_info.m_LoadingDelaySec,
				"time",
				this.m_info.m_LoadingRotateSec,
				"easeType",
				this.m_info.m_LoadingRotateEaseType,
				"space",
				1,
				"oncomplete",
				"OnAnimFinished",
				"oncompletetarget",
				this.m_parent.gameObject
			});
			iTween.RotateAdd(base.gameObject, args);
			Spell eventSpell = this.m_parent.GetEventSpell(BoxEventType.DISK_LOADING);
			eventSpell.ActivateState(SpellStateType.BIRTH);
		}
		else if (state == BoxDisk.State.MAINMENU)
		{
			this.m_parent.OnAnimStarted();
			Vector3 vector2 = this.m_info.m_MainMenuRotation - base.transform.localRotation.eulerAngles;
			Hashtable args2 = iTween.Hash(new object[]
			{
				"amount",
				vector2,
				"delay",
				this.m_info.m_MainMenuDelaySec,
				"time",
				this.m_info.m_MainMenuRotateSec,
				"easeType",
				this.m_info.m_MainMenuRotateEaseType,
				"space",
				1,
				"oncomplete",
				"OnAnimFinished",
				"oncompletetarget",
				this.m_parent.gameObject
			});
			iTween.RotateAdd(base.gameObject, args2);
			Spell eventSpell2 = this.m_parent.GetEventSpell(BoxEventType.DISK_MAIN_MENU);
			eventSpell2.ActivateState(SpellStateType.BIRTH);
		}
		return true;
	}

	// Token: 0x060021A3 RID: 8611 RVA: 0x000A47C4 File Offset: 0x000A29C4
	public void UpdateState(BoxDisk.State state)
	{
		this.m_state = state;
		if (state == BoxDisk.State.LOADING)
		{
			base.transform.localRotation = Quaternion.Euler(this.m_info.m_LoadingRotation);
			Spell eventSpell = this.m_parent.GetEventSpell(BoxEventType.DISK_LOADING);
			eventSpell.ActivateState(SpellStateType.ACTION);
		}
		else if (state == BoxDisk.State.MAINMENU)
		{
			base.transform.localRotation = Quaternion.Euler(this.m_info.m_MainMenuRotation);
			Spell eventSpell2 = this.m_parent.GetEventSpell(BoxEventType.DISK_MAIN_MENU);
			eventSpell2.ActivateState(SpellStateType.ACTION);
		}
	}

	// Token: 0x040012FF RID: 4863
	private Box m_parent;

	// Token: 0x04001300 RID: 4864
	private BoxDiskStateInfo m_info;

	// Token: 0x04001301 RID: 4865
	private BoxDisk.State m_state;

	// Token: 0x0200024B RID: 587
	public enum State
	{
		// Token: 0x04001303 RID: 4867
		LOADING,
		// Token: 0x04001304 RID: 4868
		MAINMENU
	}
}
