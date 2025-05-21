CREATE TABLE [dbo].[Visits] (
    [VisitID]       INT            IDENTITY (1, 1) NOT NULL,
    [PatientID]     INT            NOT NULL,
    [EmployeeID]    INT            NOT NULL,
    [VisitDateTime] DATETIME       NOT NULL,
    [EndDateTime]   DATETIME       NULL,
    [Notes]         NVARCHAR (MAX) NULL,
    PRIMARY KEY CLUSTERED ([VisitID] ASC),
    CONSTRAINT [FK_Visits_Patient] FOREIGN KEY ([PatientID]) REFERENCES [dbo].[Persons] ([ID]) ON DELETE CASCADE ON UPDATE CASCADE,
    CONSTRAINT [FK_Visits_Employee] FOREIGN KEY ([EmployeeID]) REFERENCES [dbo].[UserCredentialsDB] ([UserID]) ON DELETE CASCADE ON UPDATE CASCADE
);

