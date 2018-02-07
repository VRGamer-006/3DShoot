using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
	public bool canControl = true;
	public bool useFixedUpdate = true;
	
	#region custom code
	[HideInInspector]//在检视视图中隐藏变量，即不显示公共变量
	public bool running;//处于跑动状态标志位
	[HideInInspector]
	public bool walking;//处于走动状态标志位
	[HideInInspector]
	public bool canRun;//是否能够跑动标志位
	
	private GameObject mainCamera = null;//定义主摄像机对象
	
	[HideInInspector]
	public bool onLadder = false;//是否处于梯子上
	private float ladderHopSpeed = 6.0f;//爬梯速度
	#endregion
	
	[System.NonSerialized]
	public Vector3 inputMoveDirection = Vector3.zero;
	
	[System.NonSerialized]
	public bool inputJump = false;
	
	#region custom code
	[System.NonSerialized]
	public bool inputRun = false;//是否加速跑动标志位
	
	[System.NonSerialized]
	public bool inputCrouch = false;//是否下蹲标志位
	#endregion
	
	[System.Serializable]
	public class PlayerControllerMovement {
		[HideInInspector]
		public float maxForwardSpeed = 10.0f;
		[HideInInspector]
		public float maxSidewaysSpeed = 10.0f;
		[HideInInspector]
		public float maxBackwardsSpeed = 10.0f;
		
		#region custom code
		public float walkSpeed = 6.0f;//行走速度
		public float runSpeed = 9.0f;//跑动速度
		
		public bool canCrouch = true;//是否可以下蹲标志位
		public float crouchSpeed = 3.0f;//下蹲速度
		public float crouchHeight = 1.2f;//下蹲时的高度
		public float crouchSmooth = 0.1f;//下蹲时的平滑
		#endregion
		
		public AnimationCurve slopeSpeedMultiplier = new AnimationCurve(new Keyframe(-90, 1), new Keyframe(0, 1), new Keyframe(90, 0));
		
		public float maxGroundAcceleration = 30.0f;
		public float maxAirAcceleration = 20.0f;
		
		public float gravity = 10.0f;
		public float maxFallSpeed = 20.0f;
	
		[HideInInspector]
		public bool enableGravity = true;

		[System.NonSerialized]
		public CollisionFlags collisionFlags;

		[System.NonSerialized]
		public Vector3 velocity;

		[System.NonSerialized]
		public Vector3 frameVelocity = Vector3.zero;
	
		[System.NonSerialized]
		public Vector3 hitPoint = Vector3.zero;
	
		[System.NonSerialized]
		public Vector3 lastHitPoint = new Vector3(Mathf.Infinity, 0, 0);
	}
	
	public PlayerControllerMovement movement = new PlayerControllerMovement();
	
	#region custom code
	[HideInInspector]
	public bool crouch;//下蹲标志位
	private float standardHeight;//标准高度
	private GameObject lookObj;//看向的物体，保证视角的正确
	private float centerY;//竖直方向中心点
	private bool canStand;//能否站起判断变量
	private bool canStandCrouch = true;//能否站起下蹲判断变量
	#endregion

	public enum PlayerMovementTransferOnJump {
		None, 
		InitTransfer, 
		PermaTransfer, 
		PermaLocked 
	}
	
	[System.Serializable]
	public class PlayerControllerJumping {
		public bool enabled = true;
	
		public float baseHeight = 1.0f;
		
		public float extraHeight = 4.1f;
		
		public float perpAmount = 0.0f;
		
		public float steepPerpAmount = 0.5f;
		
		[System.NonSerialized]
		public bool jumping = false;
		
		[System.NonSerialized]
		public bool holdingJumpButton = false;
	
		[System.NonSerialized]
		public float lastStartTime = 0.0f;
		
		[System.NonSerialized]
		public float lastButtonDownTime = -100;
		
		[System.NonSerialized]
		public Vector3 jumpDir = Vector3.up;
	}
	
	public PlayerControllerJumping jumping = new PlayerControllerJumping();
	
	[System.Serializable]
	public class PlayerControllerMovingPlatform {
		public bool enabled = true;
		
		public PlayerMovementTransferOnJump movementTransfer = PlayerMovementTransferOnJump.PermaTransfer;
		
		[System.NonSerialized]
		public Transform hitPlatform;
		
		[System.NonSerialized]
		public Transform activePlatform;
		
		[System.NonSerialized]
		public Vector3 activeLocalPoint;
		
		[System.NonSerialized]
		public Vector3 activeGlobalPoint;
		
		[System.NonSerialized]
		public Quaternion activeLocalRotation;
		
		[System.NonSerialized]
		public Quaternion activeGlobalRotation;
		
		[System.NonSerialized]
		public Matrix4x4 lastMatrix;
		
		[System.NonSerialized]
		public Vector3 platformVelocity;
		
		[System.NonSerialized]
		public bool newPlatform;
	}

	public PlayerControllerMovingPlatform movingPlatform = new PlayerControllerMovingPlatform();
	
	[System.Serializable]
	public class PlayerControllerSliding {
		public bool enabled = true;
		
		public float slidingSpeed = 15;
		
		public float sidewaysControl = 1.0f;
		
		public float speedControl = 0.4f;
	}

	public PlayerControllerSliding sliding = new PlayerControllerSliding();
	
	#region custom code
	[System.Serializable]
	public class PlayerControllerPushing {	
		public bool canPush = true;//是否可推动刚体标志位
		public float pushPower = 2.0f;//推动的力量
	}
	public PlayerControllerPushing pushing;//声明变量
	#endregion
		
	[System.NonSerialized]
	public bool grounded = true;
	
	[System.NonSerialized]
	public Vector3 groundNormal = Vector3.zero;
	
	private Vector3 lastGroundNormal = Vector3.zero;

	private Transform tr;
	
	private CharacterController controller;
	
	void Awake () {
		controller = GetComponent<CharacterController>();
		tr = transform;
		#region custom code
		standardHeight = controller.height;//得到角色标准身高
		lookObj = GameObject.FindWithTag("LookObject");//得到游戏对象
		centerY = controller.center.y;//得到角色竖直方向上的中心点
		mainCamera = GameObject.FindWithTag("MainCamera");//拿到主摄像机对象
		canRun = true;//赋值
		canStand = true;//赋值
		#endregion
	}
	
	private void UpdateFunction() {
		Vector3 velocity = movement.velocity;

		velocity = ApplyInputVelocityChange(velocity);
	
		if(movement.enableGravity){
			if(crouch && inputJump)
				return;
			velocity = ApplyGravityAndJumping (velocity);
		}
		
		Vector3 moveDistance = Vector3.zero;
		if (MoveWithPlatform()) {
			Vector3 newGlobalPoint = movingPlatform.activePlatform.TransformPoint(movingPlatform.activeLocalPoint);
			moveDistance = (newGlobalPoint - movingPlatform.activeGlobalPoint);
			if (moveDistance != Vector3.zero)
				controller.Move(moveDistance);
			
	        Quaternion newGlobalRotation = movingPlatform.activePlatform.rotation * movingPlatform.activeLocalRotation;
	        Quaternion rotationDiff = newGlobalRotation * Quaternion.Inverse(movingPlatform.activeGlobalRotation);
	        
	        float yRotation = rotationDiff.eulerAngles.y;
	        if (yRotation != 0) {
		        tr.Rotate(0, yRotation, 0);
	        }
		}

		Vector3 lastPosition = tr.position;

		Vector3 currentMovementOffset = velocity * Time.deltaTime;
	
		float pushDownOffset = Mathf.Max(controller.stepOffset, new Vector3(currentMovementOffset.x, 0, currentMovementOffset.z).magnitude);
		if (grounded)
			currentMovementOffset -= pushDownOffset * Vector3.up;
	
		movingPlatform.hitPlatform = null;
		groundNormal = Vector3.zero;
	
		movement.collisionFlags = controller.Move (currentMovementOffset);
	
		movement.lastHitPoint = movement.hitPoint;
		lastGroundNormal = groundNormal;
	
		if (movingPlatform.enabled && movingPlatform.activePlatform != movingPlatform.hitPlatform) {
			if (movingPlatform.hitPlatform != null) {
				movingPlatform.activePlatform = movingPlatform.hitPlatform;
				movingPlatform.lastMatrix = movingPlatform.hitPlatform.localToWorldMatrix;
				movingPlatform.newPlatform = true;
			}
		}
		
		Vector3 oldHVelocity = new Vector3(velocity.x, 0, velocity.z);
		movement.velocity = (tr.position - lastPosition) / Time.deltaTime;
		Vector3 newHVelocity = new Vector3(movement.velocity.x, 0, movement.velocity.z);
		
		if (oldHVelocity == Vector3.zero) {
			movement.velocity = new Vector3(0, movement.velocity.y, 0);
		}
		else {
			float projectedNewVelocity = Vector3.Dot(newHVelocity, oldHVelocity) / oldHVelocity.sqrMagnitude;
			movement.velocity = oldHVelocity * Mathf.Clamp01(projectedNewVelocity) + movement.velocity.y * Vector3.up;
		}
	
		if (movement.velocity.y < velocity.y - 0.001f) {
			if (movement.velocity.y < 0) {
				movement.velocity.y = velocity.y;
			}
			else {
				jumping.holdingJumpButton = false;
			}
		}
	
		if (grounded && !IsGroundedTest()) {
			grounded = false;
		
			if (movingPlatform.enabled &&
				(movingPlatform.movementTransfer == PlayerMovementTransferOnJump.InitTransfer ||
				movingPlatform.movementTransfer == PlayerMovementTransferOnJump.PermaTransfer)
			) {
				movement.frameVelocity = movingPlatform.platformVelocity;
				movement.velocity += movingPlatform.platformVelocity;
			}
			
			SendMessage("OnFall", SendMessageOptions.DontRequireReceiver);
			tr.position += pushDownOffset * Vector3.up;
		}
		else if (!grounded && IsGroundedTest()) {
			grounded = true;
			jumping.jumping = false;
			SubtractNewPlatformVelocity();
			
			SendMessage("OnLand", SendMessageOptions.DontRequireReceiver);
		}
	
		if (MoveWithPlatform()) {
			movingPlatform.activeGlobalPoint = tr.position + Vector3.up * (controller.center.y - controller.height*0.5f + controller.radius);
			movingPlatform.activeLocalPoint = movingPlatform.activePlatform.InverseTransformPoint(movingPlatform.activeGlobalPoint);
			
	        movingPlatform.activeGlobalRotation = tr.rotation;
	        movingPlatform.activeLocalRotation = Quaternion.Inverse(movingPlatform.activePlatform.rotation) * movingPlatform.activeGlobalRotation; 
		}
	}
	
	void FixedUpdate() {
		if (movingPlatform.enabled) {
			if (movingPlatform.activePlatform != null) {
				if (!movingPlatform.newPlatform) {
					Vector3 lastVelocity = movingPlatform.platformVelocity;
					
					movingPlatform.platformVelocity = (
						movingPlatform.activePlatform.localToWorldMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)
						- movingPlatform.lastMatrix.MultiplyPoint3x4(movingPlatform.activeLocalPoint)
					) / Time.deltaTime;
				}
				movingPlatform.lastMatrix = movingPlatform.activePlatform.localToWorldMatrix;
				movingPlatform.newPlatform = false;
			}
			else {
				movingPlatform.platformVelocity = Vector3.zero;	
			}
		}
		
		if(useFixedUpdate) {
			UpdateFunction();
		}
	}
	
	void Update () {
		if(!useFixedUpdate) {
			UpdateFunction();
		}
		
		#region custom code
		if(Input.GetAxis("Vertical") > 0.1f && inputRun && canRun && walking){//当满足这些条件时
			if(canStand && canStandCrouch){//允许站立且允许下蹲时
				OnRunning();//调用跑动方法
			}
		}else{
			OffRunning();//调用禁止跑动方法
		}	
			
		if ((movement.velocity.x > 0.01f || movement.velocity.z > 0.01f) || (movement.velocity.x < -0.01f || movement.velocity.z < -0.01f)) {//当满足这些条件时
			walking = true;//走动标志位为真
		}else{
			walking = false;//走动标志位设为假
		}
		
		if(!canControl)//如果角色不可控,则不执行后面的代码
			return;
		
		if(movement.canCrouch){//当允许下蹲时
			if(!onLadder){//当角色没有爬梯时
				Crouch();//调用下蹲方法
			}
		}
		
		if(onLadder){//当在爬梯时
			grounded = false;//设置落地标志位假
			crouch = false;//设置下蹲标志为假
		}
		
		if(!crouch && controller.height < standardHeight-0.01f){//当没有下蹲且角色控制器高度小于角色标准高度0.01时
			controller.height = Mathf.Lerp(controller.height, standardHeight, Time.deltaTime/movement.crouchSmooth);//对角色控制器的高度线性插值，使角色控制器平滑到标准高度
			
			Vector3 tempCenter = controller.center;//不能直接用controller.center。y = 0.5f;这种做法是错误的，必须使用临时变量来对controller.center等类型的属性进行赋值
			tempCenter.y = Mathf.Lerp(tempCenter.y, centerY, Time.deltaTime/movement.crouchSmooth);//对角色控制器的中心点Y坐标线性插值
			controller.center = tempCenter;//赋值
			
			Vector3 tempPos = lookObj.transform.localPosition;//同上
			tempPos.y = Mathf.Lerp(tempPos.y, standardHeight, Time.deltaTime/movement.crouchSmooth);//对摄像机坐标线性插值，使摄像机随着角色高度的变换上下移动
			lookObj.transform.localPosition = tempPos;//赋值
		}
		#endregion
	}
	
	#region custom code
	void Crouch(){
		Vector3 up = transform.TransformDirection(Vector3.up);//得到角色的Y方向向量
	   	RaycastHit hit;//射线碰撞引用
		CharacterController charCtrl = GetComponent<CharacterController>();//得到角色控制器组件
	    Vector3 p1 = transform.position;//保存角色位置
		if(inputCrouch && !running && canStand){//当按下C键且角色不处于跑动状态，同时允许站立时，即只有站立状态时按下C键才起作用
			crouch = !crouch;//将标志位置反
		}

	   	 if (!Physics.SphereCast (p1, charCtrl.radius, transform.up, out hit, standardHeight)) {//球形碰撞检测
			if(inputJump && crouch){//当起跳时不允许下蹲
				crouch = false;//设置标志位
			}
			if(running && crouch){//当跑动时不允许下蹲
				crouch = false;//设置标志位
			}
			if(crouch){//当下蹲标志位为真时下蹲
				canStand = true;//设置标志位
			}
			canStandCrouch = true;//设置标志位
	   	}else{
	   		if(crouch){//当下蹲标志位为真时
	   			canStand = false;//设置标志位
	   		}
	   		canStandCrouch = false;//设置标志位
	   	}
		
		if(crouch){//如果下蹲标志位为真
			if(controller.height < movement.crouchHeight+0.01f && controller.height > movement.crouchHeight-0.01f)//如果这些满足条件
				return;//返回
			controller.height = Mathf.Lerp(controller.height, movement.crouchHeight, Time.deltaTime/movement.crouchSmooth);//使用线性插值改变角色控制器高度
			
			Vector3 tempCenterY = controller.center;//得到角色控制器中心点
			tempCenterY.y = Mathf.Lerp(tempCenterY.y, movement.crouchHeight/2, Time.deltaTime/movement.crouchSmooth);//线性插值改变角色控制器中心点位置
			controller.center = tempCenterY;//赋值
				
			Vector3 tempPos = lookObj.transform.localPosition;//得到lookObj对象的位置
			tempPos.y = Mathf.Lerp(tempPos.y, movement.crouchHeight, Time.deltaTime/movement.crouchSmooth);//线性插值改变位置
			lookObj.transform.localPosition = tempPos;//赋值
				
			movement.maxForwardSpeed = movement.crouchSpeed;//赋值
			movement.maxSidewaysSpeed = movement.crouchSpeed;//赋值
			movement.maxBackwardsSpeed = movement.crouchSpeed;//赋值
		}
	}
	
	void OnRunning (){
		running = true;//设置跑动标志位为真
		movement.maxForwardSpeed = movement.runSpeed;//给最大前进速度赋值
		movement.maxSidewaysSpeed = movement.runSpeed;//给侧向最大速度赋值
		jumping.extraHeight = jumping.baseHeight + 0.15f;//赋值
	}
	
	void OffRunning (){
		running = false;//设置跑动标志位为假
		if(crouch) { return; }//如果下蹲时不执行后续代码
			
		movement.maxForwardSpeed = movement.walkSpeed;//给最大前进速度赋值
		movement.maxSidewaysSpeed = movement.walkSpeed;//给侧向最大速度赋值
		movement.maxBackwardsSpeed = movement.walkSpeed/2;//给后退最大速度赋值
		jumping.extraHeight = jumping.baseHeight;//赋值，当角色走动时，跳起的额外高度为跳起的默认高度
	}
	#endregion
	
	private Vector3 ApplyInputVelocityChange(Vector3 velocity) {
		if(!canControl) {
			inputMoveDirection = Vector3.zero;
		}
		
		Vector3 desiredVelocity;
		if(grounded && TooSteep()) {
			desiredVelocity = new Vector3(groundNormal.x, 0, groundNormal.z).normalized;
			Vector3 projectedMoveDir = Vector3.Project(inputMoveDirection, desiredVelocity);
			desiredVelocity = desiredVelocity + projectedMoveDir * sliding.speedControl + (inputMoveDirection - projectedMoveDir) * sliding.sidewaysControl;
			desiredVelocity *= sliding.slidingSpeed;
		}
		else {
			desiredVelocity = GetDesiredHorizontalVelocity();
		}

		if (movingPlatform.enabled && movingPlatform.movementTransfer == PlayerMovementTransferOnJump.PermaTransfer) {
			desiredVelocity += movement.frameVelocity;
			desiredVelocity.y = 0;
		}
		
		if (grounded) {
			desiredVelocity = AdjustGroundVelocityToNormal(desiredVelocity, groundNormal);
		}
		else {
			velocity.y = 0;
		}
		
		float maxVelocityChange = GetMaxAcceleration(grounded) * Time.deltaTime;
		Vector3 velocityChangeVector = (desiredVelocity - velocity);
		if (velocityChangeVector.sqrMagnitude > maxVelocityChange * maxVelocityChange) {
			velocityChangeVector = velocityChangeVector.normalized * maxVelocityChange;
		}
		
		if (grounded || canControl)
			velocity += velocityChangeVector;
		
		if (grounded) {
			velocity.y = Mathf.Min(velocity.y, 0);
		}
		return velocity;
	}
	
	private Vector3 ApplyGravityAndJumping (Vector3 velocity) {
	
		if (!inputJump || !canControl) {
			jumping.holdingJumpButton = false;
			jumping.lastButtonDownTime = -100;
		}
		
		if (inputJump && jumping.lastButtonDownTime < 0 && canControl)
			jumping.lastButtonDownTime = Time.time;
		
		if (grounded)
			velocity.y = Mathf.Min(0, velocity.y) - movement.gravity * Time.deltaTime;
		else {
			velocity.y = movement.velocity.y - movement.gravity * Time.deltaTime;
			
			if (jumping.jumping && jumping.holdingJumpButton) {
				if (Time.time < jumping.lastStartTime + jumping.extraHeight / CalculateJumpVerticalSpeed(jumping.baseHeight)) {
					velocity += jumping.jumpDir * movement.gravity * Time.deltaTime;
				}
			}
			
			velocity.y = Mathf.Max (velocity.y, -movement.maxFallSpeed);
		}
			
		if (grounded) {
			if (jumping.enabled && canControl && (Time.time - jumping.lastButtonDownTime < 0.2)) {
				grounded = false;
				jumping.jumping = true;
				jumping.lastStartTime = Time.time;
				jumping.lastButtonDownTime = -100;
				jumping.holdingJumpButton = true;
				
				if (TooSteep()) {
					jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.steepPerpAmount);
				}
				else {
					jumping.jumpDir = Vector3.Slerp(Vector3.up, groundNormal, jumping.perpAmount);
				}
				
				velocity.y = 0;
				velocity += jumping.jumpDir * CalculateJumpVerticalSpeed (jumping.baseHeight);
				
				if (movingPlatform.enabled &&
					(movingPlatform.movementTransfer == PlayerMovementTransferOnJump.InitTransfer ||
					movingPlatform.movementTransfer == PlayerMovementTransferOnJump.PermaTransfer)
				) {
					movement.frameVelocity = movingPlatform.platformVelocity;
					velocity += movingPlatform.platformVelocity;
				}
				
				SendMessage("OnJump", SendMessageOptions.DontRequireReceiver);
			}
			else {
				jumping.holdingJumpButton = false;
			}
		}
		
		return velocity;
	}
	
	void OnControllerColliderHit (ControllerColliderHit hit) {
		if (hit.normal.y > 0 && hit.normal.y > groundNormal.y && hit.moveDirection.y < 0) {
			if ((hit.point - movement.lastHitPoint).sqrMagnitude > 0.001f || lastGroundNormal == Vector3.zero)
				groundNormal = hit.normal;
			else
				groundNormal = lastGroundNormal;
			
			movingPlatform.hitPlatform = hit.collider.transform;
			movement.hitPoint = hit.point;
			movement.frameVelocity = Vector3.zero;
		}
		
		#region custom code
		if(pushing.canPush){//如果允许推
			Rigidbody body = hit.collider.attachedRigidbody;//得到射线检测到的对象上附加的刚体
			if (body == null || body.isKinematic)//如果刚体不存在或者刚体的isKinematic参数为假
				return;//返回
	
			if (hit.moveDirection.y < -0.3f)//如果被推动的对象比角色矮
				return;//返回
	
			Vector3 pushDir = new Vector3 (hit.moveDirection.x, 0, hit.moveDirection.z);//计算推动的方向的向量
		
			body.velocity = pushDir * pushing.pushPower;//设置速度
		}
		#endregion
	}
	
	private IEnumerator SubtractNewPlatformVelocity () {
		if (movingPlatform.enabled &&
			(movingPlatform.movementTransfer == PlayerMovementTransferOnJump.InitTransfer ||
			movingPlatform.movementTransfer == PlayerMovementTransferOnJump.PermaTransfer)
		) {
			if (movingPlatform.newPlatform) {
				Transform platform = movingPlatform.activePlatform;
				yield return new WaitForFixedUpdate();
				yield return new WaitForFixedUpdate();
				if (grounded && platform == movingPlatform.activePlatform)
					yield return 1;
			}
			movement.velocity -= movingPlatform.platformVelocity;
		}
	}
	
	private bool MoveWithPlatform () {
		return (
			movingPlatform.enabled
			&& (grounded || movingPlatform.movementTransfer == PlayerMovementTransferOnJump.PermaLocked)
			&& movingPlatform.activePlatform != null
		);
	}
	
	private Vector3 GetDesiredHorizontalVelocity () {
		Vector3 desiredLocalDirection = tr.InverseTransformDirection(inputMoveDirection);
		float maxSpeed = MaxSpeedInDirection(desiredLocalDirection);
		if (grounded) {
			float movementSlopeAngle = Mathf.Asin(movement.velocity.normalized.y)  * Mathf.Rad2Deg;
			maxSpeed *= movement.slopeSpeedMultiplier.Evaluate(movementSlopeAngle);
		}
		return tr.TransformDirection(desiredLocalDirection * maxSpeed);
	}
	
	private Vector3 AdjustGroundVelocityToNormal (Vector3 hVelocity, Vector3 groundNormal) {
		Vector3 sideways = Vector3.Cross(Vector3.up, hVelocity);
		return Vector3.Cross(sideways, groundNormal).normalized * hVelocity.magnitude;
	}
	
	private bool IsGroundedTest () {
		return (groundNormal.y > 0.01f);
	}
	
	float GetMaxAcceleration (bool grounded) {
		if (grounded)
			return movement.maxGroundAcceleration;
		else
			return movement.maxAirAcceleration;
	}
	
	float CalculateJumpVerticalSpeed (float targetJumpHeight) {
		return Mathf.Sqrt (2 * targetJumpHeight * movement.gravity);
	}
	
	public bool IsJumping () {
		return jumping.jumping;
	}
	
	public bool IsSliding () {
		return (grounded && sliding.enabled && TooSteep());
	}
	
	public bool IsTouchingCeiling () {
		return (movement.collisionFlags & CollisionFlags.CollidedAbove) != 0;
	}
	
	public bool IsGrounded () {
		return grounded;
	}
	
	public bool TooSteep () {
		return (groundNormal.y <= Mathf.Cos(controller.slopeLimit * Mathf.Deg2Rad));
	}
	
	public Vector3 GetDirection () {
		return inputMoveDirection;
	}
	
	public void SetControllable (bool controllable) {
		canControl = controllable;
	}
	
	float MaxSpeedInDirection (Vector3 desiredMovementDirection) {
		if (desiredMovementDirection == Vector3.zero)
			return 0;
		else {
			float zAxisEllipseMultiplier = (desiredMovementDirection.z > 0 ? movement.maxForwardSpeed : movement.maxBackwardsSpeed) / movement.maxSidewaysSpeed;
			Vector3 temp = new Vector3(desiredMovementDirection.x, 0, desiredMovementDirection.z / zAxisEllipseMultiplier).normalized;
			float length = new Vector3(temp.x, 0, temp.z * zAxisEllipseMultiplier).magnitude * movement.maxSidewaysSpeed;
			return length;
		}
	}
	
	void SetVelocity (Vector3 velocity) {
		grounded = false;
		movement.velocity = velocity;
		movement.frameVelocity = Vector3.zero;
		SendMessage("OnExternalVelocity");
	}
}
