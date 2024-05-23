using System;
using UnityEngine;

// Token: 0x020004EA RID: 1258
[CustomEditClass]
public class ButtonListMenuDef : MonoBehaviour
{
	// Token: 0x040025BF RID: 9663
	[CustomEditField(Sections = "Header")]
	public UberText m_headerText;

	// Token: 0x040025C0 RID: 9664
	[CustomEditField(Sections = "Header")]
	public MultiSliceElement m_header;

	// Token: 0x040025C1 RID: 9665
	[CustomEditField(Sections = "Header")]
	public GameObject m_headerMiddle;

	// Token: 0x040025C2 RID: 9666
	[CustomEditField(Sections = "Layout")]
	public NineSliceElement m_background;

	// Token: 0x040025C3 RID: 9667
	[CustomEditField(Sections = "Layout")]
	public NineSliceElement m_border;

	// Token: 0x040025C4 RID: 9668
	[CustomEditField(Sections = "Layout")]
	public MultiSliceElement m_buttonContainer;

	// Token: 0x040025C5 RID: 9669
	[CustomEditField(Sections = "Template Items")]
	public UIBButton m_templateButton;

	// Token: 0x040025C6 RID: 9670
	[CustomEditField(Sections = "Template Items")]
	public GameObject m_templateHorizontalDivider;

	// Token: 0x040025C7 RID: 9671
	[CustomEditField(Sections = "Template Items")]
	public Vector3 m_horizontalDividerMinPadding = new Vector3(0f, 0f, 0.18f);
}
