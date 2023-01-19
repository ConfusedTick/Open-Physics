using System;
using System.Collections.Generic;
using System.Linq;
using Sim.Utils;
using System.Text;

namespace Sim.Particles
{
    public class Flags : ICloneable
    {

        public Dictionary<string, bool> Params = new Dictionary<string, bool> { };

        public static Flags Empty { get { return new Flags(); } }

        public static Flags Dev { get { return new Flags(new string[] { "isdevpart=1" }); } }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Flags(string[] parsable)
        {
            foreach (string pr in parsable)
            {
                if (pr == "")
                {
                    continue;
                }

                string[] args = pr.Split("=");
                Params.Add(args[0], Utils.Utils.ToBool(args[1]));
            }
        }

        public Flags(List<string> parsable)
        {
            foreach (string pr in parsable)
            {
                if (pr == "")
                {
                    continue;
                }

                string[] args = pr.Split("=");
                Params.Add(args[0], Utils.Utils.ToBool(args[1]));
            }
        }

        public Flags()
        {

        }

        public override string ToString()
        {
            return Params.ToString();
        }

        public bool Get(string par)
        {
            return Params[par];
        }

        public void Set(string key, bool value)
        {
            if (Params.ContainsKey(key))
            {
                Params[key] = value;
            }
            else
            {
                Params.Add(key, value);
            }
        }

        public bool Contains(string key)
        {
            return Params.ContainsKey(key);
        }

        public static Flags FromString(string input)
        {
            List<string> parsable = new List<string> { };
            string[] pars = input.Split(";");
            foreach (string am in pars)
            {
                if (am == "")
                {
                    continue;
                }

                parsable.Add(am);
            }
            return new Flags(parsable);
        }


        public static Flags operator +(Flags left, Flags right)
        {
            Flags outFlags = Empty;
            foreach (string flag in left.Params.Keys)
            {
                outFlags.Set(flag, left.Get(flag));
            }
            foreach (string rflag in right.Params.Keys)
            {
                if (!outFlags.Contains(rflag) | outFlags.Get(rflag) != right.Get(rflag)) outFlags.Set(rflag, right.Get(rflag));
            }
            return outFlags;
        }
    }
}
