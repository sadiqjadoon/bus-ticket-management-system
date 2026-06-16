-- =====================================================
-- STORED PROCEDURE: Get Users By Role
-- =====================================================

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetUsersByRole')
DROP PROCEDURE sp_GetUsersByRole;
GO

CREATE PROCEDURE sp_GetUsersByRole
    @RoleName NVARCHAR(256),
    @PageNumber INT = 1,
    @PageSize INT = 10
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        u.Id, u.UserName, u.Email, u.FirstName, u.LastName, 
        u.IsActive, u.CreatedAt, u.LastLoginAt
    FROM AspNetUsers u
    INNER JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    INNER JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE r.Name = @RoleName
    ORDER BY u.CreatedAt DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

-- =====================================================
-- STORED PROCEDURE: Get All Users with Pagination
-- =====================================================

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetAllUsers')
DROP PROCEDURE sp_GetAllUsers;
GO

CREATE PROCEDURE sp_GetAllUsers
    @PageNumber INT = 1,
    @PageSize INT = 10,
    @SearchTerm NVARCHAR(256) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    
    DECLARE @Offset INT = (@PageNumber - 1) * @PageSize;
    
    SELECT 
        u.Id, u.UserName, u.Email, u.FirstName, u.LastName, 
        u.IsActive, u.CreatedAt, u.LastLoginAt,
        STRING_AGG(r.Name, ', ') AS Roles
    FROM AspNetUsers u
    LEFT JOIN AspNetUserRoles ur ON u.Id = ur.UserId
    LEFT JOIN AspNetRoles r ON ur.RoleId = r.Id
    WHERE (@SearchTerm IS NULL OR u.UserName LIKE '%' + @SearchTerm + '%' 
           OR u.Email LIKE '%' + @SearchTerm + '%'
           OR u.FirstName LIKE '%' + @SearchTerm + '%')
    GROUP BY u.Id, u.UserName, u.Email, u.FirstName, u.LastName, u.IsActive, u.CreatedAt, u.LastLoginAt
    ORDER BY u.CreatedAt DESC
    OFFSET @Offset ROWS
    FETCH NEXT @PageSize ROWS ONLY;
END;
GO

PRINT 'User stored procedures created successfully!';
GO
