using System;
using UnityEngine;

// Token: 0x0200058D RID: 1421
public class MobileChatLogMessageFrame : MonoBehaviour, ITouchListItem
{
	// Token: 0x17000490 RID: 1168
	// (get) Token: 0x06004070 RID: 16496 RVA: 0x0013794C File Offset: 0x00135B4C
	// (set) Token: 0x06004071 RID: 16497 RVA: 0x00137959 File Offset: 0x00135B59
	public string Message
	{
		get
		{
			return this.text.Text;
		}
		set
		{
			this.text.Text = value;
			this.text.UpdateNow();
			this.UpdateLocalBounds();
		}
	}

	// Token: 0x17000491 RID: 1169
	// (get) Token: 0x06004072 RID: 16498 RVA: 0x00137978 File Offset: 0x00135B78
	public bool IsHeader
	{
		get
		{
			return false;
		}
	}

	// Token: 0x17000492 RID: 1170
	// (get) Token: 0x06004073 RID: 16499 RVA: 0x0013797B File Offset: 0x00135B7B
	// (set) Token: 0x06004074 RID: 16500 RVA: 0x0013797E File Offset: 0x00135B7E
	public bool Visible
	{
		get
		{
			return true;
		}
		set
		{
		}
	}

	// Token: 0x17000493 RID: 1171
	// (get) Token: 0x06004075 RID: 16501 RVA: 0x00137980 File Offset: 0x00135B80
	// (set) Token: 0x06004076 RID: 16502 RVA: 0x0013798D File Offset: 0x00135B8D
	public Color Color
	{
		get
		{
			return this.text.TextColor;
		}
		set
		{
			this.text.TextColor = value;
		}
	}

	// Token: 0x17000494 RID: 1172
	// (get) Token: 0x06004077 RID: 16503 RVA: 0x0013799B File Offset: 0x00135B9B
	// (set) Token: 0x06004078 RID: 16504 RVA: 0x001379A8 File Offset: 0x00135BA8
	public float Width
	{
		get
		{
			return this.text.Width;
		}
		set
		{
			this.text.Width = value;
			if (this.m_Background != null)
			{
				MeshFilter component = this.m_Background.GetComponent<MeshFilter>();
				float x = component.mesh.bounds.size.x;
				this.m_Background.transform.localScale = new Vector3(value / x, this.m_Background.transform.localScale.y, 1f);
				this.m_Background.transform.localPosition = new Vector3(-value / (0.5f * x), 0f, 0f);
			}
		}
	}

	// Token: 0x17000495 RID: 1173
	// (get) Token: 0x06004079 RID: 16505 RVA: 0x00137A59 File Offset: 0x00135C59
	public Bounds LocalBounds
	{
		get
		{
			return this.localBounds;
		}
	}

	// Token: 0x0600407A RID: 16506 RVA: 0x00137A64 File Offset: 0x00135C64
	private void UpdateLocalBounds()
	{
		Bounds textBounds = this.text.GetTextBounds();
		Vector3 size = textBounds.size;
		this.localBounds.center = base.transform.InverseTransformPoint(textBounds.center) + 10f * Vector3.up;
		this.localBounds.size = size;
	}

	// Token: 0x0600407B RID: 16507 RVA: 0x00137AC2 File Offset: 0x00135CC2
	virtual T GetComponent<T>()
	{
		return base.GetComponent<T>();
	}

	// Token: 0x0600407C RID: 16508 RVA: 0x00137ACA File Offset: 0x00135CCA
	virtual GameObject get_gameObject()
	{
		return base.gameObject;
	}

	// Token: 0x0600407D RID: 16509 RVA: 0x00137AD2 File Offset: 0x00135CD2
	virtual Transform get_transform()
	{
		return base.transform;
	}

	// Token: 0x0400291B RID: 10523
	public UberText text;

	// Token: 0x0400291C RID: 10524
	public GameObject m_Background;

	// Token: 0x0400291D RID: 10525
	private Bounds localBounds;
}
