using System.ComponentModel;

namespace CommonEnum
{
    public class CommonEnum
    {
        public enum DroneStateEnum
        {
            [Description("IDLE")]
            IDLE = 1,
            [Description("LOADING")]
            LOADING = 2,
            [Description("LOADED")]
            LOADED = 3,
            [Description("DELIVERING")]
            DELIVERING = 4,
            [Description("DELIVERED")]
            DELIVERED = 5,
            [Description("RETURNING")]
            RETURNING = 6
        }

        public enum DroneModelEnum
        {
            [Description("LIGHTWEIGHT")]
            LIGHTWEIGHT = 1,
            [Description("MIDDLEWEIGHT")]
            MIDDLEWEIGHT = 2,
            [Description("CRUISERWEIGHT")]
            CRUISERWEIGHT = 3,
            [Description("HEAVYWEIGHT")]
            HEAVYWEIGHT = 4,
            
        }
    }
}