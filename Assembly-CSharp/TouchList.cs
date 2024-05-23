using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000558 RID: 1368
[RequireComponent(typeof(BoxCollider))]
public class TouchList : PegUIElement, IEnumerable, IList<ITouchListItem>, ICollection<ITouchListItem>, IEnumerable<ITouchListItem>
{
	// Token: 0x1400001F RID: 31
	// (add) Token: 0x06003E94 RID: 16020 RVA: 0x0012EAC4 File Offset: 0x0012CCC4
	// (remove) Token: 0x06003E95 RID: 16021 RVA: 0x0012EADD File Offset: 0x0012CCDD
	public event Action Scrolled;

	// Token: 0x14000020 RID: 32
	// (add) Token: 0x06003E96 RID: 16022 RVA: 0x0012EAF6 File Offset: 0x0012CCF6
	// (remove) Token: 0x06003E97 RID: 16023 RVA: 0x0012EB0F File Offset: 0x0012CD0F
	public event TouchList.SelectedIndexChangingEvent SelectedIndexChanging;

	// Token: 0x14000021 RID: 33
	// (add) Token: 0x06003E98 RID: 16024 RVA: 0x0012EB28 File Offset: 0x0012CD28
	// (remove) Token: 0x06003E99 RID: 16025 RVA: 0x0012EB41 File Offset: 0x0012CD41
	public event TouchList.ScrollingEnabledChangedEvent ScrollingEnabledChanged;

	// Token: 0x14000022 RID: 34
	// (add) Token: 0x06003E9A RID: 16026 RVA: 0x0012EB5A File Offset: 0x0012CD5A
	// (remove) Token: 0x06003E9B RID: 16027 RVA: 0x0012EB73 File Offset: 0x0012CD73
	public event Action ClipSizeChanged;

	// Token: 0x14000023 RID: 35
	// (add) Token: 0x06003E9C RID: 16028 RVA: 0x0012EB8C File Offset: 0x0012CD8C
	// (remove) Token: 0x06003E9D RID: 16029 RVA: 0x0012EBA5 File Offset: 0x0012CDA5
	public event TouchList.ItemDragEvent ItemDragStarted;

	// Token: 0x14000024 RID: 36
	// (add) Token: 0x06003E9E RID: 16030 RVA: 0x0012EBBE File Offset: 0x0012CDBE
	// (remove) Token: 0x06003E9F RID: 16031 RVA: 0x0012EBD7 File Offset: 0x0012CDD7
	public event TouchList.ItemDragEvent ItemDragged;

	// Token: 0x14000025 RID: 37
	// (add) Token: 0x06003EA0 RID: 16032 RVA: 0x0012EBF0 File Offset: 0x0012CDF0
	// (remove) Token: 0x06003EA1 RID: 16033 RVA: 0x0012EC09 File Offset: 0x0012CE09
	public event TouchList.ItemDragEvent ItemDragFinished;

	// Token: 0x06003EA2 RID: 16034 RVA: 0x0012EC22 File Offset: 0x0012CE22
	IEnumerator IEnumerable.GetEnumerator()
	{
		this.EnforceInitialized();
		return this.items.GetEnumerator();
	}

	// Token: 0x1700045D RID: 1117
	// (get) Token: 0x06003EA3 RID: 16035 RVA: 0x0012EC3A File Offset: 0x0012CE3A
	public int Count
	{
		get
		{
			this.EnforceInitialized();
			return this.items.Count;
		}
	}

	// Token: 0x1700045E RID: 1118
	// (get) Token: 0x06003EA4 RID: 16036 RVA: 0x0012EC4D File Offset: 0x0012CE4D
	public bool IsReadOnly
	{
		get
		{
			this.EnforceInitialized();
			return false;
		}
	}

	// Token: 0x1700045F RID: 1119
	// (get) Token: 0x06003EA5 RID: 16037 RVA: 0x0012EC56 File Offset: 0x0012CE56
	public bool IsInitialized
	{
		get
		{
			return this.content != null;
		}
	}

	// Token: 0x17000460 RID: 1120
	// (get) Token: 0x06003EA6 RID: 16038 RVA: 0x0012EC64 File Offset: 0x0012CE64
	// (set) Token: 0x06003EA7 RID: 16039 RVA: 0x0012EC74 File Offset: 0x0012CE74
	public TouchList.ILongListBehavior LongListBehavior
	{
		get
		{
			this.EnforceInitialized();
			return this.longListBehavior;
		}
		set
		{
			this.EnforceInitialized();
			if (value == this.longListBehavior)
			{
				return;
			}
			this.allowModification = true;
			this.Clear();
			if (this.longListBehavior != null)
			{
				this.longListBehavior.ReleaseAllItems();
			}
			this.longListBehavior = value;
			if (this.longListBehavior != null)
			{
				this.RefreshList(0, false);
				this.allowModification = false;
			}
		}
	}

	// Token: 0x17000461 RID: 1121
	// (get) Token: 0x06003EA8 RID: 16040 RVA: 0x0012ECD8 File Offset: 0x0012CED8
	// (set) Token: 0x06003EA9 RID: 16041 RVA: 0x0012ED98 File Offset: 0x0012CF98
	public float ScrollValue
	{
		get
		{
			this.EnforceInitialized();
			float scrollableAmount = this.ScrollableAmount;
			float num = (scrollableAmount <= 0f) ? 0f : Mathf.Clamp01(-this.content.transform.localPosition[this.layoutDimension1] / scrollableAmount);
			if (num == 0f || num == 1f)
			{
				float outOfBoundsDist = this.GetOutOfBoundsDist(this.content.transform.localPosition[this.layoutDimension1]);
				return -outOfBoundsDist / Mathf.Max(this.contentSize, this.ClipSize[this.GetVector2Dimension(this.layoutDimension1)]) + num;
			}
			return num;
		}
		set
		{
			this.EnforceInitialized();
			Vector3? vector = this.dragBeginOffsetFromContent;
			if (vector == null && !Mathf.Approximately(this.ScrollValue, value))
			{
				float scrollableAmount = this.ScrollableAmount;
				Vector3 localPosition = this.content.transform.localPosition;
				localPosition[this.layoutDimension1] = -Mathf.Clamp01(value) * scrollableAmount;
				this.content.transform.localPosition = localPosition;
				float num = localPosition[this.layoutDimension1] - this.lastContentPosition;
				if (num != 0f)
				{
					this.PreBufferLongListItems(num < 0f);
				}
				this.lastContentPosition = localPosition[this.layoutDimension1];
				this.FixUpScrolling();
				this.OnScrolled();
			}
		}
	}

	// Token: 0x06003EAA RID: 16042 RVA: 0x0012EE60 File Offset: 0x0012D060
	private void FixUpScrolling()
	{
		if (this.longListBehavior == null || this.items.Count <= 0)
		{
			return;
		}
		Bounds bounds = this.CalculateLocalClipBounds();
		ITouchListItem touchListItem = this.items[0];
		TouchList.ItemInfo itemInfo = this.itemInfos[touchListItem];
		if (itemInfo.LongListIndex == 0 && !this.CanScrollBehind)
		{
			float num = bounds.min[this.layoutDimension1];
			Vector3 min = itemInfo.Min;
			if (min[this.layoutDimension1] != num)
			{
				Vector3 zero = Vector3.zero;
				zero[this.layoutDimension1] = num - min[this.layoutDimension1];
				for (int i = 0; i < this.items.Count; i++)
				{
					touchListItem = this.items[i];
					touchListItem.gameObject.transform.Translate(-zero);
				}
			}
		}
		else if (this.items.Count > 1 && !this.CanScrollAhead)
		{
			float num2 = bounds.max[this.layoutDimension1];
			touchListItem = this.items[this.items.Count - 1];
			itemInfo = this.itemInfos[touchListItem];
			if (itemInfo.LongListIndex >= this.longListBehavior.AllItemsCount - 1)
			{
				Vector3 max = itemInfo.Max;
				if (max[this.layoutDimension1] != num2)
				{
					Vector3 zero2 = Vector3.zero;
					zero2[this.layoutDimension1] = num2 - max[this.layoutDimension1];
					for (int j = 0; j < this.items.Count; j++)
					{
						touchListItem = this.items[j];
						touchListItem.gameObject.transform.Translate(-zero2);
					}
				}
			}
		}
	}

	// Token: 0x17000462 RID: 1122
	// (get) Token: 0x06003EAB RID: 16043 RVA: 0x0012F058 File Offset: 0x0012D258
	public float ScrollableAmount
	{
		get
		{
			if (this.longListBehavior == null)
			{
				return this.excessContentSize;
			}
			return Mathf.Max(0f, this.m_fullListContentSize - this.ClipSize[this.GetVector2Dimension(this.layoutDimension1)]);
		}
	}

	// Token: 0x17000463 RID: 1123
	// (get) Token: 0x06003EAC RID: 16044 RVA: 0x0012F0A4 File Offset: 0x0012D2A4
	public bool CanScrollAhead
	{
		get
		{
			if (this.ScrollValue < 1f)
			{
				return true;
			}
			if (this.longListBehavior != null && this.items.Count > 0)
			{
				ITouchListItem key = Enumerable.Last<ITouchListItem>(this.items);
				TouchList.ItemInfo itemInfo = this.itemInfos[key];
				for (int i = itemInfo.LongListIndex + 1; i < this.longListBehavior.AllItemsCount; i++)
				{
					if (this.longListBehavior.IsItemShowable(i))
					{
						return true;
					}
				}
			}
			return false;
		}
	}

	// Token: 0x17000464 RID: 1124
	// (get) Token: 0x06003EAD RID: 16045 RVA: 0x0012F130 File Offset: 0x0012D330
	public bool CanScrollBehind
	{
		get
		{
			if (this.ScrollValue > 0f)
			{
				return true;
			}
			if (this.longListBehavior != null && this.items.Count > 0)
			{
				ITouchListItem key = Enumerable.First<ITouchListItem>(this.items);
				TouchList.ItemInfo itemInfo = this.itemInfos[key];
				if (this.longListBehavior.AllItemsCount > 0)
				{
					for (int i = itemInfo.LongListIndex - 1; i >= 0; i--)
					{
						if (this.longListBehavior.IsItemShowable(i))
						{
							return true;
						}
					}
				}
			}
			return false;
		}
	}

	// Token: 0x17000465 RID: 1125
	// (get) Token: 0x06003EAE RID: 16046 RVA: 0x0012F1C4 File Offset: 0x0012D3C4
	// (set) Token: 0x06003EAF RID: 16047 RVA: 0x0012F1FC File Offset: 0x0012D3FC
	public float ViewWindowMinValue
	{
		get
		{
			float num = this.content.transform.localPosition[this.layoutDimension1];
			return -num / this.contentSize;
		}
		set
		{
			Vector3 localPosition = this.content.transform.localPosition;
			localPosition[this.layoutDimension1] = -Mathf.Clamp01(value) * this.contentSize;
			this.content.transform.localPosition = localPosition;
			float num = this.content.transform.localPosition[this.layoutDimension1] - this.lastContentPosition;
			if (num != 0f)
			{
				this.PreBufferLongListItems(num < 0f);
			}
			this.lastContentPosition = localPosition[this.layoutDimension1];
			this.OnScrolled();
		}
	}

	// Token: 0x17000466 RID: 1126
	// (get) Token: 0x06003EB0 RID: 16048 RVA: 0x0012F2A0 File Offset: 0x0012D4A0
	// (set) Token: 0x06003EB1 RID: 16049 RVA: 0x0012F2F4 File Offset: 0x0012D4F4
	public float ViewWindowMaxValue
	{
		get
		{
			float num = this.content.transform.localPosition[this.layoutDimension1];
			float num2 = this.ClipSize[this.GetVector2Dimension(this.layoutDimension1)];
			return (-num + num2) / this.contentSize;
		}
		set
		{
			Vector3 localPosition = this.content.transform.localPosition;
			localPosition[this.layoutDimension1] = -Mathf.Clamp01(value) * this.contentSize + this.ClipSize[this.GetVector2Dimension(this.layoutDimension1)];
			this.content.transform.localPosition = localPosition;
			float num = this.content.transform.localPosition[this.layoutDimension1] - this.lastContentPosition;
			if (num != 0f)
			{
				this.PreBufferLongListItems(num < 0f);
			}
			this.lastContentPosition = localPosition[this.layoutDimension1];
			this.OnScrolled();
		}
	}

	// Token: 0x17000467 RID: 1127
	// (get) Token: 0x06003EB2 RID: 16050 RVA: 0x0012F3B4 File Offset: 0x0012D5B4
	// (set) Token: 0x06003EB3 RID: 16051 RVA: 0x0012F414 File Offset: 0x0012D614
	public Vector2 ClipSize
	{
		get
		{
			this.EnforceInitialized();
			BoxCollider boxCollider = base.GetComponent<Collider>() as BoxCollider;
			return new Vector2(boxCollider.size.x, (this.layoutPlane != TouchList.LayoutPlane.XY) ? boxCollider.size.z : boxCollider.size.y);
		}
		set
		{
			this.EnforceInitialized();
			BoxCollider boxCollider = base.GetComponent<Collider>() as BoxCollider;
			Vector3 vector;
			vector..ctor(value.x, 0f, 0f);
			vector[1] = ((this.layoutPlane != TouchList.LayoutPlane.XY) ? boxCollider.size.y : value.y);
			vector[2] = ((this.layoutPlane != TouchList.LayoutPlane.XZ) ? boxCollider.size.z : value.y);
			Vector3 vector2 = VectorUtils.Abs(boxCollider.size - vector);
			if (vector2.x <= 0.0001f && vector2.y <= 0.0001f && vector2.z <= 0.0001f)
			{
				return;
			}
			boxCollider.size = vector;
			this.UpdateBackgroundBounds();
			if (this.longListBehavior == null)
			{
				this.RepositionItems(0, default(Vector3?));
			}
			else
			{
				this.RefreshList(0, true);
			}
			if (this.ClipSizeChanged != null)
			{
				this.ClipSizeChanged.Invoke();
			}
		}
	}

	// Token: 0x17000468 RID: 1128
	// (get) Token: 0x06003EB4 RID: 16052 RVA: 0x0012F53C File Offset: 0x0012D73C
	// (set) Token: 0x06003EB5 RID: 16053 RVA: 0x0012F560 File Offset: 0x0012D760
	public bool SelectionEnabled
	{
		get
		{
			this.EnforceInitialized();
			int? num = this.selection;
			return num != null;
		}
		set
		{
			this.EnforceInitialized();
			if (value == this.SelectionEnabled)
			{
				return;
			}
			if (value)
			{
				this.selection = new int?(-1);
			}
			else
			{
				this.selection = default(int?);
			}
		}
	}

	// Token: 0x17000469 RID: 1129
	// (get) Token: 0x06003EB6 RID: 16054 RVA: 0x0012F5A8 File Offset: 0x0012D7A8
	// (set) Token: 0x06003EB7 RID: 16055 RVA: 0x0012F5E0 File Offset: 0x0012D7E0
	public int SelectedIndex
	{
		get
		{
			this.EnforceInitialized();
			int? num = this.selection;
			return (num == null) ? -1 : this.selection.Value;
		}
		set
		{
			this.EnforceInitialized();
			if (this.SelectionEnabled)
			{
				if (value == this.selection)
				{
					return;
				}
				if (this.SelectedIndexChanging != null && !this.SelectedIndexChanging(value))
				{
					return;
				}
				ISelectableTouchListItem selectableTouchListItem = this.SelectedItem as ISelectableTouchListItem;
				ITouchListItem touchListItem2;
				if (value != -1)
				{
					ITouchListItem touchListItem = this[value];
					touchListItem2 = touchListItem;
				}
				else
				{
					touchListItem2 = null;
				}
				ISelectableTouchListItem selectableTouchListItem2 = touchListItem2 as ISelectableTouchListItem;
				if (value == -1 || (selectableTouchListItem2 != null && selectableTouchListItem2.Selectable))
				{
					this.selection = new int?(value);
				}
				if (selectableTouchListItem != null && this.selection == value)
				{
					selectableTouchListItem.Unselected();
				}
				if (this.selection == value && selectableTouchListItem2 != null)
				{
					selectableTouchListItem2.Selected();
					this.ScrollToItem(selectableTouchListItem2);
				}
			}
		}
	}

	// Token: 0x1700046A RID: 1130
	// (get) Token: 0x06003EB8 RID: 16056 RVA: 0x0012F6EC File Offset: 0x0012D8EC
	// (set) Token: 0x06003EB9 RID: 16057 RVA: 0x0012F73C File Offset: 0x0012D93C
	public ITouchListItem SelectedItem
	{
		get
		{
			this.EnforceInitialized();
			int? num = this.selection;
			ITouchListItem result;
			if (num != null && this.selection.Value != -1)
			{
				ITouchListItem touchListItem = this[this.selection.Value];
				result = touchListItem;
			}
			else
			{
				result = null;
			}
			return result;
		}
		set
		{
			this.EnforceInitialized();
			int num = this.items.IndexOf(value);
			if (num != -1)
			{
				this.SelectedIndex = num;
			}
		}
	}

	// Token: 0x1700046B RID: 1131
	public ITouchListItem this[int index]
	{
		get
		{
			return this.items[index];
		}
		set
		{
			if (this.allowModification)
			{
				this.items[index] = value;
			}
		}
	}

	// Token: 0x06003EBC RID: 16060 RVA: 0x0012F792 File Offset: 0x0012D992
	public void Add(ITouchListItem item)
	{
		this.Add(item, true);
	}

	// Token: 0x06003EBD RID: 16061 RVA: 0x0012F79C File Offset: 0x0012D99C
	public void Add(ITouchListItem item, bool repositionItems)
	{
		this.EnforceInitialized();
		if (!this.allowModification)
		{
			return;
		}
		this.items.Add(item);
		Vector3 negatedScale = this.GetNegatedScale(item.transform.localScale);
		item.transform.parent = this.content.transform;
		item.transform.localPosition = Vector3.zero;
		item.transform.localRotation = Quaternion.identity;
		if (this.orientation == TouchList.Orientation.Vertical)
		{
			item.transform.localScale = negatedScale;
		}
		this.itemInfos[item] = new TouchList.ItemInfo(item, this.layoutPlane);
		item.gameObject.SetActive(false);
		if (this.selection == -1 && item is ISelectableTouchListItem && ((ISelectableTouchListItem)item).IsSelected())
		{
			this.selection = new int?(this.items.Count - 1);
		}
		if (repositionItems)
		{
			this.RepositionItems(this.items.Count - 1, default(Vector3?));
			this.RecalculateLongListContentSize(true);
		}
	}

	// Token: 0x06003EBE RID: 16062 RVA: 0x0012F8C8 File Offset: 0x0012DAC8
	public void Clear()
	{
		this.EnforceInitialized();
		if (!this.allowModification)
		{
			return;
		}
		foreach (ITouchListItem touchListItem in this.items)
		{
			Vector3 negatedScale = this.GetNegatedScale(touchListItem.transform.localScale);
			touchListItem.transform.parent = null;
			if (this.orientation == TouchList.Orientation.Vertical)
			{
				touchListItem.transform.localScale = negatedScale;
			}
		}
		this.content.transform.localPosition = Vector3.zero;
		this.lastContentPosition = 0f;
		this.items.Clear();
		this.RecalculateSize();
		this.UpdateBackgroundScroll();
		this.RecalculateLongListContentSize(true);
		if (this.SelectionEnabled)
		{
			this.SelectedIndex = -1;
		}
		if (this.m_hoveredOverItem != null)
		{
			this.m_hoveredOverItem.TriggerOut();
			this.m_hoveredOverItem = null;
		}
	}

	// Token: 0x06003EBF RID: 16063 RVA: 0x0012F9D8 File Offset: 0x0012DBD8
	public bool Contains(ITouchListItem item)
	{
		this.EnforceInitialized();
		return this.items.Contains(item);
	}

	// Token: 0x06003EC0 RID: 16064 RVA: 0x0012F9EC File Offset: 0x0012DBEC
	public void CopyTo(ITouchListItem[] array, int arrayIndex)
	{
		this.EnforceInitialized();
		this.items.CopyTo(array, arrayIndex);
	}

	// Token: 0x06003EC1 RID: 16065 RVA: 0x0012FA04 File Offset: 0x0012DC04
	private List<ITouchListItem> GetItemsInView()
	{
		this.EnforceInitialized();
		List<ITouchListItem> list = new List<ITouchListItem>();
		float num = this.CalculateLocalClipBounds().max[this.layoutDimension1];
		int numItemsBehindView = this.GetNumItemsBehindView();
		for (int i = numItemsBehindView; i < this.items.Count; i++)
		{
			TouchList.ItemInfo itemInfo = this.itemInfos[this.items[i]];
			if ((itemInfo.Min - this.content.transform.localPosition)[this.layoutDimension1] >= num)
			{
				break;
			}
			list.Add(this.items[i]);
		}
		return list;
	}

	// Token: 0x06003EC2 RID: 16066 RVA: 0x0012FAC4 File Offset: 0x0012DCC4
	public void SetVisibilityOfAllItems()
	{
		if (this.layoutSuspended)
		{
			return;
		}
		this.EnforceInitialized();
		Bounds bounds = this.CalculateLocalClipBounds();
		for (int i = 0; i < this.items.Count; i++)
		{
			ITouchListItem touchListItem = this.items[i];
			bool flag = this.IsItemVisible_Internal(i, ref bounds);
			if (flag != touchListItem.gameObject.activeSelf)
			{
				touchListItem.gameObject.SetActive(flag);
			}
		}
	}

	// Token: 0x06003EC3 RID: 16067 RVA: 0x0012FB3C File Offset: 0x0012DD3C
	private bool IsItemVisible_Internal(int visualizedListIndex, ref Bounds localClipBounds)
	{
		ITouchListItem key = this.items[visualizedListIndex];
		TouchList.ItemInfo itemInfo = this.itemInfos[key];
		Vector3 min = itemInfo.Min;
		Vector3 max = itemInfo.Max;
		return this.IsWithinClipBounds(min, max, ref localClipBounds);
	}

	// Token: 0x06003EC4 RID: 16068 RVA: 0x0012FB84 File Offset: 0x0012DD84
	private bool IsWithinClipBounds(Vector3 localBoundsMin, Vector3 localBoundsMax, ref Bounds localClipBounds)
	{
		float num = localClipBounds.min[this.layoutDimension1];
		float num2 = localClipBounds.max[this.layoutDimension1];
		return localBoundsMax[this.layoutDimension1] >= num && localBoundsMin[this.layoutDimension1] <= num2;
	}

	// Token: 0x06003EC5 RID: 16069 RVA: 0x0012FBE8 File Offset: 0x0012DDE8
	private bool IsItemVisible(int visualizedListIndex)
	{
		Bounds bounds = this.CalculateLocalClipBounds();
		return this.IsItemVisible_Internal(visualizedListIndex, ref bounds);
	}

	// Token: 0x06003EC6 RID: 16070 RVA: 0x0012FC05 File Offset: 0x0012DE05
	public IEnumerator<ITouchListItem> GetEnumerator()
	{
		this.EnforceInitialized();
		return this.items.GetEnumerator();
	}

	// Token: 0x06003EC7 RID: 16071 RVA: 0x0012FC1D File Offset: 0x0012DE1D
	public int IndexOf(ITouchListItem item)
	{
		this.EnforceInitialized();
		return this.items.IndexOf(item);
	}

	// Token: 0x06003EC8 RID: 16072 RVA: 0x0012FC31 File Offset: 0x0012DE31
	public void Insert(int index, ITouchListItem item)
	{
		this.Insert(index, item, true);
	}

	// Token: 0x06003EC9 RID: 16073 RVA: 0x0012FC3C File Offset: 0x0012DE3C
	public void Insert(int index, ITouchListItem item, bool repositionItems)
	{
		this.EnforceInitialized();
		if (!this.allowModification)
		{
			return;
		}
		this.items.Insert(index, item);
		Vector3 negatedScale = this.GetNegatedScale(item.transform.localScale);
		item.transform.parent = this.content.transform;
		item.transform.localPosition = Vector3.zero;
		item.transform.localRotation = Quaternion.identity;
		if (this.orientation == TouchList.Orientation.Vertical)
		{
			item.transform.localScale = negatedScale;
		}
		this.itemInfos[item] = new TouchList.ItemInfo(item, this.layoutPlane);
		if (this.selection == -1 && item is ISelectableTouchListItem && ((ISelectableTouchListItem)item).IsSelected())
		{
			this.selection = new int?(index);
		}
		if (repositionItems)
		{
			this.RepositionItems(index, default(Vector3?));
			this.RecalculateLongListContentSize(true);
		}
	}

	// Token: 0x06003ECA RID: 16074 RVA: 0x0012FD48 File Offset: 0x0012DF48
	public bool Remove(ITouchListItem item)
	{
		this.EnforceInitialized();
		if (!this.allowModification)
		{
			return false;
		}
		int num = this.items.IndexOf(item);
		if (num != -1)
		{
			this.RemoveAt(num, true);
			return true;
		}
		return false;
	}

	// Token: 0x06003ECB RID: 16075 RVA: 0x0012FD87 File Offset: 0x0012DF87
	public void RemoveAt(int index)
	{
		this.RemoveAt(index, true);
	}

	// Token: 0x06003ECC RID: 16076 RVA: 0x0012FD94 File Offset: 0x0012DF94
	public void RemoveAt(int index, bool repositionItems)
	{
		this.EnforceInitialized();
		if (!this.allowModification)
		{
			return;
		}
		Vector3 negatedScale = this.GetNegatedScale(this.items[index].transform.localScale);
		ITouchListItem touchListItem = this.items[index];
		touchListItem.transform.parent = base.transform;
		if (this.orientation == TouchList.Orientation.Vertical)
		{
			this.items[index].transform.localScale = negatedScale;
		}
		this.itemInfos.Remove(this.items[index]);
		this.items.RemoveAt(index);
		if (index == this.selection)
		{
			this.selection = new int?(-1);
		}
		else
		{
			int? num = this.selection;
			if (num != null && index < num.Value)
			{
				int? num2 = this.selection;
				if (num2 != null)
				{
					this.selection = new int?(num2.Value - 1);
				}
			}
		}
		if (this.m_hoveredOverItem != null && touchListItem.GetComponent<PegUIElement>() == this.m_hoveredOverItem)
		{
			this.m_hoveredOverItem.TriggerOut();
			this.m_hoveredOverItem = null;
		}
		if (repositionItems)
		{
			this.RepositionItems(index, default(Vector3?));
			this.RecalculateLongListContentSize(true);
		}
	}

	// Token: 0x06003ECD RID: 16077 RVA: 0x0012FF0B File Offset: 0x0012E10B
	public int FindIndex(Predicate<ITouchListItem> match)
	{
		this.EnforceInitialized();
		return this.items.FindIndex(match);
	}

	// Token: 0x06003ECE RID: 16078 RVA: 0x0012FF20 File Offset: 0x0012E120
	public void Sort(Comparison<ITouchListItem> comparison)
	{
		this.EnforceInitialized();
		ITouchListItem selectedItem = this.SelectedItem;
		this.items.Sort(comparison);
		this.RepositionItems(0, default(Vector3?));
		this.selection = new int?(this.items.IndexOf(selectedItem));
	}

	// Token: 0x1700046C RID: 1132
	// (get) Token: 0x06003ECF RID: 16079 RVA: 0x0012FF6D File Offset: 0x0012E16D
	public bool IsLayoutSuspended
	{
		get
		{
			return this.layoutSuspended;
		}
	}

	// Token: 0x06003ED0 RID: 16080 RVA: 0x0012FF75 File Offset: 0x0012E175
	public void SuspendLayout()
	{
		this.EnforceInitialized();
		this.layoutSuspended = true;
	}

	// Token: 0x06003ED1 RID: 16081 RVA: 0x0012FF84 File Offset: 0x0012E184
	public void ResumeLayout(bool repositionItems = true)
	{
		this.EnforceInitialized();
		this.layoutSuspended = false;
		if (repositionItems)
		{
			this.RepositionItems(0, default(Vector3?));
		}
	}

	// Token: 0x06003ED2 RID: 16082 RVA: 0x0012FFB4 File Offset: 0x0012E1B4
	public void ResetState()
	{
		this.touchBeginScreenPosition = default(Vector2?);
		this.dragBeginOffsetFromContent = default(Vector3?);
		this.dragBeginContentPosition = Vector3.zero;
		this.lastTouchPosition = Vector3.zero;
		this.lastContentPosition = 0f;
		this.dragItemBegin = default(Vector3?);
		if (this.content != null)
		{
			this.content.transform.localPosition = Vector3.zero;
		}
	}

	// Token: 0x06003ED3 RID: 16083 RVA: 0x00130038 File Offset: 0x0012E238
	protected override void Awake()
	{
		base.Awake();
		this.content = new GameObject("Content");
		this.content.transform.parent = base.transform;
		TransformUtil.Identity(this.content.transform);
		this.layoutDimension1 = 0;
		this.layoutDimension2 = ((this.layoutPlane != TouchList.LayoutPlane.XY) ? 2 : 1);
		this.layoutDimension3 = 3 - this.layoutDimension2;
		if (this.orientation == TouchList.Orientation.Vertical)
		{
			GeneralUtils.Swap<int>(ref this.layoutDimension1, ref this.layoutDimension2);
			Vector3 one = Vector3.one;
			one[this.layoutDimension1] = -1f;
			base.transform.localScale = one;
		}
		if (this.background != null)
		{
			if (this.orientation == TouchList.Orientation.Vertical)
			{
				this.background.transform.localScale = this.GetNegatedScale(this.background.transform.localScale);
			}
			this.UpdateBackgroundBounds();
		}
	}

	// Token: 0x06003ED4 RID: 16084 RVA: 0x00130138 File Offset: 0x0012E338
	protected override void OnOver(PegUIElement.InteractionState oldState)
	{
		this.m_isHoveredOverTouchList = true;
		this.OnHover(true);
	}

	// Token: 0x06003ED5 RID: 16085 RVA: 0x00130148 File Offset: 0x0012E348
	protected override void OnOut(PegUIElement.InteractionState oldState)
	{
		this.m_isHoveredOverTouchList = false;
		if (this.m_hoveredOverItem != null)
		{
			this.m_hoveredOverItem.TriggerOut();
			this.m_hoveredOverItem = null;
		}
	}

	// Token: 0x06003ED6 RID: 16086 RVA: 0x00130180 File Offset: 0x0012E380
	private void OnHover(bool isKnownOver)
	{
		if (UniversalInputManager.Get().IsTouchMode())
		{
			return;
		}
		if (this.touchBeginItem == null)
		{
			Vector3? vector = this.dragItemBegin;
			if (vector == null)
			{
				Camera camera = CameraUtils.FindFirstByLayer(base.gameObject.layer);
				if (camera == null)
				{
					if (this.m_hoveredOverItem != null)
					{
						this.m_hoveredOverItem.TriggerOut();
						this.m_hoveredOverItem = null;
					}
					return;
				}
				RaycastHit raycastHit;
				if (!isKnownOver && (!UniversalInputManager.Get().GetInputHitInfo(camera, out raycastHit) || raycastHit.transform != base.transform) && this.m_hoveredOverItem != null)
				{
					this.m_hoveredOverItem.TriggerOut();
					this.m_hoveredOverItem = null;
				}
				base.GetComponent<Collider>().enabled = false;
				PegUIElement pegUIElement = null;
				if (UniversalInputManager.Get().GetInputHitInfo(camera, out raycastHit))
				{
					pegUIElement = raycastHit.transform.GetComponent<PegUIElement>();
				}
				base.GetComponent<Collider>().enabled = true;
				if (pegUIElement != null && this.m_hoveredOverItem != pegUIElement)
				{
					if (this.m_hoveredOverItem != null)
					{
						this.m_hoveredOverItem.TriggerOut();
					}
					pegUIElement.TriggerOver();
					this.m_hoveredOverItem = pegUIElement;
				}
				return;
			}
		}
		if (this.m_hoveredOverItem != null)
		{
			this.m_hoveredOverItem.TriggerOut();
			this.m_hoveredOverItem = null;
		}
	}

	// Token: 0x06003ED7 RID: 16087 RVA: 0x001302F4 File Offset: 0x0012E4F4
	protected override void OnPress()
	{
		Camera camera = CameraUtils.FindFirstByLayer(base.gameObject.layer);
		if (camera == null)
		{
			return;
		}
		this.touchBeginScreenPosition = new Vector2?(UniversalInputManager.Get().GetMousePosition());
		if (this.lastContentPosition != this.content.transform.localPosition[this.layoutDimension1])
		{
			return;
		}
		Vector3 vector = this.GetTouchPosition() - this.content.transform.localPosition;
		for (int i = 0; i < this.items.Count; i++)
		{
			ITouchListItem touchListItem = this.items[i];
			bool flag = touchListItem.IsHeader || touchListItem.Visible;
			if (flag && this.itemInfos[touchListItem].Contains(vector, this.layoutPlane))
			{
				this.touchBeginItem = touchListItem;
				break;
			}
		}
	}

	// Token: 0x06003ED8 RID: 16088 RVA: 0x001303F4 File Offset: 0x0012E5F4
	protected override void OnRelease()
	{
		Camera camera = CameraUtils.FindFirstByLayer(base.gameObject.layer);
		if (camera == null)
		{
			return;
		}
		if (this.touchBeginItem != null)
		{
			Vector3? vector = this.dragItemBegin;
			if (vector == null)
			{
				this.touchBeginScreenPosition = default(Vector2?);
				base.GetComponent<Collider>().enabled = false;
				PegUIElement pegUIElement = null;
				RaycastHit raycastHit;
				if (UniversalInputManager.Get().GetInputHitInfo(camera, out raycastHit))
				{
					pegUIElement = raycastHit.transform.GetComponent<PegUIElement>();
				}
				base.GetComponent<Collider>().enabled = true;
				if (pegUIElement != null)
				{
					pegUIElement.TriggerRelease();
					this.touchBeginItem = null;
				}
			}
		}
	}

	// Token: 0x06003ED9 RID: 16089 RVA: 0x001304A2 File Offset: 0x0012E6A2
	private void EnforceInitialized()
	{
		if (!this.IsInitialized)
		{
			throw new InvalidOperationException("TouchList must be initialized before using it. Please wait for Awake to finish.");
		}
	}

	// Token: 0x06003EDA RID: 16090 RVA: 0x001304BC File Offset: 0x0012E6BC
	private void Update()
	{
		if (UniversalInputManager.Get().IsTouchMode())
		{
			this.UpdateTouchInput();
		}
		else
		{
			this.UpdateMouseInput();
		}
		if (this.m_isHoveredOverTouchList)
		{
			this.OnHover(false);
		}
	}

	// Token: 0x06003EDB RID: 16091 RVA: 0x001304FC File Offset: 0x0012E6FC
	private void UpdateTouchInput()
	{
		Vector3 touchPosition = this.GetTouchPosition();
		if (UniversalInputManager.Get().GetMouseButtonUp(0))
		{
			Vector3? vector = this.dragItemBegin;
			if (vector != null && this.ItemDragFinished != null)
			{
				this.ItemDragFinished(this.touchBeginItem, this.GetItemDragDelta(touchPosition));
				this.dragItemBegin = default(Vector3?);
			}
			this.touchBeginItem = null;
			this.touchBeginScreenPosition = default(Vector2?);
			this.dragBeginOffsetFromContent = default(Vector3?);
		}
		Vector2? vector2 = this.touchBeginScreenPosition;
		if (vector2 != null)
		{
			Func<int, float, bool> func = delegate(int dimension, float inchThreshold)
			{
				int vector2Dimension = this.GetVector2Dimension(dimension);
				float num14 = this.touchBeginScreenPosition.Value[vector2Dimension] - UniversalInputManager.Get().GetMousePosition()[vector2Dimension];
				float num15 = inchThreshold * ((Screen.dpi <= 0f) ? 150f : Screen.dpi);
				return Mathf.Abs(num14) > num15;
			};
			if (this.ItemDragStarted != null && func.Invoke(this.layoutDimension2, 0.05f) && this.ItemDragStarted(this.touchBeginItem, this.GetItemDragDelta(touchPosition)))
			{
				this.dragItemBegin = new Vector3?(this.GetTouchPosition());
				this.touchBeginScreenPosition = default(Vector2?);
			}
			else if (func.Invoke(this.layoutDimension1, 0.05f))
			{
				this.dragBeginContentPosition = this.content.transform.localPosition;
				this.dragBeginOffsetFromContent = new Vector3?(this.dragBeginContentPosition - this.lastTouchPosition);
				this.touchBeginItem = null;
				this.touchBeginScreenPosition = default(Vector2?);
			}
		}
		Vector3? vector3 = this.dragItemBegin;
		float num3;
		if (vector3 != null)
		{
			if (!this.ItemDragged(this.touchBeginItem, this.GetItemDragDelta(touchPosition)))
			{
				this.dragItemBegin = default(Vector3?);
				this.touchBeginItem = null;
			}
		}
		else
		{
			Vector3? vector4 = this.dragBeginOffsetFromContent;
			if (vector4 != null)
			{
				float num = touchPosition[this.layoutDimension1] + this.dragBeginOffsetFromContent.Value[this.layoutDimension1];
				float num2 = this.GetOutOfBoundsDist(num);
				if (num2 != 0f)
				{
					num2 = TouchList.OutOfBoundsDistReducer.Invoke(Mathf.Abs(num2)) * Mathf.Sign(num2);
					num = ((num2 >= 0f) ? num2 : (-this.excessContentSize + num2));
				}
				Vector3 localPosition = this.content.transform.localPosition;
				this.lastContentPosition = localPosition[this.layoutDimension1];
				localPosition[this.layoutDimension1] = num;
				this.content.transform.localPosition = localPosition;
				if (this.lastContentPosition != localPosition[this.layoutDimension1])
				{
					this.OnScrolled();
				}
			}
			else
			{
				float contentPosition = this.content.transform.localPosition[this.layoutDimension1];
				float outOfBoundsDist = this.GetOutOfBoundsDist(contentPosition);
				num3 = this.content.transform.localPosition[this.layoutDimension1] - this.lastContentPosition;
				float num4 = num3 / Time.fixedDeltaTime;
				if (this.maxKineticScrollSpeed > Mathf.Epsilon)
				{
					if (num4 > 0f)
					{
						num4 = Mathf.Min(num4, this.maxKineticScrollSpeed);
					}
					else
					{
						num4 = Mathf.Max(num4, -this.maxKineticScrollSpeed);
					}
				}
				if (outOfBoundsDist != 0f)
				{
					Vector3 localPosition2 = this.content.transform.localPosition;
					this.lastContentPosition = contentPosition;
					float num5 = -400f * outOfBoundsDist - TouchList.ScrollBoundsSpringB * num4;
					float num6 = num4 + num5 * Time.fixedDeltaTime;
					ref Vector3 ptr = ref localPosition2;
					int num8;
					int num7 = num8 = this.layoutDimension1;
					float num9 = ptr[num8];
					localPosition2[num7] = num9 + num6 * Time.fixedDeltaTime;
					if (Mathf.Abs(this.GetOutOfBoundsDist(localPosition2[this.layoutDimension1])) < 0.05f)
					{
						float num10 = (Mathf.Abs(localPosition2[this.layoutDimension1] + this.excessContentSize) >= Mathf.Abs(localPosition2[this.layoutDimension1])) ? 0f : (-this.excessContentSize);
						localPosition2[this.layoutDimension1] = num10;
						this.lastContentPosition = num10;
					}
					this.content.transform.localPosition = localPosition2;
					this.OnScrolled();
				}
				else if (num4 != 0f)
				{
					this.lastContentPosition = this.content.transform.localPosition[this.layoutDimension1];
					float num11 = -Mathf.Sign(num4) * 10000f;
					float num12 = num4 + num11 * Time.fixedDeltaTime;
					if (Mathf.Abs(num12) >= 0.01f && Mathf.Sign(num12) == Mathf.Sign(num4))
					{
						Vector3 localPosition3 = this.content.transform.localPosition;
						ref Vector3 ptr2 = ref localPosition3;
						int num8;
						int num13 = num8 = this.layoutDimension1;
						float num9 = ptr2[num8];
						localPosition3[num13] = num9 + num12 * Time.fixedDeltaTime;
						this.content.transform.localPosition = localPosition3;
						this.OnScrolled();
					}
				}
			}
		}
		num3 = this.content.transform.localPosition[this.layoutDimension1] - this.lastContentPosition;
		if (num3 != 0f)
		{
			this.PreBufferLongListItems(num3 < 0f);
		}
		this.lastTouchPosition = touchPosition;
	}

	// Token: 0x06003EDC RID: 16092 RVA: 0x00130A74 File Offset: 0x0012EC74
	private void PreBufferLongListItems(bool scrolledAhead)
	{
		if (this.LongListBehavior == null)
		{
			return;
		}
		this.allowModification = true;
		if (scrolledAhead && this.GetNumItemsAheadOfView() < this.longListBehavior.MinBuffer)
		{
			bool flag = this.CanScrollAhead;
			if (this.items.Count > 0)
			{
				Bounds bounds = this.CalculateLocalClipBounds();
				ITouchListItem key = this.items[this.items.Count - 1];
				TouchList.ItemInfo itemInfo = this.itemInfos[key];
				Vector3 max = itemInfo.Max;
				float num = bounds.min[this.layoutDimension1];
				if (max[this.layoutDimension1] < num)
				{
					this.RefreshList(0, true);
					flag = false;
				}
			}
			if (flag)
			{
				this.LoadAhead();
			}
		}
		else if (!scrolledAhead && this.GetNumItemsBehindView() < this.longListBehavior.MinBuffer)
		{
			bool flag2 = this.CanScrollBehind;
			if (this.items.Count > 0)
			{
				Bounds bounds2 = this.CalculateLocalClipBounds();
				ITouchListItem key2 = this.items[0];
				TouchList.ItemInfo itemInfo2 = this.itemInfos[key2];
				Vector3 min = itemInfo2.Min;
				float num2 = bounds2.max[this.layoutDimension1];
				if (min[this.layoutDimension1] > num2)
				{
					this.RefreshList(0, true);
					flag2 = false;
				}
			}
			if (flag2)
			{
				this.LoadBehind();
			}
		}
		this.allowModification = false;
	}

	// Token: 0x06003EDD RID: 16093 RVA: 0x00130BF4 File Offset: 0x0012EDF4
	private void UpdateMouseInput()
	{
		Camera camera = CameraUtils.FindFirstByLayer(base.gameObject.layer);
		if (camera == null)
		{
			return;
		}
		Ray ray = camera.ScreenPointToRay(UniversalInputManager.Get().GetMousePosition());
		RaycastHit raycastHit;
		if (!base.GetComponent<Collider>().Raycast(ray, ref raycastHit, camera.farClipPlane))
		{
			return;
		}
		float num = 0f;
		if (Input.GetAxis("Mouse ScrollWheel") < 0f && this.CanScrollAhead)
		{
			num -= this.scrollWheelIncrement;
		}
		if (Input.GetAxis("Mouse ScrollWheel") > 0f && this.CanScrollBehind)
		{
			num += this.scrollWheelIncrement;
		}
		if (Mathf.Abs(num) > Mathf.Epsilon)
		{
			float num2 = this.content.transform.localPosition[this.layoutDimension1] + num;
			if (num2 <= -this.excessContentSize)
			{
				num2 = -this.excessContentSize;
			}
			else if (num2 >= 0f)
			{
				num2 = 0f;
			}
			Vector3 localPosition = this.content.transform.localPosition;
			this.lastContentPosition = localPosition[this.layoutDimension1];
			localPosition[this.layoutDimension1] = num2;
			this.content.transform.localPosition = localPosition;
			float num3 = this.content.transform.localPosition[this.layoutDimension1] - this.lastContentPosition;
			this.lastContentPosition = this.content.transform.localPosition[this.layoutDimension1];
			if (num3 != 0f)
			{
				this.PreBufferLongListItems(num3 < 0f);
			}
			this.FixUpScrolling();
			this.OnScrolled();
		}
	}

	// Token: 0x06003EDE RID: 16094 RVA: 0x00130DBC File Offset: 0x0012EFBC
	private float GetOutOfBoundsDist(float contentPosition)
	{
		float result = 0f;
		if (contentPosition < -this.excessContentSize)
		{
			result = contentPosition + this.excessContentSize;
		}
		else if (contentPosition > 0f)
		{
			result = contentPosition;
		}
		return result;
	}

	// Token: 0x06003EDF RID: 16095 RVA: 0x00130DF8 File Offset: 0x0012EFF8
	private void ScrollToItem(ITouchListItem item)
	{
		Bounds bounds = this.CalculateLocalClipBounds();
		TouchList.ItemInfo itemInfo = this.itemInfos[item];
		float num = itemInfo.Max[this.layoutDimension1] - bounds.max[this.layoutDimension1];
		if (num > 0f)
		{
			Vector3 zero = Vector3.zero;
			zero[this.layoutDimension1] = num;
			this.content.transform.Translate(zero);
			this.lastContentPosition = this.content.transform.localPosition[this.layoutDimension1];
			this.PreBufferLongListItems(true);
			this.OnScrolled();
		}
		float num2 = bounds.min[this.layoutDimension1] - itemInfo.Min[this.layoutDimension1];
		if (num2 > 0f)
		{
			Vector3 zero2 = Vector3.zero;
			zero2[this.layoutDimension1] = -num2;
			this.content.transform.Translate(zero2);
			this.lastContentPosition = this.content.transform.localPosition[this.layoutDimension1];
			this.PreBufferLongListItems(false);
			this.OnScrolled();
		}
	}

	// Token: 0x06003EE0 RID: 16096 RVA: 0x00130F40 File Offset: 0x0012F140
	private void OnScrolled()
	{
		this.UpdateBackgroundScroll();
		this.SetVisibilityOfAllItems();
		if (this.Scrolled != null)
		{
			this.Scrolled.Invoke();
		}
	}

	// Token: 0x06003EE1 RID: 16097 RVA: 0x00130F70 File Offset: 0x0012F170
	private Vector3 GetTouchPosition()
	{
		Camera camera = CameraUtils.FindFirstByLayer(base.gameObject.layer);
		if (camera == null)
		{
			return Vector3.zero;
		}
		float num = Vector3.Distance(camera.transform.position, base.GetComponent<Collider>().bounds.min);
		float num2 = Vector3.Distance(camera.transform.position, base.GetComponent<Collider>().bounds.max);
		Vector3 vector = (num >= num2) ? base.GetComponent<Collider>().bounds.max : base.GetComponent<Collider>().bounds.min;
		Plane plane;
		plane..ctor(-camera.transform.forward, vector);
		Ray ray = camera.ScreenPointToRay(UniversalInputManager.Get().GetMousePosition());
		float num3;
		plane.Raycast(ray, ref num3);
		return base.transform.InverseTransformPoint(ray.GetPoint(num3));
	}

	// Token: 0x06003EE2 RID: 16098 RVA: 0x0013106C File Offset: 0x0012F26C
	private float GetItemDragDelta(Vector3 touchPosition)
	{
		Vector3? vector = this.dragItemBegin;
		if (vector != null)
		{
			return touchPosition[this.layoutDimension2] - this.dragItemBegin.Value[this.layoutDimension2];
		}
		return 0f;
	}

	// Token: 0x06003EE3 RID: 16099 RVA: 0x001310BC File Offset: 0x0012F2BC
	private void LoadAhead()
	{
		bool flag = this.allowModification;
		bool flag2 = this.layoutSuspended;
		this.allowModification = true;
		int num = -1;
		int num2 = 0;
		int numItemsBehindView = this.GetNumItemsBehindView();
		for (int i = 0; i < numItemsBehindView - this.longListBehavior.MinBuffer; i++)
		{
			ITouchListItem item = this.items[0];
			this.RemoveAt(0, false);
			this.longListBehavior.ReleaseItem(item);
		}
		float num3 = this.CalculateLocalClipBounds().max[this.layoutDimension1];
		int num4 = 0;
		int num5 = (this.items.Count != 0) ? (this.itemInfos[Enumerable.Last<ITouchListItem>(this.items)].LongListIndex + 1) : 0;
		while (num5 < this.longListBehavior.AllItemsCount && this.items.Count < this.longListBehavior.MaxAcquiredItems && num4 < this.longListBehavior.MinBuffer)
		{
			bool flag3 = this.longListBehavior.IsItemShowable(num5);
			if (flag3)
			{
				if (num < 0)
				{
					num = this.items.Count;
				}
				ITouchListItem touchListItem = this.longListBehavior.AcquireItem(num5);
				this.Add(touchListItem, false);
				TouchList.ItemInfo itemInfo = this.itemInfos[touchListItem];
				itemInfo.LongListIndex = num5;
				num2++;
				if (itemInfo.Min[this.layoutDimension1] > num3)
				{
					num4++;
				}
			}
			num5++;
		}
		if (num >= 0)
		{
			this.layoutSuspended = false;
			this.RepositionItems(num, default(Vector3?));
		}
		this.allowModification = flag;
		if (flag2 != this.layoutSuspended)
		{
			this.layoutSuspended = flag2;
		}
	}

	// Token: 0x06003EE4 RID: 16100 RVA: 0x0013128C File Offset: 0x0012F48C
	private void LoadBehind()
	{
		bool flag = this.allowModification;
		this.allowModification = true;
		int num = 0;
		int numItemsAheadOfView = this.GetNumItemsAheadOfView();
		for (int i = 0; i < numItemsAheadOfView - this.longListBehavior.MinBuffer; i++)
		{
			ITouchListItem item = this.items[this.items.Count - 1];
			this.RemoveAt(this.items.Count - 1, false);
			this.longListBehavior.ReleaseItem(item);
		}
		float num2 = this.CalculateLocalClipBounds().min[this.layoutDimension1];
		int num3 = 0;
		int num4 = (this.items.Count != 0) ? (this.itemInfos[Enumerable.First<ITouchListItem>(this.items)].LongListIndex - 1) : (this.longListBehavior.AllItemsCount - 1);
		while (num4 >= 0 && this.items.Count < this.longListBehavior.MaxAcquiredItems && num3 < this.longListBehavior.MinBuffer)
		{
			bool flag2 = this.longListBehavior.IsItemShowable(num4);
			if (flag2)
			{
				ITouchListItem touchListItem = this.longListBehavior.AcquireItem(num4);
				this.InsertAndPositionBehind(touchListItem, num4);
				TouchList.ItemInfo itemInfo = this.itemInfos[touchListItem];
				itemInfo.LongListIndex = num4;
				num++;
				if (itemInfo.Max[this.layoutDimension1] < num2)
				{
					num3++;
				}
			}
			num4--;
		}
		this.allowModification = flag;
	}

	// Token: 0x06003EE5 RID: 16101 RVA: 0x00131424 File Offset: 0x0012F624
	private int GetNumItemsBehindView()
	{
		float num = this.CalculateLocalClipBounds().min[this.layoutDimension1];
		for (int i = 0; i < this.items.Count; i++)
		{
			ITouchListItem key = this.items[i];
			TouchList.ItemInfo itemInfo = this.itemInfos[key];
			if (itemInfo.Max[this.layoutDimension1] > num)
			{
				return i;
			}
		}
		return this.items.Count;
	}

	// Token: 0x06003EE6 RID: 16102 RVA: 0x001314B0 File Offset: 0x0012F6B0
	private int GetNumItemsAheadOfView()
	{
		float num = this.CalculateLocalClipBounds().max[this.layoutDimension1];
		for (int i = this.items.Count - 1; i >= 0; i--)
		{
			ITouchListItem key = this.items[i];
			TouchList.ItemInfo itemInfo = this.itemInfos[key];
			if (itemInfo.Min[this.layoutDimension1] < num)
			{
				return this.items.Count - 1 - i;
			}
		}
		return this.items.Count;
	}

	// Token: 0x06003EE7 RID: 16103 RVA: 0x0013154C File Offset: 0x0012F74C
	public void RefreshList(int startingLongListIndex, bool preserveScrolling)
	{
		if (this.longListBehavior == null)
		{
			return;
		}
		bool flag = this.allowModification;
		this.allowModification = true;
		int num = (this.SelectedItem != null) ? this.itemInfos[this.SelectedItem].LongListIndex : -1;
		int num2 = -2;
		int num3 = -1;
		if (startingLongListIndex > 0)
		{
			for (int i = 0; i < this.items.Count; i++)
			{
				ITouchListItem key = this.items[i];
				int longListIndex = this.itemInfos[key].LongListIndex;
				if (longListIndex >= startingLongListIndex)
				{
					num3 = i;
					break;
				}
				num2 = i;
			}
		}
		else
		{
			num3 = 0;
		}
		int num4 = (num3 != -1) ? num3 : (num2 + 1);
		Bounds bounds = base.GetComponent<Collider>().bounds;
		Vector3? initialItemPosition = default(Vector3?);
		Vector3 vector = Vector3.zero;
		int num5 = (this.orientation != TouchList.Orientation.Vertical) ? 1 : -1;
		if (preserveScrolling)
		{
			vector = this.content.transform.position;
			ref Vector3 ptr = ref vector;
			int num7;
			int num6 = num7 = this.layoutDimension1;
			float num8 = ptr[num7];
			vector[num6] = num8 - (float)num5 * bounds.extents[this.layoutDimension1];
			ref Vector3 ptr2 = ref vector;
			int num9 = num7 = this.layoutDimension1;
			num8 = ptr2[num7];
			vector[num9] = num8 + (float)num5 * this.padding[this.GetVector2Dimension(this.layoutDimension1)];
			vector[this.layoutDimension2] = bounds.center[this.layoutDimension2];
			vector[this.layoutDimension3] = bounds.center[this.layoutDimension3];
			Vector3 localPosition = this.content.transform.localPosition;
			this.content.transform.localPosition = Vector3.zero;
			Bounds bounds2 = this.CalculateLocalClipBounds();
			Vector3 min = bounds2.min;
			min[this.layoutDimension1] = -localPosition[this.layoutDimension1] + bounds2.min[this.layoutDimension1];
			this.content.transform.localPosition = localPosition;
			initialItemPosition = new Vector3?(min);
			if (num2 >= 0)
			{
				ITouchListItem touchListItem = this.items[num2];
				TouchList.ItemInfo itemInfo = this.itemInfos[touchListItem];
				vector = touchListItem.transform.position - itemInfo.Offset;
				ref Vector3 ptr3 = ref vector;
				int num10 = num7 = this.layoutDimension1;
				num8 = ptr3[num7];
				vector[num10] = num8 + (float)num5 * this.elementSpacing;
				ITouchListItem touchListItem2 = this.items[0];
				TouchList.ItemInfo itemInfo2 = this.itemInfos[touchListItem2];
				initialItemPosition = new Vector3?(touchListItem2.transform.localPosition - itemInfo2.Offset);
			}
		}
		int num11 = 0;
		if (num4 >= 0)
		{
			for (int j = this.items.Count - 1; j >= num4; j--)
			{
				num11++;
				ITouchListItem item = this.items[j];
				this.RemoveAt(j, false);
				this.longListBehavior.ReleaseItem(item);
			}
		}
		if (num3 < 0)
		{
			num3 = num2 + 1;
			if (num3 < 0)
			{
				num3 = 0;
			}
		}
		int num12 = 0;
		int num13 = startingLongListIndex;
		while (num13 < this.longListBehavior.AllItemsCount && this.items.Count < this.longListBehavior.MaxAcquiredItems)
		{
			bool flag2 = this.longListBehavior.IsItemShowable(num13);
			if (flag2)
			{
				bool flag3 = true;
				if (preserveScrolling)
				{
					flag3 = false;
					Vector3 itemSize = this.longListBehavior.GetItemSize(num13);
					Vector3 vector2 = vector;
					ref Vector3 ptr4 = ref vector2;
					int num7;
					int num14 = num7 = this.layoutDimension1;
					float num8 = ptr4[num7];
					vector2[num14] = num8 + (float)num5 * itemSize[this.layoutDimension1];
					if (bounds.Contains(vector) || bounds.Contains(vector2))
					{
						flag3 = true;
					}
					vector = vector2;
					ref Vector3 ptr5 = ref vector;
					int num15 = num7 = this.layoutDimension1;
					num8 = ptr5[num7];
					vector[num15] = num8 + (float)num5 * this.elementSpacing;
				}
				if (flag3)
				{
					num12++;
					ITouchListItem touchListItem3 = this.longListBehavior.AcquireItem(num13);
					this.Add(touchListItem3, false);
					this.itemInfos[touchListItem3].LongListIndex = num13;
				}
			}
			num13++;
		}
		this.RepositionItems(num3, initialItemPosition);
		if (num3 == 0)
		{
			this.LoadBehind();
		}
		if (num4 >= 0)
		{
			this.LoadAhead();
		}
		bool flag4 = false;
		float outOfBoundsDist = this.GetOutOfBoundsDist(this.content.transform.localPosition[this.layoutDimension1]);
		if (outOfBoundsDist != 0f && this.excessContentSize > 0f)
		{
			Vector3 localPosition2 = this.content.transform.localPosition;
			ref Vector3 ptr6 = ref localPosition2;
			int num7;
			int num16 = num7 = this.layoutDimension1;
			float num8 = ptr6[num7];
			localPosition2[num16] = num8 - outOfBoundsDist;
			float num17 = localPosition2[this.layoutDimension1] - this.content.transform.localPosition[this.layoutDimension1];
			this.content.transform.localPosition = localPosition2;
			this.lastContentPosition = this.content.transform.localPosition[this.layoutDimension1];
			if (num17 < 0f)
			{
				this.LoadAhead();
			}
			else
			{
				this.LoadBehind();
			}
			flag4 = true;
		}
		if (num >= 0 && this.items.Count > 0 && num >= this.itemInfos[Enumerable.First<ITouchListItem>(this.items)].LongListIndex && num <= this.itemInfos[Enumerable.Last<ITouchListItem>(this.items)].LongListIndex)
		{
			for (int k = 0; k < this.items.Count; k++)
			{
				ISelectableTouchListItem selectableTouchListItem = this.items[k] as ISelectableTouchListItem;
				if (selectableTouchListItem != null)
				{
					TouchList.ItemInfo itemInfo3 = this.itemInfos[selectableTouchListItem];
					if (itemInfo3.LongListIndex == num)
					{
						this.selection = new int?(k);
						selectableTouchListItem.Selected();
						break;
					}
				}
			}
		}
		flag4 = (this.RecalculateLongListContentSize(false) || flag4);
		this.allowModification = flag;
		if (flag4)
		{
			this.OnScrolled();
			this.OnScrollingEnabledChanged();
		}
	}

	// Token: 0x06003EE8 RID: 16104 RVA: 0x00131C08 File Offset: 0x0012FE08
	private void OnScrollingEnabledChanged()
	{
		if (this.ScrollingEnabledChanged == null)
		{
			return;
		}
		if (this.longListBehavior == null)
		{
			this.ScrollingEnabledChanged(this.excessContentSize > 0f);
		}
		else
		{
			this.ScrollingEnabledChanged(this.m_fullListContentSize > this.ClipSize[this.GetVector2Dimension(this.layoutDimension1)]);
		}
	}

	// Token: 0x06003EE9 RID: 16105 RVA: 0x00131C78 File Offset: 0x0012FE78
	private void RepositionItems(int startingIndex, Vector3? initialItemPosition = null)
	{
		if (this.layoutSuspended)
		{
			return;
		}
		if (this.orientation == TouchList.Orientation.Vertical)
		{
			base.transform.localScale = Vector3.one;
		}
		Vector3 localPosition = this.content.transform.localPosition;
		this.content.transform.localPosition = Vector3.zero;
		Vector3 vector = this.CalculateLocalClipBounds().min;
		if (initialItemPosition != null)
		{
			vector = initialItemPosition.Value;
		}
		ref Vector3 ptr = ref vector;
		int num2;
		int num = num2 = this.layoutDimension1;
		float num3 = ptr[num2];
		vector[num] = num3 + this.padding[this.GetVector2Dimension(this.layoutDimension1)];
		vector[this.layoutDimension3] = 0f;
		this.content.transform.localPosition = localPosition;
		this.ValidateBreadth();
		startingIndex -= startingIndex % this.breadth;
		if (startingIndex > 0)
		{
			int num4 = startingIndex - this.breadth;
			float num5 = float.MinValue;
			int num6 = num4;
			while (num6 < startingIndex && num6 < this.items.Count)
			{
				ITouchListItem key = this.items[num6];
				TouchList.ItemInfo itemInfo = this.itemInfos[key];
				num5 = Mathf.Max(itemInfo.Max[this.layoutDimension1], num5);
				num6++;
			}
			vector[this.layoutDimension1] = num5 + this.elementSpacing;
		}
		Vector3 zero = Vector3.zero;
		zero[this.layoutDimension1] = 1f;
		for (int i = startingIndex; i < this.items.Count; i++)
		{
			ITouchListItem touchListItem = this.items[i];
			if (!touchListItem.IsHeader && !touchListItem.Visible)
			{
				this.items[i].Visible = false;
				this.items[i].gameObject.SetActive(false);
			}
			else
			{
				TouchList.ItemInfo itemInfo2 = this.itemInfos[this.items[i]];
				Vector3 localPosition2 = vector + itemInfo2.Offset;
				localPosition2[this.layoutDimension2] = this.GetBreadthPosition(i) + itemInfo2.Offset[this.layoutDimension2];
				this.items[i].transform.localPosition = localPosition2;
				if ((i + 1) % this.breadth == 0)
				{
					vector = (itemInfo2.Max[this.layoutDimension1] + this.elementSpacing) * zero;
				}
			}
		}
		this.RecalculateSize();
		this.UpdateBackgroundScroll();
		if (this.orientation == TouchList.Orientation.Vertical)
		{
			base.transform.localScale = this.GetNegatedScale(Vector3.one);
		}
		this.SetVisibilityOfAllItems();
	}

	// Token: 0x06003EEA RID: 16106 RVA: 0x00131F64 File Offset: 0x00130164
	private void InsertAndPositionBehind(ITouchListItem item, int longListIndex)
	{
		if (this.items.Count == 0)
		{
			this.Add(item, true);
			return;
		}
		ITouchListItem touchListItem = Enumerable.FirstOrDefault<ITouchListItem>(this.items);
		if (touchListItem == null)
		{
			this.Insert(0, item, true);
			return;
		}
		if (this.orientation == TouchList.Orientation.Vertical)
		{
			base.transform.localScale = Vector3.one;
		}
		TouchList.ItemInfo itemInfo = this.itemInfos[touchListItem];
		Vector3 vector = touchListItem.transform.localPosition - itemInfo.Offset;
		this.Insert(0, item, false);
		this.itemInfos[item].LongListIndex = longListIndex;
		TouchList.ItemInfo itemInfo2 = this.itemInfos[item];
		Vector3 vector2 = vector;
		float num = itemInfo2.Size[this.layoutDimension1] + this.elementSpacing;
		vector2[this.layoutDimension1] = vector2[this.layoutDimension1] - num;
		vector2 += itemInfo2.Offset;
		item.transform.localPosition = vector2;
		if (this.selection == -1 && item is ISelectableTouchListItem && ((ISelectableTouchListItem)item).IsSelected())
		{
			this.selection = new int?(0);
		}
		this.RecalculateSize();
		this.UpdateBackgroundScroll();
		if (this.orientation == TouchList.Orientation.Vertical)
		{
			base.transform.localScale = this.GetNegatedScale(Vector3.one);
		}
		bool active = this.IsItemVisible(0);
		item.gameObject.SetActive(active);
	}

	// Token: 0x06003EEB RID: 16107 RVA: 0x001320F4 File Offset: 0x001302F4
	private void RecalculateSize()
	{
		float num = Math.Abs((base.GetComponent<Collider>() as BoxCollider).size[this.layoutDimension1]);
		float num2 = -num / 2f;
		float num3 = num2;
		if (Enumerable.Any<ITouchListItem>(this.items))
		{
			this.ValidateBreadth();
			int num4 = this.items.Count - 1;
			num4 -= num4 % this.breadth;
			int num5 = Math.Min(num4 + this.breadth, this.items.Count);
			for (int i = num4; i < num5; i++)
			{
				ITouchListItem key = this.items[i];
				TouchList.ItemInfo itemInfo = this.itemInfos[key];
				num3 = Math.Max(itemInfo.Max[this.layoutDimension1], num3);
			}
			this.contentSize = num3 - num2 + this.padding[this.GetVector2Dimension(this.layoutDimension1)];
			this.excessContentSize = Math.Max(this.contentSize - num, 0f);
		}
		else
		{
			this.contentSize = 0f;
			this.excessContentSize = 0f;
		}
		this.OnScrollingEnabledChanged();
	}

	// Token: 0x06003EEC RID: 16108 RVA: 0x00132228 File Offset: 0x00130428
	public bool RecalculateLongListContentSize(bool fireOnScroll = true)
	{
		if (this.longListBehavior == null)
		{
			return false;
		}
		float fullListContentSize = this.m_fullListContentSize;
		this.m_fullListContentSize = 0f;
		bool flag = true;
		for (int i = 0; i < this.longListBehavior.AllItemsCount; i++)
		{
			if (this.longListBehavior.IsItemShowable(i))
			{
				Vector3 itemSize = this.longListBehavior.GetItemSize(i);
				this.m_fullListContentSize += itemSize[this.layoutDimension1];
				if (flag)
				{
					flag = false;
				}
				else
				{
					this.m_fullListContentSize += this.elementSpacing;
				}
			}
		}
		if (this.m_fullListContentSize > 0f)
		{
			this.m_fullListContentSize += 2f * this.padding[this.GetVector2Dimension(this.layoutDimension1)];
		}
		bool flag2 = fullListContentSize != this.m_fullListContentSize;
		if (flag2 && fireOnScroll)
		{
			this.OnScrolled();
			this.OnScrollingEnabledChanged();
		}
		return flag2;
	}

	// Token: 0x06003EED RID: 16109 RVA: 0x00132334 File Offset: 0x00130534
	private void UpdateBackgroundBounds()
	{
		if (this.background == null)
		{
			return;
		}
		Vector3 size = (base.GetComponent<Collider>() as BoxCollider).size;
		size[this.layoutDimension1] = Math.Abs(size[this.layoutDimension1]);
		size[this.layoutDimension3] = 0f;
		Camera camera = CameraUtils.FindFirstByLayer((GameLayer)base.gameObject.layer);
		if (camera == null)
		{
			return;
		}
		float num = Vector3.Distance(camera.transform.position, base.GetComponent<Collider>().bounds.min);
		float num2 = Vector3.Distance(camera.transform.position, base.GetComponent<Collider>().bounds.max);
		Vector3 vector = (num <= num2) ? base.GetComponent<Collider>().bounds.max : base.GetComponent<Collider>().bounds.min;
		Vector3 zero = Vector3.zero;
		zero[this.layoutDimension3] = this.content.transform.InverseTransformPoint(vector)[this.layoutDimension3];
		this.background.SetBounds(new Bounds(zero, size));
		this.UpdateBackgroundScroll();
	}

	// Token: 0x06003EEE RID: 16110 RVA: 0x00132484 File Offset: 0x00130684
	private void UpdateBackgroundScroll()
	{
		if (this.background == null)
		{
			return;
		}
		float num = Math.Abs((base.GetComponent<Collider>() as BoxCollider).size[this.layoutDimension1]);
		float num2 = this.content.transform.localPosition[this.layoutDimension1];
		if (this.orientation == TouchList.Orientation.Vertical)
		{
			num2 *= -1f;
		}
		Vector2 offset = this.background.Offset;
		offset[this.GetVector2Dimension(this.layoutDimension1)] = num2 / num;
		this.background.Offset = offset;
	}

	// Token: 0x06003EEF RID: 16111 RVA: 0x0013252C File Offset: 0x0013072C
	private float GetBreadthPosition(int itemIndex)
	{
		float num = this.padding[this.GetVector2Dimension(this.layoutDimension2)];
		float num2 = 0f;
		int num3 = itemIndex - itemIndex % this.breadth;
		int num4 = Math.Min(num3 + this.breadth, this.items.Count);
		for (int i = num3; i < num4; i++)
		{
			if (i == itemIndex)
			{
				num2 = num;
			}
			num += this.itemInfos[this.items[i]].Size[this.layoutDimension2];
		}
		num += this.padding[this.GetVector2Dimension(this.layoutDimension2)];
		float num5 = 0f;
		float num6 = (base.GetComponent<Collider>() as BoxCollider).size[this.layoutDimension2];
		TouchList.Alignment alignment = this.alignment;
		if (this.orientation == TouchList.Orientation.Horizontal && this.alignment != TouchList.Alignment.Mid)
		{
			alignment = (this.alignment ^ TouchList.Alignment.Max);
		}
		switch (alignment)
		{
		case TouchList.Alignment.Min:
			num5 = -num6 / 2f;
			break;
		case TouchList.Alignment.Mid:
			num5 = -num / 2f;
			break;
		case TouchList.Alignment.Max:
			num5 = num6 / 2f - num;
			break;
		}
		return num5 + num2;
	}

	// Token: 0x06003EF0 RID: 16112 RVA: 0x00132688 File Offset: 0x00130888
	private Vector3 GetNegatedScale(Vector3 scale)
	{
		ref Vector3 ptr = ref scale;
		int num2;
		int num = num2 = ((this.layoutPlane != TouchList.LayoutPlane.XY) ? 2 : 1);
		float num3 = ptr[num2];
		scale[num] = num3 * -1f;
		return scale;
	}

	// Token: 0x06003EF1 RID: 16113 RVA: 0x001326C2 File Offset: 0x001308C2
	private int GetVector2Dimension(int vec3Dimension)
	{
		return (vec3Dimension != 0) ? 1 : vec3Dimension;
	}

	// Token: 0x06003EF2 RID: 16114 RVA: 0x001326D1 File Offset: 0x001308D1
	private int GetVector3Dimension(int vec2Dimension)
	{
		if (vec2Dimension == 0 || this.layoutPlane == TouchList.LayoutPlane.XY)
		{
			return vec2Dimension;
		}
		return 2;
	}

	// Token: 0x06003EF3 RID: 16115 RVA: 0x001326E7 File Offset: 0x001308E7
	private void ValidateBreadth()
	{
		if (this.longListBehavior != null)
		{
			this.breadth = 1;
		}
		else
		{
			this.breadth = Math.Max(this.breadth, 1);
		}
	}

	// Token: 0x06003EF4 RID: 16116 RVA: 0x00132714 File Offset: 0x00130914
	private Bounds CalculateLocalClipBounds()
	{
		Vector3 vector = this.content.transform.InverseTransformPoint(base.GetComponent<Collider>().bounds.min);
		Vector3 vector2 = this.content.transform.InverseTransformPoint(base.GetComponent<Collider>().bounds.max);
		Vector3 vector3 = (vector2 + vector) / 2f;
		Vector3 vector4 = VectorUtils.Abs(vector2 - vector);
		return new Bounds(vector3, vector4);
	}

	// Token: 0x04002820 RID: 10272
	private const float ScrollDragThreshold = 0.05f;

	// Token: 0x04002821 RID: 10273
	private const float ItemDragThreshold = 0.05f;

	// Token: 0x04002822 RID: 10274
	private const float KineticScrollFriction = 10000f;

	// Token: 0x04002823 RID: 10275
	private const float MinKineticScrollSpeed = 0.01f;

	// Token: 0x04002824 RID: 10276
	private const float ScrollBoundsSpringK = 400f;

	// Token: 0x04002825 RID: 10277
	private const float MinOutOfBoundsDistance = 0.05f;

	// Token: 0x04002826 RID: 10278
	private const float CLIPSIZE_EPSILON = 0.0001f;

	// Token: 0x04002827 RID: 10279
	public TouchList.Orientation orientation;

	// Token: 0x04002828 RID: 10280
	public TouchList.Alignment alignment = TouchList.Alignment.Mid;

	// Token: 0x04002829 RID: 10281
	public TouchList.LayoutPlane layoutPlane;

	// Token: 0x0400282A RID: 10282
	public float elementSpacing;

	// Token: 0x0400282B RID: 10283
	public Vector2 padding = Vector2.zero;

	// Token: 0x0400282C RID: 10284
	public int breadth = 1;

	// Token: 0x0400282D RID: 10285
	public float itemDragFinishDistance;

	// Token: 0x0400282E RID: 10286
	public TiledBackground background;

	// Token: 0x0400282F RID: 10287
	public float scrollWheelIncrement = 30f;

	// Token: 0x04002830 RID: 10288
	public Float_MobileOverride maxKineticScrollSpeed = new Float_MobileOverride();

	// Token: 0x04002831 RID: 10289
	private GameObject content;

	// Token: 0x04002832 RID: 10290
	private List<ITouchListItem> items = new List<ITouchListItem>();

	// Token: 0x04002833 RID: 10291
	private Map<ITouchListItem, TouchList.ItemInfo> itemInfos = new Map<ITouchListItem, TouchList.ItemInfo>();

	// Token: 0x04002834 RID: 10292
	private int layoutDimension1;

	// Token: 0x04002835 RID: 10293
	private int layoutDimension2;

	// Token: 0x04002836 RID: 10294
	private int layoutDimension3;

	// Token: 0x04002837 RID: 10295
	private float contentSize;

	// Token: 0x04002838 RID: 10296
	private float excessContentSize;

	// Token: 0x04002839 RID: 10297
	private float m_fullListContentSize;

	// Token: 0x0400283A RID: 10298
	private Vector2? touchBeginScreenPosition;

	// Token: 0x0400283B RID: 10299
	private Vector3? dragBeginOffsetFromContent;

	// Token: 0x0400283C RID: 10300
	private Vector3 dragBeginContentPosition = Vector3.zero;

	// Token: 0x0400283D RID: 10301
	private Vector3 lastTouchPosition = Vector3.zero;

	// Token: 0x0400283E RID: 10302
	private float lastContentPosition;

	// Token: 0x0400283F RID: 10303
	private ITouchListItem touchBeginItem;

	// Token: 0x04002840 RID: 10304
	private bool m_isHoveredOverTouchList;

	// Token: 0x04002841 RID: 10305
	private PegUIElement m_hoveredOverItem;

	// Token: 0x04002842 RID: 10306
	private TouchList.ILongListBehavior longListBehavior;

	// Token: 0x04002843 RID: 10307
	private bool allowModification = true;

	// Token: 0x04002844 RID: 10308
	private Vector3? dragItemBegin;

	// Token: 0x04002845 RID: 10309
	private bool layoutSuspended;

	// Token: 0x04002846 RID: 10310
	private int? selection;

	// Token: 0x04002847 RID: 10311
	private static readonly float ScrollBoundsSpringB = Mathf.Sqrt(1600f);

	// Token: 0x04002848 RID: 10312
	private static readonly Func<float, float> OutOfBoundsDistReducer = (float dist) => 30f * (Mathf.Log(dist + 30f) - Mathf.Log(30f));

	// Token: 0x02000571 RID: 1393
	public interface ILongListBehavior
	{
		// Token: 0x17000484 RID: 1156
		// (get) Token: 0x06003FC4 RID: 16324
		int AllItemsCount { get; }

		// Token: 0x17000485 RID: 1157
		// (get) Token: 0x06003FC5 RID: 16325
		int MaxVisibleItems { get; }

		// Token: 0x17000486 RID: 1158
		// (get) Token: 0x06003FC6 RID: 16326
		int MinBuffer { get; }

		// Token: 0x06003FC7 RID: 16327
		void ReleaseAllItems();

		// Token: 0x06003FC8 RID: 16328
		void ReleaseItem(ITouchListItem item);

		// Token: 0x06003FC9 RID: 16329
		ITouchListItem AcquireItem(int index);

		// Token: 0x17000487 RID: 1159
		// (get) Token: 0x06003FCA RID: 16330
		int MaxAcquiredItems { get; }

		// Token: 0x06003FCB RID: 16331
		bool IsItemShowable(int allItemsIndex);

		// Token: 0x06003FCC RID: 16332
		Vector3 GetItemSize(int allItemsIndex);
	}

	// Token: 0x02000582 RID: 1410
	// (Invoke) Token: 0x06004011 RID: 16401
	public delegate bool SelectedIndexChangingEvent(int index);

	// Token: 0x02000590 RID: 1424
	public enum Orientation
	{
		// Token: 0x04002922 RID: 10530
		Horizontal,
		// Token: 0x04002923 RID: 10531
		Vertical
	}

	// Token: 0x02000591 RID: 1425
	public enum Alignment
	{
		// Token: 0x04002925 RID: 10533
		Min,
		// Token: 0x04002926 RID: 10534
		Mid,
		// Token: 0x04002927 RID: 10535
		Max
	}

	// Token: 0x02000592 RID: 1426
	public enum LayoutPlane
	{
		// Token: 0x04002929 RID: 10537
		XY,
		// Token: 0x0400292A RID: 10538
		XZ
	}

	// Token: 0x02000593 RID: 1427
	private class ItemInfo
	{
		// Token: 0x06004083 RID: 16515 RVA: 0x00137B04 File Offset: 0x00135D04
		public ItemInfo(ITouchListItem item, TouchList.LayoutPlane layoutPlane)
		{
			this.item = item;
			Vector3 vector = Vector3.Scale(item.LocalBounds.min, VectorUtils.Abs(item.transform.localScale)) - item.transform.localPosition;
			Vector3 vector2 = Vector3.Scale(item.LocalBounds.max, VectorUtils.Abs(item.transform.localScale)) - item.transform.localPosition;
			this.Size = vector2 - vector;
			Vector3 vector3 = vector;
			if (layoutPlane == TouchList.LayoutPlane.XZ)
			{
				vector3.y = vector2.y;
			}
			this.Offset = item.transform.localPosition - vector3;
		}

		// Token: 0x17000496 RID: 1174
		// (get) Token: 0x06004084 RID: 16516 RVA: 0x00137BC2 File Offset: 0x00135DC2
		// (set) Token: 0x06004085 RID: 16517 RVA: 0x00137BCA File Offset: 0x00135DCA
		public Vector3 Size { get; private set; }

		// Token: 0x17000497 RID: 1175
		// (get) Token: 0x06004086 RID: 16518 RVA: 0x00137BD3 File Offset: 0x00135DD3
		// (set) Token: 0x06004087 RID: 16519 RVA: 0x00137BDB File Offset: 0x00135DDB
		public Vector3 Offset { get; private set; }

		// Token: 0x17000498 RID: 1176
		// (get) Token: 0x06004088 RID: 16520 RVA: 0x00137BE4 File Offset: 0x00135DE4
		// (set) Token: 0x06004089 RID: 16521 RVA: 0x00137BEC File Offset: 0x00135DEC
		public int LongListIndex { get; set; }

		// Token: 0x17000499 RID: 1177
		// (get) Token: 0x0600408A RID: 16522 RVA: 0x00137BF8 File Offset: 0x00135DF8
		public Vector3 Min
		{
			get
			{
				return this.item.transform.localPosition + Vector3.Scale(this.item.LocalBounds.min, VectorUtils.Abs(this.item.transform.localScale));
			}
		}

		// Token: 0x1700049A RID: 1178
		// (get) Token: 0x0600408B RID: 16523 RVA: 0x00137C48 File Offset: 0x00135E48
		public Vector3 Max
		{
			get
			{
				return this.item.transform.localPosition + Vector3.Scale(this.item.LocalBounds.max, VectorUtils.Abs(this.item.transform.localScale));
			}
		}

		// Token: 0x0600408C RID: 16524 RVA: 0x00137C98 File Offset: 0x00135E98
		public bool Contains(Vector2 point, TouchList.LayoutPlane layoutPlane)
		{
			Vector3 min = this.Min;
			Vector3 max = this.Max;
			int num = (layoutPlane != TouchList.LayoutPlane.XY) ? 2 : 1;
			return point.x > min.x && point.y > min[num] && point.x < max.x && point.y < max[num];
		}

		// Token: 0x0400292B RID: 10539
		private readonly ITouchListItem item;
	}

	// Token: 0x02000594 RID: 1428
	// (Invoke) Token: 0x0600408E RID: 16526
	public delegate void ScrollingEnabledChangedEvent(bool canScroll);

	// Token: 0x02000595 RID: 1429
	// (Invoke) Token: 0x06004092 RID: 16530
	public delegate bool ItemDragEvent(ITouchListItem item, float dragAmount);
}
