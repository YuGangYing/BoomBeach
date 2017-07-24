using CSV;

public class MVoicePlayConditionCSVStructure : BaseCSVStructure
{
	[CsvColumn (CanBeNull = true)]
	public int target_type{ get; set; } 
	//1.キャラ選択中 2.キャラ決定 3.タップ 4.スワイプ 5.シェイク 6.会話(外出) 7.会話(天気) 8.会話(近況) 9.会話(将来) 10.連続未ログイン日数 11.キャラ誕生日
  //12.プレイヤー誕生日 13.ギフト付与 14.ガチャ抽選前 15.ガチャ確変 16.ガチャおまけ獲得 17.PRチケット獲得 
	

	[CsvColumn (CanBeNull = true)]
	public int target_value{ get; set; } 
	//target_type = 10 : 日数 / target_type = 13 : m_gift_item.id

	[CsvColumn (CanBeNull = true)]
	public int time_frame{ get; set; } 
}
