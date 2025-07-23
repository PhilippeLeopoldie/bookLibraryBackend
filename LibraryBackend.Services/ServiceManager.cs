using Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraryBackend.Services;

public class ServiceManager : IServiceManager
{
    private readonly Lazy<IBookService> _bookService;
    private readonly Lazy<IOpinionService> _opinionService;
    private readonly Lazy<IGenreService> _genreService;
    private readonly Lazy<IStoryService> _storyService;

    public IBookService BookService => _bookService.Value;
    public IOpinionService OpinionService => _opinionService.Value;
    public IGenreService GenreService => _genreService.Value;
    public IStoryService StoryService => _storyService.Value;

    public ServiceManager(
        Lazy<IBookService> bookService,
        Lazy<IOpinionService> opinionService,
        Lazy<IGenreService> genreService,
        Lazy<IStoryService> storyService
        )
    {
        _bookService = bookService;
        _opinionService = opinionService;
        _genreService = genreService;
        _storyService = storyService;
    }


    

   
}
