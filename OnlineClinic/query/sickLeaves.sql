CREATE TABLE [dbo].[SickLeaves] (
    [SickLeaveID] INT IDENTITY (1, 1) NOT NULL,
    [Number] NVARCHAR(50) NOT NULL,               -- Номер больничного листа
    [PatientID] INT NOT NULL,                     -- Ссылка на пациента (ID из Persons)
    [DoctorUserID] INT NOT NULL,                  -- Ссылка на врача (UserID из UserCredentialsDB)
    [Diagnosis] NVARCHAR(500) NOT NULL,           -- Диагноз
    [StartDate] DATE NOT NULL,                    -- Дата начала нетрудоспособности
    [EndDate] DATE NOT NULL,                      -- Дата окончания нетрудоспособности
    [IssueDate] DATETIME NOT NULL DEFAULT GETDATE(), -- Дата выдачи больничного
    [Status] NVARCHAR(50) NOT NULL DEFAULT 'Active', -- Статус (Active, Closed, Extended, Cancelled)
    [Type] NVARCHAR(50) NOT NULL,                 -- Тип (Первичный, Продолжение, Дополнительный)
    [WorkPlace] NVARCHAR(255) NULL,               -- Место работы пациента
    [Comments] NVARCHAR(MAX) NULL,                -- Дополнительные комментарии
    [PreviousSickLeaveID] INT NULL,               -- Ссылка на предыдущий больничный (для продлений)
    PRIMARY KEY CLUSTERED ([SickLeaveID] ASC),
    CONSTRAINT [FK_SickLeaves_Patient] FOREIGN KEY ([PatientID]) 
        REFERENCES [dbo].[Persons] ([ID]),
    CONSTRAINT [FK_SickLeaves_Doctor] FOREIGN KEY ([DoctorUserID]) 
        REFERENCES [dbo].[UserCredentialsDB] ([UserID]),
    CONSTRAINT [FK_SickLeaves_Previous] FOREIGN KEY ([PreviousSickLeaveID]) 
        REFERENCES [dbo].[SickLeaves] ([SickLeaveID]),
    UNIQUE NONCLUSTERED ([Number] ASC),           -- Номер больничного должен быть уникальным
    CHECK ([StartDate] <= [EndDate]),
    CHECK ([IssueDate] >= [StartDate]),
    CHECK ([Status] IN ('Active', 'Closed', 'Extended', 'Cancelled')),
    CHECK ([Type] IN ('Primary', 'Continuation', 'Additional'))
);