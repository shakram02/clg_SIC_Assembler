using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assembler_SIC
{
    public class LitTabEntry
    {


        public string Name { get; }

        public int? Address { get; set; }

        public bool Interted { get; set; }
        /// <summary>
        /// Decimal value of the literal
        /// </summary>
        public string Value { get; }

        public int Size { get; private set; }

        public LitTabEntry(string name, int? address)
        {
            this.Interted = false;
            if (name == null)
                throw new InvalidOperationException("Can't assign a value to an empty string");

            this.Name = name;

            // Find the value of the literal
            if (name.StartsWith("="))
            {
                this.Value = extractValue(name);
            }
            else
            {
                throw new InvalidOperationException("Literal value is invalid");
            }

            // Add the entry to the assembler
            Assembler.LitTab.Add(name, this);
        }

        private string extractValue(string name)
        {
            // Remove the equal sign attached to X' --- ' and C' --- '
            try
            {
                name = name.Remove(0, 3);
                name = name.Remove(name.Length - 1);
            }
            catch (ArgumentOutOfRangeException exc)
            {
                LogFile.LogError("Literal doesn't follow the correct format ," + exc.Message);

                throw new InvalidOperationException("Literal value is invalid, bad formatting:" + exc.Message);
            }
            // Parse the value of the string
            return (parseName(name));
        }

        private string parseName(string name)
        {
            int value;

            // If valid, convert the number to a hex string
            if (int.TryParse(name,
                System.Globalization.NumberStyles.HexNumber,
                System.Globalization.CultureInfo.InvariantCulture,
                out value))
            {
                // Int in SIC is 3 bytes
                this.Size = 3;
                return value.ToString();
            }
            else if (name.ToLower().StartsWith("c'"))
            {
                // ASCII Hexadecimal values
                StringBuilder val = new StringBuilder();
                foreach (char character in name)
                {
                    val.Append(((int)character).ToString("X"));
                }
                this.Size = name.Length;
                return val.ToString();
            }
            else
            {
                throw new InvalidOperationException("Literal isn't a valid integeral value");
            }
        }
    }
}
