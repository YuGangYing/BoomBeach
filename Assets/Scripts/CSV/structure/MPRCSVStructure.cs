using CSV;

public class MPRCSVStructure : BaseCSVStructure
{
	[CsvColumn(CanBeNull = true)]
	public string name { get; set; }

	[CsvColumn(CanBeNull = false)]
	public int m_character_id { get; set; }

	[CsvColumn(CanBeNull = true)]
	public int validity_days { get; set; }
}
