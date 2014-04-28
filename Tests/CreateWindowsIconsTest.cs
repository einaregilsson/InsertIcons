/*
MIT License

CreateWindowsIcons, a program to add multiple icons to .NET applications. 

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
using System.Runtime.InteropServices;
using NUnit.Framework;

namespace CreateWindowsIcons.Tests
{
    [TestFixture]
    public class CreateWindowsIconsTest
    {
        [DllImport("mscoree.dll", CharSet = CharSet.Unicode)]
        private static extern bool StrongNameSignatureVerificationEx(string wszFilePath, bool fForceVerification,
            ref bool pfWasVerified);

        [Test]
        public void AssemblyThatWasntSignedGetsIconsCreatedCorrectly()
        {
            string file = "Test.UnsignedAssembly.dll";
            var creator = new WindowsIconsCreator();

            Assert.AreEqual(0, creator.GetIconCount(file));

            creator.CreateIcons(file, null);

            Assert.AreEqual(3, creator.GetIconCount(file));
        }

        [Test]
        public void AssemblyThatWasSignedGetsIconsCreatedCorrectlyButSignatureIsRuined()
        {
            string file = "Test.SignedAssembly.dll";
            var creator = new WindowsIconsCreator();

            Assert.AreEqual(0, creator.GetIconCount(file));

            creator.CreateIcons(file, null);
            bool wasVerified = false;
            bool isOK = StrongNameSignatureVerificationEx(file, true, ref wasVerified);

            Assert.AreEqual(3, creator.GetIconCount(file));
            Assert.IsFalse(wasVerified, "The file should not have been verified");
            Assert.IsFalse(isOK, "Should have returned false, because the file is not signed");
        }

        [Test]
        public void AssemblyThatWasntSignedBeforeThrowsErrorIfKeyIsPassedIn()
        {
            string file = "Test.UnsignedAssembly.dll";

            bool wasVerified = false;
            bool isOK = StrongNameSignatureVerificationEx(file, true, ref wasVerified);
            Assert.IsFalse(wasVerified, "The file should not have been verified");
            Assert.IsFalse(isOK, "Should have returned false, because the file is not signed");

            Assert.Throws<ArgumentException>( () => new WindowsIconsCreator().CreateIcons(file, "Test.snk"));
        }

        [Test]
        public void ResignAssembliesWithStrongNameKey()
        {
            string file = "Test.SignedAssembly.dll";

            bool wasVerified = false;
            bool isOK = StrongNameSignatureVerificationEx(file, true, ref wasVerified);
            Assert.IsTrue(wasVerified, "The file should have been verified");
            Assert.IsTrue(isOK, "Signature should be ok before adding icons");
            var creator = new WindowsIconsCreator();

            creator.CreateIcons(file, "Test.snk");

            isOK = StrongNameSignatureVerificationEx(file, true, ref wasVerified);
            Assert.IsTrue(wasVerified, "The file should have been verified");
            Assert.IsTrue(isOK, "Signature should be ok after adding icons");
            Assert.AreEqual(3, creator.GetIconCount(file));

        }
    }
}
