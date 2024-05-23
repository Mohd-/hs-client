using System;

// Token: 0x02000F5A RID: 3930
public class OnOffToggleButton : CheckBox
{
	// Token: 0x060074C8 RID: 29896 RVA: 0x00227CEF File Offset: 0x00225EEF
	public override void SetChecked(bool isChecked)
	{
		base.SetChecked(isChecked);
		base.SetButtonText(GameStrings.Get((!isChecked) ? this.m_offLabel : this.m_onLabel));
	}

	// Token: 0x04005F63 RID: 24419
	public string m_onLabel = "GLOBAL_ON";

	// Token: 0x04005F64 RID: 24420
	public string m_offLabel = "GLOBAL_OFF";
}
