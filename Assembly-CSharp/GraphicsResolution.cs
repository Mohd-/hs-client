using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000521 RID: 1313
public class GraphicsResolution : IComparable
{
	// Token: 0x06003D15 RID: 15637 RVA: 0x00127190 File Offset: 0x00125390
	private GraphicsResolution()
	{
	}

	// Token: 0x06003D16 RID: 15638 RVA: 0x00127198 File Offset: 0x00125398
	private GraphicsResolution(int width, int height)
	{
		this.x = width;
		this.y = height;
		this.aspectRatio = (float)this.x / (float)this.y;
	}

	// Token: 0x06003D18 RID: 15640 RVA: 0x001271DA File Offset: 0x001253DA
	public static GraphicsResolution create(Resolution res)
	{
		return new GraphicsResolution(res.width, res.height);
	}

	// Token: 0x06003D19 RID: 15641 RVA: 0x001271EF File Offset: 0x001253EF
	public static GraphicsResolution create(int width, int height)
	{
		return new GraphicsResolution(width, height);
	}

	// Token: 0x06003D1A RID: 15642 RVA: 0x001271F8 File Offset: 0x001253F8
	private static bool add(int width, int height)
	{
		GraphicsResolution graphicsResolution = new GraphicsResolution(width, height);
		if (GraphicsResolution.resolutions_.BinarySearch(graphicsResolution) >= 0)
		{
			return false;
		}
		GraphicsResolution.resolutions_.Add(graphicsResolution);
		GraphicsResolution.resolutions_.Sort();
		return true;
	}

	// Token: 0x17000450 RID: 1104
	// (get) Token: 0x06003D1B RID: 15643 RVA: 0x00127238 File Offset: 0x00125438
	public static List<GraphicsResolution> list
	{
		get
		{
			if (GraphicsResolution.resolutions_.Count == 0)
			{
				List<GraphicsResolution> list = GraphicsResolution.resolutions_;
				lock (list)
				{
					foreach (Resolution resolution in Screen.resolutions)
					{
						GraphicsResolution.add(resolution.width, resolution.height);
					}
				}
			}
			return GraphicsResolution.resolutions_;
		}
	}

	// Token: 0x17000451 RID: 1105
	// (get) Token: 0x06003D1C RID: 15644 RVA: 0x001272BC File Offset: 0x001254BC
	public static GraphicsResolution current
	{
		get
		{
			return GraphicsResolution.create(Screen.currentResolution);
		}
	}

	// Token: 0x17000452 RID: 1106
	// (get) Token: 0x06003D1D RID: 15645 RVA: 0x001272C8 File Offset: 0x001254C8
	// (set) Token: 0x06003D1E RID: 15646 RVA: 0x001272D0 File Offset: 0x001254D0
	public int x { get; private set; }

	// Token: 0x17000453 RID: 1107
	// (get) Token: 0x06003D1F RID: 15647 RVA: 0x001272D9 File Offset: 0x001254D9
	// (set) Token: 0x06003D20 RID: 15648 RVA: 0x001272E1 File Offset: 0x001254E1
	public int y { get; private set; }

	// Token: 0x17000454 RID: 1108
	// (get) Token: 0x06003D21 RID: 15649 RVA: 0x001272EA File Offset: 0x001254EA
	// (set) Token: 0x06003D22 RID: 15650 RVA: 0x001272F2 File Offset: 0x001254F2
	public float aspectRatio { get; private set; }

	// Token: 0x06003D23 RID: 15651 RVA: 0x001272FC File Offset: 0x001254FC
	public int CompareTo(object obj)
	{
		GraphicsResolution graphicsResolution = obj as GraphicsResolution;
		if (graphicsResolution == null)
		{
			return 1;
		}
		if (this.x < graphicsResolution.x)
		{
			return -1;
		}
		if (this.x > graphicsResolution.x)
		{
			return 1;
		}
		if (this.y < graphicsResolution.y)
		{
			return -1;
		}
		if (this.y > graphicsResolution.y)
		{
			return 1;
		}
		return 0;
	}

	// Token: 0x06003D24 RID: 15652 RVA: 0x00127368 File Offset: 0x00125568
	public override bool Equals(object obj)
	{
		if (obj == null)
		{
			return false;
		}
		GraphicsResolution graphicsResolution = obj as GraphicsResolution;
		return graphicsResolution != null && this.x == graphicsResolution.x && this.y == graphicsResolution.y;
	}

	// Token: 0x06003D25 RID: 15653 RVA: 0x001273B0 File Offset: 0x001255B0
	public override int GetHashCode()
	{
		int num = 23;
		num = num * 17 + this.x.GetHashCode();
		return num * 17 + this.y.GetHashCode();
	}

	// Token: 0x040026DA RID: 9946
	public static readonly List<GraphicsResolution> resolutions_ = new List<GraphicsResolution>();
}
