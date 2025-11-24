# Sistema de Gestión de Citas Médicas

## Descripción
API REST desarrollada en .NET 8 con C# para gestionar citas médicas, aplicando principios de POO e integrada con SQL Server.

---

## Requisitos Previos

- **.NET 8 SDK**
- **SQL Server Express**
- **Visual Studio 2022** o **VS Code**

---

## Instalación y Configuración

### 1. Clonar el repositorio
```bash
git clone https://github.com/EfrenReyesD/SistemaGestionCitasMedicas.git
cd SistemaGestionCitasMedicas
```

### 2. Configurar Base de Datos

Ejecutar el script SQL ubicado en la raíz del proyecto para crear la base de datos.

**Configurar ConnectionString** en `appsettings.json`:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=TU_SERVIDOR\\SQLEXPRESS;Database=SistemaGestionCitasMedicas;Trusted_Connection=true;TrustServerCertificate=true;MultipleActiveResultSets=true;Encrypt=false"
}
```

### 3. Ejecutar
```bash
dotnet restore
dotnet run
```

Swagger disponible en: `https://localhost:7056/swagger`

---

## Arquitectura

```
Controllers → Services → ApplicationDbContext → SQL Server (con Stored Procedures)
```

**Capas:**
- **Controllers:** Endpoints API REST
- **Services:** Lógica de negocio con interfaces
- **Data:** Entity Framework Core 8
- **Database:** SQL Server con SP y Triggers

**Estructura:**
```
SistemaGestionCitasMedicas/
├── Controllers/
├── Services/
├── Data/
├── Models/
├── Validators/
└── Program.cs
```

---

## Principios de POO

- **Encapsulamiento:** Servicios con interfaces
- **Polimorfismo:** `IUsuarioService`, `IPacienteService`, `IDoctorService`, `ICitaService`
- **Inyección de Dependencias:** Todos los servicios registrados en DI
- **Composición:** `Cita` contiene `Paciente` y `Doctor`

---

## Stack Tecnológico

- **.NET 8** + **ASP.NET Core Web API**
- **Entity Framework Core 8**
- **SQL Server Express**
- **FluentValidation**
- **Swagger/OpenAPI**

**Características:**
- Stored Procedures (5 implementados)
- Triggers de validación
- Caché en memoria
- Manejo de excepciones

---

## Endpoints API

Ver documentación completa en **Swagger**: `https://localhost:7056/swagger`

**Usuarios:** `GET/POST /api/usuarios`  
**Pacientes:** `GET/POST /api/pacientes`, `GET /api/pacientes/{id}/historial`  
**Doctores:** `GET/POST /api/doctores`, `GET /api/doctores/{id}/agenda`  
**Citas:** `GET/POST /api/citas`, `PUT /api/citas/{id}/reprogramar|cancelar`  
**Reportes:** `GET /api/reportes/consultas|cancelaciones`

---

## Datos de Prueba

Datos precargados en la base de datos:

- **Paciente:** `11111111-1111-1111-1111-111111111111` (María González)
- **Doctor:** `33333333-3333-3333-3333-333333333333` (Dr. Juan Pérez)
- **Cita:** `55555555-5555-5555-5555-555555555555`

Ver más en **`GUIDS_SWAGGER.md`**

---

## Comandos

```bash
dotnet restore
dotnet build
dotnet run
dotnet publish -c Release -o ./publish
```

---

## Información

- **Versión:** 1.0.0
- **Autor:** Universidad IUV Maestría
- **Framework:** .NET 8.0
- **Repositorio:** https://github.com/EfrenReyesD/SistemaGestionCitasMedicas
