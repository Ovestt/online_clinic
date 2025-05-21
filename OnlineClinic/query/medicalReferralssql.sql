CREATE TABLE [dbo].[MedicalReferrals] (
    [ReferralID] INT IDENTITY (1, 1) NOT NULL,
    [ReferralNumber] NVARCHAR(50) NOT NULL,        -- Номер направления
    [PatientID] INT NOT NULL,                      -- Ссылка на пациента (ID из Persons)
    [DoctorUserID] INT NOT NULL,                   -- Врач, выписавший направление (UserID из UserCredentialsDB)
    [ReferralDate] DATETIME NOT NULL DEFAULT GETDATE(), -- Дата выдачи направления
    [Purpose] NVARCHAR(500) NOT NULL,              -- Цель направления (консультация, исследование)
    [Speciality] NVARCHAR(100) NULL,               -- Специальность врача/отделения (если требуется)
    [ServiceType] NVARCHAR(100) NOT NULL,          -- Тип услуги (анализ, консультация, процедура)
    [DiagnosisID] INT NULL,                        -- Связанный диагноз (если есть)
    
    PRIMARY KEY CLUSTERED ([ReferralID] ASC),
    CONSTRAINT [FK_Referrals_Patient] FOREIGN KEY ([PatientID]) 
        REFERENCES [dbo].[Persons] ([ID]),
    CONSTRAINT [FK_Referrals_Doctor] FOREIGN KEY ([DoctorUserID]) 
        REFERENCES [dbo].[UserCredentialsDB] ([UserID]),
    CONSTRAINT [FK_Referrals_Diagnosis] FOREIGN KEY ([DiagnosisID]) 
        REFERENCES [dbo].[Diagnoses] ([DiagnosisID]),
    
    UNIQUE NONCLUSTERED ([ReferralNumber] ASC),    -- Номер направления должен быть уникальным
    CHECK ([ReferralDate] <= ISNULL([ReferralDate], '9999-12-31')),
);