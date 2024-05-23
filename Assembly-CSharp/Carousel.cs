using System;
using UnityEngine;

// Token: 0x02000A91 RID: 2705
public class Carousel : MonoBehaviour
{
	// Token: 0x06005E21 RID: 24097 RVA: 0x001C313D File Offset: 0x001C133D
	private void Awake()
	{
		this.m_numItems = this.m_bones.Length;
		this.m_radius = this.m_numItems / 2;
	}

	// Token: 0x06005E22 RID: 24098 RVA: 0x001C315B File Offset: 0x001C135B
	private void Start()
	{
		this.m_trackItemHit = true;
	}

	// Token: 0x06005E23 RID: 24099 RVA: 0x001C3164 File Offset: 0x001C1364
	private void OnDestroy()
	{
		this.m_itemPulledListener = null;
		this.m_itemClickedListener = null;
		this.m_itemReleasedListener = null;
		this.m_carouselSettledListener = null;
		this.m_carouselStartedScrollingListener = null;
	}

	// Token: 0x06005E24 RID: 24100 RVA: 0x001C318C File Offset: 0x001C138C
	public void UpdateUI(bool mouseDown)
	{
		if (this.m_items == null)
		{
			return;
		}
		bool flag = this.m_position < 0f || this.m_position > (float)this.m_maxPosition;
		if (this.m_touchActive)
		{
			float x = Input.mousePosition.x;
			float num = x - this.m_touchX;
			if (!this.m_scrolling && Math.Abs(this.m_touchStart.x - x) >= 10f)
			{
				this.StartScrolling();
			}
			float num2 = num * 4.5f / (float)Screen.width;
			if (this.m_position < 0f)
			{
				num2 /= 1f + Math.Abs(this.m_position) * 5f;
			}
			if (!this.m_noMouseMovement)
			{
				if (this.m_trackItemHit)
				{
					Vector3 vector;
					UniversalInputManager.Get().GetInputPointOnPlane(this.m_hitWorldPosition, out vector);
					float carouselPosition = this.GetCarouselPosition(vector.x);
					this.m_position = this.m_startX + this.m_hitCarouselPosition - carouselPosition;
				}
				else
				{
					this.m_position -= num2;
				}
			}
			this.m_momentumHistory[this.m_momentumCounter] = num2;
			this.m_momentumCounter++;
			this.m_momentumTotal++;
			flag = (this.m_position < 0f || this.m_position > (float)this.m_maxPosition);
			if (this.m_momentumCounter >= this.m_momentumHistory.Length)
			{
				this.m_momentumCounter = 0;
			}
			if (this.m_momentumTotal >= this.m_momentumHistory.Length)
			{
				this.m_momentumTotal = this.m_momentumHistory.Length;
			}
			this.m_touchX = x;
			float num3 = (float)Screen.height * 0.1f;
			float num4 = (float)Screen.height * 0.275f;
			if (this.m_itemPulledListener != null && Input.mousePosition.y - this.m_touchStart.y > num3 && Input.mousePosition.y > num4)
			{
				this.m_itemPulledListener(this.m_hitItem, this.m_hitIndex);
				this.m_touchActive = false;
				this.m_velocity = 0f;
				this.SettlePosition(0f);
			}
			if (!Input.GetMouseButton(0))
			{
				if (!this.m_noMouseMovement && this.m_scrolling)
				{
					this.m_velocity = this.CalculateVelocity();
					if (this.m_position < 0f)
					{
						this.m_targetPosition = 0f;
						this.m_velocity = 0f;
					}
					else if (this.m_position >= (float)this.m_maxPosition)
					{
						this.m_targetPosition = (float)this.m_maxPosition;
						this.m_velocity = 0f;
					}
					else if (Math.Abs(this.m_velocity) < 0.03f)
					{
						this.SettlePosition(this.m_velocity);
						this.m_velocity = 0f;
					}
				}
				if (this.m_itemReleasedListener != null)
				{
					this.m_itemReleasedListener();
				}
				this.m_touchActive = false;
			}
		}
		CarouselItem hitItem;
		int num5 = this.MouseHit(out hitItem);
		if (mouseDown && num5 >= 0)
		{
			this.m_touchActive = true;
			this.m_touchX = Input.mousePosition.x;
			this.m_touchStart = Input.mousePosition;
			this.m_velocity = 0f;
			this.m_hitIndex = num5;
			this.m_hitItem = hitItem;
			this.m_scrolling = false;
			this.m_settleCallbackCalled = false;
			if (this.m_trackItemHit)
			{
				RaycastHit raycastHit;
				UniversalInputManager.Get().GetInputHitInfo(out raycastHit);
				this.m_hitWorldPosition = raycastHit.point;
				this.m_hitCarouselPosition = this.GetCarouselPosition(this.m_hitWorldPosition.x);
				this.m_startX = this.m_position;
			}
			this.InitVelocity();
			if (this.m_itemClickedListener != null)
			{
				this.m_itemClickedListener(this.m_hitItem, num5);
			}
		}
		if (!this.m_touchActive && this.m_velocity != 0f)
		{
			if (Math.Abs(this.m_velocity) < 0.03f || flag)
			{
				this.SettlePosition(this.m_velocity);
				this.m_velocity = 0f;
			}
			else
			{
				this.m_position += this.m_velocity;
				this.m_velocity -= 0.015f * (float)Math.Sign(this.m_velocity);
			}
		}
		if (!this.m_touchActive && this.m_targetPosition != this.m_position && this.m_velocity == 0f)
		{
			this.m_position = this.m_targetPosition * 0.15f + this.m_position * 0.85f;
			if (!this.m_settleCallbackCalled && Math.Abs(this.m_position - this.m_targetPosition) < 0.1f)
			{
				this.m_settleCallbackCalled = true;
				if (this.m_carouselSettledListener != null)
				{
					this.m_carouselSettledListener();
				}
			}
			if (Math.Abs(this.m_position - this.m_targetPosition) < 0.01f)
			{
				this.m_position = this.m_targetPosition;
				this.m_scrolling = false;
			}
		}
		this.m_intPosition = (int)Mathf.Round(this.m_position);
		this.UpdateVisibleItems();
		if (this.m_doFlyIn)
		{
			float num6 = Math.Min(0.03f, Time.deltaTime);
			this.m_flyInState += num6 * 8f;
			if (this.m_flyInState > (float)this.m_bones.Length)
			{
				this.m_doFlyIn = false;
			}
		}
	}

	// Token: 0x06005E25 RID: 24101 RVA: 0x001C3728 File Offset: 0x001C1928
	public bool MouseOver()
	{
		CarouselItem carouselItem;
		return this.MouseHit(out carouselItem) >= 0;
	}

	// Token: 0x06005E26 RID: 24102 RVA: 0x001C3744 File Offset: 0x001C1944
	private float GetCarouselPosition(float x)
	{
		if (x < this.m_bones[0].transform.position.x)
		{
			return 0f;
		}
		if (x > this.m_bones[this.m_bones.Length - 1].transform.position.x)
		{
			return (float)this.m_bones.Length - 1f;
		}
		float num = this.m_bones[0].transform.position.x;
		for (int i = 1; i < this.m_bones.Length; i++)
		{
			float x2 = this.m_bones[i].transform.position.x;
			if (x >= num && x <= x2)
			{
				return (float)i + (x - num) / (x2 - num);
			}
			num = x2;
		}
		return 0f;
	}

	// Token: 0x06005E27 RID: 24103 RVA: 0x001C3824 File Offset: 0x001C1A24
	private int MouseHit(out CarouselItem itemHit)
	{
		int result = -1;
		itemHit = null;
		if (this.m_items == null || this.m_items.Length <= 0)
		{
			return -1;
		}
		for (int i = 0; i < this.m_items.Length; i++)
		{
			CarouselItem carouselItem = this.m_items[i];
			GameObject gameObject = carouselItem.GetGameObject();
			RaycastHit raycastHit;
			if (gameObject != null && UniversalInputManager.Get().InputIsOver(carouselItem.GetGameObject(), out raycastHit))
			{
				itemHit = carouselItem;
				result = i;
				break;
			}
		}
		return result;
	}

	// Token: 0x06005E28 RID: 24104 RVA: 0x001C38A8 File Offset: 0x001C1AA8
	public void SetListeners(Carousel.ItemPulled pulled, Carousel.ItemClicked clicked, Carousel.ItemReleased released, Carousel.CarouselSettled settled = null, Carousel.CarouselStartedScrolling scrolling = null)
	{
		this.m_itemPulledListener = pulled;
		this.m_itemClickedListener = clicked;
		this.m_itemReleasedListener = released;
		this.m_carouselSettledListener = settled;
		this.m_carouselStartedScrollingListener = scrolling;
	}

	// Token: 0x06005E29 RID: 24105 RVA: 0x001C38D0 File Offset: 0x001C1AD0
	private void InitVelocity()
	{
		for (int i = 0; i < this.m_momentumHistory.Length; i++)
		{
			this.m_momentumHistory[i] = 0f;
		}
		this.m_momentumCounter = 0;
		this.m_momentumTotal = 0;
	}

	// Token: 0x06005E2A RID: 24106 RVA: 0x001C3914 File Offset: 0x001C1B14
	private float CalculateVelocity()
	{
		float num = 0f;
		for (int i = 0; i < this.m_momentumTotal; i++)
		{
			num += this.m_momentumHistory[i];
		}
		return -0.9f * num / (float)this.m_momentumTotal;
	}

	// Token: 0x06005E2B RID: 24107 RVA: 0x001C395C File Offset: 0x001C1B5C
	private float DistanceFromSettle()
	{
		float num = this.m_position - (float)((int)this.m_position);
		if (num > 0.5f)
		{
			return 1f - num;
		}
		return num;
	}

	// Token: 0x06005E2C RID: 24108 RVA: 0x001C3990 File Offset: 0x001C1B90
	private void SettlePosition(float bias = 0f)
	{
		float num;
		if (bias > 0.001f)
		{
			num = Mathf.Round(this.m_position + 0.5f);
		}
		else if (bias < -0.001f)
		{
			num = Mathf.Round(this.m_position - 0.5f);
		}
		else
		{
			num = Mathf.Round(this.m_position);
		}
		num = Math.Max(0f, num);
		num = Math.Min((float)this.m_maxPosition, num);
		this.m_targetPosition = num;
	}

	// Token: 0x06005E2D RID: 24109 RVA: 0x001C3A10 File Offset: 0x001C1C10
	private void UpdateVisibleItems()
	{
		float position = this.m_position;
		int num = Mathf.FloorToInt(position);
		float num2 = position - (float)num;
		float num3 = 1f - num2;
		int num4 = 0;
		for (int i = 0; i < this.m_items.Length; i++)
		{
			int num5 = i - num + this.m_radius - 1;
			int num6 = i - num + this.m_radius;
			if (num5 < 0 || num6 >= this.m_bones.Length)
			{
				this.m_items[i].Hide();
			}
			else
			{
				this.m_items[i].Show(this);
				if (this.m_items[i].IsLoaded())
				{
					Vector3 localPosition = this.m_bones[num5].transform.localPosition;
					Vector3 localPosition2 = this.m_bones[num6].transform.localPosition;
					Vector3 localScale = this.m_bones[num5].transform.localScale;
					Vector3 localScale2 = this.m_bones[num6].transform.localScale;
					Quaternion localRotation = this.m_bones[num5].transform.localRotation;
					Quaternion localRotation2 = this.m_bones[num6].transform.localRotation;
					Vector3 localPosition3;
					localPosition3..ctor(localPosition.x * num2 + localPosition2.x * num3, localPosition.y * num2 + localPosition2.y * num3, localPosition.z * num2 + localPosition2.z * num3);
					Vector3 localScale3;
					localScale3..ctor(localScale.x * num2 + localScale2.x * num3, localScale.y * num2 + localScale2.y * num3, localScale.z * num2 + localScale2.z * num3);
					Quaternion localRotation3;
					localRotation3..ctor(localRotation.x * num2 + localRotation2.x * num3, localRotation.y * num2 + localRotation2.y * num3, localRotation.z * num2 + localRotation2.z * num3, localRotation.w * num2 + localRotation2.w * num3);
					if (this.m_doFlyIn)
					{
						float num7 = 1f;
						if (num4 >= (int)this.m_flyInState + 1)
						{
							num7 = 0f;
						}
						else if (num4 >= (int)this.m_flyInState)
						{
							num7 = this.m_flyInState - (float)Math.Floor((double)this.m_flyInState);
						}
						float num8 = 1f - num7;
						Vector3 vector;
						vector..ctor(81f, 9.4f, 4f);
						this.m_items[i].GetGameObject().transform.localPosition = new Vector3(num7 * localPosition3.x + num8 * vector.x, num7 * localPosition3.y + num8 * vector.y, num7 * localPosition3.z + num8 * vector.z);
					}
					else
					{
						this.m_items[i].GetGameObject().transform.localPosition = localPosition3;
					}
					this.m_items[i].GetGameObject().transform.localScale = localScale3;
					this.m_items[i].GetGameObject().transform.localRotation = localRotation3;
				}
				num4++;
			}
		}
	}

	// Token: 0x06005E2E RID: 24110 RVA: 0x001C3D38 File Offset: 0x001C1F38
	public void Initialize(CarouselItem[] items, int position = 0)
	{
		if (this.m_items != null)
		{
			this.ClearItems();
		}
		this.m_items = items;
		this.m_position = (this.m_targetPosition = (float)position);
		this.m_intPosition = position;
		this.DoFlyIn();
	}

	// Token: 0x06005E2F RID: 24111 RVA: 0x001C3D7C File Offset: 0x001C1F7C
	public void ClearItems()
	{
		if (this.m_items == null)
		{
			return;
		}
		foreach (CarouselItem carouselItem in this.m_items)
		{
			carouselItem.Clear();
		}
	}

	// Token: 0x06005E30 RID: 24112 RVA: 0x001C3DBC File Offset: 0x001C1FBC
	public bool AreVisibleItemsLoaded()
	{
		for (int i = 0; i < this.m_numItems; i++)
		{
			int num = this.m_intPosition + i - this.m_radius;
			if (num >= 0)
			{
				if (!this.m_items[num].IsLoaded())
				{
					Debug.Log("Not loaded " + num);
					return false;
				}
			}
		}
		return true;
	}

	// Token: 0x06005E31 RID: 24113 RVA: 0x001C3E26 File Offset: 0x001C2026
	public int GetCurrentIndex()
	{
		return this.m_intPosition;
	}

	// Token: 0x06005E32 RID: 24114 RVA: 0x001C3E2E File Offset: 0x001C202E
	public int GetTargetPosition()
	{
		return (int)this.m_targetPosition;
	}

	// Token: 0x06005E33 RID: 24115 RVA: 0x001C3E37 File Offset: 0x001C2037
	public CarouselItem GetCurrentItem()
	{
		if (this.m_items == null)
		{
			return null;
		}
		return this.m_items[this.m_intPosition];
	}

	// Token: 0x06005E34 RID: 24116 RVA: 0x001C3E54 File Offset: 0x001C2054
	public void SetPosition(int n, bool animate = false)
	{
		this.m_targetPosition = (float)n;
		if (!animate)
		{
			this.m_position = this.m_targetPosition;
		}
		else
		{
			this.StartScrolling();
			this.m_settleCallbackCalled = false;
		}
		this.DoFlyIn();
	}

	// Token: 0x06005E35 RID: 24117 RVA: 0x001C3E93 File Offset: 0x001C2093
	public bool IsScrolling()
	{
		return this.m_scrolling;
	}

	// Token: 0x06005E36 RID: 24118 RVA: 0x001C3E9B File Offset: 0x001C209B
	private void DoFlyIn()
	{
		if (this.m_useFlyIn)
		{
			this.m_doFlyIn = true;
			this.m_flyInState = 0f;
		}
	}

	// Token: 0x06005E37 RID: 24119 RVA: 0x001C3EBA File Offset: 0x001C20BA
	private void StartScrolling()
	{
		this.m_scrolling = true;
		if (this.m_carouselStartedScrollingListener != null)
		{
			this.m_carouselStartedScrollingListener();
		}
	}

	// Token: 0x040045B6 RID: 17846
	private const float MIN_VELOCITY = 0.03f;

	// Token: 0x040045B7 RID: 17847
	private const float DRAG = 0.015f;

	// Token: 0x040045B8 RID: 17848
	private const float SCROLL_THRESHOLD = 10f;

	// Token: 0x040045B9 RID: 17849
	public GameObject[] m_bones;

	// Token: 0x040045BA RID: 17850
	public Collider m_collider;

	// Token: 0x040045BB RID: 17851
	public bool m_useFlyIn;

	// Token: 0x040045BC RID: 17852
	public bool m_trackItemHit;

	// Token: 0x040045BD RID: 17853
	public bool m_noMouseMovement;

	// Token: 0x040045BE RID: 17854
	public int m_maxPosition = 4;

	// Token: 0x040045BF RID: 17855
	private int m_intPosition;

	// Token: 0x040045C0 RID: 17856
	private float m_position;

	// Token: 0x040045C1 RID: 17857
	private float m_targetPosition;

	// Token: 0x040045C2 RID: 17858
	private CarouselItem[] m_items;

	// Token: 0x040045C3 RID: 17859
	private int m_numItems;

	// Token: 0x040045C4 RID: 17860
	private int m_radius;

	// Token: 0x040045C5 RID: 17861
	private bool m_touchActive;

	// Token: 0x040045C6 RID: 17862
	private Vector2 m_touchStart;

	// Token: 0x040045C7 RID: 17863
	private float m_startX;

	// Token: 0x040045C8 RID: 17864
	private float m_touchX;

	// Token: 0x040045C9 RID: 17865
	private float m_velocity;

	// Token: 0x040045CA RID: 17866
	private int m_hitIndex;

	// Token: 0x040045CB RID: 17867
	private CarouselItem m_hitItem;

	// Token: 0x040045CC RID: 17868
	private Vector3 m_hitWorldPosition;

	// Token: 0x040045CD RID: 17869
	private float m_hitCarouselPosition;

	// Token: 0x040045CE RID: 17870
	private float m_totalMove;

	// Token: 0x040045CF RID: 17871
	private float m_moveAdjustment;

	// Token: 0x040045D0 RID: 17872
	private int m_momentumCounter;

	// Token: 0x040045D1 RID: 17873
	private int m_momentumTotal;

	// Token: 0x040045D2 RID: 17874
	private float[] m_momentumHistory = new float[5];

	// Token: 0x040045D3 RID: 17875
	private bool m_doFlyIn;

	// Token: 0x040045D4 RID: 17876
	private float m_flyInState;

	// Token: 0x040045D5 RID: 17877
	private bool m_settleCallbackCalled;

	// Token: 0x040045D6 RID: 17878
	private bool m_scrolling;

	// Token: 0x040045D7 RID: 17879
	private Carousel.ItemPulled m_itemPulledListener;

	// Token: 0x040045D8 RID: 17880
	private Carousel.ItemClicked m_itemClickedListener;

	// Token: 0x040045D9 RID: 17881
	private Carousel.ItemReleased m_itemReleasedListener;

	// Token: 0x040045DA RID: 17882
	private Carousel.CarouselSettled m_carouselSettledListener;

	// Token: 0x040045DB RID: 17883
	private Carousel.CarouselStartedScrolling m_carouselStartedScrollingListener;

	// Token: 0x02000A94 RID: 2708
	// (Invoke) Token: 0x06005E45 RID: 24133
	public delegate void ItemClicked(CarouselItem item, int index);

	// Token: 0x02000A95 RID: 2709
	// (Invoke) Token: 0x06005E49 RID: 24137
	public delegate void ItemReleased();

	// Token: 0x02000A96 RID: 2710
	// (Invoke) Token: 0x06005E4D RID: 24141
	public delegate void CarouselSettled();

	// Token: 0x02000A97 RID: 2711
	// (Invoke) Token: 0x06005E51 RID: 24145
	public delegate void CarouselStartedScrolling();

	// Token: 0x02000A98 RID: 2712
	// (Invoke) Token: 0x06005E55 RID: 24149
	public delegate void ItemPulled(CarouselItem item, int index);

	// Token: 0x02000A9E RID: 2718
	// (Invoke) Token: 0x06005E68 RID: 24168
	public delegate void CarouselMoved();
}
