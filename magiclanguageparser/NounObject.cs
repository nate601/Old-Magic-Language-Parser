using System;
using System.Collections.Generic;
using System.Text;

namespace MagicLanguageParser
{
    [Serializable]
    public class NounObject
    {
        public static List<NounObject> nounObjects = new List<NounObject>(); 
        public Noun myNoun;

        public Dictionary<string, Func<NounObject,string>> senses;

        public bool canHoldItems;

        public Dictionary<string, string> properties = new Dictionary<string, string>();
        public Dictionary<string,NounObject> nounChildren = new Dictionary<string, NounObject>();
        public string shortDescription;

        public Func<NounObject,string> LongDescription;
        public bool canBePickedUp;

        public Action<NounObject> onUpdate;

        //public Dictionary<string, Action<string>> overrideActions;   
        public Func<NounObject,bool, string> UseAction;
        public string myKey;

        public NounObject SetOnUpdate(Action<NounObject> update)
        {
            onUpdate += update;
            return this;
        }

        public NounObject(Noun myNoun, string shortDescription, Func<NounObject, string> sense,
            bool canBePickedUp = true, bool canHoldItems = false)
        {
            this.myNoun = myNoun;
            this.shortDescription = shortDescription;
            this.senses = new Dictionary<string, Func<NounObject, string>>() { {"sight",sense} };
            this.canBePickedUp = canBePickedUp;
            this.canHoldItems = canHoldItems;
            nounObjects.Add(this);
        }

        public NounObject(Noun myNoun, string shortDescription, Dictionary<string, Func<NounObject,string>> senses, bool canBePickedUp = true, bool canHoldItems = false)
        {
            this.myNoun = myNoun;
            this.shortDescription = shortDescription;
            this.senses = senses;
            this.canBePickedUp = canBePickedUp;
            this.canHoldItems = canHoldItems;
            nounObjects.Add(this);
        }

        public NounObject()
        {
        }

        public string FormattedChildren()
        {
            StringBuilder sb = new StringBuilder();
            foreach (var nounObject in nounChildren)
            {
                sb.AppendLine($"-{nounObject.Key} : {nounObject.Value.shortDescription}");
            }
            return sb.ToString();
        }
        public string FormattedExamine()
        {
            
            return $"{myNoun.englishMeaning} : {shortDescription}";

        }



        public NounObject SetUseAction(Func<NounObject, bool, string> act)
        {
            UseAction += act;
            return this;
        }
        public NounObject AddProperty(string key, string property)
        {
            
            properties.Add(key,property);
            return this;
        }
    }
}