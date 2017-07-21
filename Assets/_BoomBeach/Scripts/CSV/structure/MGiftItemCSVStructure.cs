using CSV;

public class MGiftItemCSVStructure : BaseCSVStructure
{
	[CsvColumn(CanBeNull = false)]
	public string name { get; set; }

	[CsvColumn(CanBeNull = false)]
	public int price { get; set; }

	[CsvColumn(CanBeNull = false)]
	public string resource_name { get; set; }
}
