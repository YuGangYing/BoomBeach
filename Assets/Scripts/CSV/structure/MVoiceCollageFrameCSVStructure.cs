using CSV;

public class MVoiceCollageFrameCSVStructure : BaseCSVStructure
{
	[CsvColumn(CanBeNull = false)]
	public int m_character_id { get; set; }

	[CsvColumn(CanBeNull = false)]
	public int default_num { get; set; }

	[CsvColumn(CanBeNull = false)]
	public int max_num { get; set; }

	[CsvColumn(CanBeNull = false)]
	public int extend_cost { get; set; }
}
