CREATE TABLE [dbo].[Prescriptions] (
    [PrescriptionID] INT            IDENTITY (1, 1) NOT NULL,
    [DoctorUserID]   INT            NOT NULL,
    [PatientID]      INT            NOT NULL,
    [MedicationName] NVARCHAR (200) NOT NULL,
    [Dosage]         NVARCHAR (100) NOT NULL,
    [IssueDate]      DATETIME       DEFAULT (getdate()) NOT NULL,
    [ExpiryDate]     DATETIME       NULL,
    PRIMARY KEY CLUSTERED ([PrescriptionID] ASC),
    CONSTRAINT [FK_Prescriptions_Doctor] FOREIGN KEY ([DoctorUserID]) REFERENCES [dbo].[UserCredentialsDB] ([UserID]),
    CONSTRAINT [FK_Prescriptions_Patient] FOREIGN KEY ([PatientID]) REFERENCES [dbo].[Persons] ([ID]),
    CHECK ([IssueDate]<=isnull([ExpiryDate],'9999-12-31'))
);

