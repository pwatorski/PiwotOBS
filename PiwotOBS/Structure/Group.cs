using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PiwotOBS.Structure
{
    public class Group : Container
    {
        public override void BuildChildren()
        {
            var items = OBSDeck.OBS.GetGroupSceneItemList(Name);
            BuildChildren(items);
        }
        public override void BuildOBSChildren()
        {
            var items = OBSDeck.OBS.GetGroupSceneItemList(Name);
            BuildOBSChildren(items);
        }
    }
}
