namespace Rentopolis.Repositories.Interfaces
{
    public interface ILandlordServices
    {
        Task<int> GetNumberOfOwnedPropertyStatus(string id, string neededStatus);
    }
}
