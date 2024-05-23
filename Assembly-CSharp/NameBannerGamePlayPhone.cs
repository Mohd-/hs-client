using System;
using System.Collections;
using UnityEngine;

// Token: 0x0200088A RID: 2186
public class NameBannerGamePlayPhone : MonoBehaviour
{
	// Token: 0x06005368 RID: 21352 RVA: 0x0018E398 File Offset: 0x0018C598
	private void Awake()
	{
		OverlayUI.Get().AddGameObject(base.gameObject, CanvasAnchor.CENTER, false, CanvasScaleMode.HEIGHT);
		this.m_playerName.Text = string.Empty;
		this.m_nameBoneToUse = this.m_nameBone;
	}

	// Token: 0x06005369 RID: 21353 RVA: 0x0018E3D4 File Offset: 0x0018C5D4
	private void Update()
	{
		this.UpdateAnchor();
	}

	// Token: 0x0600536A RID: 21354 RVA: 0x0018E3DC File Offset: 0x0018C5DC
	public void SetName(string name)
	{
		this.m_playerName.Text = name;
		Vector3 vector = TransformUtil.ComputeWorldScale(this.m_playerName.gameObject);
		float num = this.FUDGE_FACTOR * vector.x * this.m_playerName.GetTextWorldSpaceBounds().size.x;
		float num2 = this.m_playerName.GetTextWorldSpaceBounds().size.x * vector.x + num;
		float x = this.m_alphaBannerMiddle.GetComponent<Renderer>().bounds.size.x;
		if (num2 > x)
		{
			TransformUtil.SetLocalScaleX(this.m_alphaBanner, num2 / x);
		}
	}

	// Token: 0x0600536B RID: 21355 RVA: 0x0018E494 File Offset: 0x0018C694
	public void FadeIn()
	{
		iTween.FadeTo(this.m_alphaBanner.gameObject, 1f, 1f);
		iTween.FadeTo(this.m_playerName.gameObject, 1f, 1f);
	}

	// Token: 0x0600536C RID: 21356 RVA: 0x0018E4D8 File Offset: 0x0018C6D8
	public void SetPlayerSide(Player.Side side)
	{
		this.m_playerSide = side;
		this.UpdateAnchor();
		base.StartCoroutine(this.UpdateName());
		base.StartCoroutine(this.UpdateUnknownName());
	}

	// Token: 0x0600536D RID: 21357 RVA: 0x0018E50C File Offset: 0x0018C70C
	public Player.Side GetPlayerSide()
	{
		return this.m_playerSide;
	}

	// Token: 0x0600536E RID: 21358 RVA: 0x0018E514 File Offset: 0x0018C714
	public void Unload()
	{
		Object.DestroyImmediate(base.gameObject);
	}

	// Token: 0x0600536F RID: 21359 RVA: 0x0018E521 File Offset: 0x0018C721
	private void UpdateAnchor()
	{
		if (this.m_playerSide == Player.Side.OPPOSING)
		{
		}
	}

	// Token: 0x06005370 RID: 21360 RVA: 0x0018E534 File Offset: 0x0018C734
	private bool IsReady(Player p)
	{
		return p != null && p.GetName() != null;
	}

	// Token: 0x06005371 RID: 21361 RVA: 0x0018E54C File Offset: 0x0018C74C
	private IEnumerator UpdateName()
	{
		while (GameState.Get().GetPlayerMap().Count == 0)
		{
			yield return null;
		}
		Player p = null;
		while (p == null)
		{
			Map<int, Player> playerMap = GameState.Get().GetPlayerMap();
			foreach (Player player in playerMap.Values)
			{
				if (player.GetSide() == this.m_playerSide)
				{
					p = player;
					break;
				}
			}
			yield return null;
		}
		while (p.GetName() == null)
		{
			yield return null;
		}
		this.SetName(p.GetName());
		this.m_nameBoneToUse = this.m_nameBone;
		this.m_alphaBanner.SetActive(true);
		this.m_playerName.transform.position = this.m_nameBoneToUse.position;
		if (GameMgr.Get().IsTutorial())
		{
			yield break;
		}
		while (p.GetHero().GetClass() == TAG_CLASS.INVALID)
		{
			yield return null;
		}
		yield break;
	}

	// Token: 0x06005372 RID: 21362 RVA: 0x0018E568 File Offset: 0x0018C768
	private IEnumerator UpdateUnknownName()
	{
		yield return new WaitForSeconds(5f);
		if (this.m_playerName.Text != string.Empty)
		{
			yield break;
		}
		this.SetName(GameStrings.Get("GAMEPLAY_UNKNOWN_OPPONENT_NAME"));
		yield break;
	}

	// Token: 0x040039A3 RID: 14755
	private const float UNKNOWN_NAME_WAIT = 5f;

	// Token: 0x040039A4 RID: 14756
	public GameObject m_alphaBanner;

	// Token: 0x040039A5 RID: 14757
	public GameObject m_alphaBannerMiddle;

	// Token: 0x040039A6 RID: 14758
	public UberText m_playerName;

	// Token: 0x040039A7 RID: 14759
	public Transform m_nameBone;

	// Token: 0x040039A8 RID: 14760
	public float FUDGE_FACTOR = 0.1915f;

	// Token: 0x040039A9 RID: 14761
	private Player.Side m_playerSide;

	// Token: 0x040039AA RID: 14762
	private Transform m_nameBoneToUse;
}
