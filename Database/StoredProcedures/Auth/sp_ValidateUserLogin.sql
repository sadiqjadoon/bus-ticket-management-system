-- =====================================================
-- STORED PROCEDURE: Validate User Login
-- =====================================================
-- Purpose: Validate user credentials and return user details
-- Parameters: @UserName, @Output user details

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_ValidateUserLogin')
DROP PROCEDURE sp_ValidateUserLogin;
GO

CREATE PROCEDURE sp_ValidateUserLogin
    @UserName NVARCHAR(256),
    @Id NVARCHAR(450) OUTPUT,
    @Email NVARCHAR(256) OUTPUT,
    @PasswordHash NVARCHAR(MAX) OUTPUT,
    @FirstName NVARCHAR(100) OUTPUT,
    @LastName NVARCHAR(100) OUTPUT,
    @IsActive BIT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        @Id = Id,
        @Email = Email,
        @PasswordHash = PasswordHash,
        @FirstName = FirstName,
        @LastName = LastName,
        @IsActive = IsActive
    FROM AspNetUsers
    WHERE UserName = @UserName AND IsActive = 1;
    
    -- Update last login time
    IF @Id IS NOT NULL
    BEGIN
        UPDATE AspNetUsers 
        SET LastLoginAt = GETUTCDATE()
        WHERE Id = @Id;
    END
END;
GO

-- =====================================================
-- STORED PROCEDURE: Get User Roles
-- =====================================================

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetUserRoles')
DROP PROCEDURE sp_GetUserRoles;
GO

CREATE PROCEDURE sp_GetUserRoles
    @UserId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT r.Id, r.Name, r.NormalizedName
    FROM AspNetRoles r
    INNER JOIN AspNetUserRoles ur ON r.Id = ur.RoleId
    WHERE ur.UserId = @UserId;
END;
GO

-- =====================================================
-- STORED PROCEDURE: Get User Permissions
-- =====================================================

IF EXISTS (SELECT * FROM sys.objects WHERE type = 'P' AND name = 'sp_GetUserPermissions')
DROP PROCEDURE sp_GetUserPermissions;
GO

CREATE PROCEDURE sp_GetUserPermissions
    @UserId NVARCHAR(450)
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT DISTINCT p.PermissionId, p.PermissionCode, p.PermissionName
    FROM Permissions p
    INNER JOIN RolePermissions rp ON p.PermissionId = rp.PermissionId
    INNER JOIN AspNetUserRoles ur ON rp.RoleId = ur.RoleId
    WHERE ur.UserId = @UserId;
END;
GO

PRINT 'Auth stored procedures created successfully!';
GO
