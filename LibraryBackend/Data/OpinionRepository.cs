using LibraryBackend.Models;

namespace LibraryBackend.Data
{
  public class OpinionRepository : Repository<Opinion>
  {
    private readonly MyLibraryContext _context =default!;

    public OpinionRepository(MyLibraryContext context) : base(context)
    {
      _context= context;
    }

    public virtual Opinion UpdateOpinion(Opinion opinion, string view, string userName, int like)
    {
      opinion.View = view;
      opinion.Like = like;
      opinion.userName = userName;

      var updatedOpinion= _context.Opinion.Update(opinion);
      _context.SaveChanges();

      return updatedOpinion.Entity;
    }
  }
}