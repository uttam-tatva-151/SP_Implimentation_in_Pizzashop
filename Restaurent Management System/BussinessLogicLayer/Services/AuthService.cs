using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using PMSCore.Beans;
using PMSCore.DTOs;
using PMSCore.DTOs.Configuration;
using PMSCore.ViewModel;
using PMSData;
using PMSData.Interfaces;
using PMSServices.Interfaces;


namespace PMSServices.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepo _dalAuthService;
        private readonly IUserRepo _userRepo;
        private readonly EmailSettings _emailSettings;
        private readonly IConfiguration _configuration;
        
        public AuthService(IAuthRepo dALAuthService,IOptions<EmailSettings> emailSettings, IConfiguration configuration, IUserRepo userRepo)
        {
            _dalAuthService = dALAuthService;
            _configuration = configuration;
            _userRepo = userRepo;
            _emailSettings = emailSettings.Value;
        }

        ResponseResult result = new();
        public async Task<ResponseResult> LoginUser(LoginRequest loginRequest)
        {
            try
            {

                Userauthentication? usrObj = await _userRepo.GetUserByEmail(loginRequest.EmailId);
                if (usrObj == null)
                {
                    result.Message = Constants.ERROR_LOGIN;
                    result.Status = ResponseStatus.Error;
                }
                else
                {

                    if (BCrypt.Net.BCrypt.Verify(loginRequest.Password, usrObj.PasswordHash))
                    {
                        result.Message = Constants.SUCCESS_LOGIN;
                        result.Status = ResponseStatus.Success;
                    }
                    else
                    {
                        result.Message = Constants.ERROR_PASSWORD_MISMATCH;
                        result.Status = ResponseStatus.Error;
                    }
                    UserDto user = new()
                    {
                        UserId = usrObj.UserId,
                        Email = usrObj.EmailId,
                        UserName = usrObj.UserName
                    };
                    result.Data = user;
                }

            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }

            return result;
        }
        public async Task<ResponseResult> SendForgotPassLink(string EmailId)
        {
            try
            {
                Userauthentication? usrObj = await _userRepo.GetUserByEmail(EmailId);
                if (usrObj == null)
                {
                    result.Message = Constants.WARNING_EMAIL_NOT_FOUND;
                    result.Status = ResponseStatus.NotFound;
                }
                else
                {
                    if (await HelperToSend(EmailId))
                    {
                        result.Message = Constants.SUCCESS_FORGOT_PASSWORD;
                        result.Status = ResponseStatus.Success;
                    }
                    else
                    {
                        result.Message = Constants.ERROR_FORGOT_PASSWORD;
                        result.Status = ResponseStatus.Error;
                    }
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        public async Task<ResponseResult> ResetPassword(UpdatePassword updatePassword)
        {
            result = await _dalAuthService.CheckResetToken(updatePassword);

            if (result.Data != null)
            {
                ResetPasswordToken tokenAvail = (ResetPasswordToken)result.Data;
                TimeSpan linkLifeTime = TimeOnly.FromDateTime(DateTime.Now) - TimeOnly.FromDateTime(tokenAvail.CreateAt);
                if (linkLifeTime.Minutes < 30)
                {
                    if (updatePassword.Password == updatePassword.confirmPassword)
                    {
                        updatePassword.Password = BCrypt.Net.BCrypt.HashPassword(updatePassword.Password); 
                        if (await _dalAuthService.UpdatePassword(updatePassword))
                        {
                            result.Message = MessageHelper.GetSuccessMessageForUpdateOperation(Constants.PASSWORD);
                            result.Status = ResponseStatus.Success;
                        }
                        else
                        {
                            result.Message = MessageHelper.GetErrorMessageForUpdateOperation(Constants.PASSWORD);
                            result.Status = ResponseStatus.Error;
                        }
                    }
                    else
                    {
                        result.Message = Constants.ERROR_PASSWORD_MISMATCH;
                        result.Status = ResponseStatus.Error;
                    }

                }
                else
                {
                    result.Message = Constants.WARNING_RESET_TOKEN_EXPIRED;
                    result.Status = ResponseStatus.NotFound;
                }
            }
            else
            {
                result.Message = MessageHelper.GetInfoMessageForNoRecordsFound(Constants.RESET_PASSWORD_TOKEN);
                result.Status = ResponseStatus.Error;
            }
            return result;
        }
        private async Task<bool> HelperToSend(string emailId)
        {
            string token = await GenerateResetToken(emailId);
            string resetLink = $"{_configuration["AppUrl"]}/ResetPassword?token={token}";
            string emailBody = await GetEmailBodyAsync(Constants.FORGOT_PASSWORD_FILE);
            emailBody = emailBody.Replace("{{reset_link}}", resetLink);
            string subject = Constants.EMAIL_SUBJECT_FORGOT_PASSWORD;
            return await SendEmailAsync(emailId, subject, emailBody);
        }
        private async Task<string> GenerateResetToken(string emailId)
        {
            string token;
            using (RandomNumberGenerator randomnumbergenerator = RandomNumberGenerator.Create())
            {
                byte[] tokenBytes = new byte[32];
                randomnumbergenerator.GetBytes(tokenBytes);
                token = Convert.ToBase64String(tokenBytes)
                            .Replace("+", "-") // URL-safe Base64
                            .Replace("/", "_")
                            .Replace("=", ""); // Remove padding
            }

            if (token != null)
            {
                bool isSaved = await IsSaveResetToken(emailId, token);
                if (isSaved)
                {
                    return token;
                }
                else
                {
                    return MessageHelper.GetErrorMessageForAddOperation(Constants.RESET_PASSWORD_TOKEN);
                }

            }
            return token ?? string.Empty;
        }

        private async Task<bool> IsSaveResetToken(string emailId, string token)
        {
            try
            {
                Userauthentication? usrObj = await _userRepo.GetUserByEmail(emailId);
                if (usrObj != null)
                {
                    ResetPasswordToken newTokenRow = new()
                    {
                        ResetToken = token,
                        UserId = usrObj.UserId,
                        CreateAt = DateTime.Now,
                        IsContinue = true
                    };
                    result = await _dalAuthService.AddResetToken(newTokenRow);
                    if (result.Status == ResponseStatus.Success)
                    {
                        return true; // Token saved successfully
                    }
                    else
                    {
                        return false; // Failed to save token
                    }
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public async Task<string> GetEmailBodyAsync(string templateName)
        {
            string templatePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Templates", templateName);
            return await File.ReadAllTextAsync(templatePath);
        }
        public async Task<bool> SendEmailAsync(string emailId, string subject, string emailBody)
        {

            try
            {
                MailMessage mail = new()
                {
                    From = new MailAddress(_emailSettings.SenderEmail),
                    Subject = subject,
                    Body = emailBody,
                    IsBodyHtml = true
                };

                SmtpClient client = new(_emailSettings.SmtpServer)
                {
                    Port = _emailSettings.SmtpPort,
                    Credentials = new NetworkCredential(_emailSettings.SenderEmail, _emailSettings.SenderPassword),
                    EnableSsl = true,
                    UseDefaultCredentials = false
                };

                mail.To.Add(emailId);
                await client.SendMailAsync(mail);

                return true; // Email sent successfully
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                return false; // Email failed to send
            }
        }
    }


}
