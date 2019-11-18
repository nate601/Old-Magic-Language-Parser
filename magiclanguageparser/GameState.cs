using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicLanguageParser
{
    class GameState
    {
        public Func<Room> currentRoom;
        public RoomHolder roomHolder;

        public CharacterInventory inventory = new CharacterInventory();

        public bool isEmpowered = false;

        public List<string> firstTimeRanIds = new List<string>(); 

        public void ChangeRoom(string roomId)
        {
            currentRoom = ()=>roomHolder[roomId];
            if(!firstTimeRanIds.Contains(currentRoom().roomKey))
            {
                firstTimeRanIds.Add(currentRoom().roomKey);
                currentRoom().onEntranceFirstTime?.Invoke(currentRoom());
            }
        }

        public GameState()
        {
            roomHolder = new RoomHolder();
            ChangeRoom("GrantPuzzle_EntranceHall");
        }
        

    }

    public class CharacterInventory
    {
        public Dictionary<string, NounObject> objMap = new Dictionary<string, NounObject>();


        public void AddItem(string name, NounObject obj)
        {
            objMap.Add(name, obj);
        }

        public void RemoveItem(string key)
        {
            objMap.Remove(key);
        }
    }
}
