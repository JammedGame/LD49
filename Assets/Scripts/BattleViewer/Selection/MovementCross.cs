using System.Collections;
using UnityEngine;

public class MovementCross : MonoBehaviour
{
	private const string PrefabPath = "Selection/MovementCross";

	[SerializeField] private float hideAfterSeconds = 1f;

	private Coroutine hideCoroutine;

	public static MovementCross LoadPrefab()
	{
		return Resources.Load<MovementCross>(PrefabPath);
	}

	public void ShowTemporarily(Vector3 position)
	{
		if (hideCoroutine != null)
		{
			StopCoroutine(hideCoroutine);
			hideCoroutine = null;
		}

		Show();
		transform.localPosition = position;
		hideCoroutine = StartCoroutine(HideFlow());
	}

	private IEnumerator HideFlow()
	{
		yield return new WaitForSeconds(hideAfterSeconds);

		Hide();
		hideCoroutine = null;
	}

	private void Show()
	{
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}
}