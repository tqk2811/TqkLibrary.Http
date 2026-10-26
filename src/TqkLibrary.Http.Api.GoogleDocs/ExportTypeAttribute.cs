using System;

namespace TqkLibrary.Http.Api.GoogleDocs
{
    public class ExportTypeAttribute : Attribute
    {
        public ExportTypeAttribute(string typeName)
        {
            if (string.IsNullOrWhiteSpace(typeName)) throw new ArgumentNullException(nameof(typeName));
            TypeName = typeName;
        }
        public string TypeName { get; }
    }
}
