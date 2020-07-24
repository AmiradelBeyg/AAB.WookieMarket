using AutoMapper;
using Books.API.Entity;
using Books.API.Model;
using System;

namespace Book.UnitTest
{
    public class MappingProfileUnitTest : Profile
    {
        public MappingProfileUnitTest()
        {
            CreateMap<BookEntity, BookResponse>()
                    .ForMember(br => br.Id, opt => opt.MapFrom(be => be.Id))
                    .ForMember(br => br.Title, opt => opt.MapFrom(be => be.Title))
                    .ForMember(br => br.Author, opt => opt.MapFrom(be => be.Author))
                    .ForMember(br => br.Description, opt => opt.MapFrom(be => be.Description))
                    .ForMember(br => br.Price, opt => opt.MapFrom(be => be.Price))
                    .ForMember(br => br.CoverImageURL, opt => opt.MapFrom(be => be.CoverImageFileName));

            CreateMap<CreateBookRequest, BookEntity>()
                    .ForMember(be => be.Id, opt => opt.MapFrom(m => Guid.NewGuid()))
                    .ForMember(be => be.Title, opt => opt.MapFrom(m => m.Title))
                    .ForMember(be => be.Author, opt => opt.MapFrom(m => m.Author))
                    .ForMember(be => be.Description, opt => opt.MapFrom(m => m.Description))
                    .ForMember(be => be.Price, opt => opt.MapFrom(m => m.Price))
                    .ForMember(be => be.CoverImageFileName, opt => opt.MapFrom(m => m.CoverImage));
        }
    }
}
