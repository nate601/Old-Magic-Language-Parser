using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace MagicLanguageParser
{
    public static partial class NounPrefabs
    {

        public static Dictionary<string, NounObject> prefabs;

        public static NounObject GetPrefab(string prefabName)
        {
            foreach (var nounObject in prefabs)
            {
                if (nounObject.Key.ToLower() == prefabName.ToLower())
                {
                    return TextUtils.DeepCopy(nounObject.Value);
                }
            }
            return null;
        }


        public static void Init(NounHolder nounHolder, VerbHolder verbHolder, AdjectiveHolder adjectiveHolder,
            NounEndingHandler handler)
        {


            prefabs = new Dictionary<string, NounObject>();

            GrantPuzzlePrefabs();




            Dictionary<string, Func<NounObject, string>> tmp = new Dictionary<string, Func<NounObject, string>>();
            //{
            //    {
            //        "sight",
            //        (x) => $"The table is {x.properties["color"]}.  On the table there is:\n{x.FormattedChildren()}"
            //    }
            //};
            prefabs.Add("table", new NounObject(nounHolder.englishNounKey["Table"], "A table.", tmp, false, true));
            prefabs["table"].properties.Add("color", "white");

            //tmp = new Dictionary<string, Func<NounObject, string>>
            //{
            //    {"sight", (x) => $"The apple is {x.properties["color"]}."}
            //};
            prefabs.Add("apple", new NounObject(nounHolder.englishNounKey["Apple"], "An apple.", tmp));
            prefabs["apple"].properties.Add("color", "red");

            prefabs.Add("lever",
                new NounObject(new Noun(null, "lever"), "A wooden vertical lever.",
                    new Dictionary<string, Func<NounObject, string>>()
                    {
                        {"sight", (x) => "There is a plus above the lever and a minus below the lever."}
                    }, false)
                    .AddProperty("switchStatus", "up")
                    .SetUseAction((x, isInInventory) =>
                    {
                        if (isInInventory)
                            return "How did a lever get in your inventory lol";
                        if (!x.properties.ContainsKey("switchStatus"))
                            Debug.WriteLine("Lever did not have switchStatus property");
                        if (x.properties["switchStatus"] == "up")
                        {
                            x.properties["switchStatus"] = "down";
                            return "You flip the switch into the down position.";
                        }
                        else
                        {
                            x.properties["switchStatus"] = "up";
                            return "You flip the switch into the up position.";
                        }
                    })
                );

            #region Wands



            prefabs.Add("OrangeColorChangingWand",
                new Wand(new Noun(null, "OrangeColorChangingWand"), "An orange wand").AddProperty("inscription",
                    $"{nounHolder.englishNounKey["Apple"].Accusative(handler)} {adjectiveHolder.adjectives["orange"].word} {verbHolder.verbs["Change color"].verb}")
                    .AddProperty("color", "orange"));
            prefabs.Add("BlueColorChangingWand",
                new Wand(new Noun(null, "BlueColorChangingWand"), "A blue wand").AddProperty("inscription",
                    $"{nounHolder.englishNounKey["Apple"].Accusative(handler)} {adjectiveHolder.adjectives["blue"].word} {verbHolder.verbs["Change color"].verb}")
                    .AddProperty("color", "blue"));
            prefabs.Add("BlankWand",
                new Wand(new Noun(null, "BlankWand"), "A plain white wand").AddProperty("color", "white"));

            #endregion


            prefabs.Add("blankPaper",new NounObject(new Noun(nounHolder.englishNounKey["Paper"].stem,"Paper"),"A piece of paper.",(x)=>"A piece of paper.")
                .AddProperty("color","white")
                .AddProperty("canBeWritten",true.ToString())
                .SetUseAction
                ((x, isInInv) => {
                    if (!isInInv)
                        return "The paper has to be in your inventory!";
                    if (Program.Instance._gameState.isEmpowered == false)
                        return "You look at the paper.  Seems pretty ordinary.  Not very interesting though...";
                    if (x.properties.ContainsKey("writing"))
                    {
                        Console.WriteLine(TextUtils.Pick("You let magic flow from your fingertips to the paper."));
                        var k = Program.Instance.SpellCast.CastSpell(x.properties["writing"]);
                        Program.Instance.RemoveObject(x);
                        Console.WriteLine(k);
                        return "The paper disappears!";
                    }
                    return "Yup.  That's a sheet of paper.  Seems like you could write on it or something.";
                }
                ));
            prefabs.Add("pen",new NounObject(new Noun(null,"Pen"),"A pen",(x)=>"A pen, used for writing.").AddProperty("color","black").AddProperty("writing",null).SetUseAction(
                (x, isInInv) =>
                {
                    
                    if (!isInInv)
                        return "The pen has to be in your inventory to write with it!";
                    Console.WriteLine("What object will you write on?");
                    Console.Write(">>");
                    var objToWriteOn = Program.Instance.FindObject(Console.ReadLine(), true);
                    if (objToWriteOn != null && objToWriteOn.Value.Value == null)
                    {
                        return "I can't find that!";
                    }
                    if (objToWriteOn == null)
                    {
                        return "I can't find that";
                    }
                    if (objToWriteOn.Value.Value.properties.ContainsKey("canBeWritten") &&
                        objToWriteOn.Value.Value.properties["canBeWritten"] == true.ToString())
                    {
                        Console.WriteLine("What will you write down?");
                        Console.Write(">>");
                        var writing = Console.ReadLine();
                        objToWriteOn.Value.Value.properties["writing"] = writing;
                        return $"You write \"{writing}\" on the {objToWriteOn.Value.Key.ToLower()}";
                    }
                    return "You can't write on that!";
                }));
            #region Books

            prefabs.Add("empowermentBook",
                new Book(new Noun(null, "Old Journal"), "A book containing arcane secrets.", new List<string>()
                {
                    "The language of magic is very simple.  Nouns have four cases.  The first case is the nominative case.  This is used for the subject of the sentence.  In simple spells the nominative case doesn't come up much.",
                    "Another case is the accusative case.  This is the object that has the action acted upon.",
                    "Finally there is the topic of posession.  If the object is merely sitting in the room, then I can use the spell as usual.  But if the object is sitting on a table, in a chest, or even on my person, then I must qualify the spell with a noun in the genitive case, followed by the noun in the accusative case.",
                    $"Verbs do not conjugate depending on the subject, instead the subject is determined by the case of the noun.  I have learned the word \"{Program.Instance._verbHolder.verbs["Change color"].verb}\".  This word allows me to change the color of a noun in the accusative.  The spells are comprised of the accusative noun that I want to change the color of, \"{Program.Instance._verbHolder.verbs["Change color"].verb}\", and the color in the magical language in any order.",
                    "There are many different ways to activate spells, most involve inscribing the spell onto a magical object.",
                    "[The rest of the pages are burnt.]"
                })
                {
                    senses =
                        new Dictionary<string, Func<NounObject, string>>()
                        {
                            {"sight", (x) => $"The book is a short journal."}
                        },
                    onReadAction = (x) =>
                    {
                        if
                            (
                            Program.Instance._gameState.isEmpowered == false)
                        {
                            Console
                                .WriteLine
                                ("You feel energy rushing through you!  You can now use magical items!");
                            Program
                                .Instance
                                ._gameState
                                .isEmpowered
                                =
                                true;
                        }
                    }
                });



            #endregion Books


            prefabs.Add("debug3_pedestal",
                new NounObject(new Noun(null, "PinkPedestal"), "A pink pedestal",
                    new Dictionary<string, Func<NounObject, string>>()
                    {
                        {
                            "sight",
                            (x) =>
                                $"Written at the bottom of the pedestal, you see the words \"Pink equals {Program.Instance._adjectiveHolder.adjectives["pink"].word}\""
                        }
                    }, false, true).AddProperty("color", "pink").SetOnUpdate(
                        (x) =>
                        {

                            if (x.nounChildren.Count != 0)
                            {
                                foreach (var nounObject in x.nounChildren)
                                {
                                    if (nounObject.Value.myNoun.stem == prefabs["apple"].myNoun.stem)
                                        if (nounObject.Value.properties["color"] == "pink")
                                            if (Program.Instance._gameState.currentRoom().roomKey == "Debug3")
                                                if (
                                                    !Program.Instance._gameState.currentRoom()
                                                        .adjoiningRooms.ContainsKey(RoomDirections.SOUTH))
                                                {
                                                    Console.WriteLine("The exit door opens!");
                                                    Program.Instance._gameState.roomHolder["Debug3"].adjoiningRooms.Add(
                                                        RoomDirections.SOUTH, () => "Debug4");
                                                }
                                }
                            }
                        }));




            tmp = new Dictionary<string, Func<NounObject, string>>()
            {
                //TODO: Make more adventure-y
                {"sight", (x) => $"A machine used for inscription."}
            };
            prefabs.Add("inscriptionMachine",
                new NounObject(new Noun(null, "inscriptionMachine"),
                    "A machine used to inscribe new inscriptions to a wand.", tmp, false, false)
                {
                    UseAction = (x, isInInv) =>
                    {
                        if (isInInv)
                            Debug.WriteLine("Wow the inscription table is in the inventory, how'd that happen lol.");
                        Console.WriteLine("What wand will you inscribe?");
                        Console.Write(">>");
                        bool wandIsInInv;
                        var obj = Program.Instance.FindObject(Console.ReadLine(), out wandIsInInv, true);
                        if (obj == null)
                            return "I can't find that wand!";
                        if (!(obj.Value.Value is Wand))
                            return "You can only inscribe wands!";
                        if (wandIsInInv)
                        {
                            Console.WriteLine("What should the new inscription say?");
                            Console.Write(">>");
                            var inscription = Console.ReadLine();
                            obj.Value.Value.properties["inscription"] = inscription;
                            return $"You inscribe \"{inscription}\" onto the wand.";
                        }
                        else
                        {
                            return "The wand has to be in your inventory!";

                        }
                    }
                })
                ;
            //   prefabs.Add();

        }
    }

    public class Wand : NounObject
    {
        public Wand(Noun myNoun, string shortDescription)
        {
            this.canBePickedUp = true;
            this.myNoun = myNoun;
            this.shortDescription = shortDescription;
            this.senses = new Dictionary<string, Func<NounObject, string>>();
            this.UseAction = (x, isInInventory) =>
            {
                if (!isInInventory) return "The wand must be in your inventory to use it!";
                if (Program.Instance._gameState.isEmpowered == false)
                    return "You wave the wand around, but nothing happens!";
                if (x.properties.ContainsKey("inscription"))
                {
                    return Program.Instance.SpellCast.CastSpell(x.properties["inscription"]);
                }
                return null;

            };
        }
    }

    public class Book : NounObject
    {
        public List<string> pages;
        public Action<Book> onReadAction;
        
        public Book(Noun myNoun, string shortDescription, List<string> pages )
        {
            this.canBePickedUp = true;
            this.myNoun = myNoun;
            this.shortDescription = shortDescription;
            this.pages = pages;
        }
    }

    public interface IUnlockable
    {
        Func<Key,string> Unlock { get; set; }
        bool IsUnlocked { get; set; }
    }
    public class Door : NounObject, IUnlockable
    {
        public RoomDirections newDirection;
        public Func<string> newRoom;

        public Door(Noun myNoun, string shortDescription, Dictionary<string, Func<NounObject, string>> senses, RoomDirections direction, Func<string> newRoomF, bool canBePickedUp = true, bool canHoldItems = false) : base(myNoun, shortDescription, senses, canBePickedUp, canHoldItems)
        {
            newDirection = direction;
            newRoom = newRoomF;
        }

        public Func<Key,string> Unlock { get
        {
            return (x) =>
            {
                if (x.properties.ContainsKey("color") && properties.ContainsKey("color"))
                {
                    if (x.properties["color"] == properties["color"])
                    {
                        if (Program.Instance._gameState.currentRoom().adjoiningRooms.ContainsKey(newDirection))
                        {
                            Debug.WriteLine("Direction already filled.");
                        }
                        else
                        {
                            Program.Instance._gameState.currentRoom().adjoiningRooms.Add(newDirection, newRoom);
                            this.IsUnlocked = true;
                            Program.Instance._gameState.currentRoom().RemoveNoun(myKey);
                            return $"You unlock it.";
                        }
                    }
                    return TextUtils.Pick("The key doesn't fit in that lock!","It doesn't fit!","This isn't the right key for that lock!");
                }
                else
                {
                    Debug.WriteLine("One or more of the unlock components do not have a color!");
                    return "";
                }
            };
        } set {} }
        public bool IsUnlocked { get; set; }
    }

    public class Key : NounObject
    {
        public int isKey;
        //public string KeyId;
        public Key(Noun myNoun, string shortDescription, Dictionary<string, Func<NounObject, string>> senses, bool canBePickedUp = true, bool canHoldItems = false,int isKey = 99) : base(myNoun, shortDescription, senses, canBePickedUp, canHoldItems)
        {
            this.isKey = isKey;
            this.UseAction = (x, isInInv) =>
            {
                if (!isInInv)
                    return "The key has to be in your inventory!";
                Console.WriteLine("What will you unlock?");
                Console.Write(">>");
                var door = Program.Instance.FindObject(Console.ReadLine())?.Value;
                if (door != null && door is IUnlockable)
                {
                    return ((IUnlockable) door).Unlock(this);
                }
                else
                {
                    return "I can't find that door.";

                }
            };
        }
    }


}