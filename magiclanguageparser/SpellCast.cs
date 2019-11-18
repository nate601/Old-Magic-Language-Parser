using System;
using System.Diagnostics;
using System.Dynamic;
using System.Security.Cryptography.X509Certificates;

namespace MagicLanguageParser
{
    internal class SpellCast
    {
        private Program program;

        public SpellCast(Program program)
        {
            this.program = program;
        }


        public struct SpellIncantation
        {
            public NounObject accusativeTarget;
            public Adjective adjForTarget;
            public VerbHolder.Verb verb;
            public NounObject genitiveObject;
            public bool genitiveObjIsCaster;
        }

        public string CastSpell(string input)
        {
            var incantation = ParseIncantation(input);
            if (incantation.verb == null)
            {
                Debug.WriteLine("Spell has no verb.");
                return "The magic fizzles and dies";
            }
            var successValue = incantation.verb.action.Invoke(incantation.accusativeTarget, incantation.adjForTarget,incantation.verb);

            if (successValue)
            {
                return incantation.verb.successMessage(incantation.accusativeTarget, incantation.adjForTarget);
            }
            else
            {
                return incantation.verb.failureMessage();
            }
        }


        public SpellIncantation ParseIncantation(string input)
        {
            var incantation = input.Split(' ');

            SpellIncantation incant = new SpellIncantation();

            foreach (var w in incantation)
            {
                Debug.WriteLine($"Parsing next word: {w}");
                

                //Is Noun
                if (program._nounHolder.GetIsNoun(w, program._nounEndingHandler))
                {
                    var getNoun = program._nounHolder.GetNounFromInput(w, program._nounEndingHandler);

                    //Cannot find noun meaning.
                    if (getNoun == null)
                    {
                        Debug.WriteLine("Cannot find noun meaning.");
                        continue;
                    }
                    switch (getNoun.Value.NounCase)
                    {
                        case NounCases.Genitive:
                            if (getNoun.Value.Word.englishMeaning == "Caster")
                            {
                                incant.genitiveObjIsCaster = true;
                            }
                            else
                            {
                                incant.genitiveObject =
                                    program._gameState.currentRoom().GetNounObjectFromSceneUsingStem(new Noun(getNoun.Value.Stem, null));
                            }
                            break;
                        case NounCases.Accusative:
                            //No genitive actions
                            if (!incant.genitiveObjIsCaster && incant.genitiveObject == null)
                            {
                                incant.accusativeTarget =
                                    program._gameState.currentRoom().GetNounObjectFromSceneUsingStem(new Noun(getNoun.Value.Stem,null));
                                break;
                            }
                            //If genitive value is the caster
                            if (incant.genitiveObjIsCaster)
                            {
                                foreach (var nounObject in program._gameState.inventory.objMap)
                                {
                                    if (nounObject.Value.myNoun.stem == getNoun.Value.Stem)
                                    {
                                        incant.accusativeTarget = nounObject.Value;
                                    } 
                                }
                                break;
                            }
                            //If genitive value is another object in the scene
                            if (incant.genitiveObject != null)
                            {
                                foreach (var nounObject in incant.genitiveObject.nounChildren)
                                {
                                    if (nounObject.Value.myNoun.stem == getNoun.Value.Stem)
                                    {
                                        incant.accusativeTarget = nounObject.Value;
                                    }
                                }
                            }
                            break;
                        default:
                            throw new NotImplementedException();
                    }
                    continue;
                }

                //Is verb
                if (program._verbHolder.GetIsVerb(w))
                {
                    incant.verb = program._verbHolder.GetVerb(w);
                    continue;
                }

                //Is adj
                if (program._adjectiveHolder.GetIsAdjective(w))
                {
                    incant.adjForTarget = program._adjectiveHolder.GetAdjective(w);
                    continue;
                }
                Debug.WriteLine($"Parsing word failed: {w} is not a noun, verb, or adjective...");
            }
            return incant;

        }

        //[Obsolete("Use normal cast spell instead.")]
        //public string CastSpell(string input, bool old)
        //{
        //    var incantation = input.Split(' ');

        //    NounObject accusativeTarget = null;
        //    bool isSelfGenitiveObject = false;
        //    Adjective adjForTarget = null;
        //    VerbHolder.Verb verb = null;
        //    NounObject genitiveObject = null;
        //    foreach (var s in incantation)
        //    {
        //        Debug.WriteLine($"Parsing :{s}");
        //        if (program._nounHolder.GetIsNoun(s, program._nounEndingHandler))
        //        {
        //            var nounFromInput = program._nounHolder.GetNounFromInput(s, program._nounEndingHandler);
        //            if (nounFromInput != null && nounFromInput.Value.NounCase == NounCases.Genitive)
        //            {
        //                var nounReturn = program._nounHolder.GetNounFromInput(s, program._nounEndingHandler);
        //                if (nounReturn != null)
        //                    genitiveObject = program._gameState.currentRoom().GetNounObjectFromSceneUsingStem(new Noun(nounReturn.Value.Stem, null));
        //                if (nounFromInput.Value.Word.stem == program._nounHolder.englishNounKey["self"].stem)
        //                {
        //                    isSelfGenitiveObject = true;
        //                }
        //            }
        //            var fromInput = program._nounHolder.GetNounFromInput(s, program._nounEndingHandler);
        //            if (fromInput != null && (fromInput.Value.NounCase == NounCases.Accusative && genitiveObject == null))
        //            {
        //                accusativeTarget = program._gameState.currentRoom().GetNounObjectFromSceneUsingStem(new Noun(program._nounHolder.GetNounFromInput(s, program._nounEndingHandler).Value.Stem, null));
        //            }
        //            if (program._nounHolder.GetNounFromInput(s, program._nounEndingHandler).Value.NounCase == NounCases.Accusative && genitiveObject == null && isSelfGenitiveObject == true)
        //            {
        //                foreach (var nounObject in program._gameState.inventory.objMap)
        //                {
        //                    if (nounObject.Value.myNoun.Accusative(program._nounEndingHandler) == s)
        //                    {
        //                        accusativeTarget = nounObject.Value;
        //                    }
        //                }
        //                Console.WriteLine("test");
        //            }
        //            if (program._nounHolder.GetNounFromInput(s, program._nounEndingHandler).Value.NounCase == NounCases.Accusative && genitiveObject != null)
        //            {
        //                foreach (var nounObject in genitiveObject.nounChildren)
        //                {
        //                    if (nounObject.Value.myNoun.Accusative(program._nounEndingHandler) == s)
        //                    {
        //                        accusativeTarget = nounObject.Value;
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            if (program._verbHolder.GetIsVerb(s))
        //            {
        //                verb = program._verbHolder.GetVerb(s);
        //            }
        //            else
        //            {
        //                if (program._adjectiveHolder.GetIsAdjective(s))
        //                {
        //                    adjForTarget = program._adjectiveHolder.GetAdjective(s);
        //                }
        //            }
        //        }
        //    }
        //    if (verb == null)
        //    {
        //        Debug.WriteLine("Missing verb");
        //        return "The spell fizzles and dies.";
        //    }
        //    if (accusativeTarget == null)
        //    {
        //        Debug.WriteLine("Missing accusative target");
        //        return verb.failureMessage();
        //    }

        //    // ReSharper disable once UseNullPropagation
        //    if (accusativeTarget != null && verb != null)
        //    {
        //        var success = verb.action(accusativeTarget, adjForTarget);
        //        if (success)
        //        {
        //            Debug.WriteLine("Cast success!");
        //            return verb.successMessage(accusativeTarget, adjForTarget);
        //        }
        //        else
        //        {
        //            Debug.WriteLine("Cast failure due to unknown circumstances.");
        //            return verb.failureMessage();
        //        }
        //    }
        //    if (adjForTarget == null)
        //    {
        //        Debug.WriteLine("Cast failure due to missing adjective");
        //        return verb.failureMessage();
        //    }
        //    Debug.WriteLine("This shouldn't happen... Error in spellcasting function.");
        //    return verb.failureMessage();
        //}
    }
}