using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MagicLanguageParser
{
    public class VerbHolder
    {
        public class Verb
        {
            public string verb;
            public readonly string englishTerm;

            public Func<NounObject, Adjective,Verb, bool> action;
            public Func<NounObject, Adjective, string> successMessage;
            public Func<String> failureMessage;
            
            public Dictionary<string, string> storedValues = new Dictionary<string, string>(); 

            public Verb(string englishTerm, Func<NounObject, Adjective,Verb, bool> action,
                Func<NounObject, Adjective, string> successMessage, Func<string> failureMessage)
            {
                this.englishTerm = englishTerm;
                this.action = action;
                this.successMessage = successMessage;
                if (failureMessage != null)
                {
                    this.failureMessage = failureMessage;
                }
                else
                {
                    this.failureMessage = () => "The magic fizzles and dies.";
                }
            }
        }

        public Dictionary<string, Verb> verbs;
        public string verbEnding;
        public StemGenerator generator;

        public Verb GetVerb(string stem)
        {
            foreach (var verb in verbs)
            {
                if (verb.Value.verb == stem)
                {
                    return verb.Value;
                }
            }
            //TODO:Remove
            Console.WriteLine("ERROR: Missing verb!");
            return null;

        }
        public VerbHolder(StemGenerator generator)
        {
            verbs = new Dictionary<string, Verb>
            {
                {
                    "Change color", new Verb("Change color",
                        (noun, adjective, verb) =>
                        {
                            if (adjective == null)
                                return false;
                            if (noun == null)
                                return false;
                            if (adjective.propertyType == "color" && noun.properties.ContainsKey("color"))
                            {
                                noun.properties["color"] = adjective.englishTerm;
                                return true;
                            }
                            return false;

                        }, (noun,adjective)=> $"Poof! The {noun.myNoun.englishMeaning.ToLower( )} changes color to {adjective.englishTerm}!",()=>"Suddenly vibrant colors splash around the room, but in the end nothing happens."
                        )
                },
                
                
            };
            verbEnding = "ra";
            Debug.WriteLine("Verbs:");

            foreach (var verb in verbs)
            {
                verb.Value.verb = generator.GenerateRoot(verb.Value.englishTerm).stem + verbEnding;
                Debug.WriteLine($">>>{verb.Value.englishTerm} : {verb.Value.verb}");
            }
        }

        public bool GetIsVerb(string input)
        {
            foreach (var verb1 in verbs)
            {
                if (verb1.Value.verb == input)
                    return true;
            }
            return false;


            //foreach (var verb1 in verbs)
            //{
            //    if (verb1.Key.StartsWith(verb)) return true;
            //}
            //return false;
        }



    }
}