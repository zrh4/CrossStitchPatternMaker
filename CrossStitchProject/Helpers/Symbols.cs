using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CrossStitchProject.Helpers
{
    public class Symbols
    {
        public static IEnumerable<string> GetSymbols()
        {
            using (var symbolStream = Assembly.GetExecutingAssembly()
                .GetManifestResourceStream("CrossStitchProject.symbols.dat"))
            using (var symbolReader = new StreamReader(symbolStream))
            {
                while(!symbolReader.EndOfStream)
                {
                    yield return symbolReader.ReadLine();
                }
            }
        }
    }
}
