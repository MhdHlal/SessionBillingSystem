using System.Collections.Generic;

namespace EducationalCenter.API.Models
{
  public sealed class ApiResponse<T>
  {
    public bool Success { get; set; }
    public T? Data { get; set; } // إضافة ? للسماح بالقيم الفارغة في حالة الخطأ
    public List<ApiError> Errors { get; set; } = new List<ApiError>();

    public static ApiResponse<T> ToSuccess(T data)
    {
      return new ApiResponse<T> { Success = true, Data = data };
    }

    public static ApiResponse<T> ToError(string code, string message)
    {
      return new ApiResponse<T>
      {
        Success = false,
        Data = default!, // استخدام default! لتجنب تحذير CS8601
        Errors = new List<ApiError> { new ApiError { Code = code, Message = message } }
      };
    }
  }

  public sealed class ApiError
  {
    public string? Code { get; set; } // إضافة ?
    public string? Message { get; set; } // إضافة ?
  }
}
