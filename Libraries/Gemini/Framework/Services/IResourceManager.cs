using System.IO;
using System.Reflection;
using System.Windows.Media.Imaging;

namespace Gemini.Framework.Services
{
	public interface IResourceManager
	{
		Stream GetStream(string relativeUri, string assemblyName);

        BitmapImage GetBitmap(string relativeUri, Assembly assembly);

        BitmapImage GetBitmap(string relativeUri, string assemblyName);
		BitmapImage GetBitmap(string relativeUri);
	}
}