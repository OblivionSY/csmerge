﻿using System.IO;
using System.Text;

using Cpc.CsMerge.Core;

using NUnit.Framework;

namespace PackagesMerge.Test {
  [TestFixture]
  public class PackageParseTest {

    public class Utf8StringWriter: StringWriter {
      public override Encoding Encoding {
        get { return Encoding.UTF8; }
      }
    }

    [Test]
    public void Test() {
      var original = File.ReadAllText( @"..\..\packages.config" ).Replace( "\r", "" );

      StringReader reader = new StringReader( original );

      var packages = Package.Read( reader );

      StringWriter writer = new Utf8StringWriter();

      Package.Write( packages, writer );

      var written = writer.ToString().Replace( "\r", "" );

      Assert.That( written, Is.EqualTo( original ) );
    }
  }
}