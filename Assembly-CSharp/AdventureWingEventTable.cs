using System;
using System.Collections.Generic;

// Token: 0x020003AE RID: 942
[CustomEditClass]
public class AdventureWingEventTable : StateEventTable
{
	// Token: 0x060031C2 RID: 12738 RVA: 0x000FAA44 File Offset: 0x000F8C44
	public bool IsPlateBuy()
	{
		string lastState = base.GetLastState();
		return lastState == "PlateBuy" || lastState == "PlateInitialBuy";
	}

	// Token: 0x060031C3 RID: 12739 RVA: 0x000FAA76 File Offset: 0x000F8C76
	public bool IsPlateInitialText()
	{
		return base.GetLastState() == "PlateInitialText";
	}

	// Token: 0x060031C4 RID: 12740 RVA: 0x000FAA88 File Offset: 0x000F8C88
	public bool IsPlateInOrGoingToAnActiveState()
	{
		string lastState = base.GetLastState();
		string text = lastState;
		if (text != null)
		{
			if (AdventureWingEventTable.<>f__switch$map0 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(6);
				dictionary.Add("PlateActivate", 0);
				dictionary.Add("PlateInitialText", 0);
				dictionary.Add("PlateBuy", 0);
				dictionary.Add("PlateInitialBuy", 0);
				dictionary.Add("PlateKey", 0);
				dictionary.Add("PlateInitialKey", 0);
				AdventureWingEventTable.<>f__switch$map0 = dictionary;
			}
			int num;
			if (AdventureWingEventTable.<>f__switch$map0.TryGetValue(text, ref num))
			{
				if (num == 0)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x060031C5 RID: 12741 RVA: 0x000FAB23 File Offset: 0x000F8D23
	public void PlateActivate()
	{
		base.TriggerState("PlateActivate", true, null);
	}

	// Token: 0x060031C6 RID: 12742 RVA: 0x000FAB32 File Offset: 0x000F8D32
	public void PlateDeactivate()
	{
		base.TriggerState("PlateDeactivate", true, null);
	}

	// Token: 0x060031C7 RID: 12743 RVA: 0x000FAB41 File Offset: 0x000F8D41
	public void PlateBuy(bool initial = false)
	{
		if (this.IsPlateBuy())
		{
			return;
		}
		base.TriggerState((!initial) ? "PlateBuy" : "PlateInitialBuy", true, null);
	}

	// Token: 0x060031C8 RID: 12744 RVA: 0x000FAB6C File Offset: 0x000F8D6C
	public void PlateInitialText()
	{
		base.TriggerState("PlateInitialText", true, null);
	}

	// Token: 0x060031C9 RID: 12745 RVA: 0x000FAB7C File Offset: 0x000F8D7C
	public void PlateKey(bool initial = false)
	{
		if (!initial && !this.IsPlateBuy())
		{
			base.TriggerState("PlateBuy", true, null);
		}
		base.TriggerState((!initial) ? "PlateKey" : "PlateInitialKey", true, null);
	}

	// Token: 0x060031CA RID: 12746 RVA: 0x000FABC4 File Offset: 0x000F8DC4
	public void PlateOpen(float delay = 0f)
	{
		base.SetFloatVar("PlateOpen", "PostAnimationDelay", delay);
		base.TriggerState("PlateOpen", true, null);
	}

	// Token: 0x060031CB RID: 12747 RVA: 0x000FABE4 File Offset: 0x000F8DE4
	public void PlateCoverPreviewChest()
	{
		base.TriggerState("PlateCoverPreviewChest", false, null);
	}

	// Token: 0x060031CC RID: 12748 RVA: 0x000FABF3 File Offset: 0x000F8DF3
	public void BigChestShow()
	{
		base.TriggerState("BigChestShow", true, null);
	}

	// Token: 0x060031CD RID: 12749 RVA: 0x000FAC02 File Offset: 0x000F8E02
	public void BigChestStayOpen()
	{
		base.TriggerState("BigChestStayOpen", true, null);
	}

	// Token: 0x060031CE RID: 12750 RVA: 0x000FAC11 File Offset: 0x000F8E11
	public void BigChestOpen()
	{
		base.TriggerState("BigChestOpen", true, null);
	}

	// Token: 0x060031CF RID: 12751 RVA: 0x000FAC20 File Offset: 0x000F8E20
	public void BigChestCover()
	{
		base.TriggerState("BigChestCover", true, null);
	}

	// Token: 0x060031D0 RID: 12752 RVA: 0x000FAC2F File Offset: 0x000F8E2F
	public void AddOpenPlateStartEventListener(StateEventTable.StateEventTrigger dlg, bool once = false)
	{
		base.AddStateEventStartListener("PlateOpen", dlg, once);
	}

	// Token: 0x060031D1 RID: 12753 RVA: 0x000FAC3E File Offset: 0x000F8E3E
	public void RemoveOpenPlateStartEventListener(StateEventTable.StateEventTrigger dlg)
	{
		base.RemoveStateEventStartListener("PlateOpen", dlg);
	}

	// Token: 0x060031D2 RID: 12754 RVA: 0x000FAC4C File Offset: 0x000F8E4C
	public void AddOpenPlateEndEventListener(StateEventTable.StateEventTrigger dlg, bool once = false)
	{
		base.AddStateEventEndListener("PlateOpen", dlg, once);
	}

	// Token: 0x060031D3 RID: 12755 RVA: 0x000FAC5B File Offset: 0x000F8E5B
	public void RemoveOpenPlateEndEventListener(StateEventTable.StateEventTrigger dlg)
	{
		base.RemoveStateEventEndListener("PlateOpen", dlg);
	}

	// Token: 0x060031D4 RID: 12756 RVA: 0x000FAC69 File Offset: 0x000F8E69
	public void AddOpenChestStartEventListener(StateEventTable.StateEventTrigger dlg, bool once = false)
	{
		base.AddStateEventStartListener("BigChestOpen", dlg, once);
	}

	// Token: 0x060031D5 RID: 12757 RVA: 0x000FAC78 File Offset: 0x000F8E78
	public void RemoveOpenChestStartEventListener(StateEventTable.StateEventTrigger dlg)
	{
		base.RemoveStateEventStartListener("BigChestOpen", dlg);
	}

	// Token: 0x060031D6 RID: 12758 RVA: 0x000FAC86 File Offset: 0x000F8E86
	public void AddOpenChestEndEventListener(StateEventTable.StateEventTrigger dlg, bool once = false)
	{
		base.AddStateEventEndListener("BigChestOpen", dlg, once);
	}

	// Token: 0x060031D7 RID: 12759 RVA: 0x000FAC95 File Offset: 0x000F8E95
	public void RemoveOpenChestEndEventListener(StateEventTable.StateEventTrigger dlg)
	{
		base.RemoveStateEventEndListener("BigChestOpen", dlg);
	}

	// Token: 0x04001F08 RID: 7944
	private const string s_EventPlateActivate = "PlateActivate";

	// Token: 0x04001F09 RID: 7945
	private const string s_EventPlateDeactivate = "PlateDeactivate";

	// Token: 0x04001F0A RID: 7946
	private const string s_EventPlateInitialText = "PlateInitialText";

	// Token: 0x04001F0B RID: 7947
	private const string s_EventPlateBuy = "PlateBuy";

	// Token: 0x04001F0C RID: 7948
	private const string s_EventPlateInitialBuy = "PlateInitialBuy";

	// Token: 0x04001F0D RID: 7949
	private const string s_EventPlateKey = "PlateKey";

	// Token: 0x04001F0E RID: 7950
	private const string s_EventPlateInitialKey = "PlateInitialKey";

	// Token: 0x04001F0F RID: 7951
	private const string s_EventPlateOpen = "PlateOpen";

	// Token: 0x04001F10 RID: 7952
	private const string s_EventBigChestShow = "BigChestShow";

	// Token: 0x04001F11 RID: 7953
	private const string s_EventBigChestStayOpen = "BigChestStayOpen";

	// Token: 0x04001F12 RID: 7954
	private const string s_EventBigChestOpen = "BigChestOpen";

	// Token: 0x04001F13 RID: 7955
	private const string s_EventBigChestCover = "BigChestCover";

	// Token: 0x04001F14 RID: 7956
	private const string s_EventPlateCoverPreviewChest = "PlateCoverPreviewChest";
}
