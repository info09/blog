using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace BlogCMS.Core.Models.Content.Series
{
    public class SeriesDto : SeriesInListDto
    {
        [MaxLength(250)]
        public string? SeoDescription { get; set; }

        [MaxLength(250)]
        public string? Thumbnail { set; get; }

        public string? Content { get; set; }

        public class AutoMapperProfiles : Profile
        {
            public AutoMapperProfiles()
            {
                CreateMap<Domain.Content.Series, SeriesDto>();
            }
        }
    }
}
