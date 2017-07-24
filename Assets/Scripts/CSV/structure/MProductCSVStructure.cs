using CSV;

public class MProductCSVStructure : BaseCSVStructure
{
    [CsvColumn(CanBeNull = true)]
    public string product_id { get; set; }

    [CsvColumn(CanBeNull = true)]
    public int platform_type { get; set; }

    [CsvColumn(CanBeNull = true)]
    public string platform_name { get; set; }

    [CsvColumn(CanBeNull = true)]
    public string name { get; set; }

    [CsvColumn(CanBeNull = true)]
    public string description { get; set; }

    [CsvColumn(CanBeNull = true)]
    public string image_resource_name { get; set; }

    [CsvColumn(CanBeNull = true)]
    public int coin { get; set; }

    [CsvColumn(CanBeNull = true)]
    public int free_coin { get; set; }

    [CsvColumn(CanBeNull = true)]
    public int jpy_amount { get; set; }

}
