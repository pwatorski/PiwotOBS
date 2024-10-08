﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace PiwotOBS.Structure
{
    public class Container : SceneItem
    {
        public List<SceneItem> Items { get; protected set; }

        public Container()
        {
            Items = new List<SceneItem>();
        }

        public void AddItem(SceneItem item)
        {
            if (Items.Contains(item)) return;
            Items.Add(item);
            item.SetParent(this);
        }

        public void RemoveItem(SceneItem item)
        {
            Items.Remove(item);
            item.ClearParent();
        }

        public SceneItem? FindItem(string name)
        {
            var item = Items.Find((x)=>x.Name == name);
            if(item != null) return item;
            foreach(var subItem in Items)
            {
                if (subItem is Container container)
                {
                    item = container.FindItem(name);
                    if (item != null) return item;
                }
            }
            return null;
        }


        public virtual void BuildChildren()
        {
            Console.WriteLine($"BUILD CONTAINER: {Name}");
            throw new NotSupportedException("This method should never be invoked.");
        }

        public virtual void BuildOBSChildren()
        {
            Console.WriteLine($"BUILD CONTAINER: {Name}");
            throw new NotSupportedException("This method should never be invoked.");
        }

        public virtual void BuildChildren(IEnumerable<JsonObject> items)
        {
            Items?.Clear();
            Items = new List<SceneItem>();
            foreach (var item in items)
            {
                AddItem(SceneItem.FromJson(item));
            }
        }

        public virtual void BuildOBSChildren(IEnumerable<JsonObject> items)
        {
            Items?.Clear();
            Items = new List<SceneItem>();
            foreach (var item in items)
            {
                AddItem(SceneItem.FromOBSJson(item));
            }
        }

        public SceneItem? GetChild(string name)
        {
            foreach (var item in Items)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }

        public List<T> GetChildrenOfType<T>(bool recursive=false) where T : SceneItem
        {
            List<T> children = new List<T>();
            foreach (var item in Items)
            {
                if (item is T)
                {
                    children.Add((T)item);
                }
                if(recursive && item is Container)
                {
                    children.AddRange(((Container)item).GetChildrenOfType<T>(recursive));
                }
            }
            return children;
        }

        public string GetTreeString()
        {
            return AddTreeStringBranches($"#{Name}\n", 1);
        }
        protected string AddTreeStringBranches(string str, int depth=0)
        {
            foreach (var item in Items)
            {
                if (item is Container container)
                {
                    if(item is Scene)
                    {
                        str += "".PadLeft(depth, ' ') + $"#{item.Name}\n";
                    }
                    else
                    {
                        str += "".PadLeft(depth, ' ') + $"*{item.Name}\n";
                    }
                    
                    str = container.AddTreeStringBranches(str, depth + 1);
                }
                else
                {
                    str += "".PadLeft(depth, ' ') + $"-{item.Name}\n";
                }
            }
            return str;
        }

        public override JsonObject ToJson()
        {
            JsonObject json = base.ToJson();
            if (json["sourceType"] == null) json["sourceType"] = "OBS_SOURCE_TYPE_SCENE";
            if (json["isGroup"] == null) json["isGroup"] = IsGroup;
            json.Add("items", new JsonArray(Items.Select((x)=>x.ToJson()).ToArray()));
            return json;
        }
        public override JsonObject ToOBSJson()
        {
            JsonObject json = base.ToOBSJson();
            if (json["sourceType"] == null) json["sourceType"] = "OBS_SOURCE_TYPE_SCENE";
            if (json["isGroup"] == null) json["isGroup"] = IsGroup;
            return json;
        }
        //public OBSObject FindChild(string name, string? sceneName = null)
        //{
        //    if (sceneName == null)
        //    {
        //        sceneName
        //    }
        //    foreach (var child in Children)
        //    {
        //        if (child.SceneItemId == id)
        //            if (child is Container)
        //            {

        //            }
        //    }
        //}
    }
}
