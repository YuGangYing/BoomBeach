using System.Collections.Generic;
using CSV;

[System.Serializable]
public class MUnitCSVStructure : BaseCSVStructure
{

	[CsvColumn (CanBeNull = true)]
	public string name{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string category{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string tid{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string info_tid{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string subtitle_tid{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public int level{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string build_cost{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public int hit_points{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public int width{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public int height{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public int build_time_d{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public int build_time_h{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public int build_time_m{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public int build_time_s{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public int require_town_hall_level{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string max_stored_resource{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string produces_resource{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public int resource_per_hour{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public int resource_max{ get; set; }

	[CsvColumn (CanBeNull = true)]
	public string export_name{ get; set; }

	public string tid_level{ get { return tid + "_" + level; } }

	public MProduceUnitCSVStructure produceUnit;

	public MSkillCSVStructure skill;

	//regen_time,defense_value,
	//attack_range,attack_speed,damage,air_targets,ground_targets,min_attack_range,damage_radius,damage_spread,push_back,creates_artifacts,artifact_type,
	//artifact_capacity,deepsea_resource_reward,deepsea_common_artifact_chance,deepsea_rare_artifact_chance,deepsea_epic_artifact_chance,deepsea_random_factor,
	//deepsea_price,is_map_room,starting_energy,max_energy,energy_gain,is_vault,is_hero_house,landing_boats,can_not_move,is_global_building,explorable_regions,
	//reload_time,shots_before_reload,resource_protection_percent,damage_over_five_seconds,counters_armored,xp_gain,swf,export_name,export_name_npc,
	//export_name_construction,model_name,texture_name,mesh_name,icon,export_name_build_anim,destroy_effect,attack_effect,attack_effect2,hit_effect,
	//projectile,export_name_damaged,export_name_base,export_name_base_npc,pickup_effect,placing_effect,defender_character,defender_count,defender_z,
	//export_name_tiggered,miss_effect,building_ready_effect,export_name_boss,proj_skewing,proj_y_scaling

}
