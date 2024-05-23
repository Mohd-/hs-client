using System;
using UnityEngine;

// Token: 0x02000D49 RID: 3401
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PlayMakerFSM))]
public class PlayMakerAnimatorMoveProxy : MonoBehaviour
{
	// Token: 0x14000028 RID: 40
	// (add) Token: 0x06006AF0 RID: 27376 RVA: 0x001F7B69 File Offset: 0x001F5D69
	// (remove) Token: 0x06006AF1 RID: 27377 RVA: 0x001F7B82 File Offset: 0x001F5D82
	public event Action OnAnimatorMoveEvent;

	// Token: 0x06006AF2 RID: 27378 RVA: 0x001F7B9B File Offset: 0x001F5D9B
	private void Start()
	{
	}

	// Token: 0x06006AF3 RID: 27379 RVA: 0x001F7B9D File Offset: 0x001F5D9D
	private void Update()
	{
	}

	// Token: 0x06006AF4 RID: 27380 RVA: 0x001F7B9F File Offset: 0x001F5D9F
	private void OnAnimatorMove()
	{
		if (this.OnAnimatorMoveEvent != null)
		{
			this.OnAnimatorMoveEvent.Invoke();
		}
	}

	// Token: 0x04005355 RID: 21333
	public bool applyRootMotion;
}
