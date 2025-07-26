using System;
using System.Collections.Generic;
using System.Text;

namespace Common
{
    public static class Constants
    {
        public const string ProfilePhotos = "ProfilePhotos";
    }

    public enum UserCategoryEnum
    {
        SuperAdmin = 1,
        Client = 2,
        Customer = 3,
        Consumer = 4
    }

    public enum UserRoleEnum
    {
        Admin = 1,
        CustomerAdmin = 2,
        ClientAdmin = 3,
        SubCustomer = 4
    }




    // Introduce because .net was converting JwtRegisteredClaimNames.sub to http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier
    public static class ClaimType
    {
        public const string Custom_Sub = "custom_sub";
        public const string CompanyId = "company_Id";
    }
}
