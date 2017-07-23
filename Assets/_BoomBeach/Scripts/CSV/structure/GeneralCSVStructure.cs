using CSV;

[System.Serializable]
public class GeneralCSVStructure : BaseCSVStructure
{
	[CsvColumn (CanBeNull = true)]
	public string title{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string description{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string name{ get; set; }
}
