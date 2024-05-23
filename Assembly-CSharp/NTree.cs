using System;
using System.Collections.Generic;

// Token: 0x02000EDF RID: 3807
public class NTree<T>
{
	// Token: 0x06007218 RID: 29208 RVA: 0x00218C77 File Offset: 0x00216E77
	public NTree(T data)
	{
		this.data = data;
		this.children = new LinkedList<NTree<T>>();
	}

	// Token: 0x06007219 RID: 29209 RVA: 0x00218C94 File Offset: 0x00216E94
	public void AddDeepChild(params T[] traverse)
	{
		LinkedList<NTree<T>> linkedList = this.children;
		foreach (T t in traverse)
		{
			bool flag = false;
			foreach (NTree<T> ntree in linkedList)
			{
				if (EqualityComparer<T>.Default.Equals(ntree.data, t))
				{
					linkedList = ntree.children;
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				NTree<T> ntree2 = new NTree<T>(t);
				linkedList.AddLast(ntree2);
				linkedList = ntree2.children;
			}
		}
	}

	// Token: 0x0600721A RID: 29210 RVA: 0x00218D54 File Offset: 0x00216F54
	public void SetData(T data)
	{
		this.data = data;
	}

	// Token: 0x0600721B RID: 29211 RVA: 0x00218D5D File Offset: 0x00216F5D
	public void Traverse(TreeVisitor<T> visitor, TreePreTraverse previsitor, TreePostTraverse postvisitor, int ignoredepth = -1)
	{
		this.traverse(this, visitor, previsitor, postvisitor, ignoredepth);
	}

	// Token: 0x0600721C RID: 29212 RVA: 0x00218D6C File Offset: 0x00216F6C
	private void traverse(NTree<T> node, TreeVisitor<T> visitor, TreePreTraverse previsitor, TreePostTraverse postvisitor, int ignoredepth)
	{
		if (visitor != null && ignoredepth < 0 && !visitor(node.data))
		{
			return;
		}
		foreach (NTree<T> node2 in node.children)
		{
			if (previsitor != null && ignoredepth < 0)
			{
				previsitor();
			}
			this.traverse(node2, visitor, previsitor, postvisitor, ignoredepth - 1);
			if (postvisitor != null && ignoredepth < 0)
			{
				postvisitor();
			}
		}
	}

	// Token: 0x04005C40 RID: 23616
	private T data;

	// Token: 0x04005C41 RID: 23617
	private LinkedList<NTree<T>> children;
}
