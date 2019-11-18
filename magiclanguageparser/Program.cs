using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Net.Configuration;
using System.Text;

namespace MagicLanguageParser
{
    internal class Program
    {
        public static Program Instance;

        public CmdRegistration CmdRegistration { get; }

        public SpellCast SpellCast { get; }

        private static void Main(string[] args) => new Program().Start(args);

        public NounEndingHandler _nounEndingHandler;
        private StemGenerator _stemGenerator;
        public NounHolder _nounHolder;
        public GameState _gameState;
        public VerbHolder _verbHolder;
        public AdjectiveHolder _adjectiveHolder;

        public Program()
        {
            CmdRegistration = new CmdRegistration(this);
            SpellCast = new SpellCast(this);
        }

        public void Start(string[] args)
        {
            Instance = this;
            _nounEndingHandler = new NounEndingHandler();
            _stemGenerator = new StemGenerator();
            _nounHolder = _stemGenerator.GenerateAllStems(true);
            _verbHolder = new VerbHolder(_stemGenerator);
            _adjectiveHolder = new AdjectiveHolder(_stemGenerator);
            NounPrefabs.Init(_nounHolder, _verbHolder, _adjectiveHolder, _nounEndingHandler);
            _gameState = new GameState();

            CmdRegistration.RegisterCommands();

            if (args.Length != 0)
                return;
            MainGameLoop();

        }

        private void MainGameLoop()
        {
            while (true)
            {
                foreach (var nounObject in _gameState.currentRoom().allNounsInScene)
                {
                    nounObject.Value.onUpdate?.Invoke(nounObject.Value);
                }
                Console.Write(">");
                var input = Console.ReadLine();    
                var k = ParseLine(input);
                if (k == null)
                    continue;
                Console.Write(k);
                if (!k.EndsWith(Environment.NewLine))
                {
                    Console.WriteLine();
                }
            }
        }

        private string ParseLine(string readLine)
        {
            if (CmdRegistration.basicCommandMap.ContainsKey(readLine.ToLower()))
            {
                return CmdRegistration.basicCommandMap[readLine.ToLower()]();
            }
            if (CmdRegistration.complexCommandMap.Any(func => readLine.ToLower().Split(' ')[0] == func.Key) && readLine.ToLower().Split(' ').Length > 1)
            {
                return CmdRegistration.complexCommandMap[readLine.ToLower().Split(' ')[0]](readLine.ToLower().Substring(readLine.ToLower().Split(' ')[0].Length + 1));
            }


            #region debugging commands

            //Debug data
            if (readLine.ToLower().StartsWith("d:"))
            {
                switch (readLine.ToLower())
                {
                    case "d:connectroom":
                        Console.WriteLine("What room should I connect to this one?");
                        Console.Write(">>");
                        string whatRoomShouldIConnectToThisOne = Console.ReadLine();
                        Console.WriteLine("What direction should I connect it with?");
                        Console.Write(">>");
                        bool isWork = false;
                        RoomDirections direc;
                        isWork = Enum.TryParse(Console.ReadLine().ToUpper(), false, out direc);
                        if (isWork)
                        {
                            return "Connected rooms";
                        }
                        while (!isWork)
                        {
                            Console.WriteLine("I'm sorry, I didn't understand that, please enter \"north\" \"south\" \"east\" or \"west\".");
                            isWork = Enum.TryParse(Console.ReadLine(), false, out direc);

                        }
                        if (_gameState.currentRoom().adjoiningRooms.ContainsKey(direc))
                        {
                            return "Already have a room connected in that direction!";
                        }
                        _gameState.currentRoom().adjoiningRooms.Add(direc, () => whatRoomShouldIConnectToThisOne);
                        return "Connected rooms";
                    case "d:empower":
                        _gameState.isEmpowered = true;
                        return "You are empowered now.";
                    case "d:cast":
                        Console.Write(">>");
                        return SpellCast.CastSpell(Console.ReadLine()).ToString();


                    case "d:addnoun":
                        Console.WriteLine("What should I call this noun?");
                        Console.Write(">>");
                        string nounName = Console.ReadLine();
                        Console.WriteLine("What is the prefab of this noun called?");
                        Console.Write(">>");
                        string nounObjPossible = Console.ReadLine();
                        if (String.IsNullOrEmpty(nounObjPossible) || string.IsNullOrEmpty(nounName) | !NounPrefabs.prefabs.ContainsKey(nounObjPossible.ToLower()))
                            return "Not a valid noun.";
                        _gameState.currentRoom().Children.Add(nounName, NounPrefabs.prefabs[nounObjPossible.ToLower()]);
                        return "added";

                    case "d:isnoun":
                        Console.Write(">>");
                        return _nounHolder.GetIsNoun(Console.ReadLine(), _nounEndingHandler).ToString();

                    case "d:generateallnouns":
                        _nounHolder = _stemGenerator.GenerateAllStems(true);
                        return "";

                    case "d:parsenoun":
                        Console.Write(">>");
                        var parseInput = Console.ReadLine();
                        var returnedNounInformation = _nounHolder.GetNounFromInput(parseInput, _nounEndingHandler);
                        if (!_nounHolder.GetIsNoun(parseInput, _nounEndingHandler))
                            return "That's not a Noun!";
                        if (_nounHolder.EnglishDefinitionFromNoun(returnedNounInformation.Value.Word) == null)
                        {
                            return
                             $"Stem : {returnedNounInformation.Value.Stem}\nCase : {returnedNounInformation.Value.NounCase}\nEnglish Definition : No definition found";
                        }
                        return
                            $"Stem : {returnedNounInformation.Value.Stem}\nCase : {returnedNounInformation.Value.NounCase}\nEnglish Definition : {_nounHolder.EnglishDefinitionFromNoun(returnedNounInformation.Value.Word)}";
                    case "d:regenerateendings":
                        _nounEndingHandler = new NounEndingHandler();
                        return "Regeneration Completed!";

                    case "d:listendings":
                        return
                            $"Nominative : {_nounEndingHandler.nominativeEnding}\nGenitive : {_nounEndingHandler.genitiveEnding}\nDative : {_nounEndingHandler.dativeEnding}\nAccusative : {_nounEndingHandler.accusativeEnding}";
                    default:
                        return
                            "No matching command\n Try:\nd:declineroot\nd:regenerateendings\nd:findtypeandstemfromword\nd:generatestem\nd:generateword";
                }

            }
            #endregion debugging commands

            return TextUtils.Pick("Sorry, I missed that.", "I didn't understand that.", "That doesn't make sense!", "Huh?" );
        }

        public void RemoveObject(NounObject input)
        {
            bool isinInv;
            RemoveObject(FindObject(input,out isinInv,true).Value.Key);   
        }
        public void RemoveObject(string input)
        {
            bool isInInv;
            var x = FindObject(input, out isInInv, true);
            if (isInInv)
            {
                Program.Instance._gameState.inventory.RemoveItem(input);
            }
            else
            {
                Program.Instance._gameState.currentRoom().RemoveNoun(input);
            }
        }

        public KeyValuePair<string, NounObject>? FindObject(NounObject input, out bool isInInventory,
            bool includeInventory = false)
        {
            if (includeInventory)
            {
                foreach (var nounObject in _gameState.inventory.objMap)
                {
                    if (nounObject.Value.myKey == input.myKey)
                    {
                        isInInventory = true;
                        return nounObject;
                    }
                }
            }
            else
            {
                isInInventory = false;
                foreach (var nounObject in _gameState.currentRoom().allNounsInScene)
                {
                    if (nounObject.Value.myKey== input.myKey)
                    {
                        return nounObject;
                    }
                }
               

            }
            isInInventory = false;

            return null;
            
        } 
        public KeyValuePair<string, NounObject>? FindObject(string input, out bool isInInventory, bool includeInventory = false)
        {
            if (includeInventory)
            {
                foreach (var nounObject in _gameState.inventory.objMap)
                {
                    if (nounObject.Key.ToLower() == input.ToLower())
                    {
                        isInInventory = true;
                        return nounObject;
                    }
                }
            }
            isInInventory = false;
            foreach (var nounObject in _gameState.currentRoom().allNounsInScene)
            {
                if (nounObject.Key.ToLower() == input.ToLower())
                {
                    return nounObject;
                }
            }
            return null;
        }

        public KeyValuePair<string, NounObject>? FindObjectInInventory(string input)
        {
            bool isInInventory;
            var obj = FindObject(input,out isInInventory, true);
            return isInInventory ? obj : null;
        } 

        public KeyValuePair<string, NounObject>? FindObject(string input, bool includeInventory = false
            )
        {
            bool trash;
            return FindObject(input, out trash, includeInventory);
        }
    }
}