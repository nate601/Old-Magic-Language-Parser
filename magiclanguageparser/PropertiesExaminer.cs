using System;
using System.Collections.Generic;

namespace MagicLanguageParser
{
    public static class PropertiesExaminer
    {
        public static Dictionary<string,Func<string,NounObject,string>>  propertiesMap = new Dictionary<string, Func<string,NounObject, string>>()
        {
            {"color",(s,x)=>$"{TextUtils.Pick($"The {s.ToLower()}", "It")} is {x.properties["color"]}." },
            {"inscription",(s,x)=>$"The {s.ToLower()} has \"{x.properties["inscription"]}\" {TextUtils.Pick("inscribed","written","burned")} along the side." },
            {"material",(s,x) => $" {TextUtils.Pick($"The {s.ToLower()}","It")} is made of {x.properties["material"]}."},
            {"switchStatus",(s,x)=>$"{TextUtils.Pick($"The {s.ToLower()}","It")} is in the {x.properties["switchStatus"]} position." },
            {"writing", (s,x)=>$"It has \"{x.properties["writing"]}\" written on it." }
        };
    }
}