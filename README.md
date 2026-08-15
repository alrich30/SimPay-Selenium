# SimPay - Gestión de pagos y pruebas automatizadas

SimPay es una aplicación web para la gestión simulada de pagos. El repositorio contiene una API REST, una interfaz web, pruebas unitarias y pruebas funcionales automatizadas con Selenium WebDriver.

## Funcionalidades

La aplicación permite:

* Iniciar sesión mediante credenciales simuladas.
* Registrar pagos.
* Consultar los pagos registrados.
* Actualizar pagos existentes.
* Eliminar pagos.
* Filtrar pagos por estado, moneda, rango de montos y rango de fechas.
* Combinar varios filtros en una misma consulta.
* Visualizar estadísticas de los pagos filtrados.
* Exportar a CSV los pagos mostrados.
* Validar los datos introducidos.
* Mostrar mensajes de confirmación y error.

## Nuevo incremento

El incremento de consultas y reportes incorpora tres funcionalidades principales.

### Filtrado avanzado

Los pagos pueden filtrarse mediante:

* Estado: pendiente, completado o rechazado.
* Moneda.
* Monto mínimo.
* Monto máximo.
* Fecha inicial.
* Fecha final.

Los filtros pueden utilizarse individualmente o combinarse.

Ejemplos:

`GET /api/Payments?status=Pending`

`GET /api/Payments?currency=USD`

`GET /api/Payments?minAmount=100&maxAmount=2000`

`GET /api/Payments?fromDate=2026-08-01&toDate=2026-08-31`

`GET /api/Payments?status=Pending&currency=USD&minAmount=100`

### Panel de estadísticas

El panel muestra:

* Total de pagos visibles.
* Pagos pendientes.
* Pagos completados.
* Pagos rechazados.

Las estadísticas se actualizan automáticamente según los filtros aplicados.

### Exportación a CSV

La aplicación permite descargar los pagos visibles en formato CSV. Si existen filtros activos, solamente se exportan los registros que cumplen esos criterios.

El archivo incluye:

* Identificador del pago.
* Cuenta de origen.
* Cuenta de destino.
* Monto.
* Moneda.
* Descripción.
* Estado.
* Fecha de creación.

## Tecnologías utilizadas

* C#
* .NET 8
* ASP.NET Core Web API
* HTML
* CSS
* JavaScript
* Selenium WebDriver
* NUnit
* ExtentReports
* Google Chrome
* Visual Studio 2022

No se utilizó Selenium IDE para crear ni ejecutar las pruebas.

## Estructura del proyecto

* `SimPay.Api`: API, interfaz web y autenticación simulada.
* `SimPay.Application`: contratos, solicitudes y parámetros de consulta.
* `SimPay.Domain`: entidades y reglas del dominio.
* `SimPay.Infrastructure`: repositorio de pagos en memoria.
* `Simpay.Tests`: pruebas unitarias.
* `SimPay.SeleniumTests`: pruebas funcionales con Selenium.

## Credenciales de acceso

Usuario: `admin`

Contraseña: `SimPay123!`

Estas credenciales son exclusivamente para fines académicos y de demostración.

## Ejecución de la aplicación

1. Abrir `SimPay.sln` en Visual Studio 2022.
2. Establecer `SimPay` como proyecto de inicio.
3. Ejecutar mediante `Ctrl + F5`.
4. Acceder a `https://localhost:7058/index.html`.

La aplicación debe permanecer ejecutándose durante las pruebas Selenium.

## Ejecución de las pruebas

1. Iniciar la aplicación.
2. Abrir el Explorador de pruebas de Visual Studio.
3. Confirmar que SimPay esté disponible en `https://localhost:7058`.
4. Ejecutar todas las pruebas.

El proyecto contiene:

* 23 pruebas unitarias.
* 18 pruebas funcionales con Selenium.
* 41 pruebas automatizadas en total.

## Escenarios automatizados

### Inicio de sesión

* Inicio de sesión con credenciales válidas.
* Inicio de sesión con credenciales incorrectas.
* Envío del formulario con campos vacíos.

### Registro de pagos

* Registro con datos válidos.
* Intento de registro con la misma cuenta de origen y destino.
* Registro con el monto máximo permitido.

### Consulta de pagos

* Consulta de un pago registrado.
* Intento de acceso sin iniciar sesión.
* Consulta de varios pagos.

### Actualización de pagos

* Actualización con datos válidos.
* Intento de actualización con monto igual a cero.
* Actualización con una descripción de 200 caracteres.

### Eliminación de pagos

* Eliminación confirmada.
* Cancelación de la eliminación.
* Eliminación de un pago con una descripción de 200 caracteres.

### Consultas y reportes

* Filtrado de pagos por moneda.
* Actualización de las estadísticas mediante filtros.
* Exportación de pagos filtrados a CSV.

## Reporte y capturas

El reporte generado por ExtentReports se encuentra en:

`Simpay.Tests/SimPay.SeleniumTests/TestArtifacts/SeleniumReport.html`

Las capturas automáticas se almacenan en:

`Simpay.Tests/SimPay.SeleniumTests/TestArtifacts/Screenshots`

## Resultado final

* 23 pruebas unitarias superadas.
* 18 pruebas Selenium superadas.
* 41 pruebas superadas.
* 0 pruebas fallidas.
* 0 pruebas omitidas.

## Consideraciones

* Las pruebas Selenium requieren que SimPay esté ejecutándose.
* Las pruebas utilizan Google Chrome.
* ChromeDriver acepta el certificado HTTPS local.
* Las pruebas se ejecutan de forma no paralela.
* Los pagos se almacenan temporalmente en memoria.
* Los datos se reinician al detener la aplicación.
* Las descargas CSV de las pruebas se guardan en carpetas temporales y se eliminan al finalizar.

## Autor

Richard Alexander Torres Pérez
