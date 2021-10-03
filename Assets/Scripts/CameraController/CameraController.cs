using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public CameraSettings Settings;
	public Camera Camera;

	[Header("Panning")]
	public float MinX;
	public float MaxX;
	public float MinZ;
	public float MaxZ;

	[Header("Current")]
	public float Distance;
	public float Yaw;
	public Vector3 Position;

	void Start()
	{
		Distance = Settings.StartZoom;
		Yaw = Settings.Yaw;
	}

	public void UpdateCamera()
	{
		// update position.
		var offset = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Settings.KeyboardPanSpeed * Time.deltaTime;
		offset = Quaternion.Euler(0, Yaw, 0) * offset;
		Position += offset;

		// update zoom
		Distance -= Input.GetAxis("Mouse ScrollWheel") * Settings.ScrollWheelZoomSpeed * Time.deltaTime;

		// update rotation
		Yaw += Input.GetAxis("Yaw") * Settings.KeyboardYawSpeed * Time.deltaTime;

		// validate new value
		ClampValues();

		// update transform.
		transform.SetPositionAndRotation(Position, Quaternion.Euler(Settings.Pitch, Yaw, Settings.Roll));
		Camera.fieldOfView = Settings.FieldOfView;

		if (Distance != lastDistance)
		{
			QualitySettings.shadowDistance = Distance * 1.5f;
			Camera.transform.localPosition = new Vector3(0, 0, -Distance);
			lastDistance = Distance;
		}
	}

	float lastDistance;

	private void ClampValues()
	{
		Distance = Mathf.Clamp(Distance, Settings.MinZoom, Settings.MaxZoom);
		Position.x = Mathf.Clamp(Position.x, MinX, MaxX);
		Position.z = Mathf.Clamp(Position.z, MinZ, MaxZ);
	}

	public void SetCentralPosition()
	{
		Position = new Vector3(MinX + MaxX, 0, MinZ + MaxZ) / 2f;
	}
}