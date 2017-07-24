using UnityEngine;
using System.Collections;

public static class ComplexMathf {

	//求线段与圆交点，也可以用于求线段与圆最近点。
	public static bool CircleLineInstersect(Vector2 startPos, Vector2 endPos,Vector2 centerPos,float radius,out Vector2 intersection){
		intersection = Vector2.zero;
		if ((startPos - centerPos).sqrMagnitude > radius * radius && (endPos - centerPos).sqrMagnitude > radius * radius) {
			return false;
		}
		if ((startPos - centerPos).sqrMagnitude < radius * radius && (endPos - centerPos).sqrMagnitude < radius * radius) {
			return false;
		}

		float f =  (centerPos - startPos).magnitude * Vector2.Dot ((centerPos - startPos).normalized,(endPos - startPos).normalized);
		Vector2 pos = startPos + (endPos - startPos).normalized * f;
		if (Vector2.Distance (pos, centerPos) > radius)
			return false;
		float d = Mathf.Sqrt (radius * radius - (pos - centerPos).sqrMagnitude);
		intersection = pos - (endPos - startPos).normalized *  d;
		return true;
	}


	/// <summary>
	/// 计算角色方向;
	/// </summary>
	public static Direct CaclCharInfoDirect(Vector2 forward)
	{
		Direct direct = Direct.RIGHT;
		float degree = CaclDegree (forward);
		//charInfo.Degree = degree;
		//charInfo.NextPoint = goal;

		//Vector2 offset = new Vector2(goal.x,goal.z) - new Vector2(charPosition.x,charPosition.z);

		//charInfo.RealDegree = Mathf.Atan (offset.y / offset.x) * Mathf.Rad2Deg;

		if ((degree >= 0 && degree < 22.5) || (degree <= 360f && degree >= (360 - 22.5))) 
		{
			direct = Direct.RIGHT;		
		}
		else if( degree>=22.5&&degree<(45+22.5))
		{
			direct = Direct.RIGHTUP;	
		}
		else if( degree>=(45+22.5)&&degree<(90+22.5))
		{
			direct = Direct.UP;	
		}
		else if( degree>=(90+22.5)&&degree<(135+22.5))
		{
			direct = Direct.LEFTUP;	
		}
		else if( degree>=(135+22.5)&&degree<(180+22.5))
		{
			direct = Direct.LEFT;	
		}
		else if( degree>=(180+22.5)&&degree<(225+22.5))
		{
			direct = Direct.LEFTDOWN;	
		}
		else if( degree>=(225+22.5)&&degree<(270+22.5))
		{
			direct = Direct.DOWN;	
		}
		else if( degree>=(270+22.5)&&degree<(315+22.5))
		{
			direct = Direct.RIGHTDOWN;	
		}
		return direct;
	}

	public static float CaclDegree(Vector2 offset)
	{
		float angle = 0;
		if (offset.magnitude > 0){
			angle = Mathf.Rad2Deg*Mathf.Asin (offset.y  /offset.magnitude);
			if(offset.x<0&&offset.y>=0) angle = 180 - angle;		
			if(offset.y<0)
			{
				if (offset.x <= 0){
					angle =  180 - angle;
				}else{
					angle =  360 + angle;
				}
			}
		}
		return angle;
	}

	//两点计算角度;
	public static float CaclDegree(Vector2 begin,Vector2 end)
	{
		Vector2 offset = end - begin;				
		float angle = 0;
		if (offset.magnitude > 0){
			angle = Mathf.Rad2Deg*Mathf.Asin (offset.y  /offset.magnitude);
			if(offset.x<0&&offset.y>=0) angle = 180 - angle;		
			if(offset.y<0)
			{
				if (offset.x <= 0){
					angle =  180 - angle;
				}else{
					angle =  360 + angle;
				}
			}
		}
		return angle;
	}


}
