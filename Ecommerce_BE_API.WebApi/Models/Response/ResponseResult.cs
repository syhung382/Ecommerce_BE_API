using System;
using System.Collections.Generic;
using System.ComponentModel;
using Ecommerce_BE_API.DbContext.Models;

namespace Ecommerce_BE_API.WebApi.Models.Response
{
    public class ResponseResult<T> where T : class
    {
        public ResponseResult() { }

        public ResponseResult(RetCodeEnum retCode, string retText, T data)
        {
            this.RetCode = retCode;
            this.RetText = retText;
            switch (retCode)
            {
                case RetCodeEnum.ApiNoDelete:
                    this.RetText = "Cần phải xóa cấp con trước khi xóa.";
                    break;
                case RetCodeEnum.ApiNotRole:
                    this.RetText = "Bạn không có quyền.";
                    break;
            }
            this.Data = data;
        }

        public RetCodeEnum RetCode { get; set; }
        public string RetText { get; set; }
        public T Data { get; set; }

    }

    public enum RetCodeEnum
    {
        [Description("OK")]
        Ok = 0,
        [Description("Api Error")]
        ApiError = 1,
        [Description("Not Exists")]
        ResultNotExists = 2,
        [Description("Parammeters Invalid")]
        ParammetersInvalid = 3,
        [Description("Parammeters Not Found")]
        ParammetersNotFound = 4,
        [Description("Not delete")]
        ApiNoDelete = 5,
        [Description("Not Role")]
        ApiNotRole = 6
    }
    public class ApiSuccessResult<T> : ApiResult<T>
    {
        public ApiSuccessResult(T data)
        {
            RetCode = 0;
            RetText = "Oke";
            Data = data;
        }
        public ApiSuccessResult(T data, int retCode, string retText)
        {
            RetCode = retCode;
            RetText = retText;
            Data = data;
        }
        public ApiSuccessResult(int retCode, string retText)
        {
            RetCode = retCode;
            RetText = retText;
        }
    }

    public class ApiResult<T>
    {
        public int? RetCode { get; set; }

        public string RetText { get; set; }

        public T Data { get; set; }
    }
    public class ApiResults<T>
    {
        public int? RetCode { get; set; }

        public string RetText { get; set; }

        public List<T> Data { get; set; }
    }
    public class ApiResultPaging<T>
    {
        public int? RetCode { get; set; }

        public string RetText { get; set; }

        public T Data { get; set; }
    }
    public class ApiResultPagings<T>
    {
        public int? RetCode { get; set; }

        public string RetText { get; set; }
        public Paging paging { get; set; }

        public List<T> Data { get; set; }
    }
    public class RecordOfPage
    {
        public int TotalRow { get; set; }
    }
    public class ResStatus
    {
        public string Message { get; set; }
    }
}
