using System;
using System.Collections.Generic;

namespace MagicLanguageParser
{
    [Serializable]
    public class Noun
    {
        public string stem;
        public string englishMeaning;


        public Noun(string stem, string englMen)
        {
            this.stem = stem;
            englishMeaning = englMen;
            //test
        }
        

        public string Nominative(NounEndingHandler handler)
        {
            return stem + handler.nominativeEnding;
        }
        public string Genitive(NounEndingHandler handler)
        {
            return stem + handler.genitiveEnding;
        }
        public string Dative(NounEndingHandler handler)
        {
            return stem + handler.dativeEnding;
        }
        public string Accusative(NounEndingHandler handler)
        {
            return stem + handler.accusativeEnding;
        }

        public override string ToString()
        {
            return stem;
        }

        public string GetFormattedDisplay(NounEndingHandler handler)
        {
            return
                $"Stem : {stem}\nNominative : {Nominative(handler)}\nGenitive : {Genitive(handler)}\nDative : {Dative(handler)}\nAccusative : {Accusative(handler)}";
        }

        #region Overrides of Object

        public override bool Equals(object obj)
        {
            return obj != null && obj.ToString() == ToString();
        }

        #endregion
    }
}