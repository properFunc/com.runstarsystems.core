using Unity.Entities;

namespace RunstarSystems.ECS.Components
{
    public struct DuelStickInputPacket
    {
        public byte local_player_index;

        public short move_x;
        public short move_y;

        public short target_x;
        public short target_y;

        public ushort held_actions;
        public ushort pressed_actions;
        public ushort released_actions;
    }

    public struct DuelStickClientInputPacket : IComponentData
    {
        public uint frame;
        public byte input_count;

        public DuelStickInputPacket input_0;
        public DuelStickInputPacket input_1;
    }
}
