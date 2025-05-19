CREATE TABLE [dbo].[UserCredentialsDB] (
    [UserID]       INT            IDENTITY (1, 1) NOT NULL,
    [FIO]          NVARCHAR (200) NULL,
    [BirthDate]    DATE           NULL,
    [Login]        NVARCHAR (100) NOT NULL,
    [PasswordHash] NVARCHAR (255) NOT NULL,
    [Role]         NVARCHAR (50)  NOT NULL,
	[Speciality] NVARCHAR (100),
    PRIMARY KEY CLUSTERED ([UserID] ASC),
    UNIQUE NONCLUSTERED ([Login] ASC)
);

