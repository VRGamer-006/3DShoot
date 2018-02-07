using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour {
	public bool canControl = true;
	public bool useFixedUpdate = true;
	
	#region custom code
	[HideInInspector]//�ڼ�����ͼ�����ر�����������ʾ��������
	public bool running;//�����ܶ�״̬��־λ
	[HideInInspector]
	public bool walking;//�����߶�״̬��־λ
	[HideInInspector]
	public bool canRun;//�Ƿ��ܹ��ܶ���־λ
	
	private GameObject mainCamera = null;//���������������
	
	[HideInInspector]
	public bool onLadder = false;//�Ƿ���������
	private float ladderHopSpeed = 6.0f;//�����ٶ�
	#endregion
	
	[System.NonSerialized]
	public Vector3 inputMoveDirection = Vector3.zero;
	
	[System.NonSerialized]
	public bool inputJump = false;
	
	#region custom code
	[System.NonSerialized]
	public bool inputRun = false;//�Ƿ�����ܶ���־λ
	
	[System.NonSerialized]
	public bool inputCrouch = false;//�Ƿ��¶ױ�־λ
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
		public float walkSpeed = 6.0f;//�����ٶ�
		public float runSpeed = 9.0f;//�ܶ��ٶ�
		
		public bool canCrouch = true;//�Ƿ�����¶ױ�־λ
		public float crouchSpeed = 3.0f;//�¶��ٶ�
		public float crouchHeight = 1.2f;//�¶�ʱ�ĸ߶�
		public float crouchSmooth = 0.1f;//�¶�ʱ��ƽ��
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
	public bool crouch;//�¶ױ�־λ
	private float standardHeight;//��׼�߶�
	private GameObject lookObj;//��������壬��֤�ӽǵ���ȷ
	private float centerY;//��ֱ�������ĵ�
	private bool canStand;//�ܷ�վ���жϱ���
	private bool canStandCrouch = true;//�ܷ�վ���¶��жϱ���
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
		public bool canPush = true;//�Ƿ���ƶ������־λ
		public float pushPower = 2.0f;//�ƶ�������
	}
	public PlayerControllerPushing pushing;//��������
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
		standardHeight = controller.height;//�õ���ɫ��׼���
		lookObj = GameObject.FindWithTag("LookObject");//�õ���Ϸ����
		centerY = controller.center.y;//�õ���ɫ��ֱ�����ϵ����ĵ�
		mainCamera = GameObject.FindWithTag("MainCamera");//�õ������������
		canRun = true;//��ֵ
		canStand = true;//��ֵ
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
		if(Input.GetAxis("Vertical") > 0.1f && inputRun && canRun && walking){//��������Щ����ʱ
			if(canStand && canStandCrouch){//����վ���������¶�ʱ
				OnRunning();//�����ܶ�����
			}
		}else{
			OffRunning();//���ý�ֹ�ܶ�����
		}	
			
		if ((movement.velocity.x > 0.01f || movement.velocity.z > 0.01f) || (movement.velocity.x < -0.01f || movement.velocity.z < -0.01f)) {//��������Щ����ʱ
			walking = true;//�߶���־λΪ��
		}else{
			walking = false;//�߶���־λ��Ϊ��
		}
		
		if(!canControl)//�����ɫ���ɿ�,��ִ�к���Ĵ���
			return;
		
		if(movement.canCrouch){//�������¶�ʱ
			if(!onLadder){//����ɫû������ʱ
				Crouch();//�����¶׷���
			}
		}
		
		if(onLadder){//��������ʱ
			grounded = false;//������ر�־λ��
			crouch = false;//�����¶ױ�־Ϊ��
		}
		
		if(!crouch && controller.height < standardHeight-0.01f){//��û���¶��ҽ�ɫ�������߶�С�ڽ�ɫ��׼�߶�0.01ʱ
			controller.height = Mathf.Lerp(controller.height, standardHeight, Time.deltaTime/movement.crouchSmooth);//�Խ�ɫ�������ĸ߶����Բ�ֵ��ʹ��ɫ������ƽ������׼�߶�
			
			Vector3 tempCenter = controller.center;//����ֱ����controller.center��y = 0.5f;���������Ǵ���ģ�����ʹ����ʱ��������controller.center�����͵����Խ��и�ֵ
			tempCenter.y = Mathf.Lerp(tempCenter.y, centerY, Time.deltaTime/movement.crouchSmooth);//�Խ�ɫ�����������ĵ�Y�������Բ�ֵ
			controller.center = tempCenter;//��ֵ
			
			Vector3 tempPos = lookObj.transform.localPosition;//ͬ��
			tempPos.y = Mathf.Lerp(tempPos.y, standardHeight, Time.deltaTime/movement.crouchSmooth);//��������������Բ�ֵ��ʹ��������Ž�ɫ�߶ȵı任�����ƶ�
			lookObj.transform.localPosition = tempPos;//��ֵ
		}
		#endregion
	}
	
	#region custom code
	void Crouch(){
		Vector3 up = transform.TransformDirection(Vector3.up);//�õ���ɫ��Y��������
	   	RaycastHit hit;//������ײ����
		CharacterController charCtrl = GetComponent<CharacterController>();//�õ���ɫ���������
	    Vector3 p1 = transform.position;//�����ɫλ��
		if(inputCrouch && !running && canStand){//������C���ҽ�ɫ�������ܶ�״̬��ͬʱ����վ��ʱ����ֻ��վ��״̬ʱ����C����������
			crouch = !crouch;//����־λ�÷�
		}

	   	 if (!Physics.SphereCast (p1, charCtrl.radius, transform.up, out hit, standardHeight)) {//������ײ���
			if(inputJump && crouch){//������ʱ�������¶�
				crouch = false;//���ñ�־λ
			}
			if(running && crouch){//���ܶ�ʱ�������¶�
				crouch = false;//���ñ�־λ
			}
			if(crouch){//���¶ױ�־λΪ��ʱ�¶�
				canStand = true;//���ñ�־λ
			}
			canStandCrouch = true;//���ñ�־λ
	   	}else{
	   		if(crouch){//���¶ױ�־λΪ��ʱ
	   			canStand = false;//���ñ�־λ
	   		}
	   		canStandCrouch = false;//���ñ�־λ
	   	}
		
		if(crouch){//����¶ױ�־λΪ��
			if(controller.height < movement.crouchHeight+0.01f && controller.height > movement.crouchHeight-0.01f)//�����Щ��������
				return;//����
			controller.height = Mathf.Lerp(controller.height, movement.crouchHeight, Time.deltaTime/movement.crouchSmooth);//ʹ�����Բ�ֵ�ı��ɫ�������߶�
			
			Vector3 tempCenterY = controller.center;//�õ���ɫ���������ĵ�
			tempCenterY.y = Mathf.Lerp(tempCenterY.y, movement.crouchHeight/2, Time.deltaTime/movement.crouchSmooth);//���Բ�ֵ�ı��ɫ���������ĵ�λ��
			controller.center = tempCenterY;//��ֵ
				
			Vector3 tempPos = lookObj.transform.localPosition;//�õ�lookObj�����λ��
			tempPos.y = Mathf.Lerp(tempPos.y, movement.crouchHeight, Time.deltaTime/movement.crouchSmooth);//���Բ�ֵ�ı�λ��
			lookObj.transform.localPosition = tempPos;//��ֵ
				
			movement.maxForwardSpeed = movement.crouchSpeed;//��ֵ
			movement.maxSidewaysSpeed = movement.crouchSpeed;//��ֵ
			movement.maxBackwardsSpeed = movement.crouchSpeed;//��ֵ
		}
	}
	
	void OnRunning (){
		running = true;//�����ܶ���־λΪ��
		movement.maxForwardSpeed = movement.runSpeed;//�����ǰ���ٶȸ�ֵ
		movement.maxSidewaysSpeed = movement.runSpeed;//����������ٶȸ�ֵ
		jumping.extraHeight = jumping.baseHeight + 0.15f;//��ֵ
	}
	
	void OffRunning (){
		running = false;//�����ܶ���־λΪ��
		if(crouch) { return; }//����¶�ʱ��ִ�к�������
			
		movement.maxForwardSpeed = movement.walkSpeed;//�����ǰ���ٶȸ�ֵ
		movement.maxSidewaysSpeed = movement.walkSpeed;//����������ٶȸ�ֵ
		movement.maxBackwardsSpeed = movement.walkSpeed/2;//����������ٶȸ�ֵ
		jumping.extraHeight = jumping.baseHeight;//��ֵ������ɫ�߶�ʱ������Ķ���߶�Ϊ�����Ĭ�ϸ߶�
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
		if(pushing.canPush){//���������
			Rigidbody body = hit.collider.attachedRigidbody;//�õ����߼�⵽�Ķ����ϸ��ӵĸ���
			if (body == null || body.isKinematic)//������岻���ڻ��߸����isKinematic����Ϊ��
				return;//����
	
			if (hit.moveDirection.y < -0.3f)//������ƶ��Ķ���Ƚ�ɫ��
				return;//����
	
			Vector3 pushDir = new Vector3 (hit.moveDirection.x, 0, hit.moveDirection.z);//�����ƶ��ķ��������
		
			body.velocity = pushDir * pushing.pushPower;//�����ٶ�
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
