#nullable enable
namespace UniT.Lifecycle
{
    public interface IFocusLostListener
    {
        public void OnFocusLost();
    }

    public interface IFocusGainListener
    {
        public void OnFocusGain();
    }

    public interface IPauseListener
    {
        public void OnPause();
    }

    public interface IResumeListener
    {
        public void OnResume();
    }
}