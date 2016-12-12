using System;
using System.ComponentModel.Composition;
using System.IO;

namespace AbstractDevelop.Storage.Formats
{
    [InheritedExport]
    public interface IDataFormatProvider :
        IDisposable
    {
        void Serialize<T>(T data, Stream target);

        T Deserialize<T>(Stream source);
    }

    [InheritedExport()]
    public interface IBinaryDataFormatProvider :
        IDataFormatProvider
    {
        bool PreferBinary { get; set; }
    }
}
