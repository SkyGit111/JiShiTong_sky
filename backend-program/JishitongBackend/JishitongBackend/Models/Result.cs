namespace JishitongBackend.Models
{
    public class Result<T>
    {
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }

        public int? Code { get; set; }

        public static Result<T> Success(T data, string message = "")
        {
            return new Result<T> { IsSuccess = true, Data = data, Message = message };
        }

        public static Result<T> Failure(string message)
        {
            return new Result<T> { IsSuccess = false, Message = message };
        }

        public static Result<T> Failure(int code,string message)
        {
            return new Result<T> { IsSuccess = false,Code=code, Message = message };
        }
    }

}
