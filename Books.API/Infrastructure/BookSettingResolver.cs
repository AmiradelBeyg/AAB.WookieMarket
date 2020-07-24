using AutoMapper;
using Books.API.Entity;
using Books.API.Model;
using Microsoft.Extensions.Options;

namespace Books.API.Infrastructure
{
    public class BookSettingResolver : IValueResolver<BookEntity, BookResponse, string>
    {
        private IOptionsSnapshot<BookSettings> _bookSettings;

        public BookSettingResolver(IOptionsSnapshot<BookSettings> bookSettings)
        {
            _bookSettings = bookSettings;
        }

        public string Resolve(BookEntity source, BookResponse destination, string destMember, ResolutionContext context)
        {
            return string.IsNullOrEmpty(source.CoverImageFileName) ? null : _bookSettings.Value.CoverImageBaseUrl.Replace("[0]", source.Id.ToString());
        }
    }
}
