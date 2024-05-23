using System;
using UnityEngine;

// Token: 0x0200033F RID: 831
public class ZoneSecret : Zone
{
	// Token: 0x06002B76 RID: 11126 RVA: 0x000D89B0 File Offset: 0x000D6BB0
	private void Awake()
	{
		if (GameState.Get() != null)
		{
			GameState.Get().RegisterGameOverListener(new GameState.GameOverCallback(this.OnGameOver), null);
		}
	}

	// Token: 0x06002B77 RID: 11127 RVA: 0x000D89E0 File Offset: 0x000D6BE0
	public override void UpdateLayout()
	{
		this.m_updatingLayout = true;
		if (base.IsBlockingLayout())
		{
			base.UpdateLayoutFinished();
			return;
		}
		if (UniversalInputManager.UsePhoneUI)
		{
			this.UpdateLayout_Phone();
		}
		else
		{
			this.UpdateLayout_Default();
		}
	}

	// Token: 0x06002B78 RID: 11128 RVA: 0x000D8A28 File Offset: 0x000D6C28
	private void UpdateLayout_Default()
	{
		Vector2 vector;
		vector..ctor(1f, 2f);
		if (this.m_controller != null)
		{
			Card heroCard = this.m_controller.GetHeroCard();
			if (heroCard != null)
			{
				Bounds bounds = heroCard.GetActor().GetMeshRenderer().bounds;
				vector.x = bounds.extents.x;
				vector.y = bounds.extents.z * 0.9f;
			}
		}
		float num = 0.6f * vector.y;
		int num2 = 0;
		for (int i = 0; i < this.m_cards.Count; i++)
		{
			Card card = this.m_cards[i];
			if (this.CanAnimateCard(card))
			{
				card.ShowCard();
				Vector3 position = base.transform.position;
				float num3 = (float)(i + 1 >> 1);
				int num4 = i & 1;
				float num5;
				if (num3 > 2f)
				{
					num5 = 1f;
				}
				else if (object.Equals(num3, 1f))
				{
					num5 = 0.6f;
				}
				else
				{
					num5 = num3 / 2f;
				}
				if (num4 == 0)
				{
					position.x += vector.x * num5;
				}
				else
				{
					position.x -= vector.x * num5;
				}
				position.z -= vector.y * (num5 * num5);
				if (num3 > 2f)
				{
					position.z -= num * (num3 - 2f);
				}
				iTween.Stop(card.gameObject);
				ZoneTransitionStyle transitionStyle = card.GetTransitionStyle();
				card.SetTransitionStyle(ZoneTransitionStyle.NORMAL);
				if (transitionStyle == ZoneTransitionStyle.INSTANT)
				{
					card.EnableTransitioningZones(false);
					card.transform.position = position;
					card.transform.rotation = base.transform.rotation;
					card.transform.localScale = base.transform.localScale;
				}
				else
				{
					card.EnableTransitioningZones(true);
					num2++;
					iTween.MoveTo(card.gameObject, position, 1f);
					iTween.RotateTo(card.gameObject, base.transform.localEulerAngles, 1f);
					iTween.ScaleTo(card.gameObject, base.transform.localScale, 1f);
				}
			}
		}
		if (num2 > 0)
		{
			base.StartFinishLayoutTimer(1f);
		}
		else
		{
			base.UpdateLayoutFinished();
		}
	}

	// Token: 0x06002B79 RID: 11129 RVA: 0x000D8CCC File Offset: 0x000D6ECC
	private void UpdateLayout_Phone()
	{
		for (int i = 0; i < this.m_cards.Count; i++)
		{
			Card card = this.m_cards[i];
			if (this.CanAnimateCard(card))
			{
				card.EnableTransitioningZones(false);
				iTween.Stop(card.gameObject);
				if (i == 0)
				{
					if (!card.IsShown())
					{
						Entity entity = card.GetEntity();
						card.ShowExhaustedChange(entity.IsExhausted());
						card.ShowCard();
					}
					Actor actor = card.GetActor();
					actor.UpdateAllComponents();
				}
				card.transform.position = base.transform.position;
				card.transform.rotation = base.transform.rotation;
				card.transform.localScale = base.transform.localScale;
			}
		}
		base.UpdateLayoutFinished();
	}

	// Token: 0x06002B7A RID: 11130 RVA: 0x000D8DA4 File Offset: 0x000D6FA4
	private bool CanAnimateCard(Card card)
	{
		return !card.IsDoNotSort();
	}

	// Token: 0x06002B7B RID: 11131 RVA: 0x000D8DB4 File Offset: 0x000D6FB4
	private void OnGameOver(object userData)
	{
		Player controller = base.GetController();
		if (controller.GetTag<TAG_PLAYSTATE>(GAME_TAG.PLAYSTATE) == TAG_PLAYSTATE.WON)
		{
			return;
		}
		for (int i = 0; i < this.m_cards.Count; i++)
		{
			Card card = this.m_cards[i];
			if (this.CanAnimateCard(card))
			{
				card.HideCard();
			}
		}
	}

	// Token: 0x04001A55 RID: 6741
	private const float MAX_LAYOUT_PYRAMID_LEVEL = 2f;

	// Token: 0x04001A56 RID: 6742
	private const float LAYOUT_ANIM_SEC = 1f;
}
