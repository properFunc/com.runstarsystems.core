using Unity.Entities;

namespace RunstarSystems.ECS.Components
{
    public struct DualStickInputFrame : IComponentData
    {
        public byte local_player_index;
        public uint update_frame;

        // body
        public float move_x;
        public float move_y;

        // paddle
        public float target_x;
        public float target_y;

        // other actions
        public ushort held_actions;
        public ushort pressed_actions;
        public ushort released_actions;
    }

    public struct LocalPlayerInput : IComponentData
    {
        public byte local_player_index;
        public int input_source_type;
        public int input_source_index;
    }
}
