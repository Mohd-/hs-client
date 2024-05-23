using System;
using UnityEngine;

// Token: 0x02000D83 RID: 3459
public class BranchScript : MonoBehaviour
{
	// Token: 0x06006C26 RID: 27686 RVA: 0x001FC690 File Offset: 0x001FA890
	private void Awake()
	{
		this.timeSpawned = Time.time;
	}

	// Token: 0x06006C27 RID: 27687 RVA: 0x001FC69D File Offset: 0x001FA89D
	private void Update()
	{
	}

	// Token: 0x040054A7 RID: 21671
	public float timeSpawned;
}
