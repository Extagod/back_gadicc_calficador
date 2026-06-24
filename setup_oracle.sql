-- ============================================================
-- SCRIPT DE BASE DE DATOS ORACLE
-- Sistema de Calificación de Funcionarios Municipales (GADICC)
-- ============================================================
-- Este script crea las tablas necesarias para el sistema de
-- calificaciones QR. Las tablas PERSONA y EMPLEADO ya existen
-- en la BD del municipio; este script solo crea las tablas
-- nuevas (CALIFICACIONES, USUARIOS_ADMIN) y sus relaciones.
-- ============================================================

-- ============================================================
-- SECCION 1: TABLAS EXISTENTES (REFERENCIA - NO EJECUTAR)
-- Estas tablas ya existen en la BD del municipio.
-- Se documentan aquí para referencia del desarrollador.
-- ============================================================

/*
-- TABLA PERSONA (YA EXISTE - NO CREAR)
CREATE TABLE PERSONA (
    CEDULARUCPERSONA        NVARCHAR2(14)   NOT NULL,
    IDPARROQUIA             NUMBER,
    IDESTADOCIVIL           NUMBER,
    IDSEXO                  NUMBER,
    PRIMERNOMBREPERSONA     NVARCHAR2(100),
    SEGUNDONOMBREPERSONA    NVARCHAR2(100),
    PRIMERAPELLIDOPERSONA   NVARCHAR2(100),
    SEGUNDOAPELLIDOPERSONA  NVARCHAR2(100),
    FECHANACIMIENTOPERSONA  DATE,
    DIRECCIONPERSONA        NVARCHAR2(300),
    TELEFONO1PERSONA        NVARCHAR2(20),
    TELEFONO2PERSONA        NVARCHAR2(20),
    MOVIL1PERSONA           NVARCHAR2(20),
    MOVIL2PERSONA           NVARCHAR2(20),
    EMAIL1PERSONA           NVARCHAR2(100),
    EMAIL2PERSONA           NVARCHAR2(100),
    OBSERVACIONESPERSONA    NVARCHAR2(500),
    CEDULAREAL PERSONA      NVARCHAR2(14),
    TERCERAEDAD             NUMBER(1)       DEFAULT 0,
    DISCAPACIDAD            NUMBER(1)       DEFAULT 0,
    PORCENTAJEDISCAPACIDAD  NUMBER,
    NATURALEZAID            NUMBER,
    FALLECIDO               NUMBER(1)       DEFAULT 0,
    FECHADEFUNCION          DATE,
    ACTIVO                  NUMBER(1)       DEFAULT 1,
    TOKENRECOVERY           NVARCHAR2(200),
    CONSTRAINT PK_PERSONA PRIMARY KEY (CEDULARUCPERSONA)
);

-- TABLA EMPLEADO (YA EXISTE - NO CREAR)
CREATE TABLE EMPLEADO (
    CEDULARUCPERSONA    NVARCHAR2(14)   NOT NULL,
    NOTASEMPLEADO       VARCHAR2(250),
    IDDEPARTAMENTO      NUMBER,
    IMAGENEMPLEADO      BLOB,
    CARGOEMPLEADO       VARCHAR2(100),
    IDDISCAPACIDAD      NUMBER,
    IDTIPOFUNCIONARIO   NUMBER,
    EMPLEADOACTIVO      NUMBER          DEFAULT 1,
    IDFUNCIONARIORELOJ  NUMBER,
    TALLA               VARCHAR2(6),
    CODIGOSECTORIAL     VARCHAR2(50),
    CARGAFAMILIAR       VARCHAR2(30),
    TITULOPROFESIONAL   VARCHAR2(150),
    CONSTRAINT PK_EMPLEADO PRIMARY KEY (CEDULARUCPERSONA),
    CONSTRAINT FK_EMPLEADO_PERSONA FOREIGN KEY (CEDULARUCPERSONA)
        REFERENCES PERSONA(CEDULARUCPERSONA)
);
*/

-- ============================================================
-- SECCION 2: TABLAS NUEVAS (EJECUTAR ESTO)
-- ============================================================

-- 2.1 Tabla de QR por empleado (vincula token QR al empleado)
-- Cada empleado puede tener un QR único para recibir calificaciones
CREATE TABLE EMPLEADO_QR (
    CEDULARUCPERSONA    NVARCHAR2(14)   NOT NULL,
    TOKENQR             NVARCHAR2(32)   NOT NULL,
    CODIGOQR            CLOB,
    FECHAGENERACION     TIMESTAMP       DEFAULT SYSTIMESTAMP NOT NULL,
    ACTIVO              NUMBER(1)       DEFAULT 1 NOT NULL,
    CONSTRAINT PK_EMPLEADO_QR PRIMARY KEY (CEDULARUCPERSONA),
    CONSTRAINT FK_EMPQR_EMPLEADO FOREIGN KEY (CEDULARUCPERSONA)
        REFERENCES EMPLEADO(CEDULARUCPERSONA),
    CONSTRAINT UQ_EMPLEADO_QR_TOKEN UNIQUE (TOKENQR)
);

-- Índice para búsqueda por token QR
CREATE INDEX IDX_EMPQR_TOKEN ON EMPLEADO_QR(TOKENQR);

-- Comentarios de la tabla
COMMENT ON TABLE EMPLEADO_QR IS 'Códigos QR generados para cada empleado del municipio';
COMMENT ON COLUMN EMPLEADO_QR.CEDULARUCPERSONA IS 'Cédula del empleado (FK a EMPLEADO)';
COMMENT ON COLUMN EMPLEADO_QR.TOKENQR IS 'Token GUID único de 32 caracteres hex para la URL de encuesta';
COMMENT ON COLUMN EMPLEADO_QR.CODIGOQR IS 'Imagen QR en Base64 PNG';
COMMENT ON COLUMN EMPLEADO_QR.FECHAGENERACION IS 'Fecha/hora de generación del QR';
COMMENT ON COLUMN EMPLEADO_QR.ACTIVO IS '1=Activo, 0=Inactivo (QR anterior invalidado)';

-- 2.2 Tabla de calificaciones
CREATE TABLE CALIFICACIONES (
    IDCALIFICACION      NUMBER          NOT NULL,
    CEDULARUCPERSONA    NVARCHAR2(14)   NOT NULL,
    VALOR               NUMBER(1)       NOT NULL,
    COMENTARIOS         NVARCHAR2(500),
    FECHAHORA           TIMESTAMP       DEFAULT SYSTIMESTAMP NOT NULL,
    IPCLIENTE           VARCHAR2(45),
    DEVICEFINGERPRINT   VARCHAR2(16),
    USERAGENT           VARCHAR2(500),
    FECHACLIENTE        TIMESTAMP,
    CONSTRAINT PK_CALIFICACIONES PRIMARY KEY (IDCALIFICACION),
    CONSTRAINT FK_CALIF_EMPLEADO FOREIGN KEY (CEDULARUCPERSONA)
        REFERENCES EMPLEADO(CEDULARUCPERSONA),
    CONSTRAINT CK_CALIF_VALOR CHECK (VALOR IN (1, 2, 3, 4))
);

-- Secuencia para auto-incremento de IDCALIFICACION
CREATE SEQUENCE SEQ_CALIFICACIONES
    START WITH 1
    INCREMENT BY 1
    NOCACHE
    NOCYCLE;

-- Trigger para auto-incremento
CREATE OR REPLACE TRIGGER TRG_CALIFICACIONES_ID
BEFORE INSERT ON CALIFICACIONES
FOR EACH ROW
BEGIN
    IF :NEW.IDCALIFICACION IS NULL THEN
        :NEW.IDCALIFICACION := SEQ_CALIFICACIONES.NEXTVAL;
    END IF;
END;
/

-- Índices
CREATE INDEX IDX_CALIF_EMPLEADO ON CALIFICACIONES(CEDULARUCPERSONA);
CREATE INDEX IDX_CALIF_FECHA ON CALIFICACIONES(FECHAHORA DESC);

-- Comentarios
COMMENT ON TABLE CALIFICACIONES IS 'Calificaciones de encuestas enviadas por ciudadanos';
COMMENT ON COLUMN CALIFICACIONES.VALOR IS '1=Excelente, 2=Buena, 3=Regular, 4=Mala';
COMMENT ON COLUMN CALIFICACIONES.IPCLIENTE IS 'IP del dispositivo que envió la calificación';
COMMENT ON COLUMN CALIFICACIONES.DEVICEFINGERPRINT IS 'Huella digital del dispositivo (hash 8 chars)';
COMMENT ON COLUMN CALIFICACIONES.USERAGENT IS 'Navegador y SO del dispositivo';

-- 2.3 Tabla de usuarios administrativos (login del Panel de Escritorio)
CREATE TABLE USUARIOS_ADMIN (
    IDUSUARIO       NUMBER          NOT NULL,
    NOMBREUSUARIO   NVARCHAR2(50)   NOT NULL,
    PASSWORDHASH    NVARCHAR2(200)  NOT NULL,
    FECHACREACION   TIMESTAMP       DEFAULT SYSTIMESTAMP NOT NULL,
    CONSTRAINT PK_USUARIOS_ADMIN PRIMARY KEY (IDUSUARIO),
    CONSTRAINT UQ_USUARIOS_NOMBRE UNIQUE (NOMBREUSUARIO)
);

-- Secuencia para auto-incremento
CREATE SEQUENCE SEQ_USUARIOS_ADMIN
    START WITH 1
    INCREMENT BY 1
    NOCACHE
    NOCYCLE;

-- Trigger para auto-incremento
CREATE OR REPLACE TRIGGER TRG_USUARIOS_ADMIN_ID
BEFORE INSERT ON USUARIOS_ADMIN
FOR EACH ROW
BEGIN
    IF :NEW.IDUSUARIO IS NULL THEN
        :NEW.IDUSUARIO := SEQ_USUARIOS_ADMIN.NEXTVAL;
    END IF;
END;
/

-- Comentarios
COMMENT ON TABLE USUARIOS_ADMIN IS 'Usuarios administrativos para el Panel de Escritorio';
COMMENT ON COLUMN USUARIOS_ADMIN.PASSWORDHASH IS 'Hash BCrypt con cost factor >= 12';

-- ============================================================
-- SECCION 3: DATOS INICIALES
-- ============================================================

-- Usuario admin por defecto (contraseña: Admin123!)
-- Hash BCrypt generado con cost factor 12
INSERT INTO USUARIOS_ADMIN (IDUSUARIO, NOMBREUSUARIO, PASSWORDHASH, FECHACREACION)
VALUES (SEQ_USUARIOS_ADMIN.NEXTVAL, 'admin', '$2a$12$boDZnBuVosLhLIeyxwk2ZOULaEamHoc9U7LFQ530D36bR9gwor0u6', SYSTIMESTAMP);

COMMIT;

-- ============================================================
-- SECCION 4: VISTA PARA CONSULTA DE CALIFICACIONES CON DATOS DEL EMPLEADO
-- ============================================================

CREATE OR REPLACE VIEW V_CALIFICACIONES_EMPLEADO AS
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
    c.DEVICEFINGERPRINT,
    c.USERAGENT,
    c.FECHACLIENTE,
    eq.TOKENQR
FROM CALIFICACIONES c
INNER JOIN EMPLEADO e ON c.CEDULARUCPERSONA = e.CEDULARUCPERSONA
INNER JOIN PERSONA p ON e.CEDULARUCPERSONA = p.CEDULARUCPERSONA
LEFT JOIN EMPLEADO_QR eq ON c.CEDULARUCPERSONA = eq.CEDULARUCPERSONA
ORDER BY c.FECHAHORA DESC;

-- ============================================================
-- SECCION 5: PERMISOS (ajustar según usuario de la aplicación)
-- ============================================================

-- Ejemplo: GRANT ALL ON CALIFICACIONES TO usuario_app;
-- Ejemplo: GRANT ALL ON EMPLEADO_QR TO usuario_app;
-- Ejemplo: GRANT ALL ON USUARIOS_ADMIN TO usuario_app;
-- Ejemplo: GRANT SELECT ON V_CALIFICACIONES_EMPLEADO TO usuario_app;
-- Ejemplo: GRANT SELECT ON EMPLEADO TO usuario_app;
-- Ejemplo: GRANT SELECT ON PERSONA TO usuario_app;

-- ============================================================
-- FIN DEL SCRIPT
-- ============================================================
