using System;
using UnityEngine;

// Token: 0x02000F22 RID: 3874
public class PlayParticlesList : MonoBehaviour
{
	// Token: 0x06007379 RID: 29561 RVA: 0x002202C4 File Offset: 0x0021E4C4
	public void PlayParticle(int theIndex)
	{
		if (theIndex < 0 || theIndex > this.m_objects.Length)
		{
			Debug.LogWarning("The index is out of range");
			return;
		}
		if (this.m_objects[theIndex] == null)
		{
			Debug.LogWarningFormat("{0} PlayParticlesList object is null", new object[]
			{
				base.gameObject.name
			});
			return;
		}
		ParticleSystem component = this.m_objects[theIndex].GetComponent<ParticleSystem>();
		component.Play();
	}

	// Token: 0x04005E06 RID: 24070
	public GameObject[] m_objects;
}
