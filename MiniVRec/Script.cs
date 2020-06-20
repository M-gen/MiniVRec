using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.IO;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.CSharp.Scripting;

namespace Emugen.Script
{
    public class Script<T>
    {
        public T api;
        //CSharpScript script;
        Microsoft.CodeAnalysis.Scripting.Script<object> script;

        public Script( string path, T api )
        {
            this.api = api;
            script = CSharpScript.Create(File.ReadAllText(path), globalsType: typeof(T));
        }

        public void Run()
        {
            script.RunAsync(api).Wait();
        }

    }
}
