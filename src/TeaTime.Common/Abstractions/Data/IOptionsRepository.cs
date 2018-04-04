﻿namespace TeaTime.Common.Abstractions.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Models.Data;

    public interface IOptionsRepository
    {
        Task CreateGroupAsync(RoomItemGroup group);
        Task<RoomItemGroup> GetGroupAsync(long groupId);
        Task<RoomItemGroup> GetGroupByNameAsync(long roomId, string name);

        Task CreateAsync(Option option);
        Task<IEnumerable<Option>> GetOptionsByGroupIdAsync(long groupId);
    }
}
