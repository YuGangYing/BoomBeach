using UnityEngine;
using System.Collections;

public class LoginToMainTransition : SMTransition {

	private UISlider loadingSlider;
	private UILabel loadingStep;
	private UILabel hint_lable;

	public int MaxStep;
	public int BeforeJumpedMaxStep;
	public int CurentStep;
	public string StepDesc;

	private bool IsJumpedScene;


	void Awake()
	{

		loadingSlider = transform.Find("Panel/ScreenLayout/Center/Bottom/LOAD_BOX/Progress Bar").GetComponent<UISlider>();
		loadingStep = transform.Find("Panel/ScreenLayout/Center/Bottom/LOAD_BOX/LoadingStep").GetComponent<UILabel>();
		hint_lable = transform.Find("Panel/ScreenLayout/Center/Bottom/Hint/Label").GetComponent<UILabel>();
		IsJumpedScene = false;
		StepDesc = "("+CurentStep+"/"+MaxStep+")"+LocalizationCustom.instance.Get("TID_LOADING_PAGE_LOADING_STEP_UI");
	}


	protected override void Prepare () {

	}
	
	protected override bool Process(float elapsedTime) {


		loadingSlider.value = CurentStep * 1f / MaxStep;
		loadingStep.text = StepDesc;


		if(time_interval==0)
			hint_lable.text = LocalizationCustom.instance.RandomGet("TID_HINT_");

		if (time_interval >= 3){											
			time_interval = 0;
				
		}else{
			time_interval = time_interval + Time.deltaTime;
		}


		if(Application.platform == RuntimePlatform.IPhonePlayer){
			//HplIOSCall.ClearApplicationBadgeNumber();
		}

		if(!IsJumpedScene)
		{
			if( CurentStep<BeforeJumpedMaxStep)
			{
				return true;
			}
			else
			{
				IsJumpedScene = true;
				return false;
			}
		}
		else
		{
			CurentStep = GameLoader.Instance.initStep+1;
			StepDesc = "("+(CurentStep)+"/"+MaxStep+")"+LocalizationCustom.instance.Get("TID_LOADING_PAGE_LOADING_STEP_"+CurentStep);

			if(CurentStep<MaxStep)
			{
				return true;
			}
			else
			{
				if(waittoend<0.2f)
				{
					waittoend+=Time.deltaTime;
					return true;
				}
				else
				{
					return false;
				}
			}
		}
	}
	float waittoend;
	float time_interval;


}
