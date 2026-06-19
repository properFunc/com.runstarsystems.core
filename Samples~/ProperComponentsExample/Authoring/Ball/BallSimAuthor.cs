using Unity.Entities;
using UnityEngine;

using settings = RunstarSystems.ECS.Settings;

namespace RunstarSystems.ECS.Authoring
{
    // Connects the ecs data to the editor
    public sealed class BallSimAuthor : MonoBehaviour
    {
        [Tooltip("Setting for physics sim related data on the ball")]
        public settings.BallSimSettings author_settings =
                settings.BallSimSettings.CreateDefault();
    }

    // Bakes the editor data into the ecs data
    public sealed class BallSimBaker : Baker<BallSimAuthor>
    {
        public override void Bake(BallSimAuthor authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            AddComponent(entity, authoring.author_settings.ToRuntimeData());
        }
    }
}
