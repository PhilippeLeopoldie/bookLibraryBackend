using LibraryBackend.Core.Contracts;
using LibraryBackend.Core.Entities;
using LibraryBackend.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryBackend.Infrastructure.Repositories;

public class OpinionRepository : RepositoryBase<Opinion>, IOpinionRepository
{
    public OpinionRepository(MyLibraryContext context) : base(context)
    {
    }
}
