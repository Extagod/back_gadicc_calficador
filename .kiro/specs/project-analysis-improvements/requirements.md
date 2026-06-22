# Requirements Document

## Introduction

Este documento define los requisitos para mejorar, modularizar y extender el proyecto **ApiEncuestaPrototipe** — una Web API .NET 8 que gestiona calificaciones de encuestas para funcionarios municipales. El sistema genera códigos QR únicos por funcionario (encargado), los cuales enlazan a un formulario de encuesta en una aplicación frontend. Los usuarios escanean el QR, visualizan la información del funcionario y envían su calificación.

Se añadirá un **Panel Administrativo de Escritorio (Windows Forms)** que permitirá a los encargados autenticarse y gestionar los códigos QR de cada funcionario del Municipio.

La base de datos objetivo será **SQL Server** (en lugar de PostgreSQL). La arquitectura debe implementar un **patrón repositorio con abstracciones** para que cambiar de proveedor de base de datos en el futuro no requiera modificar las capas de servicios ni de presentación.

### Flujo Actual del Proyecto

1. **Admin crea un encargado** → `POST /api/encargados` → genera un token QR único y una imagen QR (PNG en Base64)
2. **Usuario escanea el QR** → el frontend navega a `/encuesta/{token}` → llama a `GET /api/encargados/token/{token}` → recibe la información del encargado
3. **Usuario envía calificación** → `POST /api/calificaciones` → almacena la calificación con marca de tiempo
4. **Admin consulta QR** → `GET /api/encargados/{id}/qr` → retorna el QR como imagen PNG

### Flujo Propuesto del Proyecto

1. **Encargado inicia sesión** en el Panel Administrativo (Windows Forms) con sus credenciales
2. **Encargado gestiona funcionarios** → crea, edita, elimina funcionarios del Municipio
3. **Encargado genera QR** → selecciona un funcionario y genera/regenera su código QR
4. **Encargado imprime/exporta QR** → descarga o imprime el QR generado
5. **Ciudadano escanea QR** → accede al formulario de encuesta del funcionario
6. **Ciudadano califica** → envía su calificación a través del frontend web
7. **Encargado consulta calificaciones** → visualiza reportes y calificaciones por funcionario

### Áreas de Mejora Identificadas

- **Arquitectura**: Todo el código está en un solo proyecto sin separación de capas
- **Seguridad**: Credenciales de base de datos hardcodeadas, sin autenticación
- **Validación**: Valores de calificación sin restricción, sin validación de entrada robusta
- **Manejo de Errores**: Sin manejo global de excepciones
- **Testing**: No existen pruebas unitarias ni de integración
- **Administración**: No existe interfaz administrativa para gestionar funcionarios y QRs

## Glossary

- **API**: La aplicación Web API ApiEncuestaPrototipe construida con .NET 8
- **Panel_Admin**: Aplicación de escritorio Windows Forms para la gestión administrativa
- **Encargado**: Usuario administrativo que gestiona funcionarios y genera códigos QR
- **Funcionario**: Empleado del Municipio que recibe calificaciones mediante encuesta (entidad actualmente llamada "Encargado" en la BD)
- **Calificacion**: Una puntuación/valoración enviada por un ciudadano para un Funcionario específico
- **Token_QR**: Una cadena GUID única utilizada para identificar a un Funcionario en las URLs de encuesta
- **Codigo_QR**: Una imagen PNG codificada en Base64 que representa la URL de encuesta de un Funcionario
- **Frontend**: La aplicación web cliente que muestra los formularios de encuesta a los ciudadanos
- **Valor_Calificacion**: El valor categórico de una calificación (Excelente, Buena, Regular, Mala)
- **Capa_Servicios**: Conjunto de clases de servicio que encapsulan la lógica de negocio
- **Capa_Datos**: Capa de acceso a datos que interactúa con la base de datos SQL Server mediante Entity Framework Core
- **Repositorio**: Abstracción que encapsula el acceso a datos, permitiendo cambiar el proveedor de BD sin afectar capas superiores
- **Usuario_Admin**: Entidad que almacena las credenciales de los encargados para el Panel_Admin
- **Unit_of_Work**: Patrón que agrupa múltiples operaciones de repositorio en una transacción

## Requirements

### Requisito 1: Modularizar la Solución en Proyectos Separados

**Historia de Usuario:** Como desarrollador, quiero la solución dividida en proyectos separados por responsabilidad, para que cada capa sea independiente, testeable y mantenible, y el cambio de base de datos no afecte la lógica de negocio.

#### Criterios de Aceptación

1. THE solución SHALL contener un proyecto de clase de librería para la Capa_Datos (entidades, DbContext, repositorios, migraciones) con target framework net8.0
2. THE solución SHALL contener un proyecto de clase de librería para la Capa_Servicios (lógica de negocio, interfaces de servicios) con target framework net8.0
3. THE solución SHALL contener el proyecto Web API como capa de presentación HTTP con target framework net8.0
4. THE solución SHALL contener un proyecto Windows Forms para el Panel_Admin con target framework net8.0-windows
5. THE solución SHALL contener un proyecto de pruebas unitarias usando xUnit que referencia la Capa_Servicios y valida la lógica de negocio mediante mocks de las interfaces de repositorio
6. THE solución SHALL contener un proyecto de clase de librería Capa_Abstracciones que define las interfaces de repositorio (IEncargadoRepository, ICalificacionRepository, IUsuarioAdminRepository) y las entidades del dominio, referenciable por la Capa_Servicios, la Capa_Datos, la API y el Panel_Admin sin introducir dependencias de infraestructura
7. THE Capa_Datos SHALL implementar los repositorios definidos en Capa_Abstracciones usando Entity Framework Core con el proveedor de SQL Server, y SHALL ser el único proyecto que referencia paquetes de Entity Framework Core
8. THE Capa_Servicios SHALL depender únicamente del proyecto Capa_Abstracciones y no SHALL contener referencias a paquetes de Entity Framework Core, Npgsql, ni Microsoft.Data.SqlClient
9. THE grafo de dependencias de proyectos SHALL respetar la siguiente dirección permitida: la API y el Panel_Admin referencian Capa_Servicios y Capa_Abstracciones; la Capa_Servicios referencia únicamente Capa_Abstracciones; la Capa_Datos referencia únicamente Capa_Abstracciones; no SHALL existir referencias circulares entre proyectos
10. WHEN se reemplaza el proyecto Capa_Datos por una implementación alternativa que implementa las mismas interfaces de Capa_Abstracciones con un proveedor de base de datos diferente, THE Capa_Servicios, THE Panel_Admin y THE API SHALL compilar sin errores y sin modificaciones en su código fuente
11. THE archivo de solución (.slnx) SHALL incluir todos los proyectos (Capa_Abstracciones, Capa_Datos, Capa_Servicios, API, Panel_Admin, Pruebas) y SHALL compilar exitosamente con el comando `dotnet build` desde la raíz de la solución

### Requisito 2: Implementar Capa de Servicios

**Historia de Usuario:** Como desarrollador, quiero la lógica de negocio extraída en servicios independientes que usen abstracciones de datos, para que los controladores y formularios permanezcan delgados y la lógica sea independiente del proveedor de base de datos.

#### Criterios de Aceptación

1. THE Capa_Servicios SHALL implementar un IEncargadoService con métodos para crear un encargado (recibiendo datos del encargado y retornando el encargado creado con su ID asignado), consultar un encargado por su identificador único, consultar un encargado por su TokenQR, y listar todos los encargados registrados
2. THE Capa_Servicios SHALL implementar un ICalificacionService con métodos para crear una calificación asociada a un encargado existente (recibiendo IdEncargado, Valor y Comentarios opcionales) y consultar las calificaciones de un encargado específico por su IdEncargado
3. THE Capa_Servicios SHALL implementar un IQRService con métodos para generar un código QR en formato Base64 PNG a partir de una URL y regenerar el código QR de un encargado existente dado su identificador
4. THE Capa_Servicios SHALL implementar un IAuthService con métodos para autenticar usuarios administrativos recibiendo credenciales y retornando un resultado que indique éxito o fallo de autenticación
5. THE Capa_Servicios SHALL definir interfaces para todos los servicios (IEncargadoService, ICalificacionService, IQRService, IAuthService) permitiendo su registro mediante inyección de dependencias en el contenedor de servicios de .NET
6. WHEN un servicio necesita acceder a datos, THE servicio SHALL utilizar las interfaces de repositorio (IEncargadoRepository, ICalificacionRepository) mediante inyección de constructor, sin instanciar implementaciones concretas
7. THE Capa_Servicios SHALL NO tener dependencia directa con ningún paquete de Entity Framework Core ni proveedor de base de datos específico, verificable mediante la ausencia de referencias a Microsoft.EntityFrameworkCore en el proyecto de servicios
8. IF una operación del servicio recibe un identificador de entidad que no existe en el repositorio, THEN THE servicio SHALL retornar un resultado que indique explícitamente que el recurso no fue encontrado, sin lanzar excepciones no controladas
9. WHEN un controlador recibe una petición HTTP, THE controlador SHALL delegar toda la lógica de negocio al servicio correspondiente, limitándose a mapear la petición, invocar el servicio y transformar el resultado en una respuesta HTTP
10. IF el servicio IEncargadoService recibe datos para crear un encargado, THEN THE servicio SHALL generar un TokenQR único y coordinar la generación del código QR invocando a IQRService antes de persistir la entidad mediante el repositorio

### Requisito 3: Panel Administrativo - Autenticación

**Historia de Usuario:** Como encargado municipal, quiero iniciar sesión en el panel administrativo con mis credenciales, para que solo personal autorizado pueda gestionar los códigos QR de los funcionarios.

#### Criterios de Aceptación

1. THE Panel_Admin SHALL mostrar un formulario de login al iniciar la aplicación con un campo de nombre de usuario (máximo 50 caracteres) y un campo de contraseña (máximo 128 caracteres, con caracteres enmascarados)
2. WHEN el encargado ingresa credenciales válidas, THE Panel_Admin SHALL mostrar la ventana principal de gestión en un máximo de 3 segundos
3. IF el encargado ingresa credenciales inválidas, THEN THE Panel_Admin SHALL mostrar un mensaje de error indicando que las credenciales son incorrectas sin revelar cuál campo es erróneo, y permanecer en la pantalla de login
4. THE IAuthService SHALL validar las credenciales comparando contra la tabla Usuario_Admin en la base de datos
5. THE IAuthService SHALL almacenar las contraseñas usando hash BCrypt con salt y un factor de costo mínimo de 12
6. WHILE el encargado no ha iniciado sesión, THE Panel_Admin SHALL bloquear el acceso a todas las funcionalidades de gestión, mostrando únicamente el formulario de login
7. IF el encargado envía el formulario de login con el campo de nombre de usuario o contraseña vacío, THEN THE Panel_Admin SHALL mostrar un mensaje de validación junto al campo vacío sin realizar la solicitud de autenticación
8. IF el encargado ingresa credenciales inválidas 5 veces consecutivas, THEN THE Panel_Admin SHALL deshabilitar el formulario de login durante 60 segundos mostrando un mensaje indicando el tiempo de espera restante
9. IF la base de datos no está disponible durante el intento de autenticación, THEN THE Panel_Admin SHALL mostrar un mensaje de error indicando que no se puede conectar al servidor y permitir reintentar

### Requisito 4: Panel Administrativo - Gestión de Funcionarios

**Historia de Usuario:** Como encargado municipal, quiero crear, editar y consultar funcionarios desde el panel de escritorio, para gestionar el directorio de personal que será calificado.

#### Criterios de Aceptación

1. WHEN el encargado accede a la sección de funcionarios, THE Panel_Admin SHALL mostrar un listado de todos los funcionarios registrados en un DataGridView, mostrando las columnas nombre, apellido, cargo y dirección
2. WHEN el encargado hace clic en "Nuevo Funcionario", THE Panel_Admin SHALL mostrar un formulario con los campos nombre (obligatorio, máximo 100 caracteres), apellido (obligatorio, máximo 100 caracteres), cargo (opcional, máximo 100 caracteres) y dirección (opcional, máximo 200 caracteres)
3. WHEN el encargado completa el formulario de creación con datos válidos y confirma, THE Panel_Admin SHALL guardar el funcionario en la base de datos y actualizar el listado mostrando el nuevo registro
4. IF el encargado envía el formulario con los campos nombre o apellido vacíos, THEN THE Panel_Admin SHALL mostrar un mensaje de validación junto a cada campo vacío sin enviar los datos al servidor
5. WHEN el encargado selecciona un funcionario del listado, THE Panel_Admin SHALL cargar sus datos actuales en un formulario de edición con los mismos campos y restricciones que el formulario de creación
6. WHEN el encargado modifica los datos de un funcionario en el formulario de edición y confirma, THE Panel_Admin SHALL guardar los cambios en la base de datos y actualizar la fila correspondiente en el listado
7. WHEN el encargado ingresa al menos 1 carácter en el campo de búsqueda, THE Panel_Admin SHALL filtrar el listado mostrando solo los funcionarios cuyo nombre o apellido contenga el texto ingresado, sin distinguir mayúsculas de minúsculas
8. IF la operación de guardado o edición falla por error de conexión a la base de datos, THEN THE Panel_Admin SHALL mostrar un mensaje indicando que la operación no pudo completarse y preservar los datos ingresados en el formulario

### Requisito 5: Panel Administrativo - Generación de Códigos QR

**Historia de Usuario:** Como encargado municipal, quiero generar y visualizar códigos QR para cada funcionario desde el panel, para poder imprimirlos y colocarlos en los puntos de atención.

#### Criterios de Aceptación

1. WHEN el encargado selecciona un funcionario de la lista y hace clic en "Generar QR", THE Panel_Admin SHALL crear un Token_QR como GUID único, generar la imagen Codigo_QR de al menos 300x300 píxeles codificando la URL /encuesta/{token}, y mostrarla en el PictureBox del formulario
2. IF el encargado hace clic en "Generar QR" sin haber seleccionado un funcionario, THEN THE Panel_Admin SHALL mostrar un mensaje de validación indicando que debe seleccionar un funcionario y no realizar ninguna generación
3. WHEN el encargado hace clic en "Guardar QR", THE Panel_Admin SHALL abrir un diálogo de guardado con nombre predeterminado "{Apellido}_{Nombre}_QR.png" y permitir guardar la imagen QR como archivo PNG en el sistema de archivos local
4. WHEN el encargado hace clic en "Imprimir QR", THE Panel_Admin SHALL enviar la imagen QR a la impresora predeterminada del sistema
5. IF la impresión falla por impresora no disponible o error de conexión, THEN THE Panel_Admin SHALL mostrar un mensaje de error indicando que la impresión no pudo completarse y mantener la imagen QR visible en el formulario
6. IF un funcionario ya tiene un Codigo_QR generado, THEN THE Panel_Admin SHALL mostrar el QR existente en el PictureBox al seleccionar al funcionario y habilitar el botón "Regenerar QR" en lugar de "Generar QR"
7. WHEN se regenera un QR, THE Panel_Admin SHALL invalidar el Token_QR anterior, generar un nuevo Token_QR como GUID, actualizar la imagen Codigo_QR en base de datos, y mostrar el nuevo QR en el PictureBox

### Requisito 6: Panel Administrativo - Consulta de Calificaciones

**Historia de Usuario:** Como encargado municipal, quiero consultar las calificaciones recibidas por cada funcionario, para evaluar la calidad del servicio prestado.

#### Criterios de Aceptación

1. WHEN el encargado selecciona un funcionario, THE Panel_Admin SHALL mostrar un listado de todas las calificaciones recibidas con las columnas: FechaHora, Valor (Excelente, Buena, Regular o Mala) y Comentarios, ordenadas por FechaHora de más reciente a más antigua por defecto
2. WHEN el encargado selecciona un funcionario que no tiene calificaciones registradas, THE Panel_Admin SHALL mostrar un mensaje indicando que no existen calificaciones para el funcionario seleccionado y mostrar los contadores estadísticos en cero
3. THE Panel_Admin SHALL mostrar un resumen estadístico con el total de calificaciones y el conteo por cada Valor_Calificacion (Excelente, Buena, Regular, Mala), actualizado según el filtro de fechas activo
4. THE Panel_Admin SHALL permitir filtrar calificaciones por rango de fechas mediante una fecha de inicio y una fecha de fin
5. IF el encargado ingresa una fecha de inicio posterior a la fecha de fin, THEN THE Panel_Admin SHALL indicar un error de rango inválido y no aplicar el filtro
6. IF el filtro de fechas no retorna calificaciones en el rango seleccionado, THEN THE Panel_Admin SHALL mostrar el listado vacío y los contadores estadísticos en cero

### Requisito 7: Validar Valores de Calificación

**Historia de Usuario:** Como administrador del sistema, quiero que los valores de calificación estén restringidos a opciones válidas, para que solo se almacenen calificaciones significativas en la base de datos.

#### Criterios de Aceptación

1. WHEN se envía una calificación a través de la API, THE ICalificacionService SHALL verificar que el Valor_Calificacion sea uno de: "Excelente", "Buena", "Regular", "Mala", realizando una comparación sin distinguir mayúsculas de minúsculas
2. IF se envía una calificación con un Valor_Calificacion inválido, THEN THE API SHALL retornar una respuesta 400 Bad Request con un mensaje que incluya el valor inválido recibido y la lista de valores válidos aceptados
3. IF se envía una calificación con Valor_Calificacion nulo o vacío, THEN THE API SHALL retornar una respuesta 400 Bad Request indicando que el campo es obligatorio
4. THE modelo Calificacion SHALL definir el Valor_Calificacion como un enum con los valores: Excelente = 1, Buena = 2, Regular = 3, Mala = 4
5. THE API SHALL mapear el valor de texto recibido en el campo "Calificacion" del DTO al enum Valor_Calificacion antes de persistir la entidad

### Requisito 8: Implementar Manejo Global de Excepciones en la API

**Historia de Usuario:** Como desarrollador, quiero que las excepciones no manejadas sean capturadas y formateadas de manera consistente, para que la API retorne respuestas de error predecibles.

#### Criterios de Aceptación

1. THE API SHALL implementar un middleware de manejo global de excepciones registrado en el pipeline antes de los middlewares de routing y endpoints
2. WHEN ocurre una excepción no manejada, THE API SHALL retornar una respuesta HTTP 500 con un cuerpo JSON que contenga los campos: "message" (descripción genérica del error), "correlationId" (GUID único por solicitud), y "timestamp" (fecha/hora UTC en formato ISO 8601)
3. WHEN ocurre una excepción no manejada, THE API SHALL registrar los detalles completos de la excepción (tipo, mensaje, stack trace y correlationId) usando ILogger con nivel LogLevel.Error
4. WHILE la aplicación se ejecuta en modo producción (ASPNETCORE_ENVIRONMENT = Production), THE API SHALL excluir el campo "detail" (stack trace) de la respuesta JSON al cliente
5. WHILE la aplicación se ejecuta en modo desarrollo (ASPNETCORE_ENVIRONMENT = Development), THE API SHALL incluir un campo "detail" con el stack trace completo en la respuesta JSON
6. WHEN ocurre una excepción de tipo ArgumentException o ValidationException, THE API SHALL retornar HTTP 400 en lugar de 500, con el mensaje de la excepción en el campo "message"

### Requisito 9: Gestión Segura de Configuración

**Historia de Usuario:** Como desarrollador, quiero que los valores de configuración sensibles se almacenen de forma segura, para que las credenciales no queden expuestas en el control de versiones.

#### Criterios de Aceptación

1. THE API SHALL leer la cadena de conexión de SQL Server desde la clave "ConnectionStrings:DefaultConnection" utilizando el siguiente orden de precedencia: variables de entorno (prioridad alta), User Secrets en entorno Development (prioridad media), appsettings.json (prioridad baja, solo estructura sin credenciales reales)
2. THE solución SHALL garantizar que el archivo appsettings.json versionado no contenga valores de usuario, contraseña ni tokens en ninguna cadena de conexión; la sección "ConnectionStrings" podrá contener únicamente una cadena con placeholders vacíos o una referencia descriptiva sin datos reales
3. WHEN la aplicación inicia y la cadena de conexión "ConnectionStrings:DefaultConnection" está ausente, vacía o no contiene los segmentos Server y Database, THEN THE API SHALL terminar el proceso en un máximo de 5 segundos con un mensaje de error en la salida estándar que indique el nombre de la clave de configuración faltante o incompleta
4. THE Panel_Admin SHALL leer su cadena de conexión desde un archivo de configuración local (app.config) sin que el código fuente compilable contenga literales de cadena con valores de servidor, usuario o contraseña de base de datos
5. THE Capa_Datos SHALL usar el paquete Microsoft.EntityFrameworkCore.SqlServer como proveedor de base de datos, reemplazando la referencia actual a Npgsql.EntityFrameworkCore.PostgreSQL
6. IF la cadena de conexión proporcionada tiene formato válido pero la conexión a SQL Server no puede establecerse al iniciar la aplicación, THEN THE API SHALL registrar el error de conexión en los logs de nivel Error y terminar el proceso en un máximo de 10 segundos con un mensaje indicando fallo de conectividad a la base de datos

### Requisito 10: Validación de Entrada con Data Annotations

**Historia de Usuario:** Como desarrollador, quiero que todos los DTOs y modelos sean validados con data annotations, para que los datos inválidos sean rechazados antes de llegar a la lógica de negocio.

#### Criterios de Aceptación

1. THE CrearCalificacionDto SHALL requerir el campo IdEncargado con atributo [Required] y un valor mínimo de 1 usando [Range(1, int.MaxValue)]
2. THE CrearCalificacionDto SHALL requerir el campo Calificacion con atributo [Required] y una longitud máxima de 20 caracteres usando [MaxLength(20)]
3. THE CrearCalificacionDto SHALL definir el campo Comentarios como opcional con una longitud máxima de 500 caracteres usando [MaxLength(500)]
4. THE modelo Encargado SHALL requerir el campo Nombre con atributo [Required] y una longitud mínima de 1 y máxima de 100 caracteres usando [StringLength(100, MinimumLength = 1)]
5. THE modelo Encargado SHALL requerir el campo Apellido con atributo [Required] y una longitud mínima de 1 y máxima de 100 caracteres usando [StringLength(100, MinimumLength = 1)]
6. THE modelo Encargado SHALL definir el campo Cargo como opcional con una longitud máxima de 100 caracteres usando [MaxLength(100)]
7. THE modelo Encargado SHALL definir el campo Direccion como opcional con una longitud máxima de 200 caracteres usando [MaxLength(200)]
8. IF se recibe una solicitud con datos inválidos, THEN THE API SHALL retornar un 400 Bad Request con un objeto JSON que contenga un diccionario "errors" donde cada clave es el nombre del campo y el valor es un arreglo de mensajes de error de validación

### Requisito 11: Configuración CORS Basada en Entorno

**Historia de Usuario:** Como desarrollador, quiero que los orígenes CORS estén configurados por entorno, para que la API soporte diferentes escenarios de despliegue sin cambios de código.

#### Criterios de Aceptación

1. THE API SHALL leer los orígenes CORS permitidos desde una sección de configuración en appsettings que contenga un arreglo de URLs de origen, y aplicarlos como orígenes permitidos en la política CORS al iniciar la aplicación
2. WHILE se ejecuta en modo desarrollo, THE API SHALL permitir solicitudes desde los orígenes definidos en appsettings.Development.json, permitiendo cualquier método HTTP y cualquier encabezado
3. WHILE se ejecuta en modo producción, THE API SHALL permitir solicitudes únicamente desde los orígenes definidos en appsettings.Production.json, permitiendo cualquier método HTTP y cualquier encabezado
4. IF no se configuran orígenes CORS en la sección de configuración correspondiente al entorno, THEN THE API SHALL omitir el encabezado Access-Control-Allow-Origin en las respuestas, resultando en el rechazo de todas las solicitudes de origen cruzado por parte del navegador
5. IF una solicitud proviene de un origen no incluido en la lista de orígenes configurados, THEN THE API SHALL omitir el encabezado Access-Control-Allow-Origin en la respuesta a dicha solicitud
6. WHEN la API recibe una solicitud preflight (OPTIONS) desde un origen configurado, THE API SHALL responder con los encabezados Access-Control-Allow-Origin, Access-Control-Allow-Methods y Access-Control-Allow-Headers correspondientes y un código de estado 204 o 200

### Requisito 12: Pruebas Unitarias para Servicios

**Historia de Usuario:** Como desarrollador, quiero pruebas unitarias para cada servicio, para que las regresiones se detecten tempranamente y se puedan desarrollar nuevas funcionalidades con confianza.

#### Criterios de Aceptación

1. THE proyecto de pruebas SHALL contener al menos 3 pruebas para ICalificacionService: una verificando creación exitosa con Valor_Calificacion válido (Excelente, Buena, Regular o Mala), una verificando rechazo con valor inválido, y una verificando rechazo con valor nulo o vacío
2. THE proyecto de pruebas SHALL contener al menos 3 pruebas para IEncargadoService: una verificando creación exitosa de un encargado con TokenQR generado como GUID de 32 caracteres hexadecimales, una verificando que dos encargados creados consecutivamente reciben TokenQR distintos, y una verificando que la consulta por TokenQR inexistente retorna resultado de no encontrado
3. THE proyecto de pruebas SHALL contener al menos 2 pruebas para IQRService: una verificando que GenerarQRBase64 retorna una cadena Base64 válida que al decodificar produce bytes PNG (iniciando con la firma PNG 0x89504E47), y una verificando que la imagen generada tiene al menos 300x300 píxeles
4. THE proyecto de pruebas SHALL contener al menos 3 pruebas para IAuthService: una verificando autenticación exitosa con credenciales válidas, una verificando fallo con contraseña incorrecta, y una verificando fallo con usuario inexistente
5. THE proyecto de pruebas SHALL usar Moq para crear mocks de las interfaces de repositorio (IEncargadoRepository, ICalificacionRepository, IUsuarioAdminRepository) sin acceder a base de datos real
6. WHEN se ejecuta el conjunto de pruebas con `dotnet test`, THE proyecto de pruebas SHALL alcanzar un mínimo de 80% de cobertura de líneas de código en las clases de la Capa_Servicios, medido con coverlet

### Requisito 13: Pruebas Unitarias para Panel Administrativo

**Historia de Usuario:** Como desarrollador, quiero pruebas para la lógica del panel administrativo, para asegurar que la interacción con los servicios funciona correctamente.

#### Criterios de Aceptación

1. THE proyecto de pruebas SHALL contener al menos 2 pruebas para la lógica de login: una verificando que credenciales válidas (usuario y contraseña no vacíos y coincidentes con los almacenados) retornan autenticación exitosa, y otra verificando que credenciales inválidas (usuario inexistente o contraseña incorrecta) retornan fallo de autenticación
2. THE proyecto de pruebas SHALL contener al menos 2 pruebas para la lógica de creación de funcionarios: una verificando que un funcionario con Nombre y Apellido no vacíos se crea exitosamente, y otra verificando que si Nombre o Apellido están vacíos o nulos el servicio retorna error de validación
3. THE proyecto de pruebas SHALL contener al menos 2 pruebas para la lógica de generación de QR: una verificando que al crear un funcionario se genera un TokenQR con formato GUID válido de 32 caracteres hexadecimales y un CodigoQR en Base64 no vacío, y otra verificando que dos funcionarios creados consecutivamente reciben valores de TokenQR distintos entre sí
4. THE proyecto de pruebas SHALL contener al menos 2 pruebas para la lógica de consulta de calificaciones: una verificando que al filtrar por un rango de fechas (FechaInicio y FechaFin) solo se retornan calificaciones cuya FechaHora esté dentro del rango inclusive, y otra verificando que si no existen calificaciones en el rango especificado se retorna una colección vacía
5. IF el servicio externo (IAuthService, IEncargadoService, ICalificacionService o IQRService) no está disponible o lanza excepción, THEN THE proyecto de pruebas SHALL contener al menos 1 prueba que verifique que la capa de servicio propaga o maneja la excepción sin lanzar errores no controlados

### Requisito 14: Documentar Arquitectura y Flujo del Proyecto

**Historia de Usuario:** Como nuevo miembro del equipo, quiero documentación clara de la arquitectura del proyecto y el flujo de datos, para poder entender el sistema y contribuir rápidamente.

#### Criterios de Aceptación

1. THE proyecto SHALL incluir un README.md en la raíz de la solución documentando el propósito del proyecto, el stack tecnológico (.NET 8, SQL Server, Entity Framework Core, QRCoder, Windows Forms, xUnit) e instrucciones de configuración paso a paso incluyendo prerrequisitos de software
2. THE README.md SHALL incluir un diagrama de flujo en formato Mermaid describiendo el proceso completo desde el login del encargado en el Panel_Admin, pasando por la gestión de funcionarios y generación de QR, hasta la calificación enviada por el ciudadano
3. THE README.md SHALL documentar todos los endpoints de la API en formato tabla con columnas: Método HTTP, Ruta, Descripción, Cuerpo de Solicitud (JSON de ejemplo), y Respuesta Exitosa (código HTTP y JSON de ejemplo)
4. THE README.md SHALL documentar el esquema de base de datos incluyendo nombres de tablas (Encargados, Calificaciones, UsuariosAdmin), columnas con tipos de datos SQL Server, restricciones (PK, FK, NOT NULL) y relaciones entre tablas
5. THE README.md SHALL documentar la estructura de la solución listando cada proyecto (Capa_Abstracciones, Capa_Datos, Capa_Servicios, API, Panel_Admin, Pruebas) con una descripción de su responsabilidad y sus dependencias
6. THE README.md SHALL incluir una sección "Ejecución Local" con instrucciones separadas para ejecutar la API (`dotnet run`) y el Panel_Admin, incluyendo la configuración de la cadena de conexión mediante User Secrets
