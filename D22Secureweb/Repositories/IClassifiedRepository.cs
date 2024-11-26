using D22Secureweb.Model;

namespace D22Secureweb.Repositories
{
    public interface IClassifiedRepository
    {
        Task Add(Classified cData);
        Task<bool> Delete(int classifiedID);
        Task<Classified?> Get(int ClassifiedID);
        Task<List<Classified>> GetAll();
    }
}
