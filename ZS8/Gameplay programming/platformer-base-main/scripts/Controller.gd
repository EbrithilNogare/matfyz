# Simple movement and jump
# Wrapping around the edges of the screen

extends CharacterBody2D

@export var maxSpeed: int = 400
@export var jump_force: int = 650

@export var coyoteTime: float = 0.2
@export var jumpWithDelay: float = 0.2
@export var velocitySpeedup: float = 50
@export var velocitySlowdown: float = 25
@export var jumpDurationProlong: float = .15

@export var ropeSpawner: Node2D

var timeFromLastOnFloor: float = coyoteTime + 1
var timeFromLastJumpPressed: float = jumpWithDelay + 1
var hangingOnRope: bool = false
var gravity = ProjectSettings.get_setting("physics/2d/default_gravity") * 2
var attached_rope_piece_index: int = -1


func _physics_process(delta):
	if hangingOnRope:
		velocity.y = 0
		processPhysicsOnRope(delta)
	else:
		velocity.y += gravity * delta
		processPhysicsOnGround(delta)


func processPhysicsOnGround(delta):
	# Rope controlls
	if(Input.is_action_just_pressed("Q")):
		trySpawnRope(true)
	elif(Input.is_action_just_pressed("E")):
		trySpawnRope(false)
	elif(Input.is_action_just_pressed("F")):
		ropeSpawner.addPieceOfRope()
	elif(Input.is_action_just_pressed("R")):
		ropeSpawner.removePieceOfRope()


	if is_on_floor():
		timeFromLastOnFloor = 0
	else:
		timeFromLastOnFloor += delta

	if Input.is_action_pressed("Left"):
		move_left()
	elif Input.is_action_pressed("Right"):
		move_right()
	else:
		move_stop()
		
	if Input.is_action_just_pressed("Jump"):
		if timeFromLastOnFloor < coyoteTime:
			jump()
		else:
			timeFromLastJumpPressed = 0
	elif Input.is_action_pressed("Jump"):
		keepJumping()
		
	if !is_on_floor():
		if velocity.y < 0:
			$AnimatedSprite2D.play("jump")
		else :
			$AnimatedSprite2D.play("fall")
	
	if timeFromLastJumpPressed <= jumpWithDelay && timeFromLastOnFloor < coyoteTime:
		jump()
	timeFromLastJumpPressed += delta;
	move_and_slide()
		

func processPhysicsOnRope(_delta):
	if Input.is_action_just_pressed("Jump"):
		hangingOnRope = false
		return

	if Input.is_action_pressed("Left"):
		move_left()
	elif Input.is_action_pressed("Right"):
		move_right()
	else:
		move_stop()

	if Input.is_action_just_pressed("Up"):
		if attached_rope_piece_index > 1:
			attached_rope_piece_index -= 1
			var new_rope_piece = ropeSpawner.rope_pieces[attached_rope_piece_index]
			global_position = new_rope_piece.global_position
	elif Input.is_action_just_pressed("Down"):
		if attached_rope_piece_index < ropeSpawner.rope_pieces.size() - 1:
			attached_rope_piece_index += 1
			var new_rope_piece = ropeSpawner.rope_pieces[attached_rope_piece_index]
			global_position = new_rope_piece.global_position
	else:
		pass


func move_left():
	velocity.x = clamp(velocity.x - velocitySpeedup, -maxSpeed, maxSpeed)
	$AnimatedSprite2D.play("walk")
	$AnimatedSprite2D.flip_h = true  # face left
	

func move_right():
	velocity.x = clamp(velocity.x + velocitySpeedup, -maxSpeed, maxSpeed)
	$AnimatedSprite2D.play("walk")
	$AnimatedSprite2D.flip_h = false  # face right
	

func move_stop():
	if velocity.x > 0:
		velocity.x = max(0, velocity.x - velocitySlowdown)
	elif velocity.x < 0:
		velocity.x = min(0, velocity.x + velocitySlowdown)
	$AnimatedSprite2D.play("idle")


func jump():
	timeFromLastJumpPressed = jumpWithDelay + 1
	velocity.y = -jump_force


func keepJumping():
	if timeFromLastOnFloor < jumpDurationProlong:
		velocity.y = -jump_force


func trySpawnRope(left: bool = true):
	var ropeLength = 250
	ropeSpawner.removeRope()

	var raycast = $RayCast2D
	raycast.target_position = Vector2((-ropeLength if left else ropeLength ), -ropeLength)
	raycast.force_raycast_update()

	if raycast.is_colliding():
		var collision_point = raycast.get_collision_point()
		var collision_normal = raycast.get_collision_normal()
		ropeSpawner.spawnNewRope(collision_point + collision_normal * 20, global_position)
	else:
		ropeSpawner.spawnNewRope(global_position + Vector2((-ropeLength if left else ropeLength ), -ropeLength), global_position)


func _on_rope_area_2d_body_entered(body: RopePiece) -> void:

	if hangingOnRope:
		return

	hangingOnRope = true
	velocity = Vector2()
	attached_rope_piece_index = ropeSpawner.rope_pieces.find(body)
	print("Hanging on rope: ", attached_rope_piece_index)
