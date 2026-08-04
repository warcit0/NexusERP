# PLAN MAESTRO — SISTEMA ERP EMPRESARIAL
## Arquitectura .NET Core · Azure · Multi-Tenant SaaS

> **Versión:** 1.0
> **Fecha:** Junio 2026
> **Clasificación:** Confidencial — Documento de Planeamiento Arquitectónico

---

## 💡 Nombres de Proyecto Sugeridos

| Nombre | Concepto |
|--------|----------|
| **NexusERP** | Punto de conexión de todos los módulos de negocio |
| **CrestERP** | La cúspide de la gestión empresarial |
| **VaultCore** | El núcleo seguro y confiable de la operación |
| **ArcERP** | Arquitectura de referencia comercial |
| **OrbitERP** | Todo gira alrededor de un núcleo central |

> **Recomendación:** `NexusERP` — transmite integración, modernidad y está libre de connotaciones geográficas para expandirse a otros mercados.

---

## 1. VISIÓN GENERAL DEL SISTEMA

NexusERP es un sistema de Planificación de Recursos Empresariales (ERP) de alta disponibilidad, diseñado bajo una arquitectura **SaaS multi-tenant** orientada a pequeñas y medianas empresas del sector comercial y de servicios. El sistema centraliza las operaciones de ventas, inventario, finanzas, cumplimiento fiscal y analítica de negocio en una sola plataforma nativa en la nube.

### Principios de Diseño
- **Cloud-Native First:** Diseñado para Azure desde su concepción, no adaptado a posteriori.
- **Offline-Capable POS:** La operación de caja nunca depende de la conectividad.
- **Multi-Tenant Aislado:** Cada empresa cliente tiene sus datos completamente separados.
- **API-First:** Todo el sistema se construye sobre contratos de API documentados antes del frontend.
- **Seguridad por Capas:** Autenticación, autorización, cifrado en tránsito y en reposo en todos los niveles.
- **Observabilidad Total:** Métricas, trazas y logs desde el primer día.

---

## 2. STACK TECNOLÓGICO COMPLETO

### 2.1 Backend — Core del Sistema

| Componente | Tecnología | Versión | Justificación |
|------------|------------|---------|---------------|
| **Lenguaje principal** | C# | 13 | Tipado fuerte, rendimiento, ecosistema maduro |
| **Runtime** | .NET | 9 LTS | Soporte a largo plazo, rendimiento superior |
| **Framework Web** | ASP.NET Core | 9 | API REST de alto rendimiento, minimal APIs y controllers |
| **ORM** | Entity Framework Core | 9 | Integración nativa con .NET, migraciones Code First |
| **Patrón de Arquitectura** | Clean Architecture + CQRS | — | Separación de responsabilidades, testeable y mantenible |
| **Mediador CQRS** | MediatR | 12.x | Desacopla comandos/queries del resto de la aplicación |
| **Validación** | FluentValidation | 11.x | Validaciones declarativas y reutilizables |
| **Mapeo de objetos** | AutoMapper | 13.x | Conversión limpia entre entidades y DTOs |
| **Documentación API** | Scalar / Swashbuckle | — | Documentación interactiva OpenAPI 3.1 |

### 2.2 Base de Datos — Elección Principal

> **Elección: Azure SQL Database (SQL Server en la nube)**

**Justificación:**
- Es la integración más nativa y estable con .NET y Entity Framework Core.
- Azure SQL Database ofrece **SLA de 99.99%** con georredundancia automática.
- Soporta **Always Encrypted** para datos sensibles.
- Escalabilidad automática de cómputo (serverless tier).
- Backups automáticos con retención de hasta 35 días.
- Compatible con MSSQL local para desarrollo sin discrepancias de entorno.

| Componente | Tecnología | Rol |
|------------|------------|-----|
| **Base de datos principal** | Azure SQL Database (General Purpose) | Datos transaccionales de todos los tenants |
| **Caché distribuida** | Azure Cache for Redis | Sesiones, tokens revocados, datos frecuentes |
| **Archivos / Blobs** | Azure Blob Storage | Facturas PDF, imágenes de productos, reportes |
| **Cola de mensajes** | Azure Service Bus | Eventos asincrónicos entre módulos |
| **BD Offline Local** | SQLite (vía EF Core) | Soporte offline en el cliente POS de escritorio |

### 2.3 Seguridad y Autenticación

| Componente | Tecnología | Detalle |
|------------|------------|---------|
| **Identity Provider** | ASP.NET Core Identity + Azure AD B2C | Gestión de usuarios, roles y claims |
| **Tokens** | JWT (Bearer tokens) | Stateless, 15 min access / 7 días refresh |
| **Cifrado de contraseñas** | BCrypt.Net-Next | Hashing adaptativo con salt |
| **Secretos** | Azure Key Vault | Cadenas de conexión, claves JWT, API keys externas |
| **Comunicación** | TLS 1.3 | Todo el tráfico cifrado en tránsito |
| **Autorización** | Policy-based Authorization | Roles: SuperAdmin / Admin / Gerente / Cajero |

### 2.4 Frontend

#### Web Dashboard / Panel Analítico (WebERP)
| Componente | Tecnología | Justificación |
|------------|------------|---------------|
| **Framework** | Blazor WebAssembly (.NET 9) | 100% C#, reutiliza modelos del backend |
| **UI Component Library** | MudBlazor | Componentes Material Design listos para producción |
| **Gráficos / BI** | ApexCharts for Blazor | Dashboards interactivos, gráficos en tiempo real |
| **Estado global** | Fluxor (Redux para Blazor) | Manejo predecible del estado de la aplicación |
| **Comunicación tiempo real** | SignalR | Notificaciones push, actualizaciones de dashboard en vivo |

#### Aplicación POS Desktop (Offline-Capable)
| Componente | Tecnología | Justificación |
|------------|------------|---------------|
| **Framework UI** | .NET MAUI | Cross-platform (Windows/macOS), nativo del ecosistema .NET |
| **UI Styling** | MAUI Community Toolkit | Componentes y helpers adicionales para MAUI |
| **Base de datos local** | SQLite + EF Core | Sincronización bidireccional cuando hay conectividad |
| **Sincronización** | Background Services + Azure Service Bus | Cola de transacciones pendientes |

### 2.5 Infraestructura Azure

| Recurso Azure | Tier Recomendado | Propósito |
|---------------|-----------------|-----------|
| **Azure App Service** | P2v3 (Premium) | Hosting de la Web API y Blazor WASM |
| **Azure SQL Database** | General Purpose, 4 vCores | Base de datos principal con HA automática |
| **Azure Cache for Redis** | C1 Standard | Caché distribuida para tokens y sesiones |
| **Azure Service Bus** | Standard Tier | Mensajería asíncrona entre módulos |
| **Azure Blob Storage** | LRS (Hot tier) | PDFs, imágenes, archivos exportados |
| **Azure Key Vault** | Standard | Secretos, certificados, claves de cifrado |
| **Azure Application Insights** | — | Telemetría, trazas, alertas de rendimiento |
| **Azure Container Registry** | Basic/Standard | Registro de imágenes Docker |
| **Azure DevOps** | Basic + Pipelines | CI/CD, repositorios, gestión de trabajo |
| **Azure CDN** | Standard Microsoft | Assets estáticos del frontend Blazor WASM |

### 2.6 Testing

| Tipo | Framework | Uso |
|------|-----------|-----|
| **Unit Tests** | xUnit + Moq | Tests de servicios, handlers CQRS, validadores |
| **Integration Tests** | xUnit + WebApplicationFactory | Tests de endpoints HTTP reales |
| **BD en tests** | EF Core InMemory / Testcontainers | BD efímera por test |
| **Coverage** | Coverlet + ReportGenerator | Reporte de cobertura integrado en el pipeline |
| **Arquitectura** | NetArchTest | Validar reglas de capas automáticamente |

### 2.7 Herramientas de Desarrollo

| Herramienta | Propósito |
|-------------|-----------|
| **IDE** | Visual Studio 2022 / Rider |
| **Control de versiones** | Git + Azure Repos |
| **Análisis estático** | SonarCloud (integrado en CI/CD) |
| **Containerización** | Docker + Docker Compose (entorno local) |
| **Gestión de secretos local** | dotnet user-secrets |
| **Migraciones BD** | EF Core Migrations (versionadas en código) |
| **API Testing** | Scalar UI / Postman / Bruno |
| **Reportes Excel** | ClosedXML |
| **Reportes PDF** | QuestPDF |
| **Jobs/Scheduler** | Hangfire |

---

## 3. ARQUITECTURA DEL SISTEMA

### 3.1 Estructura de Solución (.NET Solution)

```
NexusERP.sln
│
├── src/
│   ├── NexusERP.API/                  # Capa de Presentación — ASP.NET Core Web API
│   │   ├── Controllers/
│   │   ├── Middlewares/
│   │   ├── Filters/
│   │   └── Program.cs
│   │
│   ├── NexusERP.Application/          # Capa de Aplicación — CQRS, Handlers, DTOs
│   │   ├── Features/
│   │   │   ├── Ventas/
│   │   │   ├── Inventario/
│   │   │   ├── Facturacion/
│   │   │   ├── CuentasCxC/
│   │   │   ├── CuentasCxP/
│   │   │   ├── Caja/
│   │   │   └── Reportes/
│   │   ├── Common/
│   │   ├── Interfaces/
│   │   └── Validators/
│   │
│   ├── NexusERP.Domain/               # Capa de Dominio — Entidades, Reglas, Value Objects
│   │   ├── Entities/
│   │   ├── ValueObjects/
│   │   ├── Events/
│   │   └── Exceptions/
│   │
│   ├── NexusERP.Infrastructure/       # Infraestructura — BD, Repos, Servicios externos
│   │   ├── Persistence/
│   │   │   ├── Configurations/
│   │   │   ├── Migrations/
│   │   │   └── NexusDbContext.cs
│   │   ├── Repositories/
│   │   ├── Services/
│   │   │   ├── EmailService.cs
│   │   │   ├── BlobStorageService.cs
│   │   │   └── AzureServiceBusService.cs
│   │   └── Identity/
│   │
│   ├── NexusERP.WebDashboard/         # Frontend Blazor WebAssembly
│   │   ├── Pages/
│   │   ├── Components/
│   │   ├── Store/                     # Fluxor state management
│   │   └── Services/                  # HTTP clients tipados
│   │
│   └── NexusERP.POSClient/            # Aplicación .NET MAUI Desktop
│       ├── Views/
│       ├── ViewModels/
│       ├── Services/
│       └── Data/                      # SQLite local
│
├── tests/
│   ├── NexusERP.UnitTests/
│   ├── NexusERP.IntegrationTests/
│   └── NexusERP.ArchitectureTests/    # NetArchTest — validar reglas de capas
│
└── infrastructure/
    ├── docker/
    │   ├── Dockerfile.api
    │   └── docker-compose.yml
    ├── azure/
    │   ├── main.bicep                 # Infrastructure as Code
    │   └── parameters.json
    └── scripts/
        └── seed-database.sql
```

### 3.2 Patrón Multi-Tenant

```
Estrategia: Schema-per-Tenant dentro de una base de datos compartida.

- Cada tenant tiene su propio schema en Azure SQL:
    [tenant_abc].[Ventas], [tenant_abc].[Productos]...
- El TenantId se resuelve automáticamente desde el JWT claim en cada request.
- Un middleware ICurrentTenantService inyecta el contexto del tenant en el DbContext.
- El Super Admin opera en el schema [platform] para gestionar licencias.
```

### 3.3 Diagrama de Flujo de una Solicitud

```
[Cliente (Blazor / MAUI)]
        │  HTTPS + JWT
        ▼
[Azure App Service — ASP.NET Core API]
        │
   [AuthMiddleware]    → Valida JWT, extrae TenantId y UserId
        │
   [TenantMiddleware]  → Configura DbContext con el schema correcto
        │
   [Controller]        → Recibe request, valida modelo
        │
   [MediatR Handler]   → Ejecuta Command o Query
        │
    ┌───┴────────┐
    │            │
[Domain]   [Infrastructure]
Reglas     Persistencia / Caché / Servicios Externos
    │            │
    └────────────┘
        │
   [Response DTO]      → Devuelto al cliente
```

---

## 4. MÓDULOS DEL SISTEMA

### Módulo 1: Motor de Ventas y Punto de Venta
- Procesamiento de ventas con búsqueda de productos por código, nombre o escáner.
- Aplicación de descuentos, promociones y precios por lista de cliente.
- Soporte para múltiples métodos de pago (efectivo, tarjeta, transferencia, crédito).
- Modo offline con sincronización automática al recuperar conexión.
- Historial de transacciones con anulación y notas de crédito.

### Módulo 2: Facturación Electrónica y Cumplimiento Fiscal
- Generación de XML firmado según normativa fiscal local.
- Integración con la API de catálogos (CABYS, actividades económicas).
- Generación automática de PDF de factura con diseño personalizable.
- Envío a Hacienda y gestión de confirmaciones/rechazos.
- Repositorio inmutable de facturas en Azure Blob Storage.

### Módulo 3: Gestión de Inventario y Almacén
- CRUD de productos con variantes, códigos de barras y unidades de medida.
- Control de lotes y fechas de vencimiento.
- Traslados entre sucursales con trazabilidad completa.
- Ajustes de inventario por merma, conteo físico y devoluciones.
- Alertas automáticas de punto de reorden vía notificación.

### Módulo 4: Cuentas por Cobrar (CxC)
- Gestión de clientes con límites de crédito e historial.
- Registro y seguimiento de facturas a crédito.
- Calendario de pagos y estados de cuenta automatizados.
- Envío automático de cobros vía email.

### Módulo 5: Cuentas por Pagar (CxP)
- Gestión de proveedores y órdenes de compra.
- Registro de facturas de proveedor con validación de montos.
- Programación de pagos y control de vencimientos.
- Conciliación de compras con inventario.

### Módulo 6: Auditoría y Control de Caja
- Apertura y cierre de caja con arqueo ciego.
- Registro de ingresos y egresos de efectivo con categorización.
- Generación de reportes X (parcial) y Z (cierre definitivo) en PDF.
- Registro inmutable de cada movimiento para auditoría fiscal.

### Módulo 7: Dashboard Analítico y Reportes (WebERP)
- KPIs en tiempo real: ventas del día, semana y mes.
- Gráficos de tendencias: productos más vendidos, horas pico, ticket promedio.
- Comparativas entre sucursales y períodos.
- Exportación de reportes a Excel y PDF.
- Acceso remoto desde cualquier dispositivo con navegador.

### Módulo 8: Gestión de Licencias y Multi-Tenancy
- Panel de Super Admin para aprovisionar nuevos tenants.
- Gestión de planes de suscripción (Básico / Pro / Enterprise).
- Control de fechas de vencimiento y suspensión automática.
- Activación por clave única de software con hashing HMAC-SHA256.
- Registro de auditoría de todos los eventos de la plataforma.

---

## 5. DISEÑO DE API

### Convenciones Generales
- **Versioning:** Header-based `api-version: 1.0` + URI prefix `/api/v1/`
- **Formato:** JSON con estructura estándar de respuesta
- **Paginación:** Cursor-based para listados grandes, offset para paneles admin
- **Errores:** RFC 7807 Problem Details

### Estructura de Respuesta Estándar

```json
{
  "success": true,
  "data": { },
  "meta": {
    "page": 1,
    "pageSize": 20,
    "totalCount": 150,
    "requestId": "uuid-v4"
  },
  "errors": []
}
```

### Grupos de Endpoints Principales

```
POST   /api/v1/auth/login
POST   /api/v1/auth/refresh
POST   /api/v1/auth/logout

GET    /api/v1/productos
POST   /api/v1/productos
GET    /api/v1/productos/{id}
PUT    /api/v1/productos/{id}
DELETE /api/v1/productos/{id}

POST   /api/v1/ventas
GET    /api/v1/ventas/{id}
POST   /api/v1/ventas/{id}/anular

POST   /api/v1/caja/apertura
POST   /api/v1/caja/cierre
GET    /api/v1/caja/estado-actual

GET    /api/v1/inventario/movimientos
POST   /api/v1/inventario/ajuste
POST   /api/v1/inventario/traslado

GET    /api/v1/facturacion/{id}
POST   /api/v1/facturacion/emitir

GET    /api/v1/reportes/ventas-dia
GET    /api/v1/reportes/top-productos
GET    /api/v1/reportes/reporte-z/{cajaId}

# Platform (Super Admin)
GET    /api/platform/tenants
POST   /api/platform/tenants
PUT    /api/platform/tenants/{id}/suspender
GET    /api/platform/licencias
```

---

## 6. MODELO DE BASE DE DATOS — TABLAS PRINCIPALES

```sql
-- Multi-Tenant Context
[platform].[Tenants]
[platform].[Licencias]
[platform].[PlanesSubscripcion]

-- Por Tenant (schema dinámico por empresa)
[tenant].[Usuarios]
[tenant].[Roles]
[tenant].[Sucursales]
[tenant].[Cajas]
[tenant].[LogsCaja]

[tenant].[Categorias]
[tenant].[Productos]
[tenant].[LotesProducto]
[tenant].[UnidadesMedida]

[tenant].[Clientes]
[tenant].[Proveedores]

[tenant].[Ventas]
[tenant].[DetalleVentas]
[tenant].[MediosPago]

[tenant].[Facturas]
[tenant].[DetalleFacturas]
[tenant].[CodigosCabys]

[tenant].[Inventario]
[tenant].[MovimientosInventario]
[tenant].[TrasladosInventario]

[tenant].[CuentasPorCobrar]
[tenant].[PagosCxC]
[tenant].[CuentasPorPagar]
[tenant].[PagosCxP]

[tenant].[Devoluciones]
[tenant].[DetallesDevoluciones]

[tenant].[Promociones]
[tenant].[ReglasPromocion]

[tenant].[AuditoriaLog]
```

---

## 7. FASES DE TRABAJO

---

### FASE 0 — Fundación e Infraestructura (Semanas 1–2)

**Objetivo:** Levantar el esqueleto del proyecto, la solución en Azure y la pipeline de CI/CD antes de escribir una línea de lógica de negocio.

#### Backend
- [ ] Crear solución .NET 9 con la estructura Clean Architecture definida.
- [ ] Configurar EF Core con Azure SQL Database (conexión, migraciones iniciales).
- [ ] Implementar `NexusDbContext` con soporte multi-tenant (ICurrentTenantService).
- [ ] Crear entidades base: `BaseEntity`, `AuditableEntity`, `TenantEntity`.
- [ ] Implementar patrón repositorio genérico con Unit of Work.
- [ ] Configurar MediatR y pipeline de comportamientos (validación, logging, performance).
- [ ] Configurar FluentValidation global.
- [ ] Implementar `GlobalExceptionHandlerMiddleware` con Problem Details (RFC 7807).

#### Seguridad
- [ ] Configurar ASP.NET Core Identity con tablas custom en el schema `[platform]`.
- [ ] Implementar generación de JWT (access + refresh token).
- [ ] Implementar `TenantMiddleware` — resolución del tenant por JWT claim.
- [ ] Configurar integración con Azure Key Vault para secretos.
- [ ] Establecer políticas de autorización por rol.

#### Infraestructura Azure (IaC con Bicep)
- [ ] Crear Resource Group en Azure.
- [ ] Provisionar Azure SQL Database (General Purpose).
- [ ] Provisionar Azure Cache for Redis.
- [ ] Provisionar Azure Key Vault y cargar secretos iniciales.
- [ ] Provisionar Azure Blob Storage con contenedores: `facturas`, `reportes`, `imagenes`.
- [ ] Provisionar Azure App Service (staging + production slots).
- [ ] Configurar Application Insights.

#### CI/CD (Azure DevOps)
- [ ] Inicializar repositorio en Azure Repos con estrategia GitFlow.
- [ ] Crear pipeline de Build: restore → build → test → publish.
- [ ] Crear pipeline de Release a Staging: build artifact → migrations → deploy.
- [ ] Crear aprobación manual para Release a Producción.
- [ ] Integrar SonarCloud para análisis estático de código.
- [ ] Integrar publicación de reporte de cobertura (Coverlet).

**Entregables Fase 0:**
- Solución compila y pasa tests vacíos.
- La API responde en Azure Staging con endpoints de health check.
- Pipeline CI/CD completamente funcional.

---

### FASE 1 — Autenticación, Tenants y Licencias (Semanas 3–4)

**Objetivo:** El sistema puede aprovisionar empresas, activar licencias y autenticar usuarios con roles.

#### Backend
- [ ] Módulo de autenticación completo: login, refresh, logout con blacklist en Redis.
- [ ] Módulo `platform`: CRUD de Tenants, Planes de Suscripción y Licencias.
- [ ] Activación de licencia por clave única (hashing con HMAC-SHA256).
- [ ] Suspensión automática de tenants vencidos (Background Service).
- [ ] Gestión de Usuarios y Roles por tenant.
- [ ] Gestión de Sucursales por tenant.
- [ ] Gestión de Cajas por sucursal.
- [ ] Módulo de Auditoría: registro automático vía interceptor de EF Core.

#### API
- [ ] Documentar endpoints de auth y platform con Scalar/Swagger.
- [ ] Implementar versionado de API.
- [ ] Implementar rate limiting en endpoints de autenticación.

#### Frontend Web (Blazor)
- [ ] Pantalla de Login con validación y manejo de errores.
- [ ] Panel Super Admin: gestión de tenants y licencias.
- [ ] Panel inicial del tenant con datos básicos de la empresa.
- [ ] Gestión de usuarios y asignación de roles.

**Entregables Fase 1:**
- Se puede crear un tenant, activar licencia, crear usuarios y hacer login con JWT.
- El Super Admin puede administrar toda la plataforma desde la web.

---

### FASE 2 — Catálogo, Inventario y Proveedores (Semanas 5–7)

**Objetivo:** El sistema conoce los productos y puede gestionar el stock y los proveedores.

#### Backend
- [ ] CRUD de Categorías de productos.
- [ ] CRUD de Productos con variantes, imágenes en Blob Storage y código de barras.
- [ ] CRUD de Unidades de Medida.
- [ ] CRUD de Proveedores con datos fiscales y de contacto.
- [ ] Módulo de Inventario: registro de stock inicial por sucursal.
- [ ] Movimientos de inventario: entradas, salidas, ajustes y mermas.
- [ ] Traslados entre sucursales con estado (pendiente / en tránsito / recibido).
- [ ] Control de lotes y fechas de vencimiento.
- [ ] Alertas de punto de reorden (Background Service + notificación).
- [ ] Integración con API de Catálogos CABYS para clasificación de productos.

#### API
- [ ] Endpoints de productos con filtros, búsqueda full-text y paginación cursor.
- [ ] Endpoints de inventario y movimientos.
- [ ] Endpoint de búsqueda de códigos CABYS (proxy interno al catálogo).

#### Frontend Web (Blazor)
- [ ] Módulo de gestión de productos (tabla, filtros, formulario con imagen).
- [ ] Módulo de inventario: vista de stock actual por sucursal.
- [ ] Módulo de traslados con flujo de aprobación.
- [ ] Módulo de proveedores.

**Entregables Fase 2:**
- El sistema gestiona el catálogo completo de productos con stock real.

---

### FASE 3 — Ventas, Caja y Facturación (Semanas 8–12)

**Objetivo:** El corazón del negocio opera completamente. La caja vende y factura.

#### Backend
- [x] Módulo de Clientes: CRUD con límites de crédito e historial.
- [x] Módulo de Apertura/Cierre de Caja con arqueo y logs.
- [x] Motor de Ventas: registro de venta, descuentos y cálculo de impuestos.
- [x] Soporte multi-método de pago en una sola venta.
- [ ] Módulo de Devoluciones con impacto en inventario y caja.
- [ ] Motor de Facturación Electrónica:
  - [ ] Generación de XML según normativa.
  - [ ] Firma digital del comprobante.
  - [ ] Envío a la entidad fiscal y gestión de respuesta.
  - [ ] Generación de PDF y almacenamiento en Blob Storage.
- [ ] Generación de reportes X y Z en PDF.
- [ ] Módulo de Promociones y reglas de descuento automático.

#### Sincronización Offline (MAUI — API)
- [ ] Definir protocolo de sincronización: delta sync con timestamps.
- [ ] Service Bus: publicar eventos de venta desde el cliente offline.
- [ ] Background Worker en la API: consumir y procesar cola de ventas offline.
- [ ] Resolución de conflictos: política last-write-wins con log de conflictos.
- [ ] Manejo de inventario offline con reserva local y reconciliación.

#### Frontend Desktop (MAUI — POS)
- [ ] Pantalla de apertura de caja.
- [ ] Pantalla principal de venta: búsqueda de producto, carrito, total.
- [ ] Integración de escáner de código de barras.
- [ ] Pantalla de cobro con métodos de pago.
- [ ] Impresión de recibo o envío de factura digital.
- [ ] Pantalla de cierre de caja con arqueo.
- [ ] Funcionamiento completo sin conexión a internet.

#### Frontend Web (Blazor)
- [x] Historial de ventas con filtros y detalle.
- [ ] Módulo de devoluciones.
- [ ] Visualización de facturas emitidas con estado (aceptada/rechazada).

**Entregables Fase 3:**
- El POS vende, cobra, factura electrónicamente y opera offline.
- Los reportes X/Z se generan en PDF.

---

### FASE 4 — Finanzas: CxC, CxP y Compras (Semanas 13–15)

**Objetivo:** El sistema gestiona la salud financiera más allá de la venta de mostrador.

#### Backend
- [x] Módulo CxC: registro de crédito otorgado, cuotas y pagos.
- [ ] Automatización de estados de cuenta por email (Hangfire scheduler).
- [x] Alertas de vencimiento de cuentas por cobrar (UI).
- [x] Módulo CxP: registro de facturas de proveedor y programación de pagos.
- [x] Módulo de Órdenes de Compra: solicitud → aprobación → recepción.
- [x] Conciliación automática de compra recibida con movimiento de inventario.
- [x] Dashboard financiero: flujo de caja proyectado, cuentas por vencer.

#### Frontend Web (Blazor)
- [x] Módulo CxC con estado de cuenta de clientes.
- [x] Módulo CxP con calendario de pagos.
- [x] Módulo de Órdenes de Compra.

**Entregables Fase 4:**
- Control financiero completo. El negocio sabe cuánto le deben y cuánto debe.

---

### FASE 5 — Analytics, Reportes y Dashboard BI (Semanas 16–17)

**Objetivo:** El panel web se convierte en una herramienta de inteligencia de negocio.

#### Backend
- [ ] Queries optimizadas de reportes con vistas materializadas en SQL.
- [ ] Caché de reportes pesados en Redis con TTL configurable.
- [ ] Exportación de reportes a Excel (ClosedXML) y PDF (QuestPDF).
- [ ] Notificaciones push en tiempo real vía SignalR Hub.

#### Frontend Web (Blazor)
- [ ] Dashboard principal: KPIs en tiempo real con ApexCharts.
- [ ] Reporte de ventas por período, sucursal y vendedor.
- [ ] Reporte de top productos y análisis de rentabilidad.
- [ ] Reporte de inventario crítico y productos por vencer.
- [ ] Comparativa entre sucursales.
- [ ] Exportación de todos los reportes.
- [ ] Sistema de notificaciones en la barra superior.

**Entregables Fase 5:**
- El gerente tiene visibilidad total del negocio desde cualquier dispositivo.

---

### FASE 6 — Hardening, Observabilidad y Go-Live (Semanas 18–20)

**Objetivo:** El sistema está listo para producción con alta disponibilidad y monitoreo.

#### Seguridad y Compliance
- [ ] Auditoría completa de endpoints: revisión de permisos y autorización.
- [ ] Penetration testing básico (OWASP Top 10).
- [ ] Implementar Content Security Policy (CSP) headers.
- [ ] Configurar firewall en Azure SQL (IP whitelisting).
- [ ] Revisar y restringir políticas CORS a dominios de producción.
- [ ] Verificar cifrado en reposo de todos los datos sensibles.

#### Observabilidad
- [ ] Configurar Application Insights: request rates, failure rates, response times.
- [ ] Crear alertas en Azure Monitor: disponibilidad menor a 99%, error rate mayor a 1%, latencia mayor a 500ms.
- [ ] Dashboard de operaciones en Azure Portal.
- [ ] Logging estructurado con correlación de requests (correlation ID).
- [ ] Configurar retención de logs: 30 días hot, 90 días cold.

#### Performance
- [ ] Load testing con k6 o Azure Load Testing.
- [ ] Optimización de queries lentas identificadas en Application Insights.
- [ ] Configurar auto-scaling rules en Azure App Service.
- [ ] Validar que los índices de Azure SQL están correctamente definidos.

#### CI/CD Final
- [ ] Pipeline de Producción con aprobación manual de dos revisores.
- [ ] Smoke tests automáticos post-deploy en producción.
- [ ] Rollback automático si los smoke tests fallan.
- [ ] Runbook de recuperación ante desastres documentado.

**Entregables Fase 6:**
- Sistema en producción con SLA de 99.9%, monitoreo completo y plan de contingencia.

---

## 8. CI/CD — PIPELINE DETALLADO

```
Trigger: PR hacia main / Push a main

STAGE 1: BUILD & TEST
  - restore NuGet packages
  - dotnet build (--no-restore, treat-warnings-as-errors)
  - dotnet test (unit tests + architecture tests)
  - Publicar reporte de cobertura (Coverlet)
  - SonarCloud análisis estático
  - docker build (imagen de la API)
  - docker push hacia Azure Container Registry

STAGE 2: DEPLOY TO STAGING
  - Descargar imagen del ACR
  - Ejecutar EF Core migrations
  - Deploy a App Service STAGING slot
  - Smoke tests: GET /health, GET /api/v1/status
  - Integration tests contra staging

STAGE 3: PROMOTE TO PRODUCTION (aprobación manual requerida)
  - Swap de slots Staging a Production en App Service (zero-downtime)
  - Smoke tests en Production
  - Si fallan: swap automático de rollback
  - Notificación al equipo
```

---

## 9. CRONOGRAMA ESTIMADO

| Fase | Duración | Semanas | Hitos |
|------|----------|---------|-------|
| **Fase 0** — Fundación | 2 semanas | 1–2 | CI/CD activo, API en Azure |
| **Fase 1** — Auth + Tenants + Licencias | 2 semanas | 3–4 | Multi-tenant funcional |
| **Fase 2** — Catálogo + Inventario | 3 semanas | 5–7 | Gestión de productos y stock |
| **Fase 3** — Ventas + Caja + Facturación | 5 semanas | 8–12 | POS operativo con factura electrónica |
| **Fase 4** — CxC + CxP + Compras | 3 semanas | 13–15 | Control financiero completo |
| **Fase 5** — Analytics + BI | 2 semanas | 16–17 | Dashboard gerencial en vivo |
| **Fase 6** — Hardening + Go-Live | 3 semanas | 18–20 | Producción con HA garantizada |
| **Total** | **~20 semanas** | | **Sistema listo para comercializar** |

---

## 10. ESTIMACIÓN DE COSTOS AZURE (Mensual)

| Servicio | Tier | Costo Estimado USD/mes |
|----------|------|------------------------|
| Azure App Service | P2v3 (2 vCore, 7 GB) | ~$150 |
| Azure SQL Database | General Purpose 4 vCore | ~$370 |
| Azure Cache for Redis | C1 Standard | ~$55 |
| Azure Blob Storage | 100 GB LRS | ~$5 |
| Azure Service Bus | Standard | ~$10 |
| Azure Key Vault | Standard | ~$5 |
| Application Insights | Pay-as-you-go | ~$20 |
| Azure CDN | Standard | ~$10 |
| Azure DevOps | Basic (hasta 5 usuarios) | $0 |
| **Total estimado** | | **~$625/mes** |

> Estos costos pueden reducirse significativamente durante desarrollo usando tiers más bajos. Con 10 tenants pagando $100/mes cada uno, el sistema ya cubre infraestructura.

---

## 11. CONSIDERACIONES FINALES

### Riesgos Identificados

| Riesgo | Impacto | Mitigación |
|--------|---------|------------|
| Complejidad del módulo de facturación electrónica | Alto | Tratar como un sub-proyecto aislado con su propio plan |
| Sincronización offline-online con conflictos | Alto | Diseñar protocolo de resolución de conflictos desde Fase 0 |
| Migración de datos de clientes del POS anterior | Medio | Scripts ETL dedicados, no parte del desarrollo principal |
| Rendimiento bajo carga de múltiples tenants | Medio | Load testing desde la Fase 3, índices optimizados |

### Estrategia de Calidad
- Cobertura mínima de tests: **80%** para la capa de Application.
- Toda PR debe pasar SonarCloud sin nuevas issues críticas.
- Architecture tests validan automáticamente que las capas de Clean Architecture no se violen.
- Code review obligatorio de al menos 1 reviewer antes de merge a `main`.

---

*Documento generado como base de planeamiento. Diseñado para evolucionar con el proyecto.*
