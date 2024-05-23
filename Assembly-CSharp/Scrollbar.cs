using System;
using UnityEngine;

// Token: 0x02000F60 RID: 3936
public class Scrollbar : MonoBehaviour
{
	// Token: 0x17000A10 RID: 2576
	// (get) Token: 0x060074DC RID: 29916 RVA: 0x00228272 File Offset: 0x00226472
	public float ScrollValue
	{
		get
		{
			return this.m_scrollValue;
		}
	}

	// Token: 0x060074DD RID: 29917 RVA: 0x0022827C File Offset: 0x0022647C
	protected virtual void Awake()
	{
		this.m_scrollWindowHeight = this.m_scrollWindow.size.z;
		this.m_scrollWindow.enabled = false;
	}

	// Token: 0x060074DE RID: 29918 RVA: 0x002282AE File Offset: 0x002264AE
	public bool IsActive()
	{
		return this.m_isActive;
	}

	// Token: 0x060074DF RID: 29919 RVA: 0x002282B8 File Offset: 0x002264B8
	private void Update()
	{
		if (!this.m_isActive)
		{
			return;
		}
		if (this.InputIsOver())
		{
			if (Input.GetAxis("Mouse ScrollWheel") < 0f)
			{
				this.ScrollDown();
			}
			if (Input.GetAxis("Mouse ScrollWheel") > 0f)
			{
				this.ScrollUp();
			}
		}
		if (this.m_thumb.IsDragging())
		{
			this.Drag();
		}
	}

	// Token: 0x060074E0 RID: 29920 RVA: 0x00228328 File Offset: 0x00226528
	public void Drag()
	{
		Vector3 min = this.m_track.GetComponent<BoxCollider>().bounds.min;
		Camera camera = CameraUtils.FindFirstByLayer(this.m_track.layer);
		Plane plane;
		plane..ctor(-camera.transform.forward, min);
		Ray ray = camera.ScreenPointToRay(UniversalInputManager.Get().GetMousePosition());
		float num;
		if (plane.Raycast(ray, ref num))
		{
			Vector3 vector = base.transform.InverseTransformPoint(ray.GetPoint(num));
			TransformUtil.SetLocalPosZ(this.m_thumb.gameObject, Mathf.Clamp(vector.z, this.m_sliderStartLocalPos.z, this.m_sliderEndLocalPos.z));
			this.m_scrollValue = Mathf.Clamp01((vector.z - this.m_sliderStartLocalPos.z) / (this.m_sliderEndLocalPos.z - this.m_sliderStartLocalPos.z));
			this.UpdateScrollAreaPosition(false);
		}
	}

	// Token: 0x060074E1 RID: 29921 RVA: 0x0022841E File Offset: 0x0022661E
	public virtual void Show()
	{
		this.m_isActive = true;
		this.ShowThumb(true);
		base.gameObject.SetActive(true);
	}

	// Token: 0x060074E2 RID: 29922 RVA: 0x0022843A File Offset: 0x0022663A
	public virtual void Hide()
	{
		this.m_isActive = false;
		this.ShowThumb(false);
		base.gameObject.SetActive(false);
	}

	// Token: 0x060074E3 RID: 29923 RVA: 0x00228458 File Offset: 0x00226658
	public void Init()
	{
		this.m_scrollValue = 0f;
		this.m_stepSize = 1f;
		this.m_thumb.transform.localPosition = this.m_sliderStartLocalPos;
		this.m_scrollAreaStartPos = this.m_scrollArea.transform.position;
		this.UpdateScrollAreaBounds();
	}

	// Token: 0x060074E4 RID: 29924 RVA: 0x002284B0 File Offset: 0x002266B0
	public void UpdateScrollAreaBounds()
	{
		this.GetBoundsOfChildren(this.m_scrollArea);
		float z = this.m_childrenBounds.size.z;
		float num = z - this.m_scrollWindowHeight;
		this.m_scrollAreaEndPos = this.m_scrollAreaStartPos;
		if (num <= 0f)
		{
			this.m_scrollValue = 0f;
			this.Hide();
		}
		else
		{
			int num2 = (int)Mathf.Ceil(num / 5f);
			this.m_stepSize = 1f / (float)num2;
			this.m_scrollAreaEndPos.z = this.m_scrollAreaEndPos.z + num;
			this.Show();
		}
		this.UpdateThumbPosition();
		this.UpdateScrollAreaPosition(false);
	}

	// Token: 0x060074E5 RID: 29925 RVA: 0x00228555 File Offset: 0x00226755
	public virtual bool InputIsOver()
	{
		return UniversalInputManager.Get().InputIsOver(base.gameObject);
	}

	// Token: 0x060074E6 RID: 29926 RVA: 0x00228567 File Offset: 0x00226767
	protected virtual void GetBoundsOfChildren(GameObject go)
	{
		this.m_childrenBounds = TransformUtil.GetBoundsOfChildren(go);
	}

	// Token: 0x060074E7 RID: 29927 RVA: 0x00228575 File Offset: 0x00226775
	public void OverrideScrollWindowHeight(float scrollWindowHeight)
	{
		this.m_scrollWindowHeight = scrollWindowHeight;
	}

	// Token: 0x060074E8 RID: 29928 RVA: 0x00228580 File Offset: 0x00226780
	protected void ShowThumb(bool show)
	{
		if (this.m_thumb != null)
		{
			this.m_thumb.gameObject.SetActive(show);
		}
	}

	// Token: 0x060074E9 RID: 29929 RVA: 0x002285B0 File Offset: 0x002267B0
	private void UpdateThumbPosition()
	{
		this.m_thumbPosition = Vector3.Lerp(this.m_sliderStartLocalPos, this.m_sliderEndLocalPos, Mathf.Clamp01(this.m_scrollValue));
		this.m_thumb.transform.localPosition = this.m_thumbPosition;
		this.m_thumb.transform.localScale = Vector3.one;
		if (this.m_scrollValue < 0f || this.m_scrollValue > 1f)
		{
			float num = 1f / ((this.m_scrollValue >= 0f) ? this.m_scrollValue : (-this.m_scrollValue + 1f));
			float z = this.m_thumb.transform.parent.InverseTransformPoint((this.m_scrollValue >= 0f) ? this.m_thumb.GetComponent<Renderer>().bounds.min : this.m_thumb.GetComponent<Renderer>().bounds.max).z;
			float num2 = (this.m_thumbPosition.z - z) * (num - 1f);
			TransformUtil.SetLocalPosZ(this.m_thumb, this.m_thumbPosition.z + num2);
			TransformUtil.SetLocalScaleZ(this.m_thumb, num);
		}
	}

	// Token: 0x060074EA RID: 29930 RVA: 0x002286F8 File Offset: 0x002268F8
	private void UpdateScrollAreaPosition(bool tween)
	{
		if (this.m_scrollArea == null)
		{
			return;
		}
		Vector3 vector = this.m_scrollAreaStartPos + this.m_scrollValue * (this.m_scrollAreaEndPos - this.m_scrollAreaStartPos);
		if (tween)
		{
			iTween.MoveTo(this.m_scrollArea, iTween.Hash(new object[]
			{
				"position",
				vector,
				"time",
				0.2f,
				"isLocal",
				false
			}));
		}
		else
		{
			this.m_scrollArea.transform.position = vector;
		}
	}

	// Token: 0x060074EB RID: 29931 RVA: 0x002287A8 File Offset: 0x002269A8
	public void ScrollTo(float value, bool clamp = true, bool lerp = true)
	{
		this.m_scrollValue = ((!clamp) ? value : Mathf.Clamp01(value));
		this.UpdateThumbPosition();
		this.UpdateScrollAreaPosition(lerp);
	}

	// Token: 0x060074EC RID: 29932 RVA: 0x002287CF File Offset: 0x002269CF
	private void ScrollUp()
	{
		this.Scroll(-this.m_stepSize, true);
	}

	// Token: 0x060074ED RID: 29933 RVA: 0x002287E0 File Offset: 0x002269E0
	public void Scroll(float amount, bool lerp = true)
	{
		this.m_scrollValue = Mathf.Clamp01(this.m_scrollValue + amount);
		this.UpdateThumbPosition();
		this.UpdateScrollAreaPosition(lerp);
	}

	// Token: 0x060074EE RID: 29934 RVA: 0x0022880D File Offset: 0x00226A0D
	private void ScrollDown()
	{
		this.Scroll(this.m_stepSize, true);
	}

	// Token: 0x04005F71 RID: 24433
	public ScrollBarThumb m_thumb;

	// Token: 0x04005F72 RID: 24434
	public GameObject m_track;

	// Token: 0x04005F73 RID: 24435
	public Vector3 m_sliderStartLocalPos;

	// Token: 0x04005F74 RID: 24436
	public Vector3 m_sliderEndLocalPos;

	// Token: 0x04005F75 RID: 24437
	public GameObject m_scrollArea;

	// Token: 0x04005F76 RID: 24438
	public BoxCollider m_scrollWindow;

	// Token: 0x04005F77 RID: 24439
	protected bool m_isActive = true;

	// Token: 0x04005F78 RID: 24440
	protected bool m_isDragging;

	// Token: 0x04005F79 RID: 24441
	protected float m_scrollValue;

	// Token: 0x04005F7A RID: 24442
	protected Vector3 m_scrollAreaStartPos;

	// Token: 0x04005F7B RID: 24443
	protected Vector3 m_scrollAreaEndPos;

	// Token: 0x04005F7C RID: 24444
	protected float m_stepSize;

	// Token: 0x04005F7D RID: 24445
	protected Vector3 m_thumbPosition;

	// Token: 0x04005F7E RID: 24446
	protected Bounds m_childrenBounds;

	// Token: 0x04005F7F RID: 24447
	protected float m_scrollWindowHeight;
}
