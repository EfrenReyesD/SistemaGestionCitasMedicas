# Sistema de Gestión de Citas Médicas

## Descripción
API REST desarrollada en .NET 8 con C# para gestionar citas médicas, aplicando los principios de Programación Orientada a Objetos (POO).

Proyecto desarrollado para la materia de Programación Orientada a Objetos (POO) - Universidad IUV Maestría

---

## Requisitos Previos

- **.NET 8 SDK** - [Descargar aquí](https://dotnet.microsoft.com/download/dotnet/8.0)
- **Visual Studio 2022** o **VS Code**
- Navegador web moderno

---

## Instalación y Ejecución

### Opción 1: Desde Visual Studio
1. Abrir el archivo `WebApplication1.sln`
2. Presionar **F5**
3. Swagger se abrirá automáticamente

### Opción 2: Desde línea de comandos
```bash
git clone https://github.com/tu-usuario/sistema-gestion-citas-medicas.git
cd sistema-gestion-citas-medicas
dotnet restore
dotnet run
```

Luego abrir: `https://localhost:7xxx/swagger`

---

## Arquitectura del Proyecto

### Estructura de Carpetas
```
WebApplication1/
??? Controllers/
?   ??? UsuariosController.cs
?   ??? PacientesController.cs
?   ??? DoctoresController.cs
?   ??? CitasController.cs
?   ??? ReportesController.cs
?
??? Models/
?   ??? Usuario.cs (clase base)
?   ??? UsuarioManager.cs
?   ??? Paciente.cs
?   ??? Doctor.cs
?   ??? Asistente.cs
?   ??? Cita.cs
?   ??? NotaMedica.cs
?   ??? RecordatorioService.cs
?   ??? Reporte.cs
?   ??? DatosMock.cs (GUIDs fijos para pruebas)
?
??? docs/
?   ??? DiagramaClases_SistemaGestionCitasMedicas.png
?
??? Program.cs
??? appsettings.json
??? WebApplication1.csproj
??? README.md
??? GUIDS_SWAGGER.md
```

### Diagrama de Clases

![Diagrama de Clases](docs/DiagramaClases_SistemaGestionCitasMedicas.png)

---

## Principios de POO Aplicados

### 1. Herencia
- `Paciente`, `Doctor` y `Asistente` heredan de la clase base `Usuario`
- Comparten propiedades comunes: IdUsuario, Nombre, Email, PasswordHash, Rol

### 2. Encapsulamiento
- Propiedades con getters y setters
- Métodos privados como `HashPassword()` en Usuario
- Lista privada `_historial` en Paciente

### 3. Polimorfismo
- Métodos heredados pueden ser usados por cualquier tipo de Usuario

### 4. Composición
- `Cita` contiene instancias de `Paciente` y `Doctor`
- `Cita` puede tener una `NotaMedica`

### 5. Asociación
- `Doctor` se relaciona con múltiples `Cita` a través de la agenda
- `Paciente` tiene historial de `NotaMedica`

---

## Inicio Rápido

### 1. Ver datos precargados
- `GET /api/pacientes` ? 2 pacientes
- `GET /api/doctores` ? 2 doctores  
- `GET /api/citas` ? 3 citas programadas

### 2. Probar con GUIDs fijos
Ver archivo **`GUIDS_SWAGGER.md`** para copiar/pegar IDs de prueba.

**Ejemplo:**
- Paciente María: `11111111-1111-1111-1111-111111111111`
- Doctor Juan Pérez: `33333333-3333-3333-3333-333333333333`

---

## Endpoints Disponibles

Todos los endpoints incluyen documentación detallada visible en **Swagger UI**.

### Usuarios (`/api/usuarios`)
- **GET** `/api/usuarios` - Obtiene todos los usuarios del sistema
- **GET** `/api/usuarios/{id}` - Obtiene un usuario por ID
- **POST** `/api/usuarios` - Crea un nuevo usuario
- **PUT** `/api/usuarios/{id}/rol` - Asigna un rol a un usuario

### Pacientes (`/api/pacientes`)
- **GET** `/api/pacientes` - Obtiene todos los pacientes
- **GET** `/api/pacientes/{id}` - Obtiene un paciente por ID
- **POST** `/api/pacientes` - Registra un nuevo paciente
- **GET** `/api/pacientes/{id}/historial` - Obtiene el historial médico del paciente

### Doctores (`/api/doctores`)
- **GET** `/api/doctores` - Obtiene todos los doctores
- **GET** `/api/doctores/{id}` - Obtiene un doctor por ID
- **POST** `/api/doctores` - Registra un nuevo doctor
- **GET** `/api/doctores/{id}/agenda?fecha={fecha}` - Obtiene la agenda del doctor para una fecha

### Citas (`/api/citas`)
- **GET** `/api/citas` - Obtiene todas las citas
- **GET** `/api/citas/{id}` - Obtiene una cita por ID
- **POST** `/api/citas` - Programa una nueva cita
- **PUT** `/api/citas/{id}/reprogramar` - Reprograma una cita existente
- **PUT** `/api/citas/{id}/cancelar` - Cancela una cita
- **POST** `/api/citas/{id}/nota` - Agrega una nota médica a la cita

### Reportes (`/api/reportes`)
- **GET** `/api/reportes/consultas?fechaInicio={fecha}&fechaFin={fecha}` - Genera reporte de consultas
- **GET** `/api/reportes/cancelaciones?fechaInicio={fecha}&fechaFin={fecha}` - Genera reporte de cancelaciones

---

## Ejemplos de Uso

### Crear un Paciente
```json
POST /api/pacientes
{
  "nombre": "Pedro Sánchez",
  "email": "pedro.sanchez@email.com",
  "fechaNacimiento": "1985-03-20",
  "telefono": "+34 600 987 654"
}
```

### Programar una Cita
```json
POST /api/citas
{
  "fechaHora": "2025-02-01T10:00:00",
  "duracionMin": 30,
  "tipo": "Consulta General",
  "paciente": {
    "idPaciente": "11111111-1111-1111-1111-111111111111"
  },
  "doctor": {
    "idUsuario": "33333333-3333-3333-3333-333333333333"
  }
}
```

### Reprogramar una Cita
```json
PUT /api/citas/55555555-5555-5555-5555-555555555555/reprogramar
"2025-02-05T14:00:00"
```

---

## Guía de Uso en Swagger

1. **GET sin parámetros**: Clic en endpoint ? "Try it out" ? "Execute"
2. **GET con ID**: "Try it out" ? Pegar GUID ? "Execute"
3. **POST/PUT**: "Try it out" ? Pegar JSON ? "Execute"

---

## Tecnologías Utilizadas

- **.NET 8** - Framework principal
- **ASP.NET Core Web API** - Arquitectura REST
- **Swagger/OpenAPI** - Documentación interactiva
- **C# 12** - Lenguaje de programación
- **GUID** - Identificadores únicos

---

## Notas Importantes

- **Almacenamiento**: Datos en memoria (se pierden al reiniciar)
- **Datos de prueba**: Precargados automáticamente
- **GUIDs fijos**: Ver `GUIDS_SWAGGER.md`

---

## Solución de Problemas

**Error: SDK no encontrado**  
? Instalar .NET 8 SDK desde https://dotnet.microsoft.com/download

**Swagger no abre automáticamente**  
? Abrir manualmente: `https://localhost:7xxx/swagger`

---

## Información del Proyecto

- **Nombre**: SistemaGestionCitasMedicas
- **Versión**: 1.0.0
- **Autor**: Universidad IUV Maestría
- **Framework**: .NET 8.0
- **Materia**: Programación Orientada a Objetos
