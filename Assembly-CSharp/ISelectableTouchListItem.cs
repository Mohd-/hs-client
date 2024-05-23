using System;

// Token: 0x02000596 RID: 1430
public interface ISelectableTouchListItem : ITouchListItem
{
	// Token: 0x1700049B RID: 1179
	// (get) Token: 0x06004095 RID: 16533
	bool Selectable { get; }

	// Token: 0x06004096 RID: 16534
	bool IsSelected();

	// Token: 0x06004097 RID: 16535
	void Selected();

	// Token: 0x06004098 RID: 16536
	void Unselected();
}
