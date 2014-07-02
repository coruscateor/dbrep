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

Thats it.