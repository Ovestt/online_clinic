CREATE TABLE [dbo].[Diagnoses] (
    [DiagnosisID]   INT            IDENTITY (1, 1) NOT NULL,
    [PatientID]     INT            NOT NULL,
    [DoctorUserID]  INT            NOT NULL,
    [DiagnosisName] NVARCHAR (500) NOT NULL,
    [Description]   NVARCHAR (MAX) NULL,
    [DateCreated]   DATETIME       DEFAULT (getdate()) NOT NULL,
    PRIMARY KEY CLUSTERED ([DiagnosisID] ASC),
    CONSTRAINT [FK_Diagnoses_Patient] FOREIGN KEY ([PatientID]) REFERENCES [dbo].[Persons] ([ID]),
    CONSTRAINT [FK_Diagnoses_Doctor] FOREIGN KEY ([DoctorUserID]) REFERENCES [dbo].[UserCredentialsDB] ([UserID]),
);

