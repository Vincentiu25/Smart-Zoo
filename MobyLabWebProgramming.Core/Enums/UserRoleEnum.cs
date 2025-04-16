namespace MobyLabWebProgramming.Core.Enums;

/// <summary>
/// Enum for user roles, you can modify it however you see fit.
/// </summary>
public enum UserRoleEnum
{
    Admin, // all permissions + add users
    Personnel, // all permissions except for deleting animals, deleting employees, only viewing users
    Client // only view permissions, only self update for user
}