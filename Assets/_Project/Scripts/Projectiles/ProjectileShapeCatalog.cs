using UnityEngine;

namespace Projectiles
{
    [CreateAssetMenu(fileName = "ProjectileShapeCatalog", menuName = "Flexus Test/Projectile Shape Catalog")]
    public class ProjectileShapeCatalog : ScriptableObject
    {
        [SerializeField] private ProjectileShapeDefinition[] _shapes;
        
        public ProjectileShapeDefinition GetRandom()
        {
            return _shapes[Random.Range(0, _shapes.Length)];
        }
    }
}