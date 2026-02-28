#nullable enable
namespace UniT.Lifecycle
{
    public interface IFocusChangedListener
    {
        public void OnFocusGain();

        public void OnFocusLost();
    }

    public interface IPauseChangedListener
    {
        public void OnPause();

        public void OnResume();
    }
}