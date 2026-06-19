using Unity.Entities;
using UnityEngine;

using settings = RunstarSystems.ECS.Settings;

namespace RunstarSystems.ECS.Authoring
{
    // Connects the ecs data to the editor
    public sealed class BallQuantAuthor : MonoBehaviour
    {
        [Tooltip("Setting for rollback numerical stablization")]
        public settings.BallQuantSettings author_settings =
                settings.BallQuantSettings.CreateDefault();
    }

    // Bakes the editor data into the ecs data
    public sealed class BallQuantBaker : Baker<BallQuantAuthor>
    {
        public override void Bake(BallQuantAuthor authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            // You can have cascading updates here whenever you change the data
            AddComponent(entity, authoring.author_settings.ToRuntimeData());
        }
    }
}
