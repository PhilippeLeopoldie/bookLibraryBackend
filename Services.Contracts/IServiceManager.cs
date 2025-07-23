using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Contracts;

public interface IServiceManager
{
    IBookService BookService { get; }
    IOpinionService OpinionService { get; }
    IGenreService GenreService { get; }
    IStoryService StoryService { get; }
}
