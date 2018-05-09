using System;
using System.Collections.Generic;

using Leger;

namespace Leger.IO
{
    public partial class GraphXmlDeserializer
    {
        #region Nested classes declaration
        class HeaderInfo
        {
            internal Version Version { get; set; }
            internal int Types { get; set; }
            internal int Nodes { get; set; }
        }

        struct Version
        {
            public int Major;
            public int Minor;
        }

        enum FileLocation
        {
            Local,
            Remote
        }

        class ExternalFileInfo
        {
            internal FileLocation Location { get; set; }
            internal string Path { get; set; }
            internal bool Processed { get; set; } // non présent dans la déclaration d'un fichier. sert à l'algorithme de chargement
        }

        class VertexInfo
        {
            internal int Type;
            internal int InternalId;
            internal Guid PublicId;
            internal string CanonicalName;
            internal string Content;
            internal string Language; // v2.3
            internal GraphObjectTypeInfo GuidType; // n'est pas rempli directement
        }

        class EdgeInfo
        {
            internal int Type;
            internal bool Oriented;
            internal Guid PublicId;
            internal bool ContainsExternal;
            internal List<IntOrGuid> LinkedObjects = new List<IntOrGuid>();
            internal GraphObjectTypeInfo GuidType; // ne se retrouve pas directement dans le fichier, est fixé après la lecture de la déclaration grâce à la table des types
        }

        class IntOrGuid
        {
            internal bool IsInt;
            internal int IntValue;
            internal Guid GuidValue;
        }

        class AnnotationInfo
        {
            internal string Namespace;
            internal string Key;
            internal string Value;
            internal IntOrGuid Target;

            internal Annotation ToAnnotation()
            {
                return new Annotation() { Namespace = Namespace, Key = Key, Value = Value };
            }
        }
        #endregion
    }
}