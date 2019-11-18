using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MagicLanguageParser
{
    public class AdjectiveHolder
    {
        public Dictionary<string, Adjective> adjectives;

        public AdjectiveHolder(StemGenerator generator)
        {
            adjectives = new Dictionary<string, Adjective>
            {
                //Colors
                {"red", new Adjective("color", "red", generator)},
                {"green", new Adjective("color", "green", generator)},
                {"blue", new Adjective("color", "blue", generator)},
                {"orange",new Adjective("color","orange",generator)},
                {"white",new Adjective("color","white",generator) },
                {"pink",new Adjective("color","pink",generator) },
                {"yellow", new Adjective("color","yellow",generator) },
                {"teal", new Adjective("color","teal",generator) },
                {"black",new Adjective("color","black",generator) },
                {"purple",new Adjective("color","purple",generator) }
            };
            Debug.WriteLine("Adjectives:");
            foreach (var adjective in adjectives)
            {
                Debug.WriteLine($">>>{adjective.Key} : {adjective.Value.word}");
            }
        }

        public bool GetIsAdjective(string input)
        {
            return adjectives.Any(adjective => adjective.Value.word == input);
        }

        public Adjective GetAdjective(string input)
        {
            foreach (var adjective in adjectives)
            {
                Debug.WriteLine($"Comparing {input} to {adjective.Value.word}");
                if (adjective.Value.word == input)
                {
                     return adjective.Value;
                }
            }
            return null;
        }
    }

    public class Adjective
    {

        public string propertyType;
        public string englishTerm;
        public string word;

        public Adjective(string propertyType, string englishTerm, StemGenerator generator)
        {
            this.propertyType = propertyType;
            this.englishTerm = englishTerm;
            word = generator.GenerateRoot(englishTerm).stem;

        }
    }
}