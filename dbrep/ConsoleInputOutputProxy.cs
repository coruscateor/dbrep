using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using CoreComponents.Text;

namespace dbrep
{

    public class ConsoleInputOutputProxy : IInputOutputProxy
    {

        public ConsoleInputOutputProxy()
        {
        }

        public virtual string ReadLine()
        {

            string Line = Console.ReadLine();

            Console.WriteLine();

            return Line;

        }

        public virtual void WriteLine()
        {

            Console.WriteLine();

        }

        public virtual void WriteLine(string TheLine)
        {

            Console.WriteLine(TheLine);

            Console.WriteLine();

        }

        public virtual void WriteLine(object TheItem)
        {

            Console.WriteLine(TheItem);

            Console.WriteLine();

        }

        public virtual void WriteSingleLine(string TheLine)
        {

            Console.WriteLine(TheLine);

        }

        public virtual void WriteSingleLine(object TheItem)
        {

            Console.WriteLine(TheItem);

        }

        public void WriteConnectionIsOpen()
        {

            WriteLine("Connection is open");

        }

        public void WriteConnectionIsOpen(string TheConnectionString)
        {

            StringBuilder SB = StringBuilderPool.FetchOrCreate();

            SB.Append("Connection: \"");

            SB.Append(TheConnectionString);

            SB.Append("\" is open");

            string output = SB.ToString();

            StringBuilderPool.Put(SB);

            WriteLine(output);

        }

        public void WriteConnectionIsClosed()
        {

            WriteLine("Connection is closed");

        }

        public void WriteException(Exception TheException)
        {

            WriteLine(TheException.Message);

            WriteLine(TheException.StackTrace);

        }

        public void WriteDataReader(DbDataReader TheDataReader)
        {

            try
            {

                List<string> Names = new List<string>(TheDataReader.FieldCount);

                for(int i = 0; i < TheDataReader.FieldCount; ++i)
                {

                    Names.Add(TheDataReader.GetName(i));

                }

                if(Names.Count < 1)
                    return;

                if(TheDataReader.HasRows)
                {

                    int FieldCount = TheDataReader.FieldCount;

                    List<List<string>> ColumnValues = new List<List<string>>(FieldCount);

                    //for(int i = 0; i < FieldCount; ++i)
                    //{

                    //    ColumnValues.Add(new List<string>());

                    //}

                    //int y = 0;

                    object[] Values = new object[FieldCount];

                    while(TheDataReader.Read())
                    {

                        TheDataReader.GetValues(Values);

                        //List<string> CurrentList = Columns[y];

                        List<string> CurrentRow = new List<string>();

                        foreach(var Item in Values)
                        {

                            CurrentRow.Add(Item.ToString());

                        }

                        ColumnValues.Add(CurrentRow);

                        //++y;

                    }

                    WriteTable(Names, ColumnValues);

                }
                else
                {

                    WriteHeadingsOnly(Names);

                }

            }
            finally
            {

                TheDataReader.Dispose();

            }

        }

        protected void WriteHeadingsOnly(List<string> TheNames)
        {

            StringBuilder TableHorisontalChars = StringBuilderPool.FetchOrCreate("+");

            StringBuilder TableNames = StringBuilderPool.FetchOrCreate("|");


            for(int i = 0; i < TheNames.Count; ++i)
            {

                string CurrentName = TheNames[i];

                int DashesCount = CurrentName.Length + 2;

                TableNames.Append(' ');

                TableNames.Append(CurrentName);

                TableNames.Append(" |");

                for(; DashesCount > 0; --DashesCount)
                    TableHorisontalChars.Append('-');

                TableHorisontalChars.Append('+');

            }

            string THCs = TableHorisontalChars.ToString();

            StringBuilderPool.Put(TableHorisontalChars);

            WriteSingleLine(THCs);

            WriteSingleLine(TableNames);

            StringBuilderPool.Put(TableNames);

            WriteLine(THCs);

        }

        protected void WriteTable(List<string> TheNames, List<List<string>> TheColumns)
        {

            List<int> LongestLengths = new List<int>();

            //Figure out the longest strings for each row

            for(int i = 0; i < TheNames.Count; ++i)
            {

                int Longest = TheNames[i].Length;

                foreach(var Item in TheColumns)
                {

                    int CurrentLength = Item[i].Length;

                    if(CurrentLength > Longest)
                        Longest = CurrentLength;

                }

                LongestLengths.Add(Longest);

            }

            //Assemble the Horisontal dash segments

            StringBuilder SB = StringBuilderPool.FetchOrCreate("+");

            foreach(var Item in LongestLengths)
            {

                for(int i = -2; i < Item; ++i)
                    SB.Append('-');

                SB.Append('+');

            }

            string THCs = SB.ToString();

            SB.Clear();

            WriteSingleLine(THCs);
            
            WriteRow(SB, LongestLengths, TheNames);

            WriteSingleLine(THCs);

            WriteRows(SB, LongestLengths, TheColumns);

            StringBuilderPool.Put(SB);

            WriteLine(THCs);

        }

        protected void WriteRows(StringBuilder TheSB, List<int> TheLongestLengths, List<List<string>> TheItems)
        {

            foreach(var Item in TheItems)
            {

                WriteRow(TheSB, TheLongestLengths, Item);

            }

        }

        protected void WriteRow(StringBuilder TheSB, List<int> TheLongestLengths, List<string> TheItems)
        {
            
            int i = 0;

            TheSB.Append('|');

            foreach(var Item in TheItems)
            {

                TheSB.Append(' ');

                int CurrentLL = TheLongestLengths[i];

                int Difference = CurrentLL - Item.Length;

                TheSB.Append(Item);

                while(Difference > 0)
                {

                    TheSB.Append(' ');

                    --Difference;

                }

                TheSB.Append(" |");

                ++i;

            }

            //TheSB.Append('|');

            WriteSingleLine(TheSB);

            TheSB.Clear();

        }
        
    }

}
