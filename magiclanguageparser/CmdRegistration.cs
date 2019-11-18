using System;
using System.Collections.Generic;
using System.Text;



namespace MagicLanguageParser
{
    internal class CmdRegistration
    {
        private Program program;
        public Dictionary<string, Func<string>> basicCommandMap = new Dictionary<string, Func<string>>();
        public Dictionary<string, Func<string, string>> complexCommandMap = new Dictionary<string, Func<string, string>>();

        public CmdRegistration(Program program)
        {
            this.program = program;
        }

        public void RegisterCommands()
        {

            #region Movement Directions

            basicCommandMap.Add("s", () =>
            {
                if (program._gameState.currentRoom().adjoiningRooms.ContainsKey(RoomDirections.SOUTH))
                {
                    if (
                        !program._gameState.roomHolder.keyRoomMap.ContainsKey(program._gameState.currentRoom().adjoiningRooms[RoomDirections.SOUTH]()))
                    {
                        program._gameState.ChangeRoom("Debug1");
                    }
                    program._gameState.ChangeRoom(program._gameState.currentRoom().adjoiningRooms[RoomDirections.SOUTH]());
                    return "You move south.";
                }
                else
                {
                    return "You can't go that way!";
                }
            });
            basicCommandMap.Add("n", () =>
            {
                if (program._gameState.currentRoom().adjoiningRooms.ContainsKey(RoomDirections.NORTH))
                {
                    if (
                        !program._gameState.roomHolder.keyRoomMap.ContainsKey(program._gameState.currentRoom().adjoiningRooms[RoomDirections.NORTH]()))
                    {
                        program._gameState.ChangeRoom("Debug1");
                    }
                    program._gameState.ChangeRoom(program._gameState.currentRoom().adjoiningRooms[RoomDirections.NORTH]());
                    return "You move north.";
                }
                else
                {
                    return "You can't go that way!";
                }
            });
            basicCommandMap.Add("w", () =>
            {
                if (program._gameState.currentRoom().adjoiningRooms.ContainsKey(RoomDirections.WEST))
                {
                    if (
                        !program._gameState.roomHolder.keyRoomMap.ContainsKey(program._gameState.currentRoom().adjoiningRooms[RoomDirections.WEST]()))
                    {
                        program._gameState.ChangeRoom("Debug1");
                    }
                    program._gameState.ChangeRoom(program._gameState.currentRoom().adjoiningRooms[RoomDirections.WEST]());
                    return "You move west.";
                }
                else
                {
                    return "You can't go that way!";
                }
            });
            basicCommandMap.Add("e", () =>
            {
                if (program._gameState.currentRoom().adjoiningRooms.ContainsKey(RoomDirections.EAST))
                {
                    if (
                        !program._gameState.roomHolder.keyRoomMap.ContainsKey(program._gameState.currentRoom().adjoiningRooms[RoomDirections.EAST]()))
                    {
                        program._gameState.ChangeRoom("Debug1");
                    }
                    program._gameState.ChangeRoom(program._gameState.currentRoom().adjoiningRooms[RoomDirections.EAST]());
                    return "You move east.";
                }
                else
                {
                    return "You can't go that way!";
                }
            });

            #endregion Movement Directions

            basicCommandMap.Add("examine", () => program._gameState.currentRoom().FormattedRoomExamine());
            basicCommandMap.Add("x", basicCommandMap["examine"]);
            basicCommandMap.Add("l", basicCommandMap["examine"]);
            basicCommandMap.Add("look", basicCommandMap["examine"]);
            basicCommandMap.Add("cls", (() =>
            {
                Console.Clear();
                return null;
            }));
            basicCommandMap.Add("clear", basicCommandMap["cls"]);
            basicCommandMap.Add("inventory", () =>
            {
                StringBuilder sb = new StringBuilder();
                if (program._gameState.inventory.objMap.Count > 0)
                {
                    sb.AppendLine("You are carrying:");
                    foreach (var nounObject in program._gameState.inventory.objMap)
                    {
                        sb.AppendLine($"-{nounObject.Key} : {nounObject.Value.shortDescription}");
                    }
                }
                else
                {
                    sb.AppendLine("You aren't carrying anything!");
                }
                return sb.ToString();

            });
            basicCommandMap.Add("i", basicCommandMap["inventory"]);
            basicCommandMap.Add("quit", () =>
            {
                Environment.Exit(0);
                return null;
            });
            basicCommandMap.Add("q",basicCommandMap["quit"]);
            complexCommandMap.Add("read", (s) =>
            {
                var obj = program.FindObject(s, true);
                if (((Book)obj?.Value)?.pages != null)
                {


                    Book book = (Book)obj.Value.Value;
                    int currentPage = 0;
                    while (true)
                    {
                        Console.Write("\n");
                        if (book.pages.Count > currentPage)
                        {
                            Console.Write(book.pages[currentPage] + "\n");
                        }
                        else
                        {
                            currentPage -= 1;
                        }
                        Console.WriteLine($"Current Page: {currentPage + 1}");
                        Console.WriteLine("Next, Prev, Quit");
                        Console.Write(">>");
                        string cmd = Console.ReadLine();
                        switch (cmd.ToLower())
                        {
                            case "n":
                            case "next":
                            {
                                if (!(currentPage + 1 > book.pages.Count))
                                {
                                    currentPage += 1;
                                }
                                else
                                {
                                    Console.WriteLine("You've reached the end of the book.");

                                }
                                break;

                            }
                            case "p":
                            case "prev":
                            {
                                if (!(currentPage - 1 < 0))
                                {
                                    currentPage -= 1;
                                }
                                else
                                {
                                    Console.WriteLine("You've reached the beginning of the book.");
                                }
                            }
                                break;
                            case "q":
                            case "quit":
                            {

                                Console.WriteLine("You close the book.");
                                book.onReadAction(book);
                                return "";
                            }
                                break;
                            default:
                                Console.WriteLine("Please enter a valid command");
                                break;
                        }
                    }
                }
                return "You can't read that!";
            });
            complexCommandMap.Add("r", complexCommandMap["read"]);
            complexCommandMap.Add("eat", (s) => TextUtils.Pick("That doesn't seem prudent.", "I don't recommend that.", "This isn't a good idea."));
            complexCommandMap.Add("inscribe", (s) =>
            {
                //TODO:KILLAF
                //if (!s.ToLower().Contains("wand")) return "You can only inscribe on wands!";

                bool isInInventory = false;
                var obj = program.FindObject(s, out isInInventory, true);
                if (obj != null && !(obj.Value.Value is Wand))
                    return "You can only inscribe on wands!";
                if (program.FindObject("Inscription Table") == null)
                    return "You need to be near an inscription table to do that!";
                if (isInInventory)
                {
                    Console.WriteLine("What should the new inscription say?");
                    Console.Write(">>");
                    string newInscription = Console.ReadLine();
                    obj.Value.Value.properties["inscription"] = newInscription;
                    return $"You inscribe \"{newInscription}\" onto the wand.";
                }

                return "The wand has to be in your inventory!";

            });

            complexCommandMap.Add("examine", (s =>
            {
                var obj = program.FindObject(s, true);
                StringBuilder sb = new StringBuilder();
                if (obj != null && obj.Value.Value.senses != null && obj.Value.Value.senses.ContainsKey("sight"))
                {
                    sb.AppendLine(obj.Value.Value.senses?["sight"](obj.Value.Value));
                }

                if (obj != null)
                {
                    foreach (var property in obj.Value.Value.properties)
                    {
                        if (PropertiesExaminer.propertiesMap.ContainsKey(property.Key))
                        {
                            sb.Append(PropertiesExaminer.propertiesMap[property.Key](obj.Value.Key, obj.Value.Value) + " ");
                        }

                    }
                    if (obj.Value.Value.canHoldItems)
                    {
                        if (obj.Value.Value.nounChildren.Count != 0)
                        {
                            sb.AppendLine($"\nThe {obj.Value.Key.ToLower()} has:");
                            sb.AppendLine(obj.Value.Value.FormattedChildren());
                        }
                    }
                }
                if (obj == null)
                {
                    return $"I can't find the {s}.";
                }
                if (sb.Length == 0)
                {
                    return "I don't know what to say!";
                }
                return sb.ToString();
            }
                ));
            complexCommandMap.Add("x", complexCommandMap["examine"]);
            complexCommandMap.Add("look", complexCommandMap["examine"]);
            complexCommandMap.Add("l", complexCommandMap["examine"]);
            complexCommandMap.Add("zap", (s) =>
            {

                bool isInInventory;
                var obj = program.FindObject(s, out isInInventory, true);
                if (obj == null)
                {
                    return $"I can't find {s}!";
                }
                if (!isInInventory)
                {
                    return "The wand has to be in your inventory to use it!";
                }
                if (!program._gameState.isEmpowered)
                {
                    return $"You wave the wand around, but nothing happens.";
                }
                if (obj?.Value is Wand)
                {
                    if (obj.Value.Value.properties.ContainsKey("inscription"))
                    {
                        return program.SpellCast.CastSpell(obj.Value.Value.properties["inscription"]);
                    }
                }
                return "That doesn't make any sense.";
            });
            complexCommandMap.Add("z", complexCommandMap["zap"]);

            complexCommandMap.Add("take", (s) =>
            {
                var obj = program.FindObject(s);
                if (obj != null && obj.Value.Value.canBePickedUp)
                {
                    program._gameState.inventory.AddItem(obj.Value.Key, obj.Value.Value);
                    program._gameState.currentRoom().RemoveNoun(obj.Value.Key);
                    return $"You take the {obj.Value.Key.ToLower()}.";
                }
                else
                    return "You can't take that!";
            });
            complexCommandMap.Add("t", complexCommandMap["take"]);
            complexCommandMap.Add("drop", (s) =>
            {
                bool isInInv;
                var obj = program.FindObject(s, out isInInv, true);
                if (obj != null && isInInv)
                {
                    program._gameState.inventory.RemoveItem(obj.Value.Key);
                    program._gameState.currentRoom().AddNoun(obj.Value.Key, obj.Value.Value);
                    return $"You drop the {s}.";
                }
                return "You can't drop that!";
            });
            complexCommandMap.Add("place", (s) =>
            {
                bool isInInv;
                if (!s.Contains(" on ") && !s.Contains(" in "))
                {
                    return "That doesn't make sense!";


                }
                bool inOn = true;
                string invTarget = null;
                string genitiveTarget = null;
                if (s.Contains(" on "))
                {
                    if (s.Split(new[] { " on " }, StringSplitOptions.None).Length != 2)
                        return "That doesn't make sense!";
                    invTarget = s.Split(new[] { " on " }, StringSplitOptions.None)[0].Replace("the ", string.Empty);
                    genitiveTarget = s.Split(new[] { " on " }, StringSplitOptions.None)[1].Replace("the ", string.Empty);
                    inOn = false;

                }
                if (s.Contains(" in "))
                {
                    if (s.Split(new[] { " in " }, StringSplitOptions.None).Length != 2)
                        return "That doesn't make sense!";

                    invTarget = s.Split(new[] { " in " }, StringSplitOptions.None)[0].Replace("the ", string.Empty);
                    genitiveTarget = s.Split(new[] { " in " }, StringSplitOptions.None)[1].Replace("the ", string.Empty);
                    inOn = true;
                }



                var objInvTarget = program.FindObject(invTarget, out isInInv, true);
                var objGenitiveTarget = program.FindObject(genitiveTarget);
                if (objInvTarget != null && isInInv)
                {
                    if (objGenitiveTarget != null)
                    {
                        if (objGenitiveTarget.Value.Value.canHoldItems)
                        {
                            program._gameState.inventory.RemoveItem(objInvTarget.Value.Key);
                            //Console.WriteLine(_gameState.inventory.objMap.ContainsKey(invTarget).ToString());

                            objGenitiveTarget.Value.Value.nounChildren.Add(objInvTarget.Value.Key, objInvTarget.Value.Value);
                            program._gameState.currentRoom().RegisterNounInScene(objInvTarget.Value.Key, objInvTarget.Value.Value);
                            if (inOn)
                                return
                                    $"You place the {objInvTarget.Value.Key.ToLower()} in the {objGenitiveTarget.Value.Key.ToLower()}.";
                            else
                                return
                                    $"You place the {objInvTarget.Value.Key.ToLower()} on the {objGenitiveTarget.Value.Key.ToLower()}.";


                        }
                        return "You can't place things there!";
                    }
                    return $"What are you placing the {invTarget.ToLower()} on?";
                }
                return "What are you placing?";
            });
            complexCommandMap.Add("point", (s) =>
            {
                var obj = program.FindObject(s.Replace("at", string.Empty), true);

                if (obj == null)
                {
                    return $"I don't know what {s.Replace("at", string.Empty)} is.";
                }
                if (obj.Value.Value.myNoun.stem == null)
                {
                    return $"You point your finger at the {s.ToLower()}.  Nothing happens.";
                }
                if (program._gameState.isEmpowered)
                {
                    return $"You point your finger at the {s.ToLower()}.  The word \"{obj.Value.Value.myNoun.stem}\" {TextUtils.Pick("appears", "pops")} into your mind.";
                }
                return $"You point your finger at the {s.ToLower()}.  Doing this seems kinda /pointless/.";
            });
            complexCommandMap.Add("use", (s) =>
            {
                bool isInInventory = false;
                var obj = program.FindObject(s, out isInInventory, true);
                if (obj == null)
                {
                    return $"I don't know what {s} is.";
                }
                if (obj.Value.Value.UseAction == null)
                {
                    return $"You can't use the {obj.Value.Key.ToLower()}.";
                }
                return obj.Value.Value?.UseAction?.Invoke(obj.Value.Value, isInInventory);
            });
            complexCommandMap.Add("unlock", (s) =>
            {
                var k = program.FindObject(s);
                if (k == null)
                {
                    return "I can't find that!";
                }
                if (!(k.Value.Value is IUnlockable))
                {
                    return "That doesn't make sense!";
                }
                IUnlockable unlockable = (IUnlockable) k.Value.Value;

                if (unlockable.IsUnlocked)
                {
                    return "That is already unlocked!";
                }
                else
                {
                    Console.WriteLine($"Which key should be used to unlock the {k.Value.Key}?");
                    Console.Write(">>");
                    var findKey = program.FindObject(Console.ReadLine(),true);
                    if (findKey != null)
                    {
                        if (findKey.Value.Value is Key)
                        {
                            Key key = findKey.Value.Value as Key;
                            return unlockable.Unlock(key);
                        }
                        else
                        {
                            return "That isn't a key!";
                        }
                    }
                    else
                    {
                        return "I can't find that key!";
                    }

                    
                }

            });
        }
    }
}