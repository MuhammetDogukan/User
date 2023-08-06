namespace Application.MainHub
{
    public interface IMessageHub
    {
        
        Task SendMesssageToAdmins(string message);
        Task AddAdminGroup();
        Task SendMesssageToUser(string message, string userId);
        Task AddGroup(string name);
    }
}
