using UnityEngine;
using System.Collections;

public class FPSCounter : MonoBehaviour {

    public float updateInterval = 0.5F;
    private double lastInterval;
    private int frames = 0;
    //private double fps;
	//private bool guiEnabled=true;
	//private string fpsstr;

	public Direct boatdirect;
	public string boatdeck = "up";
	public string boatlight = "green";


    void Start() {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
    }
	
	float LabelSlider(string labelText, Rect screenRect, float sliderValue, float min, float sliderMaxValue) {
		GUI.Label (screenRect, labelText+sliderValue);
		screenRect.x += screenRect.width; // <- Push the Slider to the end of the Label
		return GUI.HorizontalSlider(screenRect, sliderValue, min, sliderMaxValue);
	}

    void Update() {
		/*
		if (Globals.LandCrafts != null) 
		{
			for (int i=0; i<Globals.LandCrafts.Count; i++) 
			{
				BuildInfo b = Globals.LandCrafts [i] as BuildInfo;
				LandCraft lc = b.GetComponent<LandCraft> ();
				if(lc.direct!=boatdirect)
				{
					lc.direct = boatdirect;
					lc.setTrooper(lc.currentTrooperTid,lc.currentNum);
				}
				lc.setDeck(boatdeck);
				lc.setLight(boatlight);

			}
		}
*/
        ++frames;
        double timeNow = Time.realtimeSinceStartup;
        if (timeNow > lastInterval + updateInterval) {
            //fps = frames / (timeNow - lastInterval);
            frames = 0;
            lastInterval = timeNow;
			//fpsstr = "" + fps.ToString("f2") + ";fps2:" + (1 / Time.deltaTime).ToString("f2");
        }
    }
}
