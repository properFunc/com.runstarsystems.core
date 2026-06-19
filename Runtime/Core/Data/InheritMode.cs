using System;

namespace RunstarSystems.ECS.Data
{
    /*
    *   @TODO   Think about wether or not pass only is useful
    *           Such as parent not getting the system but child
    *           does.
    */
    public enum InheritMode
    {
        DirectOnly = 0, // You want the effect but not for children
        Inheritable = 1 // Passes effect to children as well
    }
}
