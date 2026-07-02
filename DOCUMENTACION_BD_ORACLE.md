

# Documentación de Base de Datos Oracle — Sistema GADICC Calificador

## Información General

| Parámetro | Valor |
|-----------|-------|
| Motor | Oracle Database 21c Enterprise Edition |
| Nombre BD Global | GADICC_PROJECT |
| SID | GADICCPROJEC |
| Host | localhost (PC-KODY) |
| Puerto | 1521 |
| Usuario Aplicación | gadicc_app |
| Password Aplicación | gadicc2026 |
| Usuario DBA | SYS |
| Tablespace | USERS |
| Character Set | AL32UTF8 |

### Connection String (.NET)

```
Data Source=localhost:1521/GADICC_PROJECT;User Id=gadicc_app;Password=gadicc2026;
```

---

## Modelo Entidad-Relación

```mermaid
erDiagram
    PERSONA ||--o| EMPLEADO : "herencia (1:1)"
    EMPLEADO ||--o| EMPLEADO_QR : "tiene QR (1:1)"
    EMPLEADO ||--o{ CALIFICACIONES : "recibe (1:N)"

    PERSONA {
        NVARCHAR2_14 CEDULARUCPERSONA PK "Cedula o RUC"
        NVARCHAR2_100 PRIMERNOMBREPERSONA "Primer nombre"
        NVARCHAR2_100 SEGUNDONOMBREPERSONA "Segundo nombre"
        NVARCHAR2_100 PRIMERAPELLIDOPERSONA "Primer apellido"
        NVARCHAR2_100 SEGUNDOAPELLIDOPERSONA "Segundo apellido"
        NVARCHAR2_300 DIRECCIONPERSONA "Direccion"
        NVARCHAR2_20 MOVIL1PERSONA "Celular"
        NVARCHAR2_100 EMAIL1PERSONA "Email"
        NUMBER_1 ACTIVO "1=Activo 0=Inactivo"
    }

    EMPLEADO {
        NVARCHAR2_14 CEDULARUCPERSONA PK_FK "Cedula (FK a PERSONA)"
        VARCHAR2_250 NOTASEMPLEADO "Notas"
        NUMBER IDDEPARTAMENTO "FK departamento"
        VARCHAR2_100 CARGOEMPLEADO "Cargo"
        NUMBER IDTIPOFUNCIONARIO "Tipo funcionario"
        NUMBER EMPLEADOACTIVO "1=Activo"
        VARCHAR2_50 CODIGOSECTORIAL "Codigo sectorial"
        VARCHAR2_150 TITULOPROFESIONAL "Titulo"
    }

    EMPLEADO_QR {
        NVARCHAR2_14 CEDULARUCPERSONA PK_FK "Cedula (FK a EMPLEADO)"
        NVARCHAR2_32 TOKENQR UK "Token GUID unico"
        CLOB CODIGOQR "Imagen QR Base64"
        TIMESTAMP FECHAGENERACION "Fecha generacion"
        NUMBER_1 ACTIVO "1=Activo"
    }

    CALIFICACIONES {
        NUMBER IDCALIFICACION PK "ID auto (secuencia)"
        NVARCHAR2_14 CEDULARUCPERSONA FK "Cedula empleado"
        NUMBER_1 VALOR "1-4 (Excelente a Mala)"
        NVARCHAR2_500 COMENTARIOS "Comentario ciudadano"
        TIMESTAMP FECHAHORA "Fecha UTC servidor"
        VARCHAR2_45 IPCLIENTE "IP dispositivo"
        VARCHAR2_16 DEVICEFINGERPRINT "Fingerprint"
        VARCHAR2_500 USERAGENT "Navegador"
        TIMESTAMP FECHACLIENTE "Hora dispositivo"
    }

    USUARIOS_ADMIN {
        NUMBER IDUSUARIO PK "ID auto (secuencia)"
        NVARCHAR2_50 NOMBREUSUARIO UK "Username"
        NVARCHAR2_200 PASSWORDHASH "BCrypt hash"
        TIMESTAMP FECHACREACION "Fecha creacion"
    }
```

---

## Detalle de Tablas

### 1. PERSONA

Tabla maestra de datos personales. Contiene a todas las personas registradas en el sistema del municipio.

| # | Columna | Tipo | Tamaño | Nulo | PK | FK | Descripción |
|---|---------|------|--------|:----:|:--:|:--:|-------------|
| 1 | CEDULARUCPERSONA | NVARCHAR2 | 14 | NO | ✅ | | Cédula o RUC (identificador único) |
| 2 | PRIMERNOMBREPERSONA | NVARCHAR2 | 100 | SÍ | | | Primer nombre |
| 3 | SEGUNDONOMBREPERSONA | NVARCHAR2 | 100 | SÍ | | | Segundo nombre |
| 4 | PRIMERAPELLIDOPERSONA | NVARCHAR2 | 100 | SÍ | | | Primer apellido |
| 5 | SEGUNDOAPELLIDOPERSONA | NVARCHAR2 | 100 | SÍ | | | Segundo apellido |
| 6 | DIRECCIONPERSONA | NVARCHAR2 | 300 | SÍ | | | Dirección domicilio |
| 7 | MOVIL1PERSONA | NVARCHAR2 | 20 | SÍ | | | Número celular principal |
| 8 | EMAIL1PERSONA | NVARCHAR2 | 100 | SÍ | | | Email principal |
| 9 | ACTIVO | NUMBER(1) | 1 | SÍ | | | 1=Activo, 0=Inactivo |

**Constraints:**
- `PK_PERSONA` — PRIMARY KEY en CEDULARUCPERSONA

---

### 2. EMPLEADO

Tabla de empleados del municipio. Hereda de PERSONA mediante FK en la PK (patrón Table-per-Type).

| # | Columna | Tipo | Tamaño | Nulo | PK | FK | Descripción |
|---|---------|------|--------|:----:|:--:|:--:|-------------|
| 1 | CEDULARUCPERSONA | NVARCHAR2 | 14 | NO | ✅ | → PERSONA | Cédula (herencia) |
| 2 | NOTASEMPLEADO | VARCHAR2 | 250 | SÍ | | | Notas sobre el empleado |
| 3 | IDDEPARTAMENTO | NUMBER | — | SÍ | | | ID del departamento |
| 4 | CARGOEMPLEADO | VARCHAR2 | 100 | SÍ | | | Cargo que desempeña |
| 5 | IDTIPOFUNCIONARIO | NUMBER | — | SÍ | | | Tipo de funcionario |
| 6 | EMPLEADOACTIVO | NUMBER | — | SÍ | | | 1=Activo, 0=Inactivo |
| 7 | CODIGOSECTORIAL | VARCHAR2 | 50 | SÍ | | | Código sectorial |
| 8 | TITULOPROFESIONAL | VARCHAR2 | 150 | SÍ | | | Título académico/profesional |

**Constraints:**
- `PK_EMPLEADO` — PRIMARY KEY en CEDULARUCPERSONA
- `FK_EMPLEADO_PERSONA` — FOREIGN KEY (CEDULARUCPERSONA) REFERENCES PERSONA(CEDULARUCPERSONA)

**Relación con PERSONA:** Cada empleado ES una persona. La cédula es compartida como PK en ambas tablas (herencia por FK).

---

### 3. EMPLEADO_QR

Tabla que vincula cada empleado con su código QR para el sistema de encuestas. Relación 1:1 con EMPLEADO.

| # | Columna | Tipo | Tamaño | Nulo | PK | FK | Descripción |
|---|---------|------|--------|:----:|:--:|:--:|-------------|
| 1 | CEDULARUCPERSONA | NVARCHAR2 | 14 | NO | ✅ | → EMPLEADO | Cédula del empleado |
| 2 | TOKENQR | NVARCHAR2 | 32 | NO | | | Token GUID único (32 hex chars) |
| 3 | CODIGOQR | CLOB | — | SÍ | | | Imagen QR codificada en Base64 PNG |
| 4 | FECHAGENERACION | TIMESTAMP | — | NO | | | Fecha/hora de generación |
| 5 | ACTIVO | NUMBER(1) | 1 | NO | | | 1=QR vigente, 0=Invalidado |

**Constraints:**
- `PK_EMPLEADO_QR` — PRIMARY KEY en CEDULARUCPERSONA
- `FK_EMPQR_EMPLEADO` — FOREIGN KEY (CEDULARUCPERSONA) REFERENCES EMPLEADO(CEDULARUCPERSONA)
- `UQ_EMPLEADO_QR_TOKEN` — UNIQUE en TOKENQR

**Propósito:** Cuando un encargado genera el QR desde el Panel Admin, se crea un registro aquí. El TOKENQR se usa en la URL de la encuesta: `http://{IP}:5173/encuesta/{TOKENQR}`

---

### 4. CALIFICACIONES

Almacena cada calificación enviada por un ciudadano después de escanear el QR y completar la encuesta.

| # | Columna | Tipo | Tamaño | Nulo | PK | FK | Descripción |
|---|---------|------|--------|:----:|:--:|:--:|-------------|
| 1 | IDCALIFICACION | NUMBER | — | NO | ✅ | | ID secuencial (SEQ_CALIFICACIONES) |
| 2 | CEDULARUCPERSONA | NVARCHAR2 | 14 | NO | | → EMPLEADO | Cédula del empleado calificado |
| 3 | VALOR | NUMBER(1) | 1 | NO | | | Calificación: 1=Excelente, 2=Buena, 3=Regular, 4=Mala |
| 4 | COMENTARIOS | NVARCHAR2 | 500 | SÍ | | | Comentario libre del ciudadano |
| 5 | FECHAHORA | TIMESTAMP | — | NO | | | Fecha/hora UTC del servidor al guardar |
| 6 | IPCLIENTE | VARCHAR2 | 45 | SÍ | | | Dirección IP del dispositivo |
| 7 | DEVICEFINGERPRINT | VARCHAR2 | 16 | SÍ | | | Hash del dispositivo (fingerprint 8 chars) |
| 8 | USERAGENT | VARCHAR2 | 500 | SÍ | | | User-Agent del navegador completo |
| 9 | FECHACLIENTE | TIMESTAMP | — | SÍ | | | Hora reportada por el dispositivo del ciudadano |

**Constraints:**
- `PK_CALIFICACIONES` — PRIMARY KEY en IDCALIFICACION
- `FK_CALIF_EMPLEADO` — FOREIGN KEY (CEDULARUCPERSONA) REFERENCES EMPLEADO(CEDULARUCPERSONA)
- `CK_CALIF_VALOR` — CHECK (VALOR IN (1, 2, 3, 4))

**Secuencia:** `SEQ_CALIFICACIONES` — auto-incremento via trigger `TRG_CALIFICACIONES_ID`

**Índices:**
- `IDX_CALIF_EMPLEADO` — en CEDULARUCPERSONA (búsqueda por empleado)
- `IDX_CALIF_FECHA` — en FECHAHORA DESC (ordenamiento cronológico)

**Valores del campo VALOR:**
| Código | Significado | Color en UI |
|:------:|-------------|-------------|
| 1 | Excelente | Verde oscuro (#2E7D32) |
| 2 | Buena | Verde claro (#4CAF50) |
| 3 | Regular | Naranja (#FF9800) |
| 4 | Mala | Rojo (#D32F2F) |

---

### 5. USUARIOS_ADMIN

Usuarios del Panel Administrativo de Escritorio (Windows Forms).

| # | Columna | Tipo | Tamaño | Nulo | PK | FK | Descripción |
|---|---------|------|--------|:----:|:--:|:--:|-------------|
| 1 | IDUSUARIO | NUMBER | — | NO | ✅ | | ID secuencial (SEQ_USUARIOS_ADMIN) |
| 2 | NOMBREUSUARIO | NVARCHAR2 | 50 | NO | | | Username para login |
| 3 | PASSWORDHASH | NVARCHAR2 | 200 | NO | | | Hash BCrypt (cost factor ≥ 12) |
| 4 | FECHACREACION | TIMESTAMP | — | NO | | | Fecha de creación del usuario |

**Constraints:**
- `PK_USUARIOS_ADMIN` — PRIMARY KEY en IDUSUARIO
- `UQ_USUARIOS_NOMBRE` — UNIQUE en NOMBREUSUARIO

**Secuencia:** `SEQ_USUARIOS_ADMIN` — auto-incremento via trigger `TRG_USUARIOS_ADMIN_ID`

**Usuario por defecto:**
| Username | Password | Hash |
|----------|----------|------|
| admin | Admin123! | BCrypt cost 12 |

---

## Vista: V_CALIFICACIONES_EMPLEADO

Vista que une CALIFICACIONES + EMPLEADO + PERSONA para consultas de reportes.

```sql
SELECT
    c.IDCALIFICACION,
    c.CEDULARUCPERSONA,
    p.PRIMERNOMBREPERSONA || ' ' || p.PRIMERAPELLIDOPERSONA AS NOMBRE_COMPLETO,
    e.CARGOEMPLEADO,
    CASE c.VALOR
        WHEN 1 THEN 'Excelente'
        WHEN 2 THEN 'Buena'
        WHEN 3 THEN 'Regular'
        WHEN 4 THEN 'Mala'
    END AS VALOR_TEXTO,
    c.VALOR,
    c.COMENTARIOS,
    c.FECHAHORA,
    c.IPCLIENTE,
    c.DEVICEFINGERPRINT
FROM CALIFICACIONES c
INNER JOIN EMPLEADO e ON c.CEDULARUCPERSONA = e.CEDULARUCPERSONA
INNER JOIN PERSONA p ON e.CEDULARUCPERSONA = p.CEDULARUCPERSONA;
```

---

## Secuencias

| Secuencia | Tabla | Columna | Inicio | Incremento |
|-----------|-------|---------|:------:|:----------:|
| SEQ_CALIFICACIONES | CALIFICACIONES | IDCALIFICACION | 1 | 1 |
| SEQ_USUARIOS_ADMIN | USUARIOS_ADMIN | IDUSUARIO | 1 | 1 |

---

## Triggers

| Trigger | Tabla | Evento | Función |
|---------|-------|--------|---------|
| TRG_CALIFICACIONES_ID | CALIFICACIONES | BEFORE INSERT | Asigna NEXTVAL de SEQ_CALIFICACIONES si ID es NULL |
| TRG_USUARIOS_ADMIN_ID | USUARIOS_ADMIN | BEFORE INSERT | Asigna NEXTVAL de SEQ_USUARIOS_ADMIN si ID es NULL |

---

## Relaciones Detalladas

### Herencia PERSONA → EMPLEADO
- **Tipo:** Table-per-Type (TPT)
- **Cardinalidad:** 1:1 (una persona puede ser o no empleado)
- **FK:** EMPLEADO.CEDULARUCPERSONA → PERSONA.CEDULARUCPERSONA
- **Significado:** Todo empleado ES una persona; los datos personales se leen de PERSONA

### EMPLEADO → EMPLEADO_QR
- **Tipo:** 1:1 opcional
- **Cardinalidad:** Un empleado puede tener 0 o 1 QR activo
- **FK:** EMPLEADO_QR.CEDULARUCPERSONA → EMPLEADO.CEDULARUCPERSONA
- **Significado:** El QR se genera bajo demanda desde el Panel Admin

### EMPLEADO → CALIFICACIONES
- **Tipo:** 1:N (uno a muchos)
- **Cardinalidad:** Un empleado puede recibir 0 o muchas calificaciones
- **FK:** CALIFICACIONES.CEDULARUCPERSONA → EMPLEADO.CEDULARUCPERSONA
- **Significado:** Cada vez que un ciudadano escanea el QR y califica, se crea un registro

---

## Flujo de Datos

```
1. Encargado crea QR desde Panel Admin
   → INSERT en EMPLEADO_QR (token + imagen Base64)

2. Ciudadano escanea QR
   → Frontend lee token de la URL
   → GET /api/encargados/token/{token}
   → SELECT en EMPLEADO_QR (busca por TOKENQR)
   → JOIN EMPLEADO + PERSONA para obtener nombre/cargo

3. Ciudadano envía calificación
   → POST /api/calificaciones
   → INSERT en CALIFICACIONES (con IP, fingerprint, timestamp)

4. Encargado consulta reportes en Panel Admin
   → SELECT V_CALIFICACIONES_EMPLEADO
```

---

## Datos de Prueba Actuales

### PERSONA
| Cédula | Nombre | Apellido |
|--------|--------|----------|
| 0102030405 | Carlos | Gonzalez |
| 0605040302 | Mateo | Ortiz |

### EMPLEADO
| Cédula | Cargo | Activo |
|--------|-------|:------:|
| 0102030405 | Atencion al Ciudadano | 1 |
| 0605040302 | Ventanilla Unica | 1 |

### USUARIOS_ADMIN
| Username | Password |
|----------|----------|
| admin | Admin123! |

---

## Comandos Útiles

### Conectar a la BD
```bash
sqlplus gadicc_app/gadicc2026@localhost:1521/GADICC_PROJECT
```

### Ver tablas
```sql
SELECT TABLE_NAME FROM USER_TABLES ORDER BY TABLE_NAME;
```

### Ver calificaciones con datos del empleado
```sql
SELECT * FROM V_CALIFICACIONES_EMPLEADO;
```

### Contar calificaciones por empleado
```sql
SELECT CEDULARUCPERSONA, COUNT(*) AS TOTAL
FROM CALIFICACIONES
GROUP BY CEDULARUCPERSONA
ORDER BY TOTAL DESC;
```

### Ver QR activos
```sql
SELECT eq.CEDULARUCPERSONA, p.PRIMERNOMBREPERSONA, eq.TOKENQR, eq.FECHAGENERACION
FROM EMPLEADO_QR eq
JOIN PERSONA p ON eq.CEDULARUCPERSONA = p.CEDULARUCPERSONA
WHERE eq.ACTIVO = 1;
```
