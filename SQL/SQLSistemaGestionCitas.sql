/*
=============================================
SISTEMA DE GESTIÓN DE CITAS MÉDICAS
Base de Datos SQL Server
Versión: 1.0.0
Compatible con: .NET 8 / Entity Framework Core
Autor: Universidad IUV Maestría
Fecha: 2025-01-10
=============================================

CARACTERÍSTICAS:
- Implementa herencia de POO (Usuario -> Paciente/Doctor/Asistente)
- Relaciones de composición y asociación
- Constraints e índices optimizados
- Datos mock precargados
- Auditoría básica con timestamps
- Stored Procedures para operaciones comunes

=============================================
*/

USE master;
GO

-- =============================================
-- 1. CREACIÓN DE BASE DE DATOS
-- =============================================

IF EXISTS (SELECT name FROM sys.databases WHERE name = N'SistemaGestionCitasMedicas')
BEGIN
    ALTER DATABASE SistemaGestionCitasMedicas SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE SistemaGestionCitasMedicas;
END
GO

CREATE DATABASE SistemaGestionCitasMedicas
GO

USE SistemaGestionCitasMedicas;
GO

-- =============================================
-- 2. TABLA BASE: Usuarios (Clase Base - Herencia)
-- =============================================
-- Representa la clase base Usuario de la cual heredan
-- Paciente, Doctor y Asistente
-- =============================================

CREATE TABLE Usuarios (
    IdUsuario UNIQUEIDENTIFIER NOT NULL,
    Nombre NVARCHAR(200) NOT NULL,
    Email NVARCHAR(200) NOT NULL,
    PasswordHash NVARCHAR(500) NOT NULL,
    Rol NVARCHAR(50) NOT NULL,
    FechaCreacion DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    FechaModificacion DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    Activo BIT NOT NULL DEFAULT 1,
    
    CONSTRAINT PK_Usuarios PRIMARY KEY CLUSTERED (IdUsuario),
    CONSTRAINT UQ_Usuarios_Email UNIQUE NONCLUSTERED (Email),
    CONSTRAINT CK_Usuarios_Rol CHECK (Rol IN ('Paciente', 'Doctor', 'Asistente', 'Admin')),
    CONSTRAINT CK_Usuarios_Email_Formato CHECK (Email LIKE '%_@__%.__%')
);
GO

-- Comentarios de documentación
EXEC sys.sp_addextendedproperty 
    @name = N'MS_Description', 
    @value = N'Tabla base que almacena todos los usuarios del sistema (Pacientes, Doctores, Asistentes)', 
    @level0type = N'SCHEMA', @level0name = N'dbo',
    @level1type = N'TABLE',  @level1name = N'Usuarios';
GO

-- =============================================
-- 3. TABLA: Pacientes (Hereda de Usuarios)
-- =============================================

CREATE TABLE Pacientes (
    IdPaciente UNIQUEIDENTIFIER NOT NULL,
    IdUsuario UNIQUEIDENTIFIER NOT NULL,
    FechaNacimiento DATE NOT NULL,
    Telefono NVARCHAR(20) NOT NULL,
    FechaRegistro DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_Pacientes PRIMARY KEY CLUSTERED (IdPaciente),
    CONSTRAINT UQ_Pacientes_IdUsuario UNIQUE NONCLUSTERED (IdUsuario),
    CONSTRAINT FK_Pacientes_Usuarios FOREIGN KEY (IdUsuario) 
        REFERENCES Usuarios(IdUsuario) ON DELETE CASCADE,
    CONSTRAINT CK_Pacientes_FechaNacimiento CHECK (FechaNacimiento < CAST(GETDATE() AS DATE)),
    CONSTRAINT CK_Pacientes_Telefono CHECK (LEN(Telefono) >= 8)
);
GO

CREATE NONCLUSTERED INDEX IX_Pacientes_IdUsuario 
    ON Pacientes(IdUsuario);
GO

-- =============================================
-- 4. TABLA: Doctores (Hereda de Usuarios)
-- =============================================

CREATE TABLE Doctores (
    IdDoctor UNIQUEIDENTIFIER NOT NULL,
    IdUsuario UNIQUEIDENTIFIER NOT NULL,
    NumeroCedula NVARCHAR(50) NOT NULL,
    Especialidad NVARCHAR(100) NOT NULL,
    FechaRegistro DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_Doctores PRIMARY KEY CLUSTERED (IdDoctor),
    CONSTRAINT UQ_Doctores_IdUsuario UNIQUE NONCLUSTERED (IdUsuario),
    CONSTRAINT UQ_Doctores_NumeroCedula UNIQUE NONCLUSTERED (NumeroCedula),
    CONSTRAINT FK_Doctores_Usuarios FOREIGN KEY (IdUsuario) 
        REFERENCES Usuarios(IdUsuario) ON DELETE CASCADE
);
GO

CREATE NONCLUSTERED INDEX IX_Doctores_Especialidad 
    ON Doctores(Especialidad);
GO

-- =============================================
-- 5. TABLA: Asistentes (Hereda de Usuarios)
-- =============================================

CREATE TABLE Asistentes (
    IdAsistente UNIQUEIDENTIFIER NOT NULL,
    IdUsuario UNIQUEIDENTIFIER NOT NULL,
    Turno NVARCHAR(20) NULL,
    Area NVARCHAR(100) NULL,
    FechaRegistro DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_Asistentes PRIMARY KEY CLUSTERED (IdAsistente),
    CONSTRAINT UQ_Asistentes_IdUsuario UNIQUE NONCLUSTERED (IdUsuario),
    CONSTRAINT FK_Asistentes_Usuarios FOREIGN KEY (IdUsuario) 
        REFERENCES Usuarios(IdUsuario) ON DELETE CASCADE,
    CONSTRAINT CK_Asistentes_Turno CHECK (Turno IS NULL OR Turno IN ('Mañana', 'Tarde', 'Noche', 'Rotativo'))
);
GO

-- =============================================
-- 6. TABLA: Citas (Composición: Paciente + Doctor)
-- =============================================

CREATE TABLE Citas (
    IdCita UNIQUEIDENTIFIER NOT NULL,
    IdPaciente UNIQUEIDENTIFIER NOT NULL,
    IdDoctor UNIQUEIDENTIFIER NOT NULL,
    FechaHora DATETIME2(7) NOT NULL,
    DuracionMin INT NOT NULL,
    Tipo NVARCHAR(100) NOT NULL,
    Estado NVARCHAR(50) NOT NULL DEFAULT 'Programada',
    FechaCreacion DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    FechaModificacion DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    UsuarioModificacion NVARCHAR(200) NULL DEFAULT SYSTEM_USER,
    
    CONSTRAINT PK_Citas PRIMARY KEY CLUSTERED (IdCita),
    CONSTRAINT FK_Citas_Pacientes FOREIGN KEY (IdPaciente) 
        REFERENCES Pacientes(IdPaciente),
    CONSTRAINT FK_Citas_Doctores FOREIGN KEY (IdDoctor) 
        REFERENCES Doctores(IdDoctor),
    CONSTRAINT CK_Citas_DuracionMin CHECK (DuracionMin > 0 AND DuracionMin <= 480),
    CONSTRAINT CK_Citas_Estado CHECK (Estado IN ('Programada', 'Completada', 'Cancelada', 'Reprogramada', 'En Curso')),
    CONSTRAINT CK_Citas_FechaHora CHECK (FechaHora >= DATEADD(MINUTE, -5, GETDATE())) -- Permite 5 min de margen
);
GO

-- Índices para optimizar consultas frecuentes
CREATE NONCLUSTERED INDEX IX_Citas_FechaHora_Estado 
    ON Citas(FechaHora, Estado) 
    INCLUDE (IdPaciente, IdDoctor);
GO

CREATE NONCLUSTERED INDEX IX_Citas_IdPaciente_FechaHora 
    ON Citas(IdPaciente, FechaHora DESC);
GO

CREATE NONCLUSTERED INDEX IX_Citas_IdDoctor_FechaHora 
    ON Citas(IdDoctor, FechaHora) 
    WHERE Estado <> 'Cancelada'; -- Índice filtrado para agenda activa
GO

-- =============================================
-- 7. TABLA: NotasMedicas (Asociación con Citas y Pacientes)
-- =============================================

CREATE TABLE NotasMedicas (
    IdNota UNIQUEIDENTIFIER NOT NULL,
    IdCita UNIQUEIDENTIFIER NULL,
    IdPaciente UNIQUEIDENTIFIER NOT NULL,
    Fecha DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    Texto NVARCHAR(MAX) NOT NULL,
    Diagnostico NVARCHAR(MAX) NULL,
    IdDoctorAutor UNIQUEIDENTIFIER NULL,
    FechaCreacion DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_NotasMedicas PRIMARY KEY CLUSTERED (IdNota),
    CONSTRAINT FK_NotasMedicas_Citas FOREIGN KEY (IdCita) 
        REFERENCES Citas(IdCita),
    CONSTRAINT FK_NotasMedicas_Pacientes FOREIGN KEY (IdPaciente) 
        REFERENCES Pacientes(IdPaciente),
    CONSTRAINT FK_NotasMedicas_Doctores FOREIGN KEY (IdDoctorAutor) 
        REFERENCES Doctores(IdDoctor),
    CONSTRAINT CK_NotasMedicas_Texto CHECK (LEN(Texto) >= 10)
);
GO

CREATE NONCLUSTERED INDEX IX_NotasMedicas_IdPaciente_Fecha 
    ON NotasMedicas(IdPaciente, Fecha DESC);
GO

CREATE NONCLUSTERED INDEX IX_NotasMedicas_IdCita 
    ON NotasMedicas(IdCita) 
    WHERE IdCita IS NOT NULL;
GO

-- =============================================
-- 8. TABLA: Recordatorios (Opcional - RecordatorioService)
-- =============================================

CREATE TABLE Recordatorios (
    IdRecordatorio UNIQUEIDENTIFIER NOT NULL DEFAULT NEWID(),
    IdCita UNIQUEIDENTIFIER NOT NULL,
    TiempoAntesMinutos INT NOT NULL DEFAULT 60,
    FechaHoraProgramado DATETIME2(7) NOT NULL,
    FechaHoraEnviado DATETIME2(7) NULL,
    Enviado BIT NOT NULL DEFAULT 0,
    MedioEnvio NVARCHAR(20) NOT NULL DEFAULT 'Email',
    MensajeEnviado NVARCHAR(500) NULL,
    
    CONSTRAINT PK_Recordatorios PRIMARY KEY CLUSTERED (IdRecordatorio),
    CONSTRAINT FK_Recordatorios_Citas FOREIGN KEY (IdCita) 
        REFERENCES Citas(IdCita) ON DELETE CASCADE,
    CONSTRAINT CK_Recordatorios_TiempoAntes CHECK (TiempoAntesMinutos BETWEEN 5 AND 10080), -- Entre 5 min y 7 días
    CONSTRAINT CK_Recordatorios_MedioEnvio CHECK (MedioEnvio IN ('Email', 'SMS', 'WhatsApp', 'Notificacion'))
);
GO

CREATE NONCLUSTERED INDEX IX_Recordatorios_FechaHoraProgramado_Enviado 
    ON Recordatorios(FechaHoraProgramado, Enviado) 
    WHERE Enviado = 0; -- Solo recordatorios pendientes
GO

-- =============================================
-- 9. TABLA DE AUDITORÍA
-- =============================================

CREATE TABLE AuditoriaCitas (
    IdAuditoria BIGINT IDENTITY(1,1) NOT NULL,
    IdCita UNIQUEIDENTIFIER NOT NULL,
    Operacion NVARCHAR(20) NOT NULL,
    EstadoAnterior NVARCHAR(50) NULL,
    EstadoNuevo NVARCHAR(50) NULL,
    FechaHoraAnterior DATETIME2(7) NULL,
    FechaHoraNueva DATETIME2(7) NULL,
    Usuario NVARCHAR(200) NOT NULL DEFAULT SYSTEM_USER,
    FechaOperacion DATETIME2(7) NOT NULL DEFAULT GETDATE(),
    
    CONSTRAINT PK_AuditoriaCitas PRIMARY KEY CLUSTERED (IdAuditoria),
    CONSTRAINT CK_AuditoriaCitas_Operacion CHECK (Operacion IN ('INSERT', 'UPDATE', 'DELETE'))
);
GO

CREATE NONCLUSTERED INDEX IX_AuditoriaCitas_IdCita_FechaOperacion 
    ON AuditoriaCitas(IdCita, FechaOperacion DESC);
GO

-- =============================================
-- 10. TRIGGERS DE AUDITORÍA
-- =============================================

-- Trigger: Auditar cambios de estado en Citas
CREATE TRIGGER TR_Citas_AuditoriaUpdate
ON Citas
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    INSERT INTO AuditoriaCitas (IdCita, Operacion, EstadoAnterior, EstadoNuevo, FechaHoraAnterior, FechaHoraNueva, Usuario)
    SELECT 
        i.IdCita,
        'UPDATE',
        d.Estado,
        i.Estado,
        d.FechaHora,
        i.FechaHora,
        ISNULL(i.UsuarioModificacion, SYSTEM_USER)
    FROM inserted i
    INNER JOIN deleted d ON i.IdCita = d.IdCita
    WHERE i.Estado <> d.Estado 
       OR i.FechaHora <> d.FechaHora;
END;
GO

-- Trigger: Actualizar FechaModificacion automáticamente
CREATE TRIGGER TR_Citas_ActualizarFechaModificacion
ON Citas
AFTER UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    UPDATE Citas
    SET FechaModificacion = GETDATE()
    FROM Citas c
    INNER JOIN inserted i ON c.IdCita = i.IdCita;
END;
GO

-- Trigger: Validar conflictos de horario (mismo doctor, misma hora)
CREATE TRIGGER TR_Citas_ValidarConflictoHorario
ON Citas
AFTER INSERT, UPDATE
AS
BEGIN
    SET NOCOUNT ON;
    
    IF EXISTS (
        SELECT 1
        FROM inserted i
        INNER JOIN Citas c ON c.IdDoctor = i.IdDoctor 
            AND c.IdCita <> i.IdCita
            AND c.Estado NOT IN ('Cancelada')
            AND (
                -- La nueva cita comienza durante una existente
                (i.FechaHora BETWEEN c.FechaHora AND DATEADD(MINUTE, c.DuracionMin, c.FechaHora))
                OR
                -- La nueva cita termina durante una existente
                (DATEADD(MINUTE, i.DuracionMin, i.FechaHora) BETWEEN c.FechaHora AND DATEADD(MINUTE, c.DuracionMin, c.FechaHora))
                OR
                -- La nueva cita engloba completamente una existente
                (i.FechaHora <= c.FechaHora AND DATEADD(MINUTE, i.DuracionMin, i.FechaHora) >= DATEADD(MINUTE, c.DuracionMin, c.FechaHora))
            )
    )
    BEGIN
        RAISERROR ('El doctor ya tiene una cita programada en ese horario', 16, 1);
        ROLLBACK TRANSACTION;
    END
END;
GO

-- =============================================
-- 11. STORED PROCEDURES
-- =============================================

-- SP: Obtener agenda de un doctor por fecha
CREATE OR ALTER PROCEDURE sp_ObtenerAgendaDoctor
    @IdDoctor UNIQUEIDENTIFIER,
    @Fecha DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.IdCita,
        c.FechaHora,
        c.DuracionMin,
        DATEADD(MINUTE, c.DuracionMin, c.FechaHora) AS FechaHoraFin,
        c.Tipo,
        c.Estado,
        up.Nombre AS NombrePaciente,
        up.Email AS EmailPaciente,
        pac.Telefono AS TelefonoPaciente,
        CASE WHEN n.IdNota IS NOT NULL THEN 1 ELSE 0 END AS TieneNota
    FROM Citas c
    INNER JOIN Pacientes pac ON c.IdPaciente = pac.IdPaciente
    INNER JOIN Usuarios up ON pac.IdUsuario = up.IdUsuario
    LEFT JOIN NotasMedicas n ON c.IdCita = n.IdCita
    WHERE c.IdDoctor = @IdDoctor
      AND CAST(c.FechaHora AS DATE) = @Fecha
      AND c.Estado <> 'Cancelada'
    ORDER BY c.FechaHora;
END;
GO

-- SP: Obtener historial médico completo de un paciente
CREATE OR ALTER PROCEDURE sp_ObtenerHistorialPaciente
    @IdPaciente UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        n.IdNota,
        n.Fecha AS FechaNota,
        n.Texto,
        n.Diagnostico,
        c.FechaHora AS FechaCita,
        c.Tipo AS TipoCita,
        c.Estado AS EstadoCita,
        ud.Nombre AS NombreDoctor,
        d.Especialidad AS EspecialidadDoctor,
        d.NumeroCedula AS CedulaDoctor
    FROM NotasMedicas n
    LEFT JOIN Citas c ON n.IdCita = c.IdCita
    LEFT JOIN Doctores d ON c.IdDoctor = d.IdDoctor OR n.IdDoctorAutor = d.IdDoctor
    LEFT JOIN Usuarios ud ON d.IdUsuario = ud.IdUsuario
    WHERE n.IdPaciente = @IdPaciente
    ORDER BY n.Fecha DESC;
END;
GO

-- SP: Generar reporte de consultas en un rango de fechas
CREATE OR ALTER PROCEDURE sp_ReporteConsultas
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.IdCita,
        c.FechaHora,
        c.DuracionMin,
        c.Tipo,
        c.Estado,
        up.Nombre AS Paciente,
        p.Telefono AS TelefonoPaciente,
        ud.Nombre AS Doctor,
        d.Especialidad,
        d.NumeroCedula,
        CASE WHEN n.IdNota IS NOT NULL THEN 'Sí' ELSE 'No' END AS TieneNota,
        n.Diagnostico
    FROM Citas c
    INNER JOIN Pacientes p ON c.IdPaciente = p.IdPaciente
    INNER JOIN Usuarios up ON p.IdUsuario = up.IdUsuario
    INNER JOIN Doctores d ON c.IdDoctor = d.IdDoctor
    INNER JOIN Usuarios ud ON d.IdUsuario = ud.IdUsuario
    LEFT JOIN NotasMedicas n ON c.IdCita = n.IdCita
    WHERE CAST(c.FechaHora AS DATE) BETWEEN @FechaInicio AND @FechaFin
      AND c.Estado <> 'Cancelada'
    ORDER BY c.FechaHora DESC;
END;
GO

-- SP: Generar reporte de cancelaciones
CREATE OR ALTER PROCEDURE sp_ReporteCancelaciones
    @FechaInicio DATE,
    @FechaFin DATE
AS
BEGIN
    SET NOCOUNT ON;
    
    SELECT 
        c.IdCita,
        c.FechaHora AS FechaHoraProgramada,
        c.Tipo,
        up.Nombre AS Paciente,
        ud.Nombre AS Doctor,
        d.Especialidad,
        c.FechaModificacion AS FechaCancelacion,
        c.UsuarioModificacion AS UsuarioQueCancelo,
        DATEDIFF(HOUR, c.FechaCreacion, c.FechaModificacion) AS HorasEntreCreacionYCancelacion
    FROM Citas c
    INNER JOIN Pacientes p ON c.IdPaciente = p.IdPaciente
    INNER JOIN Usuarios up ON p.IdUsuario = up.IdUsuario
    INNER JOIN Doctores d ON c.IdDoctor = d.IdDoctor
    INNER JOIN Usuarios ud ON d.IdUsuario = ud.IdUsuario
    WHERE c.Estado = 'Cancelada'
      AND CAST(c.FechaModificacion AS DATE) BETWEEN @FechaInicio AND @FechaFin
    ORDER BY c.FechaModificacion DESC;
END;
GO

-- SP: Reprogramar una cita
CREATE OR ALTER PROCEDURE sp_ReprogramarCita
    @IdCita UNIQUEIDENTIFIER,
    @NuevaFechaHora DATETIME2,
    @Usuario NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        DECLARE @EstadoActual NVARCHAR(50);
        
        SELECT @EstadoActual = Estado
        FROM Citas
        WHERE IdCita = @IdCita;
        
        IF @EstadoActual IS NULL
        BEGIN
            RAISERROR('La cita no existe', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN -1;
        END
        
        IF @EstadoActual <> 'Programada'
        BEGIN
            RAISERROR('Solo se pueden reprogramar citas en estado Programada', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN -2;
        END
        
        UPDATE Citas
        SET FechaHora = @NuevaFechaHora,
            Estado = 'Reprogramada',
            FechaModificacion = GETDATE(),
            UsuarioModificacion = ISNULL(@Usuario, SYSTEM_USER)
        WHERE IdCita = @IdCita;
        
        COMMIT TRANSACTION;
        RETURN 0;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
        RETURN -99;
    END CATCH
END;
GO

-- SP: Cancelar una cita
CREATE OR ALTER PROCEDURE sp_CancelarCita
    @IdCita UNIQUEIDENTIFIER,
    @Usuario NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRANSACTION;
    
    BEGIN TRY
        DECLARE @EstadoActual NVARCHAR(50);
        
        SELECT @EstadoActual = Estado
        FROM Citas
        WHERE IdCita = @IdCita;
        
        IF @EstadoActual IS NULL
        BEGIN
            RAISERROR('La cita no existe', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN -1;
        END
        
        IF @EstadoActual = 'Cancelada'
        BEGIN
            RAISERROR('La cita ya está cancelada', 16, 1);
            ROLLBACK TRANSACTION;
            RETURN -2;
        END
        
        UPDATE Citas
        SET Estado = 'Cancelada',
            FechaModificacion = GETDATE(),
            UsuarioModificacion = ISNULL(@Usuario, SYSTEM_USER)
        WHERE IdCita = @IdCita;
        
        COMMIT TRANSACTION;
        RETURN 0;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0
            ROLLBACK TRANSACTION;
        
        DECLARE @ErrorMessage NVARCHAR(4000) = ERROR_MESSAGE();
        RAISERROR(@ErrorMessage, 16, 1);
        RETURN -99;
    END CATCH
END;
GO

-- =============================================
-- 12. VISTAS ÚTILES
-- =============================================

-- Vista: Citas con información completa
CREATE OR ALTER VIEW vw_CitasCompletas
AS
SELECT 
    c.IdCita,
    c.FechaHora,
    c.DuracionMin,
    DATEADD(MINUTE, c.DuracionMin, c.FechaHora) AS FechaHoraFin,
    c.Tipo,
    c.Estado,
    c.FechaCreacion,
    c.FechaModificacion,
    -- Paciente
    p.IdPaciente,
    up.Nombre AS NombrePaciente,
    up.Email AS EmailPaciente,
    p.Telefono AS TelefonoPaciente,
    p.FechaNacimiento AS FechaNacimientoPaciente,
    DATEDIFF(YEAR, p.FechaNacimiento, GETDATE()) AS EdadPaciente,
    -- Doctor
    d.IdDoctor,
    ud.Nombre AS NombreDoctor,
    ud.Email AS EmailDoctor,
    d.NumeroCedula AS CedulaDoctor,
    d.Especialidad AS EspecialidadDoctor,
    -- Nota
    n.IdNota,
    CASE WHEN n.IdNota IS NOT NULL THEN 1 ELSE 0 END AS TieneNota
FROM Citas c
INNER JOIN Pacientes p ON c.IdPaciente = p.IdPaciente
INNER JOIN Usuarios up ON p.IdUsuario = up.IdUsuario
INNER JOIN Doctores d ON c.IdDoctor = d.IdDoctor
INNER JOIN Usuarios ud ON d.IdUsuario = ud.IdUsuario
LEFT JOIN NotasMedicas n ON c.IdCita = n.IdCita;
GO

-- Vista: Dashboard de estadísticas
CREATE OR ALTER VIEW vw_EstadisticasGenerales
AS
SELECT 
    (SELECT COUNT(*) FROM Pacientes) AS TotalPacientes,
    (SELECT COUNT(*) FROM Doctores) AS TotalDoctores,
    (SELECT COUNT(*) FROM Citas WHERE Estado = 'Programada') AS CitasProgramadas,
    (SELECT COUNT(*) FROM Citas WHERE Estado = 'Completada') AS CitasCompletadas,
    (SELECT COUNT(*) FROM Citas WHERE Estado = 'Cancelada') AS CitasCanceladas,
    (SELECT COUNT(*) FROM NotasMedicas) AS TotalNotasMedicas,
    (SELECT COUNT(DISTINCT IdPaciente) FROM Citas WHERE Estado = 'Completada') AS PacientesAtendidos,
    (SELECT COUNT(*) FROM Citas WHERE CAST(FechaHora AS DATE) = CAST(GETDATE() AS DATE)) AS CitasHoy;
GO

-- =============================================
-- 13. FUNCIONES ÚTILES
-- =============================================

-- Función: Calcular edad de un paciente
CREATE OR ALTER FUNCTION fn_CalcularEdad(@FechaNacimiento DATE)
RETURNS INT
AS
BEGIN
    RETURN DATEDIFF(YEAR, @FechaNacimiento, GETDATE()) - 
           CASE WHEN DATEADD(YEAR, DATEDIFF(YEAR, @FechaNacimiento, GETDATE()), @FechaNacimiento) > GETDATE() 
                THEN 1 ELSE 0 END;
END;
GO

-- Función: Obtener disponibilidad de un doctor
CREATE OR ALTER FUNCTION fn_DoctorDisponible(
    @IdDoctor UNIQUEIDENTIFIER,
    @FechaHora DATETIME2,
    @DuracionMin INT
)
RETURNS BIT
AS
BEGIN
    DECLARE @Disponible BIT = 1;
    
    IF EXISTS (
        SELECT 1
        FROM Citas
        WHERE IdDoctor = @IdDoctor
          AND Estado <> 'Cancelada'
          AND (
              (@FechaHora BETWEEN FechaHora AND DATEADD(MINUTE, DuracionMin, FechaHora))
              OR
              (DATEADD(MINUTE, @DuracionMin, @FechaHora) BETWEEN FechaHora AND DATEADD(MINUTE, DuracionMin, FechaHora))
              OR
              (@FechaHora <= FechaHora AND DATEADD(MINUTE, @DuracionMin, @FechaHora) >= DATEADD(MINUTE, DuracionMin, FechaHora))
          )
    )
    BEGIN
        SET @Disponible = 0;
    END
    
    RETURN @Disponible;
END;
GO

-- =============================================
-- 14. INSERCIÓN DE DATOS MOCK
-- =============================================

PRINT 'Insertando datos mock...';
GO

-- Insertar Usuarios (Pacientes)
INSERT INTO Usuarios (IdUsuario, Nombre, Email, PasswordHash, Rol)
VALUES 
    ('11111111-1111-1111-1111-111111111111', N'María González', 'maria.gonzalez@email.com', 'cGFjMTIz', 'Paciente'),
    ('22222222-2222-2222-2222-222222222222', N'Carlos Rodríguez', 'carlos.rodriguez@email.com', 'cGFjMTIz', 'Paciente');

-- Insertar Pacientes
INSERT INTO Pacientes (IdPaciente, IdUsuario, FechaNacimiento, Telefono)
VALUES 
    ('11111111-1111-1111-1111-111111111111', '11111111-1111-1111-1111-111111111111', '1990-05-15', '+34 600 123 456'),
    ('22222222-2222-2222-2222-222222222222', '22222222-2222-2222-2222-222222222222', '1979-08-22', '+34 600 234 567');

-- Insertar Usuarios (Doctores)
INSERT INTO Usuarios (IdUsuario, Nombre, Email, PasswordHash, Rol)
VALUES 
    ('33333333-3333-3333-3333-333333333333', N'Dr. Juan Pérez', 'juan.perez@hospital.com', 'ZG9jMTIz', 'Doctor'),
    ('44444444-4444-4444-4444-444444444444', N'Dra. Laura Sánchez', 'laura.sanchez@hospital.com', 'ZG9jMTIz', 'Doctor');

-- Insertar Doctores
INSERT INTO Doctores (IdDoctor, IdUsuario, NumeroCedula, Especialidad)
VALUES 
    ('33333333-3333-3333-3333-333333333333', '33333333-3333-3333-3333-333333333333', 'MED-12345', N'Cardiología'),
    ('44444444-4444-4444-4444-444444444444', '44444444-4444-4444-4444-444444444444', 'MED-67890', N'Pediatría');

-- Insertar Usuarios (Asistentes)
INSERT INTO Usuarios (IdUsuario, Nombre, Email, PasswordHash, Rol)
VALUES 
    ('88888888-8888-8888-8888-888888888888', N'Admin Sistema', 'admin@sistema.com', 'YWRtMTIz', 'Admin'),
    ('99999999-9999-9999-9999-999999999999', N'Recepcionista María', 'recepcion@hospital.com', 'cmVjMTIz', 'Asistente');

-- Insertar Asistentes
INSERT INTO Asistentes (IdAsistente, IdUsuario, Turno, Area)
VALUES 
    ('99999999-9999-9999-9999-999999999999', '99999999-9999-9999-9999-999999999999', N'Mañana', N'Recepción');

-- Insertar Citas
DECLARE @FechaHoy DATETIME2 = GETDATE();

INSERT INTO Citas (IdCita, IdPaciente, IdDoctor, FechaHora, DuracionMin, Tipo, Estado)
VALUES 
    ('55555555-5555-5555-5555-555555555555', 
     '11111111-1111-1111-1111-111111111111', 
     '33333333-3333-3333-3333-333333333333', 
     DATEADD(DAY, 5, @FechaHoy), 30, N'Consulta General', 'Programada'),
    
    ('66666666-6666-6666-6666-666666666666', 
     '22222222-2222-2222-2222-222222222222', 
     '44444444-4444-4444-4444-444444444444', 
     DATEADD(DAY, 7, @FechaHoy), 45, N'Control Pediátrico', 'Programada'),
    
    ('77777777-7777-7777-7777-777777777777', 
     '11111111-1111-1111-1111-111111111111', 
     '33333333-3333-3333-3333-333333333333', 
     DATEADD(DAY, 10, @FechaHoy), 30, N'Revisión Cardiológica', 'Programada');

-- Insertar una Nota Médica de ejemplo
INSERT INTO NotasMedicas (IdNota, IdCita, IdPaciente, Fecha, Texto, Diagnostico, IdDoctorAutor)
VALUES 
    (NEWID(), 
     NULL, 
     '11111111-1111-1111-1111-111111111111', 
     DATEADD(DAY, -30, GETDATE()), 
     N'Paciente acude por primera vez. Antecedentes de hipertensión familiar.',
     N'Hipertensión arterial leve - Iniciar tratamiento con dieta baja en sodio',
     '33333333-3333-3333-3333-333333333333');

-- Insertar Recordatorios para las citas
INSERT INTO Recordatorios (IdRecordatorio, IdCita, TiempoAntesMinutos, FechaHoraProgramado, MedioEnvio)
SELECT 
    NEWID(),
    IdCita,
    60, -- 1 hora antes
    DATEADD(MINUTE, -60, FechaHora),
    'Email'
FROM Citas
WHERE Estado = 'Programada';

GO

-- =============================================
-- 15. CONSULTAS DE VERIFICACIÓN
-- =============================================

PRINT '================================================';
PRINT 'BASE DE DATOS CREADA EXITOSAMENTE';
PRINT '================================================';
PRINT '';
PRINT 'RESUMEN DE DATOS:';
PRINT '-------------------';

SELECT 
    'Usuarios' AS Tabla, COUNT(*) AS Cantidad FROM Usuarios
UNION ALL
SELECT 'Pacientes', COUNT(*) FROM Pacientes
UNION ALL
SELECT 'Doctores', COUNT(*) FROM Doctores
UNION ALL
SELECT 'Asistentes', COUNT(*) FROM Asistentes
UNION ALL
SELECT 'Citas', COUNT(*) FROM Citas
UNION ALL
SELECT 'NotasMedicas', COUNT(*) FROM NotasMedicas
UNION ALL
SELECT 'Recordatorios', COUNT(*) FROM Recordatorios;

PRINT '';
PRINT '================================================';
PRINT 'CONSULTAS DE PRUEBA:';
PRINT '================================================';

-- Ver todos los pacientes
PRINT '';
PRINT '-- PACIENTES --';
SELECT u.Nombre, u.Email, p.FechaNacimiento, p.Telefono,
       dbo.fn_CalcularEdad(p.FechaNacimiento) AS Edad
FROM Pacientes p
INNER JOIN Usuarios u ON p.IdUsuario = u.IdUsuario;

-- Ver todos los doctores
PRINT '';
PRINT '-- DOCTORES --';
SELECT u.Nombre, u.Email, d.NumeroCedula, d.Especialidad
FROM Doctores d
INNER JOIN Usuarios u ON d.IdUsuario = u.IdUsuario;

-- Ver todas las citas
PRINT '';
PRINT '-- CITAS --';
SELECT * FROM vw_CitasCompletas;

-- Ver estadísticas
PRINT '';
PRINT '-- ESTADÍSTICAS --';
SELECT * FROM vw_EstadisticasGenerales;

PRINT '';
PRINT '================================================';
PRINT 'STORED PROCEDURES DISPONIBLES:';
PRINT '- sp_ObtenerAgendaDoctor';
PRINT '- sp_ObtenerHistorialPaciente';
PRINT '- sp_ReporteConsultas';
PRINT '- sp_ReporteCancelaciones';
PRINT '- sp_ReprogramarCita';
PRINT '- sp_CancelarCita';
PRINT '================================================';
GO