using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MagicLanguageParser
{
    class RoomHolder
    {
        public Dictionary<string, Room> keyRoomMap;

        public RoomHolder()
        {
            keyRoomMap = new Dictionary<string, Room>
            {
                {
                    "Debug1", new Room("Debug Room", "A room for debugging",
                        "A fairly plain room for debugging.  There is nothing to see here.  To the south you see a wooden cabin.",
                        new Dictionary<RoomDirections, Func<string>>()
                        {
                            {RoomDirections.SOUTH, () => "Debug2"},
                        })
                        .AddNoun("Inscription Table",NounPrefabs.prefabs["inscriptionMachine"])
                        .AddNoun("Old Journal",NounPrefabs.prefabs["empowermentBook"])
                },
                {
                    "Debug2",
                    new Room("Wooden Cabin", "A small wooden cabin.",
                        "A small wooden cabin in the middle of a forest.  There is some sort of light to the north of you.  To the east there is a metallic door that is labeled \"entrance\".",
                        new Dictionary<RoomDirections, Func<string>>
                        {
                            {RoomDirections.NORTH, () => "Debug1"},
                            {RoomDirections.EAST, () =>  "Debug3"}
                        })
                        .AddNoun("Table", TextUtils.DeepCopy(NounPrefabs.prefabs["table"]))
                        .AddNounChild("Table","Orange Wand",NounPrefabs.prefabs["OrangeColorChangingWand"])
                        .AddNounChild("Table","Blue Wand",NounPrefabs.prefabs["BlueColorChangingWand"])
                        .AddNounChild("Table","White Wand", NounPrefabs.prefabs["BlankWand"])
                        .AddNoun("Apple", NounPrefabs.prefabs["apple"])

                },
                {
                    "Debug3",
                    new Room("White Room","A white room","A small white room with two doors, labeled \"entrance\" and \"exit\".  The entrance door is to the west, and the exit door is to the south.  There is a pink pedestal sitting in the center of the room that is connected by a wire to the door to the south.", new Dictionary<RoomDirections, Func<string>>()
                    {
                        { RoomDirections.WEST, ()=>"Debug2"}
                    })
                    .AddNoun("Pedestal",NounPrefabs.prefabs["debug3_pedestal"])
                },
                { 
                    "Debug4",
                    new Room("Golden Room","A golden room.","A small golden room.  To the north there is a door labeled \"exit\".  To the west there is a door with a sign above it.", new Dictionary<RoomDirections, Func<string>>()
                    {
                        {RoomDirections.NORTH, ()=>"Debug3" },
                        {RoomDirections.WEST, ()=>"Debug5" }
                    })
                    .AddNoun("Sign",new NounObject(new Noun(null,"sign"),"A wooden sign.",new Dictionary<string, Func<NounObject, string>>()
                    {
                        {"sight",(x)=> $"The sign reads: Do not enter this room.  It is a dead end. (But if you do enter you might want a couple wands...)" }
                    },false))
                    
                },
                {
                    "Debug5",
                    new Room("Dead End","A dead end.","Congratulations, you have reached a dead end.  There is no escape from this room. (kinda).  To the north, there is a door.", null)
                        .SetOnEntranceFirstTime(
                            (x) =>
                            {
                                Console.WriteLine("After you enter the room you hear a loud crashing sound.  THE DOOR DISAPPEARED.  WHY DIDN'T YOU READ THE SIGN YOU MORON");
                            }
                        )
                        .AddNoun("Northern  Door",new Door(new Noun(null,"orangeDoor"),"A locked door.",new Dictionary<string, Func<NounObject, string>>() { {"sight",(x)=>"A locked door to the north."} }, RoomDirections.NORTH, ()=>"Debug4",false,false).AddProperty("color","orange"))
                        .AddNoun("Key",new Key(new Noun(Program.Instance._nounHolder.englishNounKey["Key"].stem,"Key"),"The first key.",new Dictionary<string, Func<NounObject, string>>() { {"sight",(x)=>"The first key of many."} },true,false ).AddProperty("color","white"))
                        .AddNoun("Inscription Table",NounPrefabs.prefabs["inscriptionMachine"])
                },
                {
                    "Debug6",
                    new Room("Study","A study","A study.  A room for studying.  There is a door to the south.  Bookshelves line the walls with a desk in the center of the room.",new Dictionary<RoomDirections, Func<string>>()
                    {
                        {RoomDirections.SOUTH, ()=>"Debug1" }
                    })
                    .AddNoun("Table",NounPrefabs.GetPrefab("Table"))
                        .AddNounChild("Table","Paper", NounPrefabs.GetPrefab("blankPaper"))
                        .AddNounChild("Table","Pen",NounPrefabs.GetPrefab("Pen"))
                        
                },

                #region GrantPuzzle
                {
                    "GrantPuzzle_EntranceHall",
                    new Room("Entrance Hall","A small room","There is an ajar door to the east.  A green door lies to the north.",new Dictionary<RoomDirections, Func<string>>()
                    {
                        {RoomDirections.EAST,()=>"GrantPuzzle_Closet"}
                        ,
                        {RoomDirections.SOUTH, ()=>"Debug6" }
                    })
                    .AddNoun("Northern Door", new Door(new Noun(null,"greenDoor"),"A locked door.",new Dictionary<string, Func<NounObject, string>>() { {"sight",(x)=>"A locked door to the north"} },RoomDirections.NORTH, ()=>"GrantPuzzle_MainRoom",false,false).AddProperty("color","green"))
                },
                {
                    "GrantPuzzle_Closet",
                    new Room("Closet","The room is somewhat cramped.","The room is somewhat cramped.",new Dictionary<RoomDirections, Func<string>>() { {RoomDirections.WEST, ()=>"GrantPuzzle_EntranceHall"} })
                    .AddNoun("Table",NounPrefabs.GetPrefab("table"))
                    .AddNounChild("Table","Key",new Key(new Noun(Program.Instance._nounHolder.englishNounKey["Key"].stem,"Key"),"It is a small metallic key.",null,true, false).AddProperty("color","green"))
                },
                {
                    "GrantPuzzle_MainRoom",
                    new Room("Main Room","A main room","A large decorated cobblestone room.  To the north there are three pedestals colored red, green, and blue.  To the east there is a lever, with a plus above it and a minus below it.  There is a purple door to the west, and an open green door to the south.", new Dictionary<RoomDirections, Func<string>>()
                    {
                        {RoomDirections.SOUTH, ()=>"GrantPuzzle_EntranceHall" }
                    })
                    .AddNoun("Western Door",new Door(new Noun(null,"purpleDoor"),"A locked door to the west",null,RoomDirections.WEST, ()=>"GrantPuzzle_RewardRoom",false).AddProperty("color","purple"))
                    .AddNoun("Lever",NounPrefabs.GetPrefab("lever"))
                    .AddNoun("Red Pedestal",NounPrefabs.GetPrefab("grantpuzzle_redpedestal"))
                    .AddNoun("Green Pedestal",NounPrefabs.GetPrefab("GrantPuzzle_GreenPedestal"))
                    .AddNoun("Blue Pedestal",NounPrefabs.GetPrefab("GrantPuzzle_BluePedestal"))
                }
                
,
                {
                    "GrantPuzzle_RewardRoom",
                    new Room("Reward Room","A reward room?","A reward room? I guess man. Idk.",new Dictionary<RoomDirections, Func<string>>() { {RoomDirections.EAST, ()=>"GrantPuzzle_MainRoom"} })
                    .AddNoun("Inscription Table",NounPrefabs.GetPrefab("inscriptionMachine"))
                }               

                #endregion GrantPuzzle








            };
            foreach (var room in keyRoomMap)
            {
                room.Value.roomKey = room.Key;
            }
        }



        public Room this[string debug1] => keyRoomMap.Where(room => room.Key.ToLower() == debug1.ToLower()).Select(room => room.Value).FirstOrDefault();
    }
}