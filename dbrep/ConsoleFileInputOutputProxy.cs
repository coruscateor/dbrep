using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace dbrep
{
    
    public class ConsoleFileInputOutputProxy : ConsoleInputOutputProxy
    {

        protected string myPath; 

        public ConsoleFileInputOutputProxy(string ThePath)
        {

            myPath = ThePath;

        }

        public string Path
        {

            get
            {

                return myPath;

            }

        }

        public override string ReadLine()
        {

            string Line = base.ReadLine();

            Log(Line);

            return Line;

        }

        public override void WriteLine()
        {

            using(StreamWriter SW = new StreamWriter(myPath, true))
            {

                SW.WriteLine();

            }

        }

        public override void WriteLine(string TheLine)
        {

            base.WriteLine(TheLine);

            Log(TheLine);

        }

        public override void WriteLine(object TheItem)
        {

            base.WriteLine(TheItem);

            Log(TheItem);

        }

        public override void WriteSingleLine(string TheLine)
        {

            base.WriteSingleLine(TheLine);

            using(StreamWriter SW = new StreamWriter(myPath, true))
            {

                SW.WriteLine(TheLine);

            }

        }

        public override void WriteSingleLine(object TheItem)
        {

            base.WriteSingleLine(TheItem);

            using(StreamWriter SW = new StreamWriter(myPath, true))
            {

                SW.WriteLine(TheItem);

            }

        }

        protected void Log(string TheLine)
        {

            using(StreamWriter SW = new StreamWriter(myPath, true))
            {

                SW.WriteLine(TheLine);

                SW.WriteLine();

            }

        }

        protected void Log(object TheItem)
        {

            using(StreamWriter SW = new StreamWriter(myPath, true))
            {

                SW.WriteLine(TheItem);

                SW.WriteLine();

            }

        }

    }

}
