using ZenjectInstaller.Bind;

#if UNITY_EDITOR
namespace ZenjectInstaller.DiGraph
{
    [UnityEngine.CreateAssetMenu(menuName = "DI/Graph Snapshot")]
    public sealed class DiGraphSnapshot : UnityEngine.ScriptableObject
    {
        public System.Collections.Generic.List<BindSpec> specs = new();
    }
}
#endif