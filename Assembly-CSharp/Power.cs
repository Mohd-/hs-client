using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.XPath;

// Token: 0x02000345 RID: 837
public class Power
{
	// Token: 0x06002BCA RID: 11210 RVA: 0x000D9A8F File Offset: 0x000D7C8F
	public string GetDefinition()
	{
		return this.mDefinition;
	}

	// Token: 0x06002BCB RID: 11211 RVA: 0x000D9A97 File Offset: 0x000D7C97
	public PlayErrors.PlayRequirementInfo GetPlayRequirementInfo()
	{
		return this.mPlayRequirementInfo;
	}

	// Token: 0x06002BCC RID: 11212 RVA: 0x000D9AA0 File Offset: 0x000D7CA0
	public static Power GetDefaultAttackPower()
	{
		if (Power.s_defaultAttackPower == null)
		{
			Power.s_defaultAttackPower = new Power();
			List<PlayErrors.ErrorType> list = new List<PlayErrors.ErrorType>();
			list.Add(PlayErrors.ErrorType.REQ_TARGET_TO_PLAY);
			list.Add(PlayErrors.ErrorType.REQ_ENEMY_TARGET);
			Power.s_defaultAttackPower.mPlayRequirementInfo.requirementsMap = PlayErrors.GetRequirementsMap(list);
		}
		return Power.s_defaultAttackPower;
	}

	// Token: 0x06002BCD RID: 11213 RVA: 0x000D9AF0 File Offset: 0x000D7CF0
	public static Power GetDefaultMasterPower()
	{
		if (Power.s_defaultMasterPower == null)
		{
			Power.s_defaultMasterPower = new Power();
			List<PlayErrors.ErrorType> requirements = new List<PlayErrors.ErrorType>();
			Power.s_defaultMasterPower.mPlayRequirementInfo.requirementsMap = PlayErrors.GetRequirementsMap(requirements);
		}
		return Power.s_defaultMasterPower;
	}

	// Token: 0x06002BCE RID: 11214 RVA: 0x000D9B34 File Offset: 0x000D7D34
	public static Power Create(string definition, List<Power.PowerInfo> infos)
	{
		Power power = new Power();
		power.mDefinition = definition;
		List<PlayErrors.ErrorType> list = new List<PlayErrors.ErrorType>();
		if (infos != null && infos.Count > 0)
		{
			foreach (Power.PowerInfo powerInfo in infos)
			{
				PlayErrors.ErrorType reqId = powerInfo.reqId;
				int param = powerInfo.param;
				PlayErrors.ErrorType errorType = reqId;
				switch (errorType)
				{
				case PlayErrors.ErrorType.REQ_TARGET_MAX_ATTACK:
					power.mPlayRequirementInfo.paramMaxAtk = param;
					break;
				default:
					if (errorType != PlayErrors.ErrorType.REQ_MINION_CAP_IF_TARGET_AVAILABLE)
					{
						if (errorType != PlayErrors.ErrorType.REQ_MINIMUM_ENEMY_MINIONS)
						{
							if (errorType != PlayErrors.ErrorType.REQ_TARGET_MIN_ATTACK)
							{
								if (errorType != PlayErrors.ErrorType.REQ_MINIMUM_TOTAL_MINIONS)
								{
									if (errorType == PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS)
									{
										power.mPlayRequirementInfo.paramMinNumFriendlyMinions = param;
									}
								}
								else
								{
									power.mPlayRequirementInfo.paramMinNumTotalMinions = param;
								}
							}
							else
							{
								power.mPlayRequirementInfo.paramMinAtk = param;
							}
						}
						else
						{
							power.mPlayRequirementInfo.paramMinNumEnemyMinions = param;
						}
					}
					else
					{
						power.mPlayRequirementInfo.paramNumMinionSlotsWithTarget = param;
					}
					break;
				case PlayErrors.ErrorType.REQ_TARGET_WITH_RACE:
					power.mPlayRequirementInfo.paramRace = param;
					break;
				case PlayErrors.ErrorType.REQ_NUM_MINION_SLOTS:
					power.mPlayRequirementInfo.paramNumMinionSlots = param;
					break;
				}
				list.Add(reqId);
			}
		}
		power.mPlayRequirementInfo.requirementsMap = PlayErrors.GetRequirementsMap(list);
		return power;
	}

	// Token: 0x06002BCF RID: 11215 RVA: 0x000D9CC4 File Offset: 0x000D7EC4
	public static Power LoadFromXml(XmlElement rootElement)
	{
		Power power = new Power();
		power.mDefinition = rootElement.GetAttribute("definition");
		XPathNavigator xpathNavigator = rootElement.CreateNavigator();
		XPathExpression xpathExpression = xpathNavigator.Compile("./PlayRequirement");
		XPathNodeIterator xpathNodeIterator = xpathNavigator.Select(xpathExpression);
		List<PlayErrors.ErrorType> list = new List<PlayErrors.ErrorType>();
		while (xpathNodeIterator.MoveNext())
		{
			XPathNavigator xpathNavigator2 = xpathNodeIterator.Current;
			XmlElement xmlElement = (XmlElement)((IHasXmlNode)xpathNavigator2).GetNode();
			int num;
			if (int.TryParse(xmlElement.GetAttribute("reqID"), ref num))
			{
				PlayErrors.ErrorType errorType = (PlayErrors.ErrorType)num;
				list.Add(errorType);
				if (errorType == PlayErrors.ErrorType.REQ_TARGET_MIN_ATTACK)
				{
					if (!int.TryParse(xmlElement.GetAttribute("param"), ref power.mPlayRequirementInfo.paramMinAtk))
					{
						Log.Rachelle.Print(string.Format("Unable to read play requirement param minAtk for power {0}.", power.GetDefinition()), new object[0]);
					}
				}
				else if (errorType == PlayErrors.ErrorType.REQ_TARGET_MAX_ATTACK)
				{
					if (!int.TryParse(xmlElement.GetAttribute("param"), ref power.mPlayRequirementInfo.paramMaxAtk))
					{
						Log.Rachelle.Print(string.Format("Unable to read play requirement param maxAtk for power {0}.", power.GetDefinition()), new object[0]);
					}
				}
				else if (errorType == PlayErrors.ErrorType.REQ_TARGET_WITH_RACE)
				{
					if (!int.TryParse(xmlElement.GetAttribute("param"), ref power.mPlayRequirementInfo.paramRace))
					{
						Log.Rachelle.Print(string.Format("Unable to read play requirement param race for power {0}.", power.GetDefinition()), new object[0]);
					}
				}
				else if (errorType == PlayErrors.ErrorType.REQ_NUM_MINION_SLOTS)
				{
					if (!int.TryParse(xmlElement.GetAttribute("param"), ref power.mPlayRequirementInfo.paramNumMinionSlots))
					{
						Log.Rachelle.Print(string.Format("Unable to read play requirement param num minion slots for power {0}.", power.GetDefinition()), new object[0]);
					}
				}
				else if (errorType == PlayErrors.ErrorType.REQ_MINION_CAP_IF_TARGET_AVAILABLE)
				{
					if (!int.TryParse(xmlElement.GetAttribute("param"), ref power.mPlayRequirementInfo.paramNumMinionSlotsWithTarget))
					{
						Log.Rachelle.Print(string.Format("Unable to read play requirement param num minion slots with target for power {0}.", power.GetDefinition()), new object[0]);
					}
				}
				else if (errorType == PlayErrors.ErrorType.REQ_MINIMUM_ENEMY_MINIONS)
				{
					if (!int.TryParse(xmlElement.GetAttribute("param"), ref power.mPlayRequirementInfo.paramMinNumEnemyMinions))
					{
						Log.Rachelle.Print(string.Format("Unable to read play requirement param num enemy minions for power {0}.", power.GetDefinition()), new object[0]);
					}
				}
				else if (errorType == PlayErrors.ErrorType.REQ_MINIMUM_TOTAL_MINIONS)
				{
					if (!int.TryParse(xmlElement.GetAttribute("param"), ref power.mPlayRequirementInfo.paramMinNumTotalMinions))
					{
						Log.Rachelle.Print(string.Format("Unable to read play requirement param num total minions for power {0}.", power.GetDefinition()), new object[0]);
					}
				}
				else if (errorType == PlayErrors.ErrorType.REQ_TARGET_IF_AVAILABLE_AND_MINIMUM_FRIENDLY_MINIONS && !int.TryParse(xmlElement.GetAttribute("param"), ref power.mPlayRequirementInfo.paramMinNumFriendlyMinions))
				{
					Log.Rachelle.Print(string.Format("Unable to read play requirement param num friendly minions for power {0}.", power.GetDefinition()), new object[0]);
				}
			}
		}
		power.mPlayRequirementInfo.requirementsMap = PlayErrors.GetRequirementsMap(list);
		return power;
	}

	// Token: 0x04001A76 RID: 6774
	private string mDefinition = string.Empty;

	// Token: 0x04001A77 RID: 6775
	private PlayErrors.PlayRequirementInfo mPlayRequirementInfo = new PlayErrors.PlayRequirementInfo();

	// Token: 0x04001A78 RID: 6776
	private static Power s_defaultAttackPower;

	// Token: 0x04001A79 RID: 6777
	private static Power s_defaultMasterPower;

	// Token: 0x02000347 RID: 839
	public struct PowerInfo
	{
		// Token: 0x04001A7C RID: 6780
		public PlayErrors.ErrorType reqId;

		// Token: 0x04001A7D RID: 6781
		public int param;
	}
}
