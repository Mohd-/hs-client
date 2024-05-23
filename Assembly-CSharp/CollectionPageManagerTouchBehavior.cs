using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000707 RID: 1799
public class CollectionPageManagerTouchBehavior : PegUICustomBehavior
{
	// Token: 0x060049E4 RID: 18916 RVA: 0x00161B08 File Offset: 0x0015FD08
	protected override void Awake()
	{
		base.Awake();
		CollectionPageManager component = base.GetComponent<CollectionPageManager>();
		this.pageLeftRegion = component.m_pageLeftClickableRegion;
		this.pageRightRegion = component.m_pageRightClickableRegion;
		this.pageDragRegion = component.m_pageDraggableRegion;
		this.pageDragRegion.gameObject.SetActive(true);
		this.pageDragRegion.AddEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OnPageDraggableRegionDown));
	}

	// Token: 0x060049E5 RID: 18917 RVA: 0x00161B70 File Offset: 0x0015FD70
	protected override void OnDestroy()
	{
		this.pageDragRegion.gameObject.SetActive(false);
		this.pageDragRegion.RemoveEventListener(UIEventType.PRESS, new UIEvent.Handler(this.OnPageDraggableRegionDown));
		base.OnDestroy();
	}

	// Token: 0x060049E6 RID: 18918 RVA: 0x00161BB0 File Offset: 0x0015FDB0
	public override bool UpdateUI()
	{
		if (CollectionInputMgr.Get().HasHeldCard() || (CraftingManager.Get() != null && CraftingManager.Get().IsCardShowing()))
		{
			return false;
		}
		bool flag = false;
		if (UniversalInputManager.Get().GetMouseButtonUp(0))
		{
			flag = (this.swipeState == CollectionPageManagerTouchBehavior.SwipeState.Success);
			this.swipeState = CollectionPageManagerTouchBehavior.SwipeState.None;
		}
		return this.swipeState != CollectionPageManagerTouchBehavior.SwipeState.None || flag;
	}

	// Token: 0x060049E7 RID: 18919 RVA: 0x00161C1E File Offset: 0x0015FE1E
	private void OnPageDraggableRegionDown(UIEvent e)
	{
		if (base.gameObject == null)
		{
			return;
		}
		this.TryStartPageTurnGesture();
	}

	// Token: 0x060049E8 RID: 18920 RVA: 0x00161C38 File Offset: 0x0015FE38
	private void TryStartPageTurnGesture()
	{
		if (this.swipeState == CollectionPageManagerTouchBehavior.SwipeState.Update)
		{
			return;
		}
		base.StartCoroutine(this.HandlePageTurnGesture());
	}

	// Token: 0x060049E9 RID: 18921 RVA: 0x00161C54 File Offset: 0x0015FE54
	private Vector2 GetTouchPosition()
	{
		Vector3 touchPosition = W8Touch.Get().GetTouchPosition();
		return new Vector2(touchPosition.x, touchPosition.y);
	}

	// Token: 0x060049EA RID: 18922 RVA: 0x00161C80 File Offset: 0x0015FE80
	private IEnumerator HandlePageTurnGesture()
	{
		if (!UniversalInputManager.Get().IsTouchMode())
		{
			yield return null;
		}
		this.m_swipeStartPosition = this.GetTouchPosition();
		this.swipeState = CollectionPageManagerTouchBehavior.SwipeState.Update;
		float pixelTurnDist = Mathf.Clamp(this.TurnDist * (float)Screen.currentResolution.width, 2f, 300f);
		float pixelDist = 0f;
		PegUIElement touchDownPageTurnRegion = this.HitTestPageTurnRegions();
		while (!UniversalInputManager.Get().GetMouseButtonUp(0))
		{
			Vector2 swipeCurrentPosition = this.GetTouchPosition();
			pixelDist = (swipeCurrentPosition - this.m_swipeStartPosition).x;
			if (pixelDist <= -pixelTurnDist && this.pageRightRegion.enabled)
			{
				this.pageRightRegion.TriggerRelease();
				this.swipeState = CollectionPageManagerTouchBehavior.SwipeState.Success;
				yield break;
			}
			if (pixelDist >= pixelTurnDist && this.pageLeftRegion.enabled)
			{
				this.pageLeftRegion.TriggerRelease();
				this.swipeState = CollectionPageManagerTouchBehavior.SwipeState.Success;
				yield break;
			}
			yield return null;
		}
		if (touchDownPageTurnRegion != null && touchDownPageTurnRegion == this.HitTestPageTurnRegions())
		{
			touchDownPageTurnRegion.TriggerRelease();
		}
		this.swipeState = CollectionPageManagerTouchBehavior.SwipeState.None;
		yield break;
	}

	// Token: 0x060049EB RID: 18923 RVA: 0x00161C9C File Offset: 0x0015FE9C
	private PegUIElement HitTestPageTurnRegions()
	{
		PegUIElement pegUIElement = null;
		this.pageDragRegion.GetComponent<Collider>().enabled = false;
		RaycastHit raycastHit;
		if (UniversalInputManager.Get().GetInputHitInfo(out raycastHit))
		{
			pegUIElement = raycastHit.collider.GetComponent<PegUIElement>();
			if (pegUIElement != this.pageLeftRegion && pegUIElement != this.pageRightRegion)
			{
				pegUIElement = null;
			}
		}
		this.pageDragRegion.GetComponent<Collider>().enabled = true;
		return pegUIElement;
	}

	// Token: 0x0400310B RID: 12555
	private float TurnDist = 0.07f;

	// Token: 0x0400310C RID: 12556
	private PegUIElement pageLeftRegion;

	// Token: 0x0400310D RID: 12557
	private PegUIElement pageRightRegion;

	// Token: 0x0400310E RID: 12558
	private PegUIElement pageDragRegion;

	// Token: 0x0400310F RID: 12559
	private CollectionPageManagerTouchBehavior.SwipeState swipeState;

	// Token: 0x04003110 RID: 12560
	private Vector2 m_swipeStartPosition;

	// Token: 0x020007B6 RID: 1974
	private enum SwipeState
	{
		// Token: 0x040034B7 RID: 13495
		None,
		// Token: 0x040034B8 RID: 13496
		Update,
		// Token: 0x040034B9 RID: 13497
		Success
	}
}
