extends StaticBody2D

func on_collide(other):
	PlayerState.decrease_health(10)