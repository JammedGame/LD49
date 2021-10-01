using UnityEngine;

[CreateAssetMenu]
public class CameraSettings : ScriptableObject
{
	[Header("Angle")]
	public float Pitch;
	public float Yaw;
	public float Roll;

	[Header("FoV")]
	public float FieldOfView;

	[Header("Keyboard")]
	public float KeyboardPanSpeed;
	public float KeyboardYawSpeed;

	[Header("Zoom")]
	public float StartZoom;
	public float MinZoom, MaxZoom;
	public float ScrollWheelZoomSpeed;

	[Header("Follow target")]
	public Vector3 FollowOffset;
	public bool MatchYaw;

	void OnValidate()
	{
		if (MinZoom > StartZoom) MinZoom = StartZoom;
		if (MaxZoom < StartZoom) MaxZoom = StartZoom;
	}
}