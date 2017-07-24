using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Pathfinding;
using Pathfinding.RVO;

//改编自AIPath
public class MoveAction : BaseAction{

	public const string actionName = "astarMoveAction";

	public float fireDist = 10;
	//float mSqrFireDist;
	public Vector3 targetPos;
	Seeker mSeeker;
	tk2dSpriteAnimator mAnim;
	tk2dSprite mSprite;

	/** Determines how often it will search for new paths.
	 * If you have fast moving targets or AIs, you might want to set it to a lower value.
	 * The value is in seconds between path requests.
	 */
	public float repathRate = 0.5F;

	/** Target to move towards.
	 * The AI will try to follow/move towards this target.
	 * It can be a point on the ground where the player has clicked in an RTS for example, or it can be the player object in a zombie game.
	 */
	public Transform target;

	/** Enables or disables searching for paths.
	 * Setting this to false does not stop any active path requests from being calculated or stop it from continuing to follow the current path.
	 * \see #canMove
	 */
	public bool canSearch = true;

	/** Enables or disables movement.
	 * \see #canSearch */
	public bool canMove = true;

	/** Maximum velocity.
	 * This is the maximum speed in world units per second.
	 */
	public float speed = 3;

	public float targetDeltaTime = 0.033f;


	/** Rotation speed.
	 * Rotation is calculated using Quaternion.SLerp. This variable represents the damping, the higher, the faster it will be able to rotate.
	 */
	public float turningSpeed = 5;

	/** Distance from the target point where the AI will start to slow down.
	 * Note that this doesn't only affect the end point of the path
	 * but also any intermediate points, so be sure to set #forwardLook and #pickNextWaypointDist to a higher value than this
	 */
	public float slowdownDistance = 0.6F;

	/** Determines within what range it will switch to target the next waypoint in the path */
	public float pickNextWaypointDist = 2;

	/** Target point is Interpolated on the current segment in the path so that it has a distance of #forwardLook from the AI.
	 * See the detailed description of AIPath for an illustrative image */
	public float forwardLook = 1;

	/** Distance to the end point to consider the end of path to be reached.
	 * When this has been reached, the AI will not move anymore until the target changes and OnTargetReached will be called.
	 */
	public float endReachedDistance = 0.05f;

	/** Do a closest point on path check when receiving path callback.
	 * Usually the AI has moved a bit between requesting the path, and getting it back, and there is usually a small gap between the AI
	 * and the closest node.
	 * If this option is enabled, it will simulate, when the path callback is received, movement between the closest node and the current
	 * AI position. This helps to reduce the moments when the AI just get a new path back, and thinks it ought to move backwards to the start of the new path
	 * even though it really should just proceed forward.
	 */
	public bool closestOnPathCheck = true;

	protected float minMoveScale = 0.2F;

	/** Cached Transform component */
	protected Transform mTrans;

	/** Time when the last path request was sent */
	protected float lastRepath = -9999;

	/** Current path which is followed */
	public Path path;

	/** Current index in the path which is current target */
	protected int currentWaypointIndex = 0;

	/** Holds if the end-of-path is reached
	 * \see TargetReached */
	protected bool targetReached = false;

	/** Only when the previous path has been returned should be search for a new path */
	protected bool canSearchAgain = true;

	protected Vector3 lastFoundWaypointPosition;
	protected float lastFoundWaypointTime = -9999;

	/** Returns if the end-of-path has been reached
	 * \see targetReached */
	public bool TargetReached {
		get {
			return targetReached;
		}
	}

	/** Initializes reference variables.
	 * If you override this function you should in most cases call base.Awake () at the start of it.
	 * */
	public override void OnAwake () {
		mTrans = mSelf.transform;
		mAnim = mSelf.GetComponentInChildren<tk2dSpriteAnimator> ();
		mSprite = mAnim.GetComponent<tk2dSprite>();
		mSeeker = mSelf.GetComponent<Seeker> ();
	}

	public override void OnEnter (){
		isHandleMove = false;
		isFire = false;
		canMove = false;
		lastRepath = -9999;
		currentWaypointIndex = 0;
		canSearchAgain = true;
		lastFoundWaypointPosition = GetFeetPosition();
		Repath ();
	}

	Vector3 preDir = Vector3.zero;
	public override void OnUpdate () {
		if (!canMove || isFire || isHandleMove) {
			return; 
		}
		if(target.GetComponent<TestAStarAI>().hp <= 0){
			Fsm.currentStateName = TestAStarController.scaneStateName;
			return;
		}
		//mSqrFireDist = fireDist * fireDist;
		 
		//mTrans.position = path.vectorPath[path.vectorPath.Count-1];
		//return;
		/*
		if(Vector3.SqrMagnitude(mTrans.position - targetPos) <= mSqrFireDist)
		{
			//mTrans.LookAt (targetPos);
			int right = 1;
			float defaultAngle = 5f / 180f * Mathf.PI;
			float sqrMinInterval = 0.5f;
			pos = mTrans.position;
			float totalAngle = 0;
			isFire = true;
			path.Reset ();
			path.Cleanup ();
			Fsm.currentStateName = TestAStarController.attackStateName;
			BaseState state = Fsm.stateDic [TestAStarController.attackStateName];
			AttackAction action = (AttackAction)( state.actionDic[AttackAction.actionName]);
			action.target = target;

			while (true) {
				//1.如果x距离有人并且在攻击，则向一个右（左）方方向移动
				//2.如果右（左）方是建筑，则相反方向
				//3.如果两边都不可以，则在本格子找位置
				bool isUnitBeside = false;
				foreach(TestAStarAI unit in units){
					if(unit.moveAction.isFire && unit.moveAction!= this){
						if (Vector3.SqrMagnitude (unit.transform.position-pos) < sqrMinInterval * sqrMinInterval) {
							isUnitBeside = true;
							isHandleMove = true;
							break;
						}
					}
				}
				//如果超过30度，则找一个就近点

				//绕点旋转，应该左右旋转，哪边少去哪边
				if (isUnitBeside) {
					Vector3 offPos = pos - targetPos;
					Quaternion qua = new Quaternion (0, Mathf.Sin (defaultAngle / 2), 0, Mathf.Cos (defaultAngle / 2));
					offPos = qua * offPos;
					pos = offPos + targetPos;
					sqrMinInterval = sqrMinInterval * 0.5f;//每次弱化最低距离和角度
					defaultAngle = defaultAngle * 0.5f;
				} else {
					//tr.GetComponent<TestAStarAI> ().StopAllCoroutines();
					isFire = true;
					//tr.GetComponent<TestAStarAI> ().StartCoroutine(CmdMove());
					break;
					//Dosomething;
				}
			}

		}
		*/
		CalculateVelocity(GetFeetPosition());

		//Rotate towards targetDirection (filled in by CalculateVelocity)
		//RotateTowards(targetDirection);
		//mTrans.Translate(dir*targetDeltaTime, Space.World);
		if (preDir != targetDirection) {
			preDir = targetDirection;
			PlayMove (targetDirection);
		}
		mTrans.Translate(targetDirection.normalized * speed * targetDeltaTime, Space.World);
	}

	public override void OnExit(){
		path = null;
	}

	GraphNode blockNode;//计算出来的攻击点
	//List<GraphNode> nodes;
	/*
	void Update () {
		if(Input.GetMouseButtonDown(0)){
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			zeroIntersect = ray.origin + ray.direction * (ray.origin.y / -ray.direction.y);
			//moveAction.Reset();
			TestAStarController.Instance ().researchQueue.Enqueue (this);
			if (blockNode != null) {
				blockNode.Penalty -= penalty;
				if (blockNode.Penalty >= 1000000)
					blockNode.Penalty -= 1000000;
				blockNode = null;
			}

		}
		//moveAction.OnUpdate ();
	}
	*/
	//Path mPath = null;
	public void Repath(){
		if (blockNode != null) {
			blockNode.Penalty -= penalty;
			if (blockNode.Penalty >= 1000000)
				blockNode.Penalty -= 1000000;
			blockNode = null;
		}
		mSeeker.StartPath (mTrans.position, targetPos,OnPathComplete);
		//moveAction.targetPos = zeroIntersect;
		//AstarPath.active.ReturnPaths (true);
	}

	public void OnPathComplete (Path p)
	{
		
		TestAStarController.Instance ().researchAble = true;
		//Debug.Log (p.vectorPath.Count);
		path = p;
		canMove = true;
		/*
		GameObject ob = new GameObject("LineRenderer", typeof(LineRenderer));
		LineRenderer lr = ob.GetComponent<LineRenderer>();
		//lr.sharedMaterial = lineMat;
		lr.SetWidth(0.2f, 0.2f);
		lr.SetVertexCount(p.vectorPath.Count);
		for (int i = 0; i < p.vectorPath.Count; i++) {
			lr.SetPosition(i, p.vectorPath[i]);
		}
		*/
		GridGraph gg = (GridGraph)AstarPath.active.graphs [0];
		/*
		for (int i = 0; i < p.path.Count; i++) {
			p.path [i].Penalty += 1;
		}
		*/
		//求路径与目标圆的交点。
		//后面在寻路的单位经过这点的消耗就变多了。
		for (int i = 0; i < p.vectorPath.Count; i++) {
			if (i + 1 >= p.vectorPath.Count) {
				break;
			}
			Vector3 startPos = p.vectorPath [i];
			Vector3 endPos = p.vectorPath [i + 1];
			Vector2 intersection;
			if (ComplexMathf.CircleLineInstersect (new Vector2 (startPos.x, startPos.z),new Vector2 (endPos.x, endPos.z), 
				new Vector2 (targetPos.x, targetPos.z), fireDist, out intersection)) {
				Vector3 pos = new Vector3 (intersection.x, 0, intersection.y);
				NNInfo info = gg.GetNearest (pos);

				uint index = info.node.Penalty / penalty;
				Debug.Log (index);
				pos = (Vector3)info.node.position;
				switch(index){
				case 0:
					break;
				case 1:
					pos += new Vector3 (0.3f,0,-0.3f);
					break;
				case 2:
					pos += new Vector3 (-0.3f,0,-0.3f);
					break;
				case 3:
					pos += new Vector3 (-0.3f,0,0.3f);
					break;
				case 4:
					pos += new Vector3 (-0.3f,0,0.3f);
					break;
				}
				p.vectorPath.RemoveRange (i+1,p.vectorPath.Count-i-1);
				p.vectorPath.Insert (i+1,pos);
				info.node.Penalty += penalty;
				if (info.node.Penalty >= penalty * 5) {
					info.node.Penalty += 1000000;
				}
				blockNode = info.node;
				break;
			}
		}
	}

	/// <summary>
	/// 播放行走动画;
	/// </summary>
	public virtual void PlayMove(Vector3 dir)
	{
		//tk2dSprite _sprite = anim.GetComponent<tk2dSprite>();
		//以下是动画播放;
		Direct direct = ComplexMathf.CaclCharInfoDirect(new Vector2(dir.x,dir.z));
		if (direct == Direct.LEFT || direct == Direct.LEFTDOWN || direct == Direct.LEFTUP || direct == Direct.UP) 
			mSprite.FlipX = true;
		else
			mSprite.FlipX = false;
		Globals.PlayTk2dAnim ("Walk",direct,mAnim);
	}

	public virtual void OnTargetReached () {
		canMove = false;
		//int right = 1;
		//float defaultAngle = 5f / 180f * Mathf.PI;
		//float sqrMinInterval = 0.5f;
		pos = mTrans.position;
		//float totalAngle = 0;
		isFire = true;
		path.Reset ();
		path.Cleanup ();
		Fsm.currentStateName = TestAStarController.attackStateName;
		BaseState state = Fsm.stateDic [TestAStarController.attackStateName];
		AttackAction action = (AttackAction)( state.actionDic[AttackAction.actionName]);
		action.target = target;
		//NNInfo info = AstarPath.active.graphs [0].GetNearest (tr.position);//得到当前node
		//info.node.Walkable = false;
		//End of path has been reached
		//If you want custom logic for when the AI has reached it's destination
		//add it here
		//You can also create a new script which inherits from this one
		//and override the function in that script
	}

	/** Called when a requested path has finished calculation.
	 * A path is first requested by #SearchPath, it is then calculated, probably in the same or the next frame.
	 * Finally it is returned to the seeker which forwards it to this function.\n
	 */
	/*
	public virtual void OnPathComplete (Path _p) {
		ABPath p = _p as ABPath;

		if (p == null) throw new System.Exception("This function only handles ABPaths, do not use special path types");

		canSearchAgain = true;

		//Claim the new path
		p.Claim(this);

		// Path couldn't be calculated of some reason.
		// More info in p.errorLog (debug string)
		if (p.error) {
			p.Release(this);
			return;
		}

		//Release the previous path
		if (path != null) path.Release(this);

		//Replace the old path
		path = p;

		//Reset some variables
		currentWaypointIndex = 0;
		targetReached = false;

		//The next row can be used to find out if the path could be found or not
		//If it couldn't (error == true), then a message has probably been logged to the console
		//however it can also be got using p.errorLog
		//if (p.error)

		if (closestOnPathCheck) {
			// Simulate movement from the point where the path was requested
			// to where we are right now. This reduces the risk that the agent
			// gets confused because the first point in the path is far away
			// from the current position (possibly behind it which could cause
			// the agent to turn around, and that looks pretty bad).
			Vector3 p1 = Time.time - lastFoundWaypointTime < 0.3f ? lastFoundWaypointPosition : p.originalStartPoint;
			Vector3 p2 = GetFeetPosition();
			Vector3 dir = p2-p1;
			float magn = dir.magnitude;
			dir /= magn;
			int steps = (int)(magn/pickNextWaypointDist);

			#if ASTARDEBUG
			Debug.DrawLine(p1, p2, Color.red, 1);
			#endif

			for (int i = 0; i <= steps; i++) {
				CalculateVelocity(p1);
				p1 += dir;
			}
		}
	}
	*/

	/*
	public void Reset(){
		isHandleMove = false;
		isFire = false;
		canMove = false;
		//if(path!=null)path.Release (this);
		lastRepath = -9999;
		currentWaypointIndex = 0;
		canSearchAgain = true;
		lastFoundWaypointPosition = GetFeetPosition();
	}
*/

	public virtual Vector3 GetFeetPosition () {
		return mTrans.position;
	}

	public bool isHandleMove;
	public bool isFire;
	Vector3 pos;
	IEnumerator CmdMove(){
		mTrans.LookAt (pos);
		Vector3 forward = mTrans.forward;
		while(true){
			float targetDist = Vector3.SqrMagnitude (mTrans.position-pos);
			if(targetDist <= endReachedDistance || targetDist < speed*targetDeltaTime){
				mTrans.position = pos;
				isFire = true;
				mTrans.LookAt (targetPos);
				yield break;
			}
			mTrans.Translate(forward * speed *targetDeltaTime, Space.World);
			yield return null;
		}
	}


	/** Point to where the AI is heading.
	 * Filled in by #CalculateVelocity */
	protected Vector3 targetPoint;
	/** Relative direction to where the AI is heading.
	 * Filled in by #CalculateVelocity */
	protected Vector3 targetDirection;

	protected float XZSqrMagnitude (Vector3 a, Vector3 b) {
		float dx = b.x-a.x;
		float dz = b.z-a.z;
		return dx*dx + dz*dz;
	}

	GraphNode preNode;
	uint penalty = 1;
	/** Calculates desired velocity.
	 * Finds the target path segment and returns the forward direction, scaled with speed.
	 * A whole bunch of restrictions on the velocity is applied to make sure it doesn't overshoot, does not look too far ahead,
	 * and slows down when close to the target.
	 * /see speed
	* /see endReachedDistance
	* /see slowdownDistance
	* /see CalculateTargetPoint
	* /see targetPoint
	* /see targetDirection
	* /see currentWaypointIndex
	*/
	protected Vector3 CalculateVelocity (Vector3 currentPosition) {
		if (path == null || path.vectorPath == null || path.vectorPath.Count == 0) return Vector3.zero;

		List<Vector3> vPath = path.vectorPath;

		if (vPath.Count == 1) {
			vPath.Insert(0, currentPosition);
		}

		if (currentWaypointIndex >= vPath.Count) { currentWaypointIndex = vPath.Count-1; }

		if (currentWaypointIndex <= 1) currentWaypointIndex = 1;

		while (true) {
			if (currentWaypointIndex < vPath.Count-1) {
				//There is a "next path segment"
				float dist = XZSqrMagnitude(vPath[currentWaypointIndex], currentPosition);
				//Mathfx.DistancePointSegmentStrict (vPath[currentWaypointIndex+1],vPath[currentWaypointIndex+2],currentPosition);
				if (dist < pickNextWaypointDist*pickNextWaypointDist) {
					lastFoundWaypointPosition = currentPosition;
					lastFoundWaypointTime = Time.time;
					currentWaypointIndex++;
				} else {
					break;
				}
			} else {
				break;
			}
		}

		//Vector3 dir = vPath[currentWaypointIndex] - vPath[currentWaypointIndex-1];
		Vector3 targetPosition = CalculateTargetPoint(currentPosition, vPath[currentWaypointIndex-1], vPath[currentWaypointIndex]);


		Vector3 dir = targetPosition-currentPosition;
		dir.y = 0;
		float targetDist = dir.magnitude;

		float slowdown = Mathf.Clamp01(targetDist / slowdownDistance);

		this.targetDirection = dir;
		this.targetPoint = targetPosition;

		if (currentWaypointIndex == vPath.Count-1 && targetDist <= endReachedDistance) {
			if (!targetReached) { targetReached = true; OnTargetReached(); }

			//Send a move request, this ensures gravity is applied
			return Vector3.zero;
		}

		Vector3 forward = mTrans.forward;
		float dot = Vector3.Dot(dir.normalized, forward);

//		if (preNode != info.node) {
//			penalty = info.node.Penalty;
//			info.node.Penalty++;
//			if(preNode!=null)
//				preNode.Penalty--;
//			preNode = info.node;
//		} 
		float sp = speed * Mathf.Max(dot, minMoveScale) * slowdown ;//TODO
//		float sp = speed * Mathf.Max(dot, minMoveScale) * slowdown / (1 + info.node.Penalty);//TODO

		#if ASTARDEBUG
		Debug.DrawLine(vPath[currentWaypointIndex-1], vPath[currentWaypointIndex], Color.black);
		Debug.DrawLine(GetFeetPosition(), targetPosition, Color.red);
		Debug.DrawRay(targetPosition, Vector3.up, Color.red);
		Debug.DrawRay(GetFeetPosition(), dir, Color.yellow);
		Debug.DrawRay(GetFeetPosition(), forward*sp, Color.cyan);
		#endif

		sp = Mathf.Clamp(sp, 0, targetDist/(targetDeltaTime*2));

		return forward*sp;
	}

	/** Rotates in the specified direction.
	 * Rotates around the Y-axis.
	 * \see turningSpeed
	 */
	protected virtual void RotateTowards (Vector3 dir) {
		if (dir == Vector3.zero) return;

		Quaternion rot = mTrans.rotation;
		Quaternion toTarget = Quaternion.LookRotation(dir);

		rot = Quaternion.Slerp(rot, toTarget, turningSpeed*targetDeltaTime);
		Vector3 euler = rot.eulerAngles;
		euler.z = 0;
		euler.x = 0;
		rot = Quaternion.Euler(euler);

		mTrans.rotation = rot;
	}

	/** Calculates target point from the current line segment.
	 * \param p Current position
	 * \param a Line segment start
	 * \param b Line segment end
	 * The returned point will lie somewhere on the line segment.
	 * \see #forwardLook
	 * \todo This function uses .magnitude quite a lot, can it be optimized?
	 */
	protected Vector3 CalculateTargetPoint (Vector3 p, Vector3 a, Vector3 b) {
		a.y = p.y;
		b.y = p.y;

		float magn = (a-b).magnitude;
		if (magn == 0) return a;

		float closest = Mathf.Clamp01(VectorMath.ClosestPointOnLineFactor(a, b, p));
		Vector3 point = (b-a)*closest + a;
		float distance = (point-p).magnitude;

		float lookAhead = Mathf.Clamp(forwardLook - distance, 0.0F, forwardLook);

		float offset = lookAhead / magn;
		offset = Mathf.Clamp(offset+closest, 0.0F, 1.0F);
		return (b-a)*offset + a;
	}
}
