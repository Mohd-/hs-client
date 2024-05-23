using System;
using UnityEngine;

// Token: 0x02000A81 RID: 2689
public abstract class CardBackPagingArrowBase : MonoBehaviour
{
	// Token: 0x06005D8C RID: 23948
	public abstract void EnablePaging(bool enable);

	// Token: 0x06005D8D RID: 23949
	public abstract void AddEventListener(UIEventType eventType, UIEvent.Handler handler);
}
