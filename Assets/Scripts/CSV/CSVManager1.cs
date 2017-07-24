//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using BattleFramework.Data;
//using CSV;
//using System.IO;
//
//public class CSVManager : SingleMonoBehaviour<CSVManager>
//{
//	//	public const string CSV_PATH = @"Assets/CSV";//
//	private const string CSV_CONVENTION = "m_convention";
//	//	public const string CSV_HELP = "m_help";
//	private const string CSV_NG = "m_ng";
//	private const string CSV_QA = "m_qa";
//	private const string CSV_VOICE = "m_voice";
//	private const string CSV_VOICE_PLAY_CONDITION = "m_voice_play_condition";
//	private const string CSV_CHARACTER = "m_character";
//	private const string CSV_CHARACTER_MOTION = "m_character_motion";
//	private const string CSV_PRODUCT = "m_product";
//	private const string CSV_PR = "m_pr";
//	private const string CSV_GIFT = "m_gift_item";
//	private const string CSV_VCFrame = "m_voicecollage_frame";
//  
//	private CsvContext mCsvContext;
//
//	public List<VersionCSVStructure> VersionList { get; private set; }
//
//	public List<GeneralCSVStructure> ConventionList { get; private set; }
//
//	public Dictionary<int, GeneralCSVStructure> ConventionDic { get; private set; }
//
//	public List<GeneralCSVStructure> NgList { get; private set; }
//
//	public Dictionary<int, GeneralCSVStructure> NgDic { get; private set; }
//
//	public List<GeneralCSVStructure> QaList { get; private set; }
//
//	public Dictionary<int, GeneralCSVStructure> QaDic { get; private set; }
//
//	public List<MVoiceCSVStructure> VoiceList { get; private set; }
//
//	public Dictionary<int, MVoiceCSVStructure> VoiceDic { get; private set; }
//
//	public  Dictionary<int, List<MVoiceCSVStructure>> VoiceDicByCharacterId { get; private set; }
//
//	public List<MVoicePlayConditionCSVStructure> VoicePlayConditionList { get; private set; }
//
//	public Dictionary<int, MVoicePlayConditionCSVStructure> VoicePlayConditionDic { get; private set; }
//
//	public List<MCharacterCSVStructure> CharacterList { get; private set; }
//
//	public Dictionary<int, MCharacterCSVStructure> CharacterDic { get; private set; }
//
//	public List<MCharacterMotionCSVStructure> CharacterMotionList { get; private set; }
//
//	public Dictionary<int, MCharacterMotionCSVStructure> CharacterMotionDic { get; private set; }
//
//	public List<MProductCSVStructure> ProductList { get; private set; }
//
//	public List<MPRCSVStructure> PRList { get; private set; }
//
//	public Dictionary<int, MPRCSVStructure> PRDic { get; private set; }
//
//	public List<MGiftItemCSVStructure> GiftList { get; private set; }
//
//	public Dictionary<int, MGiftItemCSVStructure> GiftDic { get; private set; }
//
//	public List<MVoiceCollageFrameCSVStructure> VCFrameList { get; private set; }
//
//	public Dictionary<int, MVoiceCollageFrameCSVStructure> VCFrameDic { get; private set; }
//
//
//	void Awake ()
//	{
//		LoadCSV ();
//	}
//
//	byte[] GetCSV (string fileName)
//	{
//		#if UNITY_EDITOR
//		return Resources.Load<TextAsset> ("CSV/" + fileName).bytes;
//		#else
//		return ResourcesManager.Ins.GetCSV (fileName);
//		#endif
//	}
//
//	void LoadCSV ()
//	{
//		mCsvContext = new CsvContext ();
//		LoadNG ();
//		LoadQA ();
//		LoadConvention ();
//		LoadCharacter ();
//		LoadVoice ();
//		LoadCharacterMotion ();
//		LoadVoicePlayCondition ();
//		LoadProduct ();
//		LoadPR ();
//		LoadGift ();
//		LoadVCFrame ();
//	}
//
//	void LoadNG ()
//	{
//		NgList = CreateCSVList<GeneralCSVStructure> (CSV_NG);
//		NgDic = GetDictionary (NgList);
//	}
//
//	void LoadConvention ()
//	{
//		ConventionList = CreateCSVList<GeneralCSVStructure> (CSV_CONVENTION);
//		ConventionDic = GetDictionary (ConventionList);
//	}
//
//	void LoadQA ()
//	{
//		QaList = CreateCSVList<GeneralCSVStructure> (CSV_QA);
//		QaDic = GetDictionary (QaList);
//	}
//
//	void LoadVoice ()
//	{
//		VoiceList = CreateCSVList<MVoiceCSVStructure> (CSV_VOICE);
//		VoiceDic = GetDictionary (VoiceList);
//		VoiceDicByCharacterId = new Dictionary<int, List<MVoiceCSVStructure>> ();
//		for (int i = 0; i < VoiceList.Count; i++) {
//			if (!VoiceDicByCharacterId.ContainsKey (VoiceList [i].m_character_id))
//				VoiceDicByCharacterId.Add (VoiceList [i].m_character_id, new List<MVoiceCSVStructure> ());
//			VoiceDicByCharacterId [VoiceList [i].m_character_id].Add (VoiceList [i]);
//
//			if(CharacterDic.ContainsKey(VoiceList [i].m_character_id)){
//				MCharacterCSVStructure charaCSV = CharacterDic[VoiceList [i].m_character_id];
//				if(charaCSV.voiceDicByCondition == null){
//					charaCSV.voiceDicByCondition = new Dictionary<int, List<MVoiceCSVStructure>> ();
//				}
//				if(!charaCSV.voiceDicByCondition.ContainsKey(VoiceList [i].m_voice_play_condition_id)){
//					charaCSV.voiceDicByCondition.Add (VoiceList [i].m_voice_play_condition_id,new List<MVoiceCSVStructure>());
//				}
//				charaCSV.voiceDicByCondition [VoiceList [i].m_voice_play_condition_id].Add (VoiceList [i]);
//			}
//		}
//		Debug.Log ("characterVoiceCount:" + VoiceDicByCharacterId.Count);
//	}
//
//	void LoadCharacter ()
//	{
//		CharacterList = CreateCSVList<MCharacterCSVStructure> (CSV_CHARACTER);
//		CharacterDic = GetDictionary (CharacterList);
//	}
//
//	void LoadCharacterMotion ()
//	{
//		CharacterMotionList = CreateCSVList<MCharacterMotionCSVStructure> (CSV_CHARACTER_MOTION);
//		CharacterMotionDic = GetDictionary (CharacterMotionList);
//	}
//
//	void LoadVoicePlayCondition ()
//	{
//		VoicePlayConditionList = CreateCSVList<MVoicePlayConditionCSVStructure> (CSV_VOICE_PLAY_CONDITION);
//		VoicePlayConditionDic = GetDictionary (VoicePlayConditionList);
//	}
//
//	void LoadPR ()
//	{
//		PRList = CreateCSVList<MPRCSVStructure> (CSV_PR);
//		PRDic = GetDictionary (PRList);
//	}
//
//	void LoadGift ()
//	{
//		GiftList = CreateCSVList<MGiftItemCSVStructure> (CSV_GIFT);
//		GiftDic = GetDictionary (GiftList);
//	}
//
//	void LoadVCFrame ()
//	{
//		VCFrameList = CreateCSVList<MVoiceCollageFrameCSVStructure> (CSV_VCFrame);
//		VCFrameDic = GetDictionary (VCFrameList);
//	}
//
//	public List<T> CreateCSVList<T> (string csvname)
//	where T:BaseCSVStructure, new()
//	{
//		var stream = new MemoryStream (GetCSV (csvname));
//		var reader = new StreamReader (stream);
//		IEnumerable<T> list = mCsvContext.Read<T> (reader);
//		return new List<T> (list);
//	}
//
//	Dictionary<int,T> GetDictionary<T> (List<T> list) where T : BaseCSVStructure
//	{
//		Dictionary<int,T> dic = new Dictionary<int, T> ();
//		foreach (T t in list) {
//			if (!dic.ContainsKey (t.id))
//				dic.Add (t.id, t);
//			else
//				Debug.Log (string.Format ("Multi key:{0}{1}", typeof(T).ToString (), t.id).YellowColor ());
//		}
//		return dic;
//	}
//
//	private void LoadProduct ()
//	{
//#if UNITY_IOS
//		ProductList = CreateCSVList<MProductCSVStructure> (CSV_PRODUCT).FindAll (product => product.platform_type == 1);
//#elif UNITY_ANDROID
//        ProductList = CreateCSVList<MProductCSVStructure>(CSV_PRODUCT).FindAll(product => product.platform_type == 2);		
//#endif
//	}
//
//	public MCharacterCSVStructure GetCharacter (int charaId)
//	{
//		if (this.CharacterDic.ContainsKey (charaId)) {
//			return this.CharacterDic [charaId];
//		}
//		return null;
//	}
//
//	public MVoiceCSVStructure GetVoice (int voiceId)
//	{
//		if (this.VoiceDic.ContainsKey (voiceId)) {
//			return this.VoiceDic [voiceId];
//		}
//		return null;
//	}
//
//	public MGiftItemCSVStructure GetGift (int giftId)
//	{
//		if (this.GiftDic.ContainsKey (giftId)) {
//			return this.GiftDic [giftId];
//		}
//		return null;
//	}
//
//	public List<MVoiceCSVStructure> GetVoiceByCharaId (int charaId)
//	{
//		if (VoiceDicByCharacterId.ContainsKey (charaId)) {
//			return this.VoiceDicByCharacterId [charaId];
//		}
//		return null;
//	}
//
//	public MVoiceCSVStructure GetVoiceByCondition (int charaId, int conditionId)
//	{
//		MCharacterCSVStructure character = GetCharacter (charaId);
//		if(character != null){
//			List<MVoiceCSVStructure> voiceList = character.voiceDicByCondition [conditionId];
//			if(voiceList.Count > 0){
//				return voiceList [0];
//			}
//		}
//		return null;
//	}
//
//}
