using System;
using UnityEngine;

// Token: 0x02000D8E RID: 3470
public class SparkScript : MonoBehaviour
{
	// Token: 0x06006C44 RID: 27716 RVA: 0x001FDA50 File Offset: 0x001FBC50
	private void Awake()
	{
		AudioSource component = base.GetComponent<AudioSource>();
		if (Random.value >= 0.5f)
		{
			component.clip = this.clip1;
		}
		else
		{
			component.clip = this.clip2;
		}
	}

	// Token: 0x040054DA RID: 21722
	public AudioClip clip1;

	// Token: 0x040054DB RID: 21723
	public AudioClip clip2;
}
