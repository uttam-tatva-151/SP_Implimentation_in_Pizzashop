namespace PMSCore.Beans
{

    //For type defination use <T>
    public class ResponseResult
    {
        public Object Data { get; set; } = new();
        public string Message { get; set; } = string.Empty;
        public ResponseStatus Status { get; set; }
    }
    public enum ResponseStatus
    {
        Error,
        NotFound,
        Success
    }

}
