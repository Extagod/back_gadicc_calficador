# Implementation Plan: Project Analysis Improvements

## Overview

Reestructuración del proyecto ApiEncuestaPrototipe desde una API monolítica hacia una solución multi-proyecto con separación por capas (Capa_Abstracciones, Capa_Datos, Capa_Servicios, API, Panel_Admin, Pruebas). Se migra de PostgreSQL a SQL Server, se implementa validación robusta, manejo global de excepciones, un Panel Administrativo de escritorio (Windows Forms) y pruebas unitarias con xUnit + Moq.

## Tasks

- [x] 1. Set up solution structure and project scaffolding
  - [x] 1.1 Create the .slnx solution file and all project files
    - Create `GadiccCalificador.slnx` in the solution root
    - Create `Capa_Abstracciones/Capa_Abstracciones.csproj` (class library, net8.0, no external packages)
    - Create `Capa_Datos/Capa_Datos.csproj` (class library, net8.0, references Capa_Abstracciones, packages: Microsoft.EntityFrameworkCore.SqlServer 8.0.x, Microsoft.EntityFrameworkCore.Design 8.0.x)
    - Create `Capa_Servicios/Capa_Servicios.csproj` (class library, net8.0, references Capa_Abstracciones, packages: QRCoder 1.4.x, BCrypt.Net-Next 4.0.x)
    - Refactor `ApiEncuestaPrototipe/ApiEncuestaPrototipe.csproj` to reference Capa_Servicios, Capa_Abstracciones, Capa_Datos; add Swashbuckle.AspNetCore; remove Npgsql references
    - Create `Panel_Admin/Panel_Admin.csproj` (WinForms, net8.0-windows, references Capa_Servicios, Capa_Abstracciones, Capa_Datos, packages: System.Configuration.ConfigurationManager 8.0.x)
    - Create `Pruebas/Pruebas.csproj` (xUnit test project, net8.0, references Capa_Servicios, Capa_Abstracciones, packages: xunit 2.7.x, xunit.runner.visualstudio 2.5.x, Moq 4.20.x, coverlet.collector 6.0.x)
    - Add all projects to the .slnx file
    - Verify `dotnet build` compiles successfully from the root
    - _Requirements: 1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 1.11_

- [x] 2. Implement Capa_Abstracciones (entities, interfaces, enums, DTOs, result types)
  - [x] 2.1 Create domain entities and enums
    - Create `Capa_Abstracciones/Enums/ValorCalificacion.cs` with values: Excelente=1, Buena=2, Regular=3, Mala=4
    - Create `Capa_Abstracciones/Entities/Encargado.cs` with data annotations ([Required], [StringLength], [MaxLength]) for Nombre, Apellido, Cargo, Direccion, CodigoQR, TokenQR and navigation property Calificaciones
    - Create `Capa_Abstracciones/Entities/Calificacion.cs` with [Key], [Required], [MaxLength] and navigation to Encargado
    - Create `Capa_Abstracciones/Entities/UsuarioAdmin.cs` with [Key], [Required], [StringLength] for NombreUsuario and PasswordHash
    - _Requirements: 7.4, 10.4, 10.5, 10.6, 10.7, 1.6_

  - [x] 2.2 Create DTOs
    - Create `Capa_Abstracciones/DTOs/CrearCalificacionDto.cs` with [Required][Range(1, int.MaxValue)] IdEncargado, [Required][MaxLength(20)] Calificacion, [MaxLength(500)] Comentarios
    - Create `Capa_Abstracciones/DTOs/CrearEncargadoDto.cs` with [Required][StringLength(100, MinimumLength=1)] Nombre and Apellido, [MaxLength(100)] Cargo, [MaxLength(200)] Direccion
    - _Requirements: 10.1, 10.2, 10.3, 10.4, 10.5, 10.6, 10.7_

  - [x] 2.3 Create result types and service/repository interfaces
    - Create `Capa_Abstracciones/Common/ServiceResult.cs` with generic Success, NotFound, ValidationError, Error factory methods and ServiceErrorType enum
    - Create `Capa_Abstracciones/Common/AuthResult.cs` with Success(nombreUsuario) and Failed(message) factory methods
    - Create `Capa_Abstracciones/Interfaces/IEncargadoRepository.cs` (ObtenerPorIdAsync, ObtenerPorTokenQRAsync, ObtenerTodosAsync, AgregarAsync, ActualizarAsync)
    - Create `Capa_Abstracciones/Interfaces/ICalificacionRepository.cs` (AgregarAsync, ObtenerPorEncargadoIdAsync, ObtenerPorEncargadoYRangoFechasAsync)
    - Create `Capa_Abstracciones/Interfaces/IUsuarioAdminRepository.cs` (ObtenerPorNombreUsuarioAsync)
    - Create `Capa_Abstracciones/Interfaces/IEncargadoService.cs` (CrearEncargadoAsync, ObtenerPorIdAsync, ObtenerPorTokenQRAsync, ObtenerTodosAsync, ActualizarEncargadoAsync, RegenerarQRAsync)
    - Create `Capa_Abstracciones/Interfaces/ICalificacionService.cs` (CrearCalificacionAsync, ObtenerPorEncargadoAsync, ObtenerPorEncargadoYRangoAsync)
    - Create `Capa_Abstracciones/Interfaces/IQRService.cs` (GenerarQRBase64)
    - Create `Capa_Abstracciones/Interfaces/IAuthService.cs` (AutenticarAsync)
    - _Requirements: 1.6, 2.5, 2.6, 1.9_

- [x] 3. Implement Capa_Datos (DbContext, repositories, SQL Server provider)
  - [x] 3.1 Create AppDbContext with EF Core configuration
    - Create `Capa_Datos/AppDbContext.cs` with DbSets for Encargados, Calificaciones, UsuariosAdmin
    - Configure Fluent API in OnModelCreating: table names, max lengths, required fields, unique index on TokenQR, unique index on NombreUsuario, cascade delete Encargado→Calificaciones
    - _Requirements: 1.7, 9.5_

  - [x] 3.2 Implement repository classes
    - Create `Capa_Datos/Repositories/EncargadoRepository.cs` implementing IEncargadoRepository with EF Core queries
    - Create `Capa_Datos/Repositories/CalificacionRepository.cs` implementing ICalificacionRepository including date range filter
    - Create `Capa_Datos/Repositories/UsuarioAdminRepository.cs` implementing IUsuarioAdminRepository
    - _Requirements: 1.7, 2.6_

- [x] 4. Implement Capa_Servicios (business logic services)
  - [x] 4.1 Implement EncargadoService
    - Create `Capa_Servicios/EncargadoService.cs` implementing IEncargadoService
    - Implement CrearEncargadoAsync: trim fields, generate TokenQR (Guid.NewGuid().ToString("N")), call IQRService.GenerarQRBase64 with frontend URL + token, persist via repository
    - Implement ObtenerPorIdAsync, ObtenerPorTokenQRAsync, ObtenerTodosAsync with NotFound handling
    - Implement ActualizarEncargadoAsync with existence check
    - Implement RegenerarQRAsync: invalidate old token, generate new GUID, regenerate QR image, update via repository
    - _Requirements: 2.1, 2.6, 2.8, 2.10_

  - [x] 4.2 Implement CalificacionService
    - Create `Capa_Servicios/CalificacionService.cs` implementing ICalificacionService
    - Implement CrearCalificacionAsync: validate non-empty Calificacion string, parse enum case-insensitive with Enum.TryParse, verify encargado exists, create and persist entity
    - Implement ObtenerPorEncargadoAsync and ObtenerPorEncargadoYRangoAsync
    - Return ValidationError with list of valid values when enum parse fails
    - Return NotFound when encargado doesn't exist
    - _Requirements: 2.2, 7.1, 7.2, 7.3, 7.5, 2.8_

  - [x] 4.3 Implement QRServiceImpl
    - Create `Capa_Servicios/QRServiceImpl.cs` implementing IQRService
    - Use QRCoder's QRCodeGenerator + PngByteQRCode with ECCLevel.Q, pixelsPerModule=20
    - Return Convert.ToBase64String of PNG bytes
    - _Requirements: 2.3_

  - [x] 4.4 Implement AuthService
    - Create `Capa_Servicios/AuthService.cs` implementing IAuthService
    - Implement AutenticarAsync: lookup user by username, verify password with BCrypt.Net.BCrypt.Verify, return generic error message for both user-not-found and wrong-password cases
    - _Requirements: 2.4, 3.4, 3.5_

- [x] 5. Checkpoint - Verify solution builds
  - Ensure all tests pass, ask the user if questions arise.

- [x] 6. Refactor API project (controllers, middleware, CORS, configuration)
  - [x] 6.1 Implement GlobalExceptionMiddleware
    - Create `ApiEncuestaPrototipe/Middleware/GlobalExceptionMiddleware.cs`
    - Log full exception details with ILogger (correlationId, type, message, stack trace)
    - Map ArgumentException/ValidationException → 400, others → 500
    - Return JSON with message, correlationId, timestamp; include detail (stack trace) only in Development
    - _Requirements: 8.1, 8.2, 8.3, 8.4, 8.5, 8.6_

  - [x] 6.2 Refactor Program.cs with DI registration, CORS, and configuration validation
    - Validate ConnectionStrings:DefaultConnection at startup (check for Server and Database segments, exit with error if missing)
    - Register DbContext with UseSqlServer
    - Register all repository and service interfaces as Scoped
    - Configure CORS reading AllowedOrigins from Cors:AllowedOrigins config section
    - Register middleware pipeline: GlobalExceptionMiddleware (first), UseCors, UseAuthorization, MapControllers
    - Add Swagger (AddEndpointsApiExplorer, AddSwaggerGen)
    - _Requirements: 9.1, 9.2, 9.3, 9.5, 11.1, 11.2, 11.3, 11.4, 11.5, 11.6_

  - [x] 6.3 Refactor EncargadoController and create CalificacionController
    - Refactor `ApiEncuestaPrototipe/Controllers/EncargadoController.cs`: inject IEncargadoService, delegate all logic to service, map ServiceResult to HTTP responses (Success→200, NotFound→404, ValidationError→400)
    - Create `ApiEncuestaPrototipe/Controllers/CalificacionController.cs`: POST /api/calificaciones with model validation, GET /api/calificaciones/encargado/{id}
    - Ensure ModelState validation returns 400 with errors dictionary as per requirement 10.8
    - _Requirements: 2.9, 7.1, 7.2, 7.3, 10.8_

  - [x] 6.4 Update appsettings files
    - Update `appsettings.json`: empty ConnectionStrings:DefaultConnection, AppSettings:FrontendUrl, empty Cors:AllowedOrigins array, Logging config (no credentials)
    - Update `appsettings.Development.json`: Cors:AllowedOrigins with localhost:5173 and localhost:3000
    - Create `appsettings.Production.json` with production origin placeholder
    - _Requirements: 9.1, 9.2, 11.1, 11.2, 11.3_

- [x] 7. Implement Panel_Admin (Windows Forms application)
  - [x] 7.1 Create LoginForm
    - Create `Panel_Admin/LoginForm.cs` and `Panel_Admin/LoginForm.Designer.cs`
    - Add TextBox for username (maxLength 50), password TextBox (maxLength 128, PasswordChar='*'), login button, error labels
    - Implement field validation (show error if empty without calling service)
    - Implement authentication via IAuthService.AutenticarAsync
    - Implement 5 failed attempts → 60 second lockout with countdown display
    - Handle connection errors with user-friendly message
    - On success: open MainForm, hide LoginForm
    - _Requirements: 3.1, 3.2, 3.3, 3.6, 3.7, 3.8, 3.9_

  - [x] 7.2 Create MainForm with Funcionarios tab
    - Create `Panel_Admin/MainForm.cs` and `Panel_Admin/MainForm.Designer.cs` with TabControl
    - Tab Funcionarios: DataGridView with columns (Nombre, Apellido, Cargo, Direccion), search TextBox, New/Edit buttons
    - Implement create funcionario form with field validation (Nombre and Apellido required, max lengths)
    - Implement edit funcionario: load selected row data into form, save changes
    - Implement search filter: filter DataGridView when ≥1 character typed (case-insensitive name/apellido match)
    - Handle save/edit errors with user-friendly message preserving form data
    - _Requirements: 4.1, 4.2, 4.3, 4.4, 4.5, 4.6, 4.7, 4.8_

  - [x] 7.3 Create MainForm QR tab
    - Tab QR: PictureBox for QR display, Generate QR / Regenerate QR / Save QR / Print QR buttons
    - Validate funcionario selection before generating QR
    - Call IEncargadoService.RegenerarQRAsync, decode Base64, display in PictureBox
    - Implement Save QR with SaveFileDialog (default name: {Apellido}_{Nombre}_QR.png)
    - Implement Print QR to default printer with error handling
    - Show existing QR when selecting a funcionario that already has one; toggle Generate/Regenerate button
    - _Requirements: 5.1, 5.2, 5.3, 5.4, 5.5, 5.6, 5.7_

  - [x] 7.4 Create MainForm Calificaciones tab
    - Tab Calificaciones: DataGridView with columns (FechaHora, Valor, Comentarios) ordered by FechaHora descending
    - Show "No hay calificaciones" message when empty
    - Display statistical summary: total count and count per ValorCalificacion
    - Implement date range filter (DateTimePicker for start/end)
    - Validate fechaInicio <= fechaFin, show error if invalid
    - Update statistics based on active filter
    - _Requirements: 6.1, 6.2, 6.3, 6.4, 6.5, 6.6_

  - [x] 7.5 Create Panel_Admin Program.cs with DI composition root
    - Configure ServiceCollection with DbContext (UseSqlServer), repositories, services
    - Read connection string from app.config (ConfigurationManager)
    - Build ServiceProvider and launch LoginForm with injected IAuthService
    - Create `Panel_Admin/App.config` with ConnectionStrings section (no real credentials)
    - _Requirements: 9.4, 1.4, 1.9_

- [x] 8. Checkpoint - Verify full solution builds
  - Ensure all tests pass, ask the user if questions arise.

- [x] 9. Implement unit tests (Pruebas project)
  - [x] 9.1 Write CalificacionService tests
    - Test CrearCalificacionAsync with valid values (Excelente, Buena, Regular, Mala) → Success
    - Test CrearCalificacionAsync with invalid value → ValidationError with value list
    - Test CrearCalificacionAsync with empty/null value → ValidationError "obligatorio"
    - Test CrearCalificacionAsync with non-existent encargado → NotFound
    - _Requirements: 12.1, 7.1, 7.2, 7.3_

  - [x] 9.2 Write property test for enum validation completeness
    - **Property 2: Enum Completeness** — ∀ calificación válida: calificación.Valor ∈ {Excelente, Buena, Regular, Mala}
    - **Validates: Requirements 7.1, 7.4**

  - [x] 9.3 Write EncargadoService tests
    - Test CrearEncargadoAsync → Success with TokenQR as 32 hex char GUID
    - Test two consecutive creations produce distinct TokenQR values
    - Test ObtenerPorTokenQRAsync with non-existent token → NotFound
    - _Requirements: 12.2, 2.1, 2.10_

  - [x] 9.4 Write property test for token uniqueness
    - **Property 1: Token Uniqueness** — ∀ encargado₁, encargado₂: id₁ ≠ id₂ ⟹ TokenQR₁ ≠ TokenQR₂
    - **Validates: Requirements 12.2, 5.1**

  - [x] 9.5 Write QRService tests
    - Test GenerarQRBase64 returns valid Base64 that decodes to PNG (signature 0x89504E47)
    - Test generated image is at least 300x300 pixels
    - _Requirements: 12.3, 5.1_

  - [x] 9.6 Write property test for QR validity
    - **Property 5: QR Validity** — ∀ qr generado: decode(qr).bytes[0..3] = [0x89, 0x50, 0x4E, 0x47]
    - **Validates: Requirements 12.3, 5.1**

  - [x] 9.7 Write AuthService tests
    - Test AutenticarAsync with valid credentials → AuthResult.Success
    - Test AutenticarAsync with wrong password → AuthResult.Failed
    - Test AutenticarAsync with non-existent user → AuthResult.Failed
    - Verify error messages are identical for wrong password and non-existent user
    - _Requirements: 12.4, 3.4, 3.5_

  - [x] 9.8 Write property test for auth security
    - **Property 3: Auth Security** — ∀ login_attempt: mensaje_error(usuario_inexistente) = mensaje_error(contraseña_incorrecta)
    - **Validates: Requirements 3.3, 3.5, 12.4**

  - [x] 9.9 Write Panel_Admin logic tests
    - Test login with valid credentials → authenticated
    - Test login with invalid credentials → failed
    - Test create funcionario with valid data → success
    - Test create funcionario with empty Nombre/Apellido → validation error
    - Test QR generation produces 32 hex char TokenQR and non-empty CodigoQR
    - Test two consecutive QR generations produce distinct TokenQR values
    - Test calificaciones filter by date range returns only matching records
    - Test calificaciones filter with no results returns empty collection
    - Test service exception is handled gracefully (does not throw uncontrolled)
    - _Requirements: 13.1, 13.2, 13.3, 13.4, 13.5_

- [x] 10. Checkpoint - Run all tests and verify coverage
  - Ensure all tests pass, ask the user if questions arise.

- [x] 11. Create project documentation
  - [x] 11.1 Write README.md
    - Document project purpose, tech stack (.NET 8, SQL Server, EF Core, QRCoder, Windows Forms, xUnit)
    - Include setup prerequisites and step-by-step configuration instructions
    - Include Mermaid flow diagram (login → gestión → QR → calificación)
    - Document all API endpoints in table format (Method, Route, Description, Request Body example, Success Response)
    - Document database schema (tables, columns, types, constraints, relationships)
    - Document solution structure (each project with responsibility and dependencies)
    - Include "Ejecución Local" section with separate instructions for API (dotnet run) and Panel_Admin, including User Secrets configuration
    - _Requirements: 14.1, 14.2, 14.3, 14.4, 14.5, 14.6_

- [x] 12. Final checkpoint - Ensure full build and all tests pass
  - Ensure all tests pass, ask the user if questions arise.

## Notes

- Tasks marked with `*` are optional and can be skipped for faster MVP
- Each task references specific requirements for traceability
- Checkpoints ensure incremental validation
- Property tests validate universal correctness properties from the design document
- Unit tests validate specific examples and edge cases
- The solution uses C# with .NET 8 throughout; Panel_Admin targets net8.0-windows for WinForms support
- SQL Server replaces PostgreSQL — all Npgsql references must be removed
- BCrypt.Net-Next is used for password hashing (cost factor ≥ 12)
- QRCoder generates PNG QR images with ECCLevel.Q

## Task Dependency Graph

```json
{
  "waves": [
    { "id": 0, "tasks": ["1.1"] },
    { "id": 1, "tasks": ["2.1", "2.2", "2.3"] },
    { "id": 2, "tasks": ["3.1", "3.2"] },
    { "id": 3, "tasks": ["4.1", "4.2", "4.3", "4.4"] },
    { "id": 4, "tasks": ["6.1", "6.2", "6.3", "6.4"] },
    { "id": 5, "tasks": ["7.1", "7.5"] },
    { "id": 6, "tasks": ["7.2", "7.3", "7.4"] },
    { "id": 7, "tasks": ["9.1", "9.2", "9.3", "9.4", "9.5", "9.6", "9.7", "9.8", "9.9"] },
    { "id": 8, "tasks": ["11.1"] }
  ]
}
```
