InsertIcons
-----------

A simple console program to add multiple icons to a .NET application. To learn more about why you might need this and for a more detailed description of the program see the blog post at http://einaregilsson.com/add-multiple-icons-to-a-dotnet-application/.

The program is licensed under the MIT license. It's heavily based on MIT licensed code from the great library ResourceLib, which I highly recommend and you can find at https://github.com/dblock/resourcelib. It also uses MIT licensed code from the Mono project (http://mono-project.com) for strong name signing.

Below is the help output from the program to show you what it can do:
	
	CreateWindowsIcons  Copyright (C) 2012 Einar Egilsson

	See http://einaregilsson.com/add-multiple-icons-to-a-dotnet-application/ 
	for more information about this program and how to use it.

	Usage: CreateWindowsIcons <assemblyfile> [<keyfile>]

	<assemblyfile>    A .NET assembly (.exe or .dll) that you want to add
					  Windows icons to. Any .ico files that have been embedded
					  as managed resources to the assembly will be converted
					  to Windows icons that can be used for shortcuts, jump
					  lists and other things.

	<keyfile>         This parameter is optional. If it is included then the
					  assembly will be re-signed with a strong name after 
					  inserting the icons.
                  
					  Note that this should only be used to re-sign
					  assemblies that were signed before inserting icons
					  into them. If you pass in the <keyfile> parameter for
					  a file that was not signed before then the program 
					  will exit with an error message.