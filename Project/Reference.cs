using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml.Linq;

using Mono.Cecil;

namespace Project {
  public class Reference: Item {
    private XElement _originalItemElement;

    public string Include { get; private set; }

    public Version ReferenceAssemblyVersion {
      get; private set;
    }

    public Reference CloneWith( string include = null, Version version = null ) {
      Reference copy = (Reference) MemberwiseClone();
      if ( include != null ) {
        copy.Include = include;
        copy._originalItemElement = null;
      }
      if ( version != null ) {
        copy.ReferenceAssemblyVersion = version;
        copy._originalItemElement = null;
      }
      return copy;
    }

    public string ReferenceAssemblyName {
      get; private set;
    }

    public AssemblyName GetAssemblyName() {
      return new AssemblyName( Include );
    }

    public bool? SpecificVersion { get; private set; }
    public string HintPath { get; private set; }
    public bool? Private { get; private set; }
    public string RequiredTargetFramework { get; private set; }
    public string Name { get; private set; }
    public bool? EmbedInteropTypes { get; private set; }
    public string Aliases { get; private set; }
    public string FusionName { get; private set; }

    public override string Key {
      get { return ReferenceAssemblyName; }
    }

    public override string ToString() {

      var propertyNames = new List<string>();

      propertyNames.Add( Include );
      propertyNames.AddPropertyIfNotNull( HintPath, "HintPath" );
      propertyNames.AddPropertyIfNotNull( Private, "Private" );
      propertyNames.AddPropertyIfNotNull( SpecificVersion, "SpecificVersion" );
      propertyNames.AddPropertyIfNotNull( RequiredTargetFramework, "RequiredTargetFramework" );

      return string.Join( Environment.NewLine, propertyNames );
    }

    public override bool Equals( Item other ) {
      return Equals( (object) other );
    }

    public override XElement ToElement( XNamespace ns ) {
      if ( _originalItemElement != null ) {
        return _originalItemElement;
      }

      XElement e = new XElement( ns.GetName( Action ) );

      e.Add( new XAttribute( "Include", Include ) );
      AddElement( e, "Name", Name );
      AddElement( e, "FusionName", FusionName );
      AddElement( e, "HintPath", HintPath );
      AddElement( e, "Private", Private );
      AddElement( e, "EmbedInteropTypes", EmbedInteropTypes );
      AddElement( e, "Aliases", Aliases );
      AddElement( e, "SpecificVersion", SpecificVersion );
      AddElement( e, "RequiredTargetFramework", RequiredTargetFramework );
      return e;
    }

    private static void AddElement( XElement parent, string elementName, object value ) {
      if ( value != null ) {
        parent.Add( new XElement( parent.Name.Namespace.GetName( elementName ), value ) );
      }
    }

    public bool Equals( Reference other ) {
      if ( ReferenceEquals( null, other ) ) {
        return false;
      }
      if ( ReferenceEquals( this, other ) ) {
        return true;
      }
      return string.Equals( HintPath, other.HintPath ) &&
        SpecificVersion == other.SpecificVersion &&
        string.Equals( Include, other.Include );
    }

    public override bool Equals( object obj ) {
      if ( ReferenceEquals( null, obj ) ) {
        return false;
      }
      if ( ReferenceEquals( this, obj ) ) {
        return true;
      }
      return obj.GetType() == GetType() && Equals( (Reference) obj );
    }

    public override int GetHashCode() {
      unchecked {
        var hashCode = HintPath != null ? HintPath.GetHashCode() : 0;
        hashCode = ( hashCode * 397 ) ^ SpecificVersion.GetHashCode();
        hashCode = ( hashCode * 397 ) ^ ( Include != null ? Include.GetHashCode() : 0 );
        return hashCode;
      }
    }

    public Reference( XElement originalItemElement ) {
      _originalItemElement = originalItemElement;
      Include = originalItemElement.Attribute( "Include" ).Value;

      try {
        AssemblyNameReference reference = AssemblyNameReference.Parse( Include );
        ReferenceAssemblyVersion = reference.Version;
        ReferenceAssemblyName = reference.FullName;
      }
      catch ( Exception ) {
        // Sometimes we cannot parse the full name due say $(MyVersion) in the project file
        // so for now we use this hack.
        ReferenceAssemblyName = Include.Split( new[] { ',' }, StringSplitOptions.RemoveEmptyEntries )[0];
        ReferenceAssemblyVersion = null;
      }

      Name = originalItemElement.SameNsElement( "Name" ).GetValueOrNull();
      EmbedInteropTypes = originalItemElement.SameNsElement( "EmbedInteropTypes" ).GetBoolOrNull();
      Aliases = originalItemElement.SameNsElement( "Aliases" ).GetValueOrNull();
      FusionName = originalItemElement.SameNsElement( "FusionName" ).GetValueOrNull();
      RequiredTargetFramework = originalItemElement.SameNsElement( "RequiredTargetFramework" ).GetValueOrNull();
      Private = originalItemElement.SameNsElement( "Private" ).GetBoolOrNull();
      SpecificVersion = originalItemElement.SameNsElement( "SpecificVersion" ).GetBoolOrNull();
      HintPath = originalItemElement.SameNsElement( "HintPath" ).GetValueOrNull();
    }

    public Reference( string include, bool? specificVersion, bool? @private, string hintPath ) {
      Include = include;
      SpecificVersion = specificVersion;
      Private = @private;
      HintPath = hintPath;
    }

    private Reference() { }

    public Reference Clone() {
      return new Reference {
        Include = Include,
        SpecificVersion = SpecificVersion,
        Private = Private,
        HintPath = HintPath,
        Aliases = Aliases,
        EmbedInteropTypes = EmbedInteropTypes,
        FusionName = FusionName,
        Name = Name,
        RequiredTargetFramework = RequiredTargetFramework,
      };
    }
  }
}