using System;
using UnityEngine;

// Token: 0x02000843 RID: 2115
public class CardAudio
{
	// Token: 0x06005118 RID: 20760 RVA: 0x0018269D File Offset: 0x0018089D
	public CardAudio(string path)
	{
		this.m_path = path;
	}

	// Token: 0x06005119 RID: 20761 RVA: 0x001826AC File Offset: 0x001808AC
	public AudioSource GetAudio()
	{
		if (this.m_source == null && !string.IsNullOrEmpty(this.m_path))
		{
			GameObject gameObject = AssetLoader.Get().LoadSound(this.m_path, true, false);
			if (gameObject == null)
			{
				Debug.LogErrorFormat("CardAudio.GetAudio: Failed to load audio at {0}.", new object[]
				{
					this.m_path
				});
				return null;
			}
			this.m_source = gameObject.GetComponent<AudioSource>();
		}
		return this.m_source;
	}

	// Token: 0x0600511A RID: 20762 RVA: 0x00182726 File Offset: 0x00180926
	public void Clear()
	{
		Object.Destroy(this.m_source);
	}

	// Token: 0x040037EE RID: 14318
	private string m_path;

	// Token: 0x040037EF RID: 14319
	private AudioSource m_source;
}
