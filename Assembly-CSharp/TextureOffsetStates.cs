using System;
using System.Linq;
using UnityEngine;

// Token: 0x02000639 RID: 1593
public class TextureOffsetStates : MonoBehaviour
{
	// Token: 0x06004532 RID: 17714 RVA: 0x0014C6B9 File Offset: 0x0014A8B9
	private void Awake()
	{
		this.m_originalMaterial = base.GetComponent<Renderer>().sharedMaterial;
	}

	// Token: 0x170004BA RID: 1210
	// (get) Token: 0x06004533 RID: 17715 RVA: 0x0014C6CC File Offset: 0x0014A8CC
	// (set) Token: 0x06004534 RID: 17716 RVA: 0x0014C6D4 File Offset: 0x0014A8D4
	public string CurrentState
	{
		get
		{
			return this.m_currentState;
		}
		set
		{
			TextureOffsetState textureOffsetState = Enumerable.FirstOrDefault<TextureOffsetState>(this.m_states, (TextureOffsetState s) => s.Name.Equals(value, 3));
			if (textureOffsetState != null)
			{
				this.m_currentState = value;
				if (textureOffsetState.Material == null)
				{
					base.GetComponent<Renderer>().material = this.m_originalMaterial;
				}
				else
				{
					base.GetComponent<Renderer>().material = textureOffsetState.Material;
				}
				Material material = base.GetComponent<Renderer>().material;
				material.mainTextureOffset = textureOffsetState.Offset;
			}
		}
	}

	// Token: 0x04002C05 RID: 11269
	public TextureOffsetState[] m_states;

	// Token: 0x04002C06 RID: 11270
	private string m_currentState;

	// Token: 0x04002C07 RID: 11271
	private Material m_originalMaterial;
}
