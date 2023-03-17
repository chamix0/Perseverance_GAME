namespace Player.Observer_pattern
{
    public interface IObserver
    {
        public void OnNotify(PlayerActions playerAction);
    }
}