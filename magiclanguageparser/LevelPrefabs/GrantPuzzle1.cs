using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MagicLanguageParser
{
    public static partial class NounPrefabs
    {

        private static void GrantPuzzlePrefabs()
        {
            #region GrantPuzzlePrefabs

            prefabs.Add("grantpuzzle_redpedestal",
                new NounObject(new Noun(null, "redPedsetal"), "A red pedestal",
                    (x) => "A pedestal, it is glowing with magical power.", false).AddProperty("color", "red")
                    .SetUseAction((x, isInInv) =>
                    {
                        var key = Program.Instance.FindObject("key", true)?.Value;
                        var currentKeyColor = key.properties["color"];
                        var leverState = Program.Instance.FindObject("lever").Value.Value.properties["switchStatus"];
                        if (leverState == "up")
                        {
                            switch (currentKeyColor)
                            {
                                case "green":
                                    key.properties["color"] = "yellow";
                                    break;
                                case "teal":
                                    key.properties["color"] = "white";
                                    break;
                                case "blue":
                                    key.properties["color"] = "purple";
                                    break;
                                default:
                                    return "Nothing happens...";
                            }

                        }
                        else
                        {
                            switch (currentKeyColor)
                            {
                                case "white":
                                    key.properties["color"] = "teal";
                                    break;
                                case "yellow":
                                    key.properties["color"] = "green";
                                    break;
                                case "purple":
                                    key.properties["color"] = "blue";
                                    break;
                                case "red":
                                    key.properties["color"] = "black";
                                    break;
                                default:
                                    return "Nothing happens...";
                            }
                        }
                        return $"The key shines and turns {key.properties["color"]}.";

                    })
                );
            prefabs.Add("grantpuzzle_greenpedestal",
                new NounObject(new Noun(null, "greenPedsetal"), "A green pedestal",
                    (x) => "A pedestal, it is glowing with magical power.", false).AddProperty("color", "green")
                    .SetUseAction((x, isInInv) =>
                    {
                        var key = Program.Instance.FindObject("Key", true)?.Value;
                        var currentKeyColor = key.properties["color"];
                        var leverState = Program.Instance.FindObject("lever").Value.Value.properties["switchStatus"];
                        if (leverState == "up")
                        {
                            switch (currentKeyColor)
                            {
                                case "red":
                                    key.properties["color"] = "yellow";
                                    return $"The key shines and turns {key.properties["color"]}.";
                                case "blue":
                                    key.properties["color"] = "teal";
                                    return $"The key shines and turns {key.properties["color"]}.";
                                case "purple":
                                    key.properties["color"] = "white";
                                    return $"The key shines and turns {key.properties["color"]}.";
                                default:
                                    return "Nothing happens...";
                            }
                        }
                        else
                        {
                            switch (currentKeyColor)
                            {
                                case "yellow":
                                    key.properties["color"] = "red";
                                    return $"The key shines and turns {key.properties["color"]}.";
                                case "teal":
                                    key.properties["color"] = "blue";
                                    return $"The key shines and turns {key.properties["color"]}.";
                                case "white":
                                    key.properties["color"] = "purple";
                                    return $"The key shines and turns {key.properties["color"]}.";
                                case "green":
                                    key.properties["color"] = "black";
                                    return $"The key shines and turns {key.properties["color"]}.";
                                default:
                                    return "Nothing happens...";
                            }
                        }
                    }));
            prefabs.Add("grantpuzzle_bluepedestal",
                new NounObject(new Noun(null, "bluePedsetal"), "A blue pedestal",
                    (x) => "A pedestal, it is glowing with magical power.", false).AddProperty("color", "blue")
                    .SetUseAction((x, isInInv) =>
                    {
                        var key = Program.Instance.FindObject("Key", true)?.Value;
                        var currentKeyColor = key.properties["color"];
                        var leverState = Program.Instance.FindObject("lever").Value.Value.properties["switchStatus"];
                        if (leverState == "up")
                        {
                            switch (currentKeyColor)
                            {
                                case "yellow":
                                    key.properties["color"] = "white";
                                    return $"The key shines and turns {key.properties["color"]}.";
                                case "red":
                                    key.properties["color"] = "purple";
                                    return $"The key shines and turns {key.properties["color"]}.";
                                case "green":
                                    key.properties["color"] = "teal";
                                    return $"The key shines and turns {key.properties["color"]}.";
                                default:
                                    return "Nothing happens...";
                            }
                        }
                        else
                        {
                            switch (currentKeyColor)
                            {
                                case "white":
                                    key.properties["color"] = "yellow";
                                    return $"The key shines and turns {key.properties["color"]}.";
                                case "purple":
                                    key.properties["color"] = "red";
                                    return $"The key shines and turns {key.properties["color"]}.";
                                case "teal":
                                    key.properties["color"] = "green";
                                    return $"The key shines and turns {key.properties["color"]}.";
                                case "blue":
                                    key.properties["color"] = "black";
                                    return $"The key shines and turns {key.properties["color"]}.";
                                default:
                                    return "Nothing happens...";
                            }
                        }
                    }));
            #endregion GrantPuzzlePrefabs

        }
    }
}

