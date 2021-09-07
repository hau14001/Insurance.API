using System;
using System.Collections.Generic;
using System.Linq;

namespace Insurance.Infrastructure.Data
{
    public class PagedResult<T>
    {
        public List<T> Items { get; set; }

        public int PageIndex { get; set; }

        public int PageSize { get; set; } = 10;

        public int TotalCount { get; set; }

        public int TotalPage => (int)Math.Ceiling(TotalCount / (double)PageSize);

        public PagedResult<TDto> ChangeType<TDto>(Func<T, TDto> cast) => new PagedResult<TDto>()
        {
            Items = Items.Select(cast).ToList(),
            PageIndex = PageIndex,
            PageSize = PageSize,
            TotalCount = TotalCount
        };

        public PagedResult<TDto> ChangeData<TDto>(List<TDto> data) => new()
        {
            Items = data,
            PageIndex = PageIndex,
            PageSize = PageSize,
            TotalCount = TotalCount
        };
    }
}