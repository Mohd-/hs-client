using System;
using UnityEngine;

// Token: 0x02000D6F RID: 3439
[RequireComponent(typeof(PlayMakerFSM))]
[RequireComponent(typeof(Animator))]
public class PlayMakerAnimatorIKProxy : MonoBehaviour
{
	// Token: 0x14000029 RID: 41
	// (add) Token: 0x06006BBF RID: 27583 RVA: 0x001FAA75 File Offset: 0x001F8C75
	// (remove) Token: 0x06006BC0 RID: 27584 RVA: 0x001FAA8E File Offset: 0x001F8C8E
	public event Action<int> OnAnimatorIKEvent;

	// Token: 0x06006BC1 RID: 27585 RVA: 0x001FAAA7 File Offset: 0x001F8CA7
	private void OnAnimatorIK(int layerIndex)
	{
		if (this.OnAnimatorIKEvent != null)
		{
			this.OnAnimatorIKEvent.Invoke(layerIndex);
		}
	}

	// Token: 0x0400543E RID: 21566
	private Animator _animator;
}
