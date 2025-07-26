namespace Common.Interfaces
{
    public interface ICurrentUserService
    {
        string GetCurrentUsername();

        int GetCurrentCompanyId();
    }

}
