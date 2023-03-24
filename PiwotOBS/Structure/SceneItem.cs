using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PiwotOBS.Structure
{

    public partial class SceneItem
    {
        public SceneItem? Parent { get; protected set; }

        internal void ClearParent()
        {
            Parent = null;
        }

        internal void SetParent(SceneItem parent) 
        {
            Parent = parent;
        }

    }
}
