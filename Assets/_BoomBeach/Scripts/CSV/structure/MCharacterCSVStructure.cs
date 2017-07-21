using CSV;
using System.Collections.Generic;

public class MCharacterCSVStructure : BaseCSVStructure
{
	[CsvColumn (CanBeNull = true)]
	public string name{ get; set; }

	[CsvColumn (CanBeNull = false)]
	public string gender_type{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string description{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string birthday{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string cv_name{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string resource_name{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string icon_name{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string icon_select_name{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string base_select_name{ get; set; }

	public Dictionary<int,List<MVoiceCSVStructure>> voiceDicByCondition;

}
