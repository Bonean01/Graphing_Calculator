using System.Collections.Generic;

public class Camera
{
	float xCoord;
	float yCoord;
	float speed;
	public Camera(float xCoord, float yCoord, float speed)
	{
		this.xCoord = xCoord;
		this.yCoord = yCoord;
		this.speed = speed;
	}
	public float getXCoord() { return xCoord; }
	public float getYCoord() { return yCoord; }
	public float getSpeed() { return speed; }
	public void MoveUp() { yCoord += speed; }
	public void MoveDown() { yCoord -= speed; }
	public void MoveLeft() { xCoord -= speed; }
	public void MoveRight() { xCoord += speed; }

	// Makes the speed aligned with the world coordinates.
	public void UpdateSpeed(float tileSize) { speed = tileSize / 10; }
}