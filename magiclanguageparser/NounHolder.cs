using System.Collections.Generic;
using System.Linq;

namespace MagicLanguageParser
{
    public class NounHolder
    {
        public string[] englishNouns = new[] {"Caster","Self", "Opponent", "Apple","Table","Key","Paper"};
        public Dictionary<string, Noun> englishNounKey;

       

        public string EnglishDefinitionFromNoun(Noun noun)
        {
            return englishNounKey.Where(word1 => Equals(word1.Value, noun)).Select(word1 => word1.Key).FirstOrDefault();
        }
        public bool GetIsNoun(string input, NounEndingHandler handler)
        {
            return input.EndsWith(handler.nominativeEnding) | input.EndsWith(handler.genitiveEnding) |
                   input.EndsWith(handler.dativeEnding) | input.EndsWith(handler.accusativeEnding) ;
        }
        public struct NounReturn
        {
            public NounCases NounCase;
            public string Stem;
            public Noun Word;

            public NounReturn(NounCases nounCase, string stem, Noun word)
            {
                NounCase = nounCase;
                Stem = stem;
                Word = word;
            }
        }
        public NounReturn? GetNounFromInput(string input, NounEndingHandler handler)
        {
           
            if (input.EndsWith(handler.nominativeEnding))
            {
                return new NounReturn(NounCases.Nominative, input.Substring(0, input.Length - handler.nominativeEnding.Length),new Noun(input.Substring(0, input.Length - handler.nominativeEnding.Length), "missingno"));
              
            }
            if (input.EndsWith(handler.genitiveEnding))
            {
                return new NounReturn(NounCases.Genitive, input.Substring(0, input.Length - handler.nominativeEnding.Length), new Noun(input.Substring(0, input.Length - handler.nominativeEnding.Length), "missingno"));

            }
            if (input.EndsWith(handler.dativeEnding))
            {
                return new NounReturn(NounCases.Dative, input.Substring(0, input.Length - handler.nominativeEnding.Length), new Noun(input.Substring(0, input.Length - handler.nominativeEnding.Length), "missingno"));

            }
            if (input.EndsWith(handler.accusativeEnding))
            {
                return new NounReturn(NounCases.Accusative, input.Substring(0, input.Length - handler.nominativeEnding.Length), new Noun(input.Substring(0, input.Length - handler.nominativeEnding.Length), "missingno"));

            }
            return null;
        }
    }
}