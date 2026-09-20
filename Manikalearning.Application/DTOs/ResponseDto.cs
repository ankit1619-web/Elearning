using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manikalearning.Application.DTOs
{
    public class ResponseDto<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
         
        public static ResponseDto<T> Success(T data, string message = "Operation completed successfully.")
        {
            return new ResponseDto<T>
            {
                IsSuccess = true,
                Data = data,
                Message = message
            };
        }
         
        public static ResponseDto<T> Failure(string message, List<string> errors = null)
        {
            return new ResponseDto<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors ?? new List<string>()
            };
        }
    }
}
