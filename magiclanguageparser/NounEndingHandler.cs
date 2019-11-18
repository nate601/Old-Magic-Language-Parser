using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

// ReSharper disable InconsistentNaming

namespace MagicLanguageParser
{
    public class StemGenerator
    {
        Random rng = new Random();

        public NounHolder GenerateAllStems(bool debugPrint)
        {
            NounHolder holder = new NounHolder();
            holder.englishNounKey = new Dictionary<string, Noun>();
            Debug.WriteLine("D: Nouns:");
            foreach (var englishNoun in holder.englishNouns)
            {
                var k = GenerateRoot(englishNoun);
                holder.englishNounKey.Add(englishNoun,k);
                //TODO:Remove in final
                Debug.WriteLine($">>>{englishNoun}  :  {k}");
            }
            return holder;
        }
        private readonly char[] letterList = {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
            'w', 'x', 'y', 'z'
        };
        private readonly char[] vowelList = {'a','e','i','o','u'};
        private List<char> consonantList = new List<char>();

        public StemGenerator()
        {
            foreach (var consonantLetter in letterList.Where(car => !vowelList.Contains(car)))
            {
                consonantList.Add(consonantLetter);
            }
        }

        public Noun GenerateRoot(string englishNoun)
        {
            int length = rng.Next(2, 6);
            StringBuilder sb = new StringBuilder();
            int consonantsInARow = 0;
            for (int i = 0; i < length; i++)
            {
                char curLetter;
                if (consonantsInARow >= 2)
                {
                    sb.Append(vowelList[rng.Next(0, vowelList.Length)]);
                    consonantsInARow = 0;
                    continue;
                }

                curLetter = letterList[rng.Next(0, letterList.Length)];
                if (consonantList.Contains(curLetter))
                {
                    consonantsInARow++;
                }
                sb.Append(curLetter);
                
            }
            sb.Append(vowelList[rng.Next(0, vowelList.Length)]);
            return new Noun(sb.ToString(), englishNoun);
        }
    }

    public class NounEndingHandler
    {
        private readonly char[] letterList = {
            'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v',
            'w', 'x', 'y', 'z'
        };
        private readonly char[] vowelList = { 'a', 'e', 'i', 'o', 'u' };
        private readonly List<char> consonantList = new List<char>();

        public string nominativeEnding;
        public string genitiveEnding;
        public string dativeEnding;
        public string accusativeEnding;
        public List<string> allPossibleEndings = new List<string>();
        readonly Random rng = new Random();
        public NounEndingHandler()
        {
            foreach (var car in letterList.Where(car => !vowelList.Contains(car)))
            {
                consonantList.Add(car);
            }
            //var rng = new Random();
            if(rng == null)
                rng = new Random();
            var usedEndings = new List<string>();
            Func<string[]> generatePossibleEndingTest = () =>
            {
                List<string> endings = new List<string>();
                for (int i = 0; i < 20; i++)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(vowelList[rng.Next(0, vowelList.Length)]);

                    sb.Append(consonantList[rng.Next(0, consonantList.Count)]);
                    endings.Add(sb.ToString());
                }
                return endings.ToArray();
            };
            allPossibleEndings.AddRange(generatePossibleEndingTest());
            Func<string> getRandomEnding = () => allPossibleEndings[rng.Next(0, allPossibleEndings.Count)];
            Func<string> getUniqueEnding = () =>
            {
                string currentEnding;
                do
                {
                    currentEnding = getRandomEnding();
                } while (usedEndings.Contains(currentEnding));
                usedEndings.Add(currentEnding);
                return currentEnding;

            };
            nominativeEnding = getUniqueEnding();
            genitiveEnding = getUniqueEnding();
            dativeEnding = getUniqueEnding();
            accusativeEnding = getUniqueEnding();



        }
        

    }

    public enum NounCases
    {
        Nominative,
        Genitive,
        Dative,
        Accusative
    }
}
