using System;
using UnityEngine;

// Token: 0x0200056E RID: 1390
public class MobileFriendListItem : MonoBehaviour, ITouchListItem, ISelectableTouchListItem
{
	// Token: 0x17000479 RID: 1145
	// (get) Token: 0x06003FA2 RID: 16290 RVA: 0x00134DBA File Offset: 0x00132FBA
	// (set) Token: 0x06003FA3 RID: 16291 RVA: 0x00134DC2 File Offset: 0x00132FC2
	public MobileFriendListItem.TypeFlags Type { get; set; }

	// Token: 0x1700047A RID: 1146
	// (get) Token: 0x06003FA4 RID: 16292 RVA: 0x00134DCB File Offset: 0x00132FCB
	public Bounds LocalBounds
	{
		get
		{
			return this.m_localBounds;
		}
	}

	// Token: 0x1700047B RID: 1147
	// (get) Token: 0x06003FA5 RID: 16293 RVA: 0x00134DD3 File Offset: 0x00132FD3
	public bool Selectable
	{
		get
		{
			return this.Type == MobileFriendListItem.TypeFlags.Friend || this.Type == MobileFriendListItem.TypeFlags.NearbyPlayer;
		}
	}

	// Token: 0x06003FA6 RID: 16294 RVA: 0x00134DEF File Offset: 0x00132FEF
	public void SetParent(ITouchListItem parent)
	{
		this.m_parent = parent;
	}

	// Token: 0x06003FA7 RID: 16295 RVA: 0x00134DF8 File Offset: 0x00132FF8
	public void SetShowObject(GameObject showobj)
	{
		this.m_showObject = showobj;
	}

	// Token: 0x1700047C RID: 1148
	// (get) Token: 0x06003FA8 RID: 16296 RVA: 0x00134E01 File Offset: 0x00133001
	public bool IsHeader
	{
		get
		{
			return this.m_parent == null;
		}
	}

	// Token: 0x1700047D RID: 1149
	// (get) Token: 0x06003FA9 RID: 16297 RVA: 0x00134E0C File Offset: 0x0013300C
	// (set) Token: 0x06003FAA RID: 16298 RVA: 0x00134E27 File Offset: 0x00133027
	public bool Visible
	{
		get
		{
			return this.m_parent != null && this.m_parent.Visible;
		}
		set
		{
			if (this.m_showObject == null)
			{
				return;
			}
			if (value != this.m_showObject.activeSelf)
			{
				this.m_showObject.SetActive(value);
			}
		}
	}

	// Token: 0x06003FAB RID: 16299 RVA: 0x00134E58 File Offset: 0x00133058
	private void Awake()
	{
		Transform parent = base.transform.parent;
		TransformProps transformProps = new TransformProps();
		TransformUtil.CopyWorld(transformProps, base.transform);
		base.transform.parent = null;
		TransformUtil.Identity(base.transform);
		this.m_localBounds = this.ComputeWorldBounds();
		base.transform.parent = parent;
		TransformUtil.CopyWorld(base.transform, transformProps);
	}

	// Token: 0x06003FAC RID: 16300 RVA: 0x00134EC0 File Offset: 0x001330C0
	public bool IsSelected()
	{
		FriendListUIElement component = base.GetComponent<FriendListUIElement>();
		return component != null && component.IsSelected();
	}

	// Token: 0x06003FAD RID: 16301 RVA: 0x00134EE8 File Offset: 0x001330E8
	public void Selected()
	{
		FriendListUIElement component = base.GetComponent<FriendListUIElement>();
		if (component != null)
		{
			component.SetSelected(true);
		}
	}

	// Token: 0x06003FAE RID: 16302 RVA: 0x00134F10 File Offset: 0x00133110
	public void Unselected()
	{
		FriendListUIElement component = base.GetComponent<FriendListUIElement>();
		if (component != null)
		{
			component.SetSelected(false);
		}
	}

	// Token: 0x06003FAF RID: 16303 RVA: 0x00134F37 File Offset: 0x00133137
	public Bounds ComputeWorldBounds()
	{
		return TransformUtil.ComputeSetPointBounds(base.gameObject);
	}

	// Token: 0x06003FB0 RID: 16304 RVA: 0x00134F44 File Offset: 0x00133144
	virtual T GetComponent<T>()
	{
		return base.GetComponent<T>();
	}

	// Token: 0x06003FB1 RID: 16305 RVA: 0x00134F4C File Offset: 0x0013314C
	virtual GameObject get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x06003FB2 RID: 16306 RVA: 0x00134F54 File Offset: 0x00133154
	virtual Transform get_transform()
	{
		return base.transform;
	}

	// Token: 0x040028C9 RID: 10441
	private Bounds m_localBounds;

	// Token: 0x040028CA RID: 10442
	private ITouchListItem m_parent;

	// Token: 0x040028CB RID: 10443
	private GameObject m_showObject;

	// Token: 0x0200056F RID: 1391
	[Flags]
	public enum TypeFlags
	{
		// Token: 0x040028CE RID: 10446
		Request = 128,
		// Token: 0x040028CF RID: 10447
		NearbyPlayer = 64,
		// Token: 0x040028D0 RID: 10448
		CurrentGame = 32,
		// Token: 0x040028D1 RID: 10449
		Friend = 16,
		// Token: 0x040028D2 RID: 10450
		Recruit = 8,
		// Token: 0x040028D3 RID: 10451
		Header = 1
	}
}
