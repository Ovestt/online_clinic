CREATE TABLE Persons (
    ID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(50) NOT NULL,              -- Имя
    LastName NVARCHAR(50) NOT NULL,                -- Фамилия
    MiddleName NVARCHAR(50),                       -- Отчество
    BirthDate DATE NOT NULL,                       -- Дата рождения
    PhoneNumber NVARCHAR(20),                      -- Номер телефона
    Gender CHAR(1) CHECK (Gender IN ('M', 'F')),   -- Пол (M - мужской, F - женский)
    SNILS NVARCHAR(14),                            -- СНИЛС (формат: XXX-XXX-XXX XX)
    RegistrationAddress NVARCHAR(500) NOT NULL,     -- Адрес регистрации
    ActualAddress NVARCHAR(500)                    -- Фактическое место жительства
);
