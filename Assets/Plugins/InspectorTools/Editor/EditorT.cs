namespace UnityEditor
{
	public class Editor<T> : Editor where T : UnityEngine.Object
	{
		public new T target => (T)base.target;
	}
}