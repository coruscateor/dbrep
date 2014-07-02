﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using CoreComponents;
using CoreComponents.Text;
using CoreComponents.Data;
 
namespace dbrep
{

    public class CMDMain<TExecutor> where TExecutor : IExecutor, new()
    {

        public CMDMain()
        {
        }

        public void Run(string[] args)
        {

            Dictionary<string, string> CommandLineItems;

            string OutputPath = null;

            string ConnectionString = null;

            bool OutputFiles = false;

            string CmdInput = null;

            bool Quit = false;

            try
            {

                if(TryParseCommandLine(args, out CommandLineItems))
                {

                    //Connectionstring - most likely in quotes

                    if(CommandLineItems.ContainsKey("-con"))
                    {

                        ConnectionString = CommandLineItems["-con"];

                    }

                    //Logging output - can be an autogenerated file or a specified file provided as a parameter.

                    if(CommandLineItems.ContainsKey("-out"))
                    {

                        OutputFiles = true;

                        OutputPath = CommandLineItems["-out"];

                    }

                    //The provided command or a file containing one or more commands to execute on startup.

                    if(CommandLineItems.ContainsKey("-cmd"))
                    {

                        string Cmd = CommandLineItems["-cmd"];

                        if(File.Exists(Cmd))
                        {

                            using(StreamReader SR = new StreamReader(Cmd))
                            {

                                CmdInput = SR.ReadToEnd();

                            }

                        }
                        else
                        {

                            CmdInput = Cmd;

                        }

                    }

                    //Quit immdiately after 

                    if(CommandLineItems.ContainsKey("-q"))
                    {

                        Quit = true;

                    }

                }

            }
            catch(Exception e)
            {

                Write(e);

                return;

            }

            TExecutor myExecutor = new TExecutor();

            myExecutor.ConnectionString = ConnectionString;

            IInputOutputProxy IOProxy;

            if(OutputFiles)
            {

                try
                {

                    //Create the log file

                    if(string.IsNullOrWhiteSpace(OutputPath))
                    {

                        DateTime CurrentDateTime = DateTime.Now;

                        StringBuilder SB = StringBuilderPool.FetchOrCreate();

                        SB.Append(Process.GetCurrentProcess().ProcessName);

                        SB.Append("-log-");

                        SB.Append(CurrentDateTime.Year);

                        SB.Append('-');

                        SB.Append(CurrentDateTime.Month);

                        SB.Append('-');

                        SB.Append(CurrentDateTime.Day);

                        SB.Append('-');

                        SB.Append(CurrentDateTime.Hour);

                        SB.Append('-');

                        SB.Append(CurrentDateTime.Minute);

                        SB.Append('-');

                        SB.Append(CurrentDateTime.Second);

                        if(OS.IsWindows)
                            SB.Append(".txt");

                        OutputPath = SB.ToString();

                        StringBuilderPool.Put(SB);

                        File.Create(OutputPath).Dispose();

                    }
                    else
                    {

                        if(!File.Exists(OutputPath))
                            File.Create(OutputPath).Dispose();

                    }

                }
                catch(Exception e)
                {

                    Write(e);

                    return;

                }

                IOProxy = new ConsoleFileInputOutputProxy(OutputPath);

            }
            else
            {

                IOProxy = new ConsoleInputOutputProxy();

            }

            if(!string.IsNullOrWhiteSpace(CmdInput))
            {

                try
                {

                    myExecutor.Open();

                    IOProxy.WriteConnectionIsOpen(ConnectionString);

                    IOProxy.WriteLine(CmdInput);

                    IOProxy.WriteDataReader(myExecutor.ExecuteReader(CmdInput));

                }
                catch(Exception e)
                {

                    IOProxy.WriteException(e);

                }
                finally
                {

                    myExecutor.Close();

                    IOProxy.WriteConnectionIsClosed();

                }

            }

            if(Quit)
                return;

            try
            {

                myExecutor.Open();

                IOProxy.WriteConnectionIsOpen(ConnectionString);
                
                //Write and execute CmdInput if it has a value.

                if(!string.IsNullOrWhiteSpace(CmdInput))
                {

                    IOProxy.WriteLine(CmdInput);

                    IOProxy.WriteDataReader(myExecutor.ExecuteReader(CmdInput));

                }

                //Start the loop

                while(true)
                {

                    CmdInput = IOProxy.ReadLine();

                    if(CmdInput == "q" || CmdInput == "Q")
                        break;

                    try
                    {

                        if(CmdInput.Length > 0)
                            IOProxy.WriteDataReader(myExecutor.ExecuteReader(CmdInput));
                        else
                            IOProxy.WriteLine();

                    }
                    catch(Exception e)
                    {

                        IOProxy.WriteException(e);

                        if(!myExecutor.ConnectionIsActive)
                            break;

                    }

                }

            }
            catch(Exception e)
            {

                IOProxy.WriteException(e);

            }
            finally
            {

                myExecutor.Close();

                IOProxy.WriteConnectionIsClosed();

            }

            IOProxy.WriteLine("bye!");

        }

        bool TryParseCommandLine(string[] args, out Dictionary<string, string> TheFoundArguments)
        {

            if(args.Length > 0)
            {

                List<string> Arguments = new List<string>();

                Arguments.Add("-con");

                Arguments.Add("-cmd");

                Arguments.Add("-out");

                TheFoundArguments = new Dictionary<string, string>();

                string CurrentArgument = null;

                for(int i = 0; i < args.Length; ++i)
                {

                    string Current = args[i];

                    if(Arguments.Contains(Current))
                    {

                        CurrentArgument = Current;

                        TheFoundArguments.Add(Current, "");

                        Arguments.Remove(Current);

                    }
                    else
                    {

                        if(string.IsNullOrWhiteSpace(CurrentArgument))
                            throw new Exception("Unrecognised argument: " + Current);

                        TheFoundArguments[CurrentArgument] = Current;

                    }

                }

                return true;

            }

            TheFoundArguments = null;

            return false;

        }

        void Write(Exception TheException)
        {

            Console.WriteLine(TheException.Message);

            Console.WriteLine();

            Console.WriteLine(TheException.StackTrace);

            Console.WriteLine();

            Console.ReadLine();

        }

    }

}
