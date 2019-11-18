using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace MagicLanguageParser
{
    static class TextUtils
    {
        public static Random rng = new Random();
        public static string Pick(params string[] args)
        {
            return args[rng.Next(0, args.Length)];
        }

        public static T DeepCopy<T>(T other)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(ms,other);
                ms.Position = 0;
                return (T) formatter.Deserialize(ms);
            }
        }
    }
}
