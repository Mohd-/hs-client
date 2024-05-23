using System;
using UnityEngine;

// Token: 0x02000625 RID: 1573
public class BoardStandardGame : MonoBehaviour
{
	// Token: 0x060044C4 RID: 17604 RVA: 0x0014AACE File Offset: 0x00148CCE
	private void Awake()
	{
		BoardStandardGame.s_instance = this;
		if (LoadingScreen.Get() != null)
		{
			LoadingScreen.Get().NotifyMainSceneObjectAwoke(base.gameObject);
		}
	}

	// Token: 0x060044C5 RID: 17605 RVA: 0x0014AAF6 File Offset: 0x00148CF6
	private void Start()
	{
		this.DeckColors();
	}

	// Token: 0x060044C6 RID: 17606 RVA: 0x0014AAFE File Offset: 0x00148CFE
	private void OnDestroy()
	{
		BoardStandardGame.s_instance = null;
	}

	// Token: 0x060044C7 RID: 17607 RVA: 0x0014AB06 File Offset: 0x00148D06
	public static BoardStandardGame Get()
	{
		return BoardStandardGame.s_instance;
	}

	// Token: 0x060044C8 RID: 17608 RVA: 0x0014AB0D File Offset: 0x00148D0D
	public Transform FindBone(string name)
	{
		return this.m_BoneParent.Find(name);
	}

	// Token: 0x060044C9 RID: 17609 RVA: 0x0014AB1C File Offset: 0x00148D1C
	public Collider FindCollider(string name)
	{
		Transform transform = this.m_ColliderParent.Find(name);
		return (!(transform == null)) ? transform.GetComponent<Collider>() : null;
	}

	// Token: 0x060044CA RID: 17610 RVA: 0x0014AB50 File Offset: 0x00148D50
	public void DeckColors()
	{
		foreach (GameObject gameObject in this.m_DeckGameObjects)
		{
			gameObject.GetComponent<Renderer>().material.color = Board.Get().m_DeckColor;
		}
	}

	// Token: 0x04002B9B RID: 11163
	public Transform m_BoneParent;

	// Token: 0x04002B9C RID: 11164
	public Transform m_ColliderParent;

	// Token: 0x04002B9D RID: 11165
	public GameObject[] m_DeckGameObjects;

	// Token: 0x04002B9E RID: 11166
	private static BoardStandardGame s_instance;
}
