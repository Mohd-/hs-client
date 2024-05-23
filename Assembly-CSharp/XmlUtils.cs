using System;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

// Token: 0x02000348 RID: 840
public class XmlUtils
{
	// Token: 0x06002BD5 RID: 11221 RVA: 0x000DA020 File Offset: 0x000D8220
	public static string EscapeXPathSearchString(string search)
	{
		char[] array = new char[]
		{
			'\'',
			'"'
		};
		StringBuilder stringBuilder = new StringBuilder();
		int num = search.IndexOfAny(array);
		if (num == -1)
		{
			stringBuilder.Append('\'');
			stringBuilder.Append(search);
			stringBuilder.Append('\'');
		}
		else
		{
			stringBuilder.Append("concat(");
			int num2 = 0;
			while (num != -1)
			{
				stringBuilder.Append('\'');
				stringBuilder.Append(search, num2, num - num2);
				stringBuilder.Append("', ");
				string text;
				if (search.get_Chars(num) == '\'')
				{
					text = "\"'\", ";
				}
				else
				{
					text = "'\"', ";
				}
				stringBuilder.Append(text);
				num2 = num + 1;
				num = search.IndexOfAny(array, num + 1);
			}
			stringBuilder.Append('\'');
			stringBuilder.Append(search, num2, search.Length - num2);
			stringBuilder.Append("')");
		}
		return stringBuilder.ToString();
	}

	// Token: 0x06002BD6 RID: 11222 RVA: 0x000DA118 File Offset: 0x000D8318
	public static XmlDocument LoadXmlDocFromTextAsset(TextAsset asset)
	{
		string text = null;
		using (StringReader stringReader = new StringReader(asset.text))
		{
			text = stringReader.ReadToEnd();
		}
		if (text == null)
		{
			return null;
		}
		XmlDocument xmlDocument = new XmlDocument();
		xmlDocument.LoadXml(text);
		return xmlDocument;
	}

	// Token: 0x06002BD7 RID: 11223 RVA: 0x000DA174 File Offset: 0x000D8374
	public static void RemoveAllChildNodes(XmlNode node)
	{
		if (node == null)
		{
			return;
		}
		while (node.HasChildNodes)
		{
			node.RemoveChild(node.FirstChild);
		}
	}
}
