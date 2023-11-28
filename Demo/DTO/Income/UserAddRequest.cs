﻿using Demo.Entities;

namespace Demo.DTO.Income;

public record UserAddRequest(string? UserName, string? FirstName, string? LastName, string? Email, string? Phone)
{
    public User ToEntity(Guid requestId)
    {
        return new()
        {
            CreateRequestId = requestId,
            UserName = UserName,
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            Phone = Phone,
        };
    }
}
