INSERT INTO [dbo].[UserCredentialsDB] 
    ([FIO], [BirthDate], [Login], [PasswordHash], [Role], [Speciality])
VALUES
    -- Администратор
    ('Иванов Иван Иванович', '1980-05-15', 'admin', CONVERT(NVARCHAR(255), HASHBYTES('MD5', 'admin123'), 2), 'администратор', NULL),
    
    -- Врачи
    ('Петрова Анна Сергеевна', '1975-11-22', 'petrova_a', CONVERT(NVARCHAR(255), HASHBYTES('MD5', 'doctor1'), 2), 'врач', 'терапевт'),
    ('Сидоров Дмитрий Владимирович', '1988-03-10', 'sidorov_d', CONVERT(NVARCHAR(255), HASHBYTES('MD5', 'doctor2'), 2), 'врач', 'хирург'),
    
    -- Регистраторы
    ('Кузнецова Ольга Петровна', '1990-07-30', 'kuznetsova_o', CONVERT(NVARCHAR(255), HASHBYTES('MD5', 'reg123'), 2), 'регистратор', NULL),
    ('Смирнов Алексей Игоревич', '1992-09-14', 'smirnov_a', CONVERT(NVARCHAR(255), HASHBYTES('MD5', 'reg456'), 2), 'регистратор', NULL);