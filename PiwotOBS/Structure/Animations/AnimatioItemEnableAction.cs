using System.Text.Json.Nodes;

namespace PiwotOBS.Structure.Animations
{
    public class AnimatioItemEnableAction
    {
        public bool Enable { get; protected set; }
        public bool Disable { get; protected set; }
        public AnimatioItemEnableAction(bool enable, bool disable)
        {
            Enable = enable;
            Disable = disable;
        }
        public JsonObject ToJson()
        {
            return new JsonObject
            {
                { nameof(Enable), Enable },
                { nameof(Disable), Disable }
            };
        }

        public static AnimatioItemEnableAction FromJson(JsonObject source)
        {
            bool? enable = (bool?)source["Enable"] ?? throw new Exception("No enable flag!");
            bool? disable = (bool?)source["Disable"] ?? throw new Exception("No disable flag!");
            return new AnimatioItemEnableAction((bool)enable, (bool)disable);
        }

    }
}
