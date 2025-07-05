public class RegisterResult
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public string? ErrorMessage { get; set; }

    public static RegisterResult Fail(string message)
        => new RegisterResult { Success = false, ErrorMessage = message };

    public static RegisterResult Ok(string token)
        => new RegisterResult { Success = true, Token = token };
}
