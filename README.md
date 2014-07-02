dbrep is a framework for creating command line programmes for database interaction.

Depends on the CoreComponents project.

usage:

	using System;
	using CoreComponents.Data;
	using dbrep;
	
	namespace sqlitrep
	{
		
		class Program
		{
			
			static void Main(string[] args)
			{
				
				new CMDMain<IExecutorImpementer>().Run(args);
				
			}
			
		}
		
	}

Arguments:

-con - The connection string

-out - Log the output. Can be followed by the path of a file to use otherwise the file is created for use in the working directory.

-cmd - The provided command as text or as a file path.

-q - Quit straight away; does not enter the repl.



That's it.