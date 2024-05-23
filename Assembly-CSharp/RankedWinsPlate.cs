using System;

// Token: 0x020007EE RID: 2030
public class RankedWinsPlate : PegUIElement
{
	// Token: 0x06004EF3 RID: 20211 RVA: 0x00176C34 File Offset: 0x00174E34
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		base.GetComponent<TooltipZone>().ShowTooltip(GameStrings.Get("GLUE_TOOLTIP_GOLDEN_WINS_HEADER"), GameStrings.Get("GLUE_TOOLTIP_GOLDEN_WINS_DESC"), 5f, true);
	}

	// Token: 0x06004EF4 RID: 20212 RVA: 0x00176C67 File Offset: 0x00174E67
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		base.GetComponent<TooltipZone>().HideTooltip();
	}
}
