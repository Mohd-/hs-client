using System;

// Token: 0x020008D9 RID: 2265
public class ZoneChange
{
	// Token: 0x0600554C RID: 21836 RVA: 0x001981E8 File Offset: 0x001963E8
	public ZoneChangeList GetParentList()
	{
		return this.m_parentList;
	}

	// Token: 0x0600554D RID: 21837 RVA: 0x001981F0 File Offset: 0x001963F0
	public void SetParentList(ZoneChangeList parentList)
	{
		this.m_parentList = parentList;
	}

	// Token: 0x0600554E RID: 21838 RVA: 0x001981F9 File Offset: 0x001963F9
	public PowerTask GetPowerTask()
	{
		return this.m_powerTask;
	}

	// Token: 0x0600554F RID: 21839 RVA: 0x00198201 File Offset: 0x00196401
	public void SetPowerTask(PowerTask powerTask)
	{
		this.m_powerTask = powerTask;
	}

	// Token: 0x06005550 RID: 21840 RVA: 0x0019820A File Offset: 0x0019640A
	public Entity GetEntity()
	{
		return this.m_entity;
	}

	// Token: 0x06005551 RID: 21841 RVA: 0x00198212 File Offset: 0x00196412
	public void SetEntity(Entity entity)
	{
		this.m_entity = entity;
	}

	// Token: 0x06005552 RID: 21842 RVA: 0x0019821B File Offset: 0x0019641B
	public Zone GetDestinationZone()
	{
		return this.m_destinationZone;
	}

	// Token: 0x06005553 RID: 21843 RVA: 0x00198223 File Offset: 0x00196423
	public void SetDestinationZone(Zone zone)
	{
		this.m_destinationZone = zone;
	}

	// Token: 0x06005554 RID: 21844 RVA: 0x0019822C File Offset: 0x0019642C
	public TAG_ZONE GetDestinationZoneTag()
	{
		return this.m_destinationZoneTag;
	}

	// Token: 0x06005555 RID: 21845 RVA: 0x00198234 File Offset: 0x00196434
	public void SetDestinationZoneTag(TAG_ZONE tag)
	{
		this.m_destinationZoneTag = tag;
	}

	// Token: 0x06005556 RID: 21846 RVA: 0x00198240 File Offset: 0x00196440
	public int GetDestinationPosition()
	{
		int? destinationPos = this.m_destinationPos;
		return (destinationPos != null) ? this.m_destinationPos.Value : 0;
	}

	// Token: 0x06005557 RID: 21847 RVA: 0x00198274 File Offset: 0x00196474
	public void SetDestinationPosition(int pos)
	{
		this.m_destinationPos = new int?(pos);
	}

	// Token: 0x06005558 RID: 21848 RVA: 0x00198284 File Offset: 0x00196484
	public int GetDestinationControllerId()
	{
		int? destinationControllerId = this.m_destinationControllerId;
		return (destinationControllerId != null) ? this.m_destinationControllerId.Value : 0;
	}

	// Token: 0x06005559 RID: 21849 RVA: 0x001982B8 File Offset: 0x001964B8
	public void SetDestinationControllerId(int controllerId)
	{
		this.m_destinationControllerId = new int?(controllerId);
	}

	// Token: 0x0600555A RID: 21850 RVA: 0x001982C6 File Offset: 0x001964C6
	public Zone GetSourceZone()
	{
		return this.m_sourceZone;
	}

	// Token: 0x0600555B RID: 21851 RVA: 0x001982CE File Offset: 0x001964CE
	public void SetSourceZone(Zone zone)
	{
		this.m_sourceZone = zone;
	}

	// Token: 0x0600555C RID: 21852 RVA: 0x001982D7 File Offset: 0x001964D7
	public TAG_ZONE GetSourceZoneTag()
	{
		return this.m_sourceZoneTag;
	}

	// Token: 0x0600555D RID: 21853 RVA: 0x001982DF File Offset: 0x001964DF
	public void SetSourceZoneTag(TAG_ZONE tag)
	{
		this.m_sourceZoneTag = tag;
	}

	// Token: 0x0600555E RID: 21854 RVA: 0x001982E8 File Offset: 0x001964E8
	public int GetSourcePosition()
	{
		int? sourcePos = this.m_sourcePos;
		return (sourcePos != null) ? this.m_sourcePos.Value : 0;
	}

	// Token: 0x0600555F RID: 21855 RVA: 0x0019831C File Offset: 0x0019651C
	public void SetSourcePosition(int pos)
	{
		this.m_sourcePos = new int?(pos);
	}

	// Token: 0x06005560 RID: 21856 RVA: 0x0019832A File Offset: 0x0019652A
	public bool HasSourceZone()
	{
		return this.m_sourceZone != null;
	}

	// Token: 0x06005561 RID: 21857 RVA: 0x00198338 File Offset: 0x00196538
	public bool HasSourceZoneTag()
	{
		return this.m_sourceZoneTag != TAG_ZONE.INVALID;
	}

	// Token: 0x06005562 RID: 21858 RVA: 0x00198348 File Offset: 0x00196548
	public bool HasSourcePosition()
	{
		int? sourcePos = this.m_sourcePos;
		return sourcePos != null;
	}

	// Token: 0x06005563 RID: 21859 RVA: 0x00198363 File Offset: 0x00196563
	public bool HasSourceData()
	{
		return this.HasSourceZoneTag() || this.HasSourcePosition();
	}

	// Token: 0x06005564 RID: 21860 RVA: 0x00198380 File Offset: 0x00196580
	public bool HasDestinationZone()
	{
		return this.m_destinationZone != null;
	}

	// Token: 0x06005565 RID: 21861 RVA: 0x0019838E File Offset: 0x0019658E
	public bool HasDestinationZoneTag()
	{
		return this.m_destinationZoneTag != TAG_ZONE.INVALID;
	}

	// Token: 0x06005566 RID: 21862 RVA: 0x0019839C File Offset: 0x0019659C
	public bool HasDestinationPosition()
	{
		int? destinationPos = this.m_destinationPos;
		return destinationPos != null;
	}

	// Token: 0x06005567 RID: 21863 RVA: 0x001983B8 File Offset: 0x001965B8
	public bool HasDestinationControllerId()
	{
		int? destinationControllerId = this.m_destinationControllerId;
		return destinationControllerId != null;
	}

	// Token: 0x06005568 RID: 21864 RVA: 0x001983D4 File Offset: 0x001965D4
	public bool HasDestinationData()
	{
		return this.HasDestinationZoneTag() || this.HasDestinationPosition() || this.HasDestinationControllerId();
	}

	// Token: 0x06005569 RID: 21865 RVA: 0x00198409 File Offset: 0x00196609
	public bool HasDestinationZoneChange()
	{
		return this.HasDestinationZoneTag() || this.HasDestinationControllerId();
	}

	// Token: 0x0600556A RID: 21866 RVA: 0x00198428 File Offset: 0x00196628
	public override string ToString()
	{
		return string.Format("powerTask=[{0}] entity={1} srcZoneTag={2} srcPos={3} dstZoneTag={4} dstPos={5}", new object[]
		{
			this.m_powerTask,
			this.m_entity,
			this.m_sourceZoneTag,
			this.m_sourcePos,
			this.m_destinationZoneTag,
			this.m_destinationPos
		});
	}

	// Token: 0x04003B52 RID: 15186
	private ZoneChangeList m_parentList;

	// Token: 0x04003B53 RID: 15187
	private PowerTask m_powerTask;

	// Token: 0x04003B54 RID: 15188
	private Entity m_entity;

	// Token: 0x04003B55 RID: 15189
	private Zone m_sourceZone;

	// Token: 0x04003B56 RID: 15190
	private TAG_ZONE m_sourceZoneTag;

	// Token: 0x04003B57 RID: 15191
	private int? m_sourcePos;

	// Token: 0x04003B58 RID: 15192
	private Zone m_destinationZone;

	// Token: 0x04003B59 RID: 15193
	private TAG_ZONE m_destinationZoneTag;

	// Token: 0x04003B5A RID: 15194
	private int? m_destinationPos;

	// Token: 0x04003B5B RID: 15195
	private int? m_destinationControllerId;
}
