using System;
using System.Collections.Generic;
using System.Text;

namespace Sim.Commands
{
    public class GlobalValues
    {

        public static Dictionary<string, string> Values = new Dictionary<string, string>();

        public static void SetValue(string name, string val)
        {
            if (ContainsValue(name))
            {
                Values[name] = val;
            }
            else
            {
                Values.Add(name, val);
            }
        }

        public static void RemoveValue(string name)
        {
            if (ContainsValue(name))
            {
                Values.Remove(name);
            }
        }

        public static bool ContainsValue(string name)
        {
            return Values.ContainsKey(name);
        }

        public static string GetValue(string name)
        {
            if (ContainsValue(name))
            {
                return Values[name];
            }
            else
            {
                return null;
            }
        }

    }
}
