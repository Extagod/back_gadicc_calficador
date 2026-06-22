SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;

USE GadiccCalificador;
GO

-- Tabla Encargados
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Encargados')
BEGIN
    CREATE TABLE Encargados (
        IdEncargado INT IDENTITY(1,1) PRIMARY KEY,
        Nombre NVARCHAR(100) NOT NULL,
        Apellido NVARCHAR(100) NOT NULL,
        Cargo NVARCHAR(100) NULL,
        Direccion NVARCHAR(200) NULL,
        TokenQR NVARCHAR(32) NULL,
        CodigoQR NVARCHAR(MAX) NULL
    );
END
GO

-- Índice único en TokenQR
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Encargados_TokenQR')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_Encargados_TokenQR 
    ON Encargados(TokenQR) 
    WHERE TokenQR IS NOT NULL;
END
GO

-- Tabla Calificaciones
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Calificaciones')
BEGIN
    CREATE TABLE Calificaciones (
        IdCalificacion INT IDENTITY(1,1) PRIMARY KEY,
        IdEncargado INT NOT NULL,
        Valor INT NOT NULL,
        Comentarios NVARCHAR(500) NULL,
        FechaHora DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT FK_Calificaciones_Encargados 
            FOREIGN KEY (IdEncargado) 
            REFERENCES Encargados(IdEncargado) 
            ON DELETE CASCADE
    );
END
GO

-- Tabla UsuariosAdmin
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UsuariosAdmin')
BEGIN
    CREATE TABLE UsuariosAdmin (
        IdUsuario INT IDENTITY(1,1) PRIMARY KEY,
        NombreUsuario NVARCHAR(50) NOT NULL,
        PasswordHash NVARCHAR(MAX) NOT NULL,
        FechaCreacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );
END
GO

-- Índice único en NombreUsuario
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_UsuariosAdmin_NombreUsuario')
BEGIN
    CREATE UNIQUE NONCLUSTERED INDEX IX_UsuariosAdmin_NombreUsuario 
    ON UsuariosAdmin(NombreUsuario);
END
GO

-- Insertar usuario admin por defecto (password: Admin123!)
-- BCrypt hash de "Admin123!" con cost factor 12
IF NOT EXISTS (SELECT * FROM UsuariosAdmin WHERE NombreUsuario = 'admin')
BEGIN
    INSERT INTO UsuariosAdmin (NombreUsuario, PasswordHash, FechaCreacion)
    VALUES ('admin', '$2a$12$boDZnBuVosLhLIeyxwk2ZOULaEamHoc9U7LFQ530D36bR9gwor0u6', SYSUTCDATETIME());
END
GO

SELECT 'Database setup complete' AS Result;
GO
