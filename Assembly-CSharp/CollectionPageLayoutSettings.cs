using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020006E6 RID: 1766
[Serializable]
public class CollectionPageLayoutSettings
{
	// Token: 0x060048E0 RID: 18656 RVA: 0x0015C5F4 File Offset: 0x0015A7F4
	public CollectionPageLayoutSettings.Variables GetVariables(CollectionManagerDisplay.ViewMode mode)
	{
		CollectionPageLayoutSettings.Variables variables = this.m_layoutVariables.Find((CollectionPageLayoutSettings.Variables v) => mode == v.m_ViewMode);
		if (variables == null)
		{
			return new CollectionPageLayoutSettings.Variables();
		}
		return variables;
	}

	// Token: 0x04003001 RID: 12289
	[CustomEditField(ListTable = true)]
	public List<CollectionPageLayoutSettings.Variables> m_layoutVariables = new List<CollectionPageLayoutSettings.Variables>();

	// Token: 0x020006E7 RID: 1767
	[Serializable]
	public class Variables
	{
		// Token: 0x04003002 RID: 12290
		public CollectionManagerDisplay.ViewMode m_ViewMode;

		// Token: 0x04003003 RID: 12291
		public int m_ColumnCount = 4;

		// Token: 0x04003004 RID: 12292
		public int m_RowCount = 2;

		// Token: 0x04003005 RID: 12293
		public float m_Scale;

		// Token: 0x04003006 RID: 12294
		public float m_ColumnSpacing;

		// Token: 0x04003007 RID: 12295
		public float m_RowSpacing;

		// Token: 0x04003008 RID: 12296
		public Vector3 m_Offset;
	}
}
