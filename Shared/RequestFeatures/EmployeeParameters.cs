﻿namespace Shared.RequestFeatures
{
    public class EmployeeParameters : RequestParameters
    {
        public EmployeeParameters() => OrderBy = "name";

        public uint MinAge { get; set; }

        public uint MaxAge { get; set; } = 100;

        public bool ValidAgeRange => MinAge < MaxAge;

        public string? SearchTerm { get; set; }

        public string? OrderBy { get; set; }
    }
}
