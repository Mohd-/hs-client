using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200092B RID: 2347
[AddComponentMenu("Pixelplacement/iTweenPath")]
public class iTweenPath : MonoBehaviour
{
	// Token: 0x060056D1 RID: 22225 RVA: 0x001A08D0 File Offset: 0x0019EAD0
	public iTweenPath()
	{
		List<Vector3> list = new List<Vector3>();
		list.Add(Vector3.zero);
		list.Add(Vector3.zero);
		this.nodes = list;
		this.initialName = string.Empty;
		this.pathVisible = true;
		base..ctor();
	}

	// Token: 0x060056D3 RID: 22227 RVA: 0x001A093C File Offset: 0x0019EB3C
	private void OnEnable()
	{
		string key = iTweenPath.FixupPathName(this.pathName);
		if (!iTweenPath.paths.ContainsKey(key))
		{
			iTweenPath.paths.Add(key, this);
		}
	}

	// Token: 0x060056D4 RID: 22228 RVA: 0x001A0971 File Offset: 0x0019EB71
	private void OnDisable()
	{
		iTweenPath.paths.Remove(iTweenPath.FixupPathName(this.pathName));
	}

	// Token: 0x060056D5 RID: 22229 RVA: 0x001A098C File Offset: 0x0019EB8C
	private void OnDrawGizmosSelected()
	{
		if (this.pathVisible && this.nodes.Count > 0)
		{
			iTween.DrawPath(this.nodes.ToArray(), this.pathColor);
		}
	}

	// Token: 0x060056D6 RID: 22230 RVA: 0x001A09CC File Offset: 0x0019EBCC
	public static Vector3[] GetPath(string requestedName)
	{
		requestedName = iTweenPath.FixupPathName(requestedName);
		if (iTweenPath.paths.ContainsKey(requestedName))
		{
			return iTweenPath.paths[requestedName].nodes.ToArray();
		}
		Debug.Log("No path with that name (" + requestedName + ") exists! Are you sure you wrote it correctly?");
		return null;
	}

	// Token: 0x060056D7 RID: 22231 RVA: 0x001A0A20 File Offset: 0x0019EC20
	public static Vector3[] GetPathReversed(string requestedName)
	{
		requestedName = iTweenPath.FixupPathName(requestedName);
		if (iTweenPath.paths.ContainsKey(requestedName))
		{
			List<Vector3> range = iTweenPath.paths[requestedName].nodes.GetRange(0, iTweenPath.paths[requestedName].nodes.Count);
			range.Reverse();
			return range.ToArray();
		}
		Debug.Log("No path with that name (" + requestedName + ") exists! Are you sure you wrote it correctly?");
		return null;
	}

	// Token: 0x060056D8 RID: 22232 RVA: 0x001A0A94 File Offset: 0x0019EC94
	public static string FixupPathName(string name)
	{
		return name.ToLower();
	}

	// Token: 0x04003D8A RID: 15754
	public string pathName = string.Empty;

	// Token: 0x04003D8B RID: 15755
	public Color pathColor = Color.cyan;

	// Token: 0x04003D8C RID: 15756
	public List<Vector3> nodes;

	// Token: 0x04003D8D RID: 15757
	public static Map<string, iTweenPath> paths = new Map<string, iTweenPath>();

	// Token: 0x04003D8E RID: 15758
	public bool initialized;

	// Token: 0x04003D8F RID: 15759
	public string initialName;

	// Token: 0x04003D90 RID: 15760
	public bool pathVisible;
}
