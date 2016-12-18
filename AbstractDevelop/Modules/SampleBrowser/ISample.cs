using Gemini.Framework.Services;

namespace AbstractDevelop.Modules.SampleBrowser
{
    public interface ISample
    {
        string Name { get; }
        void Activate(IShell shell);
    }
}