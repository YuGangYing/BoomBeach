using System.Collections.Generic;
using CSV;

public class MProduceUnitCSVStructure : BaseCSVStructure {

	[CsvColumn (CanBeNull = true)]
	public string unit_tid;

	[CsvColumn (CanBeNull = true)]
	public int unit_space;

}
