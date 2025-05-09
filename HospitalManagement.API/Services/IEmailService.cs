namespace HospitalManagement.API.Services
{
    public interface IEmailService
    {
        void SendEmail(string toEmail, string subject, string body);
    }
}
