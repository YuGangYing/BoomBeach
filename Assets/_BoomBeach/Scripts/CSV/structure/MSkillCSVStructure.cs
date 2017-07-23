using System.Collections.Generic;
using CSV;

public class MSkillCSVStructure : BaseCSVStructure {

	[CsvColumn (CanBeNull = true)]
	public int name;

	[CsvColumn (CanBeNull = true)]
	public int attack_range;

	[CsvColumn (CanBeNull = true)]
	public int min_attack_range;

	[CsvColumn (CanBeNull = true)]
	public int attack_speed;

	[CsvColumn (CanBeNull = true)]
	public int damage;

	[CsvColumn (CanBeNull = true)]
	public int damage_radius;

	[CsvColumn (CanBeNull = true)]
	public int reload_time;

	[CsvColumn (CanBeNull = true)]
	public int shots_before_reload;
}
