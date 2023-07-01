using LibraryBackend.Models;

namespace LibraryBackend.Data
{
  public interface IOpinionRepository : IRepository<Opinion>
  {
    Opinion UpdateOpinion(Opinion opinion, string view, string userName, int like);
  }
}