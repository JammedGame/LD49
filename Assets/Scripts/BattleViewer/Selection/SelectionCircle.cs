using Game.View;
using UnityEngine;
using UnityEngine.Serialization;

public class SelectionCircle : MonoBehaviour
{
	private const string PrefabPath = "Selection/SelectionCircle";

	[SerializeField] private MeshRenderer meshRenderer;

	[SerializeField] private Material defaultMaterial;

	[FormerlySerializedAs("greenMaterial")] [SerializeField]
	private Material friendMaterial;

	[FormerlySerializedAs("redMaterial")] [SerializeField]
	private Material enemyMaterial;

	[SerializeField] private Material friendActionMaterial;

	[SerializeField] private Material enemyActionMaterial;

	private BattleObjectView followedView;

	private void Update()
	{
		SyncWithFollowedView();
	}

	public static SelectionCircle LoadPrefab()
	{
		return Resources.Load<SelectionCircle>(PrefabPath);
	}

	public void ShowDefault()
	{
		meshRenderer.material = defaultMaterial;
		Show();
	}

	public void ShowFriend()
	{
		meshRenderer.material = friendMaterial;
		Show();
	}

	public void ShowEnemy()
	{
		meshRenderer.material = enemyMaterial;
		Show();
	}

	public void ShowFriendAction()
	{
		meshRenderer.material = friendActionMaterial;
		Show();
	}

	public void ShowEnemyAction()
	{
		meshRenderer.material = enemyActionMaterial;
		Show();
	}

	private void Show()
	{
		gameObject.SetActive(true);
	}

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	public void Follow(BattleObjectView targetView)
	{
		followedView = targetView;
		SyncWithFollowedView();
	}

	private void SyncWithFollowedView()
	{
		if (followedView != null)
		{
			var myTransform = transform;
			var followedTransform = followedView.transform;
			myTransform.position = followedTransform.position;
			myTransform.localScale = followedTransform.localScale;
		}
		else
		{
			Hide();
		}
	}
}