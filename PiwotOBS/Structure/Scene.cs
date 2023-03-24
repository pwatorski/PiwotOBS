using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PiwotOBS.Structure
{
    public class Scene : Container
    {
        public Scene() 
        {
        }
        protected Scene(string sceneName) 
        {
            SourceName = sceneName;
            BuildOBSChildren();
        }
        public override void BuildChildren()
        {
            var items = OBSDeck.OBS.GetSceneItemList(Name);
            BuildChildren(items);
        }

        public override void BuildOBSChildren()
        {
            var items = OBSDeck.OBS.GetSceneItemList(Name);
            BuildOBSChildren(items);
        }

        public static Scene GetRootScene(string sceneName)
        {
            return new Scene(sceneName);
        }
    }
}
