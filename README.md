# SimPay — Pruebas automatizadas con Selenium

SimPay es una aplicación web para la gestión simulada de pagos. Este repositorio contiene la aplicación, las pruebas unitarias y las pruebas automatizadas de interfaz desarrolladas con Selenium WebDriver.

El proyecto fue realizado como parte de la asignación de pruebas automatizadas con Selenium.

## Funcionalidades

La aplicación permite realizar las siguientes operaciones:

- Iniciar sesión mediante credenciales simuladas.
- Registrar pagos.
- Consultar los pagos registrados.
- Actualizar pagos existentes.
- Eliminar pagos.
- Validar los datos introducidos en los formularios.
- Mostrar mensajes de confirmación y error.

## Tecnologías utilizadas

- C#
- .NET 8
- ASP.NET Core Web API
- HTML
- CSS
- JavaScript
- Selenium WebDriver
- NUnit
- ExtentReports
- Google Chrome
- Visual Studio 2022

No se utilizó Selenium IDE para crear ni ejecutar las pruebas.

## Estructura del proyecto

El repositorio está organizado en los siguientes proyectos:

- `SimPay.Api`: API, interfaz web y autenticación simulada.
- `SimPay.Application`: contratos y solicitudes de la aplicación.
- `SimPay.Domain`: entidades y reglas del dominio.
- `SimPay.Infrastructure`: implementación del repositorio en memoria.
- `Simpay.Tests`: pruebas unitarias de la aplicación.
- `SimPay.SeleniumTests`: pruebas automatizadas con Selenium.

## Credenciales de acceso

Para iniciar sesión en la aplicación se utilizan las siguientes credenciales simuladas:

- Usuario: `admin`
- Contraseña: `SimPay123!`

Estas credenciales son exclusivamente para fines académicos y de demostración.

## Ejecución de la aplicación

1. Abrir la solución `SimPay.sln` en Visual Studio 2022.
2. Establecer `SimPay` como proyecto de inicio.
3. Ejecutar la aplicación sin depuración mediante `Ctrl + F5`.
4. Acceder a `https://localhost:7058/index.html`.

La aplicación debe permanecer ejecutándose durante las pruebas de Selenium.

## Ejecución de las pruebas

1. Abrir el Explorador de pruebas de Visual Studio.
2. Verificar que la aplicación esté disponible en `https://localhost:7058`.
3. Seleccionar el proyecto `SimPay.SeleniumTests`.
4. Ejecutar todas las pruebas.

El proyecto contiene:

- 17 pruebas unitarias.
- 15 pruebas automatizadas con Selenium.
- 32 pruebas en total.

## Escenarios automatizados

Se automatizaron cinco historias de usuario:

1. Inicio de sesión.
2. Registro de pagos.
3. Consulta de pagos.
4. Actualización de pagos.
5. Eliminación de pagos.

Cada historia contiene tres escenarios de prueba. En conjunto, las pruebas incluyen caminos felices, pruebas negativas y pruebas de límites.

### Inicio de sesión

- Inicio de sesión con credenciales válidas.
- Inicio de sesión con credenciales incorrectas.
- Envío del formulario con los campos vacíos.

### Registro de pagos

- Registro de un pago con datos válidos.
- Intento de registro utilizando la misma cuenta de origen y destino.
- Registro de un pago con el monto máximo permitido.

### Consulta de pagos

- Consulta de un pago registrado.
- Intento de acceso sin iniciar sesión.
- Consulta de varios pagos registrados.

### Actualización de pagos

- Actualización de un pago con datos válidos.
- Intento de actualización utilizando un monto igual a cero.
- Actualización con una descripción de 200 caracteres.

### Eliminación de pagos

- Eliminación confirmada de un pago.
- Cancelación de la eliminación de un pago.
- Eliminación de un pago con una descripción de 200 caracteres.

## Reporte HTML y capturas

El reporte HTML generado por ExtentReports se encuentra en:

`Simpay.Tests/SimPay.SeleniumTests/TestArtifacts/SeleniumReport.html`

Las capturas automáticas de cada escenario se encuentran en:

`Simpay.Tests/SimPay.SeleniumTests/TestArtifacts/Screenshots`

Cada prueba genera automáticamente una captura de pantalla al finalizar. Las capturas también aparecen asociadas a sus respectivos escenarios dentro del reporte HTML.

## Resultado de la ejecución

La ejecución final produjo los siguientes resultados:

- 17 pruebas unitarias superadas.
- 15 pruebas Selenium superadas.
- 32 pruebas superadas.
- 0 pruebas fallidas.
- 0 pruebas omitidas.

## Consideraciones

- La aplicación debe estar ejecutándose antes de iniciar las pruebas Selenium.
- Las pruebas utilizan Google Chrome.
- El certificado HTTPS local es aceptado por la configuración de ChromeDriver.
- Las pruebas se ejecutan de forma no paralela para evitar interferencias entre escenarios.
- El repositorio utiliza datos almacenados temporalmente en memoria.
- Los datos se reinician cuando se detiene y vuelve a iniciar la aplicación.

## Autor

Richard Alexander Torres Pérez