namespace HellTrail.Core.DialogueSystem
{
    public class Response
    {
        public string name;

        public ResponseDelegate OnUseResponse;

        public void UseResponse(Dialogue page)
        {
            OnUseResponse?.Invoke(page);
        }
    }

    public delegate void ResponseDelegate(Dialogue page);
}
