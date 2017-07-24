using CSV;

public class MCharacterMotionCSVStructure : BaseCSVStructure
{
	[CsvColumn (CanBeNull = true)]
	public string name{ get; set; }
	//idol/joy/angry/aha

	[CsvColumn (CanBeNull = true)]
	public string display_name{ get; set; }
}
