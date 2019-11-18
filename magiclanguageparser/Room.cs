using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MagicLanguageParser
{
    public class Room
    {
        public string roomName;
        public string shortDescription;
        public string longDescription;
        public string roomKey;
        public Dictionary<string,NounObject> Children = new Dictionary<string, NounObject>();
        public Dictionary<string, NounObject> allNounsInScene = new Dictionary<string, NounObject>();

        public Dictionary<RoomDirections, Func<String>> adjoiningRooms = new Dictionary<RoomDirections, Func<String>>();

        public Action<Room> onEntranceFirstTime;

        public Room SetOnEntranceFirstTime(Action<Room> en)
        {
            onEntranceFirstTime = en;
            return this;
        } 


        public Room AddNoun(string name, NounObject obj)
        {
            Children.Add(name,obj);
            RegisterNounInScene(name,obj);
            return this;
        }

        public Room AddNounChild( string name,string objname, NounObject obj)
        {
            Children[name].nounChildren.Add(objname,obj);
            RegisterNounInScene(objname, obj);
            return this;
        }
        public Room RegisterNounInScene(string name, NounObject obj)
        {
            allNounsInScene.Add(name,obj);
            obj.myKey = name;
            return this;
        }
       

        public void RemoveNoun(string noun)
        {
            Children.Remove(noun);
            foreach (var nounObject in Children)
            {
                nounObject.Value.nounChildren.Remove(noun);
            }
            allNounsInScene.Remove(noun);
        }

        public string FormattedRoomExamine()
        {

            if (Children.Count > 0)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("In the room there is:");
                foreach (var nounObject in Children)
                {
                    if (nounObject.Value.shortDescription != null)
                        sb.AppendLine($"-{nounObject.Key} : {nounObject.Value.shortDescription}");
                }
                return $"{roomName}\n{longDescription}\n{sb}";

            }
            return $"{roomName}\n{longDescription}";
        }
        public Room(string roomName, string shortDescription, string longDescription, Dictionary<RoomDirections, Func<string>> adjoiningRooms)
        {
            this.roomName = roomName;
            this.shortDescription = shortDescription;
            this.longDescription = longDescription;
            if (adjoiningRooms != null)
                this.adjoiningRooms = adjoiningRooms;
        }

        public NounObject GetNounObjectFromSceneUsingStem(Noun nounsearch)
        {
            
            foreach (var nounObject in Children)
            {
                if (nounObject.Value.myNoun.stem == nounsearch.stem)
                {
                    return nounObject.Value;
                }
            }
            return null;
            //return Children.FirstOrDefault(o => o.Value.myNoun.stem == nounsearch.stem);

        }
    }
    public enum RoomDirections
    {
        NORTH, SOUTH, EAST, WEST
    }
}