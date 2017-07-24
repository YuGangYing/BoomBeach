using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CSV;

[System.Serializable]
public class MIslandGridCSVStructure : BaseCSVStructure {

	[CsvColumn (CanBeNull = true)]
	public string name{ get; set;}

	[CsvColumn (CanBeNull = true)]
	public string x{ get; set;}

	[CsvColumn (CanBeNull = true)]
	public string y{ get; set;}

}
