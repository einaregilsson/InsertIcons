/*
MIT License

InsertIcons, a program to add multiple icons to .NET applications. 

Copyright (c) 2012 Einar Egilsson
http://einaregilsson.com/add-multiple-icons-to-a-dotnet-application/
 
This program includes other MIT licensed code from the following projects:

   * ResourceLib (https://github.com/dblock/resourcelib), to insert the icons.
   * Mono (http://mono-project.com), for strong name signing assemblies.

You may freely use and distribute this software under the terms of the following license agreement.

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated 
documentation files (the "Software"), to deal in the Software without restriction, including without limitation 
the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and 
to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of 
the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO 
THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, 
TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.CSharp;
using NUnit.Framework;
using InsertIconsProgram = InsertIcons.Program;

namespace InsertIcons.Tests
{
    [TestFixture]
    public class InsertIconsTest
    {
        [DllImport("mscoree.dll", CharSet = CharSet.Unicode)]
        private static extern bool StrongNameSignatureVerificationEx(string wszFilePath, bool fForceVerification, ref bool pfWasVerified);


        [Test]
        public void InsertIconsExplicitList()
        {
            string file = CompileAssembly();
            int result = InsertIconsProgram.Main(new[]
                                                     {
                                                         file,
                                                         @"Data\icons\cmd.ico;Data\icons\icon1.ico;Data\icons\sec.ico"
                                                     });
            Assert.AreEqual(0, result);
        }

        [Test]
        public void InsertIconsDirectoryOfIcons()
        {
            string file = CompileAssembly();
            int result = InsertIconsProgram.Main(new[]
                                                     {
                                                         file,
                                                         @"Data\icons"
                                                     });
            Assert.AreEqual(0, result);
        }

        [Test]
        public void InsertIconsFileWithIconNames()
        {
            string file = CompileAssembly();
            int result = InsertIconsProgram.Main(new[]
                                                     {
                                                         file,
                                                         @"Data\icons.txt"
                                                     });
            Assert.AreEqual(0, result);
        }

        [Test]
        public void InsertIconsFromStdInput()
        {
            string file = CompileAssembly();

            Console.SetIn(new StringReader("Data\\icons\\icon1.ico\r\nData\\icons\\sec.ico"));
            int result = InsertIconsProgram.Main(new[]
                                                     {
                                                         file
                                                     });
            Assert.AreEqual(0, result);
        }

        [Test]
        public void AssemblyThatWasntSignedBeforeThrowsErrorIfKeyIsPassedIn()
        {
            string file = CompileAssembly(false);

            bool wasVerified = false;
            bool isOK = StrongNameSignatureVerificationEx(file, true, ref wasVerified);
            Assert.IsFalse(wasVerified, "The file should not have been verified");
            Assert.IsFalse(isOK, "Should have returned false, because the file is not signed");

            Console.SetIn(new StringReader("Data\\icons\\icon1.ico\r\nData\\icons\\sec.ico"));
            int result = InsertIconsProgram.Main(new[]
                                                     {
                                                         file,
                                                         @"Data\icons\cmd.ico;Data\icons\icon1.ico;Data\icons\sec.ico",
                                                         @"Data\TestApp.snk"
                                                     });
            Assert.AreEqual(1, result);
        }

        [Test]
        public void ResignAssembliesWithStrongNameKey()
        {
            string file = CompileAssembly();

            bool wasVerified = false;
            bool isOK = StrongNameSignatureVerificationEx(file, true, ref wasVerified);
            Assert.IsTrue(wasVerified, "The file should have been verified");
            Assert.IsTrue(isOK, "Signature should be ok before adding icons");

            Console.SetIn(new StringReader("Data\\icons\\icon1.ico\r\nData\\icons\\sec.ico"));
            int result = InsertIconsProgram.Main(new[]
                                                     {
                                                         file,
                                                         @"Data\icons\cmd.ico;Data\icons\icon1.ico;Data\icons\sec.ico",
                                                         @"Data\TestApp.snk"
                                                     });
            Assert.AreEqual(0, result);

            isOK = StrongNameSignatureVerificationEx(file, true, ref wasVerified);
            Assert.IsTrue(wasVerified, "The file should have been verified");
            Assert.IsTrue(isOK, "Signature should be ok after adding icons");

        }

        private string CompileAssembly(bool sign=true)
        {
            string source = @"
public class Program {
    public static void Main(){
            System.Console.WriteLine(""My name is "" + System.Reflection.Assembly.GetExecutingAssembly().GetName());
        }
}
    ";
            string tempFile = Path.GetTempFileName() + ".exe";
            CSharpCodeProvider codeProvider = new CSharpCodeProvider();
            CompilerParameters parameters = new CompilerParameters();
            parameters.GenerateExecutable = true;
            parameters.OutputAssembly = tempFile;
            if (sign)
            {
                parameters.CompilerOptions = "/keyfile:Data\\Testapp.snk";
            }
            CompilerResults results = codeProvider.CompileAssemblyFromSource(parameters, source);
            Assert.AreEqual(0, results.Errors.Count);
            return tempFile;
        }
    }
}
