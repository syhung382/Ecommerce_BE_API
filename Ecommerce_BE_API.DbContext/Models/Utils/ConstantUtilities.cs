using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce_BE_API.DbContext.Models.Utils
{
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

    public enum GenderEnum
    {
        [Description("Nam")]
        Male = 1,
        [Description("Nữ")]
        Female = 2,
        [Description("Khác")]
        Other = 3
    }
    
    public enum UserRoleEnum
    {
        [Description("Người dùng")]
        User = 1,
        [Description("Quản trị viên")]
        Staff = 2,
    }

    public enum AdminRoleEnum
    {
        [Description("Staff")]
        Staff = 3,
        [Description("Moderator")]
        Moderator = 4,
        [Description("Admin")]
        Admin = 5,
        [Description("Super Admin")]
        SuperAdmin = 6,
    }

    public enum BannedEnum
    {
        [Description("Không")]
        No = 0,
        [Description("Banned")]
        Yes = 1,
    }

    public enum ActiveEnum
    {
        [Description("Chưa kích hoạt")]
        No = 0,
        [Description("Đã kích hoạt")]
        Yes = 1,
    }

    public enum UserStatusEnum
    {
        [Description("Hoạt động")]
        Active = 0,
        [Description("Tạm xóa")]
        TemporarilyDeleted = 1,
    }

    public enum StatusEnum
    {
        [Description("Kích hoạt")]
        Active = 0,
        [Description("Chưa kích hoạt")]
        InActive= 1,
    }

    public enum IsFirstLoginEnum
    {
        [Description("Chưa từng đăng nhập")]
        FirstLoggedIn = 0,
        [Description("Đã từng đăng nhập")]
        AlreadyLoggedIn = 1,
    }

    public enum ErrorCategoryCode
    {
        Success = 0,
        ItemNotFound = 1,
        ParentNotFound = 2,
        InvalidStatus = 3,
        TitleEmpty = 4,
        HasChildCategory = 5,
        HasRelatedProduct = 6
    }
    public enum ErrorProductCode
    {
        Success = 0,
        ItemNotFound = 1,
        CategoryNotFound = 2,
        InvalidStatus = 3,
        TitleEmpty = 4,
        PriceInvalid = 5,
        PriceSaleInvalid = 6,
    }
    public enum ErrorTypeOfProductCode
    {
        Success = 0,
        ItemNotFound = 1,
        InvalidStatus = 2,
        TitleEmpty = 3,
    }
}
