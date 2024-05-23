using System;
using UnityEngine;

// Token: 0x0200088C RID: 2188
public class BoardTutorial : MonoBehaviour
{
	// Token: 0x0600537E RID: 21374 RVA: 0x0018E73C File Offset: 0x0018C93C
	private void Awake()
	{
		BoardTutorial.s_instance = this;
		SceneUtils.EnableRenderers(this.m_Highlight, false);
		SceneUtils.EnableRenderers(this.m_EnemyHighlight, false);
		if (LoadingScreen.Get() != null)
		{
			LoadingScreen.Get().NotifyMainSceneObjectAwoke(base.gameObject);
		}
	}

	// Token: 0x0600537F RID: 21375 RVA: 0x0018E787 File Offset: 0x0018C987
	private void OnDestroy()
	{
		BoardTutorial.s_instance = null;
	}

	// Token: 0x06005380 RID: 21376 RVA: 0x0018E78F File Offset: 0x0018C98F
	public static BoardTutorial Get()
	{
		return BoardTutorial.s_instance;
	}

	// Token: 0x06005381 RID: 21377 RVA: 0x0018E796 File Offset: 0x0018C996
	public void EnableHighlight(bool enable)
	{
		if (this.m_highlightEnabled == enable)
		{
			return;
		}
		this.m_highlightEnabled = enable;
		this.UpdateHighlight();
	}

	// Token: 0x06005382 RID: 21378 RVA: 0x0018E7B2 File Offset: 0x0018C9B2
	public void EnableEnemyHighlight(bool enable)
	{
		if (this.m_enemyHighlightEnabled == enable)
		{
			return;
		}
		this.m_enemyHighlightEnabled = enable;
		this.UpdateEnemyHighlight();
	}

	// Token: 0x06005383 RID: 21379 RVA: 0x0018E7CE File Offset: 0x0018C9CE
	public void EnableFullHighlight(bool enable)
	{
		this.EnableHighlight(enable);
		this.EnableEnemyHighlight(enable);
	}

	// Token: 0x06005384 RID: 21380 RVA: 0x0018E7DE File Offset: 0x0018C9DE
	public bool IsHighlightEnabled()
	{
		return this.m_highlightEnabled;
	}

	// Token: 0x06005385 RID: 21381 RVA: 0x0018E7E8 File Offset: 0x0018C9E8
	private void UpdateHighlight()
	{
		if (this.m_highlightEnabled)
		{
			SceneUtils.EnableRenderers(this.m_Highlight, this.m_highlightEnabled);
			this.m_Highlight.GetComponent<Animation>().Play("Glow_PlayArea_Player_On");
		}
		else
		{
			this.m_Highlight.GetComponent<Animation>().Play("Glow_PlayArea_Player_Off");
		}
	}

	// Token: 0x06005386 RID: 21382 RVA: 0x0018E844 File Offset: 0x0018CA44
	private void UpdateEnemyHighlight()
	{
		if (this.m_enemyHighlightEnabled)
		{
			SceneUtils.EnableRenderers(this.m_EnemyHighlight, this.m_enemyHighlightEnabled);
			this.m_EnemyHighlight.GetComponent<Animation>().Play("Glow_PlayArea_Player_On");
		}
		else
		{
			this.m_EnemyHighlight.GetComponent<Animation>().Play("Glow_PlayArea_Player_Off");
		}
	}

	// Token: 0x040039AD RID: 14765
	public GameObject m_Highlight;

	// Token: 0x040039AE RID: 14766
	public GameObject m_EnemyHighlight;

	// Token: 0x040039AF RID: 14767
	public Light m_ManaSpotlight;

	// Token: 0x040039B0 RID: 14768
	private static BoardTutorial s_instance;

	// Token: 0x040039B1 RID: 14769
	private bool m_highlightEnabled;

	// Token: 0x040039B2 RID: 14770
	private bool m_enemyHighlightEnabled;
}
