# Migración a Oracle Database

## Resumen de Cambios

Este documento detalla la migración del sistema de calificaciones desde SQL Server hacia Oracle Database, adaptándose al esquema existente del municipio (tablas PERSONA y EMPLEADO).

## 1. Esquema de Base de Datos Oracle

### Tablas Existentes (del municipio)

#### PERSONA (tabla padre - datos personales)
| Columna | Tipo | PK | Not Null | Descripción |
|---------|------|:--:|:--------:|-------------|
| CEDULARUCPERSONA | NVARCHAR2(14) | ✅ | ✅ | Cédula o RUC |
| IDPARROQUIA | NUMBER | | | FK a parroquia |
| IDESTADOCIVIL | NUMBER | | | FK a estado civil |
| IDSEXO | NUMBER | | | FK a sexo |
| PRIMERNOMBREPERSONA | NVARCHAR2(100) | | | Primer nombre |
| SEGUNDONOMBREPERSONA | NVARCHAR2(100) | | | Segundo nombre |
| PRIMERAPELLIDOPERSONA | NVARCHAR2(100) | | | Primer apellido |
| SEGUNDOAPELLIDOPERSONA | NVARCHAR2(100) | | | Segundo apellido |
| FECHANACIMIENTOPERSONA | DATE | | | Fecha de nacimiento |
| DIRECCIONPERSONA | NVARCHAR2(300) | | | Dirección |
| TELEFONO1PERSONA | NVARCHAR2(20) | | | Teléfono 1 |
| TELEFONO2PERSONA | NVARCHAR2(20) | | | Teléfono 2 |
| MOVIL1PERSONA | NVARCHAR2(20) | | | Celular 1 |
| MOVIL2PERSONA | NVARCHAR2(20) | | | Celular 2 |
| EMAIL1PERSONA | NVARCHAR2(100) | | | Email 1 |
| EMAIL2PERSONA | NVARCHAR2(100) | | | Email 2 |
| OBSERVACIONESPERSONA | NVARCHAR2(500) | | | Observaciones |
| CEDULAREAL PERSONA | NVARCHAR2(14) | | | Cédula real |
| TERCERAEDAD | NUMBER(1) | | | Tercera edad (1/0) |
| DISCAPACIDAD | NUMBER(1) | | | Tiene discapacidad (1/0) |
| PORCENTAJEDISCAPACIDAD | NUMBER | | | % discapacidad |
| NATURALEZAID | NUMBER | | | Naturaleza |
| FALLECIDO | NUMBER(1) | | | Fallecido (1/0) |
| FECHADEFUNCION | DATE | | | Fecha defunción |
| ACTIVO | NUMBER(1) | | | Activo (1/0) |
| TOKENRECOVERY | NVARCHAR2(200) | | | Token recuperación |

#### EMPLEADO (tabla hija - hereda de PERSONA)
| Columna | Tipo | PK/FK | Not Null | Descripción |
|---------|------|:-----:|:--------:|-------------|
| CEDULARUCPERSONA | NVARCHAR2(14) | PK + FK→PERSONA | ✅ | Cédula (herencia) |
| NOTASEMPLEADO | VARCHAR2(250) | | | Notas |
| IDDEPARTAMENTO | NUMBER | | | FK a departamento |
| IMAGENEMPLEADO | BLOB(4000) | | | Foto del empleado |
| CARGOEMPLEADO | VARCHAR2(100) | | | Cargo |
| IDDISCAPACIDAD | NUMBER | | | FK a discapacidad |
| IDTIPOFUNCIONARIO | NUMBER | | | Tipo de funcionario |
| EMPLEADOACTIVO | NUMBER | | | Activo (1/0) |
| IDFUNCIONARIORELOJ | NUMBER | | | ID reloj biométrico |
| TALLA | VARCHAR2(6) | | | Talla uniforme |
| CODIGOSECTORIAL | VARCHAR2(50) | | | Código sectorial |
| CARGAFAMILIAR | VARCHAR2(30) | | | Cargas familiares |
| TITULOPROFESIONAL | VARCHAR2(150) | | | Título profesional |

### Tablas Nuevas (creadas para el sistema de calificaciones)

#### CALIFICACIONES
| Columna | Tipo | PK/FK | Not Null | Descripción |
|---------|------|:-----:|:--------:|-------------|
| IDCALIFICACION | NUMBER | PK (Sequence) | ✅ | ID auto-incremental |
| CEDULARUCPERSONA | NVARCHAR2(14) | FK→EMPLEADO | ✅ | Cédula del empleado calificado |
| VALOR | NUMBER(1) | | ✅ | 1=Excelente, 2=Buena, 3=Regular, 4=Mala |
| COMENTARIOS | NVARCHAR2(500) | | | Comentario del ciudadano |
| FECHAHORA | TIMESTAMP | | ✅ | Fecha/hora UTC del servidor |
| IPCLIENTE | VARCHAR2(45) | | | IP del dispositivo |
| DEVICEFINGERPRINT | VARCHAR2(16) | | | Fingerprint del dispositivo |
| USERAGENT | VARCHAR2(500) | | | Navegador/dispositivo |
| FECHACLIENTE | TIMESTAMP | | | Hora del dispositivo |
| TOKENQR | NVARCHAR2(32) | UNIQUE | | Token QR del empleado |
| CODIGOQR | CLOB | | | Imagen QR en Base64 |

#### USUARIOS_ADMIN
| Columna | Tipo | PK/FK | Not Null | Descripción |
|---------|------|:-----:|:--------:|-------------|
| IDUSUARIO | NUMBER | PK (Sequence) | ✅ | ID auto-incremental |
| NOMBREUSUARIO | NVARCHAR2(50) | UNIQUE | ✅ | Username para login |
| PASSWORDHASH | NVARCHAR2(200) | | ✅ | BCrypt hash |
| FECHACREACION | TIMESTAMP | | ✅ | Fecha creación |

### Relaciones

```
PERSONA (1) ←──── (1) EMPLEADO     [Herencia por FK - CEDULARUCPERSONA]
EMPLEADO (1) ────── (N) CALIFICACIONES [FK - CEDULARUCPERSONA]
```

- EMPLEADO.CEDULARUCPERSONA → PERSONA.CEDULARUCPERSONA
- CALIFICACIONES.CEDULARUCPERSONA → EMPLEADO.CEDULARUCPERSONA
- No se necesita tabla intermedia: es una relación 1:N directa (un empleado tiene muchas calificaciones)

## 2. Cambios en el Proyecto .NET

### Paquetes NuGet

| Proyecto | Antes (SQL Server) | Ahora (Oracle) |
|----------|-------------------|----------------|
| Capa_Datos | Microsoft.EntityFrameworkCore.SqlServer 8.0.11 | Oracle.EntityFrameworkCore 8.23.60 |
| Capa_Datos | Microsoft.EntityFrameworkCore.Design 8.0.11 | Microsoft.EntityFrameworkCore.Design 8.0.11 (se mantiene) |
| Panel_Admin | Microsoft.EntityFrameworkCore.SqlServer 8.0.11 | Oracle.EntityFrameworkCore 8.23.60 |

### Connection String Oracle

```
Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=<IP_ORACLE>)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=<SERVICE>)));User Id=<USUARIO>;Password=<CLAVE>;
```

O formato simplificado:
```
Data Source=<HOST>:1521/<SERVICE_NAME>;User Id=<USUARIO>;Password=<CLAVE>;
```

### Cambios en Program.cs

```csharp
// Antes (SQL Server):
options.UseSqlServer(connectionString)

// Ahora (Oracle):
options.UseOracle(connectionString)
```

### Cambios en Entidades

La entidad `Encargado` se reemplaza por `Empleado` que hereda datos de `Persona`:
- PK cambia de `int IdEncargado` a `string CedulaRucPersona` (NVARCHAR2(14))
- Se agregan los campos del esquema existente

## 3. Script SQL Oracle

Ver archivo: `setup_oracle.sql`
