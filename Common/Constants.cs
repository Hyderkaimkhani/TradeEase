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

    public static class WialonProperties
    {
        public const string AvailableUnit = "avl_unit";
        public const string PhoneNumber = "sys_phone_number";
        public const string UniqueId = "sys_unique_id";
    }

    public static class Flags
    {
        public const string Position = "4194304";
        public const string Address = "1255211008";
        public const string UniqueId = "sys_unique_id";
    }

    public static class ServiceURLConstants
    {
        public const string WialonLogin = "token/login&params=";
        public const string LoadIneterval = "messages/load_interval&params=";
        public const string GetTrips = "unit/get_trips&params=";
        public const string GetCoordinates = "/gis_geocode?coords="; 
        public const string SearchItemsParams = "core/search_items&params=";
        public const string SetSpeedLimit = "unit/update_report_settings&params=";
        public const string GetSpeedLimit = "unit/get_report_settings&params=";
        public const string AddUpdateGeofence = "resource/update_zone&params=";
    }

    // Introduce because .net was converting JwtRegisteredClaimNames.sub to http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier
    public static class ClaimType
    {
        public const string Custom_Sub = "custom_sub";
    }
}
