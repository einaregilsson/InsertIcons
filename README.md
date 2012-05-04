InsertIcons
-----------

A simple console program to add multiple icons to a .NET application. To learn more about why you might need this and for a more detailed description of the program see the blog post at http://einaregilsson.com/add-multiple-icons-to-a-dotnet-application/.

The program is licensed under the MIT license. It's heavily based on MIT licensed code from the great library ResourceLib, which I highly recommend and you can find at https://github.com/dblock/resourcelib. It also uses MIT licensed code from the Mono project (http://mono-project.com) for strong name signing.

The program takes three parameters. Below is the help output from the program to show you what it can do:


    InsertIcons  Copyright (C) 2012 Einar Egilsson
    
    See http://einaregilsson.com/add-multiple-icons-to-a-dotnet-application/ 
    for more information about this program and how to use it.
    
    Usage: InsertIcons <assemblyfile> <icons> [<keyfile>]
    
    <assemblyfile>    A .NET assembly (or any PE file really) that you want
                      to add icons to.
    
    <icons>           The <icons> parameter can accept 4 different types
                      of arguments:
                      
                      1. It can be a list of .ico files seperated by ;, e.g.
                         icon1.ico;icon2.ico;icon3.ico.
                         
                      2. It can be a directory name, in which case all .ico
                         files in the directory will be added, in 
                         alphabetical order.
    
                      3. It can be the name of a text file which contains a
                         list of icons to add to the assembly. The file
                         should simply have one .ico file path on each line.
       
                      4. It can be left out completely, in which case the
                         program reads filenames from the standard input
                         stream. This allows for filenames to be piped into
                         the program, e.g. 
                              
                             dir /b /s *.ico | InsertIcons myfile.exe
    
    <keyfile>         This parameter is optional. If it is included then the
                      assembly will be re-signed with a strong name after 
                      inserting the icons.
                      
                      Note that this should only be used to re-sign
                      assemblies that were signed before inserting icons
                      into them. If you pass in the <keyfile> parameter for
                      a file that was not signed before then the program 
                      will exit with an error message.
