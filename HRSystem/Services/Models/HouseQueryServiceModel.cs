﻿namespace HRSystem.Services.Models
{
    public class HouseQueryServiceModel
    {
        public int TotalHousesCount { get; set; }
        public IEnumerable<HouseServiceModel> Houses { get; set; } = new List<HouseServiceModel>();
    }
}
