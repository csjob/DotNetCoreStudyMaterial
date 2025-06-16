namespace DotNetCoreWebAPI.DI
{
    public class MessageService : IMessageService
    {
        public string GetMessage() => "Hello from DI!";
    }
}
